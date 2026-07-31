using FluentAssertions;
using IntelliMed.Core.DTOs;
using IntelliMed.Core.Entities;
using IntelliMed.Infrastructure.Data;
using IntelliMed.Infrastructure.Repositories;
using IntelliMed.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntelliMed.Tests.Repositories;

public class ClientRepositoryTests : IDisposable
{
    private readonly ClientRepository _repository;
    private readonly AppDbContext _context;

    public ClientRepositoryTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _repository = new ClientRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ReturnsNewClientId()
    {
        // Arrange
        var dto = new CreateClientDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "0412345678",
            DateOfBirth = new DateTime(1990, 5, 15),
            Address = "123 Main Street"
        };

        // Act
        var result = await _repository.CreateAsync(dto);

        // Assert
        result.Should().BeGreaterThan(0);
        var client = await _context.Clients.FindAsync(result);
        client.Should().NotBeNull();
        client!.FirstName.Should().Be("John");
        client.LastName.Should().Be("Doe");
        client.Email.Should().Be("john.doe@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingClient_ReturnsClientDto()
    {
        // Arrange
        var client = new Client
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Phone = "0498765432",
            DateOfBirth = new DateTime(1985, 3, 20),
            IsActive = true
        };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(client.Id);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
        result.Email.Should().Be("jane.smith@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingClient_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithExistingClient_UpdatesClient()
    {
        // Arrange
        var client = new Client
        {
            FirstName = "Original",
            LastName = "Name",
            Email = "original@example.com",
            Phone = "0411111111",
            IsActive = true
        };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateClientDto
        {
            FirstName = "Updated",
            LastName = "Name",
            Email = "updated@example.com",
            Phone = "0422222222"
        };

        // Act
        await _repository.UpdateAsync(client.Id, updateDto);

        // Assert
        var updatedClient = await _context.Clients.FindAsync(client.Id);
        updatedClient!.FirstName.Should().Be("Updated");
        updatedClient.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingClient_ThrowsException()
    {
        // Arrange
        var updateDto = new UpdateClientDto
        {
            FirstName = "Test",
            LastName = "User"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task SearchAsync_WithNameQuery_ReturnsMatchingClients()
    {
        // Arrange
        var clients = new[]
        {
            new Client { FirstName = "John", LastName = "Doe", Email = "john@example.com", IsActive = true },
            new Client { FirstName = "John", LastName = "Smith", Email = "john.smith@example.com", IsActive = true },
            new Client { FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", IsActive = true }
        };
        _context.Clients.AddRange(clients);
        await _context.SaveChangesAsync();

        var search = new ClientSearchDto { Query = "John" };

        // Act
        var result = (await _repository.SearchAsync(search)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.LastName == "Doe");
        result.Should().Contain(p => p.LastName == "Smith");
    }

    [Fact]
    public async Task SearchAsync_WithEmailQuery_ReturnsMatchingClients()
    {
        // Arrange
        var clients = new[]
        {
            new Client { FirstName = "Test", LastName = "User1", Email = "test1@example.com", IsActive = true },
            new Client { FirstName = "Test", LastName = "User2", Email = "test2@example.com", IsActive = true },
            new Client { FirstName = "Other", LastName = "Person", Email = "other@example.com", IsActive = true }
        };
        _context.Clients.AddRange(clients);
        await _context.SaveChangesAsync();

        var search = new ClientSearchDto { Query = "test1@example.com" };

        // Act
        var result = (await _repository.SearchAsync(search)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Email.Should().Be("test1@example.com");
    }

    [Fact]
    public async Task SearchAsync_WithActiveFilter_ReturnsOnlyActiveClients()
    {
        // Arrange
        var clients = new[]
        {
            new Client { FirstName = "Active", LastName = "Client", Email = "active@example.com", IsActive = true },
            new Client { FirstName = "Inactive", LastName = "Client", Email = "inactive@example.com", IsActive = false }
        };
        _context.Clients.AddRange(clients);
        await _context.SaveChangesAsync();

        var search = new ClientSearchDto { IsActive = true };

        // Act
        var result = (await _repository.SearchAsync(search)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].FirstName.Should().Be("Active");
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPage()
    {
        // Arrange
        for (int i = 1; i <= 25; i++)
        {
            _context.Clients.Add(new Client
            {
                FirstName = $"Client{i:D2}",
                LastName = "Test",
                Email = $"client{i}@example.com",
                IsActive = true
            });
        }
        await _context.SaveChangesAsync();

        var search = new ClientSearchDto { Page = 2, PageSize = 10 };

        // Act
        var (items, totalCount) = await _repository.GetPagedAsync(search);

        // Assert
        totalCount.Should().Be(25);
        items.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllClients()
    {
        // Arrange
        var clients = new[]
        {
            new Client { FirstName = "Client1", LastName = "Test", Email = "p1@example.com", IsActive = true },
            new Client { FirstName = "Client2", LastName = "Test", Email = "p2@example.com", IsActive = true }
        };
        _context.Clients.AddRange(clients);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync()).ToList();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeleteAsync_RemovesClient()
    {
        // Arrange
        var client = new Client
        {
            FirstName = "ToDelete",
            LastName = "Client",
            Email = "delete@example.com",
            IsActive = true
        };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        var clientId = client.Id;

        // Act
        await _repository.DeleteAsync(clientId);

        // Assert
        var deletedClient = await _context.Clients.FindAsync(clientId);
        deletedClient.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_WithExistingClient_ReturnsTrue()
    {
        // Arrange
        var client = new Client
        {
            FirstName = "Exists",
            LastName = "Client",
            Email = "exists@example.com",
            IsActive = true
        };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(client.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingClient_ReturnsFalse()
    {
        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    #region Comprehensive Client Creation Tests

    [Fact]
    public async Task CreateAsync_WithMinimalRequiredFields_ReturnsCreatedClient()
    {
        // Arrange - Only required fields
        var dto = new CreateClientDto
        {
            FirstName = "Minimal",
            LastName = "Client",
            DateOfBirth = new DateTime(1990, 1, 1),
            DobAccuracy = DobAccuracyEnum.Day,
            Address = "123 Test St",
            City = "Testville",
            State = "VIC",
            PostalCode = "3000"
        };

        // Act
        var clientId = await _repository.CreateAsync(dto);

        // Assert
        var created = await _context.Clients.FindAsync(clientId);
        created.Should().NotBeNull();
        created!.FirstName.Should().Be("Minimal");
        created.LastName.Should().Be("Client");
        created.IsActive.Should().BeTrue();
        created.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task CreateAsync_WithAllGenders_ReturnsCorrectClients()
    {
        // Test all gender values
        var genders = new[]
        {
            GenderEnum.Unspecified,
            GenderEnum.Male,
            GenderEnum.Female,
            GenderEnum.Other
        };

        foreach (var gender in genders)
        {
            var dto = CreateMinimalClientDto();
            dto.FirstName = $"Client{gender}";
            dto.Gender = gender;

            var clientId = await _repository.CreateAsync(dto);
            var created = await _context.Clients.FindAsync(clientId);
            created!.Gender.Should().Be(gender, $"Gender {gender} should be saved correctly");
        }
    }

    [Fact]
    public async Task CreateAsync_WithAllMaritalStatuses_ReturnsCorrectClients()
    {
        // Test all marital status values
        var statuses = new[]
        {
            MaritalStatusEnum.Unknown,
            MaritalStatusEnum.Single,
            MaritalStatusEnum.Married,
            MaritalStatusEnum.DeFacto,
            MaritalStatusEnum.Divorced,
            MaritalStatusEnum.Widowed,
            MaritalStatusEnum.Separated
        };

        foreach (var status in statuses)
        {
            var dto = CreateMinimalClientDto();
            dto.FirstName = $"Client{status}";
            dto.MaritalStatus = status;

            var clientId = await _repository.CreateAsync(dto);
            var created = await _context.Clients.FindAsync(clientId);
            created!.MaritalStatus.Should().Be(status, $"Marital status {status} should be saved correctly");
        }
    }

    [Fact]
    public async Task CreateAsync_WithAllDobAccuracies_ReturnsCorrectClients()
    {
        // Test all DOB accuracy values
        var accuracies = new[]
        {
            DobAccuracyEnum.Day,
            DobAccuracyEnum.Month,
            DobAccuracyEnum.Year,
            DobAccuracyEnum.Estimated
        };

        foreach (var accuracy in accuracies)
        {
            var dto = CreateMinimalClientDto();
            dto.FirstName = $"Client{accuracy}";
            dto.DobAccuracy = accuracy;

            var clientId = await _repository.CreateAsync(dto);
            var created = await _context.Clients.FindAsync(clientId);
            created!.DobAccuracy.Should().Be(accuracy, $"DOB accuracy {accuracy} should be saved correctly");
        }
    }

    [Fact]
    public async Task CreateAsync_WithFullDetails_ReturnsCompleteClient()
    {
        // Arrange
        var dto = CreateFullClientDto();

        // Act
        var clientId = await _repository.CreateAsync(dto);

        // Assert
        var created = await _repository.GetByIdAsync(clientId);
        created.Should().NotBeNull();
        created!.FirstName.Should().Be("John");
        created.LastName.Should().Be("Doe");
        created.MiddleName.Should().Be("Michael");
        created.PreferredName.Should().Be("Johnny");
        created.Gender.Should().Be(GenderEnum.Male);
        created.DateOfBirth.Should().Be(new DateTime(1985, 6, 15));
        created.Address.Should().Be("123 Main Street");
        created.City.Should().Be("Melbourne");
        created.State.Should().Be("VIC");
        created.PostalCode.Should().Be("3000");
        created.Email.Should().Be("john.doe@example.com");
        created.Phone.Should().Be("0398765432");
        created.MobilePhone.Should().Be("0412345678");
        created.InterpreterRequired.Should().BeTrue();
        created.InterpreterLanguage.Should().Be("Italian");
        created.MaritalStatus.Should().Be(MaritalStatusEnum.Married);
        created.FileNumber.Should().Be("FILE001");
    }

    #endregion

    #region Comprehensive Client Retrieval Tests

    [Fact]
    public async Task GetByIdAsync_AfterMultipleCreates_ReturnsCorrectClient()
    {
        // Arrange
        var dto1 = CreateMinimalClientDto();
        dto1.FirstName = "First";
        dto1.LastName = "Client";

        var dto2 = CreateMinimalClientDto();
        dto2.FirstName = "Second";
        dto2.LastName = "Client";

        var dto3 = CreateMinimalClientDto();
        dto3.FirstName = "Third";
        dto3.LastName = "Client";

        var id1 = await _repository.CreateAsync(dto1);
        var id2 = await _repository.CreateAsync(dto2);
        var id3 = await _repository.CreateAsync(dto3);

        // Act
        var client1 = await _repository.GetByIdAsync(id1);
        var client2 = await _repository.GetByIdAsync(id2);
        var client3 = await _repository.GetByIdAsync(id3);

        // Assert
        client1!.FirstName.Should().Be("First");
        client2!.FirstName.Should().Be("Second");
        client3!.FirstName.Should().Be("Third");
    }

    [Fact]
    public async Task GetByIdAsync_WithFullClient_ReturnsAllFields()
    {
        // Arrange
        var dto = CreateFullClientDto();
        var clientId = await _repository.CreateAsync(dto);

        // Act
        var client = await _repository.GetByIdAsync(clientId);

        // Assert
        client.Should().NotBeNull();
        client!.FirstName.Should().Be("John");
        client.MiddleName.Should().Be("Michael");
        client.PreferredName.Should().Be("Johnny");
        client.InterpreterRequired.Should().BeTrue();
        client.InterpreterLanguage.Should().Be("Italian");
        client.FileNumber.Should().Be("FILE001");
    }

    [Fact]
    public async Task GetByIdAsync_WithArchivedClient_ReturnsClient()
    {
        // Arrange
        var dto = CreateMinimalClientDto();
        var clientId = await _repository.CreateAsync(dto);
        await _repository.ArchiveAsync(clientId); // Archive instead of delete

        // Act
        var client = await _repository.GetByIdAsync(clientId);

        // Assert - GetById should still return the client even if archived
        client.Should().NotBeNull();
        client!.IsActive.Should().BeFalse();
    }

    #endregion

    #region Comprehensive Client Update Tests

    [Fact]
    public async Task UpdateAsync_UpdatesBasicFields()
    {
        // Arrange
        var createDto = CreateMinimalClientDto();
        var clientId = await _repository.CreateAsync(createDto);

        var updateDto = new UpdateClientDto
        {
            FirstName = "UpdatedFirst",
            LastName = "UpdatedLast",
            DateOfBirth = new DateTime(1990, 1, 1),
            DobAccuracy = DobAccuracyEnum.Day,
            Address = "456 Updated Street",
            City = "UpdatedCity",
            State = "NSW",
            PostalCode = "2000",
            Email = "updated@example.com",
            Phone = "0299999999",
            Gender = GenderEnum.Female
        };

        // Act
        await _repository.UpdateAsync(clientId, updateDto);

        // Assert
        var updated = await _context.Clients.FindAsync(clientId);
        updated!.FirstName.Should().Be("UpdatedFirst");
        updated.LastName.Should().Be("UpdatedLast");
        updated.Gender.Should().Be(GenderEnum.Female);
        updated.Address.Should().Be("456 Updated Street");
        updated.City.Should().Be("UpdatedCity");
        updated.State.Should().Be("NSW");
        updated.PostalCode.Should().Be("2000");
        updated.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesContactDetails()
    {
        // Arrange
        var createDto = CreateMinimalClientDto();
        var clientId = await _repository.CreateAsync(createDto);

        var updateDto = new UpdateClientDto
        {
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            DateOfBirth = createDto.DateOfBirth,
            DobAccuracy = createDto.DobAccuracy,
            Address = createDto.Address,
            City = createDto.City,
            State = createDto.State,
            PostalCode = createDto.PostalCode,
            Email = "new.email@example.com",
            Phone = "0399998888",
            MobilePhone = "0499888777",
            BusinessHoursPhone = "0388886666",
            FaxNumber = "0388885555",
            AcceptSms = true,
            AcceptEmail = false
        };

        // Act
        await _repository.UpdateAsync(clientId, updateDto);

        // Assert
        var updated = await _context.Clients.FindAsync(clientId);
        updated!.Email.Should().Be("new.email@example.com");
        updated.MobilePhone.Should().Be("0499888777");
        updated.BusinessHoursPhone.Should().Be("0388886666");
        updated.FaxNumber.Should().Be("0388885555");
        updated.AcceptSms.Should().BeTrue();
        updated.AcceptEmail.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFileDetails()
    {
        // Arrange
        var createDto = CreateMinimalClientDto();
        var clientId = await _repository.CreateAsync(createDto);

        var updateDto = new UpdateClientDto
        {
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            DateOfBirth = createDto.DateOfBirth,
            DobAccuracy = createDto.DobAccuracy,
            Address = createDto.Address,
            City = createDto.City,
            State = createDto.State,
            PostalCode = createDto.PostalCode,
            FileNumber = "FILE999",
            UrNumber = "UR123456",
            LastSeenDate = new DateTime(2026, 7, 20)
        };

        // Act
        await _repository.UpdateAsync(clientId, updateDto);

        // Assert
        var updated = await _context.Clients.FindAsync(clientId);
        updated!.FileNumber.Should().Be("FILE999");
        updated.UrNumber.Should().Be("UR123456");
        updated.LastSeenDate.Should().Be(new DateTime(2026, 7, 20));
    }

    [Fact]
    public async Task UpdateAsync_SetsUpdatedAtTimestamp()
    {
        // Arrange
        var createDto = CreateMinimalClientDto();
        var clientId = await _repository.CreateAsync(createDto);
        var created = await _context.Clients.FindAsync(clientId);
        var originalCreatedAt = created!.CreatedAt;

        await Task.Delay(10); // Small delay to ensure different timestamp

        var updateDto = new UpdateClientDto
        {
            FirstName = "Updated",
            LastName = createDto.LastName,
            DateOfBirth = createDto.DateOfBirth,
            DobAccuracy = createDto.DobAccuracy,
            Address = createDto.Address,
            City = createDto.City,
            State = createDto.State,
            PostalCode = createDto.PostalCode
        };

        // Act
        await _repository.UpdateAsync(clientId, updateDto);

        // Assert
        var updated = await _context.Clients.FindAsync(clientId);
        updated!.UpdatedAt.Should().NotBeNull();
        updated.UpdatedAt.Should().BeOnOrAfter(originalCreatedAt);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesAllGenders()
    {
        // Test updating all gender values
        var genders = new[]
        {
            GenderEnum.Unspecified,
            GenderEnum.Male,
            GenderEnum.Female,
            GenderEnum.Other
        };

        foreach (var gender in genders)
        {
            var dto = CreateMinimalClientDto();
            var clientId = await _repository.CreateAsync(dto);

            var updateDto = new UpdateClientDto
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                DobAccuracy = dto.DobAccuracy,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Gender = gender
            };

            await _repository.UpdateAsync(clientId, updateDto);

            var updated = await _context.Clients.FindAsync(clientId);
            updated!.Gender.Should().Be(gender, $"Gender {gender} should be updated correctly");
        }
    }

    #endregion

    #region Helper Methods

    private static CreateClientDto CreateMinimalClientDto()
    {
        return new CreateClientDto
        {
            FirstName = "Test",
            LastName = "Client",
            DateOfBirth = new DateTime(1990, 1, 1),
            DobAccuracy = DobAccuracyEnum.Day,
            Address = "123 Test Street",
            City = "Testville",
            State = "VIC",
            PostalCode = "3000",
            Email = "test@example.com",
            Phone = "0398765432"
        };
    }

    private static CreateClientDto CreateFullClientDto()
    {
        return new CreateClientDto
        {
            // Personal
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "Michael",
            PreferredName = "Johnny",
            MaidenName = null,
            Title = "Mr",
            Gender = GenderEnum.Male,
            DateOfBirth = new DateTime(1985, 6, 15),
            DobAccuracy = DobAccuracyEnum.Day,
            PlaceOfBirth = "Melbourne",
            InterpreterRequired = true,
            InterpreterLanguage = "Italian",
            MaritalStatus = MaritalStatusEnum.Married,
            Ethnicity = "Australian",

            // Residential address
            Address = "123 Main Street",
            City = "Melbourne",
            State = "VIC",
            PostalCode = "3000",

            // Contact Details
            Email = "john.doe@example.com",
            Phone = "0398765432",
            BusinessHoursPhone = "0398765433",
            MobilePhone = "0412345678",
            FaxNumber = "0398765434",
            AcceptSms = true,
            AcceptEmail = true,
            AcceptOnlineAppointments = true,
            AcceptSmsMarketing = false,
            Notes = "Regular client",
            Warnings = null,
            NextOfKinClientId = null,
            NextOfKinName = "Mary Doe",
            NextOfKinPhone = "0412345679",
            EmergencyContactClientId = null,
            EmergencyContactName = "Bob Doe",
            EmergencyContactPhone = "0412345680",
            SameAsNextOfKin = false,

            // File
            FileNumber = "FILE001",
            UrNumber = "UR001",
            Deceased = false,
            ProviderId = null,
            LastSeenDate = new DateTime(2026, 7, 15),

            // Lifecard
            LifeCardNum = "LC123456"
        };
    }

    #endregion
}
