using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColor : Action
{
    public Renderer renderer;
    public Color color;
    public override void performAction()
    {
        if (renderer is null)
            return;

        renderer.material.color = color;
    }
}
