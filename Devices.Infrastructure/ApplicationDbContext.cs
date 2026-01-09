using Devices.Infrastructure.Extensions;
using Devices.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Devices.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Device> Devices => Set<Device>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureDevice();

            base.OnModelCreating(modelBuilder);
        }
    }
}