namespace OrderManagementService.Core.Interfaces;

public class ServiceResult<T>
{
    public bool Success => Error == null;
    public readonly ServiceError? Error;
    public readonly T? Data;

    private ServiceResult(T? data)
    {
        Data = data;
    }
     
    private ServiceResult(ServiceErrorCode code, string? message, Exception? ex)
    {
        Error = new ServiceError(code, message ?? string.Empty, ex);
    }

    private ServiceResult(ServiceError error)
    {
        Error = error;
    }
    
    public static ServiceResult<T> Ok(T data) => new (data);

    public static ServiceResult<T> Fail(ServiceErrorCode code, string? message = null, Exception? ex = null) 
        => new (code, message, ex);

    public static ServiceResult<T> Fail(ServiceError error) => new(error);
}