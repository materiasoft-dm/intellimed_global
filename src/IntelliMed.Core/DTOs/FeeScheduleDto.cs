namespace IntelliMed.Core.DTOs;

public class FeeScheduleDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateFeeScheduleDto
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
}

public class UpdateFeeScheduleDto
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
    public bool IsArchived { get; set; }
}

public class FeeScheduleItemDto
{
    public int Id { get; set; }
    public int FeeScheduleId { get; set; }
    public int BillingItemId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Fee { get; set; }
}

public class SaveFeeScheduleItemDto
{
    public int BillingItemId { get; set; }
    public decimal Fee { get; set; }
}
