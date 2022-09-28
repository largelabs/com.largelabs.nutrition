using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarraPlatformAnimationManager : MonoBehaviourBase
{
    [Header("VFX")]
    [SerializeField] LocalScalePingPong scaleAppear = null;
    [SerializeField] LocalScalePingPong scaleDisappear = null;
    [SerializeField] SpriteAlphaLerp alphaAppear = null;
    [SerializeField] SpriteAlphaLerp alphaDisappear = null;
    [SerializeField] LocalPositionPingPong nudgePingPong = null;
    [SerializeField] LocalPositionPingPong wobblePingPong = null;

    [Header("Animations")]
    [SerializeField] SpriteRenderer rnd = null;
    [SerializeField] Sprite baseSprite = null;
    [SerializeField] SpriteFrameSwapper openUp = null;    
    [SerializeField] SpriteFrameSwapper closeUp = null;

    [Header("SFX")]
    [SerializeField] HarraSFXProvider sfxProvider = null;

    bool open = false;
    private InterpolatorsManager interpolatorsManager = null;
    
    public bool IsOpen => open;

    public void ResetSprite()
    {
        rnd.sprite = baseSprite;
        rnd.transform.localScale = MathConstants.VECTOR_3_ZERO;
        rnd.color = new Color(rnd.color.r, rnd.color.g, rnd.color.b, 0f);
        open = false;
    }

    [ExposePublicMethod]
    public void PlatformAppear(float i_time)
    {
        if (interpolatorsManager == null)
        {
            Debug.LogError("No Interpolator assigned!");
            return;
        }

        if (alphaAppear != null)
            alphaAppear.LerpAlpha(null, null, i_time, interpolatorsManager, null, null, null);

        if (scaleAppear != null)
        {
            scaleAppear.AssignInterpolators(interpolatorsManager);
            scaleAppear.StartPingPong(i_time, 1);
        }
    }

    [ExposePublicMethod]
    public void PlatformDisppear(float i_time)
    {
        if (interpolatorsManager == null)
        {
            Debug.LogError("No Interpolator assigned!");
            return;
        }

        if (alphaDisappear != null)
            alphaDisappear.LerpAlpha(null, null, i_time, interpolatorsManager, null, null, null);

        if (scaleDisappear != null)
        {
            scaleDisappear.AssignInterpolators(interpolatorsManager);
            scaleDisappear.StartPingPong(i_time, 1);
        }

        CloseUp();
    }

    [ExposePublicMethod]
    public void OpenUp()
    {
        if (openUp == null) return;
        // sfx suggestion: sound for platform opening
        if (sfxProvider != null)
        {
            sfxProvider.PlayPlatformOpenningSFX();
            sfxProvider.PlayPlatformOpenning2SFX();
        }
        openUp.ResetAnimation();
        openUp.Play();
        open = true;
    } 
    
    [ExposePublicMethod]
    public void CloseUp()
    {
        if (closeUp == null) return;

        closeUp.ResetAnimation();
        closeUp.Play();
        open = false;
    }

    [ExposePublicMethod]
    public void Nudge()
    {
        if (interpolatorsManager == null)
        {
            Debug.LogError("No Interpolator assigned!");
            return;
        }

        if (nudgePingPong != null)
        {
            Debug.LogError("Nudge");
            nudgePingPong.AssignInterpolators(interpolatorsManager);
            nudgePingPong.StartPingPong();
        }
    }
    
    [ExposePublicMethod]
    public void Wobble()
    {
        if (interpolatorsManager == null)
        {
            Debug.LogError("No Interpolator assigned!");
            return;
        }

        if (wobblePingPong != null)
        {
            wobblePingPong.AssignInterpolators(interpolatorsManager);
            wobblePingPong.StartPingPong();
        }
    }

    public void RegisterSFX(HarraSFXProvider i_sfx)
    {
        sfxProvider = i_sfx;
    }

    public void RegisterInterpolators(InterpolatorsManager i_interps)
    {
        interpolatorsManager = i_interps;

        if (wobblePingPong != null)
            wobblePingPong.AssignInterpolators(interpolatorsManager);
        if (nudgePingPong != null)
            nudgePingPong.AssignInterpolators(interpolatorsManager);

        if (scaleAppear != null)
            scaleAppear.AssignInterpolators(interpolatorsManager);
        if (scaleDisappear != null)
            scaleDisappear.AssignInterpolators(interpolatorsManager);
    }
}
