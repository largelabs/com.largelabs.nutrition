using UnityEngine;

public abstract class StateFeatureAbstract : MonoBehaviourBase
{
    #region Public

    public void FeatureInit()
    {
        onInit();
    }
    public void FeatureStart()
    {
        onStart();
    }

    public void FeatureUpdate()
    {
        onUpdate();
    }
    public void FeatureFixedUpdate()
    {
        onFixedUpdate();
    }
    public void FeatureExit()
    {
        onExit();
    }
    #endregion

    #region Protected
    protected virtual void onInit() { }
    protected virtual void onStart() { }
    protected virtual void onUpdate() { }
    protected virtual void onFixedUpdate() { }
    protected virtual void onExit() { }
    #endregion
}
