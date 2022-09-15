using System;
using System.Collections.Generic;
using UnityEngine;

public class HarraPlatformSpawnManager : MonoBehaviourBase
{
    [SerializeField] private HarraPlatformSpawner harraPlatformSpawner = null;
    [SerializeField] private List<HarraPlatformRow> platformRows = null;

    protected override void Awake()
    {
        base.Awake();

        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
    }

    [ExposePublicMethod]
    public void GenerateNewMap()
    {
        harraPlatformSpawner.DespawnAllPlatforms();

        int length = 0;
        float rng = 0f;
        float globalChance = 0f;
        IReadOnlyList<float> greenChances = null;
        IReadOnlyList<float> yellowChances = null;
        IReadOnlyList<float> orangeChances = null;

        IReadOnlyList<Transform> anchorsInRow = null;
        foreach (HarraPlatformRow platformRow in platformRows)
        {
            anchorsInRow = platformRow.Anchors;
            length = anchorsInRow.Count;

            globalChance = platformRow.GlobalSpawnChance;
            greenChances = platformRow.GreenSpawnChances;
            yellowChances = platformRow.YellowSpawnChances;
            orangeChances = platformRow.OrangeSpawnChances;

            for (int i = 0; i < length; i++)
            {
                rng = UnityEngine.Random.Range(0f, 1f);
                if (rng < globalChance) // spawn a platform at anchor
                {
                    harraPlatformSpawner.SpawnHaraPlatform(choosePlatformType((float)i/length, greenChances, yellowChances, orangeChances),
                                                            anchorsInRow[i].position);
                }
            }
        }
    }

    private HarraPlatformSpawner.PlatformType choosePlatformType(float i_idxRatio, 
        IReadOnlyList<float> i_greenChances, 
        IReadOnlyList<float> i_yellowChances, 
        IReadOnlyList<float> i_orangeChances)
    {
        float ratio = Mathf.Clamp01(i_idxRatio);
        float chanceOfGreen = i_greenChances[Mathf.FloorToInt(ratio * (i_greenChances.Count - 1))];
        float chanceOfYellow = i_yellowChances[Mathf.FloorToInt(ratio * (i_yellowChances.Count - 1))];
        float chanceOfOrange = i_orangeChances[Mathf.FloorToInt(ratio * (i_orangeChances.Count - 1))];

        float totalChance = chanceOfGreen + chanceOfYellow + chanceOfOrange;

        float rng = UnityEngine.Random.Range(0f, totalChance);

        if (rng <= chanceOfGreen)
            return HarraPlatformSpawner.PlatformType.Green;
        else if (rng <= chanceOfGreen + chanceOfYellow)
            return HarraPlatformSpawner.PlatformType.Yellow;
        else
            return HarraPlatformSpawner.PlatformType.Orange;
    }

    [ExposePublicMethod]
    public void SpawnAtAllAnchors(HarraPlatformSpawner.PlatformType i_platformType)
    {
        IReadOnlyList<Transform> anchorsInRow = null;
        foreach (HarraPlatformRow platformRow in platformRows)
        {
            anchorsInRow = platformRow.Anchors;
            foreach (Transform anchor in anchorsInRow)
            {
                harraPlatformSpawner.SpawnHaraPlatform(HarraPlatformSpawner.PlatformType.Green, anchor.position);
            }
        }
    }
}
