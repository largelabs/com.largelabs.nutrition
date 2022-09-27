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
    
    public bool IsOpen => open;

    public void ResetSprite()
    {
        rnd.sprite = baseSprite;
        rnd.transform.localScale = MathConstants.VECTOR_3_ZERO;
        rnd.color = new Color(rnd.color.r, rnd.color.g, rnd.color.b, 0f);
        open = false;
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
    public void Nudge(InterpolatorsManager i_interps)
    {
        if (nudgePingPong != null)
        {
            Debug.LogError("Nudge");
            nudgePingPong.AssignInterpolators(i_interps);
            nudgePingPong.StartPingPong();
        }
    }
    
    [ExposePublicMethod]
    public void Wobble(InterpolatorsManager i_interps)
    {
        if (wobblePingPong != null)
        {
            wobblePingPong.AssignInterpolators(i_interps);
            wobblePingPong.StartPingPong();
        }
    }

    public void RegisterSFX(HarraSFXProvider i_sfx)
    {
        sfxProvider = i_sfx;
    }
}
