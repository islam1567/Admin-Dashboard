using Admin_Dashboard.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard.Core.DbContext
{
    public class ApplecationDbContext : IdentityDbContext<ApplecationUser>
    {
        public ApplecationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Log> Logs { get; set; }
        public DbSet<Message> Messages { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    builder.Entity<ApplecationUser>(e =>
        //    {
        //        e.ToTable("Userss");
        //    });
        //}
    }
}
