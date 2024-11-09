using Admin_Dashboard.Core.Dtos.Auth;
using Admin_Dashboard.Core.Dtos.General;
using System.Security.Claims;

namespace Admin_Dashboard.Core.Interfaces
{
    public interface IAuthService
    {
        Task<GeneralServiceResponseDto> SeedRolesAsync();
        Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<LoginServiceresponseDto?> LoginAsync(LoginDto loginDto);
        Task<GeneralServiceResponseDto> UpdateRolesAsync(ClaimsPrincipal user, UpdateRoleDto updateRoleDto);
        Task<LoginServiceresponseDto?> MeAsync(MeDto meDto);
        Task<IEnumerable<UserInfoResult>> GetUserListAsync();
        Task<UserInfoResult?> GetUserDetailsByUserNameAsync(string username);
        Task<IEnumerable<string>> GetUserNamesListAsync();
    }
}
