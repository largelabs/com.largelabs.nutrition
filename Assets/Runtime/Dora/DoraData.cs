using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoraData", menuName = "ScriptableObjects/DoraData", order = 1)]
public class DoraData : ScriptableObject
{
    [Tooltip("The maximum possible value of the initial durability of each kernel")]
    [SerializeField] [Range(0.01f, 1.0f)] private float maxInitialDurability = 1.0f;

    [Tooltip("The minimum possible value of the initial durability of each kernel")]
    [SerializeField] [Range(0.01f, 1.0f)] private float minInitialDurability = 0.7f;

    [Tooltip("The wait time (in seconds) between each corn-wide durability loss for kernels")]
    [SerializeField] [Range(0.0f, 10.0f)] private float durabilityLossInterval = 0.1f;

    [Tooltip("The total time (in seconds) it would take for a kernel on this corn to go from durability 1.0 to durability 0.0")]
    [SerializeField] [Range(0.0f, 300.0f)] private float durabilityFullLossTime = 30.0f;

    public float MaxInitialDurability => maxInitialDurability;
    public float MinInitialDurability => minInitialDurability;
    public float DurabilityLossInterval => durabilityLossInterval;
    public float DurabilityLossFullTime => durabilityFullLossTime;
    public float DurabilityLossPerInterval 
    { 
        get { return (1f / durabilityFullLossTime * durabilityLossInterval); } 
    }
}
