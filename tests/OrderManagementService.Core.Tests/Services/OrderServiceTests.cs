using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Interfaces;
using OrderManagementService.Core.Services;
using Moq;
using OrderManagementService.Core.Entities.OrderStatePattern;
using OrderManagementService.Core.Interfaces.Repositories;
using OrderManagementService.Core.Interfaces.Services;
using OrderManagementService.Core.Models;

namespace OrderManagementService.Core.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock = new (MockBehavior.Strict);
        private readonly Mock<IMenuItemService> _menuItemServiceMock = new (MockBehavior.Strict);
        private readonly Mock<IMapService> _mapServiceMock = new (MockBehavior.Strict);
        private readonly Mock<IStateFactory> _stateFactoryMock = new (MockBehavior.Strict);
        
        private static readonly string UserId = Guid.NewGuid().ToString();
        
        private OrderService CreateSut() => new (
            _orderRepositoryMock.Object,
            _menuItemServiceMock.Object,
            _mapServiceMock.Object,
            _stateFactoryMock.Object);

        public OrderServiceTests()
        {
            _mapServiceMock
                .Setup(proxy => proxy.IsValidAddressAsync(It.IsAny<OrderDeliveryAddress>()))
                .ReturnsAsync(ServiceResult<bool>.Ok(true));
        }
        
        [Fact]
        public async Task GetFullOrderAsync_ShouldReturnOrder_WhenOrderExists()
        {
            var order = new Order { Id = 1 };
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(order);

            var result = await CreateSut().GetFullOrderAsync(1);

            Assert.True(result.Success);
            Assert.Equal(order, result.Data);
        }

        [Fact]
        public async Task GetFullOrderAsync_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Order?)null);

            var result = await CreateSut().GetFullOrderAsync(1);

            Assert.False(result.Success);
            Assert.Equal(ServiceErrorCode.NotFound, result.Error!.Value.Code);
        }

        [Fact]
        public async Task FilterOrdersAsync_ShouldReturnOrders_WhenOrdersExist()
        {
            var orders = new List<OrderBasic> { new() { Id = 1 } };
            _orderRepositoryMock
                .Setup(repo => repo.GetByStatusAndTypeAsync(OrderStatus.Pending, OrderType.Pickup))
                .ReturnsAsync(orders);

            var result = await CreateSut().FilterOrdersAsync(OrderStatus.Pending, OrderType.Pickup);

            Assert.True(result.Success);
            Assert.Equal(orders, result.Data);
        }

        [Fact]
        public async Task PlaceOrderAsync_Pickup_ShouldReturnOrder_WhenValidRequest()
        {
            var orderPlacementRequest = new OrderPlacementRequest
            {
                OrderType = OrderType.Pickup,
                Items = new Dictionary<int, OrderPlacementRequest.Item>
                {
                    { 1, new OrderPlacementRequest.Item { Quantity = 1, SpecialInstructions = "1"} },
                    { 2, new OrderPlacementRequest.Item { Quantity = 1, SpecialInstructions = "2"} }
                },
                ContactDetails = new OrderContactDetails { Name = "John Doe", PhoneNumber = "1234567890" },
                DeliveryAddress = new OrderDeliveryAddress { Street = "123 Main St", City = "Springfield", PostalCode = "62701" }
            };

            var menuItems = new List<MenuItem>
            {
                new() { Id = 1, Price = 10 },
                new() { Id = 2, Price = 20 }
            };
            
            _menuItemServiceMock
                .Setup(service => service.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(ServiceResult<List<MenuItem>>.Ok(menuItems));

            _orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>())).ReturnsAsync(1);

            var result = await CreateSut().PlaceOrderAsync(UserId, orderPlacementRequest);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal(OrderStatus.Pending, result.Data.Status);
            Assert.Equal(orderPlacementRequest.OrderType, result.Data.Type);
            Assert.Null(result.Data.DeliveryAddress);
            Assert.Equal(orderPlacementRequest.SpecialInstructions, result.Data.SpecialInstructions);
            Assert.Equal(UserId, result.Data.CreatedBy);
            Assert.Equal(orderPlacementRequest.ContactDetails, result.Data.ContactDetails);
            Assert.Equal(30, result.Data.TotalAmount);
            Assert.Collection(result.Data.Items,
                item =>
                {
                    Assert.Equal(1, item.MenuItemId);
                    Assert.Equal(10, item.UnitPrice);
                    Assert.Equal(1, item.Quantity);
                    Assert.Equal("1", item.SpecialInstructions);
                },
                item =>
                {
                    Assert.Equal(2, item.MenuItemId);
                    Assert.Equal(20, item.UnitPrice);
                    Assert.Equal(1, item.Quantity);
                    Assert.Equal("2", item.SpecialInstructions);
                });
            Assert.Equal(OrderStatus.Pending, result.Data.Status);
            Assert.Equal(OrderStatus.Pending, result.Data.Status);
        }
        
        [Fact]
        public async Task PlaceOrderAsync_Delivery_ShouldReturnOrder_WhenValidRequest()
        {
            var orderPlacementRequest = new OrderPlacementRequest
            {
                OrderType = OrderType.Delivery,
                Items = new Dictionary<int, OrderPlacementRequest.Item>
                {
                    { 1, new OrderPlacementRequest.Item { Quantity = 1, SpecialInstructions = "1"} },
                    { 2, new OrderPlacementRequest.Item { Quantity = 1, SpecialInstructions = "2"} }
                },
                ContactDetails = new OrderContactDetails { Name = "John Doe", PhoneNumber = "1234567890" },
                DeliveryAddress = new OrderDeliveryAddress { Street = "123 Main St", City = "Springfield", PostalCode = "62701" }
            };

            var menuItems = new List<MenuItem>
            {
                new() { Id = 1, Price = 10 },
                new() { Id = 2, Price = 20 }
            };
            
            _menuItemServiceMock
                .Setup(service => service.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(ServiceResult<List<MenuItem>>.Ok(menuItems));

            _orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>())).ReturnsAsync(1);

            var result = await CreateSut().PlaceOrderAsync(UserId, orderPlacementRequest);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal(OrderStatus.Pending, result.Data.Status);
            Assert.Equal(orderPlacementRequest.OrderType, result.Data.Type);
            Assert.Equal(orderPlacementRequest.DeliveryAddress, result.Data.DeliveryAddress);
            Assert.Equal(orderPlacementRequest.SpecialInstructions, result.Data.SpecialInstructions);
            Assert.Equal(UserId, result.Data.CreatedBy);
            Assert.Equal(orderPlacementRequest.ContactDetails, result.Data.ContactDetails);
            Assert.Equal(30, result.Data.TotalAmount);
            Assert.Collection(result.Data.Items,
                item =>
                {
                    Assert.Equal(1, item.MenuItemId);
                    Assert.Equal(10, item.UnitPrice);
                    Assert.Equal(1, item.Quantity);
                    Assert.Equal("1", item.SpecialInstructions);
                },
                item =>
                {
                    Assert.Equal(2, item.MenuItemId);
                    Assert.Equal(20, item.UnitPrice);
                    Assert.Equal(1, item.Quantity);
                    Assert.Equal("2", item.SpecialInstructions);
                });
            Assert.Equal(OrderStatus.Pending, result.Data.Status);
            Assert.Equal(OrderStatus.Pending, result.Data.Status);
        }
        
        [Fact]
        public async Task PlaceOrderAsync_Delivery_AddressNull_ReturnsError()
        {
            var orderPlacementRequest = new OrderPlacementRequest
            {
                OrderType = OrderType.Delivery,
                Items = new Dictionary<int, OrderPlacementRequest.Item>
                {
                    { 1, new OrderPlacementRequest.Item { Quantity = 1, SpecialInstructions = "1"} },
                    { 2, new OrderPlacementRequest.Item { Quantity = 1, SpecialInstructions = "2"} }
                },
                ContactDetails = new OrderContactDetails { Name = "John Doe", PhoneNumber = "1234567890" }
            };

            var menuItems = new List<MenuItem>
            {
                new() { Id = 1, Price = 10 },
                new() { Id = 2, Price = 20 }
            };
            
            _menuItemServiceMock
                .Setup(service => service.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(ServiceResult<List<MenuItem>>.Ok(menuItems));

            _orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>())).ReturnsAsync(1);

            var result = await CreateSut().PlaceOrderAsync(UserId, orderPlacementRequest);

            Assert.False(result.Success);
            Assert.Equal(ServiceErrorCode.BadRequest, result.Error!.Value.Code);
        }
        
        [Fact]
        public async Task PlaceOrderAsync_NoItems_ReturnsError()
        {
            var orderPlacementRequest = new OrderPlacementRequest
            {
                OrderType = OrderType.Pickup,
                Items = new Dictionary<int, OrderPlacementRequest.Item>(),
                ContactDetails = new OrderContactDetails { Name = "John Doe", PhoneNumber = "1234567890" },
                DeliveryAddress = new OrderDeliveryAddress { Street = "123 Main St", City = "Springfield", PostalCode = "62701" }
            };

            var menuItems = new List<MenuItem>
            {
                new() { Id = 1, Price = 10 },
                new() { Id = 2, Price = 20 }
            };
            
            _menuItemServiceMock
                .Setup(service => service.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(ServiceResult<List<MenuItem>>.Ok(menuItems));

            _orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>())).ReturnsAsync(1);

            var result = await CreateSut().PlaceOrderAsync(UserId, orderPlacementRequest);

            Assert.False(result.Success);
            Assert.Equal(ServiceErrorCode.BadRequest, result.Error!.Value.Code);
        }
        
        [Fact]
        public async Task PlaceOrderAsync_InvalidQuantities_ReturnsError()
        {
            var orderPlacementRequest = new OrderPlacementRequest
            {
                OrderType = OrderType.Pickup,
                Items = new Dictionary<int, OrderPlacementRequest.Item>
                {
                    { 1, new OrderPlacementRequest.Item { Quantity = 0, SpecialInstructions = "1"} },
                    { 2, new OrderPlacementRequest.Item { Quantity = 1, SpecialInstructions = "2"} }
                },
                ContactDetails = new OrderContactDetails { Name = "John Doe", PhoneNumber = "1234567890" },
                DeliveryAddress = new OrderDeliveryAddress { Street = "123 Main St", City = "Springfield", PostalCode = "62701" }
            };

            var menuItems = new List<MenuItem>
            {
                new() { Id = 1, Price = 10 },
                new() { Id = 2, Price = 20 }
            };
            
            _menuItemServiceMock
                .Setup(service => service.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(ServiceResult<List<MenuItem>>.Ok(menuItems));

            _orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>())).ReturnsAsync(1);

            var result = await CreateSut().PlaceOrderAsync(UserId, orderPlacementRequest);

            Assert.False(result.Success);
            Assert.Equal(ServiceErrorCode.BadRequest, result.Error!.Value.Code);
        }
        
        [Fact]
        public async Task PlaceOrderAsync_NonExistingMenuItems_ReturnsError()
        {
            var orderPlacementRequest = new OrderPlacementRequest
            {
                OrderType = OrderType.Pickup,
                Items = new Dictionary<int, OrderPlacementRequest.Item>
                {
                    { 1, new OrderPlacementRequest.Item { Quantity = 1, SpecialInstructions = "1"} },
                    { 2, new OrderPlacementRequest.Item { Quantity = 1, SpecialInstructions = "2"} }
                },
                ContactDetails = new OrderContactDetails { Name = "John Doe", PhoneNumber = "1234567890" },
                DeliveryAddress = new OrderDeliveryAddress { Street = "123 Main St", City = "Springfield", PostalCode = "62701" }
            };

            var menuItems = new List<MenuItem>
            {
                new() { Id = 1, Price = 10 },
            };
            
            _menuItemServiceMock
                .Setup(service => service.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(ServiceResult<List<MenuItem>>.Ok(menuItems));

            _orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>())).ReturnsAsync(1);

            var result = await CreateSut().PlaceOrderAsync(UserId, orderPlacementRequest);

            Assert.False(result.Success);
            Assert.Equal(ServiceErrorCode.BadRequest, result.Error!.Value.Code);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnSuccess_WhenValidTransition()
        {
            var order = new OrderBasic { Id = 1, Status = OrderStatus.Pending };
            _orderRepositoryMock.Setup(repo => repo.GetBasicByIdAsync(1)).ReturnsAsync(order);

            var stateMock = new Mock<IOrderState>();
            stateMock
                .Setup(state => state.Transition(OrderStatus.Preparing))
                .Returns((stateMock.Object, null));
            _stateFactoryMock.Setup(factory => factory.CreateState(order)).Returns(stateMock.Object);

            _orderRepositoryMock
                .Setup(repo => repo.UpdateStatusAsync(1, stateMock.Object, "user1", It.IsAny<byte[]>()))
                .ReturnsAsync(1);

            var result = await CreateSut().UpdateStatusAsync("user1", 1, OrderStatus.Preparing);

            Assert.True(result.Success);
        }
        
        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnError_WhenInValidTransition()
        {
            var order = new OrderBasic { Id = 1, Status = OrderStatus.Pending };
            _orderRepositoryMock
                .Setup(repo => repo.GetBasicByIdAsync(1))
                .ReturnsAsync(order);

            var stateMock = new Mock<IOrderState>();
            stateMock
                .Setup(state => state.Transition(OrderStatus.Preparing))
                .Returns((stateMock.Object, "some error"));
            
            _stateFactoryMock
                .Setup(factory => factory.CreateState(order))
                .Returns(stateMock.Object);

            _orderRepositoryMock
                .Setup(repo => repo.UpdateStatusAsync(1, stateMock.Object, "user1", It.IsAny<byte[]>()))
                .ReturnsAsync(1);

            var result = await CreateSut().UpdateStatusAsync("user1", 1, OrderStatus.Preparing);

            Assert.False(result.Success);
            Assert.Equal(ServiceErrorCode.BadRequest, result.Error!.Value.Code);
        }
        
        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnError_WhenOrderNotFound()
        {
            _orderRepositoryMock.Setup(repo => repo.GetBasicByIdAsync(1)).ReturnsAsync((OrderBasic?)null);

            var result = await CreateSut().UpdateStatusAsync("user1", 1, OrderStatus.Preparing);

            Assert.False(result.Success);
            Assert.Equal(ServiceErrorCode.NotFound, result.Error!.Value.Code);
        }
        
        [Theory]
        [InlineData(0, ServiceErrorCode.NotFound)]
        [InlineData(2, ServiceErrorCode.Generic)]
        public async Task UpdateStatusAsync_ShouldReturnError_WhenUpdateCountNotOne(
            int modifiedCount,
            ServiceErrorCode expectedErrorCode)
        {
            var order = new OrderBasic { Id = 1, Status = OrderStatus.Pending };
            _orderRepositoryMock.Setup(repo => repo.GetBasicByIdAsync(1)).ReturnsAsync(order);

            var stateMock = new Mock<IOrderState>();
            stateMock
                .Setup(state => state.Transition(OrderStatus.Preparing))
                .Returns((stateMock.Object, null));
            _stateFactoryMock.Setup(factory => factory.CreateState(order)).Returns(stateMock.Object);

            _orderRepositoryMock
                .Setup(repo => repo.UpdateStatusAsync(1, stateMock.Object, "user1", It.IsAny<byte[]>()))
                .ReturnsAsync(modifiedCount);

            var result = await CreateSut().UpdateStatusAsync("user1", 1, OrderStatus.Preparing);

            Assert.False(result.Success);
            Assert.Equal(expectedErrorCode, result.Error!.Value.Code);
        }

        [Fact]
        public async Task SetDeliveryStatus_ShouldReturnSuccess_WhenValidDeliveryStaffId()
        {
            var order = new OrderBasic { Id = 1, Status = OrderStatus.ReadyForDelivery };
            _orderRepositoryMock.Setup(repo => repo.GetBasicByIdAsync(1)).ReturnsAsync(order);

            var stateMock = new Mock<IOrderState>();
            stateMock.Setup(state => state.SetDeliveryStaffId("staff123")).Returns((stateMock.Object, null));
            _stateFactoryMock.Setup(factory => factory.CreateState(order)).Returns(stateMock.Object);

            _orderRepositoryMock.Setup(repo => repo.SetDeliveryStaffAsync(1, stateMock.Object, "user1", It.IsAny<byte[]>()))
                .ReturnsAsync(1);

            var result = await CreateSut().SetDeliveryStatus("user1", 1, "staff123");

            Assert.True(result.Success);
        }
        
        [Fact]
        public async Task SetDeliveryStatus_ShouldReturnError_WhenCannotSetStaffId()
        {
            var order = new OrderBasic { Id = 1, Status = OrderStatus.ReadyForDelivery };
            _orderRepositoryMock.Setup(repo => repo.GetBasicByIdAsync(1)).ReturnsAsync(order);

            var stateMock = new Mock<IOrderState>();
            stateMock.Setup(state => state.SetDeliveryStaffId("staff123")).Returns((stateMock.Object, "some error"));
            _stateFactoryMock.Setup(factory => factory.CreateState(order)).Returns(stateMock.Object);

            _orderRepositoryMock
                .Setup(repo => repo.SetDeliveryStaffAsync(1, stateMock.Object, "user1", It.IsAny<byte[]>()))
                .ReturnsAsync(1);

            var result = await CreateSut().SetDeliveryStatus("user1", 1, "staff123");

            Assert.False(result.Success);
            Assert.Equal(ServiceErrorCode.BadRequest, result.Error!.Value.Code);
        }
        
        [Fact]
        public async Task SetDeliveryStatus_ShouldReturnError_WhenOrderNotFound()
        {
            _orderRepositoryMock.Setup(repo => repo.GetBasicByIdAsync(1)).ReturnsAsync((OrderBasic?)null);

            var result = await CreateSut().SetDeliveryStatus("user1", 1, "staff123");
            
            Assert.False(result.Success);
            Assert.Equal(ServiceErrorCode.NotFound, result.Error!.Value.Code);
        }
        
        [Theory]
        [InlineData(0, ServiceErrorCode.NotFound)]
        [InlineData(2, ServiceErrorCode.Generic)]
        public async Task SetDeliveryStaffAsync_ShouldReturnError_WhenUpdateCountNotOne(
            int modifiedCount,
            ServiceErrorCode expectedErrorCode)
        {
            var order = new OrderBasic { Id = 1, Status = OrderStatus.ReadyForDelivery };
            _orderRepositoryMock.Setup(repo => repo.GetBasicByIdAsync(1)).ReturnsAsync(order);

            var stateMock = new Mock<IOrderState>();
            stateMock.Setup(state => state.SetDeliveryStaffId("staff123")).Returns((stateMock.Object, null));
            _stateFactoryMock.Setup(factory => factory.CreateState(order)).Returns(stateMock.Object);

            _orderRepositoryMock
                .Setup(repo => repo.SetDeliveryStaffAsync(1, stateMock.Object, "user1", It.IsAny<byte[]>()))
                .ReturnsAsync(modifiedCount);

            var result = await CreateSut().SetDeliveryStatus("user1", 1, "staff123");

            Assert.False(result.Success);
            Assert.Equal(expectedErrorCode, result.Error!.Value.Code);
        }
    }
}