using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMinigameTimer : MonoBehaviour
{
    [SerializeField] MinigameTimer timer = null;
    [SerializeField] Text secondsTxt = null;
    [SerializeField] Text minutesTxt = null;

    private void Update()
    {
        minutesTxt.text = timer.GetMinutesString();
        secondsTxt.text = timer.GetSecondsString();
    }
}
