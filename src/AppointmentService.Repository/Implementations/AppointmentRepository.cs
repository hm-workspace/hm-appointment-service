using Dapper;
using AppointmentService.Data;
using AppointmentService.InternalModels.Entities;
using AppointmentService.Utils.Common;
using System.Data;

namespace AppointmentService.Repository;

public class AppointmentRepository : BaseRepository, IAppointmentRepository
{
    public AppointmentRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    public async Task<PagedResult<AppointmentEntity>> GetAppointmentsAsync(SearchQuery searchQuery)
    {
        return await ExecuteWithConnectionAsync(async connection =>
        {
            using var grid = await connection.QueryMultipleAsync(
                StoredProcedureNames.GetAppointmentsPaged,
                new { searchQuery.PageNumber, searchQuery.PageSize, SearchTerm = searchQuery.SearchTerm },
                commandType: CommandType.StoredProcedure);

            var items = (await grid.ReadAsync<AppointmentEntity>()).ToList();
            var total = await grid.ReadFirstAsync<int>();
            return new PagedResult<AppointmentEntity>(items, total, searchQuery.PageNumber, searchQuery.PageSize);
        });
    }

    public Task<AppointmentEntity?> GetAppointmentByIdAsync(int id)
    {
        return QuerySingleOrDefaultAsync<AppointmentEntity>(
            StoredProcedureNames.GetAppointmentById,
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public Task<AppointmentEntity?> GetAppointmentByAppointmentIdAsync(string appointmentId)
    {
        return QuerySingleOrDefaultAsync<AppointmentEntity>(
            StoredProcedureNames.GetAppointmentByAppointmentId,
            new { AppointmentId = appointmentId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<PagedResult<AppointmentEntity>> GetPatientAppointmentsAsync(int patientId, int pageNumber, int pageSize)
    {
        return await ExecuteWithConnectionAsync(async connection =>
        {
            using var grid = await connection.QueryMultipleAsync(
                StoredProcedureNames.GetPatientAppointmentsPaged,
                new { PatientId = patientId, PageNumber = pageNumber, PageSize = pageSize },
                commandType: CommandType.StoredProcedure);

            var items = (await grid.ReadAsync<AppointmentEntity>()).ToList();
            var total = await grid.ReadFirstAsync<int>();
            return new PagedResult<AppointmentEntity>(items, total, pageNumber, pageSize);
        });
    }

    public async Task<PagedResult<AppointmentEntity>> GetDoctorAppointmentsAsync(int doctorId, int pageNumber, int pageSize)
    {
        return await ExecuteWithConnectionAsync(async connection =>
        {
            using var grid = await connection.QueryMultipleAsync(
                StoredProcedureNames.GetDoctorAppointmentsPaged,
                new { DoctorId = doctorId, PageNumber = pageNumber, PageSize = pageSize },
                commandType: CommandType.StoredProcedure);

            var items = (await grid.ReadAsync<AppointmentEntity>()).ToList();
            var total = await grid.ReadFirstAsync<int>();
            return new PagedResult<AppointmentEntity>(items, total, pageNumber, pageSize);
        });
    }

    public async Task<PagedResult<AppointmentEntity>> GetUpcomingAppointmentsAsync(int pageNumber, int pageSize)
    {
        return await ExecuteWithConnectionAsync(async connection =>
        {
            using var grid = await connection.QueryMultipleAsync(
                StoredProcedureNames.GetUpcomingAppointmentsPaged,
                new { PageNumber = pageNumber, PageSize = pageSize },
                commandType: CommandType.StoredProcedure);

            var items = (await grid.ReadAsync<AppointmentEntity>()).ToList();
            var total = await grid.ReadFirstAsync<int>();
            return new PagedResult<AppointmentEntity>(items, total, pageNumber, pageSize);
        });
    }

    public async Task<PagedResult<AppointmentEntity>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
    {
        return await ExecuteWithConnectionAsync(async connection =>
        {
            using var grid = await connection.QueryMultipleAsync(
                StoredProcedureNames.GetAppointmentsByDateRangePaged,
                new { StartDate = startDate, EndDate = endDate, PageNumber = pageNumber, PageSize = pageSize },
                commandType: CommandType.StoredProcedure);

            var items = (await grid.ReadAsync<AppointmentEntity>()).ToList();
            var total = await grid.ReadFirstAsync<int>();
            return new PagedResult<AppointmentEntity>(items, total, pageNumber, pageSize);
        });
    }

    public async Task<bool> CheckAppointmentConflictAsync(int doctorId, DateTime appointmentDate, string appointmentTime, int? excludeAppointmentId = null)
    {
        var conflict = await ExecuteScalarAsync<int>(
            StoredProcedureNames.CheckAppointmentConflict,
            new
            {
                DoctorId = doctorId,
                AppointmentDate = appointmentDate,
                AppointmentTime = appointmentTime,
                ExcludeAppointmentId = excludeAppointmentId
            },
            commandType: CommandType.StoredProcedure);

        return conflict > 0;
    }

    public async Task<string> GenerateAppointmentIdAsync()
    {
        return await ExecuteScalarAsync<string>(
            StoredProcedureNames.GenerateAppointmentId,
            commandType: CommandType.StoredProcedure) ?? string.Empty;
    }

    public async Task<AppointmentEntity> CreateAppointmentAsync(AppointmentEntity appointment)
    {
        var id = await ExecuteScalarAsync<int>(
            StoredProcedureNames.CreateAppointment,
            appointment,
            commandType: CommandType.StoredProcedure);

        appointment.Id = id;
        if (string.IsNullOrWhiteSpace(appointment.AppointmentId))
        {
            appointment.AppointmentId = $"APT{id:000}";
        }
        return appointment;
    }

    public async Task<AppointmentEntity?> UpdateAppointmentAsync(int id, AppointmentEntity appointment)
    {
        var rowsAffected = await ExecuteAsync(
            StoredProcedureNames.UpdateAppointment,
            new
            {
                Id = id,
                appointment.PatientId,
                appointment.DoctorId,
                appointment.AppointmentDate,
                appointment.AppointmentTime,
                appointment.Reason,
                appointment.Status,
                appointment.Notes,
                appointment.CancelReason
            },
            commandType: CommandType.StoredProcedure);

        if (rowsAffected <= 0)
        {
            return null;
        }

        return await GetAppointmentByIdAsync(id);
    }

    public async Task<bool> DeleteAppointmentAsync(int id)
    {
        var rowsAffected = await ExecuteAsync(
            StoredProcedureNames.DeleteAppointment,
            new { Id = id },
            commandType: CommandType.StoredProcedure);
        return rowsAffected > 0;
    }

    public async Task<bool> CancelAppointmentAsync(int id, string reason)
    {
        var rowsAffected = await ExecuteAsync(
            StoredProcedureNames.CancelAppointment,
            new { Id = id, Reason = reason },
            commandType: CommandType.StoredProcedure);
        return rowsAffected > 0;
    }
}


