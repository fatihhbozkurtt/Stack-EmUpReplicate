using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class CoinStackHandler : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private GameObject coinPrefab;

    [SerializeField] private DisplayerCoinStack displayerStackPrefab;

    [SerializeField] private ColorEnumSo colorDataSo;

    [Header("Debug")] [SerializeField] private CellController parentCell;
    [SerializeField] private CoinStackData coinStackData;
    [HideInInspector] public bool forDisplay;

    private void Start()
    {
        if (forDisplay) return;

        parentCell = transform.GetComponentInParent<CellController>();
        parentCell.SetOccupied(this);

        AssignRandomData();
        SpawnCoins();
        SetValueText();
        SetCoinMaterials();
    }

    #region Data Assigning Region

    private void AssignRandomData()
    {
        coinStackData = DataExtensions.GetRandomStackData();
        coinStackData.coins = new();
        name = "Stack_" + coinStackData.colorEnum;
    }

    void AssignNewParentCell(CellController newParentCell)
    {
        parentCell.SetFree();
        parentCell = newParentCell;
        parentCell.SetOccupied(this);
        
        transform.SetParent(newParentCell.transform);
    }
    void SpawnCoins()
    {
        for (int i = 0; i < coinStackData.value; i++)
        {
            GameObject cloneCoin =
                Instantiate(coinPrefab,
                    transform.position + new Vector3(0, i * .1f, 0),
                    coinPrefab.transform.rotation,
                    transform);
            coinStackData.coins.Add(cloneCoin.transform);
        }
    }

    private void SetValueText(int givenValue = 0)
    {
        Transform lastCoin = coinStackData.coins[^1];
        TextMeshProUGUI txt = lastCoin.GetComponentInChildren<TextMeshProUGUI>(true);
        txt.transform.parent.gameObject.SetActive(true);
        txt.text = givenValue == 0 ? coinStackData.value.ToString() : givenValue.ToString();
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

    public void JumpToAnotherCell(CoinStackHandler targetCoinStack,
        List<CoinStackHandler> selectedCoinStack)
    {
        parentCell.SetFree();
        transform.DOJump(targetCoinStack.transform.position, 5, 1, 0.2f)
            .OnComplete(() =>
            {
                if (selectedCoinStack.IndexOf(targetCoinStack) == selectedCoinStack.Count - 1)
                {
                    OnJumpedLastCoinStack(targetCoinStack, selectedCoinStack);
                    return;
                }

                CoinStackHandler nextStack = selectedCoinStack[selectedCoinStack.IndexOf(targetCoinStack) + 1];
                targetCoinStack.JumpToAnotherCell(nextStack, selectedCoinStack);
                gameObject.SetActive(false);
            });
    }

    private void OnJumpedLastCoinStack(CoinStackHandler targetCoinStack, List<CoinStackHandler> selectedCoinStack)
    {
        int totalValue = 0;
        CoinStackHandler last = selectedCoinStack[selectedCoinStack.IndexOf(targetCoinStack)];
        foreach (var csh in selectedCoinStack)
        {
            if (csh == last) continue;

            totalValue += csh.GetStackData().value;
            Destroy(csh.gameObject);
        }

        last.IncrementValue(totalValue);
    }

    private void OnValueEqualsZero()
    {
        parentCell.SetFree();
        transform.DOScale(Vector3.zero, .25f)
            .OnComplete(() => Destroy(gameObject));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void IncrementValue(int additionalValue)
    {
        coinStackData.value += additionalValue;
        SetValueText();

        if (coinStackData.value < 10) return;

        PerformBlast();
    }

    private async UniTask PerformBlast()
    {
        var blastCount = coinStackData.value / 10;
        var myPos = transform.position;
        var mode = coinStackData.value % 10;
        bool destroySelf = false;

        #region Incrementing Same Colored Stack

        CoinStackHandler targetSameColoredStack =
            DataExtensions.GetSpecificStackByColor(coinStackData.colorEnum, this);
        if (targetSameColoredStack)
        {
            for (int j = 0; j < mode; j++)
            {
                await UniTask.WaitForSeconds(0.1f);
                targetSameColoredStack.ManuallyAddCoin(myPos);
            }

            destroySelf = true;
        }
        else
        {
            coinStackData.value = mode;
            SetValueText();
        }

        await UniTask.WaitForSeconds(0.1f);

        #endregion

        Debug.LogWarning("Blast count: " + blastCount);
        for (var i = 0; i < blastCount; i++)
        {
            #region Triggering Neighbors And Add Coins

            foreach (var cell in parentCell.neighbours)
            {
                if (cell.GetCoinStackObj() == null) continue;
                CoinStackHandler neighborStack = cell.GetCoinStackObj();
                await UniTask.WaitForSeconds(0.1f);
                neighborStack.ManuallyAddCoin(myPos);
            }

            #endregion

            #region Instantiating Clone Stacks

            await UniTask.WaitForSeconds(i == 0 ? 0 : 0.35f);

            DisplayerCoinStack cloneDisplayerStack =
                Instantiate(displayerStackPrefab,
                    new Vector3(myPos.x, myPos.y + 10, myPos.z),
                    Quaternion.identity);

            cloneDisplayerStack.Initialize(GetMaterialFromColorSo(coinStackData.colorEnum));

            #endregion
        }

        if (destroySelf)
            OnValueEqualsZero();

        FeedManager.instance.OnBlastEnded();
    }

    private void ManuallyAddCoin(Vector3 originPos)
    {
        var txt = coinStackData.coins[^1].GetComponentInChildren<TextMeshProUGUI>(true);
        txt.transform.parent.gameObject.SetActive(true);

        var cloneCoin = Instantiate(coinPrefab, originPos,
            coinPrefab.transform.rotation,
            transform);

        var targetPos = transform.position + new Vector3(0, coinStackData.coins.Count * .1f, 0);
        cloneCoin.transform.DOJump(targetPos, 5, 1, .35f);
        coinStackData.coins.Add(cloneCoin.transform);

        coinStackData.value++;
        SetValueText();
        SetCoinMaterials();
    }

    public void MoveAnotherCell(CellController newParentCell)
    {
        AssignNewParentCell(newParentCell);
        transform.DOMove(newParentCell.GetCenter(), 0.25f);
    }
}

[System.Serializable]
public class CoinStackData
{
    public int value;
    public ColorEnum colorEnum;
    public List<Transform> coins;
}