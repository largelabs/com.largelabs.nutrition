using System.Collections;
using UnityEngine;

public class DoraSFXProvider : MonoBehaviourBase
{
    [SerializeField] private AudioSource music = null;

    [SerializeField] private AudioSource[] bigBiteSFXs = null;
    [SerializeField] private AudioSource smallBiteSFX = null;
    [SerializeField] private AudioSource chewSFX = null;
    [SerializeField] private AudioSource rangeSelectionSFX = null;

    [SerializeField] private AudioSource ambientSound = null;
    [SerializeField] private AudioSource ambientFireSFX = null;
    [SerializeField] private AudioSource timeBonusSFX = null;

    [SerializeField] private AudioSource[] hurtMouthSFXs = null;
    [SerializeField] private AudioSource movementSFX = null;
    [SerializeField] private AudioSource landedSFX = null;

    [SerializeField] private AudioSource uiKernelPositiveSFX = null;
    [SerializeField] private AudioSource uiKernelNegativeSFX = null;


    #region PUBLIC API

    public void StartMusic()
    {
        music.Play();
    }

    public void StopMusic()
    {
        music.Stop();
    }

    public void PlayMovementSFX()
    {
        movementSFX?.Play();
    }

    public void PlayLandedSFX()
    {
        landedSFX?.Play();
    }

    public void PlayHurtMouthSFX()
    {
        playRandomSoundFromArray(hurtMouthSFXs);
    }

    public void PlayUIKernelSFX(KernelStatus i_status)
    {
        if(i_status == KernelStatus.Burnt)
            uiKernelNegativeSFX?.Play();
        else
            uiKernelPositiveSFX?.Play();
    }

    public void PlayTimeBonusSFX()
    {
        timeBonusSFX?.Play();
    }

    public void StartAmbientSounds()
    {
        ambientSound?.Play();
        ambientFireSFX?.Play();
    }

    public void StopAmbientSounds()
    {
        ambientSound?.Stop();
        ambientFireSFX?.Stop();
    }

    public void PlayRangeSFX(float i_addedPitch)
    {
        if (null == rangeSelectionSFX) return;

        if (rangeSelectionSFX.isPlaying == false) rangeSelectionSFX?.Play();
        rangeSelectionSFX.pitch += i_addedPitch;
    }

    public void StopRangeSFX()
    {
        if (null == rangeSelectionSFX) return;

        rangeSelectionSFX.pitch = 1f;
        rangeSelectionSFX.Stop();
    }

    public void PlaySmallBiteSFX()
    {
        if (null == smallBiteSFX) return;

        float randomPitch = Random.Range(1f, 2f);
        smallBiteSFX.pitch = randomPitch;
        smallBiteSFX.Play();
    }

    public void PlayBigBiteSFX()
    {
        if (null == bigBiteSFXs) return;

        StartCoroutine(playBigBiteSFX());
    }

    #endregion

    #region PRIVATE

    IEnumerator playBigBiteSFX()
    {
        playRandomSoundFromArray(bigBiteSFXs);

        while (bigBiteSFXs[0].isPlaying || bigBiteSFXs[1].isPlaying)
        {
            yield return null;
        }
        chewSFX.Play();
    }

    private void playRandomSoundFromArray(AudioSource[] i_audioSources)
    {
        if (null == i_audioSources) return;

        int randomSFX = Random.Range(0, i_audioSources.Length);
        i_audioSources[randomSFX]?.Play();
    }


    #endregion

}

