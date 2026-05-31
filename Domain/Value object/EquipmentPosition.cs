using Domain.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Value_object;

public sealed class EquipmentPosition
{
    public int RowNumber { get; }
    public int SpotNumber { get; }
    public EquipmentPosition(int rowNumber, int spotNumber)
    {
        if (rowNumber < 1) throw new DomainException("Row number must be greater than 0.");
        if (spotNumber < 1) throw new DomainException("Spot number must be greater than 0.");

        RowNumber = rowNumber;
        SpotNumber = spotNumber;
    }
}