using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraScoreManager : MonoBehaviourBase
{
    [SerializeField] DoraGameData doraGameData = null;
    [SerializeField] float increaseOfMultiplier = 0.1f;
    [SerializeField] private ScorePopupSpawner scorePopupSpawner = null;

    int score = 0;

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
        int scoreToAdd = 0;
        ScorePopupSpawner.PopupType popupType = ScorePopupSpawner.PopupType.Positive;
        Vector3 worldPos = Vector3.zero;
        // get position from first kernel (which should be center)
        // multiplier increases in each loop (after each kernel in the stack)


        scorePopupSpawner.PlayScore(popupType, worldPos, i_animTime, i_alphaTime, scoreToAdd, i_yoffset);
        addToScore(scoreToAdd);
    }
    #endregion


    #region PRIVATE API

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
