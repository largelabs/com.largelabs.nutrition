using PathologicalGames;
using System.Collections;
using UnityEngine;

public class UIPooledBiteAnimation : UIDoraBiteAnimation
{
    SpawnPool pool = null;
    Coroutine alphaRoutine = null;

    #region PUBLIC API

    private void OnEnable()
    {
        Debug.LogError("ENABLE");
    }

    private void OnDisable()
    {
        Debug.LogError("DISABLE");
    }

    public void Play(SpawnPool i_pool, InterpolatorsManager i_interpolators, UIDoraEatRangeFeedback i_rangeFeedback)
    {
        this.DisposeCoroutine(ref alphaRoutine);
        rangeFeedback = i_rangeFeedback;
        pool = i_pool;

        Play();
        setAlpha(0f);
        alphaRoutine = StartCoroutine(updateAlpha(i_interpolators));
    }

    public override void Stop()
    {
        base.Stop();

        this.DisposeCoroutine(ref alphaRoutine);
        pool.Despawn(transform);
        rangeFeedback = null;
    }

    #endregion

    #region PROTECTED

    protected override bool isPlaybackDone => base.isPlaybackDone && null == alphaRoutine;

    #endregion

    #region PRIVATE

    void setAlpha(float i_alpha)
    {
        Color col = mouthImage.color;
        col.a = i_alpha;
        mouthImage.color = col;
    }

    IEnumerator updateAlpha(InterpolatorsManager i_interpolators)
    {
        ITypedAnimator<float> inter = i_interpolators.Animate(0f, 0.5f, 0.2f, new AnimationMode(AnimationType.Bounce));
        while (inter.IsAnimating)
        {
            setAlpha(inter.Current);
            yield return null;
        }

        this.DisposeCoroutine(ref alphaRoutine);
    }

    #endregion
}
