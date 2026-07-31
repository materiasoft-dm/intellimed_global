using IntelliMed.Core.DTOs;
using IntelliMed.Core.Entities;
using IntelliMed.Core.Interfaces;
using IntelliMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntelliMed.Infrastructure.Repositories;

public class ClinicSettingsRepository : Repository<ClinicSettings>, IClinicSettingsRepository
{
    public ClinicSettingsRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ClinicSettingsDto> GetSingletonAsync()
    {
        var settings = await _dbSet.SingleAsync(s => s.Id == 1);
        return new ClinicSettingsDto
        {
            PracticeName = settings.PracticeName,
            BusinessRegistrationNumber = settings.BusinessRegistrationNumber,
            Phone = settings.Phone,
            Fax = settings.Fax,
            Email = settings.Email,
            Website = settings.Website,
            Address = settings.Address,
            City = settings.City,
            PostalCode = settings.PostalCode,
            State = settings.State,
            MinimumTimeslotMinutes = settings.MinimumTimeslotMinutes,
            SmtpEnabled = settings.SmtpEnabled,
            SmtpHost = settings.SmtpHost,
            SmtpPort = settings.SmtpPort,
            SmtpUsername = settings.SmtpUsername,
            SmtpPassword = settings.SmtpPassword,
            SmtpFromEmail = settings.SmtpFromEmail,
            SmtpFromName = settings.SmtpFromName,
            SmtpUseSsl = settings.SmtpUseSsl
        };
    }

    public async Task UpdateSingletonAsync(UpdateClinicSettingsRequest request)
    {
        var settings = await _dbSet.SingleAsync(s => s.Id == 1);
        settings.PracticeName = request.PracticeName;
        settings.BusinessRegistrationNumber = request.BusinessRegistrationNumber;
        settings.Phone = request.Phone;
        settings.Fax = request.Fax;
        settings.Email = request.Email;
        settings.Website = request.Website;
        settings.Address = request.Address;
        settings.City = request.City;
        settings.PostalCode = request.PostalCode;
        settings.State = request.State;
        settings.MinimumTimeslotMinutes = request.MinimumTimeslotMinutes;
        settings.SmtpEnabled = request.SmtpEnabled;
        settings.SmtpHost = request.SmtpHost;
        settings.SmtpPort = request.SmtpPort;
        settings.SmtpUsername = request.SmtpUsername;
        settings.SmtpPassword = request.SmtpPassword;
        settings.SmtpFromEmail = request.SmtpFromEmail;
        settings.SmtpFromName = request.SmtpFromName;
        settings.SmtpUseSsl = request.SmtpUseSsl;
        await _context.SaveChangesAsync();
    }
}
