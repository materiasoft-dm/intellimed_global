using IntelliMed.Core.Entities;

namespace IntelliMed.Core.DTOs;

public class InvoiceDto
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public string? ClinicName { get; set; }
    public string? ClinicBusinessRegistrationNumber { get; set; }
    public string? ClinicAddress { get; set; }
    public string? ClinicCity { get; set; }
    public string? ClinicState { get; set; }
    public string? ClinicPostalCode { get; set; }
    public string? ClinicPhone { get; set; }
    public string? ClinicEmail { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public int? AppointmentId { get; set; }
    public int? PractitionerId { get; set; }
    public string? PractitionerName { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountOwing => TotalAmount - AmountPaid;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}

public class InvoiceItemDto
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int? BillingItemId { get; set; }
    public int? FeeScheduleId { get; set; }
    public string? BillingItemCode { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime? ServiceDate { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public string? Note { get; set; }
    public decimal NetAmount => UnitPrice * Quantity + TaxAmount;
    public decimal TotalPrice => NetAmount - Discount;
}

public class PaymentDto
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? ClientName { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public string MethodName => Method.ToString();
    public string? Reference { get; set; }
    public DateTime PaymentDate { get; set; }
}

public class CreateInvoiceDto
{
    public int ClinicId { get; set; }
    public int ClientId { get; set; }
    public int? AppointmentId { get; set; }
    public int? PractitionerId { get; set; }
    public DateTime DueDate { get; set; }
    public string? Notes { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; } = new();
}

public class CreateInvoiceItemDto
{
    public int? BillingItemId { get; set; }
    public int? FeeScheduleId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime? ServiceDate { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public string? Note { get; set; }
}

/// <summary>Looks up a billing item's price for the live line-item entry UI — a plain catalog/price-list lookup, no calculation.</summary>
public class ResolveLineRequest
{
    public int BillingItemId { get; set; }
    public int? FeeScheduleId { get; set; }
}

public class ResolveLineResult
{
    public decimal Fee { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CreatePaymentDto
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public string? Reference { get; set; }
    public DateTime PaymentDate { get; set; }
}

public class InvoiceSearchDto
{
    public int? ClinicId { get; set; }
    public int? ClientId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public InvoiceStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class PaymentSearchDto
{
    public int? ClinicId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public PaymentMethod? Method { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
