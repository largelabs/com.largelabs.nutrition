using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerAction2D : MonoBehaviourBase
{
    public Action OnTriggerAction = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerAction?.Invoke();
    }
}
