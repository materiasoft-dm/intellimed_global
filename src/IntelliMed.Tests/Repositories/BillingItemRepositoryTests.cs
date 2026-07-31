using FluentAssertions;
using IntelliMed.Core.DTOs;
using IntelliMed.Core.Entities;
using IntelliMed.Infrastructure.Data;
using IntelliMed.Infrastructure.Repositories;
using IntelliMed.Tests.Helpers;
using Xunit;

namespace IntelliMed.Tests.Repositories;

public class BillingItemRepositoryTests : IDisposable
{
    private readonly BillingItemRepository _repository;
    private readonly AppDbContext _context;

    public BillingItemRepositoryTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _repository = new BillingItemRepository(_context);

        _context.BillingItems.AddRange(
            new BillingItem { Code = "CONS-STD", Description = "Standard consultation", Fee = 75.00m, IsActive = true },
            new BillingItem { Code = "CONS-LONG", Description = "Long consultation", Fee = 120.00m, IsActive = true },
            new BillingItem { Code = "CONS-OLD", Description = "Retired consultation type", Fee = 60.00m, IsActive = false });
        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task SearchAsync_MatchingCode_ReturnsItem()
    {
        var result = (await _repository.SearchAsync("STD")).ToList();

        result.Should().ContainSingle();
        result[0].Code.Should().Be("CONS-STD");
    }

    [Fact]
    public async Task SearchAsync_MatchingDescription_ReturnsItem()
    {
        var result = (await _repository.SearchAsync("long")).ToList();

        result.Should().ContainSingle();
        result[0].Code.Should().Be("CONS-LONG");
    }

    [Fact]
    public async Task SearchAsync_ExcludesInactiveItems()
    {
        var result = (await _repository.SearchAsync("OLD")).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllActiveAsync_ReturnsActiveItemsOrderedByCode()
    {
        var result = await _repository.GetAllActiveAsync();

        result.Select(r => r.Code).Should().ContainInOrder("CONS-LONG", "CONS-STD");
    }

    [Fact]
    public async Task CreateAsync_ThenGetById_RoundTrips()
    {
        var id = await _repository.CreateAsync(new CreateBillingItemDto { Code = "NEW", Description = "New item", Fee = 50m });

        var result = await _repository.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Code.Should().Be("NEW");
        result.Fee.Should().Be(50m);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_ChangesFee()
    {
        var id = await _repository.CreateAsync(new CreateBillingItemDto { Code = "NEW", Description = "New item", Fee = 50m });

        await _repository.UpdateAsync(id, new UpdateBillingItemDto { Code = "NEW", Description = "New item", Fee = 65m, IsActive = true });

        (await _repository.GetByIdAsync(id))!.Fee.Should().Be(65m);
    }

    [Fact]
    public async Task ArchiveAsync_SetsInactiveAndExcludesFromActiveList()
    {
        var id = await _repository.CreateAsync(new CreateBillingItemDto { Code = "NEW", Description = "New item", Fee = 50m });

        await _repository.ArchiveAsync(id);

        (await _repository.GetByIdAsync(id))!.IsActive.Should().BeFalse();
        (await _repository.GetAllActiveAsync()).Should().NotContain(i => i.Id == id);
    }
}
