using System.Data;

namespace AppointmentService.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

