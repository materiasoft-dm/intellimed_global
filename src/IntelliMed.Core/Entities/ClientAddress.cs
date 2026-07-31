namespace IntelliMed.Core.Entities;

public enum ClientAddressType
{
    Postal,
    Other
}

public class ClientAddress
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public ClientAddressType AddressType { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string AddressSubType { get; set; } = "Not Specified";
    public string? Community { get; set; }

    public Client? Client { get; set; }
}
