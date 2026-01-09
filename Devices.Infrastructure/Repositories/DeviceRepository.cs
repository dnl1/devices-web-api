// Devices.Data/Repositories/DeviceRepository.cs
using Devices.Domain.Entities;
using Devices.Domain.Enums;
using Devices.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Devices.Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly ApplicationDbContext _context;

    public DeviceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Device?> GetByIdAsync(int id)
    {
        return await _context.Devices.FindAsync(id);
    }

    public async Task<IEnumerable<Device>> GetAllAsync()
    {
        return await _context.Devices.ToListAsync();
    }

    public async Task<IEnumerable<Device>> GetByBrandAsync(string brand)
    {
        return await _context.Devices
            .Where(d => d.Brand.ToLower() == brand.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Device>> GetByStateAsync(DeviceState state)
    {
        return await _context.Devices
            .Where(d => d.State == state)
            .ToListAsync();
    }

    public async Task<Device> AddAsync(Device device)
    {
        _context.Devices.Add(device);
        await _context.SaveChangesAsync();
        return device;
    }

    public async Task<Device> UpdateAsync(Device device)
    {
        _context.Entry(device).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return device;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var device = await GetByIdAsync(id);
        if (device == null) return false;

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Devices.AnyAsync(d => d.Id == id);
    }
}