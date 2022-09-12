using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoraGameplayData", menuName = "ScriptableObjects/Dora/DoraGameplayData", order = 1)]
public class DoraGameplayData : ScriptableObject
{
    [Header("Default Settings")]
    [SerializeField] private float defaultRotationSpeed = 50f;

    [Header("Frenzy Settings")]
    [SerializeField] private float frenzyTime = 5f;
    [SerializeField] private float frenzyRotationSpeed = 150f;
    [SerializeField] private float cursorAutoMoveSpeed = 10f;

    #region GETTERS
    
    public float DefaultRotationSpeed => defaultRotationSpeed;
    public float FrenzyTime => frenzyTime;
    public float FrenzyRotationSpeed => frenzyRotationSpeed;
    public float CursorAutoMoveSpeed => cursorAutoMoveSpeed;

    #endregion

}
