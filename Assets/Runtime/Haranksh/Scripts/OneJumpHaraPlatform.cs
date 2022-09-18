using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneJumpHaraPlatform : HaraPlatformAbstract
{
    [SerializeField] SpriteRenderer visualObject = null;
    [SerializeField] BoxCollider2D collider = null;
    public override void onCollision()
    {
        //if (gameObject.tag == "Bouncy")
        //{
        //    gameObject.tag = "Untagged";
        //    gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        //}

        visualObject.color = Color.clear;
        collider.enabled = false;
    }
}
