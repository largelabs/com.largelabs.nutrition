using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMinigameTimer : MonoBehaviour
{
    [SerializeField] MinigameTimer timer = null;

    Text timerTxt = null;

    private void Start()
    {
        timerTxt = GetComponent<Text>();
    }

    private void Update()
    {
        timerTxt.text = timer.DisplayTimer(); 
    }

    //OnTimePaused ==> Blink
}
