using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoraGameData", menuName = "ScriptableObjects/Dora/DoraGameData", order = 1)]
public class DoraGameData : ScriptableObject
{
    [SerializeField] private int goodKernelScore = 50;
    [SerializeField] private int burntKernelScore = -25;
    [SerializeField] private int superKernelScore = 300;

    [SerializeField] private float baseTimer = 120f;

    #region GETTERS
    public int GoodKernelScore => goodKernelScore;
    public int BurntKernelScore => burntKernelScore;
    public int SuperKernelScore => superKernelScore;

    public float BaseTimer => baseTimer;
    #endregion
}
