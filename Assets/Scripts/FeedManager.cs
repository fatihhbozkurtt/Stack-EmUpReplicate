using System.Collections.Generic;
using UnityEngine;

public class FeedManager : MonoSingleton<FeedManager>
{
    [SerializeField] List<CellController> emptyCells = new();

    public void OnBlastEnded()
    {
        #region Detect Empty Cells

        GridManager gridManager = GridManager.instance;

        List<CellController> gridPlan = GridManager.instance.gridPlan;

        foreach (CellController cell in gridPlan)
        {
            if (cell.isOccupied) continue;
            if (emptyCells.Contains(cell)) continue;

            emptyCells.Add(cell);
        }

        #endregion

        #region Move Down Target CSH

        List<int> appliedColumnIndex = new List<int>();
        foreach (var emptyCell in emptyCells)
        {
            if (appliedColumnIndex.Contains(emptyCell.GetCoordinates().x)) continue;

            appliedColumnIndex.Add(emptyCell.GetCoordinates().x);
            List<CellController> occupiedCellsOnSameColumn =
                gridManager.GetCellsOnTheSameColumn(emptyCell.GetCoordinates().x);

            for (var i = 0; i < occupiedCellsOnSameColumn.Count; i++)
            {
                CoinStackHandler movableCsh = occupiedCellsOnSameColumn[i].GetCoinStackObj();
                CellController targetCellToMove = gridManager
                    .GetGridCellByCoordinates(new Vector2Int(emptyCell.GetCoordinates().x, i));
                occupiedCellsOnSameColumn[i].SetFree();
                movableCsh.MoveAnotherCell(targetCellToMove);
            }
        }

        #endregion

        #region Add New CSH

        for (int rowIndex = 0; rowIndex < gridManager.gridHeight; rowIndex++)
        {
            List<CellController> column = gridManager.GetCellsOnTheSameColumn(rowIndex);
            int emptyCellCount = 0;
            foreach (CellController cell in column)
            {
                if (cell.isOccupied) continue;

                emptyCellCount++;
            }

            for (int y = 0; y < emptyCellCount; y++)
            {
                Vector2Int epmtyCellCoordinate = new Vector2Int(rowIndex,
                    gridManager.gridHeight - emptyCellCount + y);

                CellController cell = gridManager.GetGridCellByCoordinates(epmtyCellCoordinate);
                cell.InstantiateCSH();
            }
        }

        #endregion
    }
}

[System.Serializable]
public class ColumnCellData
{
}