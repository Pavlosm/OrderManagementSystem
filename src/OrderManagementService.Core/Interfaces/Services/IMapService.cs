using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Models;

namespace OrderManagementService.Core.Interfaces.Services;

public interface IMapService
{
    Task<ServiceResult<bool>> IsValidAddressAsync(OrderDeliveryAddress deliveryAddress);
}


    