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

	[ExposePublicMethod]
	public virtual void GoToMainMenu()
    {
		SceneManager.LoadScene("Main Menu");
	}

    public void EnterMiniGame() => StartCoroutine(introSequence());

    public void EndMiniGame(bool i_endWithSuccess)
    {
		this.DisposeCoroutine(ref updateGameplayCoroutine);

		onGameplayEnded();

		if (i_endWithSuccess)
		{
			successCouroutine = StartCoroutine(onSuccess());
		}
		else
		{
			failureCoroutine = StartCoroutine(onFailure());
		}

    }

    public void ExitMiniGame() => SceneManager.LoadScene(0); //or should we use scene name instead?

    #endregion

    #region PROTECTED API

    protected abstract IEnumerator introRoutine();
	protected abstract IEnumerator onSuccess();
	protected abstract IEnumerator onFailure();
	protected abstract void onGameplayStarted();
	protected abstract void onGameplayEnded();
	protected virtual void onGameplayUpdate() { }

	protected void disposeAllCoroutines()
    {
		this.DisposeCoroutine(ref introCouroutine);
		this.DisposeCoroutine(ref successCouroutine);
		this.DisposeCoroutine(ref failureCoroutine);
		this.DisposeCoroutine(ref updateGameplayCoroutine);
	}

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
		this.DisposeCoroutine(ref introCouroutine);
		onGameplayStarted();
		updateGameplayCoroutine = StartCoroutine(updateGameplay());
	}

    #endregion

}
