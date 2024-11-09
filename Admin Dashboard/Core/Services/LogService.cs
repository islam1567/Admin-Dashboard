using Admin_Dashboard.Core.DbContext;
using Admin_Dashboard.Core.Dtos.Log;
using Admin_Dashboard.Core.Entities;
using Admin_Dashboard.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Security.Claims;

namespace Admin_Dashboard.Core.Services
{
    public class LogService : ILogService
    {
        private readonly ApplecationDbContext context;

        public LogService(ApplecationDbContext context)
        {
            this.context = context;
        }

        public async Task SaveNewLog(string username, string descreption)
        {
            var newlog = new Log()
            {
                UserName = username,
                Descreption = descreption,
            };

            await context.Logs.AddAsync(newlog);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GetLogDto>> GetLogAsync()
        {
            var log = await context.Logs
                .Select(e => new GetLogDto
                {
                    CreateAt = e.CreateAt,
                    Descreption = e.Descreption,
                    UserName = e.UserName
                })
                .OrderByDescending(e => e.CreateAt)
                .ToListAsync();

            return log;
                
        }

        public async Task<IEnumerable<GetLogDto>> GetMyLogAsync(ClaimsPrincipal user)
        {
            var log = await context.Logs
                .Where(e => e.UserName == user.Identity.Name)
                .Select(e => new GetLogDto
                {
                    CreateAt = e.CreateAt,
                    Descreption = e.Descreption,
                    UserName = e.UserName
                })
                .OrderByDescending(e => e.CreateAt)
                .ToListAsync();

            return log;
        }

        
    }
}
