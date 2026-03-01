using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Modules.Inventory.Application.StorageUnits.GetStorageUnit;

namespace Bintainer.Modules.Inventory.Application.UnitTests.StorageUnits.GetStorageUnit;

public class GetStorageUnitQueryHandlerTests
{
    private readonly IDbConnectionFactory _dbConnectionFactory = Substitute.For<IDbConnectionFactory>();

    [Fact]
    public void Handler_CanBeConstructed()
    {
        var handler = new GetStorageUnitQueryHandler(_dbConnectionFactory);

        handler.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_CallsOpenConnectionAsync()
    {
        var connection = Substitute.For<DbConnection>();
        _dbConnectionFactory.OpenConnectionAsync().Returns(new ValueTask<DbConnection>(connection));

        var handler = new GetStorageUnitQueryHandler(_dbConnectionFactory);
        var query = new GetStorageUnitQuery(Guid.NewGuid());

        // The handler will throw because the mock connection can't execute SQL,
        // but we verify it at least calls OpenConnectionAsync
        try
        {
            await handler.Handle(query, CancellationToken.None);
        }
        catch
        {
            // Expected - mock connection can't execute Dapper SQL
        }

        await _dbConnectionFactory.Received(1).OpenConnectionAsync();
    }
}
