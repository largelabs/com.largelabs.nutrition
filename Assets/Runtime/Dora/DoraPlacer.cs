using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraPlacer : MonoBehaviourBase
{
    public enum DoraPositions
    {
        FrontRight,
        BackRight,
        BackLeft,
        FrontLeft
    }

    [SerializeField] private List<Transform> anchors = null;

    // spawn cobs
    // place cobs on grill based on anchors
    // register cobs to the mover script
}
