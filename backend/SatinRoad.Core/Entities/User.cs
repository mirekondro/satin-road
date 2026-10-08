using LinqToDB.Mapping;

namespace SatinRoad.Core.Entities;

[Table("users")]
public class User
{
    [Column("id"), PrimaryKey, Identity] public int Id { get; set; }
    [Column("username"), NotNull]        public string Username { get; set; } = "";
    [Column("password_hash"), NotNull]   public string PasswordHash { get; set; } = "";
    [Column("role"), NotNull]            public string Role { get; set; } = "User";
    [Column("is_shut_down"), NotNull]    public bool IsShutDown { get; set; }
    [Column("created_at"), NotNull]      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}