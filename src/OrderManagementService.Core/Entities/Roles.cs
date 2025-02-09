using System.Collections.Frozen;

namespace OrderManagementService.Core.Entities;

public class Role
{
    public const string Admin = "Admin";
    public const string DeliveryStaff = "DeliveryStuff";
    public const string ReasturantStuff = "ReasturantStuff";
    
    public static readonly FrozenSet<string> AllRoles = new[]{ Admin, DeliveryStaff, ReasturantStuff }.ToFrozenSet();
    public static readonly FrozenSet<string> AllValidRegistrationRoles = new[]{ Admin, DeliveryStaff, ReasturantStuff }.ToFrozenSet();

}