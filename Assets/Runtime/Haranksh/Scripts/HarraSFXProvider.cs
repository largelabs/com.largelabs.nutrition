using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarraSFXProvider : MonoBehaviour
{
    [SerializeField] private AudioSource music = null;
    [SerializeField] private AudioSource failure = null;
    [SerializeField] private AudioSource platformOpenning = null;
    [SerializeField] private AudioSource platformOpenning2 = null;
    [SerializeField] private AudioSource popupAppear = null;
    [SerializeField] private AudioSource score = null;
    [SerializeField] private AudioSource stack = null;
    [SerializeField] private AudioSource bannerAppearSFX = null;
    [SerializeField] private AudioSource bannerTextSFX = null;

    #region PUBLIC API

    public void PlayMusic()
    {
        music.Play();
    }
    public void StopMusic()
    {
        music.Stop();
    }

    public void PlayFailureSFX()
    {
        failure?.Play();
    }
    public void StopFailureSFX()
    {
        failure?.Stop();
    }

    public void PlayPlatformOpenningSFX()
    {
        platformOpenning?.Play();
    }
    public void StopPlatformOpenningSFX()
    {
        platformOpenning?.Stop();
    }

    public void PlayPlatformOpenning2SFX()
    {
        platformOpenning2?.Play();
    }
    public void StopPlatformOpenning2SFX()
    {
        platformOpenning2?.Stop();
    }

    public void PlayAppearSFX()
    {
        popupAppear?.Play();
    }
    public void StopAppearSFX()
    {
        popupAppear?.Stop();
    }

    public void PlayScoreSFX()
    {
        score?.Play();
    }
    public void StopScoreSFX()
    {
        score?.Stop();
    }

    public void PlayStackSFX()
    {
        stack?.Play();
    }
    public void StopStackSFX()
    {
        stack?.Stop();
    }

    public void PlayBannerSFX()
    {
        bannerAppearSFX?.Play();
    }
    public void StopBannerSFX()
    {
        bannerAppearSFX?.Stop();
    }

    public void PlayBannerTextSFX()
    {
        bannerTextSFX?.Play();
    }
    public void StopBannerTextSFX()
    {
        bannerTextSFX?.Stop();
    }

    #endregion
}
