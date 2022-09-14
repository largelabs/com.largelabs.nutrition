using System.Collections.Generic;
using UnityEngine;

public class AnchorList : MonoBehaviourBase
{
    [SerializeField] private List<Transform> anchors = null;

    public IReadOnlyList<Transform> Anchors => anchors;

    protected override void Awake()
    {
        base.Awake();

        Transform[] temp = GetComponentsInChildren<Transform>();

        int length = temp.Length;
        for (int i = 0; i < length; i++)
        {
            if (temp[i] != transform)
                anchors.Add(temp[i]);
        }
    }
}
