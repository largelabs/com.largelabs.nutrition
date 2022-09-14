using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIDoraRaycastPointer : MonoBehaviourBase
{
    [SerializeField] DoraInputs inputs = null;
    [SerializeField] Transform selectionFeedbackTr = null;
    [SerializeField] Image cursorImage = null;
    [SerializeField] DoraAbstractController controller = null;
    [SerializeField] InterpolatorsManager interpolators = null;

    [SerializeField] DoraSelectionRaycastSource source = null;
    [SerializeField] RectTransform canvasRect = null;
    [SerializeField] RectTransform cursorRect = null;

    Coroutine moveCursorRoutine = null;

    #region UNITY AND CORE

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

        if(true == i_enable)
        {
            inputs.OnEatStarted += onEatStarted;
            inputs.OnEatReleased += onEatReleased;
            inputs.OnEat += recenterCursor;
        }
        else
        {
            inputs.OnEatStarted -= onEatStarted;
            inputs.OnEatReleased -= onEatReleased;
            inputs.OnEat -= recenterCursor;

            onEatReleased();
        }
    }

    #endregion

    #region PRIVATE

    void onEatStarted()
    {
        selectionFeedbackTr.gameObject.SetActive(true);
    }

    void recenterCursor()
    {
        if(controller.CurrentSelectionRadius > 0)
        {
            if(null == moveCursorRoutine)
            {


                moveCursorRoutine = StartCoroutine(updateCursorPosition(null));
            }
        }
    }

    IEnumerator updateCursorPosition(ITypedAnimator<Vector3> i_interpolator)
    {
        yield break;
    }

    void onEatReleased()
    {
        selectionFeedbackTr.gameObject.SetActive(false);
        this.DisposeCoroutine(ref moveCursorRoutine);
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
