using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraScoreManager : MonoBehaviourBase
{
    [SerializeField] DoraGameData doraGameData = null;
    [SerializeField] float increaseOfMultiplier = 0.1f;

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
    
    public void AddValueScore(int i_score)
    {
        addToScore(i_score);
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
