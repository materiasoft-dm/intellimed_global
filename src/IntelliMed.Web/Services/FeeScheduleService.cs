using System.Net.Http.Json;
using IntelliMed.Core.DTOs;

namespace IntelliMed.Web.Services;

public class FeeScheduleService : IFeeScheduleService
{
    private readonly HttpClient _httpClient;

    public FeeScheduleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<FeeScheduleDto>> GetAllActiveAsync() =>
        await _httpClient.GetFromJsonAsync<List<FeeScheduleDto>>("api/fee-schedules") ?? new();

    public async Task<FeeScheduleDto?> GetByIdAsync(int id) =>
        await _httpClient.GetFromJsonAsync<FeeScheduleDto>($"api/fee-schedules/{id}");

    public async Task<int?> CreateAsync(CreateFeeScheduleDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/fee-schedules", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task<bool> UpdateAsync(int id, UpdateFeeScheduleDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/fee-schedules/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ArchiveAsync(int id)
    {
        var response = await _httpClient.PostAsync($"api/fee-schedules/{id}/archive", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<FeeScheduleItemDto>> GetItemsAsync(int feeScheduleId) =>
        await _httpClient.GetFromJsonAsync<List<FeeScheduleItemDto>>($"api/fee-schedules/{feeScheduleId}/items") ?? new();

    public async Task<bool> SaveItemAsync(int feeScheduleId, SaveFeeScheduleItemDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/fee-schedules/{feeScheduleId}/items", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveItemAsync(int feeScheduleId, int billingItemId)
    {
        var response = await _httpClient.DeleteAsync($"api/fee-schedules/{feeScheduleId}/items/{billingItemId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<ResolveLineResult?> ResolveLineAsync(ResolveLineRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/fee-schedules/resolve-line", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<ResolveLineResult>();
    }
}
