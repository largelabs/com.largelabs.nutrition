using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class PopupSpawner : MonoBehaviourBase
{
    public enum PopupType
    {
        Positive,
        Super,
        Negative,
        TimeBonus
    }

    [Header("Spawn Points")]
    [SerializeField] private Transform defaultWorldPosition = null;

    [Header("Color")]
    [SerializeField] private Color positiveColor = Color.white;
    [SerializeField] private Color negativeColor = Color.white;
    [SerializeField] private Color superColor = Color.white;

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
    private static readonly string BONUS_TIME = "Bonus_Time";

    private int counter = 0;

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
    public void PlayPopup(PopupType i_popupType,
                          Vector3? i_worldPosition,
                          float i_animTime,
                          float i_alphaTime,
                          int i_score,
                          //bool i_isSeconds,
                          float i_yOffset)
    {
        playSound(i_popupType);

        if (i_worldPosition == null)
            spawnScorePopup(i_popupType, defaultWorldPosition.position, i_animTime, i_alphaTime, i_score/*, i_isSeconds*/, i_yOffset);
        else
            spawnScorePopup(i_popupType, i_worldPosition.Value, i_animTime, i_alphaTime, i_score/*, i_isSeconds*/, i_yOffset);
    }
    
    public void PlayPopupWithAnchor(PopupType i_popupType,
                          RectTransform i_anchor,
                          float i_animTime,
                          float i_alphaTime,
                          int i_score,
                          float i_yOffset)
    {
        playSound(i_popupType);

        spawnScorePopupWithAnchor(i_popupType, i_anchor, i_animTime, i_alphaTime, i_score, i_yOffset);
    }

    [ExposePublicMethod]
    public void PlayPopupCenter(PopupType i_popupType, int i_score)
    {
        PlayPopup(i_popupType, mainCam.transform.position, 1.0f, 0.5f, i_score, 20f);
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
        if (i_popupType == PopupType.Positive)
            return positiveColor;
        else if (i_popupType == PopupType.Negative)
            return negativeColor;
        else if (i_popupType == PopupType.Super)
            return superColor;
        else if (i_popupType == PopupType.TimeBonus)
            return positiveColor;
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
        else if (i_popupType == PopupType.TimeBonus)
            return scorePool.Spawn(BONUS_TIME);
        else
            return null;
    }

    private void spawnScorePopup(PopupType i_popupType,
                                 Vector3 i_worldPosition,
                                 float i_animTime,
                                 float i_alphaTime,
                                 int i_score,
                                // bool i_isSeconds,
                                 float i_yOffset
                                )
    {
        Transform tr = getPopupPrefab(i_popupType);
        counter++;
        //Debug.LogError("spawned: " + tr.gameObject + " score spawned: " + counter);

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
                                        i_score/*, i_isSeconds*/, i_yOffset, parentCanvasRectTransform, mainCam,
                                        animCurve, interpolatorManager);
            }
        }
    } 
    
    private void spawnScorePopupWithAnchor(PopupType i_popupType,
                                            RectTransform i_anchor,
                                            float i_animTime,
                                            float i_alphaTime,
                                            int i_score,
                                            float i_yOffset)
    {
        Transform tr = getPopupPrefab(i_popupType);
        counter++;
        //Debug.LogError("spawned: " + tr.gameObject + " score spawned: " + counter);

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

                popup.AnimateWithAnchor(i_anchor, i_animTime, i_alphaTime,
                                        i_score, i_yOffset, animCurve, interpolatorManager);
            }
        }
    }

    private void despawnScorePopup(UIFloatingScore i_popup)
    {
        if (i_popup == null) return;
        //Debug.LogError("despawned: " + i_popup.gameObject);
        i_popup.SetAlpha(0);

        i_popup.OnAnimationEnded -= despawnScorePopup;

        if (livingPopups != null)
            livingPopups.Remove(i_popup);

        scorePool?.Despawn(i_popup.transform);
    }
    #endregion
}
