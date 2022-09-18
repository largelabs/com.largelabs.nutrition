using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class HaraMiniGame : MiniGameFlow
{
	[SerializeField] VCamSwitcher vCamSwitcher = null;
	[SerializeField] CinemachineVirtualCamera introCam_0 = null;
	[SerializeField] CinemachineVirtualCamera introCam_1 = null;
	[SerializeField] CinemachineVirtualCamera playerCam = null;

	[SerializeField] StateMachine playerStateMachine = null;
	[SerializeField] Controls playerControls = null;
	[SerializeField] HarrankashTouchEventDispatcher touchEventDispatcher = null;
	[SerializeField] PhysicsBody2D harrankashPhysicsBody = null;
	[SerializeField] PositionAnimator harrankashRopeSlide = null;

	[SerializeField] HarraPlatformSpawnManager platformSpawnManager = null;
	[SerializeField] TriggerAction2D endTrigger = null;

	private int currentPile = 0;
	private int orangeCount = 0;
	private Vector3 originPosition = Vector3.zero;

	private Coroutine nextPileRoutine = null;

    #region UNITY
    private void Start()
    {
		originPosition = playerStateMachine.transform.position;
		platformSpawnManager.GenerateNewMap(0);
		EnterMiniGame();
	}
    #endregion

    #region PROTECTED
    protected override IEnumerator introRoutine()
	{
		vCamSwitcher.SwitchToVCam(introCam_1);
		yield return this.Wait(1f);

		playerStateMachine.SetState<HarankashIdleState>();
		playerControls.DisableControls();
		Debug.LogError("DisabledControls");
		yield return this.Wait(1f);

		vCamSwitcher.SwitchToVCam(playerCam);
		yield return this.Wait(2f);
	}

	protected override void onGameplayStarted()
	{
		endTrigger.OnTriggerAction += nextPile;
		touchEventDispatcher.OnTouchOrange += collectOrange;
		touchEventDispatcher.OnTouchCart += failGame;

		playerControls.EnableControls();
		Debug.LogError("Gameplay Start! Controls Activated.");
	}

    protected override void onGameplayUpdate()
	{
	}

	protected override void onGameplayEnded()
	{
		endTrigger.OnTriggerAction -= nextPile;
		touchEventDispatcher.OnTouchOrange -= collectOrange;
		touchEventDispatcher.OnTouchCart -= failGame;
	}

	protected override IEnumerator onSuccess()
	{
		Debug.LogError("SUCCESS");
		yield break;
	}

	protected override IEnumerator onFailure()
	{
		Debug.LogError("FAIL");
		playerStateMachine.SetGenericState("d");
		playerControls.DisableControls();
		yield break;
	}
	#endregion

	#region PRIVATE


	private void failGame()
	{
		EndMiniGame(false);
	}

	private void collectOrange()
	{
		orangeCount++;
	}

	private void nextPile()
	{
		if (currentPile == 2)
			EndMiniGame(true);

		touchEventDispatcher.OnTouchCart -= failGame;
		playerControls.DisableControls();
		harrankashPhysicsBody.SetVelocity(Vector2.zero);
		harrankashPhysicsBody.SetGravityModifier(0);
		platformSpawnManager.DespawnMap();
		orangeCount = 0;

		if (nextPileRoutine == null)
			nextPileRoutine = StartCoroutine(nextPileSequence());
	}

    private IEnumerator nextPileSequence()
    {
		vCamSwitcher.SwitchToVCam(introCam_0);
		yield return this.Wait(2f);
		//playerStateMachine.transform.position = originPosition;

		playerStateMachine.SetState<HarankashIdleState>();
		harrankashRopeSlide.MoveToPosition(playerStateMachine.transform.position, null, null, null, null, null, null);
		yield return this.Wait(4f);
		playerStateMachine.transform.position = originPosition;

		vCamSwitcher.SwitchToVCam(introCam_1);
		yield return this.Wait(2f);

		platformSpawnManager.GenerateNewMap(++currentPile);

		vCamSwitcher.SwitchToVCam(playerCam);
		yield return this.Wait(2f);

		playerControls.EnableControls();
		touchEventDispatcher.OnTouchCart += failGame;

		this.DisposeCoroutine(ref nextPileRoutine);
	}
	#endregion
}
