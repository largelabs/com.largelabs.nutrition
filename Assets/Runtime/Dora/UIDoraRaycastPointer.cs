using UnityEngine;

public class UIDoraRaycastPointer : MonoBehaviourBase
{
    [SerializeField] DoraInputs inputs = null;
    [SerializeField] GameObject selectionFeedbackGo = null;

    [SerializeField] DoraSelectionRaycastSource source = null;
    [SerializeField] RectTransform canvasRect = null;
    [SerializeField] RectTransform cursorRect = null;

    private void Start()
    {
        inputs.OnEatStarted += onEatStarted;
        inputs.OnEatReleased += onEatReleased;
    }

    private void LateUpdate()
    {
        updateCursorPosition();
    }

    void onEatStarted()
    {
        selectionFeedbackGo.SetActive(true);
    }

    void onEatReleased()
    {
        selectionFeedbackGo.SetActive(false);
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
}
