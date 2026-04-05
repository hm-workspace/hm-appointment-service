using Dapper;
using AppointmentService.Data;
using AppointmentService.InternalModels.Entities;
using AppointmentService.Utils.Common;

namespace AppointmentService.Repository;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AppointmentRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public Task<PagedResult<AppointmentEntity>> GetAppointmentsAsync(SearchQuery searchQuery)
    {
        var query = AppointmentInMemoryStore.Appointments.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(searchQuery.SearchTerm))
        {
            query = query.Where(x =>
                x.AppointmentId.Contains(searchQuery.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                x.Reason.Contains(searchQuery.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                x.Status.Contains(searchQuery.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var total = query.Count();
        var items = query.OrderBy(x => x.AppointmentDate)
            .Skip((searchQuery.PageNumber - 1) * searchQuery.PageSize)
            .Take(searchQuery.PageSize)
            .ToList();

        return Task.FromResult(new PagedResult<AppointmentEntity>(items, total, searchQuery.PageNumber, searchQuery.PageSize));
    }

    public Task<AppointmentEntity?> GetAppointmentByIdAsync(int id) =>
        Task.FromResult(AppointmentInMemoryStore.Appointments.FirstOrDefault(x => x.Id == id));

    public Task<AppointmentEntity?> GetAppointmentByAppointmentIdAsync(string appointmentId) =>
        Task.FromResult(AppointmentInMemoryStore.Appointments.FirstOrDefault(x => x.AppointmentId.Equals(appointmentId, StringComparison.OrdinalIgnoreCase)));

    public Task<PagedResult<AppointmentEntity>> GetPatientAppointmentsAsync(int patientId, int pageNumber, int pageSize)
    {
        return Task.FromResult(PageResult(AppointmentInMemoryStore.Appointments.Where(x => x.PatientId == patientId), pageNumber, pageSize));
    }

    public Task<PagedResult<AppointmentEntity>> GetDoctorAppointmentsAsync(int doctorId, int pageNumber, int pageSize)
    {
        return Task.FromResult(PageResult(AppointmentInMemoryStore.Appointments.Where(x => x.DoctorId == doctorId), pageNumber, pageSize));
    }

    public Task<PagedResult<AppointmentEntity>> GetUpcomingAppointmentsAsync(int pageNumber, int pageSize)
    {
        var now = DateTime.UtcNow;
        return Task.FromResult(PageResult(
            AppointmentInMemoryStore.Appointments.Where(x =>
                x.AppointmentDate >= now &&
                !x.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)),
            pageNumber,
            pageSize));
    }

    public Task<PagedResult<AppointmentEntity>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
    {
        return Task.FromResult(PageResult(
            AppointmentInMemoryStore.Appointments.Where(x => x.AppointmentDate >= startDate && x.AppointmentDate <= endDate),
            pageNumber,
            pageSize));
    }

    public Task<bool> CheckAppointmentConflictAsync(int doctorId, DateTime appointmentDate, string appointmentTime, int? excludeAppointmentId = null)
    {
        var conflict = AppointmentInMemoryStore.Appointments.Any(x =>
            x.DoctorId == doctorId &&
            x.AppointmentDate.Date == appointmentDate.Date &&
            x.AppointmentTime.Equals(appointmentTime, StringComparison.OrdinalIgnoreCase) &&
            (!excludeAppointmentId.HasValue || x.Id != excludeAppointmentId.Value) &&
            !x.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(conflict);
    }

    public Task<string> GenerateAppointmentIdAsync()
    {
        var next = AppointmentInMemoryStore.Appointments.Count + 1;
        return Task.FromResult($"APT{next:000}");
    }

    public Task<AppointmentEntity> CreateAppointmentAsync(AppointmentEntity appointment)
    {
        appointment.Id = Interlocked.Increment(ref AppointmentInMemoryStore.AppointmentSeed);
        AppointmentInMemoryStore.Appointments.Add(appointment);
        return Task.FromResult(appointment);
    }

    public Task<AppointmentEntity?> UpdateAppointmentAsync(int id, AppointmentEntity appointment)
    {
        var existing = AppointmentInMemoryStore.Appointments.FirstOrDefault(x => x.Id == id);
        if (existing is null)
        {
            return Task.FromResult<AppointmentEntity?>(null);
        }

        existing.PatientId = appointment.PatientId;
        existing.DoctorId = appointment.DoctorId;
        existing.AppointmentDate = appointment.AppointmentDate;
        existing.AppointmentTime = appointment.AppointmentTime;
        existing.Reason = appointment.Reason;
        existing.Status = appointment.Status;
        existing.Notes = appointment.Notes;

        return Task.FromResult<AppointmentEntity?>(existing);
    }

    public Task<bool> DeleteAppointmentAsync(int id)
    {
        var existing = AppointmentInMemoryStore.Appointments.FirstOrDefault(x => x.Id == id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        AppointmentInMemoryStore.Appointments.Remove(existing);
        return Task.FromResult(true);
    }

    public Task<bool> CancelAppointmentAsync(int id, string reason)
    {
        var appointment = AppointmentInMemoryStore.Appointments.FirstOrDefault(x => x.Id == id);
        if (appointment is null)
        {
            return Task.FromResult(false);
        }

        appointment.Status = "Cancelled";
        appointment.CancelReason = reason;
        return Task.FromResult(true);
    }

    private static PagedResult<AppointmentEntity> PageResult(IEnumerable<AppointmentEntity> source, int pageNumber, int pageSize)
    {
        var total = source.Count();
        var items = source.OrderBy(x => x.AppointmentDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return new PagedResult<AppointmentEntity>(items, total, pageNumber, pageSize);
    }
}

internal static class AppointmentInMemoryStore
{
    public static int AppointmentSeed = 1;

    public static readonly List<AppointmentEntity> Appointments =
    [
        new AppointmentEntity
        {
            Id = 1,
            AppointmentId = "APT001",
            PatientId = 1,
            DoctorId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1).Date.AddHours(10),
            AppointmentTime = "10:00",
            Reason = "General Consultation",
            Status = "Scheduled",
            Notes = string.Empty,
            CancelReason = string.Empty
        }
    ];
}


