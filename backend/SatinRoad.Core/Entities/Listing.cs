using LinqToDB.Mapping;

namespace SatinRoad.Core.Entities;

[Table("listings")]
public class Listing
{
    [Column("id"), PrimaryKey, Identity] public int Id { get; set; }
    [Column("vendor_id"), NotNull]       public int VendorId { get; set; }
    [Column("category_id"), NotNull]     public int CategoryId { get; set; }
    [Column("title"), NotNull]           public string Title { get; set; } = "";
    [Column("description")]              public string? Description { get; set; }
    [Column("price"), NotNull]           public decimal Price { get; set; }
    [Column("stock"), NotNull]           public int Stock { get; set; }
    [Column("is_active"), NotNull]       public bool IsActive { get; set; } = true;
    [Column("created_at"), NotNull]      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}