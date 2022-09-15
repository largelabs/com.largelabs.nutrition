using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIDoraBiteAnimation : MonoBehaviour
{
    [SerializeField] protected UIDoraEatRangeFeedback rangeFeedback = null;
    [SerializeField] protected UIImageFrameSwapper mouthFrameSwapper = null;
    [SerializeField] protected Image mouthImage = null;

    Coroutine waitForPlaybackEndedRoutine = null;

    #region PUBLIC API

    public void Play()
    {
        gameObject.SetActive(true);
        transform.localScale = rangeFeedback.GetCurrentBiteTargetScale();
        transform.position = rangeFeedback.transform.position;

        mouthFrameSwapper.ResetAnimation();
        mouthFrameSwapper.Play();
        waitForPlaybackEndedRoutine = StartCoroutine(waitForPlaybackEnded());
    }

    public virtual void Stop()
    {
        this.DisposeCoroutine(ref waitForPlaybackEndedRoutine);
        gameObject.SetActive(false);
    }

    public bool IsPlaying => null != waitForPlaybackEndedRoutine;

    #endregion

    #region PROTECTED

    protected virtual bool isPlaybackDone => false == mouthFrameSwapper.IsPlaying;

    #endregion

    #region PRIVATE

    IEnumerator waitForPlaybackEnded()
    {
        while (true == mouthFrameSwapper.IsPlaying) yield return null;
        Stop();
    }

    #endregion


}
