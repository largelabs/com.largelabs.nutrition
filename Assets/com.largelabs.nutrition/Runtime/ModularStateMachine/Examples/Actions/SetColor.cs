using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColor : Action
{
    public new Renderer renderer;
    public Color color;
    public override void PerformAction()
    {
        if (renderer is null)
            return;

        renderer.material.color = color;
    }
}
