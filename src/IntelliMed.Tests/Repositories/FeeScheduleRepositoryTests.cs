using FluentAssertions;
using IntelliMed.Core.DTOs;
using IntelliMed.Core.Entities;
using IntelliMed.Infrastructure.Data;
using IntelliMed.Infrastructure.Repositories;
using IntelliMed.Tests.Helpers;
using Xunit;

namespace IntelliMed.Tests.Repositories;

public class FeeScheduleRepositoryTests : IDisposable
{
    private readonly FeeScheduleRepository _repository;
    private readonly AppDbContext _context;
    private readonly int _billingItemId;

    public FeeScheduleRepositoryTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _repository = new FeeScheduleRepository(_context);

        var billingItem = new BillingItem { Code = "CONS-STD", Description = "Standard consultation", Fee = 75.00m, IsActive = true };
        _context.BillingItems.Add(billingItem);
        _context.SaveChanges();
        _billingItemId = billingItem.Id;
    }

    public void Dispose() => _context.Dispose();

    private async Task<int> CreateScheduleAsync(string code = "CORP") =>
        await _repository.CreateAsync(new CreateFeeScheduleDto { Code = code, Description = "Corporate contract rate" });

    [Fact]
    public async Task CreateAsync_ThenGetById_RoundTrips()
    {
        var id = await CreateScheduleAsync();

        var result = await _repository.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Code.Should().Be("CORP");
        result.IsArchived.Should().BeFalse();
    }

    [Fact]
    public async Task ArchiveAsync_ExcludesFromActiveList()
    {
        var id = await CreateScheduleAsync();

        await _repository.ArchiveAsync(id);

        (await _repository.GetAllActiveAsync()).Should().NotContain(s => s.Id == id);
    }

    [Fact]
    public async Task SaveItemAsync_NewBillingItem_AddsPriceOverride()
    {
        var scheduleId = await CreateScheduleAsync();

        await _repository.SaveItemAsync(scheduleId, new SaveFeeScheduleItemDto { BillingItemId = _billingItemId, Fee = 60m });

        var items = await _repository.GetItemsAsync(scheduleId);
        items.Should().ContainSingle();
        items[0].Fee.Should().Be(60m);
    }

    [Fact]
    public async Task SaveItemAsync_CalledTwiceForSameBillingItem_UpdatesInsteadOfDuplicating()
    {
        var scheduleId = await CreateScheduleAsync();

        await _repository.SaveItemAsync(scheduleId, new SaveFeeScheduleItemDto { BillingItemId = _billingItemId, Fee = 60m });
        await _repository.SaveItemAsync(scheduleId, new SaveFeeScheduleItemDto { BillingItemId = _billingItemId, Fee = 65m });

        var items = await _repository.GetItemsAsync(scheduleId);
        items.Should().ContainSingle();
        items[0].Fee.Should().Be(65m);
    }

    [Fact]
    public async Task RemoveItemAsync_RemovesPriceOverride()
    {
        var scheduleId = await CreateScheduleAsync();
        await _repository.SaveItemAsync(scheduleId, new SaveFeeScheduleItemDto { BillingItemId = _billingItemId, Fee = 60m });

        await _repository.RemoveItemAsync(scheduleId, _billingItemId);

        (await _repository.GetItemsAsync(scheduleId)).Should().BeEmpty();
    }

    [Fact]
    public async Task ResolveLineAsync_NoScheduleSpecified_ReturnsCatalogFee()
    {
        var result = await _repository.ResolveLineAsync(new ResolveLineRequest { BillingItemId = _billingItemId, FeeScheduleId = null });

        result.Should().NotBeNull();
        result!.Fee.Should().Be(75.00m);
    }

    [Fact]
    public async Task ResolveLineAsync_ScheduleHasOverride_ReturnsOverriddenFee()
    {
        var scheduleId = await CreateScheduleAsync();
        await _repository.SaveItemAsync(scheduleId, new SaveFeeScheduleItemDto { BillingItemId = _billingItemId, Fee = 60m });

        var result = await _repository.ResolveLineAsync(new ResolveLineRequest { BillingItemId = _billingItemId, FeeScheduleId = scheduleId });

        result!.Fee.Should().Be(60m);
    }

    [Fact]
    public async Task ResolveLineAsync_ScheduleHasNoOverrideForItem_FallsBackToCatalogFee()
    {
        var scheduleId = await CreateScheduleAsync();

        var result = await _repository.ResolveLineAsync(new ResolveLineRequest { BillingItemId = _billingItemId, FeeScheduleId = scheduleId });

        result!.Fee.Should().Be(75.00m);
    }

    [Fact]
    public async Task ResolveLineAsync_UnknownBillingItem_ReturnsNull()
    {
        var result = await _repository.ResolveLineAsync(new ResolveLineRequest { BillingItemId = 9999, FeeScheduleId = null });

        result.Should().BeNull();
    }
}
