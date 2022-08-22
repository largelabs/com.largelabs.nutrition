using System.Collections;
using UnityEngine;
using Cinemachine;

public class HaraMiniGame : MiniGameFlow //Should it be a Singleton?
{
	[SerializeField] private CinemachineVirtualCamera camera;
	[SerializeField] private InterpolatorsManager cameraInterpolatorsManager;

	protected override IEnumerator introRoutine()
	{
		interpolatorsManager.Animate(camera.m_Lens.OrthographicSize, 5f, 2f, new AnimationMode(AnimationType.Ease_In_Out)); //not working
		yield return null;
	}

	protected override IEnumerator onFailure()
	{
		Debug.Log("failed");
		yield return null;
	}

	protected override void onGameplayEnded()
	{
		Debug.Log("Gameplay ended");
	}

	protected override void onGameplayStarted()
	{
		Debug.Log("Gameplay started");
	}

	protected override void onGameplayUpdate()
	{
		Debug.Log("Gameplay update");
	}

	protected override IEnumerator onSuccess()
	{
		Debug.Log("success");
		yield return null;
	}
}
