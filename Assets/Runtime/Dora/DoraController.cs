using UnityEngine;

public class DoraController : MonoBehaviourBase
{
    [SerializeField] DoraCellMap defaultCellMap = null;
    [SerializeField] DoraCellSelector cellSelector = null;

    DoraCellMap cellMap = null;

    #region UNITY AND CORE

    void Start ()
    {
        base.Awake();
        SetCellMap(defaultCellMap);
    }

    #endregion

    #region PUBLIC API

    public void SetCellMap(DoraCellMap i_cellMap)
    {
        cellMap = i_cellMap;
        if (null == cellMap) DisableController();
        cellSelector.SetCellMap(cellMap);
    }

    public void EnableController()
    {

    }

    public void DisableController()
    {
        cellSelector.ClearSelection();
    }

    #endregion


}
