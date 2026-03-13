using AppointmentService.InternalModels.Entities;
using AppointmentService.Utils.Common;

namespace AppointmentService.Repository;

public interface IAppointmentRepository
{
    Task<PagedResult<AppointmentEntity>> GetAppointmentsAsync(SearchQuery searchQuery);
    Task<AppointmentEntity?> GetAppointmentByIdAsync(int id);
    Task<AppointmentEntity?> GetAppointmentByAppointmentIdAsync(string appointmentId);
    Task<PagedResult<AppointmentEntity>> GetPatientAppointmentsAsync(int patientId, int pageNumber, int pageSize);
    Task<PagedResult<AppointmentEntity>> GetDoctorAppointmentsAsync(int doctorId, int pageNumber, int pageSize);
    Task<PagedResult<AppointmentEntity>> GetUpcomingAppointmentsAsync(int pageNumber, int pageSize);
    Task<PagedResult<AppointmentEntity>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
    Task<bool> CheckAppointmentConflictAsync(int doctorId, DateTime appointmentDate, string appointmentTime, int? excludeAppointmentId = null);
    Task<string> GenerateAppointmentIdAsync();
    Task<AppointmentEntity> CreateAppointmentAsync(AppointmentEntity appointment);
    Task<AppointmentEntity?> UpdateAppointmentAsync(int id, AppointmentEntity appointment);
    Task<bool> DeleteAppointmentAsync(int id);
    Task<bool> CancelAppointmentAsync(int id, string reason);
}

