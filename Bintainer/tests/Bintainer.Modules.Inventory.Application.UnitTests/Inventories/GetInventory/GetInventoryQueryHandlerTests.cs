using System.Data;
using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Modules.Inventory.Application.Inventories.GetInventory;
using Microsoft.Data.Sqlite;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Inventories.GetInventory;

public class GetInventoryQueryHandlerTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly GetInventoryQueryHandler _handler;

    public GetInventoryQueryHandlerTests()
    {
        SqlMapper.RemoveTypeMap(typeof(Guid));
        SqlMapper.AddTypeHandler(new GuidAsStringHandler());

        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _connection.Execute("ATTACH ':memory:' AS inventory");
        _connection.Execute("""
            CREATE TABLE inventory.inventories (
                id TEXT PRIMARY KEY,
                name TEXT NOT NULL,
                user_id TEXT NOT NULL
            )
            """);

        _dbConnectionFactory = Substitute.For<IDbConnectionFactory>();
        _dbConnectionFactory.OpenConnectionAsync().Returns(new ValueTask<DbConnection>(_connection));

        _handler = new GetInventoryQueryHandler(_dbConnectionFactory);
    }

    [Fact]
    public async Task Handle_InventoryExists_ReturnsResponse()
    {
        var inventoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _connection.Execute(
            "INSERT INTO inventory.inventories (id, name, user_id) VALUES (@Id, @Name, @UserId)",
            new { Id = inventoryId.ToString(), Name = "Test Inventory", UserId = userId.ToString() });

        var query = new GetInventoryQuery(inventoryId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Test Inventory");
    }

    [Fact]
    public async Task Handle_InventoryNotFound_ReturnsFailure()
    {
        var query = new GetInventoryQuery(Guid.NewGuid());
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Inventories.NotFound");
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    private sealed class GuidAsStringHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToString();
            parameter.DbType = DbType.String;
        }

        public override Guid Parse(object value)
        {
            return Guid.Parse(value.ToString()!);
        }
    }
}
