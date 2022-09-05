using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using UnityEngine.UI;

public class ScorePopupSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform defaultWorldPosition = null;

    [Header("Color")]
    [SerializeField] private Color positiveColor = Color.white;
    [SerializeField] private Color negativeColor = Color.white;

    [Header("Pooling")]
    [SerializeField] private SpawnPool scorePool = null;
    [SerializeField] private RectTransform parentCanvasRectTransform = null;

    [Header("SFX")]
    [SerializeField] private AudioClip successSound = null;
    [SerializeField] private AudioClip superSuccessSound = null;
    [SerializeField] private AudioClip failureSound = null;

    private List<UIFloatingScore> livingPopups = null;

    private AudioSource audioPlayer = null;

    private static readonly string BONUS_SCORE = "Bonus_Score";
    private static readonly string SUPER_BONUS_SCORE = "Super_Bonus_Score";

    private void Awake()
    {
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
    public void PlayPositiveScore()
    {

    }

    public void PlayNegativeScore()
    {

    }

    public void PlaySuperScore()
    {

    }
    #endregion

    #region PRIVATE API
    private void spawnScorePopup()
    {

    }

    private void despawnScorePopup()
    {

    }
    #endregion
}
