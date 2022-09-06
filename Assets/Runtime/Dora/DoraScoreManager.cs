using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraScoreManager : MonoBehaviourBase
{
    [SerializeField] DoraGameData doraGameData = null;
    [SerializeField] float increaseOfMultiplier = 0.1f;
    [SerializeField] private ScorePopupSpawner scorePopupSpawner = null;

    int score = 0;

    float goodBaseScore = 0f;
    float burntBaseScore = 0f;

    protected override void Awake()
    {
        base.Awake();
        goodBaseScore = doraGameData.GoodKernelScore;
        burntBaseScore = doraGameData.BurntKernelScore;
    }

    #region PUBLIC API

    public void AddScore(int i_numberOfKernels)
    {
        if (i_numberOfKernels == 1) addToScore(doraGameData.GoodKernelScore);
        else
        {
            float multiplier = 1 + (i_numberOfKernels * increaseOfMultiplier);
            float scoreToAdd = doraGameData.GoodKernelScore * multiplier * i_numberOfKernels;
            addToScore((int)scoreToAdd);
        }
    }

    public void RemoveScore(int i_numberOfBurntKernels)
    {
        if (i_numberOfBurntKernels == 1) removeFromScore(doraGameData.BurntKernelScore);
        else
        {
            float multiplier = 1 + (i_numberOfBurntKernels * increaseOfMultiplier);
            float scoreToRemove = doraGameData.BurntKernelScore * multiplier * i_numberOfBurntKernels;
            removeFromScore((int)scoreToRemove);
        }
    }

    public void AddScoreByValue(int i_score,
                                ScorePopupSpawner.PopupType i_popupType,
                                Vector3 i_worldPos,
                                float i_animTime,
                                float i_alphaTime,
                                float i_yoffset
                                )
    {
        scorePopupSpawner.PlayScore(i_popupType, i_worldPos, i_animTime, i_alphaTime, i_score, i_yoffset);
        addToScore(i_score);
    }


    public void AddScoreByKernels(Queue<DoraKernel> i_eatenKernels,
                                  float i_animTime,
                                  float i_alphaTime,
                                  float i_yoffset
                                  )
    {
        if (i_eatenKernels == null || i_eatenKernels.Count == 0)
        {
            Debug.LogError("Invalid eaten kernel queue! Returning...");
            return;
        }

        int scoreToAdd = 0;
        ScorePopupSpawner.PopupType popupType = ScorePopupSpawner.PopupType.Positive;

        if (i_eatenKernels.Count > 13) popupType = ScorePopupSpawner.PopupType.Super;

        DoraKernel currKernel = null;

        // get position from first kernel (which should be center)
        currKernel = i_eatenKernels.Dequeue();
        if(currKernel.IsBurnt) popupType = ScorePopupSpawner.PopupType.Positive;
        Vector3 worldPos = currKernel.transform.position;

        float multiplier = 1f;
        scoreToAdd += getKernelScore(currKernel, multiplier);

        // multiplier increases in each loop (after each kernel in the stack)
        while (i_eatenKernels.TryDequeue(out currKernel))
        {
            if (currKernel.IsBurnt) popupType = ScorePopupSpawner.PopupType.Positive;
            multiplier += 1f;
            scoreToAdd += getKernelScore(currKernel, multiplier);
        }

        if (scoreToAdd < 0)
            popupType = ScorePopupSpawner.PopupType.Negative;

        scorePopupSpawner.PlayScore(popupType, worldPos, i_animTime, i_alphaTime, scoreToAdd, i_yoffset);
        addToScore(scoreToAdd);
    }
    #endregion


    #region PRIVATE API
    private int getKernelScore(DoraKernel i_kernel, float i_multiplier)
    {
        return Mathf.CeilToInt(( i_kernel.IsBurnt? burntBaseScore:goodBaseScore ) * i_multiplier);
    }

    void addToScore(int i_score)
    {
        score += i_score;
    }

    void removeFromScore(int i_score)
    {
        score -= i_score;
    }

    #endregion
}
