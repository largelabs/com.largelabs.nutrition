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
    [SerializeField] Transform centerAnchor = null;
    [SerializeField] InterpolatorsManager interpolatorsManager = null;

    Coroutine moveTimerRootRoutine = null;
    Coroutine scaleRoutine = null;


    Vector3 timerRootInitialPosition = MathConstants.VECTOR_3_ZERO;

    #region UNITY AND CORE

    private void Awake()
    {
        timerRootInitialPosition = timerRoot.position;
        timer.OnDisplayUpdateRequest += updateTimerDisplay;
    }

    private void OnDestroy()
    {
        timer.OnDisplayUpdateRequest -= updateTimerDisplay;
    }

    #endregion

    #region PUBLIC API


    public void MoveTimerRootToCenter()
    {
        this.DisposeCoroutine(ref moveTimerRootRoutine); 
        this.DisposeCoroutine(ref scaleRoutine);

        moveTimerRootRoutine = StartCoroutine(moveTimeRoot(timerRoot.position, centerAnchor.position, 0.5f));

        scaleRoutine = StartCoroutine(
            updateScale(timerRoot, interpolatorsManager.Animate(
            timerRoot.localScale,
            MathConstants.VECTOR_3_ONE * 1.5f,
            0.5f,
            new AnimationMode(AnimationType.Ease_In_Out))));
    }

    public void MoveTimerRootToInitialPosition()
    {
        this.DisposeCoroutine(ref moveTimerRootRoutine);
        this.DisposeCoroutine(ref scaleRoutine);

        moveTimerRootRoutine = StartCoroutine(moveTimeRoot(timerRoot.position, timerRootInitialPosition, 0.5f));

        scaleRoutine = StartCoroutine(
            updateScale(timerRoot, interpolatorsManager.Animate(
            timerRoot.localScale,
            MathConstants.VECTOR_3_ONE,
            0.5f,
            new AnimationMode(AnimationType.Ease_In_Out))));
    }

    public void PlayTimeBonusPopup(float i_added)
    {
        popupSpawner.PlayPopupWithAnchor(PopupSpawner.PopupType.TimeBonus, anchorTime, 0.5f, 0.1f, Mathf.CeilToInt(i_added), 30f);
    }

    public void BumpTimer()
    {
        this.DisposeCoroutine(ref scaleRoutine);

        scaleRoutine = StartCoroutine(
            updateScale(timerRoot, interpolatorsManager.Animate(
            timerRoot.localScale,
            timerRoot.localScale * 1.5f,
            0.5f,
            new AnimationMode(AnimationType.Bounce))));
    }

    #endregion

    #region PRIVATE

    IEnumerator moveTimeRoot(Vector3 i_start, Vector3 i_target, float i_time)
    {
        ITypedAnimator<Vector3> interpolator = interpolatorsManager.Animate(i_start, i_target, i_time, new AnimationMode(AnimationType.Ease_In_Out));
        while (true == interpolator.IsAnimating)
        {
            timerRoot.position = interpolator.Current;
            yield return null;
        }

        timerRoot.position = i_target;
        this.DisposeCoroutine(ref moveTimerRootRoutine);

    }

    void updateTimerDisplay(string i_minutes, string i_seconds)
    {
        minutesTxt.text = i_minutes;
        secondsTxt.text = i_seconds;
    }

    IEnumerator updateScale(Transform i_tr, ITypedAnimator<Vector3> i_scaleAnimator)
    {
        while (true == i_scaleAnimator.IsAnimating)
        {
            i_tr.localScale = i_scaleAnimator.Current;
            yield return null;
        }

        this.DisposeCoroutine(ref scaleRoutine);

    }

    #endregion

}
