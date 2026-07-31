using System.Net.Http.Json;
using IntelliMed.Core.DTOs;

namespace IntelliMed.Web.Services;

public class ClientService : IClientService
{
    private readonly HttpClient _httpClient;

    public ClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResult<ClientDto>> SearchClientsAsync(ClientSearchDto search)
    {
        var args = new List<string>
        {
            $"page={search.Page}",
            $"pageSize={search.PageSize}",
            $"includeArchived={search.IncludeArchived}"
        };

        void AddIfSet(string key, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                args.Add($"{key}={Uri.EscapeDataString(value)}");
        }

        void AddDate(string key, DateTime? value)
        {
            if (value.HasValue)
                args.Add($"{key}={value.Value:yyyy-MM-dd}");
        }

        void AddBool(string key, bool? value)
        {
            if (value.HasValue)
                args.Add($"{key}={value.Value}");
        }

        AddIfSet("query", search.Query);
        if (search.IsActive.HasValue) args.Add($"isActive={search.IsActive.Value}");

        AddIfSet("surname", search.Surname);
        AddIfSet("givenName", search.GivenName);
        if (search.Gender.HasValue) args.Add($"gender={search.Gender.Value}");
        AddIfSet("fileNumber", search.FileNumber);
        AddIfSet("lifeCardNum", search.LifeCardNum);
        AddDate("dobFrom", search.DobFrom);
        AddDate("dobTo", search.DobTo);

        AddIfSet("address", search.Address);
        AddIfSet("city", search.City);
        AddIfSet("postalCode", search.PostalCode);
        AddIfSet("state", search.State);

        AddIfSet("postalAddress", search.PostalAddress);
        AddIfSet("postalCity", search.PostalCity);
        AddIfSet("postalPostalCode", search.PostalPostalCode);
        AddIfSet("postalState", search.PostalState);

        AddIfSet("homePhone", search.HomePhone);
        AddIfSet("businessHoursPhone", search.BusinessHoursPhone);
        AddIfSet("mobilePhone", search.MobilePhone);
        AddIfSet("email", search.Email);

        AddDate("createdFrom", search.CreatedFrom);
        AddDate("createdTo", search.CreatedTo);

        AddIfSet("warnings", search.Warnings);
        AddIfSet("notes", search.Notes);
        AddIfSet("referredBy", search.ReferredBy);
        if (search.ClientType.HasValue) args.Add($"clientType={search.ClientType.Value}");
        AddIfSet("urNumber", search.UrNumber);

        if (search.Deceased.HasValue) args.Add($"deceased={search.Deceased.Value}");
        AddBool("acceptEmail", search.AcceptEmail);
        AddBool("acceptSms", search.AcceptSms);
        AddBool("acceptSmsMarketing", search.AcceptSmsMarketing);

        var uri = "api/clients/search?" + string.Join("&", args);
        return await _httpClient.GetFromJsonAsync<PagedResult<ClientDto>>(uri)
            ?? new PagedResult<ClientDto>();
    }

    private record CreateResult(int Id);

    public async Task<int?> CreateClientAsync(CreateClientDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/clients", dto);
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<CreateResult>();
            return result?.Id;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Create client error: {ex.Message}");
            return null;
        }
    }

    public async Task<ClientDto?> GetClientByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/clients/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<ClientDto>();
    }

    public async Task<bool> UpdateClientAsync(int id, UpdateClientDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/clients/{id}", dto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Update client error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ArchiveClientAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/clients/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Archive client error: {ex.Message}");
            return false;
        }
    }
}
