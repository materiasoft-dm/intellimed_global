using System.Net.Http.Json;
using IntelliMed.Core.DTOs;

namespace IntelliMed.Web.Services;

public class BillingItemService : IBillingItemService
{
    private readonly HttpClient _httpClient;

    public BillingItemService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<BillingItemDto>> GetAllActiveAsync() =>
        await _httpClient.GetFromJsonAsync<List<BillingItemDto>>("api/billing-items") ?? new();

    public async Task<List<BillingItemDto>> SearchAsync(string? query) =>
        await _httpClient.GetFromJsonAsync<List<BillingItemDto>>($"api/billing-items/search?query={Uri.EscapeDataString(query ?? string.Empty)}") ?? new();

    public async Task<int?> CreateAsync(CreateBillingItemDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/billing-items", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task<bool> UpdateAsync(int id, UpdateBillingItemDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/billing-items/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ArchiveAsync(int id)
    {
        var response = await _httpClient.PostAsync($"api/billing-items/{id}/archive", null);
        return response.IsSuccessStatusCode;
    }
}
