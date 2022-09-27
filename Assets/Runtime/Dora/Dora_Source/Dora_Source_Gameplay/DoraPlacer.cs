using System.Collections.Generic;
using UnityEngine;

public class DoraPlacer : MonoBehaviourBase
{

    [Tooltip("Should follow this order: front right -> back right -> front left -> back left")]
    [SerializeField] private List<Transform> anchors = null;
    [SerializeField] private DoraSpawner doraSpawner = null;

    Queue<Transform> positionQueue = null;


    #region PUBLIC API

    public DoraCellMap SpawnDoraAtAnchor(Transform i_anchor, Vector3 i_offset)
    {
        if (doraSpawner == null)
        {
            Debug.LogError("Dora Spawner is null! Returning...");
            return null;
        }

        if (null == i_anchor) return null;

        return doraSpawner.SpawnDoraCobAtAnchor(i_anchor, i_offset);
    }

    public Transform GetNextAnchor()
    {
        return getQueuedDoraPosition();
    }

    #endregion

    #region PRIVATE

    Transform getQueuedDoraPosition()
    {
        if (null == positionQueue || positionQueue.Count == 0) 
            positionQueue = new Queue<Transform>(CollectionUtilities.Shuffle<Transform>(anchors));

        return positionQueue.Dequeue();
    }

    #endregion

}
