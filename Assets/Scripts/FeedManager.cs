using System.Collections.Generic;
using UnityEngine;

public class FeedManager : MonoSingleton<FeedManager>
{
    [SerializeField] List<CellController> emptyCells = new List<CellController>();

    public void OnBlastEnded()
    {
        #region Detect Empty Cells

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
                GridManager.instance.GetCellsOnTheSameColumn(emptyCell.GetCoordinates().x);

            for (var i = 0; i < occupiedCellsOnSameColumn.Count; i++)
            {
                CoinStackHandler movableCsh = occupiedCellsOnSameColumn[i].GetCoinStackObj();
                CellController targetCellToMove = GridManager.instance
                    .GetGridCellByCoordinates(new Vector2Int(emptyCell.GetCoordinates().x, i));
                occupiedCellsOnSameColumn[i].SetFree();
                movableCsh.MoveAnotherCell(targetCellToMove);
            }
        }

        #endregion

        // #region Add New CSH
        //w
        // #endregion
    }
}