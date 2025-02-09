using Microsoft.EntityFrameworkCore.Migrations;
using OrderManagementService.Core.Entities;

namespace OrderManagementService.Infrastructure;

public static class DbInit
{
    public const string DbAdminId = "51b80095-79b1-42d4-9b43-ae3d7c27e318";

    public static void Seed(MigrationBuilder migrationBuilder)
    {
        var categoriesCols = new[]
        {
           nameof(Category.Name), nameof(Category.CreatedAt), nameof(Category.CreatedBy)
        };
        
        var menuItemCols = new[]
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
            "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "PasswordHash"
        };
        
        var roleCols = new[]
        {
            "Id", "Name", "NormalizedName"
        };
        
        const string adminId = "e7175586-af0b-47d7-b972-1ada1493d94c";
        const string delivery1Id = "863ff660-1aa5-4550-b6c7-7e687e501a45";
        const string delivery2Id = "bdfe46a2-9cc8-4d68-8ea5-cf923863be09";
        const string restaurant1Id = "9e4c32f0-3ed3-4746-8476-82a43334ac94";
        const string restaurant2Id = "a5ae0dcb-f562-405a-a90e-5946fd1c3230";
        
        migrationBuilder.InsertData(
            "AspNetUsers",
            userCols,
            [
                DbAdminId, "system", "syste@gmail.com", true, Guid.NewGuid().ToString(), 0, true, false, false,
                null
            ]);
        
        migrationBuilder.InsertData(
            "AspNetUsers",
            userCols,
            [
                adminId, "admin", "superuser@gmail.com", true, Guid.NewGuid().ToString(), 0, true, false, false,
                "AQAAAAIAAYagAAAAEBmXCC0Rwgq1RuH4yDKKdvgtmr6Yxxi56hMgCtTrqaWKjXDSD/7oDLBGSYEO6x7u0w=="
            ]);
        
        migrationBuilder.InsertData(
            "AspNetUsers",
            userCols,
            [
                delivery1Id, "delivery1", "delivery1@gmail.com", true, Guid.NewGuid().ToString(), 0, true, false, false,
                "AQAAAAIAAYagAAAAEGCmXu2cGPpsX8BitvTcFV/qyrEgb4REDzO1fqU8D3eZzMscexZvaJyu9lNimadXjA==",
            ]);
        
        migrationBuilder.InsertData(
            "AspNetUsers",
            userCols,
            [
                delivery2Id, "delivery2", "delivery2@gmail.com", true, Guid.NewGuid().ToString(), 0, true, false, false,
                "AQAAAAIAAYagAAAAEAn80QOBE/uuSqCQjZ/IrvWAA4cB8iddp+dbF+KAQ3WV7IPqXcrWNt3d3erGUwB3Zw==",
            ]);
        
        migrationBuilder.InsertData(
            "AspNetUsers",
            userCols,
            [
               restaurant1Id, "restaurant1", "restaurant1@gmail.com", true, Guid.NewGuid().ToString(), 0, true, false, false,
                "AQAAAAIAAYagAAAAEBTXoIG3gzWYTQ85UYDKaLne7mlFZfaNwl4rCWMrtKiaqyKiCPyuxT8HBzEB/aWr6A==",
            ]);
        
        migrationBuilder.InsertData(
            "AspNetUsers",
            userCols,
            [
                restaurant2Id, "restaurant2", "restaurant2@gmail.com", true, Guid.NewGuid().ToString(), 0, true, false, false,
                "AQAAAAIAAYagAAAAENNtZFytKBzypU6i797BNYCnko86IfCENPqsgGhMkr1fJEvN9MnHnN4vdiD656immw==",
            ]);
        
        migrationBuilder.InsertData(
            "AspNetRoles", 
            roleCols,
            [Role.Admin, Role.Admin, Role.Admin]);
        migrationBuilder.InsertData(
            "AspNetRoles",
            roleCols,
            [Role.ReasturantStuff, Role.ReasturantStuff, Role.ReasturantStuff]);
        migrationBuilder.InsertData(
            "AspNetRoles", 
            roleCols,
            [Role.DeliveryStaff, Role.DeliveryStaff, Role.DeliveryStaff]);

        migrationBuilder.InsertData("AspNetUserRoles", ["UserId", "RoleId"], [adminId, Role.Admin]);
        migrationBuilder.InsertData("AspNetUserRoles", ["UserId", "RoleId"], [delivery1Id, Role.DeliveryStaff]);
        migrationBuilder.InsertData("AspNetUserRoles", ["UserId", "RoleId"], [delivery2Id, Role.DeliveryStaff]);
        migrationBuilder.InsertData("AspNetUserRoles", ["UserId", "RoleId"], [restaurant1Id, Role.ReasturantStuff]);
        migrationBuilder.InsertData("AspNetUserRoles", ["UserId", "RoleId"], [restaurant2Id, Role.ReasturantStuff]);
        
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
            ["Cheeseburger", 5.99m, false, DateTime.UtcNow, adminId]);
        migrationBuilder.InsertData(
            "MenuItem", 
            menuItemCols,
            ["Pizza", 7.50, false, DateTime.UtcNow, adminId]);
        migrationBuilder.InsertData(
            "MenuItem", 
            menuItemCols,
            ["Donut", 3.20m, false, DateTime.UtcNow, adminId]);
        migrationBuilder.InsertData(
            "MenuItem", 
            menuItemCols,
            ["Water", 3.20m, false, DateTime.UtcNow, adminId]);
        migrationBuilder.InsertData(
            "MenuItem", 
            menuItemCols,
            ["Coke", 3.20m, false, DateTime.UtcNow, adminId]);
        
        migrationBuilder.InsertData("CategoryMenuItem", categoryMenuItemCols, [2, 1]);
        migrationBuilder.InsertData("CategoryMenuItem", categoryMenuItemCols, [2, 2]);
        migrationBuilder.InsertData("CategoryMenuItem", categoryMenuItemCols, [3, 3]);
        migrationBuilder.InsertData("CategoryMenuItem", categoryMenuItemCols, [4, 4]);
        migrationBuilder.InsertData("CategoryMenuItem", categoryMenuItemCols, [4, 5]);
    }
}