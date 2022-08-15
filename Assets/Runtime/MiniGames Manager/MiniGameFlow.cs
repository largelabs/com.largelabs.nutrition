using System.Collections;
using UnityEngine.SceneManagement;

public abstract class MiniGameFlow : MonoBehaviourBase
{
	protected abstract IEnumerator introRoutine();
	protected abstract IEnumerator onSuccess();
	protected abstract IEnumerator onFailure();
	protected abstract void onGameplayStarted();
	protected abstract void onGameplayEnded();
	protected abstract void onGameplayUpdate();

	private IEnumerator introSequence()
	{
		yield return StartCoroutine(introRoutine());
		startMiniGame();
	}

	private IEnumerator updateGameplay()
	{
		while (true)
		{
			onGameplayUpdate();
			yield return null;
		}
	}

	private void startMiniGame()
	{
		onGameplayStarted();
		StartCoroutine(updateGameplay());
	}

	public void EnterMiniGame() => StartCoroutine(introSequence());

	public void EndMiniGame()
	{
		StopCoroutine(updateGameplay());
		onGameplayEnded();
	}

	public void ExitMiniGame() => SceneManager.LoadScene(0); //or should we use scene name instead?

}
