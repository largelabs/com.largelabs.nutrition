using UnityEngine;
using UnityEngine.UI;

public class UIDoraRaycastPointer : MonoBehaviourBase
{
    [SerializeField] DoraInputs inputs = null;
    [SerializeField] Transform selectionFeedbackTr = null;
    [SerializeField] Image cursorImage = null;

    [SerializeField] DoraSelectionRaycastSource source = null;
    [SerializeField] RectTransform canvasRect = null;
    [SerializeField] RectTransform cursorRect = null;

    #region UNITY AND CORE

    private void Start()
    {
        inputs.OnEatStarted += onEatStarted;
        inputs.OnEatReleased += onEatReleased;
    }

    private void LateUpdate()
    {
        selectionFeedbackTr.position = transform.position;
        updateCursorPosition();
    }

    #endregion

    #region PUBLIC API

    public void EnablePointer(bool i_enable)
    {
        cursorImage.enabled = i_enable;
    }

    #endregion

    #region PRIVATE

    void onEatStarted()
    {
        selectionFeedbackTr.gameObject.SetActive(true);
    }

    void onEatReleased()
    {
        selectionFeedbackTr.gameObject.SetActive(false);
    }

    void updateCursorPosition()
    {
        Vector2 resultAnchoredPosition = cursorRect.anchoredPosition;

        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(source.transform.position);
        Vector2 canvasPos = new Vector2(
        ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
        ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

        resultAnchoredPosition = canvasPos;

        cursorRect.anchoredPosition = resultAnchoredPosition;
    }


    #endregion
}
