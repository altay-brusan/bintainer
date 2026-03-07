using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

using Dapper;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.GetStorageUnit;

internal sealed class GetStorageUnitQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetStorageUnitQuery, StorageUnitResponse>
{
    public async Task<Result<StorageUnitResponse>> Handle(GetStorageUnitQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                su.id AS Id, su.name AS Name, su.columns AS Columns, su.rows AS Rows,
                su.compartment_count AS CompartmentCount, su.inventory_id AS InventoryId,
                b.id AS Id, b.column AS Column, b.row AS Row,
                c.id AS Id, c.index AS Index, c.label AS Label,
                c.component_id AS ComponentId, p.part_number AS ComponentPartNumber, c.quantity AS Quantity
            FROM inventory.storage_units su
            LEFT JOIN inventory.bins b ON b.storage_unit_id = su.id
            LEFT JOIN inventory.compartments c ON c.bin_id = b.id
            LEFT JOIN inventory.components p ON p.id = c.component_id
            WHERE su.id = @StorageUnitId
            ORDER BY b.column, b.row, c.index
            """;

        var storageUnitDictionary = new Dictionary<Guid, StorageUnitResponse>();
        var binDictionary = new Dictionary<Guid, BinResponse>();

        await connection.QueryAsync<StorageUnitRow, BinRow?, CompartmentRow?, StorageUnitResponse>(
            sql,
            (su, bin, compartment) =>
            {
                if (!storageUnitDictionary.TryGetValue(su.Id, out var storageUnit))
                {
                    storageUnit = new StorageUnitResponse(
                        su.Id, su.Name, su.Columns, su.Rows,
                        su.CompartmentCount, su.InventoryId, []);
                    storageUnitDictionary[su.Id] = storageUnit;
                }

                if (bin is not null && !binDictionary.ContainsKey(bin.Id))
                {
                    var binResponse = new BinResponse(bin.Id, bin.Column, bin.Row, []);
                    binDictionary[bin.Id] = binResponse;
                    storageUnit.Bins.Add(binResponse);
                }

                if (bin is not null && compartment is not null && binDictionary.TryGetValue(bin.Id, out var existingBin))
                {
                    existingBin.Compartments.Add(new CompartmentResponse(
                        compartment.Id, compartment.Index, compartment.Label,
                        compartment.ComponentId, compartment.ComponentPartNumber, compartment.Quantity));
                }

                return storageUnit;
            },
            new { request.StorageUnitId },
            splitOn: "Id,Id");

        var result = storageUnitDictionary.Values.FirstOrDefault();

        if (result is null)
        {
            return Result.Failure<StorageUnitResponse>(StorageUnitErrors.NotFound(request.StorageUnitId));
        }

        return result;
    }

    private sealed record StorageUnitRow(Guid Id, string Name, int Columns, int Rows, int CompartmentCount, Guid InventoryId);
    private sealed record BinRow(Guid Id, int Column, int Row);
    private sealed record CompartmentRow(Guid Id, int Index, string Label, Guid? ComponentId, string? ComponentPartNumber, int Quantity);
}
