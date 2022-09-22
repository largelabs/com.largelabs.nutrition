using System.Collections;
using UnityEngine;

public class HarrankashCelebrationState : State
{
    [SerializeField] SpriteFrameSwapper celebrationFrames = null;
    [SerializeField] SpriteFrameSwapper jumpAnticipationFrames = null;

    Coroutine jumpRoutine = null;

    public void PlayJumpSequence()
    {
        if (jumpRoutine == null)
        {
            jumpRoutine = StartCoroutine(jumpSequence());
        }
    }

    private IEnumerator jumpSequence()
    {
        celebrationFrames.Stop();
        jumpAnticipationFrames.Play();
        yield return this.Wait(0.3f);
        jumpAnticipationFrames.Stop();

        setState<HarankashJumpState>();

        this.DisposeCoroutine(ref jumpRoutine);
    }

    #region STATE API
    protected override void onStateEnter()
    {
        celebrationFrames.Play();
    }

    protected override void onStateExit()
    {

    }

    protected override void onStateInit()
    {

    }

    protected override void onStateUpdate()
    {

    }
    #endregion
}
