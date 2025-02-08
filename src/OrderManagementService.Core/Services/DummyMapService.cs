using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Interfaces;
using OrderManagementService.Core.Interfaces.Services;

namespace OrderManagementService.Core.Services;

public class DummyMapService : IMapService
{
    public Task<ServiceResult<bool>> IsValidAddressAsync(OrderDeliveryAddress deliveryAddress)
    {
        return Task.FromResult(ServiceResult<bool>.Ok(true));
    }
}