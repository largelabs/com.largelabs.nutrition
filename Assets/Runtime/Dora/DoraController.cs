using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoraController : MonoBehaviourBase
{
    [SerializeField] DoraCellMap defaultCellMap = null;
    [SerializeField] DoraCellSelector cellSelector = null;
    [SerializeField] KernelSpawner kernelSpawner = null;
    [SerializeField] DoraScoreManager scoreManager = null;

    DoraActions inputActions = null;
    DoraCellMap cellMap = null;
    Coroutine moveRoutine = null;
    Coroutine eatRoutine = null;


    #region UNITY AND CORE

    void Start ()
    {
        base.Awake();

        initInputs();
        EnableController();

        SetCellMap(defaultCellMap);
    }

    #endregion

    #region PUBLIC API

    [ExposePublicMethod]
    public void EnableController()
    {
        inputActions.Player.Move.Enable();
    }

    [ExposePublicMethod]
    public void DisableController()
    {
        this.DisposeCoroutine(ref moveRoutine);
        inputActions.Player.Move.Disable();
        cellSelector.ClearSelection();
    }

    public void SetCellMap(DoraCellMap i_cellMap)
    {
        cellMap = i_cellMap;
        if (null == cellMap) DisableController();
        cellSelector.SetCellMap(cellMap);
    }

    #endregion

    #region PRIVATE

    IEnumerator dispatchEatRoutine()
    {
        while (true)
        {
            Vector2Int? currentSelect = cellSelector.CurrentOriginCell;
            if (null == currentSelect)
            {
                this.DisposeCoroutine(ref eatRoutine);
                yield break;
            }

            int selectRadius = 0;

            while (true == inputActions.Player.Eat.IsPressed())
            {
                if(selectRadius <= cellSelector.MaxSelectionRadius)
                {
                    cellSelector.SelectRange(cellSelector.CurrentOriginCell.Value, selectRadius, true, false, false);
                    yield return this.Wait(0.25f);
                    selectRadius++;
                }
                else
                {
                    yield return null;
                }

            }

            yield return null;
        }
    }

    IEnumerator dispatchMoveRoutine()
    {
        while (true)
        {
            Vector2Int? currentSelect = cellSelector.CurrentOriginCell;
            if (null == currentSelect)
            {
                this.DisposeCoroutine(ref moveRoutine);
                yield break;
            }

            Vector2 inputValue = inputActions.Player.Move.ReadValue<Vector2>();

            Vector2Int nextSelect = currentSelect.Value;
            nextSelect.y += (int)inputValue.x;
            nextSelect.x += (int)inputValue.y;

            cellSelector.SelectCell(nextSelect, true, true);

            yield return this.Wait(0.2f);
        }
    }

    void initInputs()
    {
        if (null == inputActions) inputActions = new DoraActions();
        inputActions.Player.Move.started += onMoveStarted;
        inputActions.Player.Move.canceled += onMoveCanceled;

        inputActions.Player.Eat.started += onEatStarted;
        inputActions.Player.Eat.canceled += onEatCanceled;
    }

    private void onEatStarted(InputAction.CallbackContext obj)
    {
        if (null != eatRoutine) return;
        inputActions.Player.Move.Disable();

        Debug.Log("pressed");

        eatRoutine = StartCoroutine(dispatchEatRoutine());
    }

    private void onEatCanceled(InputAction.CallbackContext obj)
    {
        inputActions.Player.Move.Enable();

        Debug.Log("released");

        eatKernels();

        this.DisposeCoroutine(ref eatRoutine);
    }

    private void onMoveStarted(InputAction.CallbackContext obj)
    {
        if (null != moveRoutine) return;
        inputActions.Player.Eat.Disable();
        moveRoutine = StartCoroutine(dispatchMoveRoutine());
    }

    private void onMoveCanceled(InputAction.CallbackContext obj)
    {
        inputActions.Player.Eat.Enable();
        this.DisposeCoroutine(ref moveRoutine);
    }

    private void eatKernels()
    {
        Dictionary<Vector2Int, DoraCellData> cellsDictionary = cellSelector.SelectedRange;
        int burntKenrelsCount = 0;
        foreach (KeyValuePair<Vector2Int, DoraCellData> pair in cellsDictionary)
        {
            DoraCellData cell = pair.Value;
            if (true == cell.KernelIsBurnt()) burntKenrelsCount++;
            kernelSpawner.DespawnKernel(cell.Kernel);
            cell.Reset();
        }

        scoreManager.AddScore(cellsDictionary.Count - burntKenrelsCount);
        scoreManager.RemoveScore(burntKenrelsCount);
    }

    #endregion


}
