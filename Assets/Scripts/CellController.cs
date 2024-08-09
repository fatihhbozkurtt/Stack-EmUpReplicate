using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Transform cellGround;

    [Header("Debug")] public bool isPickable;
    [SerializeField] Vector2Int coordinates;
    [SerializeField] List<CellController> neighbours;


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
        if (!isPickable) return;
        if (!GameManager.instance.isLevelActive) return;

        cellGround.SetParent(null);
    }


    #region GETTERS & SETTERS

    private void SetFree()
    {
        isPickable = true;
    }

    public Vector2Int GetCoordinates()
    {
        return coordinates;
    }

    private List<CellController> GetNeighbors()
    {
        List<CellController> gridCells = GridManager.instance.gridPlan;
        List<CellController> neighbors = new();

        int[] dx = { 1, 0, -1, 0 };
        int[] dz = { 0, 1, 0, -1 };

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

    #endregion
}