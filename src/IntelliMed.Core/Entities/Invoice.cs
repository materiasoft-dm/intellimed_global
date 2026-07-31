using System.ComponentModel.DataAnnotations.Schema;

namespace IntelliMed.Core.Entities;

public class Invoice
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public int ClientId { get; set; }
    public int? AppointmentId { get; set; }
    public int? PractitionerId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountOwing => TotalAmount - AmountPaid;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Client? Client { get; set; }
    public Appointment? Appointment { get; set; }
    public Practitioner? Practitioner { get; set; }
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class InvoiceItem
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }

    /// <summary>Optional — pick from the practice's billing item catalog to prefill Description/UnitPrice, or leave null for a free-text line.</summary>
    public int? BillingItemId { get; set; }

    /// <summary>Optional per-line price-list override — null uses the billing item's own base Fee.</summary>
    public int? FeeScheduleId { get; set; }

    public string Description { get; set; } = string.Empty;
    public DateTime? ServiceDate { get; set; }
    public int Quantity { get; set; } = 1;

    // Charged fee per unit — prefilled from the catalog/price list when one is picked, always editable.
    public decimal UnitPrice { get; set; }

    // Line-level discount and tax.
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }

    // Computed money fields.
    [NotMapped]
    public decimal NetAmount => UnitPrice * Quantity + TaxAmount;
    [NotMapped]
    public decimal TotalPrice => NetAmount - Discount;

    /// <summary>Free-text note attached to this line item — shown via the "i" icon in the line-items table.</summary>
    public string? Note { get; set; }

    // Navigation properties
    public Invoice? Invoice { get; set; }
    public BillingItem? BillingItem { get; set; }
    public FeeSchedule? FeeSchedule { get; set; }
}

public class Payment
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public string? Reference { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Invoice? Invoice { get; set; }
}

public enum InvoiceStatus
{
    Draft,
    Sent,
    Paid,
    PartiallyPaid,
    Overdue,
    Cancelled
}

public enum PaymentMethod
{
    Cash,
    Cheque,
    Eftpos,
    CreditCard,
    BankTransfer,
    Other
}
