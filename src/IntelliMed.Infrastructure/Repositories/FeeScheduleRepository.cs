using IntelliMed.Core.DTOs;
using IntelliMed.Core.Entities;
using IntelliMed.Core.Interfaces;
using IntelliMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntelliMed.Infrastructure.Repositories;

public class FeeScheduleRepository : IFeeScheduleRepository
{
    private readonly AppDbContext _context;

    public FeeScheduleRepository(AppDbContext context)
    {
        _context = context;
    }

    private static FeeScheduleDto ToDto(FeeSchedule e) => new()
    {
        Id = e.Id,
        Code = e.Code,
        Description = e.Description,
        Note = e.Note,
        IsArchived = e.IsArchived,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    public async Task<List<FeeScheduleDto>> GetAllActiveAsync() =>
        await _context.FeeSchedules
            .Where(f => !f.IsArchived)
            .OrderBy(f => f.Code)
            .Select(f => ToDto(f))
            .ToListAsync();

    public async Task<FeeScheduleDto?> GetByIdAsync(int id)
    {
        var schedule = await _context.FeeSchedules.FindAsync(id);
        return schedule == null ? null : ToDto(schedule);
    }

    public async Task<int> CreateAsync(CreateFeeScheduleDto dto)
    {
        var schedule = new FeeSchedule
        {
            Code = dto.Code,
            Description = dto.Description,
            Note = dto.Note,
            CreatedAt = DateTime.UtcNow
        };
        await _context.FeeSchedules.AddAsync(schedule);
        await _context.SaveChangesAsync();
        return schedule.Id;
    }

    public async Task UpdateAsync(int id, UpdateFeeScheduleDto dto)
    {
        var schedule = await _context.FeeSchedules.FindAsync(id);
        if (schedule == null) throw new InvalidOperationException($"Fee schedule with ID {id} not found");

        schedule.Code = dto.Code;
        schedule.Description = dto.Description;
        schedule.Note = dto.Note;
        schedule.IsArchived = dto.IsArchived;
        schedule.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task ArchiveAsync(int id)
    {
        var schedule = await _context.FeeSchedules.FindAsync(id);
        if (schedule == null) throw new InvalidOperationException($"Fee schedule with ID {id} not found");

        schedule.IsArchived = true;
        schedule.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<List<FeeScheduleItemDto>> GetItemsAsync(int feeScheduleId) =>
        await _context.FeeScheduleItems
            .Include(i => i.BillingItem)
            .Where(i => i.FeeScheduleId == feeScheduleId)
            .OrderBy(i => i.BillingItem!.Code)
            .Select(i => new FeeScheduleItemDto
            {
                Id = i.Id,
                FeeScheduleId = i.FeeScheduleId,
                BillingItemId = i.BillingItemId,
                Code = i.BillingItem!.Code,
                Description = i.BillingItem.Description,
                Fee = i.Fee
            })
            .ToListAsync();

    public async Task SaveItemAsync(int feeScheduleId, SaveFeeScheduleItemDto dto)
    {
        var existing = await _context.FeeScheduleItems
            .FirstOrDefaultAsync(i => i.FeeScheduleId == feeScheduleId && i.BillingItemId == dto.BillingItemId);

        if (existing != null)
        {
            existing.Fee = dto.Fee;
        }
        else
        {
            await _context.FeeScheduleItems.AddAsync(new FeeScheduleItem
            {
                FeeScheduleId = feeScheduleId,
                BillingItemId = dto.BillingItemId,
                Fee = dto.Fee
            });
        }
        await _context.SaveChangesAsync();
    }

    public async Task RemoveItemAsync(int feeScheduleId, int billingItemId)
    {
        var existing = await _context.FeeScheduleItems
            .FirstOrDefaultAsync(i => i.FeeScheduleId == feeScheduleId && i.BillingItemId == billingItemId);
        if (existing != null)
        {
            _context.FeeScheduleItems.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<ResolveLineResult?> ResolveLineAsync(ResolveLineRequest request)
    {
        var billingItem = await _context.BillingItems.FindAsync(request.BillingItemId);
        if (billingItem == null) return null;

        var fee = billingItem.Fee;
        if (request.FeeScheduleId.HasValue)
        {
            var scheduleItem = await _context.FeeScheduleItems
                .FirstOrDefaultAsync(i => i.FeeScheduleId == request.FeeScheduleId.Value && i.BillingItemId == request.BillingItemId);
            if (scheduleItem != null) fee = scheduleItem.Fee;
        }

        return new ResolveLineResult { Fee = fee, Description = billingItem.Description };
    }
}
