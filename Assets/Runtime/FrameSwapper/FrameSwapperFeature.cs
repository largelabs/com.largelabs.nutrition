using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameSwapperFeature : StateFeatureAbstract
{
    [SerializeField] SpriteFrameSwapper frameSwapper;
    protected override void onStart()
    {
        frameSwapper.enabled = true;
    }
    protected override void onExit()
    {
        frameSwapper.enabled = false;
    }
}
