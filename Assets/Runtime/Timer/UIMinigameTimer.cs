using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMinigameTimer : MonoBehaviour
{
    [SerializeField] MinigameTimer timer = null;
    [SerializeField] Text secondsTxt = null;
    [SerializeField] Text minutesTxt = null;
    [SerializeField] PopupSpawner popupSpawner = null;
    [SerializeField] RectTransform anchorTime = null;
    [SerializeField] Transform timerRoot = null;
    [SerializeField] InterpolatorsManager interpolatorsManager = null;

    #region UNITY AND CORE

    private void Awake()
    {
        timer.OnDisplayUpdateRequest += updateTimerDisplay;
        timer.OnAddedSeconds += onAddedSeconds;
    }

    private void OnDestroy()
    {
        timer.OnDisplayUpdateRequest -= updateTimerDisplay;
        timer.OnAddedSeconds -= onAddedSeconds;
    }

    #endregion

    #region PRIVATE

    void onAddedSeconds(float i_added)
    {
        popupSpawner.PlayPopupWithAnchor(PopupSpawner.PopupType.TimeBonus, anchorTime, 0.5f, 0.1f, Mathf.CeilToInt(i_added), 30f);

        StartCoroutine(
            scaleRoutine(timerRoot, interpolatorsManager.Animate(
            MathConstants.VECTOR_3_ONE,
            MathConstants.VECTOR_3_ONE * 1.25f,
            0.5f,
            new AnimationMode(AnimationType.Bounce))));
    }

    void updateTimerDisplay(string i_minutes, string i_seconds)
    {
        minutesTxt.text = i_minutes;
        secondsTxt.text = i_seconds;
    }

    IEnumerator scaleRoutine(Transform i_tr, ITypedAnimator<Vector3> i_scaleAnimator)
    {
        while (true == i_scaleAnimator.IsAnimating)
        {
            i_tr.localScale = i_scaleAnimator.Current;
            yield return null;
        }
    }

    #endregion

}
