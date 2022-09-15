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

    #region PUBLIC API

    [ExposePublicMethod]
    public void GenerateNewMap()
    {
        harraPlatformSpawner.DespawnAllPlatforms();

        int length = 0;

        float rng = 0f;
        IReadOnlyList<float> globalChances = null;
        float currGlobalChance = 0f;
        float idxRatio = 0f;

        IReadOnlyList<float> greenChances = null;
        IReadOnlyList<float> yellowChances = null;
        IReadOnlyList<float> orangeChances = null;

        IReadOnlyList<Transform> anchorsInRow = null;
        foreach (HarraPlatformRow platformRow in platformRows)
        {
            anchorsInRow = platformRow.Anchors;
            length = anchorsInRow.Count;

            globalChances = platformRow.GlobalSpawnChances;
            greenChances = platformRow.GreenSpawnChances;
            yellowChances = platformRow.YellowSpawnChances;
            orangeChances = platformRow.OrangeSpawnChances;

            for (int i = 0; i < length; i++)
            {
                idxRatio = Mathf.Clamp01((float)i / length);
                currGlobalChance = globalChances[getIdxAtRatio(idxRatio, globalChances.Count)];
                rng = UnityEngine.Random.Range(0f, 1f);
                if (rng <= currGlobalChance) // spawn a platform at anchor
                {
                    harraPlatformSpawner.SpawnHaraPlatform(choosePlatformType(idxRatio, greenChances, yellowChances, orangeChances),
                                                            anchorsInRow[i].position);
                }
            }
        }
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
    #endregion

    #region PRIVATE
    private HarraPlatformSpawner.PlatformType choosePlatformType(float i_idxRatio,
        IReadOnlyList<float> i_greenChances,
        IReadOnlyList<float> i_yellowChances,
        IReadOnlyList<float> i_orangeChances)
    {
        float chanceOfGreen = i_greenChances[getIdxAtRatio(i_idxRatio, i_greenChances.Count)];
        float chanceOfYellow = i_yellowChances[getIdxAtRatio(i_idxRatio, i_yellowChances.Count)];
        float chanceOfOrange = i_orangeChances[getIdxAtRatio(i_idxRatio, i_orangeChances.Count)];

        float totalChance = chanceOfGreen + chanceOfYellow + chanceOfOrange;

        float rng = UnityEngine.Random.Range(0f, totalChance);

        if (rng <= chanceOfGreen)
            return HarraPlatformSpawner.PlatformType.Green;
        else if (rng <= chanceOfGreen + chanceOfYellow)
            return HarraPlatformSpawner.PlatformType.Yellow;
        else
            return HarraPlatformSpawner.PlatformType.Orange;
    }

    private int getIdxAtRatio(float i_ratio, int i_listCount)
    {
        return Mathf.Clamp(Mathf.FloorToInt(i_ratio * (i_listCount)), 0, i_listCount - 1);
    }
    #endregion
}
