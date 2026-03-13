namespace AppointmentService.InternalModels.Entities;

public class AppointmentEntity
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
}


