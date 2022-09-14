using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FrameSwapperFeature : StateFeatureAbstract
{
    [SerializeField] SpriteFrameSwapper frameSwapper = null;

    // Code review comments :
    // Lacks all the options that we might need for such a script.
    // Adding some examples for said options below for future implementation
    [SerializeField] bool playOnStateEnter = true; //Done
    [SerializeField] bool stopOnStateExit = true;  //Done
    [SerializeField] float playDelay = 0f;  //Done
    [SerializeField] UnityEvent onStartNewCycle = null; // register to cycle events on the frame swapper itself
                                                        // and trigger these Unity events internally
                                                        //Done
                                                        
    private int lastFrameLoopCount;

    protected override void onStart()
    {
        if(frameSwapper!= null)
        {
            if (playOnStateEnter)
            {
                StartCoroutine(start_animation());
            }
        }
        else
        {
            Debug.LogError("No Frame Swapper Assigned to FrameSwaperFeature");
        }
    }
    protected override void onExit()
    {
        if(frameSwapper != null)
        {
            if (stopOnStateExit)
            {
                frameSwapper.Stop();
            }
        }
    }

    IEnumerator start_animation()
    {
        yield return new WaitForSeconds(playDelay);
        frameSwapper.Play();
        lastFrameLoopCount = frameSwapper.LoopCount;
    }


    //This needs checking if it's effecient enough
    protected override void onUpdate()
    {
        if(lastFrameLoopCount != frameSwapper.LoopCount)
        {
            if(onStartNewCycle != null)
            {
                onStartNewCycle.Invoke();
            }
            lastFrameLoopCount = frameSwapper.LoopCount;
        }
    }
}
