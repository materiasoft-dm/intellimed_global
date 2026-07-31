using IntelliMed.Core.DTOs;

namespace IntelliMed.Web.Services;

public interface IBillingItemService
{
    Task<List<BillingItemDto>> GetAllActiveAsync();
    Task<List<BillingItemDto>> SearchAsync(string? query);
    Task<int?> CreateAsync(CreateBillingItemDto dto);
    Task<bool> UpdateAsync(int id, UpdateBillingItemDto dto);
    Task<bool> ArchiveAsync(int id);
}
