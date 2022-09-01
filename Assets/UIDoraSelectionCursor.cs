
using UnityEngine;
using UnityEngine.UI;

public class UIDoraSelectionCursor : MonoBehaviourBase
{
    [SerializeField] bool updateCursorPositionY = true;
    [SerializeField] RectTransform canvasRect = null;
    [SerializeField] Image cursorImage = null;
    [SerializeField] RectTransform cursorRect = null;
    [SerializeField] DoraController controller = null;

    Vector3[] extentPointsBuffer = null;

    #region UNITY AND CORE

    private void Update()
    {
        if (null == controller.SelectionProvider) return;
        Vector2Int? currentCell = controller.SelectionProvider.CurrentOriginCell;

        if (null != currentCell)
        {
            DoraCellData cell = controller.CurrentCellProvider.GetCell(currentCell.Value, false, false);
            cursorImage.enabled = false == cell.HasKernel;
            updateCursorPosition(cell);
            updateCursorSize(cell);
        }
        else
        {
            cursorImage.enabled = false;
        }
    }

    #endregion

    #region PRIVATE

    void updateCursorPosition(DoraCellData i_cell)
    {
        if (null == i_cell) return;

        Vector2 resultAnchoredPosition = cursorRect.anchoredPosition;

        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(i_cell.CellBounds.center);
        Vector2 canvasPos = new Vector2(
        ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
        ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

        resultAnchoredPosition.x = canvasPos.x;
        if(true == updateCursorPositionY) resultAnchoredPosition.y = canvasPos.y;

        cursorRect.anchoredPosition = resultAnchoredPosition;
    }

    void updateCursorSize(DoraCellData i_cell)
    {
        if (null == i_cell) return;

        Rect uiRect = getUIBoundingBoxRect(canvasRect, i_cell.CellBounds, ref extentPointsBuffer);

        float size = Mathf.Max(uiRect.size.x, uiRect.size.y);
        cursorRect.sizeDelta = new Vector2(size, size);
    }


    private Rect getUIBoundingBoxRect(RectTransform i_canvasRect, Bounds i_aabb, ref Vector3[] i_extentPointsBuf)
    {
        Camera camera = Camera.main;
        if (camera == null || i_canvasRect == null) return Rect.zero;

        Vector3 aabbCenter = i_aabb.center;
        Vector3 aabbExtents = i_aabb.extents;
        Vector3 minAabbExt = aabbCenter - aabbExtents;
        Vector3 maxAabbExt = aabbCenter + aabbExtents;

        if (null == i_extentPointsBuf) i_extentPointsBuf = new Vector3[7];

        i_extentPointsBuf[0] = new Vector3(maxAabbExt.x, minAabbExt.y, minAabbExt.z);
        i_extentPointsBuf[1] = new Vector3(minAabbExt.x, minAabbExt.y, maxAabbExt.z);
        i_extentPointsBuf[2] = new Vector3(maxAabbExt.x, minAabbExt.y, maxAabbExt.z);
        i_extentPointsBuf[3] = new Vector3(minAabbExt.x, maxAabbExt.y, minAabbExt.z);
        i_extentPointsBuf[4] = new Vector3(maxAabbExt.x, maxAabbExt.y, minAabbExt.z);
        i_extentPointsBuf[5] = new Vector3(minAabbExt.x, maxAabbExt.y, maxAabbExt.z);
        i_extentPointsBuf[6] = maxAabbExt;

        Vector3 minScreen = camera.WorldToScreenPoint(minAabbExt);
        Vector3 maxScreen = minScreen;

        for (int i = 0; i < 7; i++)
        {
            Vector3 v = camera.WorldToScreenPoint(i_extentPointsBuf[i]);
            minScreen = Vector2.Min(minScreen, v);
            maxScreen = Vector2.Max(maxScreen, v);
        }
        var sizeDelta = i_canvasRect.sizeDelta / 2f;
        return new Rect(minScreen.x - sizeDelta.x, minScreen.y - sizeDelta.y, maxScreen.x - minScreen.x, maxScreen.y - minScreen.y);
    }

    #endregion

}

