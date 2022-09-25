using System.Collections;
using UnityEngine;

public class DoraMover : MonoBehaviourBase
{
    [SerializeField] private Transform playAnchor = null;
    [SerializeField] private Transform doneAnchor = null;
    [SerializeField] private InterpolatorsManager interpolatorManager = null;
    [SerializeField] private AnimationCurve playMoveCurve = null;

    #region PUBLIC API

    public void ResetMover()
    {
        StopAllCoroutines();
    }

    public IEnumerator MoveCobOffScreen(Transform i_cob, float i_time)
    {
        if (null == i_cob) yield break;

        yield return StartCoroutine(animateToTransform(i_cob, doneAnchor, i_time, playMoveCurve));
    }

    public IEnumerator MoveCobToGameView(Transform i_cob, float i_time)
    {
        yield return StartCoroutine(animateToTransform(i_cob, playAnchor, i_time,
                                playMoveCurve));
    }

    #endregion

    #region PRIVATE API

    IEnumerator animateToTransform(Transform i_cob, Transform i_target, float i_time, AnimationCurve i_curve)
    {
        AnimationMode mode = new AnimationMode(i_curve);

        Vector3 targetScale = new Vector3(i_target.localScale.x / i_cob.lossyScale.x,
                                            i_target.localScale.y / i_cob.lossyScale.y,
                                            i_target.localScale.z / i_cob.lossyScale.z);
        ITypedAnimator<Vector3> scaleInterpolator = interpolatorManager.Animate(i_cob.localScale, targetScale, i_time, mode, false, 0f, null);
        ITypedAnimator<Vector3> posInterpolator = interpolatorManager.Animate(i_cob.position, i_target.position, i_time, mode, false, 0f, null);

        while (true == posInterpolator.IsActive)
        {
            i_cob.position = posInterpolator.Current;
            i_cob.localScale = scaleInterpolator.Current;
            yield return null;
        }

        i_cob.transform.position = new Vector3(i_target.position.x, i_target.position.y, 0);
    }

    #endregion
}
