using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Text))]
public class UIFloatingScore : MonoBehaviour
{
    RectTransform thisRectTransform = null;
    Text scoreText = null;
    Vector2 screenOffset;

    public Action<UIFloatingScore> OnAnimationEnded = null;

    Coroutine AnimationRoutine = null;

    private void Awake()
    {
        getAttachedComponents();
    }

    #region PUBLIC API
    public void SetAlpha(float i_alpha)
    {
        if (scoreText != null)
        {
            Color temp = scoreText.color;
            scoreText.color = new Color(temp.r, temp.g, temp.b, i_alpha);
        }
    }
    
    public void SetColor(Color i_color)
    {
        if (scoreText != null)
            scoreText.color = i_color;
    }
    
    public void SetColorNoAlpha(Color i_color)
    {
        if (scoreText != null)
            scoreText.color = new Color(i_color.r, i_color.g, i_color.b, scoreText.color.a);
    }

    public void Animate(Vector3 i_worldPosition, 
                        float i_animTime,
                        float i_alphaTime,
                        int i_score, 
                        float i_yOffset,
                        RectTransform i_canvasRect, 
                        Camera i_camera,
                        AnimationCurve i_curve,
                        InterpolatorsManager i_interpolatorManager
                        )
    {
        if (AnimationRoutine == null)
        {
            getAttachedComponents();
            screenOffset = new Vector2((float)i_canvasRect.sizeDelta.x / 2f, (float)i_canvasRect.sizeDelta.y / 2f);

            Vector2 viewPortPos = i_camera.WorldToViewportPoint(i_worldPosition);
            Vector2 newPos = new Vector2(viewPortPos.x * i_canvasRect.sizeDelta.x, viewPortPos.y * i_canvasRect.sizeDelta.y);

            Vector2 spawnPos = newPos - screenOffset;

            AnimationRoutine = StartCoroutine
                ( animateScorePopup(spawnPos, i_animTime, i_alphaTime,
                                    i_score, i_yOffset, i_curve, i_interpolatorManager) );
        }
    }
    
    public void Animate(Vector2 i_position, 
                        float i_animTime,
                        float i_alphaTime,
                        int i_score, 
                        float i_yOffset,
                        AnimationCurve i_curve,
                        InterpolatorsManager i_interpolatorManager
                        )
    {
        if (AnimationRoutine == null)
        {
            getAttachedComponents();

            AnimationRoutine = StartCoroutine
                ( animateScorePopup(i_position, i_animTime, i_alphaTime,
                                    i_score, i_yOffset, i_curve, i_interpolatorManager) );
        }
    }

    #endregion

    #region PRIVATE API
    private IEnumerator animateScorePopup(Vector2 i_position,
                                            float i_animTime,
                                            float i_alphaTime,
                                            int i_score,
                                            float i_yOffset,
                                            AnimationCurve i_curve,
                                            InterpolatorsManager i_interpolatorManager
                                            )
    {
        AnimationMode mode = new AnimationMode(i_curve);
        thisRectTransform.localPosition = i_position;

        scoreText.text = "";
        if (i_score > 0)
            scoreText.text += "+";
        scoreText.text += i_score.ToString();

        ITypedAnimator<float> yInterpolator = i_interpolatorManager.Animate(thisRectTransform.position.y, thisRectTransform.position.y + i_yOffset, i_animTime, mode, true, 0f, null);
        ITypedAnimator<float> alphaInterpolator = i_interpolatorManager.Animate(0f, 1f, i_alphaTime, mode, true, 0f, null);

        // try is animating
        // try separating the two
        // try using the bigger time
        float currTime = 0f;
        float targetTime = i_animTime > i_alphaTime ? i_animTime : i_alphaTime;
        while (currTime < targetTime)
        {
            if(yInterpolator.IsAnimating)
                updatePosition(yInterpolator.Current);

            //SetAlpha(1f);
            if(alphaInterpolator.IsAnimating)
                SetAlpha(alphaInterpolator.Current);
            yield return null;

            currTime += Time.deltaTime;
        }

        OnAnimationEnded?.Invoke(this);
        this.DisposeCoroutine(ref AnimationRoutine);
    }

    private void updatePosition(float i_pos)
    {
        Vector2 newPos = scoreText.transform.position;
        newPos.y = i_pos;
        scoreText.transform.position = newPos;
    }

    private void getAttachedComponents()
    {
        if (thisRectTransform == null)
            thisRectTransform = GetComponent<RectTransform>();

        if (scoreText == null)
            scoreText = GetComponent<Text>();
    }
    #endregion
}
