using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ColorEnumData", order = 1)]
public class ColorEnumSo : ScriptableObject
{
    public List<ColorDataWrappers> colorDataWrappers;
}
[System.Serializable]
public class ColorDataWrappers
{
    public ColorEnum colorEnum;
    public Material material;
}