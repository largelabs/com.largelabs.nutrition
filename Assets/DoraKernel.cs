using System.Collections;
using UnityEngine;

public class DoraKernel : MonoBehaviourBase
{
    [SerializeField] AnimationCurve appearCurve = null;

    InterpolatorsManager interpolators = null;
    Coroutine appearRoutine = null;

    public void Init(InterpolatorsManager i_interpolators)
    {
        gameObject.SetActive(false);
        interpolators = i_interpolators;
    }

    public void Appear(bool i_animated)
    {
        gameObject.SetActive(true);

        if (true == i_animated)
        {
            if (null == appearRoutine) appearRoutine = StartCoroutine(appearAnimated());
        }
        else
        {
            onDidAppear();
        }
    }

    IEnumerator appearAnimated()
    {
        transform.localScale = MathConstants.VECTOR_3_ZERO;

        AnimationMode mode = new AnimationMode(appearCurve);
        ITypedAnimator<Vector3> scaleInterpolator = interpolators.Animate(MathConstants.VECTOR_3_ZERO, MathConstants.VECTOR_3_ONE, 0.5f, mode, false);

        while(true == scaleInterpolator.IsActive)
        {
            transform.localScale = scaleInterpolator.Current;
            yield return null;
        }

        onDidAppear();
    }

    void onDidAppear()
    {
        transform.localScale = MathConstants.VECTOR_3_ONE;
        if (null != appearRoutine) StopCoroutine(appearRoutine);
        appearRoutine = null;
    }

}
