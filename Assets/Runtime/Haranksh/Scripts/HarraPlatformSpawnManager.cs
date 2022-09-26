using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HarraPlatformSpawnManager : MonoBehaviourBase
{
    [SerializeField] private HarraPlatformSpawner harraPlatformSpawner = null;
    [SerializeField] private List<HarraPlatformRow> platformRows_0 = null;
    [SerializeField] private List<HarraPlatformRow> platformRows_1 = null;
    [SerializeField] private List<HarraPlatformRow> platformRows_2 = null;
    [SerializeField] private int maxOrange = 10;

    [Header("Platform Appear Settings")]
    [SerializeField] int numberToSpawnTogether = 2;
    [SerializeField] float baseSpawnDelay = 0.05f;
    [SerializeField] float spawnDelayVariance = 0.02f;
    [SerializeField] float appearTime = 0.2f;
    [SerializeField] float disappearTime = 0.2f;
    [SerializeField] InterpolatorsManager interpolatorsManager = null;

    int currOrange = 0;

    System.Random rng = null;

    Coroutine platformAnimationRoutine = null;
    Coroutine platformDisappearRoutine = null;

    protected override void Awake()
    {
        base.Awake();

        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        rng = new System.Random((int)DateTime.Now.Ticks);
    }

    #region PUBLIC API
    public bool MapIsAnimating => platformAnimationRoutine != null;

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
        bool prevSolo = false;
        bool prevOrange = false;
        int spawnedInPrevRow = 0;
        int spawnedInCurrRow = 0;
        int iterationsSinceLastSpawn = 0;
        currOrange = 0;

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

            prevSolo = false;
            prevOrange = false;
            spawnedPrev = false;

            spawnedInPrevRow = spawnedInCurrRow;
            spawnedInCurrRow = 0;
            iterationsSinceLastSpawn = 0;

            for (int i = 0; i < length; i++)
            {
                idxRatio = Mathf.Clamp01((float)i / length);

                currGlobalChance = globalChances[getIdxAtRatio(idxRatio, globalChances.Count)];

                currGlobalChance += currGlobalChance * Mathf.Clamp01(iterationsSinceLastSpawn * 0.2f);

                if (spawnedInPrevRow < 2)
                    currGlobalChance *= 2f;

                rng = UnityEngine.Random.Range(0f, 1f);
                if (length == 1 || (prevSolo && prevOrange) || rng <= currGlobalChance) // spawn a platform at anchor
                {
                    harraPlatformSpawner.SpawnHaraPlatform(choosePlatformType(idxRatio, greenChances, yellowChances, orangeChances, ref prevSolo, ref prevOrange, spawnedPrev),
                                                            anchorsInRow[i].position);

                    spawnedPrev = true;
                    spawnedInCurrRow++;
                    iterationsSinceLastSpawn = 0;
                }
                else
                {
                    spawnedPrev = false;
                    iterationsSinceLastSpawn++;
                }
            }
        }
    }

    [ExposePublicMethod]
    public void MapAppear()
    {
        if (platformAnimationRoutine == null)
            platformAnimationRoutine = StartCoroutine(platformAnimationSequence(true));
    }

    [ExposePublicMethod]
    public void DespawnMap(bool i_animated)
    {
        if (i_animated)
        {
            if (platformAnimationRoutine == null)
                platformAnimationRoutine = StartCoroutine(platformAnimationSequence(false));
        }
        else
            harraPlatformSpawner.DespawnAllPlatforms();
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
        ref bool i_prevSolo,
        ref bool i_prevOrange,
        bool i_spawnedPrev)
    {
        float chanceOfGreen = i_greenChances[getIdxAtRatio(i_idxRatio, i_greenChances.Count)];
        float chanceOfYellow = i_yellowChances[getIdxAtRatio(i_idxRatio, i_yellowChances.Count)];
        float chanceOfOrange = i_orangeChances[getIdxAtRatio(i_idxRatio, i_orangeChances.Count)];

        float totalChance = chanceOfGreen + chanceOfYellow + chanceOfOrange;

        float rng = UnityEngine.Random.Range(0f, totalChance);

        HarraPlatformSpawner.PlatformType ret = HarraPlatformSpawner.PlatformType.Green;

        if (rng <= chanceOfGreen)
        {
            i_prevOrange = false;
            ret = HarraPlatformSpawner.PlatformType.Green;
        }
        else if (rng <= chanceOfGreen + chanceOfYellow)
        {
            i_prevOrange = false;
            ret = HarraPlatformSpawner.PlatformType.Yellow;
        }
        else
        {
            if (currOrange < maxOrange && !i_prevOrange && !i_prevSolo)// && (i_spawnedPrev || i_idxRatio == 0))
            {
                i_prevOrange = true;
                ret = HarraPlatformSpawner.PlatformType.Orange;
                currOrange++;
            }
            else
            {
                i_prevOrange = false;
                rng = UnityEngine.Random.Range(0f, chanceOfGreen + chanceOfYellow);
                if (rng <= chanceOfGreen)
                    ret = HarraPlatformSpawner.PlatformType.Green;
                else
                    ret = HarraPlatformSpawner.PlatformType.Yellow;
            }
        }

        if (i_spawnedPrev)
            i_prevSolo = false;
        else
            i_prevSolo = true;

        return ret;
    }

    private List<HarraPlatformRow> getRowList(int i_type)
    {
        if (i_type == 0)
            return platformRows_0;
        else if (i_type == 1)
            return platformRows_1;
        else 
        {
            if (i_type % 2 == 0)
                return platformRows_2;
            else
                return platformRows_1;
        }
    }
    private int getIdxAtRatio(float i_ratio, int i_listCount)
    {
        return Mathf.Clamp(Mathf.FloorToInt(i_ratio * (i_listCount)), 0, i_listCount - 1);
    }

    private IEnumerator platformAnimationSequence(bool i_appear)
    {
        IReadOnlyList<HaraPlatformAbstract> livingPlatforms = harraPlatformSpawner.LivingPlatforms;
        List<HaraPlatformAbstract> shuffledPlatforms = livingPlatforms.OrderBy(a => rng.Next()).ToList();

        float minSpawnDelay = baseSpawnDelay - spawnDelayVariance;
        float maxSpawnDelay = baseSpawnDelay + spawnDelayVariance;

        int length = shuffledPlatforms.Count;
        int spawnedSinceDelay = 0;
        for (int i = 0; i < length; i++)
        {
            GameObject shuffledPlatform = shuffledPlatforms[i].gameObject;
            if (shuffledPlatform.activeSelf == false) shuffledPlatform.SetActive(true);

            if (i_appear)
                shuffledPlatform.GetComponent<HarraPlatformAnimationManager>().PlatformAppear(interpolatorsManager, appearTime);
            else
                shuffledPlatform.GetComponent<HarraPlatformAnimationManager>().PlatformDisppear(interpolatorsManager, disappearTime);

            spawnedSinceDelay++;

            if (spawnedSinceDelay == numberToSpawnTogether && i < length - 1)
            {
                yield return this.Wait(baseSpawnDelay + (UnityEngine.Random.Range(minSpawnDelay, maxSpawnDelay)));
                spawnedSinceDelay = 0;
            }
        }

        yield return this.Wait(i_appear ? appearTime : disappearTime);

        if(i_appear == false)
            harraPlatformSpawner.DespawnAllPlatforms();

        this.DisposeCoroutine(ref platformAnimationRoutine);
    }
    #endregion
}
