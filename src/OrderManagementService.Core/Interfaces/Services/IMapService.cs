using OrderManagementService.Core.Entities;

namespace OrderManagementService.Core.Interfaces.Services;

public interface IMapService
{
    Task<ServiceResult<bool>> IsValidAddressAsync(OrderDeliveryAddress deliveryAddress);
}


    