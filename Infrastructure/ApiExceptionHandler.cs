using GasMonitoringApi.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GasMonitoringApi.Infrastructure;

public class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        int status;
        string message;

        if (exception is ServiceException error)
        {
            status = error.Error switch
            {
                ServiceError.Validation => 400,
                ServiceError.NotFound => 404,
                ServiceError.Conflict => 409,
                ServiceError.Unauthorized => 401,
                _ => 500
            };
            message = error.Message;
        }
        else if (exception is DbUpdateException { InnerException: SqliteException { SqliteErrorCode: 19 } })
        {
            status = 409;
            message = "Изменение нарушает ограничения данных. Обновите данные и повторите запрос.";
        }
        else
        {
            return false;
        }

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = message
        }, cancellationToken);
        return true;
    }
}
