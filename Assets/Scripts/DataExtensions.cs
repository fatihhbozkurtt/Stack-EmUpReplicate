using System;
using UnityEngine;
using Random = UnityEngine.Random;
public static class DataExtensions
{
    public static CoinStackData GetRandomStackData()
    {
        var csd = new CoinStackData
        {
            value = Random.Range(1, 5),
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
}