using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class MiniGameFlow : MonoBehaviourBase
{
	private Coroutine introCouroutine = null;
    private Coroutine successCouroutine = null;
    private Coroutine failureCoroutine = null;
    private Coroutine updateGameplayCoroutine = null;

    #region PUBLIC API

    public void EnterMiniGame() => StartCoroutine(introSequence());

    public void EndMiniGame(bool i_endWithSuccess)
    {
		this.DisposeCoroutine(ref updateGameplayCoroutine);

		if(i_endWithSuccess)
		{
			successCouroutine = StartCoroutine(onSuccess());
		}
		else
		{
			failureCoroutine = StartCoroutine(onFailure());
		}

        onGameplayEnded();
    }

    public void ExitMiniGame() => SceneManager.LoadScene(0); //or should we use scene name instead?

    #endregion

    #region PROTECTED API

    protected abstract IEnumerator introRoutine();
	protected abstract IEnumerator onSuccess();
	protected abstract IEnumerator onFailure();
	protected abstract void onGameplayStarted();
	protected abstract void onGameplayEnded();
	protected abstract void onGameplayUpdate();

    #endregion

    #region PRIVATE

    private IEnumerator introSequence()
	{
		yield return introCouroutine = StartCoroutine(introRoutine());
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
		updateGameplayCoroutine = StartCoroutine(updateGameplay());
	}

    #endregion

}
