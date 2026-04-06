using System.Data;
using AppointmentService.Data;
using Dapper;

namespace AppointmentService.Repository;

public abstract class BaseRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    protected BaseRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    protected async Task<TResult> ExecuteWithConnectionAsync<TResult>(
        Func<IDbConnection, Task<TResult>> databaseOperation,
        Func<Task<TResult>> fallbackOperation)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            return await databaseOperation(connection);
        }
        catch
        {
            return await fallbackOperation();
        }
    }

    protected async Task<TResult> ExecuteWithConnectionAsync<TResult>(
        Func<IDbConnection, Task<TResult>> databaseOperation,
        Func<TResult> fallbackOperation)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            return await databaseOperation(connection);
        }
        catch
        {
            return fallbackOperation();
        }
    }

    protected async Task<TResult> ExecuteWithConnectionAsync<TResult>(
        Func<IDbConnection, Task<TResult>> databaseOperation)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await databaseOperation(connection);
    }

    protected async Task ExecuteWithConnectionAsync(
        Func<IDbConnection, Task> databaseOperation,
        Func<Task> fallbackOperation)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            await databaseOperation(connection);
        }
        catch
        {
            await fallbackOperation();
        }
    }

    protected async Task ExecuteWithConnectionAsync(
        Func<IDbConnection, Task> databaseOperation,
        Action fallbackOperation)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            await databaseOperation(connection);
        }
        catch
        {
            fallbackOperation();
        }
    }

    protected Task<T?> QuerySingleOrDefaultAsync<T>(
        string sql,
        object? param = null,
        Func<T?>? fallback = null,
        CommandType? commandType = null)
        where T : class
    {
        return ExecuteWithConnectionAsync(
            async connection => await connection.QuerySingleOrDefaultAsync<T>(sql, param, commandType: commandType),
            fallback ?? (() => default(T)));
    }

    protected Task<IEnumerable<T>> QueryAsync<T>(
        string sql,
        object? param = null,
        Func<IEnumerable<T>>? fallback = null,
        CommandType? commandType = null)
    {
        return ExecuteWithConnectionAsync(
            async connection => await connection.QueryAsync<T>(sql, param, commandType: commandType),
            fallback ?? (() => Enumerable.Empty<T>()));
    }

    protected Task<int> ExecuteAsync(
        string sql,
        object? param = null,
        Func<int>? fallback = null,
        CommandType? commandType = null)
    {
        return ExecuteWithConnectionAsync(
            async connection => await connection.ExecuteAsync(sql, param, commandType: commandType),
            fallback ?? (() => 0));
    }

    protected Task<T> ExecuteScalarAsync<T>(
        string sql,
        object? param = null,
        Func<T>? fallback = null,
        CommandType? commandType = null)
    {
        return ExecuteWithConnectionAsync(
            async connection => await connection.ExecuteScalarAsync<T>(sql, param, commandType: commandType),
            fallback ?? (() => default(T)!));
    }
}
