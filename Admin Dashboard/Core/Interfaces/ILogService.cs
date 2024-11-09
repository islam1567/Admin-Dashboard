using Admin_Dashboard.Core.Dtos.Log;
using System.Security.Claims;

namespace Admin_Dashboard.Core.Interfaces
{
    public interface ILogService
    {
        Task SaveNewLog(string username, string descreption);
        Task<IEnumerable<GetLogDto>> GetLogAsync();
        Task<IEnumerable<GetLogDto>> GetMyLogAsync(ClaimsPrincipal user);

    }
}
