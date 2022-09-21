using UnityEngine.UI;
using UnityEngine;

public class DoraScoreManager : MonoBehaviourBase
{
    [SerializeField] DoraGameData doraGameData = null;
    [SerializeField] float increaseOfMultiplier = 0.1f;
    [SerializeField] private PopupSpawner scorePopupSpawner = null;
    [SerializeField] private Text scoreText = null;

    int score = 0;

    RectTransform scoreRect = null;
    float goodBaseScore = 0f;
    float burntBaseScore = 0f;

    protected override void Awake()
    {
        base.Awake();
        scoreRect = scoreText.rectTransform;
        goodBaseScore = doraGameData.GoodKernelScore;
        burntBaseScore = doraGameData.BurntKernelScore;
    }

    #region PUBLIC API

    public void ResetScoreManager()
    {
        addToScore(-score);
    }

    public RectTransform ScoreRect => scoreRect;

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
                                PopupSpawner.PopupType i_popupType,
                                Vector3 i_worldPos,
                                float i_animTime,
                                float i_alphaTime,
                                float i_yoffset
                                )
    {
        scorePopupSpawner.PlayPopup(i_popupType, i_worldPos, i_animTime, i_alphaTime, i_score, i_yoffset);
        addToScore(i_score);
    }
    
    public void AddScoreByValue(int i_score,
                                PopupSpawner.PopupType i_popupType,
                                RectTransform i_anchor,
                                float i_animTime,
                                float i_alphaTime,
                                float i_yoffset
                                )
    {
        scorePopupSpawner.PlayPopupWithAnchor(i_popupType, i_anchor, i_animTime, i_alphaTime, i_score, i_yoffset);
        addToScore(i_score);
    } 
    
    public void AddScoreByStatus(ScoreKernelInfo i_info,
                                RectTransform i_anchor,
                                float i_animTime,
                                float i_alphaTime,
                                float i_yoffset
                                )
    {
        int scoreToAdd = getKernelScoreByStatus(i_info.KernelStatus, i_info.ScoreMultiplier);
        scorePopupSpawner.PlayPopupWithAnchor(getPopupType(i_info.KernelStatus), i_anchor, i_animTime, i_alphaTime,
                                    scoreToAdd, i_yoffset);
        addToScore(scoreToAdd);
    }
    
    public void AddScoreByStatus(KernelStatus i_status,
                                float i_multiplier,
                                Vector3 i_worldPos,
                                float i_animTime,
                                float i_alphaTime,
                                float i_yoffset
                                )
    {
        int scoreToAdd = getKernelScoreByStatus(i_status, i_multiplier);
        scorePopupSpawner.PlayPopup(getPopupType(i_status), i_worldPos, i_animTime, i_alphaTime,
                                    scoreToAdd, i_yoffset);
        addToScore(scoreToAdd);
    }
    #endregion


    #region PRIVATE API
    private PopupSpawner.PopupType getPopupType(KernelStatus i_status)
    {
        if (i_status == KernelStatus.Super)
            return PopupSpawner.PopupType.Positive;
        else if (i_status == KernelStatus.Normal)
            return PopupSpawner.PopupType.Positive;
        else if (i_status == KernelStatus.Burnt)
            return PopupSpawner.PopupType.Negative;
        else
        {
            Debug.LogError("invalid kernel status");
            return PopupSpawner.PopupType.Positive;
        }
    }

    private int getKernelScoreByStatus(KernelStatus i_status, float i_multiplier)
    {
        float score = 0;

        if (i_status == KernelStatus.Super)
            score = goodBaseScore;
        else if (i_status == KernelStatus.Normal)
            score = goodBaseScore;
        else if (i_status == KernelStatus.Burnt)
            score = burntBaseScore;

        return Mathf.CeilToInt(score * i_multiplier);
    }

    void addToScore(int i_score)
    {
        score += i_score;
        score = Mathf.Clamp(score, 0, 999999);
        scoreText.text = score.ToString("000000");

    }

    void removeFromScore(int i_score)
    {
        score -= i_score;
    }

    #endregion
}