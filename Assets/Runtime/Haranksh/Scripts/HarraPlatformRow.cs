using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarraPlatformRow : AnchorList
{
    [Header("Preset Config")]
    [SerializeField] private HarraPlatformRowConfigs spawnConfigs = null;

    [Header("Config Overrides")]
    [SerializeField] [Range(0f, 1f)] private List<float> globalSpawnChances = null;
    [SerializeField] private bool overrideGlobal = false;

    [SerializeField] [Range(0f, 1f)] private List<float> greenSpawnChances = null;
    [SerializeField] private bool overrideGreen = false;

    [SerializeField] [Range(0f, 1f)] private List<float> yellowSpawnChances = null;
    [SerializeField] private bool overrideYellow = false;

    [SerializeField] [Range(0f, 1f)] private List<float> orangeSpawnChances = null;
    [SerializeField] private bool overrideOrange = false;


    #region GETTERS
    public IReadOnlyList<float> GlobalSpawnChances => (overrideGlobal || spawnConfigs == null)? globalSpawnChances:spawnConfigs.GlobalSpawnChances;
    public IReadOnlyList<float> GreenSpawnChances => (overrideGreen || spawnConfigs == null) ? greenSpawnChances:spawnConfigs.GreenSpawnChances;
    public IReadOnlyList<float> YellowSpawnChances => (overrideYellow || spawnConfigs == null) ? yellowSpawnChances:spawnConfigs.YellowSpawnChances;
    public IReadOnlyList<float> OrangeSpawnChances => (overrideOrange || spawnConfigs == null) ? orangeSpawnChances:spawnConfigs.OrangeSpawnChances;
    #endregion
}