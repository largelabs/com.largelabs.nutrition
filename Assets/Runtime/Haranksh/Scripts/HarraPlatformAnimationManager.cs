using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarraPlatformAnimationManager : MonoBehaviourBase
{
    [Header("VFX")]
    [SerializeField] LocalScalePingPong scaleAppear = null;
    [SerializeField] LocalScalePingPong scaleDisppear = null;
    [SerializeField] SpriteAlphaLerp alphaAppear = null;
    [SerializeField] SpriteAlphaLerp alphaDisappear = null;

    [Header("Animations")]
    [SerializeField] SpriteRenderer rnd = null;
    [SerializeField] Sprite baseSprite = null;
    [SerializeField] SpriteFrameSwapper openUp = null;    
    [SerializeField] SpriteFrameSwapper closeUp = null;    

    public void ResetSprite()
    {
        rnd.sprite = baseSprite;
    }

    [ExposePublicMethod]
    public void PlatformAppear(InterpolatorsManager i_interps, float i_time)
    {
        if (alphaAppear != null)
            alphaAppear.LerpAlpha(null, null, i_time, i_interps, null, null, null);

        if (scaleAppear != null)
        {
            scaleAppear.AssignInterpolators(i_interps);
            scaleAppear.StartPingPong(i_time, 1);
        }
    }

    [ExposePublicMethod]
    public void PlatformDisppear(InterpolatorsManager i_interps, float i_time)
    {
        if (alphaDisappear != null)
            alphaDisappear.LerpAlpha(null, null, i_time, i_interps, null, null, null);

        if (scaleDisppear != null)
        {
            scaleDisppear.AssignInterpolators(i_interps);
            scaleDisppear.StartPingPong(i_time, 1);
        }

        CloseUp();
    }

    [ExposePublicMethod]
    public void OpenUp()
    {
        if (openUp == null) return;

        openUp.ResetAnimation();
        openUp.Play();
    } 
    
    [ExposePublicMethod]
    public void CloseUp()
    {
        if (closeUp == null) return;

        closeUp.ResetAnimation();
        closeUp.Play();
    }
}
