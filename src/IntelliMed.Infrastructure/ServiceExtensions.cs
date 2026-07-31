using IntelliMed.Core.Interfaces;
using IntelliMed.Infrastructure.Data;
using IntelliMed.Infrastructure.Repositories;
using IntelliMed.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntelliMed.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // Add DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        // Register repositories
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IPractitionerRepository, PractitionerRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IBillingItemRepository, BillingItemRepository>();
        services.AddScoped<IFeeScheduleRepository, FeeScheduleRepository>();
        services.AddScoped<IClientAddressRepository, ClientAddressRepository>();
        services.AddScoped<IClientReferralRepository, ClientReferralRepository>();
        services.AddScoped<IClientOccupationRepository, ClientOccupationRepository>();
        services.AddScoped<IClientFamilyRelationshipRepository, ClientFamilyRelationshipRepository>();
        services.AddScoped<IUserDefinedFieldTypeRepository, UserDefinedFieldTypeRepository>();
        services.AddScoped<IClientUserDefinedFieldValueRepository, ClientUserDefinedFieldValueRepository>();
        services.AddScoped<IProviderGroupRepository, ProviderGroupRepository>();
        services.AddScoped<IClinicSettingsRepository, ClinicSettingsRepository>();
        services.AddScoped<IClinicRepository, ClinicRepository>();
        services.AddScoped<IAppointmentTypeSettingRepository, AppointmentTypeSettingRepository>();
        services.AddScoped<IProviderScheduleRepository, ProviderScheduleRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<ISearchActionRepository, SearchActionRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAppointmentReminderService, AppointmentReminderService>();
        services.AddScoped<IDatabaseBackupRepository, DatabaseBackupRepository>();
        services.AddScoped<IDatabaseBackupSettingsRepository, DatabaseBackupSettingsRepository>();
        services.AddScoped<IDatabaseBackupService, DatabaseBackupService>();
        services.AddHostedService<DatabaseBackupBackgroundService>();

        return services;
    }
}