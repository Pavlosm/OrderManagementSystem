namespace OrderManagementService.Core.Interfaces;

public struct ServiceError(ServiceErrorCode code, string? message, Exception? exception)
{
    public readonly ServiceErrorCode Code = code;
    public readonly string? Message = message ?? string.Empty;
    public readonly Exception? Exception = exception;
}