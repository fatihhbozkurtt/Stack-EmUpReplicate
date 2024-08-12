using System;
using System.Collections.Generic;
using UnityEngine;

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

        [SerializeField] private List<CellController> cells = new();
        [SerializeField] private List<CoinStackHandler> selectedCoinStacks = new();
        private bool _isDragging = false;

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
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && _isDragging)
            {
                if (selectedCoinStacks.Count > 1)
                {
                    int totalValue = 0;
                    for (int i = 0; i < selectedCoinStacks.Count; i++)
                    {
                        if (i != selectedCoinStacks.Count - 1)
                        {
                            totalValue += selectedCoinStacks[i].GetStackData().value;
                            selectedCoinStacks[i].Disappear();
                        }
                        else
                        {
                            selectedCoinStacks[i].IncrementValue(totalValue);
                        }
                    }
                }

                _isDragging = false;
                selectedCoinStacks.Clear();  
                cells.Clear();  
            }
        }
    }
}