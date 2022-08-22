using System.Collections;
using UnityEngine;

public class HaraMiniGame : MiniGameFlow //Should it be a Singleton?
{
	protected override IEnumerator introRoutine()
	{
		Debug.Log("started");
		yield return this.Wait(2);
		Debug.Log("ended");
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
