using Admin_Dashboard.Core.DbContext;
using Admin_Dashboard.Core.Dtos.General;
using Admin_Dashboard.Core.Dtos.Message;
using Admin_Dashboard.Core.Entities;
using Admin_Dashboard.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Admin_Dashboard.Core.Services
{
    public class MessageService : IMessageService
    {

        private readonly ApplecationDbContext context;
        private readonly ILogService logservice;
        private readonly UserManager<ApplecationUser> usermanager;

        public MessageService(ApplecationDbContext context, ILogService logservice, UserManager<ApplecationUser> usermanager)
        {
            this.context = context;
            this.logservice = logservice;
            this.usermanager = usermanager;
        }

        public async Task<GeneralServiceResponseDto> CreateNewMessageAsync(ClaimsPrincipal user, CreateMessageDto createMessageDto)
        {
            if(user.Identity.Name == createMessageDto.RecevirUserName)
            {
                return new GeneralServiceResponseDto()
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Sender and Recevir can not be same",
                };
            }

            var isRecevirusernamevalid = usermanager.Users.Any(e => e.UserName == createMessageDto.RecevirUserName);
            if(!isRecevirusernamevalid)
            {
                return new GeneralServiceResponseDto()
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Recevir UserName is not valid",
                };
            }

            Message newmessage = new Message()
            {
                SendUserName=user.Identity.Name,
                ReceverUserName=createMessageDto.RecevirUserName,
                Text= createMessageDto.Text
            };

            await context.Messages.AddAsync(newmessage);
            await context.SaveChangesAsync();
            await logservice.SaveNewLog(user.Identity.Name, "Send Message");

            return new GeneralServiceResponseDto()
            {
                IsSuccess = true,
                StatusCode = 201,
                Message = "Message saved successfully",
            };
        }

        public async Task<IEnumerable<GetMessageDto>> GetMessageAsync()
        {
            var messages = await context.Messages
                .Select(e => new GetMessageDto()
                {
                    Id = e.Id,
                    SenderUserName = e.SendUserName,
                    RecevirUserName = e.ReceverUserName,
                    Text = e.Text,
                    CreateAt = e.CreateAt,
                })
                .OrderByDescending(e => e.CreateAt)
                .ToListAsync();

            return messages;
        }

        public async Task<IEnumerable<GetMessageDto>> GetMyMessageAsync(ClaimsPrincipal user)
        {
            var messages = await context.Messages
                .Where(e => e.SendUserName == user.Identity.Name || e.ReceverUserName == user.Identity.Name)
                .Select(e => new GetMessageDto()
                {
                    Id = e.Id,
                    SenderUserName = e.SendUserName,
                    RecevirUserName = e.ReceverUserName,
                    Text = e.Text,
                    CreateAt = e.CreateAt,
                })
                .OrderByDescending(e => e.CreateAt)
                .ToListAsync();

            return messages;
        }
    }
}
