using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CoinStackHandler : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private GameObject coinPrefab;

    [SerializeField] private ColorEnumSo colorDataSo;

    [Header("Debug")] [SerializeField] private CellController parentCell;
    [SerializeField] private CoinStackData coinStackData;

    private void Start()
    {
        parentCell = transform.GetComponentInParent<CellController>();
        parentCell.SetOccupied(this);

        AssignRandomData();
        SpawnCoins();
        SetValueText();
        SetCoinMaterials();
    }

    public void Disappear()
    {
        parentCell.SetFree();
        Destroy(gameObject);
    }

    public void IncrementValue(int additionalValue)
    {
        coinStackData.value += additionalValue;
        SetValueText();
        
    }

    #region Data Assigning Region

    private void AssignRandomData()
    {
        coinStackData = DataExtensions.GetRandomStackData();
        coinStackData.coins = new();
        name = "Stack_" + coinStackData.colorEnum;
    }

    void SpawnCoins()
    {
        for (int i = 0; i < coinStackData.value; i++)
        {
            GameObject cloneCoin =
                Instantiate(coinPrefab, transform.position + new Vector3(0, i * .1f, 0), coinPrefab.transform.rotation,
                    transform);
            coinStackData.coins.Add(cloneCoin.transform);
        }
    }

    private void SetValueText()
    {
        Transform lastCoin = coinStackData.coins[^1];
        TextMeshProUGUI txt = lastCoin.GetComponentInChildren<TextMeshProUGUI>(true);
        txt.transform.parent.gameObject.SetActive(true);
        txt.text = coinStackData.value.ToString();
    }

    private void SetCoinMaterials()
    {
        foreach (Transform coin in coinStackData.coins)
        {
            coin.GetComponent<MeshRenderer>().material =
                GetMaterialFromColorSo(coinStackData.colorEnum);
        }
    }


    private Material GetMaterialFromColorSo(ColorEnum targetColorEnum)
    {
        Material mat = null;

        foreach (var cdw in colorDataSo.colorDataWrappers)
        {
            if (cdw.colorEnum == targetColorEnum)
            {
                mat = cdw.material;
            }
        }

        return mat;
    }

    public CoinStackData GetStackData()
    {
        return coinStackData;
    }

    #endregion
}

[System.Serializable]
public class CoinStackData
{
    public int value;
    public ColorEnum colorEnum;
    public List<Transform> coins;
}