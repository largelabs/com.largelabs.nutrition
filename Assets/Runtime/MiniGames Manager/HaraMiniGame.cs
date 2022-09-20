using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaraMiniGame : MiniGameFlow
{
	[Header("Camera Work")]
	[SerializeField] VCamSwitcher vCamSwitcher = null;
	[SerializeField] CinemachineVirtualCamera introCam_0 = null;
	[SerializeField] CinemachineVirtualCamera introCam_1 = null;
	[SerializeField] CinemachineVirtualCamera playerCam = null;

	[Header("Player Components")]
	[SerializeField] StateMachine playerStateMachine = null;
	[SerializeField] Controls playerControls = null;
	[SerializeField] HarrankashTouchEventDispatcher touchEventDispatcher = null;
	[SerializeField] PhysicsBody2D harrankashPhysicsBody = null;
	[SerializeField] PositionAnimator harrankashRopeSlide = null;

	[Header("Sequence Components")]
	[SerializeField] HarraPlatformSpawnManager platformSpawnManager = null;
	[SerializeField] TriggerAction2D endTrigger = null;
	[SerializeField] UIHarrankashStack harraStack = null;
	[SerializeField] SpriteHarrankashSpawner spriteHarraSpawner = null;
	[SerializeField] InterpolatorsManager interpolatorsManager = null;
	[SerializeField] AnimationCurve slideCurve = null;
	[SerializeField] Transform ropeSlideStart = null;
	[SerializeField] Transform ropeSlideEnd = null;

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
		AddUIHarra();
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

		yield return StartCoroutine(UIHarraSlideSequence());

		harrankashRopeSlide.MoveToPosition(playerStateMachine.transform.position, null, null, null, null, null, null);
		yield return this.Wait(4f);
		playerStateMachine.transform.position = originPosition;
		harrankashPhysicsBody.ResetGravityModifier();

		vCamSwitcher.SwitchToVCam(introCam_1);
		yield return this.Wait(2f);

		platformSpawnManager.GenerateNewMap(++currentPile);

		vCamSwitcher.SwitchToVCam(playerCam);
		yield return this.Wait(2f);

		playerControls.EnableControls();
		touchEventDispatcher.OnTouchCart += failGame;

		this.DisposeCoroutine(ref nextPileRoutine);
	}

	private IEnumerator UIHarraSlideSequence()
    {
		harraStack.DestackHarrankash();

		harraStack.OnDiscardHarrankash += ropeSlideHarra;

		while (harraStack.IsDestacking)
        {
			Debug.LogError("destacking");
			yield return null;
		}

		harraStack.OnDiscardHarrankash -= ropeSlideHarra;
	}

	private void ropeSlideHarra()
    {
		SpriteFrameSwapper spawnedHarra = 
			spriteHarraSpawner.SpawnTransformAtAnchor(ropeSlideStart, Vector3.zero, SpriteHarrankashTypes.Orange,
			true, false, false);

		PositionAnimator posAnim = spawnedHarra.GetComponent<PositionAnimator>();
		if (posAnim != null)
			posAnim.MoveToPosition(spawnedHarra.transform.position, ropeSlideEnd.position, true, 4f, interpolatorsManager, slideCurve, null);
    }
	#endregion

	#region DEBUG
	[ExposePublicMethod]
    public void AddUIHarra()
    {
		Queue<float> test = new Queue<float>();
		test.Enqueue(1f);
		harraStack.CollectUIElements(test);
    }

	[ExposePublicMethod]
	public void Destack()
    {
		StartCoroutine(UIHarraSlideSequence());
	}
	#endregion
}
