using UnityEngine;
using UnityEngine.Events;

public class FrameSwapperFeature : StateFeatureAbstract
{
    [SerializeField] SpriteFrameSwapper frameSwapper = null;

    // Code review comments :
    // Lacks all the options that we might need for such a script.
    // Adding some examples for said options below for future implementation
    [SerializeField] bool playOnStateEnter = true;
    [SerializeField] bool stopOnStateExit = true;
    [SerializeField] float playDelay = 0f;
    [SerializeField] UnityEvent onStartNewCycle = null; // register to cycle events on the frame swapper itself
                                                        // and trigger these Unity events internally

    protected override void onStart()
    {
        // Code review comments :
        // Don't use the enabled flag, but rather the public API (Play and Stop)

        // Test references to null and log, to avoid NullRefExceptions
        frameSwapper.enabled = true;
    }
    protected override void onExit()
    {
        frameSwapper.enabled = false;
    }
}
