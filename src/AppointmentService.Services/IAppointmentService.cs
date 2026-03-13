using AppointmentService.InternalModels.DTOs;
using AppointmentService.Utils.Common;

namespace AppointmentService.Services;

public interface IAppointmentService
{
    Task<ApiResponse<PagedResult<AppointmentDto>>> GetAppointmentsAsync(SearchQuery searchQuery);
    Task<ApiResponse<AppointmentDto>> GetAppointmentByIdAsync(int id);
    Task<ApiResponse<AppointmentDto>> GetAppointmentByAppointmentIdAsync(string appointmentId);
    Task<ApiResponse<PagedResult<AppointmentDto>>> GetPatientAppointmentsAsync(int patientId, int pageNumber, int pageSize);
    Task<ApiResponse<PagedResult<AppointmentDto>>> GetDoctorAppointmentsAsync(int doctorId, int pageNumber, int pageSize);
    Task<ApiResponse<PagedResult<AppointmentDto>>> GetUpcomingAppointmentsAsync(int pageNumber, int pageSize);
    Task<ApiResponse<PagedResult<AppointmentDto>>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
    Task<ApiResponse<bool>> CheckAppointmentConflictAsync(int doctorId, DateTime appointmentDate, string appointmentTime, int? excludeAppointmentId = null);
    Task<ApiResponse<string>> GenerateAppointmentIdAsync();
    Task<ApiResponse<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto);
    Task<ApiResponse<AppointmentDto>> UpdateAppointmentAsync(int id, UpdateAppointmentDto updateAppointmentDto);
    Task<ApiResponse<string>> DeleteAppointmentAsync(int id);
    Task<ApiResponse<string>> CancelAppointmentAsync(int id, CancelAppointmentRequest request);
}
