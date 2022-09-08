using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class DoraScoreManager : MonoBehaviourBase
{
    [SerializeField] DoraGameData doraGameData = null;
    [SerializeField] float increaseOfMultiplier = 0.1f;
    [SerializeField] private ScorePopupSpawner scorePopupSpawner = null;
    [SerializeField] private Text scoreText = null;

    int score = 0;

    float goodBaseScore = 0f;
    float burntBaseScore = 0f;
    float superBaseScore = 0f;

    protected override void Awake()
    {
        base.Awake();
        goodBaseScore = doraGameData.GoodKernelScore;
        burntBaseScore = doraGameData.BurntKernelScore;
        superBaseScore = doraGameData.SuperKernelScore;
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
    
    public void AddScoreByValue(int i_score,
                                ScorePopupSpawner.PopupType i_popupType,
                                RectTransform i_anchor,
                                float i_animTime,
                                float i_alphaTime,
                                float i_yoffset
                                )
    {
        scorePopupSpawner.PlayScoreWithAnchor(i_popupType, i_anchor, i_animTime, i_alphaTime, i_score, i_yoffset);
        addToScore(i_score);
    } 
    
    public void AddScoreByStatus(DoraKernel.KernelStatus i_status,
                                float i_multiplier,
                                RectTransform i_anchor,
                                float i_animTime,
                                float i_alphaTime,
                                float i_yoffset
                                )
    {
        int scoreToAdd = getKernelScoreByStatus(i_status, i_multiplier);
        scorePopupSpawner.PlayScoreWithAnchor(getPopupType(i_status), i_anchor, i_animTime, i_alphaTime,
                                    scoreToAdd, i_yoffset);
        addToScore(scoreToAdd);
    }
    
    public void AddScoreByStatus(DoraKernel.KernelStatus i_status,
                                float i_multiplier,
                                Vector3 i_worldPos,
                                float i_animTime,
                                float i_alphaTime,
                                float i_yoffset
                                )
    {
        int scoreToAdd = getKernelScoreByStatus(i_status, i_multiplier);
        scorePopupSpawner.PlayScore(getPopupType(i_status), i_worldPos, i_animTime, i_alphaTime,
                                    scoreToAdd, i_yoffset);
        addToScore(scoreToAdd);
    }


    //public void AddScoreByKernels(IReadOnlyList<HashSet<DoraKernel>> i_eatenKernels,
    //                              float i_animTime,
    //                              float i_alphaTime,
    //                              float i_yoffset
    //                              )
    //{
    //    if (i_eatenKernels == null || i_eatenKernels.Count == 0)
    //    {
    //        Debug.LogError("Invalid eaten kernel collection! Returning...");
    //        return;
    //    }

    //    //ScorePopupSpawner.PopupType popupType = ScorePopupSpawner.PopupType.Positive;

    //    //if (i_eatenKernels.Count > 13) popupType = ScorePopupSpawner.PopupType.Super;

    //    //if (i_eatenKernels[0].Count > 1)
    //    //{
    //    //    Debug.LogError("More than one origin cell! Returning...");
    //    //    return;
    //    //}

    //    //handle origin kernel alone to get world pos
    //    //foreach (DoraKernel kernel in i_eatenKernels[0])
    //    //    currKernel = kernel;
    //    //if (currKernel.IsBurnt) popupType = ScorePopupSpawner.PopupType.Positive;
    //    //Vector3 worldPos = currKernel.transform.position;

    //    //scoreToAdd = getKernelScore(currKernel, multiplier);
    //    //scoreManagerKernels.Add(new ScoreManagerKernel(scoreToAdd, currKernel.Status));
    //    //totalScoreToAdd += scoreToAdd;

    //    // multiplier increases in each loop (after each selection step in the list)
    //    //Vector3 originWorldPos = Vector3.zero;
    //    int scoreToAdd = 0;
    //    int totalScoreToAdd = 0;
    //    float multiplier = 1f;
    //    List<ScoreManagerKernel> scoreManagerKernels = new List<ScoreManagerKernel>();
    //    HashSet<DoraKernel> currSet = null;
    //    int length = i_eatenKernels.Count;
    //    for (int i = 0; i < length; i++)
    //    {
    //        currSet = i_eatenKernels[i];
    //        foreach (DoraKernel kernel in currSet)
    //        {
    //            //if (kernel.IsBurnt) popupType = ScorePopupSpawner.PopupType.Positive;
    //            //if (i == 0)
    //                //originWorldPos = kernel.transform.position;
    //            scoreToAdd = getKernelScore(kernel, multiplier);
    //            scoreManagerKernels.Add(new ScoreManagerKernel(scoreToAdd, kernel.Status));
    //            totalScoreToAdd += scoreToAdd;
    //        }
    //        multiplier += 1;
    //    }

    //    uiKernelManager.HandleKernelStack(scoreManagerKernels);

    //    //if (scoreToAdd < 0)
    //        //popupType = ScorePopupSpawner.PopupType.Negative;

    //    //scorePopupSpawner.PlayScore(ScorePopupSpawner.PopupType.Positive, originWorldPos, i_animTime, i_alphaTime, totalScoreToAdd, i_yoffset);
    //    //addToScore(totalScoreToAdd);
    //}
    #endregion


    #region PRIVATE API
    private ScorePopupSpawner.PopupType getPopupType(DoraKernel.KernelStatus i_status)
    {
        if (i_status == DoraKernel.KernelStatus.Super)
            return ScorePopupSpawner.PopupType.Super;
        else if (i_status == DoraKernel.KernelStatus.Normal)
            return ScorePopupSpawner.PopupType.Positive;
        else if (i_status == DoraKernel.KernelStatus.Burnt)
            return ScorePopupSpawner.PopupType.Negative;
        else
        {
            Debug.LogError("invalid kernel status");
            return ScorePopupSpawner.PopupType.Positive;
        }
    }

    private int getKernelScore(DoraKernel i_kernel, float i_multiplier)
    {
        return getKernelScoreByStatus(i_kernel.Status, i_multiplier);
    }
    
    private int getKernelScoreByStatus(DoraKernel.KernelStatus i_status, float i_multiplier)
    {
        float score = 0;

        if (i_status == DoraKernel.KernelStatus.Super)
            score = superBaseScore;
        else if (i_status == DoraKernel.KernelStatus.Normal)
            score = goodBaseScore;
        else if (i_status == DoraKernel.KernelStatus.Burnt)
            score = burntBaseScore;

        return Mathf.CeilToInt(score * i_multiplier);
    }

    void addToScore(int i_score)
    {
        score += i_score;
        scoreText.text = score.ToString();
    }

    void removeFromScore(int i_score)
    {
        score -= i_score;
    }

    #endregion
}