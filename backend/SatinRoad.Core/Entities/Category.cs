using LinqToDB.Mapping;

namespace SatinRoad.Core.Entities;

[Table("categories")]
public class Category
{
    [Column("id"), PrimaryKey, Identity] public int Id { get; set; }
    [Column("name"), NotNull]            public string Name { get; set; } = "";
}