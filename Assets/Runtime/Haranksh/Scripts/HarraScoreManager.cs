using UnityEngine;
using UnityEngine.UI;

public class HarraScoreManager : MonoBehaviourBase
{
    [SerializeField] PopupSpawner scorePopups = null;
    [SerializeField] int platformSore = 60;
    [SerializeField] Text scoreText = null;

    private int totalScore = 0;
    public void AddScore(Vector3 i_platformPos)
    {
        // sfx suggestion: popup score sound (can be added by assigning audio clip on the scorepopup prefab)

        scorePopups.PlayPopup(PopupSpawner.PopupType.Positive, i_platformPos, 0.5f, 0.1f, platformSore, 10f);
        totalScore += platformSore;
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
