using IntelliMed.Core.DTOs;
using IntelliMed.Core.Entities;
using IntelliMed.Core.Interfaces;
using IntelliMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntelliMed.Infrastructure.Repositories;

public class BillingItemRepository : IBillingItemRepository
{
    private readonly AppDbContext _context;

    public BillingItemRepository(AppDbContext context)
    {
        _context = context;
    }

    private static BillingItemDto ToDto(BillingItem e) => new()
    {
        Id = e.Id,
        Code = e.Code,
        Description = e.Description,
        Fee = e.Fee,
        IsActive = e.IsActive
    };

    public async Task<List<BillingItemDto>> GetAllActiveAsync() =>
        await _context.BillingItems
            .Where(b => b.IsActive)
            .OrderBy(b => b.Code)
            .Select(b => ToDto(b))
            .ToListAsync();

    public async Task<List<BillingItemDto>> SearchAsync(string? query)
    {
        var items = _context.BillingItems.Where(b => b.IsActive).AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.ToLower();
            items = items.Where(b => b.Code.ToLower().Contains(term) || b.Description.ToLower().Contains(term));
        }
        return await items.OrderBy(b => b.Code).Take(50).Select(b => ToDto(b)).ToListAsync();
    }

    public async Task<BillingItemDto?> GetByIdAsync(int id)
    {
        var item = await _context.BillingItems.FindAsync(id);
        return item == null ? null : ToDto(item);
    }

    public async Task<int> CreateAsync(CreateBillingItemDto dto)
    {
        var item = new BillingItem
        {
            Code = dto.Code,
            Description = dto.Description,
            Fee = dto.Fee,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _context.BillingItems.AddAsync(item);
        await _context.SaveChangesAsync();
        return item.Id;
    }

    public async Task UpdateAsync(int id, UpdateBillingItemDto dto)
    {
        var item = await _context.BillingItems.FindAsync(id);
        if (item == null) throw new InvalidOperationException($"Billing item with ID {id} not found");

        item.Code = dto.Code;
        item.Description = dto.Description;
        item.Fee = dto.Fee;
        item.IsActive = dto.IsActive;
        item.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task ArchiveAsync(int id)
    {
        var item = await _context.BillingItems.FindAsync(id);
        if (item == null) throw new InvalidOperationException($"Billing item with ID {id} not found");

        item.IsActive = false;
        item.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
