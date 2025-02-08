namespace OrderManagementService.Core.Interfaces;

public interface IServiceResult
{
    bool Success { get; }
    ServiceError? Error { get; }
}