using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaraMiniGame : MiniGameFlow
{
	[Header("Gameflow")]
	[SerializeField] HarraGameData gameData = null;

	[Header("Score")]
	[SerializeField] HarraScoreManager scoreManager = null;

	[Header("Timer")]
	[SerializeField] MinigameTimer mgTimer = null;
	[SerializeField] UIMinigameTimer uiMGTimer = null;

	[Header("Camera Work")]
	[SerializeField] VCamSwitcher vCamSwitcher = null;
	[SerializeField] CinemachineVirtualCamera introCam_0 = null;
	[SerializeField] CinemachineVirtualCamera introCam_1 = null;
	[SerializeField] CinemachineVirtualCamera introCam_2 = null;
	[SerializeField] CinemachineVirtualCamera playerCam = null;

	[Header("Player Components")]
	[SerializeField] StateMachine playerStateMachine = null;
	[SerializeField] HarrankashCelebrationState celebState = null;
	[SerializeField] Controls playerControls = null;
	[SerializeField] HarrankashPlatformEventDispatcher touchEventDispatcher = null;
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
	[SerializeField] SpriteRenderer bannerText = null;
	[SerializeField] Sprite bannerStart = null;
	[SerializeField] Sprite bannerJump = null;

	[Header("Pile Sequence Components")]
	[SerializeField] HarraPlatformSpawnManager platformSpawnManager = null;
	[SerializeField] TriggerAction2D endTrigger = null;
	[SerializeField] GameObject topPlatform = null;
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
	[SerializeField] UIHarrankashEndGamePopup harrankashPopup = null;

	[Header("End Sequence Components")]
	[SerializeField] LocalScalePingPong gameOverScale = null;
	[SerializeField] SpriteAlphaLerp gameOverFade = null;

    [Header("Sounds")]
    [SerializeField] HarraSFXProvider sfxProvider = null;

    private int currentPile = 0;
    private int orangeCount = 0;
    private Vector3 originPosition = Vector3.zero;

    private Coroutine nextPileRoutine = null;
    private Coroutine bannerRoutine = null;

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
        EnterMiniGame();
    }
    #endregion

    #region PROTECTED
    protected override IEnumerator introRoutine()
    {
        vCamSwitcher.SwitchToVCam(introCam_1);

        platformSpawnManager.GenerateNewMap(0);

        yield return this.Wait(1f);

        platformSpawnManager.MapAppear();
        playerStateMachine.SetState<HarankashIdleState>();
        playerControls.DisableControls();
        playerControls.SetLock(true);
        Debug.LogError("DisabledControls");
        yield return this.Wait(1f);

        vCamSwitcher.SwitchToVCam(playerCam);
        yield return this.Wait(2f);

        while (platformSpawnManager.MapIsAnimating)
            yield return null;

        bannerText.sprite = bannerStart;
        yield return StartCoroutine(bannerSequence());
    }

	protected override void onGameplayStarted()
	{
		endTrigger.OnTriggerAction += harraSlide;
		touchEventDispatcher.OnTouchOrange += collectOrange;
		touchEventDispatcher.OnFirstTouchNormal += collectNormal;
		touchEventDispatcher.OnFailConditionMet += failGame;
		mgTimer.OnTimerEnded += timeOut;

		playerControls.SetLock(false);
		playerControls.EnableControls();
		Debug.LogError("Gameplay Start! Controls Activated.");
	}

    protected override void onGameplayUpdate()
	{
	}

    protected override void onGameplayEnded()
    {
        unregisterEvents();
        mgTimer.PauseTimer();
    }

    protected override IEnumerator onSuccess()
    {
        Debug.LogError("SUCCESS");
        vCamSwitcher.SwitchToVCam(playerCam);

        // sfx suggestion: success sound (note that victory sound could be played with celebration state)
        // celebration sound would be for every pile finish but this would only be if all piles are finished

        // show score banner popup
        yield return this.Wait(2.5f);
        showEndgamePopup();

        playerControls.SetLock(false);
        playerControls.EnableControls();
    }

	protected override IEnumerator onFailure()
	{
		Debug.LogError("FAIL");
		playerStateMachine.GetComponentInChildren<TrailRenderer>().enabled = false;
		//playerStateMachine.SetState<HarankashIdleState>();
		//yield return null;
		playerStateMachine.SetGenericState("d");

        // sfx suggestion: failure sound
        sfxProvider.PlayFailureSFX();

		yield return this.Wait(0.5f);
		// show score banner popup
		showEndgamePopup();
		// sfx suggestion: board appear sound

		playerControls.SetLock(false);
		playerControls.EnableControls();
	}
	#endregion

	#region PRIVATE
	private void timeOut()
    {
		if (playerStateMachine.CurrentState.GetType() == typeof(HarankashIdleState))
			failGame();
    }

	private void failGame()
	{
		EndMiniGame(false);
	}

    private void resetGame()
    {
        stopBannerSequence();

        diactivateBanner();

        unregisterEvents();

        playerControls.DisableControls();

        playerStateMachine.gameObject.SetActive(true);
        playerStateMachine.SetState<HarankashIdleState>();
        playerStateMachine.transform.position = originPosition;

        currentPile = 0;
        mgTimer.ResetTimer();
        scoreManager.gameObject.SetActive(false);
        uiMGTimer.gameObject.SetActive(false);

        spriteHarraSpawner.DespawnAllTransforms();

        platformSpawnManager.DespawnMap(false);
    }

    private void stopBannerSequence()
    {
        this.DisposeCoroutine(ref bannerRoutine);

        textScale.StopPingPong();
        leafRotation_0.StopPingPong();
        leafRotation_1.StopPingPong();
        leafRotation_2.StopPingPong();
        shineRotation.StopPingPong();

    }

    private void collectOrange(Vector3 i_platformPos)
    {
        scoreManager.AddScore(i_platformPos);
        orangeCount++;
        AddUIHarra();
    }

	private void harraSlide()
    {
		bool end = false;
		if (currentPile == gameData.PileAmount - 1)
			end = true;//EndMiniGame(true);

		scoreManager.gameObject.SetActive(false);
		uiMGTimer.gameObject.SetActive(false);
		mgTimer.PauseTimer();
		touchEventDispatcher.OnFailConditionMet -= failGame;
		playerControls.DisableControls();
		playerControls.SetLock(true);
		harrankashPhysicsBody.SetVelocityX(0f);

		if (nextPileRoutine == null)
			nextPileRoutine = StartCoroutine(harraSlideSequence(end));
	}

	private void pileSwitch()
    {
		playerControls.DisableControls();
		playerControls.SetLock(true);

		harrankashPhysicsBody.SetVelocity(Vector2.zero);
		playerStateMachine.gameObject.SetActive(false);
		SpriteFrameSwapper spawnedHarra = spriteHarraSpawner.SpawnTransformAtAnchor(playerRopeSlideStart, MathConstants.VECTOR_3_ZERO,
											HarraEnumReference.SpriteHarrankashTypes.OrangePlayer, true, false, false);

		if (nextPileRoutine == null)
			nextPileRoutine = StartCoroutine(pileSwitchSequence(spawnedHarra));
	}

	/*private void nextPile()
	{
		if (currentPile == maxPiles - 1)
			EndMiniGame(true);

		scoreManager.gameObject.SetActive(false);
		uiMGTimer.gameObject.SetActive(false);
		mgTimer.PauseTimer();
		touchEventDispatcher.OnTouchCart -= failGame;
		playerControls.DisableControls();
		harrankashPhysicsBody.SetVelocity(Vector2.zero);
		//harrankashPhysicsBody.SetGravityModifier(0);
		playerStateMachine.gameObject.SetActive(false);
		SpriteFrameSwapper spawnedHarra = spriteHarraSpawner.SpawnTransformAtAnchor(playerRopeSlideStart, MathConstants.VECTOR_3_ZERO,
											HarraEnumReference.SpriteHarrankashTypes.OrangePlayer, true, false, false);
		//harrankashPhysicsBody.ResetGravityModifier();
		orangeCount = 0;

		if (nextPileRoutine == null)
			nextPileRoutine = StartCoroutine(nextPileSequence(spawnedHarra));
	}
	*/

    /*private IEnumerator nextPileSequence(SpriteFrameSwapper i_spawnedHarra)
    {
		// add a way to get vcam switch time
		vCamSwitcher.SwitchToVCam(introCam_2);
		yield return this.Wait(2f);

        float playerTime = slidePlayer(i_spawnedHarra);
        playerStateMachine.transform.position = originPosition;
        playerStateMachine.gameObject.SetActive(true);
        playerStateMachine.SetState<HarankashIdleState>();
        playerControls.DisableControls();

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

		bannerText.sprite = bannerJump;
		yield return StartCoroutine(bannerSequence());

		playerControls.EnableControls();
		touchEventDispatcher.OnTouchCart += failGame;

		this.DisposeCoroutine(ref nextPileRoutine);
	}
	*/

	private IEnumerator harraSlideSequence(bool i_end)
    {
		// add a way to get vcam switch time
		vCamSwitcher.SwitchToVCam(introCam_2);
		vCamSwitcher.LockSwitching(true);
		yield return this.Wait(2f);

		yield return StartCoroutine(UIHarraSlideSequence());

		if (i_end == false)
		{
			celebState.PlayJumpSequence();
			endTrigger.OnTriggerAction -= harraSlide;
			endTrigger.OnTriggerAction += pileSwitch;
		}
		else
			EndMiniGame(true);

		//while (celebState.IsJumping)
		//	yield return null;

		this.DisposeCoroutine(ref nextPileRoutine);
	}

	private IEnumerator pileSwitchSequence(SpriteFrameSwapper i_spawnedHarra)
    {

		float playerTime = slidePlayer(i_spawnedHarra);
		playerStateMachine.transform.position = originPosition;
		playerStateMachine.gameObject.SetActive(true);
		playerStateMachine.SetState<HarankashIdleState>();
		playerControls.DisableControls();
		playerControls.SetLock(true);

		yield return this.Wait(playerTime / 2f);

		spriteHarraSpawner.DespawnTransform(i_spawnedHarra);
		vCamSwitcher.LockSwitching(false);
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

		bannerText.sprite = bannerJump;
		yield return bannerRoutine = StartCoroutine(bannerSequence());

		playerControls.SetLock(false);
		playerControls.EnableControls();
		touchEventDispatcher.OnFailConditionMet += failGame;

		endTrigger.OnTriggerAction -= pileSwitch;
		endTrigger.OnTriggerAction += harraSlide;

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
			spriteHarraSpawner.SpawnTransformAtAnchor(slideStart, Vector3.zero, HarraEnumReference.SpriteHarrankashTypes.OrangePlat,
			true, false, false);

		SpriteRenderer[] rnds = spawnedHarra.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer rnd in rnds)
			rnd.flipX = slideRight == false;

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

        scoreManager.gameObject.SetActive(true);
        uiMGTimer.gameObject.SetActive(true);
        mgTimer.SetTimer(gameData.PileTimes[Mathf.Clamp(currentPile, 0, gameData.PileTimes.Count - 1)], true);
        mgTimer.StartTimer();

        diactivateBanner();
    }

    private void diactivateBanner()
    {
        if (bannerPositionIn.transform.parent.gameObject.activeSelf == false) return;

		leafRotation_0.StopPingPong();
		leafRotation_1.StopPingPong();
		leafRotation_2.StopPingPong();
		shineRotation.StopPingPong();
		textScale.SetScale(MathConstants.VECTOR_3_ZERO);
		bannerPositionIn.transform.parent.gameObject.SetActive(false);
	}

	private void showEndgamePopup()
    {
        harrankashPopup.SetScore(scoreManager.TotalScore.ToString());
        harrankashPopup.Appear(true);
        sfxProvider.PlayAppearSFX();
    }

    private void unregisterEvents()
    {
        endTrigger.OnTriggerAction -= harraSlide;
        touchEventDispatcher.OnTouchOrange -= collectOrange;
		touchEventDispatcher.OnFirstTouchNormal -= collectNormal;
        touchEventDispatcher.OnFailConditionMet -= failGame;
        mgTimer.OnTimerEnded -= timeOut;
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

    [ExposePublicMethod]
    public void ShowScorePopup()
    {
        showEndgamePopup();
    }
    #endregion

    #region PUBLIC API

    [ExposePublicMethod]
    public void RestartGame()
    {
        disposeAllCoroutines();
        resetGame();
        EnterMiniGame();
    }

    #endregion
}