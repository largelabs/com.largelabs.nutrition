using System.Collections;
using UnityEngine;

public class UIDoraBiteAnimation : MonoBehaviour
{
    [SerializeField] UIDoraEatRangeFeedback rangeFeedback = null;
    [SerializeField] UIImageFrameSwapper mouthFrameSwapper = null;

    Coroutine waitForPlaybackEndedRoutine = null;

    #region PUBLIC API

    public void Play()
    {
        gameObject.SetActive(true);
        transform.localScale = rangeFeedback.GetCurrentRangeTargetScale();
        transform.position = rangeFeedback.transform.position;

        mouthFrameSwapper.ResetAnimation();
        mouthFrameSwapper.Play();
        waitForPlaybackEndedRoutine = StartCoroutine(waitForPlaybackEnded());
    }

    public void Stop()
    {
        this.DisposeCoroutine(ref waitForPlaybackEndedRoutine);
        gameObject.SetActive(false);
    }

    public bool IsPlaying => null != waitForPlaybackEndedRoutine;

    #endregion

    #region PRIVATE

    IEnumerator waitForPlaybackEnded()
    {
        while (mouthFrameSwapper.IsPlaying) yield return null;
        Stop();
    }

    #endregion


}
