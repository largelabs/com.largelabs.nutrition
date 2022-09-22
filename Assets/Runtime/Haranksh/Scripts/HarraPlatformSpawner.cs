using PathologicalGames;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HarraPlatformSpawner : MonoBehaviourBase
{
    public enum PlatformType
    {
        Green,
        Yellow,
        Orange
    }

    [SerializeField] private SpawnPool platformPool = null;

    private static readonly string PLATFORM_G = "PlatformG";
    private static readonly string PLATFORM_Y = "PlatformY";
    private static readonly string PLATFORM_R = "PlatformR";

    private List<HaraPlatformAbstract> livingPlatforms = null;

    #region UNITY API
    protected override void Awake()
    {
        base.Awake();

        livingPlatforms = new List<HaraPlatformAbstract>();
    }
    #endregion

    #region PUBLIC API
    public IReadOnlyList<HaraPlatformAbstract> LivingPlatforms => livingPlatforms;

    public HaraPlatformAbstract SpawnHaraPlatform(PlatformType i_platformType, Vector3 i_worldPosition)
    {
        if (platformPool == null)
        {
            Debug.LogError("Harra Platform Pool is null! Returning...");
            return null;
        }

        Transform tr = platformPool.Spawn(getPlatformType(i_platformType));
        tr.position = i_worldPosition;

        HaraPlatformAbstract ret = tr.GetComponent<HaraPlatformAbstract>();

        if (ret == null)
            Debug.LogError("No hara platform component attached to prefab root!");
        else
        {
            livingPlatforms.Add(ret);
            ret.EnableCollider(true);
        }

        return ret;
    }

    public void DespawnHarraPlatform(HaraPlatformAbstract i_haraPlatform)
    {
        despawnHarraPlatform(i_haraPlatform);
        livingPlatforms.Remove(i_haraPlatform);
    }

    public void DespawnAllPlatforms()
    {
        if (platformPool == null) return;
        if (livingPlatforms == null || livingPlatforms.Count < 1) return;

        int length = livingPlatforms.Count;
        for (int i = 0; i < length; i++)
        {
            despawnHarraPlatform(livingPlatforms[i]);
        }

        livingPlatforms.Clear();
    }
    #endregion

    #region PRIVATE API
    private string getPlatformType(PlatformType i_platformType)
    {
        if (i_platformType == PlatformType.Green)
            return PLATFORM_G;
        else if (i_platformType == PlatformType.Yellow)
            return PLATFORM_Y;
        else if (i_platformType == PlatformType.Orange)
            return PLATFORM_R;
        else
        {
            Debug.LogError("Undefined platform type!");
            return null;
        }
    }

    private void despawnHarraPlatform(HaraPlatformAbstract i_haraPlatform)
    {
        platformPool?.Despawn(i_haraPlatform.transform);

        HarraPlatformAnimationManager animations = i_haraPlatform.GetComponent<HarraPlatformAnimationManager>();
        if (animations != null)
            animations.ResetSprite();
    }
    #endregion
}
