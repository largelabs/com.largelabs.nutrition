using System;
using System.Collections;

public abstract class MiniGameFlow : MonoBehaviourBase
{

	public abstract void introRoutine();

	public abstract void onGameplayStarted();

	public abstract void onGameplayEnded();

	public abstract void onSuccess();

	public abstract void onFailure();

	public abstract void onExitMiniGame();


}
