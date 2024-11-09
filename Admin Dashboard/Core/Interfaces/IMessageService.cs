using Admin_Dashboard.Core.Dtos.General;
using Admin_Dashboard.Core.Dtos.Message;
using System.Security.Claims;

namespace Admin_Dashboard.Core.Interfaces
{
    public interface IMessageService
    {
        Task<GeneralServiceResponseDto> CreateNewMessageAsync(ClaimsPrincipal user, CreateMessageDto createMessageDto);
        Task<IEnumerable<GetMessageDto>> GetMessageAsync();
        Task<IEnumerable<GetMessageDto>> GetMyMessageAsync(ClaimsPrincipal user);
    }
}
