namespace GasMonitoringApi.Services;

public enum ServiceError
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized
}

public class ServiceException : Exception
{
    public ServiceError Error { get; }

    public ServiceException(ServiceError error, string message) : base(message)
    {
        Error = error;
    }
}
