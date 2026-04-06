namespace AppointmentService.Repository;

public static class StoredProcedureNames
{
    public const string GetAppointmentsPaged = "dbo.GetAppointmentsPaged";
    public const string GetAppointmentById = "dbo.GetAppointmentById";
    public const string GetAppointmentByAppointmentId = "dbo.GetAppointmentByAppointmentId";
    public const string GetPatientAppointmentsPaged = "dbo.GetPatientAppointmentsPaged";
    public const string GetDoctorAppointmentsPaged = "dbo.GetDoctorAppointmentsPaged";
    public const string GetUpcomingAppointmentsPaged = "dbo.GetUpcomingAppointmentsPaged";
    public const string GetAppointmentsByDateRangePaged = "dbo.GetAppointmentsByDateRangePaged";
    public const string CheckAppointmentConflict = "dbo.CheckAppointmentConflict";
    public const string GenerateAppointmentId = "dbo.GenerateAppointmentId";
    public const string CreateAppointment = "dbo.CreateAppointment";
    public const string UpdateAppointment = "dbo.UpdateAppointment";
    public const string DeleteAppointment = "dbo.DeleteAppointment";
    public const string CancelAppointment = "dbo.CancelAppointment";
}
