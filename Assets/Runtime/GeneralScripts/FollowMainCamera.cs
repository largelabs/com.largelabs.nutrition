using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMainCamera : FollowTransform
{
    private void Awake()
    {
        transformToFollow = Camera.main.transform;
    }
}
