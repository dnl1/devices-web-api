using Devices.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Devices.Data.Extensions
{
    internal static class ConfigurationExtensions
    {
        internal static void ConfigureDevice(this ModelBuilder modelBuilder)
        {
            var device = modelBuilder.Entity<Device>();

            device.ToTable("devices");

            device.HasKey(d => d.Id);

            device.Property(d => d.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            device.Property(d => d.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(150);

            device.Property(d => d.Brand)
                .HasColumnName("brand")
                .IsRequired()
                .HasMaxLength(150);

            device.Property(d => d.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            device.Property(d => d.State)
                .HasColumnName("state");
        }
    }
}