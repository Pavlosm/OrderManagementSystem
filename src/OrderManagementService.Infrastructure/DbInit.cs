using Microsoft.EntityFrameworkCore.Migrations;
using OrderManagementService.Core.Entities;

namespace OrderManagementService.Infrastructure;

public static class DbInit
{
    public static void Seed(MigrationBuilder migrationBuilder)
    {
        var categoriesCols = new string[]
        {
           nameof(Category.Name), nameof(Category.CreatedAt), nameof(Category.CreatedBy)
        };
        
        var menuItemCols = new string[]
        {
            nameof(MenuItem.Name), nameof(MenuItem.Price), nameof(MenuItem.Deleted), 
            nameof(MenuItem.CreatedAt), nameof(MenuItem.CreatedBy)
        };
        
        var categoryMenuItemCols = new[]
        {
            "CategoriesId", "MenuItemsId"
        };
        
        var userCols = new[]
        {
            "Id", "UserName", "Email", "EmailConfirmed", "SecurityStamp", "AccessFailedCount", 
            "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled"
        };
        
        var roleCols = new[]
        {
            "Id", "Name", "NormalizedName"
        };

        var adminId = Guid.NewGuid().ToString();
        
        migrationBuilder.InsertData(
            "AspNetUsers",
            userCols,
            [
                adminId, "superuser", "superuser@gmail.com,", true, Guid.NewGuid().ToString(), 0, 
                true, false, false
            ]);
        
        migrationBuilder.InsertData(
            "AspNetRoles", 
            roleCols,
            [Role.Admin, Role.Admin, Role.Admin]);
        migrationBuilder.InsertData(
            "AspNetRoles",
            roleCols,
            [Role.Cook, Role.Cook, Role.Cook]);
        migrationBuilder.InsertData(
            "AspNetRoles", 
            roleCols,
            [Role.DeliveryMan, Role.DeliveryMan, Role.DeliveryMan]);

        migrationBuilder.InsertData(
            "Categories", 
            categoriesCols, 
            ["Appetizer", DateTime.UtcNow, adminId]);
        migrationBuilder.InsertData(
            "Categories",
            categoriesCols, 
            ["Main Course", DateTime.UtcNow, adminId]);
        migrationBuilder.InsertData(
            "Categories", categoriesCols, 
            ["Desert", DateTime.UtcNow, adminId]);
        migrationBuilder.InsertData(
            "Categories", 
            categoriesCols, 
            ["Beverage", DateTime.UtcNow, adminId]);
        
        migrationBuilder.InsertData(
            "MenuItem",
            menuItemCols,
            ["Cheeseburger", 5.99m, true, DateTime.UtcNow, adminId]);
        migrationBuilder.InsertData(
            "MenuItem", 
            menuItemCols,
            ["Pizza", 7.50, true, DateTime.UtcNow, adminId]);
        migrationBuilder.InsertData(
            "MenuItem", 
            menuItemCols,
            ["Donut", 3.20m, true, DateTime.UtcNow, adminId]);
        migrationBuilder.InsertData(
            "MenuItem", 
            menuItemCols,
            ["Water", 3.20m, true, DateTime.UtcNow, adminId]);
        migrationBuilder.InsertData(
            "MenuItem", 
            menuItemCols,
            ["Coke", 3.20m, true, DateTime.UtcNow, adminId]);
        
        migrationBuilder.InsertData("CategoryMenuItem", categoryMenuItemCols, [2, 1]);
        migrationBuilder.InsertData("CategoryMenuItem", categoryMenuItemCols, [2, 2]);
        migrationBuilder.InsertData("CategoryMenuItem", categoryMenuItemCols, [3, 3]);
        migrationBuilder.InsertData("CategoryMenuItem", categoryMenuItemCols, [4, 4]);
        migrationBuilder.InsertData("CategoryMenuItem", categoryMenuItemCols, [4, 5]);
    }
}