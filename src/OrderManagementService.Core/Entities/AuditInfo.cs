 namespace OrderManagementService.Core.Entities;

 /// <summary>
 /// Represents an entity that keeps track of creation and last update information.
 /// </summary>
 public abstract class AuditInfo
 {
     /// <summary>
     /// Gets or sets the identifier of the user who created the entity.
     /// </summary>
     public string CreatedBy { get; set; } = null!;

     /// <summary>
     /// Gets or sets the date and time when the entity was created.
     ///
     /// 
     /// </summary>
     public DateTime CreatedAt { get; set; }

     /// <summary>
     /// Gets or sets the identifier of the user who last updated the entity, if any.
     /// This should be a valid user id.
     /// </summary>
     public string? LastUpdatedBy { get; set; }

     /// <summary>
     /// Gets or sets the date and time when the entity was last updated, if any.
     /// </summary>
     public DateTime? LastUpdatedAt { get; set; }
 }