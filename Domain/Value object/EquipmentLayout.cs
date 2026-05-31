using Domain.Entity;
using Domain.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Value_object;

public sealed record class EquipmentLayout
{
    public int RowCount { get; }
    public int SpotsPerRow { get; }

    public EquipmentLayout(int rowCount, int spotsPerRow)
    {
        if (rowCount < 1) throw new DomainException("Row count must be greater than 0.", nameof(rowCount));
        if (spotsPerRow < 1) throw new DomainException("Spots per row must be greater than 0.", nameof(spotsPerRow));

        RowCount = rowCount;
        SpotsPerRow = spotsPerRow;
    }
}