using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace EssentialManagers.Scripts
{
    public class InputManager : MonoSingleton<InputManager>
    {
        #region Base

        public event Action TouchStartEvent;
        public event Action TouchEndEvent;

        public void OnPointerDown()
        {
            TouchStartEvent?.Invoke();
        }

        public void OnPointerUp()
        {
            TouchEndEvent?.Invoke();
        }

        #endregion

        [Header("References")] [SerializeField]
        private LineRenderer lrPrefab;

        [Header("Debug")] [SerializeField] private List<CellController> cells = new();
        [SerializeField] private List<CoinStackHandler> selectedCoinStacks = new();
        private bool _isDragging = false;
        private LineRenderer _currentLine;
        private const float LineVerticalOffset = 0.3f;

        void Update()
        {
            // Check for mouse down event
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.TryGetComponent(out CellController cell)
                        && cell.GetCoinStackObj() != null)
                    {
                        selectedCoinStacks.Clear(); // Clear the list before starting a new drag
                        cells.Clear(); // Clear the list before starting a new drag

                        cells.Add(cell);
                        selectedCoinStacks.Add(cell.GetCoinStackObj());
                        _isDragging = true;

                        Vector3 cellPos = cell.transform.position + Vector3.up * LineVerticalOffset;
                        _currentLine = Instantiate(lrPrefab, cellPos, Quaternion.identity, transform);
                        _currentLine.positionCount = 1;
                        _currentLine.SetPosition(0, cellPos);
                    }
                }
            }

            // Check for mouse drag event
            if (Input.GetMouseButton(0) && _isDragging)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.TryGetComponent(out CellController cell)
                        && cell.GetCoinStackObj() != null)
                    {
                        if (selectedCoinStacks.Contains(cell.GetCoinStackObj())) return;
                        if (selectedCoinStacks[^1].GetStackData().colorEnum !=
                            cell.GetCoinStackObj().GetStackData().colorEnum) return;
                        if (!cells[^1].neighbours.Contains(cell)) return;

                        cells.Add(cell);
                        selectedCoinStacks.Add(cell.GetCoinStackObj());
                        
                        _currentLine.positionCount = cells.Count;
                        _currentLine.SetPosition(cells.Count - 1,
                            cell.transform.position + Vector3.up * LineVerticalOffset);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && _isDragging)
            {
                if (selectedCoinStacks.Count > 1)
                {
                    selectedCoinStacks[0]
                        .JumpToAnotherCell(selectedCoinStacks[1]
                            , new List<CoinStackHandler>(selectedCoinStacks));
                }

                _isDragging = false;
                selectedCoinStacks.Clear();
                cells.Clear();
                Destroy(_currentLine.gameObject);
            }
        }
    }
}