using IntelliMed.Core.DTOs;

namespace IntelliMed.Core.Interfaces;

public interface IBillingItemRepository
{
    Task<List<BillingItemDto>> GetAllActiveAsync();
    Task<List<BillingItemDto>> SearchAsync(string? query);
    Task<BillingItemDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateBillingItemDto dto);
    Task UpdateAsync(int id, UpdateBillingItemDto dto);
    Task ArchiveAsync(int id);
}
