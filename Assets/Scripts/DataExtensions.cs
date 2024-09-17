using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class DataExtensions
{
    public static CoinStackData GetRandomStackData()
    {
        var csd = new CoinStackData
        {
            value = Random.Range(1, 10),
            colorEnum = GetRandomColorEnum(),
        };

        return csd;
    }

    private static ColorEnum GetRandomColorEnum()
    {
        Array values = Enum.GetValues(typeof(ColorEnum));
        var index = Random.Range(1, values.Length); // Start from index 1 to exclude None

        return (ColorEnum)values.GetValue(index);
    }

    public static CoinStackHandler GetSpecificStackByColor(ColorEnum targetColor, CoinStackHandler callerStack)
    {
        CoinStackHandler csh = null;

        foreach (var cell in GridManager.instance.GetShuffledGridPlan())
        {
            if (!cell.GetCoinStackObj()) continue;
            if (cell.GetCoinStackObj().GetStackData().colorEnum != targetColor) continue;
            if (cell.GetCoinStackObj() == callerStack) continue;

            csh = cell.GetCoinStackObj();
            break;
        }

        return csh;
    }
}