using System.Collections;
using UnityEngine;

public class HarrankashCelebrationState : State
{
    [SerializeField] SpriteFrameSwapper celebrationFrames = null;
    [SerializeField] SpriteFrameSwapper jumpAnticipationFrames = null;

    [SerializeField] Transform visualObjectRoot = null;
    [SerializeField] float visualObjectYOffset = -0.0174f;

    [SerializeField] TrailRenderer trail = null;

    Coroutine jumpRoutine = null;

    public bool IsJumping => jumpRoutine != null;

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
        Vector3 temp = visualObjectRoot.localPosition;
        visualObjectRoot.localPosition = new Vector3(temp.x, visualObjectYOffset, temp.z);
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
        trail.enabled = false;
    }

    protected override void onStateExit()
    {
        trail.enabled = true;

    }

    protected override void onStateInit()
    {

    }

    protected override void onStateUpdate()
    {

    }
    #endregion
}
