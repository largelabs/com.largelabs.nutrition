

using System.Collections.Generic;
using UnityEngine;

public class SOStateMachine : ScriptableObject
{
    List<SOState> states = null;
    SOState curentState = null;


    void Update()
    {
        if (null != curentState) curentState.Update();
    }


    public void SetState(SOState i_state)
    {
        curentState = i_state;
    }
}

public class SOState : ScriptableObject
{
    public virtual void OnStateEnter()
    {
    }

    public virtual void Update()
    {
    }
}

public class RedState : SOState
{
    Renderer rnd = null;
    Material mat = null;

    public override void OnStateEnter()
    {
        mat = rnd.material;
        mat.SetColor("_Color", Color.red);
    }
}

public class DoNothingState : SOState
{

    public override void OnStateEnter()
    {

    }
}