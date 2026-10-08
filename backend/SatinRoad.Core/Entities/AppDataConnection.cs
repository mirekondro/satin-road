using LinqToDB;
using LinqToDB.Data;
using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Data;

public class AppDataConnection : DataConnection
{
    public AppDataConnection(DataOptions<AppDataConnection> options) : base(options.Options) { }

    public ITable<User>     Users      => this.GetTable<User>();
    public ITable<Category> Categories => this.GetTable<Category>();
    public ITable<Listing>  Listings   => this.GetTable<Listing>();
    public ITable<Order>    Orders     => this.GetTable<Order>();
}