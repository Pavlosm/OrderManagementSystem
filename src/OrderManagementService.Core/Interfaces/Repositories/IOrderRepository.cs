using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Models;

namespace OrderManagementService.Core.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId);
    Task<int> CreateAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Guid id);
} 