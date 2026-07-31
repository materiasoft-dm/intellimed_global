using IntelliMed.Core.DTOs;

namespace IntelliMed.Core.Interfaces;

public interface IFeeScheduleRepository
{
    Task<List<FeeScheduleDto>> GetAllActiveAsync();
    Task<FeeScheduleDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateFeeScheduleDto dto);
    Task UpdateAsync(int id, UpdateFeeScheduleDto dto);
    Task ArchiveAsync(int id);

    Task<List<FeeScheduleItemDto>> GetItemsAsync(int feeScheduleId);
    Task SaveItemAsync(int feeScheduleId, SaveFeeScheduleItemDto dto);
    Task RemoveItemAsync(int feeScheduleId, int billingItemId);

    /// <summary>Plain price lookup for the invoice line-item picker — the fee schedule's override if one exists, else the billing item's own base Fee. No calculation.</summary>
    Task<ResolveLineResult?> ResolveLineAsync(ResolveLineRequest request);
}
