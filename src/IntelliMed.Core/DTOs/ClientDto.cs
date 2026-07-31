using IntelliMed.Core.Entities;

namespace IntelliMed.Core.DTOs;

public class ClientDto
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public ClientTypeEnum Type { get; set; }

    // Personal
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? MiddleName { get; set; }
    public string? PreferredName { get; set; }
    public string? MaidenName { get; set; }
    public string? Title { get; set; }
    public GenderEnum? Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Age => CalculateAge(DateOfBirth);
    public DobAccuracyEnum DobAccuracy { get; set; }
    public string? PlaceOfBirth { get; set; }
    public bool InterpreterRequired { get; set; }
    public string? InterpreterLanguage { get; set; }
    public MaritalStatusEnum? MaritalStatus { get; set; }
    public string? Ethnicity { get; set; }

    // Residential address
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string FullAddress => string.IsNullOrWhiteSpace(Address)
        ? string.Empty
        : $"{Address}, {City} {State} {PostalCode}";

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
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}

public class CreateClientDto
{
    /// <summary>Set server-side from the caller's current clinic context (X-Clinic-Id header), not client-supplied.</summary>
    public int ClinicId { get; set; }

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

    // Residential address
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
}

public class UpdateClientDto
{
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
    public DobAccuracyEnum DobAccuracy { get; set; }
    public string? PlaceOfBirth { get; set; }
    public bool InterpreterRequired { get; set; }
    public string? InterpreterLanguage { get; set; }
    public MaritalStatusEnum? MaritalStatus { get; set; }
    public string? Ethnicity { get; set; }

    // Residential address
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
}

public class ClientSearchDto
{
    public string? Query { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    /// <summary>Set server-side from the caller's current clinic context (X-Clinic-Id header). Null = no clinic filtering.</summary>
    public int? ClinicId { get; set; }

    // Basic
    public string? Surname { get; set; }
    public string? GivenName { get; set; }
    public GenderEnum? Gender { get; set; }
    public string? FileNumber { get; set; }
    public string? LifeCardNum { get; set; }
    public DateTime? DobFrom { get; set; }
    public DateTime? DobTo { get; set; }

    // Residential address
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? State { get; set; }

    // Postal address (matched against ClientAddress rows of type Postal)
    public string? PostalAddress { get; set; }
    public string? PostalCity { get; set; }
    public string? PostalPostalCode { get; set; }
    public string? PostalState { get; set; }

    // Contact
    public string? HomePhone { get; set; }
    public string? BusinessHoursPhone { get; set; }
    public string? MobilePhone { get; set; }
    public string? Email { get; set; }

    // Date ranges
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    // Misc
    public string? Warnings { get; set; }
    public string? Notes { get; set; }
    public string? ReferredBy { get; set; }
    public ClientTypeEnum? ClientType { get; set; }
    public string? UrNumber { get; set; }

    // Flags
    public bool? Deceased { get; set; }
    public bool IncludeArchived { get; set; }
    public bool? AcceptEmail { get; set; }
    public bool? AcceptSms { get; set; }
    public bool? AcceptSmsMarketing { get; set; }
}
