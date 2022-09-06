using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using UnityEngine.UI;

public class ScorePopupSpawner : MonoBehaviourBase
{
    public enum PopupType
    {
        Positive,
        Super,
        Negative
    }

    [Header("Spawn Points")]
    [SerializeField] private Transform defaultWorldPosition = null;

    [Header("Color")]
    [SerializeField] private Color positiveColor = Color.white;
    [SerializeField] private Color negativeColor = Color.white;

    [Header("Pooling")]
    [SerializeField] private SpawnPool scorePool = null;

    [Header("SFX")]
    [SerializeField] private AudioClip positiveSound = null;
    [SerializeField] private AudioClip superSound = null;
    [SerializeField] private AudioClip negativeSound = null;

    [Header("Spawn Requirements")]
    [SerializeField] private RectTransform parentCanvasRectTransform = null;
    [SerializeField] private Camera mainCam = null;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private InterpolatorsManager interpolatorManager = null;

    private HashSet<UIFloatingScore> livingPopups = null;

    private AudioSource audioPlayer = null;

    private static readonly string BONUS_SCORE = "Bonus_Score";
    private static readonly string SUPER_BONUS_SCORE = "Super_Bonus_Score";

    protected override void Awake()
    {
        base.Awake();

        if (audioPlayer == null)
        {
            audioPlayer = gameObject.GetComponent<AudioSource>();

            if (audioPlayer == null)
                audioPlayer = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Reset()
    {
        if (scorePool != null)
            scorePool.DespawnAll();

        if (livingPopups != null)
            livingPopups.Clear();
    }

    #region PUBLIC API
    public void PlayScore(PopupType i_popupType,
                          Vector3? i_worldPosition,
                          float i_animTime,
                          float i_alphaTime,
                          int i_score,
                          float i_yOffset)
    {
        playSound(i_popupType);

        if (i_worldPosition == null)
            spawnScorePopup(i_popupType, defaultWorldPosition.position, i_animTime, i_alphaTime, i_score, i_yOffset);
        else
            spawnScorePopup(i_popupType, i_worldPosition.Value, i_animTime, i_alphaTime, i_score, i_yOffset);
    }

    [ExposePublicMethod]
    public void PlayScoreCenter(PopupType i_popupType, int i_score)
    {
        PlayScore(i_popupType, Vector3.zero, 0.5f, 0.2f, i_score, 20f);
    }
    #endregion

    #region PRIVATE API
    private void playSound(PopupType i_popupType)
    {
        if (audioPlayer == null) return;

        if (i_popupType == PopupType.Positive)
        {
            if(positiveSound != null)
                audioPlayer.PlayOneShot(positiveSound);
        }
        else if (i_popupType == PopupType.Negative)
        {
            if(negativeSound != null)
                audioPlayer.PlayOneShot(negativeSound);
        }
        else if (i_popupType == PopupType.Super)
        {
            if(superSound != null)
                audioPlayer.PlayOneShot(superSound);
        }
    }
    
    private Color getColor(PopupType i_popupType)
    {
        if (i_popupType == PopupType.Positive || i_popupType == PopupType.Super)
            return positiveColor;
        else if (i_popupType == PopupType.Negative)
            return negativeColor;
        else
        {
            Debug.LogError("Invalid Popup Type!");
            return Color.white;
        }
    }

    private Transform getPopupPrefab(PopupType i_popupType)
    {
        if (i_popupType == PopupType.Positive || i_popupType == PopupType.Negative)
            return scorePool.Spawn(BONUS_SCORE);
        else if (i_popupType == PopupType.Super)
            return scorePool.Spawn(SUPER_BONUS_SCORE);
        else
            return null;
    }

    private void spawnScorePopup(PopupType i_popupType,
                                 Vector3 i_worldPosition,
                                 float i_animTime,
                                 float i_alphaTime,
                                 int i_score,
                                 float i_yOffset
                                )
    {
        Transform tr = getPopupPrefab(i_popupType);

        if (tr != null)
        {
            UIFloatingScore popup = tr.GetComponent<UIFloatingScore>();

            if (popup != null)
            {
                popup.SetAlpha(0);

                if (livingPopups == null) livingPopups = new HashSet<UIFloatingScore>();
                popup.SetColorNoAlpha(getColor(i_popupType));
                popup.OnAnimationEnded += despawnScorePopup;
                livingPopups.Add(popup);

                popup.Animate(i_worldPosition, i_animTime, i_alphaTime,
                                        i_score, i_yOffset, parentCanvasRectTransform, mainCam,
                                        animCurve, interpolatorManager);
            }
        }
    }

    private void despawnScorePopup(UIFloatingScore i_popup)
    {
        if (i_popup == null) return;
        i_popup.SetAlpha(0);

        i_popup.OnAnimationEnded -= despawnScorePopup;

        if (livingPopups != null)
            livingPopups.Remove(i_popup);

        scorePool?.Despawn(i_popup.transform);
    }
    #endregion
}
