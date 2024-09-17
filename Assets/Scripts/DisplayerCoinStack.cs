using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DisplayerCoinStack : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private TextMeshProUGUI txt;

    [SerializeField] private List<Transform> coins;

    public void Initialize( Material targetMat)
    {
        txt.text = 10.ToString();
        foreach (Transform coin in coins)
        {
            coin.GetComponent<MeshRenderer>().material =
                targetMat;
        }

        transform.DOScale(Vector3.one * 2, 0.35f)
            .OnComplete((() => transform.DOScale(Vector3.zero, 0.35f)
                .OnComplete((() => Destroy(gameObject)))));
    }
}