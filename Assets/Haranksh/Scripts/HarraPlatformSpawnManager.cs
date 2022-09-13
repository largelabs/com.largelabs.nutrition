using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarraPlatformSpawnManager : MonoBehaviourBase
{
    [SerializeField] private HarraPlatformSpawner harraPlatformSpawner = null;
    [SerializeField] private List<AnchorList> possibleSpawnsPerRow = null;

    [ExposePublicMethod]
    public void GenerateNewMap()
    {
        harraPlatformSpawner.DespawnAllPlatforms();
    }

    [ExposePublicMethod]
    public void spawnAtAllAnchors(HarraPlatformSpawner.PlatformType i_platformType)
    {
        IReadOnlyList<Transform> anchorsInRow = null;
        foreach (AnchorList anchorList in possibleSpawnsPerRow)
        {
            anchorsInRow = anchorList.Anchors;
            foreach (Transform anchor in anchorsInRow)
            {
                harraPlatformSpawner.SpawnHaraPlatform(HarraPlatformSpawner.PlatformType.Green, anchor.position);
            }
        }
    }
}
