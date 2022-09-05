using UnityEngine;
using UnityEngine.UI;

public class UIDoraEatRangeFeedback : MonoBehaviour
{
    [SerializeField] Image cursorImage = null;
    [SerializeField] AnimationCurve alphaCurve = null;
    [SerializeField] InterpolatorsManager interpolators = null;
    [SerializeField] DoraController controller = null;

    ITypedAnimator<Vector3> scaleInterpolator = null;
    ITypedAnimator<float> alphaInterpolator = null;

    float scaleMultiplier = 4f;

    private void OnEnable()
    {
        onScaleAnimationEnded(null);
        onAlphaAnimationEnded(null);
    }

    private void Update()
    {
        if (null != scaleInterpolator && true == scaleInterpolator.IsActive) transform.localScale = scaleInterpolator.Current;
        if (null != alphaInterpolator && true == alphaInterpolator.IsActive) setAlpha(alphaInterpolator.Current);

    }


    void onScaleAnimationEnded(ITypedAnimator<Vector3> i_interpolator)
    {
        transform.localScale = MathConstants.VECTOR_3_ONE;

        if(true == gameObject.activeSelf)
            scaleInterpolator = interpolators.Animate(MathConstants.VECTOR_3_ONE, MathConstants.VECTOR_3_ONE * scaleMultiplier * (controller.CurrentSelectionRadius + 1), 0.5f, new AnimationMode(AnimationType.Ease_In_Out), false, 0.2f, onScaleAnimationEnded);
    }

    void onAlphaAnimationEnded(ITypedAnimator<float> i_interpolator)
    {
        setAlpha(0f);

        if (true == gameObject.activeSelf)
            alphaInterpolator = interpolators.Animate(0f, 0.5f, 0.5f, new AnimationMode(AnimationType.Bounce), false, 0.2f, onAlphaAnimationEnded);
    }

    void setAlpha(float i_alpha)
    {
        Color col = cursorImage.color;
        col.a = i_alpha;
        cursorImage.color = col;
    }

}
