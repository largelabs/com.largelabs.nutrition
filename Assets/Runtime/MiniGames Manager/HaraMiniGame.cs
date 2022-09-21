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
	[SerializeField] CinemachineVirtualCamera introCam_2 = null;
	[SerializeField] CinemachineVirtualCamera playerCam = null;

	[Header("Player Components")]
	[SerializeField] StateMachine playerStateMachine = null;
	[SerializeField] Controls playerControls = null;
	[SerializeField] HarrankashTouchEventDispatcher touchEventDispatcher = null;
	[SerializeField] PhysicsBody2D harrankashPhysicsBody = null;
	[SerializeField] PositionAnimator harrankashRopeSlide = null;

	[Header("Banner Sequence Components")]
	[SerializeField] PositionAnimator bannerPositionIn = null;
	[SerializeField] PositionAnimator bannerPositionOut = null;
	[SerializeField] LocalScalePingPong textScale = null;
	[SerializeField] LocalRotationPingPong leafRotation_0 = null;
	[SerializeField] LocalRotationPingPong leafRotation_1 = null;
	[SerializeField] LocalRotationPingPong leafRotation_2 = null;
	[SerializeField] LocalRotationPingPong shineRotation = null;

	[Header("Pile Sequence Components")]
	[SerializeField] HarraPlatformSpawnManager platformSpawnManager = null;
	[SerializeField] TriggerAction2D endTrigger = null;
	[SerializeField] UIHarrankashStack harraStack = null;
	[SerializeField] SpriteHarrankashSpawner spriteHarraSpawner = null;
	[SerializeField] InterpolatorsManager interpolatorsManager = null;
	[SerializeField] AnimationCurve slideCurve = null;
	[SerializeField] Transform rope0SlideStart = null;
	[SerializeField] Transform rope0SlideEnd = null;
	[SerializeField] Transform rope1SlideStart = null;
	[SerializeField] Transform rope1SlideEnd = null;
	[SerializeField] Transform playerRopeSlideStart = null;
	[SerializeField] Transform playerRopeSlideEnd = null;
	[SerializeField] float slideSpeed = 2f;

	private int currentPile = 0;
	private int orangeCount = 0;
	private Vector3 originPosition = Vector3.zero;

	private Coroutine nextPileRoutine = null;

	private Transform currentRopeSlideStart = null;
	private Vector3 currentRopeSlideEnd = MathConstants.VECTOR_3_ZERO;
	// add currentSlidHarra to keep track of when to switch ropes
	private bool slideRight = true;
	private int maxOnRope = 27;
	private int currentSlid = 0;

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

		platformSpawnManager.MapAppear();
		playerStateMachine.SetState<HarankashIdleState>();
		playerControls.DisableControls();
		Debug.LogError("DisabledControls");
		yield return this.Wait(1f);

		vCamSwitcher.SwitchToVCam(playerCam);
		yield return this.Wait(2f);

		while (platformSpawnManager.MapIsAnimating)
			yield return null;

		yield return StartCoroutine(bannerSequence());
	}

	protected override void onGameplayStarted()
	{
		endTrigger.OnTriggerAction += nextPile;
		touchEventDispatcher.OnTouchOrange += collectOrange;
		touchEventDispatcher.OnTouchCart += failGame;

		//currentRopeSlideStart = rope0SlideStart.position;
		//currentRopeSlideEnd = rope0SlideEnd.position;

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
		//harrankashPhysicsBody.SetGravityModifier(0);
		playerStateMachine.gameObject.SetActive(false);
		SpriteFrameSwapper spawnedHarra = spriteHarraSpawner.SpawnTransformAtAnchor(playerRopeSlideStart, MathConstants.VECTOR_3_ZERO,
											SpriteHarrankashTypes.OrangePlayer, true, false, false);
		//harrankashPhysicsBody.ResetGravityModifier();
		orangeCount = 0;

		if (nextPileRoutine == null)
			nextPileRoutine = StartCoroutine(nextPileSequence(spawnedHarra));
	}

    private IEnumerator nextPileSequence(SpriteFrameSwapper i_spawnedHarra)
    {
		// add a way to get vcam switch time
		vCamSwitcher.SwitchToVCam(introCam_2);
		yield return this.Wait(2f);

		float playerTime = slidePlayer(i_spawnedHarra);
		playerStateMachine.transform.position = originPosition;
		playerStateMachine.gameObject.SetActive(true);

		playerStateMachine.SetState<HarankashIdleState>();

		yield return StartCoroutine(UIHarraSlideSequence());
		yield return this.Wait(playerTime/2);

		spriteHarraSpawner.DespawnTransform(i_spawnedHarra);
		vCamSwitcher.SwitchToVCam(introCam_1);
		platformSpawnManager.DespawnMap(true);
		yield return this.Wait(2f);

		while (platformSpawnManager.MapIsAnimating)
			yield return null;

		platformSpawnManager.GenerateNewMap(++currentPile);
		platformSpawnManager.MapAppear();
		vCamSwitcher.SwitchToVCam(playerCam);
		yield return this.Wait(2f);

		while (platformSpawnManager.MapIsAnimating)
			yield return null;

		yield return StartCoroutine(bannerSequence());

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

	private float slidePlayer(SpriteFrameSwapper i_spawnedHarra)
    {
		Transform slideStart = i_spawnedHarra.transform;

		PositionAnimator posAnim = i_spawnedHarra.GetComponent<PositionAnimator>();
		float time = Vector3.Distance(slideStart.position, playerRopeSlideEnd.position) / slideSpeed;
		if (posAnim != null)
			posAnim.MoveToPosition(i_spawnedHarra.transform.position, playerRopeSlideEnd.position, true, time, interpolatorsManager, slideCurve, null);

		return time;
	}

	private void ropeSlideHarra()
    {
		if (currentSlid > 26)
        {
			slideRight = !slideRight;
			currentSlid = 0;
        }

		Transform slideStart = slideRight ? rope0SlideStart : rope1SlideStart;
		Transform slideEnd = slideRight ? rope0SlideEnd : rope1SlideEnd;

		SpriteFrameSwapper spawnedHarra = 
			spriteHarraSpawner.SpawnTransformAtAnchor(slideStart, Vector3.zero, SpriteHarrankashTypes.OrangePlat,
			true, false, false);

		PositionAnimator posAnim = spawnedHarra.GetComponent<PositionAnimator>();
		Vector3 endPos = Vector3.Lerp(slideStart.position, slideEnd.position, 1 - ((float)currentSlid / maxOnRope));
		float time = Vector3.Distance(slideStart.position, endPos) / slideSpeed;
		if (posAnim != null)
			posAnim.MoveToPosition(spawnedHarra.transform.position, endPos, true, time, interpolatorsManager, slideCurve, null);

		currentSlid++;
	}

	private IEnumerator bannerSequence()
	{
		bannerPositionIn.transform.parent.gameObject.SetActive(true);
		bannerPositionIn.MoveToPosition();
		while (bannerPositionIn.IsMoving)
			yield return null;

		textScale.StartPingPong(0.3f, MathConstants.VECTOR_3_ZERO, MathConstants.VECTOR_3_ONE * 1.5f, 1, false);
		leafRotation_0.StartPingPong(0.5f, -1);
		leafRotation_1.StartPingPong(0.5f, -1);
		leafRotation_2.StartPingPong(0.5f, -1);
		shineRotation.StartPingPong(10f, 1);
		while (textScale.isScaling)
			yield return null;

		textScale.StartPingPong(0.2f, textScale.transform.localScale, MathConstants.VECTOR_3_ONE, 1, false);
		yield return this.Wait(1f);

		bannerPositionOut.MoveToPosition();
		while (bannerPositionOut.IsMoving)
			yield return null;

		leafRotation_0.StopPingPong();
		leafRotation_1.StopPingPong();
		leafRotation_2.StopPingPong();
		shineRotation.StopPingPong();
		textScale.SetScale(MathConstants.VECTOR_3_ZERO);
		bannerPositionIn.transform.parent.gameObject.SetActive(false);
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
