using LinqToDB.Mapping;

namespace SatinRoad.Core.Entities;

[Table("orders")]
public class Order
{
    [Column("id"), PrimaryKey, Identity] public int Id { get; set; }
    [Column("buyer_id"), NotNull]        public int BuyerId { get; set; }
    [Column("vendor_id"), NotNull]       public int VendorId { get; set; }
    [Column("listing_id"), NotNull]      public int ListingId { get; set; }
    [Column("quantity"), NotNull]        public int Quantity { get; set; }
    [Column("unit_price"), NotNull]      public decimal UnitPrice { get; set; }
    [Column("discount_applied"), NotNull] public bool DiscountApplied { get; set; }
    [Column("total"), NotNull]           public decimal Total { get; set; }
    [Column("created_at"), NotNull]      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}