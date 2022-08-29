using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraScoreManager : MonoBehaviourBase
{
    [SerializeField] int kernelPostiveScore = 10;
    [SerializeField] int kernelNegativeScore = 15;
    [SerializeField] float increaseOfMultiplier = 0.1f;

    int score = 0;

    #region PUBLIC API

    public void AddScore(int i_numberOfKernels)
    {
        if (i_numberOfKernels == 1) addToScore(kernelPostiveScore);
        else
        {
            float multiplier = 1 + (i_numberOfKernels * increaseOfMultiplier);
            float scoreToAdd = kernelPostiveScore * multiplier * i_numberOfKernels;
            addToScore((int)scoreToAdd);
        }
    }

    public void RemoveScore(int i_numberOfBurntKernels)
    {
        if (i_numberOfBurntKernels == 1) removeFromScore(kernelPostiveScore);
        else
        {
            float multiplier = 1 + (i_numberOfBurntKernels * increaseOfMultiplier);
            float scoreToRemove = kernelPostiveScore * multiplier * i_numberOfBurntKernels;
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
