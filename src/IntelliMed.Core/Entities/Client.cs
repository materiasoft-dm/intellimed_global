namespace IntelliMed.Core.Entities;

public class Client
{
    public int Id { get; set; }

    public int ClinicId { get; set; }
    public Clinic? Clinic { get; set; }

    public ClientTypeEnum Type { get; set; } = ClientTypeEnum.Person;

    // Personal
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? PreferredName { get; set; }
    public string? MaidenName { get; set; }
    public string? Title { get; set; }
    public GenderEnum? Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DobAccuracyEnum DobAccuracy { get; set; } = DobAccuracyEnum.Day;
    public string? PlaceOfBirth { get; set; }
    public bool InterpreterRequired { get; set; }
    public string? InterpreterLanguage { get; set; }
    public MaritalStatusEnum? MaritalStatus { get; set; }
    public string? Ethnicity { get; set; }

    // Residential address (Postal/Other addresses live in ClientAddress)
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;

    // Contact Details
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? BusinessHoursPhone { get; set; }
    public string? MobilePhone { get; set; }
    public string? FaxNumber { get; set; }
    public bool AcceptSms { get; set; }
    public bool AcceptEmail { get; set; }
    public bool AcceptOnlineAppointments { get; set; }
    public bool AcceptSmsMarketing { get; set; }
    public string? Notes { get; set; }
    public string? Warnings { get; set; }
    public int? NextOfKinClientId { get; set; }
    public string? NextOfKinName { get; set; }
    public string? NextOfKinPhone { get; set; }
    public int? EmergencyContactClientId { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public bool SameAsNextOfKin { get; set; }

    // File
    public string? FileNumber { get; set; }
    public string? UrNumber { get; set; }
    public bool Deceased { get; set; }
    public int? ProviderId { get; set; }
    public DateTime? LastSeenDate { get; set; }

    // Lifecard
    public string? LifeCardNum { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Client? NextOfKinClient { get; set; }
    public Client? EmergencyContactClient { get; set; }
    public Practitioner? Provider { get; set; }
}
