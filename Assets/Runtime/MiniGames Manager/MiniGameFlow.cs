using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class MiniGameFlow : MonoBehaviourBase
{
	// Code review comments : 
	// keep references to the running coroutines with Coroutine objects.
	// Use this.DisposeCoroutine(ref myCoroutineObject) to stop them and release the Coroutine ref

	Coroutine introCouroutine = null;
    Coroutine successCouroutine = null;
    Coroutine failureCoroutine = null;
    Coroutine updateGameplayCoroutine = null;

	// Success and failure coroutines are never triggered.
	// Trigger them in ExitMiniGame with a boolean in the parameters bool i_endWithSuccess


    #region PUBLIC API

    public void EnterMiniGame() => StartCoroutine(introSequence());

    public void EndMiniGame()
    {
        StopCoroutine(updateGameplay()); 
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

    #endregion

}
