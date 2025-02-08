namespace OrderManagementService.Core.Interfaces;

public class VoidServiceResult : IServiceResult
{
    public bool Success => Error == null;
    public ServiceError? Error { get; }
    
    private VoidServiceResult() { }
    
    private VoidServiceResult(ServiceError error)
    {
        Error = error;
    }
    
    private VoidServiceResult(ServiceErrorCode code, string? message, Exception? ex) 
        : this(new ServiceError(code, message ?? string.Empty, ex)) {}
    
    public static VoidServiceResult Ok() => new ();
    

    public static VoidServiceResult Fail(ServiceErrorCode code, string? message = null, Exception? ex = null) 
        => new (code, message, ex);

    public static VoidServiceResult Fail(ServiceError error) => new(error);
}