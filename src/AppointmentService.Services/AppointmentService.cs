using AppointmentService.InternalModels.DTOs;
using AppointmentService.InternalModels.Entities;
using AppointmentService.Repository;
using AppointmentService.Utils.Common;

namespace AppointmentService.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentService(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<ApiResponse<PagedResult<AppointmentDto>>> GetAppointmentsAsync(SearchQuery searchQuery)
    {
        var page = await _appointmentRepository.GetAppointmentsAsync(searchQuery);
        var dto = new PagedResult<AppointmentDto>(page.Items.Select(AppointmentDto.FromEntity).ToList(), page.TotalCount, page.PageNumber, page.PageSize);
        return ApiResponse<PagedResult<AppointmentDto>>.Ok(dto);
    }

    public async Task<ApiResponse<AppointmentDto>> GetAppointmentByIdAsync(int id)
    {
        var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
        return appointment is null ? ApiResponse<AppointmentDto>.Fail("Appointment not found") : ApiResponse<AppointmentDto>.Ok(AppointmentDto.FromEntity(appointment));
    }

    public async Task<ApiResponse<AppointmentDto>> GetAppointmentByAppointmentIdAsync(string appointmentId)
    {
        var appointment = await _appointmentRepository.GetAppointmentByAppointmentIdAsync(appointmentId);
        return appointment is null ? ApiResponse<AppointmentDto>.Fail("Appointment not found") : ApiResponse<AppointmentDto>.Ok(AppointmentDto.FromEntity(appointment));
    }

    public async Task<ApiResponse<PagedResult<AppointmentDto>>> GetPatientAppointmentsAsync(int patientId, int pageNumber, int pageSize)
    {
        var page = await _appointmentRepository.GetPatientAppointmentsAsync(patientId, pageNumber, pageSize);
        var dto = new PagedResult<AppointmentDto>(page.Items.Select(AppointmentDto.FromEntity).ToList(), page.TotalCount, page.PageNumber, page.PageSize);
        return ApiResponse<PagedResult<AppointmentDto>>.Ok(dto);
    }

    public async Task<ApiResponse<PagedResult<AppointmentDto>>> GetDoctorAppointmentsAsync(int doctorId, int pageNumber, int pageSize)
    {
        var page = await _appointmentRepository.GetDoctorAppointmentsAsync(doctorId, pageNumber, pageSize);
        var dto = new PagedResult<AppointmentDto>(page.Items.Select(AppointmentDto.FromEntity).ToList(), page.TotalCount, page.PageNumber, page.PageSize);
        return ApiResponse<PagedResult<AppointmentDto>>.Ok(dto);
    }

    public async Task<ApiResponse<PagedResult<AppointmentDto>>> GetUpcomingAppointmentsAsync(int pageNumber, int pageSize)
    {
        var page = await _appointmentRepository.GetUpcomingAppointmentsAsync(pageNumber, pageSize);
        var dto = new PagedResult<AppointmentDto>(page.Items.Select(AppointmentDto.FromEntity).ToList(), page.TotalCount, page.PageNumber, page.PageSize);
        return ApiResponse<PagedResult<AppointmentDto>>.Ok(dto);
    }

    public async Task<ApiResponse<PagedResult<AppointmentDto>>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
    {
        var page = await _appointmentRepository.GetAppointmentsByDateRangeAsync(startDate, endDate, pageNumber, pageSize);
        var dto = new PagedResult<AppointmentDto>(page.Items.Select(AppointmentDto.FromEntity).ToList(), page.TotalCount, page.PageNumber, page.PageSize);
        return ApiResponse<PagedResult<AppointmentDto>>.Ok(dto);
    }

    public async Task<ApiResponse<bool>> CheckAppointmentConflictAsync(int doctorId, DateTime appointmentDate, string appointmentTime, int? excludeAppointmentId = null)
    {
        return ApiResponse<bool>.Ok(await _appointmentRepository.CheckAppointmentConflictAsync(doctorId, appointmentDate, appointmentTime, excludeAppointmentId));
    }

    public async Task<ApiResponse<string>> GenerateAppointmentIdAsync()
    {
        return ApiResponse<string>.Ok(await _appointmentRepository.GenerateAppointmentIdAsync());
    }

    public async Task<ApiResponse<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto)
    {
        var conflict = await _appointmentRepository.CheckAppointmentConflictAsync(
            createAppointmentDto.DoctorId,
            createAppointmentDto.AppointmentDate,
            createAppointmentDto.AppointmentTime);

        if (conflict)
        {
            return ApiResponse<AppointmentDto>.Fail("Appointment conflict detected for doctor and slot");
        }

        if (string.IsNullOrWhiteSpace(createAppointmentDto.AppointmentId))
        {
            createAppointmentDto.AppointmentId = await _appointmentRepository.GenerateAppointmentIdAsync();
        }

        var entity = new AppointmentEntity
        {
            AppointmentId = createAppointmentDto.AppointmentId,
            PatientId = createAppointmentDto.PatientId,
            DoctorId = createAppointmentDto.DoctorId,
            AppointmentDate = createAppointmentDto.AppointmentDate,
            AppointmentTime = createAppointmentDto.AppointmentTime,
            Reason = createAppointmentDto.Reason,
            Status = string.IsNullOrWhiteSpace(createAppointmentDto.Status) ? "Scheduled" : createAppointmentDto.Status,
            Notes = createAppointmentDto.Notes,
            CancelReason = string.Empty
        };

        var created = await _appointmentRepository.CreateAppointmentAsync(entity);
        return ApiResponse<AppointmentDto>.Ok(AppointmentDto.FromEntity(created), "Appointment created successfully");
    }

    public async Task<ApiResponse<AppointmentDto>> UpdateAppointmentAsync(int id, UpdateAppointmentDto updateAppointmentDto)
    {
        var conflict = await _appointmentRepository.CheckAppointmentConflictAsync(
            updateAppointmentDto.DoctorId,
            updateAppointmentDto.AppointmentDate,
            updateAppointmentDto.AppointmentTime,
            id);

        if (conflict)
        {
            return ApiResponse<AppointmentDto>.Fail("Appointment conflict detected for doctor and slot");
        }

        var entity = new AppointmentEntity
        {
            AppointmentId = updateAppointmentDto.AppointmentId,
            PatientId = updateAppointmentDto.PatientId,
            DoctorId = updateAppointmentDto.DoctorId,
            AppointmentDate = updateAppointmentDto.AppointmentDate,
            AppointmentTime = updateAppointmentDto.AppointmentTime,
            Reason = updateAppointmentDto.Reason,
            Status = updateAppointmentDto.Status,
            Notes = updateAppointmentDto.Notes
        };

        var updated = await _appointmentRepository.UpdateAppointmentAsync(id, entity);
        return updated is null ? ApiResponse<AppointmentDto>.Fail("Appointment not found") : ApiResponse<AppointmentDto>.Ok(AppointmentDto.FromEntity(updated), "Appointment updated successfully");
    }

    public async Task<ApiResponse<string>> DeleteAppointmentAsync(int id)
    {
        var deleted = await _appointmentRepository.DeleteAppointmentAsync(id);
        return deleted ? ApiResponse<string>.Ok("Appointment deleted successfully") : ApiResponse<string>.Fail("Appointment not found");
    }

    public async Task<ApiResponse<string>> CancelAppointmentAsync(int id, CancelAppointmentRequest request)
    {
        var cancelled = await _appointmentRepository.CancelAppointmentAsync(id, request.Reason);
        return cancelled ? ApiResponse<string>.Ok("Appointment cancelled successfully") : ApiResponse<string>.Fail("Appointment not found");
    }
}
