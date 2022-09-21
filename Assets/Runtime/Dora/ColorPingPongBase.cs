using System.Collections;
using UnityEngine;

public abstract class ColorPingPongBase : MonoBehaviourBase
{
    [SerializeField] protected Color baseColor = Color.yellow;
    [SerializeField] protected Color targetColor = Color.magenta;
    [SerializeField] protected InterpolatorsManager interpolatorsManager = null;
    [SerializeField] protected AnimationCurve singleLerpCurve = null;
    [SerializeField] protected bool clampValues = true;

    protected ITypedAnimator<Color> colorInterpolator = null;
    protected Coroutine pingPongRoutine = null;
    protected Color originalColor = Color.white;
    protected bool resetColorOnFinish = true;

    private void OnDestroy()
    {
        StopPingPong();
    }

    #region PUBLIC API
    public virtual void StartPingPong(float i_singleLerpTime,
                                      Color? i_baseColor,
                                      Color? i_targetColor,
                                      int i_numberOfLerps,
                                      bool i_resetColorOnFinish)
    {
        resetColorOnFinish = i_resetColorOnFinish;
        if (pingPongRoutine == null)
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, i_baseColor, i_targetColor, i_numberOfLerps));

    }

    [ExposePublicMethod]
    public void StartPingPong(float i_singleLerpTime,
                              int i_numberOfLerps)
    {
        if (pingPongRoutine == null)
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, null, null, i_numberOfLerps));
    }

    [ExposePublicMethod]
    public virtual void StopPingPong()
    {
        this.DisposeCoroutine(ref pingPongRoutine);
    }

    [ExposePublicMethod]
    public void PausePingPong()
    {
        if (colorInterpolator != null)
            colorInterpolator.Pause();
    }

    [ExposePublicMethod]
    public void ResumePingPong()
    {
        if (colorInterpolator != null)
            colorInterpolator.Resume();
    }
    #endregion

    #region PRIVATE 
    protected abstract IEnumerator pingPongSequence(float i_singleLerpTime,
                                                    Color? i_baseColor,
                                                    Color? i_targetColor,
                                                    int i_numberOfLerps);
    #endregion
}
