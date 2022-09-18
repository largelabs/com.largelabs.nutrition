using System;
using System.Collections.Generic;
using UnityEngine;

public class HarraPlatformSpawnManager : MonoBehaviourBase
{
    [SerializeField] private HarraPlatformSpawner harraPlatformSpawner = null;
    [SerializeField] private List<HarraPlatformRow> platformRows_0 = null;
    [SerializeField] private List<HarraPlatformRow> platformRows_1 = null;
    [SerializeField] private List<HarraPlatformRow> platformRows_2 = null;

    protected override void Awake()
    {
        base.Awake();

        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
    }

    #region PUBLIC API

    [ExposePublicMethod]
    public void GenerateNewMap(int i_type)
    {
        List<HarraPlatformRow> chosenList = getRowList(i_type);

        harraPlatformSpawner.DespawnAllPlatforms();

        int length = 0;

        float rng = 0f;
        IReadOnlyList<float> globalChances = null;
        float currGlobalChance = 0f;
        float idxRatio = 0f;

        bool spawnedPrev = false;
        bool prevOrange = false;
        int spawnedInPrevRow = 0;
        int spawnedInCurrRow = 0;

        IReadOnlyList<float> greenChances = null;
        IReadOnlyList<float> yellowChances = null;
        IReadOnlyList<float> orangeChances = null;

        IReadOnlyList<Transform> anchorsInRow = null;
        foreach (HarraPlatformRow platformRow in chosenList)
        {
            anchorsInRow = platformRow.Anchors;
            length = anchorsInRow.Count;

            globalChances = platformRow.GlobalSpawnChances;
            greenChances = platformRow.GreenSpawnChances;
            yellowChances = platformRow.YellowSpawnChances;
            orangeChances = platformRow.OrangeSpawnChances;

            prevOrange = false;
            spawnedPrev = false;

            spawnedInPrevRow = spawnedInCurrRow;
            spawnedInCurrRow = 0;

            for (int i = 0; i < length; i++)
            {
                idxRatio = Mathf.Clamp01((float)i / length);

                currGlobalChance = globalChances[getIdxAtRatio(idxRatio, globalChances.Count)];

                if (spawnedInPrevRow < 2)
                    currGlobalChance *= 2f;

                rng = UnityEngine.Random.Range(0f, 1f);
                if (prevOrange || rng <= currGlobalChance) // spawn a platform at anchor
                {
                    harraPlatformSpawner.SpawnHaraPlatform(choosePlatformType(idxRatio, greenChances, yellowChances, orangeChances, out prevOrange, spawnedPrev),
                                                            anchorsInRow[i].position);

                    spawnedPrev = true;
                    spawnedInCurrRow++;
                }
                else
                    spawnedPrev = false;
            }
        }
    }

    [ExposePublicMethod]
    public void SpawnAtAllAnchors(HarraPlatformSpawner.PlatformType i_platformType, int i_type)
    {
        List<HarraPlatformRow> chosenList = getRowList(i_type);

        IReadOnlyList<Transform> anchorsInRow = null;
        foreach (HarraPlatformRow platformRow in chosenList)
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
        IReadOnlyList<float> i_orangeChances,
        out bool o_prevOrange,
        bool i_spawnedPrev)
    {
        float chanceOfGreen = i_greenChances[getIdxAtRatio(i_idxRatio, i_greenChances.Count)];
        float chanceOfYellow = i_yellowChances[getIdxAtRatio(i_idxRatio, i_yellowChances.Count)];
        float chanceOfOrange = i_orangeChances[getIdxAtRatio(i_idxRatio, i_orangeChances.Count)];

        float totalChance = chanceOfGreen + chanceOfYellow + chanceOfOrange;

        float rng = UnityEngine.Random.Range(0f, totalChance);

        o_prevOrange = false;

        if (rng <= chanceOfGreen)
            return HarraPlatformSpawner.PlatformType.Green;
        else if (rng <= chanceOfGreen + chanceOfYellow)
            return HarraPlatformSpawner.PlatformType.Yellow;
        else
        {
            if (i_spawnedPrev || i_idxRatio == 0)
            {
                o_prevOrange = true;
                return HarraPlatformSpawner.PlatformType.Orange;
            }
            else
            {
                rng = UnityEngine.Random.Range(0f, chanceOfGreen + chanceOfYellow);

                o_prevOrange = false;

                if (rng <= chanceOfGreen)
                    return HarraPlatformSpawner.PlatformType.Green;
                else
                    return HarraPlatformSpawner.PlatformType.Yellow;
            }
        }
    }

    private List<HarraPlatformRow> getRowList(int i_type)
    {
        if (i_type == 0)
            return platformRows_0;
        else if (i_type == 1)
            return platformRows_1;
        else
            return platformRows_2;
    }
    private int getIdxAtRatio(float i_ratio, int i_listCount)
    {
        return Mathf.Clamp(Mathf.FloorToInt(i_ratio * (i_listCount)), 0, i_listCount - 1);
    }
    #endregion
}
