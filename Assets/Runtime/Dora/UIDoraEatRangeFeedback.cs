using UnityEngine;
using UnityEngine.UI;

public class UIDoraEatRangeFeedback : MonoBehaviour
{
    [SerializeField] Image cursorImage = null;
    [SerializeField] Image mouthImage = null;
    [SerializeField] AnimationCurve alphaCurve = null;
    [SerializeField] AnimationCurve scaleCurve = null;

    [SerializeField] InterpolatorsManager interpolators = null;
    [SerializeField] DoraAbstractController controller = null;
    [SerializeField] float scaleMultiplier = 0.75f;
    [SerializeField] float minAnimationTime = 0.25f;
    [SerializeField] float maxAnimationTime = 0.5f;

    [SerializeField] float minAnimationDelay = 0.2f;
    [SerializeField] float maxAnimationDelay = 0.2f;


    ITypedAnimator<Vector3> scaleInterpolator = null;
    ITypedAnimator<float> alphaInterpolator = null;

    #region UNITY AND CORE

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

    #endregion

    #region PUBLIC API

    public Vector3 GetCurrentRangeTargetScale()
    {
        return controller.CurrentSelectionRadius == 0 ? MathConstants.VECTOR_3_ONE : MathConstants.VECTOR_3_ONE * scaleMultiplier * (controller.CurrentSelectionRadius + 1);
    }

    #endregion

    void getAnimationParameters(out float i_animTime, out float i_animDelay)
    {
        float t = (float)controller.CurrentSelectionRadius / (float)controller.MaxSelectionRadius;

        i_animTime = Mathf.Lerp(maxAnimationTime, minAnimationTime, t);

        i_animDelay = Mathf.Lerp(maxAnimationDelay, minAnimationDelay, t);
    }

    void onScaleAnimationEnded(ITypedAnimator<Vector3> i_interpolator)
    {
        transform.localScale = MathConstants.VECTOR_3_ONE;

        if (true == gameObject.activeSelf)
        {
            float animTime, animDelay = 0f;
            getAnimationParameters(out animTime, out animDelay);

            Vector3 startScale = controller.CurrentSelectionRadius == 0 ? MathConstants.VECTOR_3_ONE * scaleMultiplier : MathConstants.VECTOR_3_ONE;
            Vector3 finalScale = GetCurrentRangeTargetScale();
           
            
            scaleInterpolator = interpolators.Animate(startScale, finalScale, animTime, new AnimationMode(scaleCurve), false, animDelay, onScaleAnimationEnded);
        }
    }

    void onAlphaAnimationEnded(ITypedAnimator<float> i_interpolator)
    {
        setAlpha(0f);

        if (true == gameObject.activeSelf)
        {
            float animTime, animDelay = 0f;
            getAnimationParameters(out animTime, out animDelay);
            alphaInterpolator = interpolators.Animate(0f, 0.5f, animTime, new AnimationMode(alphaCurve), false, animDelay, onAlphaAnimationEnded);
        }

    }

    void setAlpha(float i_alpha)
    {
        Color col = cursorImage.color;
        col.a = Mathf.Clamp(i_alpha, 0f, 0.5f);
        cursorImage.color = col;

        col = mouthImage.color;
        col.a = i_alpha;
        mouthImage.color = col;
    }

}
