using Admin_Dashboard.Core.Contents;
using Admin_Dashboard.Core.DbContext;
using Admin_Dashboard.Core.Dtos.Auth;
using Admin_Dashboard.Core.Dtos.General;
using Admin_Dashboard.Core.Entities;
using Admin_Dashboard.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Admin_Dashboard.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplecationUser> usermanager;
        private readonly RoleManager<IdentityRole> rolemanager;
        private readonly ILogService logService;
        private readonly IConfiguration configuration;

        public AuthService(UserManager<ApplecationUser> usermanager, RoleManager<IdentityRole> rolemanager, ILogService logService, IConfiguration configuration)
        {
            this.usermanager = usermanager;
            this.rolemanager = rolemanager;
            this.logService = logService;
            this.configuration = configuration;
        }

        public async Task<GeneralServiceResponseDto> SeedRolesAsync()
        {
            bool isOwnerRoleExist = await rolemanager.RoleExistsAsync(StaticUserRole.OWNER);
            bool isAdminRoleExist = await rolemanager.RoleExistsAsync(StaticUserRole.ADMIN);
            bool isManagerRoleExist = await rolemanager.RoleExistsAsync(StaticUserRole.MANAGER);
            bool isUserRoleExist = await rolemanager.RoleExistsAsync(StaticUserRole.USER);

            if(isOwnerRoleExist && isAdminRoleExist && isManagerRoleExist && isUserRoleExist)
                return new GeneralServiceResponseDto()
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message="Roles Seeding is Already Done"
                };

            await rolemanager.CreateAsync(new IdentityRole(StaticUserRole.OWNER));
            await rolemanager.CreateAsync(new IdentityRole(StaticUserRole.ADMIN));
            await rolemanager.CreateAsync(new IdentityRole(StaticUserRole.MANAGER));
            await rolemanager.CreateAsync(new IdentityRole(StaticUserRole.USER));

            return new GeneralServiceResponseDto()
            {
                IsSuccess = true,
                StatusCode = 201,
                Message = "Roles Seeding is Done successfully"
            };
        }

        public async Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var isExistsUser = await usermanager.FindByNameAsync(registerDto.UserName);
            if (isExistsUser is not null)
            {
                return new GeneralServiceResponseDto()
                {
                    IsSuccess = false,
                    StatusCode = 409,
                    Message = "User Already Exists"
                };
            }

            ApplecationUser newuser = new ApplecationUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                Address = registerDto.Address,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var CreateUserResult = await usermanager.CreateAsync(newuser, registerDto.Password);

            if (!CreateUserResult.Succeeded)
            {
                var error = "User Creation Field Because";
                foreach (var item in CreateUserResult.Errors)
                {
                    error += "#" + item.Description;
                }
                return new GeneralServiceResponseDto()
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = error
                };
            }

            await usermanager.AddToRoleAsync(newuser, StaticUserRole.USER);
            await logService.SaveNewLog(newuser.UserName, "Register to Webside");

            return new GeneralServiceResponseDto()
            {
                IsSuccess = true,
                StatusCode = 201,
                Message = "User Created Successfully"
            };
        }

        public async Task<LoginServiceresponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await usermanager.FindByNameAsync(loginDto.UserName);
            if (user is null)
                return null;

            var passwordcorrect = await usermanager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordcorrect)
                return null;

            var newToken = await GenerateJWTTokenAsync(user);
            var roles = await usermanager.GetRolesAsync(user);
            var userinfo = GenerateUserInfoObject(user, roles);
            await logService.SaveNewLog(user.UserName, "New Login");

            return new LoginServiceresponseDto()
            {
                NewToken = newToken,
                UserInfo = userinfo
            };

        }

        public async Task<GeneralServiceResponseDto> UpdateRolesAsync(ClaimsPrincipal User, UpdateRoleDto updateRoleDto)
        {
            var user = await usermanager.FindByNameAsync(updateRoleDto.UserName);
            if (user is null)
                return new GeneralServiceResponseDto()
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Invalid UserName"
                };

            var userRoles = await usermanager.GetRolesAsync(user);
            if (User.IsInRole(StaticUserRole.ADMIN))
            {
                if (updateRoleDto.NewRole == RoleType.MANAGER || updateRoleDto.NewRole == RoleType.USER)
                {
                    if (userRoles.Any(e => e.Equals(StaticUserRole.OWNER) || e.Equals(StaticUserRole.ADMIN)))
                    {
                        return new GeneralServiceResponseDto()
                        {
                            IsSuccess = false,
                            StatusCode = 403,
                            Message = "You are not allowed to change role of this user"
                        };
                    }
                    else
                    {
                        await usermanager.RemoveFromRolesAsync(user, userRoles);
                        await usermanager.AddToRoleAsync(user, updateRoleDto.NewRole.ToString());
                        await logService.SaveNewLog(user.UserName, "User Role Updated");
                        return new GeneralServiceResponseDto()
                        {
                            IsSuccess = true,
                            StatusCode = 200,
                            Message = "Role Updated Succesfullt"
                        };
                    }
                }
                else return new GeneralServiceResponseDto()
                {
                    IsSuccess = false,
                    StatusCode = 403,
                    Message = "You are not allowed to change role of this user"
                };
            }
            else
            {
                if (userRoles.Any(e => e.Equals(StaticUserRole.OWNER)))
                {
                    return new GeneralServiceResponseDto()
                    {
                        IsSuccess = false,
                        StatusCode = 403,
                        Message = "You are not allowed to change role of this user"
                    };
                }
                else
                {
                    await usermanager.RemoveFromRolesAsync(user, userRoles);
                    await usermanager.AddToRoleAsync(user, updateRoleDto.NewRole.ToString());
                    await logService.SaveNewLog(user.UserName, "User Role Updated");
                    return new GeneralServiceResponseDto()
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = "Role Updated Succesfullt"
                    };
                }
            }
        }

        public async Task<LoginServiceresponseDto?> MeAsync(MeDto meDto)
        {
            ClaimsPrincipal handler = new JwtSecurityTokenHandler().ValidateToken(meDto.Token, new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = "",
                ValidAudience = "",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("HFFjjddGDEEppfppecnnBB"))
            }, out SecurityToken securityToken);

            string decodedUsername = handler.Claims.First(e => e.Type == ClaimTypes.Name).Value;
            if (decodedUsername is null)
                return null;

            var user = await usermanager.FindByNameAsync(decodedUsername);
            if (user is null)
                return null;

            var newToken = await GenerateJWTTokenAsync(user);
            var roles = await usermanager.GetRolesAsync(user);
            var userinfo = GenerateUserInfoObject(user, roles);
            await logService.SaveNewLog(user.UserName, "New Token Generated");

            return new LoginServiceresponseDto()
            {
                NewToken = newToken,
                UserInfo = userinfo
            };
        }

        public async Task<IEnumerable<UserInfoResult>> GetUserListAsync()
        {
            var users = await usermanager.Users.ToListAsync();
            List<UserInfoResult> userInfoResults = new List<UserInfoResult>();
            foreach (var user in users)
            {
                var Roles = await usermanager.GetRolesAsync(user);
                var userinfo = GenerateUserInfoObject(user, Roles);
                userInfoResults.Add(userinfo);
            }

            return userInfoResults;
        }

        public async Task<UserInfoResult?> GetUserDetailsByUserNameAsync(string username)
        {
            var user = await usermanager.FindByNameAsync(username);
            if (user is null)
                return null;

            var Roles = await usermanager.GetRolesAsync(user);
            var userinfo = GenerateUserInfoObject(user, Roles); 

            return userinfo;
        }      

        public async Task<IEnumerable<string>> GetUserNamesListAsync()
        {
            var userNames = await usermanager.Users
                .Select(e => e.UserName)
                .ToListAsync();

            return userNames;
        }
      

        private async Task<string> GenerateJWTTokenAsync(ApplecationUser user)
        {
            var userRoles = await usermanager.GetRolesAsync(user);

            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("HGFhjdjdjkfdkhflshfslRED"));
            var signingcredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);

            var tokenObject = new JwtSecurityToken(
                issuer: "",
                audience: "",
                expires: DateTime.Now.AddHours(3),
                claims:authClaims,
                signingCredentials: signingcredentials
                );

            string token= new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }

        private UserInfoResult GenerateUserInfoObject(ApplecationUser user, IEnumerable<string> Roles)
        {
            return new UserInfoResult()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LasrName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                CreateAt = user.CreateAt,
                Roles = user.Roles
            };
        }
    }
}
