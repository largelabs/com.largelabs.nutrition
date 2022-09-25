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

    #region PUBLIC API

    [ExposePublicMethod]
    public DoraCellMap SpawnDoraAtAnchor(DoraPositions i_doraPosition)
    {
        if (doraSpawner == null)
        {
            Debug.LogError("Dora Spawner is null! Returning...");
            return null;
        }

        int spawnIndex = (int)i_doraPosition;
        DoraCellMap currCob = null;
        if (spawnIndex < 0 || spawnIndex >= anchors.Count)
        {
            Debug.LogError("No valid anchor provided for chosen dora position! Returning!");
            return null;
        }
        else
        {
            currCob = doraSpawner.SpawnDoraCobAtAnchor(anchors[spawnIndex]);
        }

        return currCob;
    }

    #endregion

}
