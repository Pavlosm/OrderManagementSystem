namespace OrderManagementService.Core.Entities.OrderStatePattern;

public interface IStateFactory
{
    IOrderState CreateState(OrderBasic orderBasic); 
}