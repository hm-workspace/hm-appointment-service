using Microsoft.AspNetCore.Mvc;
using AppointmentService.Utils.Common;
using AppointmentService.InternalModels.DTOs;
using AppointmentService.Services;

namespace AppointmentService.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AppointmentDto>>>> GetAppointments([FromQuery] SearchQuery searchQuery)
    {
        return Ok(await _appointmentService.GetAppointmentsAsync(searchQuery));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetAppointment(int id)
    {
        var response = await _appointmentService.GetAppointmentByIdAsync(id);
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet("by-appointment-id/{appointmentId}")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetAppointmentByAppointmentId(string appointmentId)
    {
        var response = await _appointmentService.GetAppointmentByAppointmentIdAsync(appointmentId);
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet("patient/{patientId:int}")]
    public async Task<ActionResult<ApiResponse<PagedResult<AppointmentDto>>>> GetPatientAppointments(int patientId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _appointmentService.GetPatientAppointmentsAsync(patientId, pageNumber, pageSize));
    }

    [HttpGet("doctor/{doctorId:int}")]
    public async Task<ActionResult<ApiResponse<PagedResult<AppointmentDto>>>> GetDoctorAppointments(int doctorId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _appointmentService.GetDoctorAppointmentsAsync(doctorId, pageNumber, pageSize));
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<ApiResponse<PagedResult<AppointmentDto>>>> GetUpcomingAppointments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _appointmentService.GetUpcomingAppointmentsAsync(pageNumber, pageSize));
    }

    [HttpGet("date-range")]
    public async Task<ActionResult<ApiResponse<PagedResult<AppointmentDto>>>> GetAppointmentsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _appointmentService.GetAppointmentsByDateRangeAsync(startDate, endDate, pageNumber, pageSize));
    }

    [HttpGet("check-conflict")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckAppointmentConflict([FromQuery] int doctorId, [FromQuery] DateTime appointmentDate, [FromQuery] string appointmentTime, [FromQuery] int? excludeAppointmentId = null)
    {
        return Ok(await _appointmentService.CheckAppointmentConflictAsync(doctorId, appointmentDate, appointmentTime, excludeAppointmentId));
    }

    [HttpGet("generate-id")]
    public async Task<ActionResult<ApiResponse<string>>> GenerateAppointmentId()
    {
        return Ok(await _appointmentService.GenerateAppointmentIdAsync());
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> CreateAppointment([FromBody] CreateAppointmentDto createAppointmentDto)
    {
        var response = await _appointmentService.CreateAppointmentAsync(createAppointmentDto);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(nameof(GetAppointment), new { id = response.Data?.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> UpdateAppointment(int id, [FromBody] UpdateAppointmentDto updateAppointmentDto)
    {
        var response = await _appointmentService.UpdateAppointmentAsync(id, updateAppointmentDto);
        if (!response.Success)
        {
            if (response.Message.Contains("conflict", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(response);
            }

            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteAppointment(int id)
    {
        var response = await _appointmentService.DeleteAppointmentAsync(id);
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<ApiResponse<string>>> CancelAppointment(int id, [FromBody] CancelAppointmentRequest request)
    {
        var response = await _appointmentService.CancelAppointmentAsync(id, request);
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}


