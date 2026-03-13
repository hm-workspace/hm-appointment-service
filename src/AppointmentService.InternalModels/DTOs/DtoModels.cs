using AppointmentService.InternalModels.Entities;

namespace AppointmentService.InternalModels.DTOs;

public class CreateAppointmentDto
{
    public string AppointmentId { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string AppointmentTime { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Scheduled";
    public string Notes { get; set; } = string.Empty;
}

public class UpdateAppointmentDto : CreateAppointmentDto
{
}

public class CancelAppointmentRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class AppointmentDto
{
    public int Id { get; set; }
    public string AppointmentId { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string AppointmentTime { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string CancelReason { get; set; } = string.Empty;

    public static AppointmentDto FromEntity(AppointmentEntity entity) => new()
    {
        Id = entity.Id,
        AppointmentId = entity.AppointmentId,
        PatientId = entity.PatientId,
        DoctorId = entity.DoctorId,
        AppointmentDate = entity.AppointmentDate,
        AppointmentTime = entity.AppointmentTime,
        Reason = entity.Reason,
        Status = entity.Status,
        Notes = entity.Notes,
        CancelReason = entity.CancelReason
    };
}


