using IntelliMed.Core.DTOs;

namespace IntelliMed.Web.Services;

public interface IFeeScheduleService
{
    Task<List<FeeScheduleDto>> GetAllActiveAsync();
    Task<FeeScheduleDto?> GetByIdAsync(int id);
    Task<int?> CreateAsync(CreateFeeScheduleDto dto);
    Task<bool> UpdateAsync(int id, UpdateFeeScheduleDto dto);
    Task<bool> ArchiveAsync(int id);
    Task<List<FeeScheduleItemDto>> GetItemsAsync(int feeScheduleId);
    Task<bool> SaveItemAsync(int feeScheduleId, SaveFeeScheduleItemDto dto);
    Task<bool> RemoveItemAsync(int feeScheduleId, int billingItemId);
    Task<ResolveLineResult?> ResolveLineAsync(ResolveLineRequest request);
}
