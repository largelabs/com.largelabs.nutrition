using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIDoraBiteAnimation : MonoBehaviour
{
    [SerializeField] protected UIDoraEatRangeFeedback rangeFeedback = null;
    [SerializeField] protected UIImageFrameSwapper mouthFrameSwapper = null;
    [SerializeField] protected Image mouthImage = null;
    [SerializeField] ShakeEffect2D shakeEffect = null;
    [SerializeField] UIImageColorPingPong colorPingPong = null;

    Coroutine waitForPlaybackEndedRoutine = null;
    Coroutine negativeRoutine = null;

    #region PUBLIC API

    public void Play(bool i_negative)
    {
        mouthImage.color = Color.white;
        gameObject.SetActive(true);
        transform.localScale = rangeFeedback.GetCurrentBiteTargetScale();
        transform.position = rangeFeedback.transform.position;

        mouthFrameSwapper.ResetAnimation();
        mouthFrameSwapper.Play();

        if (true == i_negative)
            negativeRoutine = StartCoroutine(hurtMouth());

        waitForPlaybackEndedRoutine = StartCoroutine(waitForPlaybackEnded());
    }

    public virtual void Stop()
    {
        this.DisposeCoroutine(ref negativeRoutine);
        this.DisposeCoroutine(ref waitForPlaybackEndedRoutine);
        colorPingPong?.StopPingPong();
        mouthImage.color = Color.white;
        gameObject.SetActive(false);
    }

    public bool IsPlaying => null != waitForPlaybackEndedRoutine;

    #endregion

    #region PROTECTED

    protected virtual bool isPlaybackDone => false == mouthFrameSwapper.IsPlaying && null == negativeRoutine;

    #endregion

    #region PRIVATE

    IEnumerator hurtMouth()
    {
        yield return this.Wait(0.15f);
        colorPingPong.StartPingPong(0.2f, -1);
        shakeEffect.StartShake();
        while (true == shakeEffect.IsShaking) yield return null;
        this.DisposeCoroutine(ref negativeRoutine);

    }

    IEnumerator waitForPlaybackEnded()
    {
        while (false == isPlaybackDone) yield return null;
        Stop();
    }

    #endregion


}
