using UnityEngine;
using UnityEngine.UI;

public class HarraScoreManager : MonoBehaviourBase
{
    [SerializeField] PopupSpawner scorePopups = null;
    [SerializeField] Text scoreText = null;
    [SerializeField] HarraSFXProvider sfxProvider = null;

    private int totalScore = 0;

    public void AddScore(Vector3 i_platformPos, int i_score)
    {
        // sfx suggestion: popup score sound (can be added by assigning audio clip on the scorepopup prefab)
        sfxProvider.PlayScoreSFX();

        scorePopups.PlayPopup(PopupSpawner.PopupType.Positive, i_platformPos, 0.5f, 0.1f, i_score, 20f);
        totalScore += i_score;

        string temp = totalScore.ToString();

        while (temp.Length < 4)
            temp = "0" + temp;
        scoreText.text = temp;
    }

    public int TotalScore
    {
        get { return totalScore; }
    }
}
