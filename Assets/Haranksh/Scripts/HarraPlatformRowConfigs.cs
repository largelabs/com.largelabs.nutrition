using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HarraPlatformRowConfig", menuName = "com.largelabs.nutrition/HarraPlatformRowConfig", order = 1)]
public class HarraPlatformRowConfigs : ScriptableObject
{
    [SerializeField] [Range(0f, 1f)] private float globalSpawnChance = 0.8f;

    // each list contains the spawn chance for each section in the row
    // e.g. if greenSpawnChances contains 3 float values then each one of them
    // is mapped to 33% of the row
    [SerializeField] [Range(0f, 1f)] private List<float> greenSpawnChances = null;
    [SerializeField] [Range(0f, 1f)] private List<float> yellowSpawnChances = null;
    [SerializeField] [Range(0f, 1f)] private List<float> orangeSpawnChances = null;

    #region GETTERS
    public float GlobalSpawnChance => globalSpawnChance;

    public IReadOnlyList<float> GreenSpawnChances => greenSpawnChances;
    public IReadOnlyList<float> YellowSpawnChances => yellowSpawnChances;
    public IReadOnlyList<float> OrangeSpawnChances => orangeSpawnChances;

    #endregion
}
