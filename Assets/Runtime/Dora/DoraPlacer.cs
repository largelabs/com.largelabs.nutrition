using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraPlacer : MonoBehaviourBase
{
    public enum DoraPositions
    {
        FrontRight,
        BackRight,
        FrontLeft,
        BackLeft
    }

    [Tooltip("Should follow this order: front right -> back right -> front left -> back left")]
    [SerializeField] private List<Transform> anchors = null;
    [SerializeField] private DoraSpawner doraSpawner = null;
    [SerializeField] private DoraMover doraMover = null;

    // spawn cobs
    // place cobs on grill based on anchors
    // register cobs to the mover script

    #region PUBLIC API
    [ExposePublicMethod]
    public void SpawnDoraAtAllAnchors()
    {
        if (doraSpawner == null)
        {
            Debug.LogError("Dora Spawner is null! Returning...");
            return;
        }

        int length = anchors.Count;
        DoraCellMap currCob = null;
        for (int i = 0; i < length; i++)
        {
            currCob = doraSpawner.SpawnDoraCobAtAnchor(anchors[i]);

            if (doraMover != null)
            {
                if (currCob != null)
                    doraMover.RegisterCob(currCob.transform);
                else
                    Debug.LogError("Cob is null, not going to register!");
            }
            else
                Debug.LogError("Dora Mover is null, Cob not registered!");
        }
    } 
    
    [ExposePublicMethod]
    public void SpawnDoraAtAnchor(DoraPositions i_doraPosition)
    {
        if (doraSpawner == null)
        {
            Debug.LogError("Dora Spawner is null! Returning...");
            return;
        }

        int spawnIndex = (int)i_doraPosition;
        DoraCellMap currCob = null;
        if (spawnIndex < 0 || spawnIndex >= anchors.Count)
        {
            Debug.LogError("No valid anchor provided for chosen dora position! Returning!");
            return;
        }
        else
        {
            currCob = doraSpawner.SpawnDoraCobAtAnchor(anchors[spawnIndex]);

            if (doraMover != null)
            {
                if (currCob != null)
                    doraMover.RegisterCob(currCob.transform);
                else
                    Debug.LogError("Cob is null, not going to register!");
            }
            else
                Debug.LogError("Dora Mover is null, Cob not registered!");
        }
    }
    #endregion

    #region PRIVATE API

    #endregion
}
