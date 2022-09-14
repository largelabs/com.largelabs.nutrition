using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneJumpHaraPlatform : HaraPlatformAbstract
{
    public override void onCollision()
    {
        if (gameObject.tag == "Bouncy")
        {
            gameObject.tag = "Untagged";
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }
}
