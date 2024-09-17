using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Transform cellGround;

    [SerializeField] private CoinStackHandler coinStackHandler;

    [Header("Debug")] public bool isPickable;
    public bool isOccupied;
    [SerializeField] private CoinStackHandler csh;
    [SerializeField] Vector2Int coordinates;
    public List<CellController> neighbours;

    private void Start()
    {
        name = coordinates.ToString();

        neighbours = GetNeighbors();
    }

    public void Initialize(Vector2Int initCoords)
    {
        coordinates = initCoords;
    }

    private void OnMouseDown()
    {
        //    if (!isPickable) return;
        if (!GameManager.instance.isLevelActive) return;

        //  csh.gameObject.SetActive(false);
    }

    #region GETTERS & SETTERS

    public void SetOccupied(CoinStackHandler _csh)
    {
        csh = _csh;
        isOccupied = true;
    }

    public void SetFree()
    {
        csh = null;
        isOccupied = false;
    }

    public CoinStackHandler GetCoinStackObj()
    {
        return csh;
    }

    public Vector2Int GetCoordinates()
    {
        return coordinates;
    }

    private List<CellController> GetNeighbors()
    {
        List<CellController> gridCells = GridManager.instance.gridPlan;
        List<CellController> neighbors = new();

        // Direction vectors for 8 directions (including diagonals)
        int[] dx = { 1, 1, 0, -1, -1, -1, 0, 1 };
        int[] dz = { 0, 1, 1, 1, 0, -1, -1, -1 };

        for (int i = 0; i < dx.Length; i++)
        {
            Vector2Int neighborCoordinates = coordinates + new Vector2Int(dx[i], dz[i]);
            CellController neighbor = gridCells.Find(cell => cell.coordinates == neighborCoordinates);

            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public Vector3 GetCenter()
    {
        return transform.position + new Vector3(0, 0.3f, 0);
    }

    #endregion
}