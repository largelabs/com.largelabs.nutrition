using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HarraGameData", menuName = "ScriptableObjects/Harra/HarraGameData", order = 1)]
public class HarraGameData : ScriptableObject
{
    [SerializeField] int orangeScore = 60;
    [SerializeField] int normalScore = 10;
    [SerializeField] int pileAmount = 4;
    [SerializeField] List<float> pileTimes = null;

    [SerializeField] int maxHarraPerRope = 26;

    public int OrangeScore => orangeScore;
    public int NormalScore => normalScore;
    public int PileAmount => pileAmount;
    public IReadOnlyList<float> PileTimes => pileTimes;

    public int MaxHarraPerRope => maxHarraPerRope;
}
