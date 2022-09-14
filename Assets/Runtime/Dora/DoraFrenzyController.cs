using System.Collections;
using UnityEngine;

public class DoraFrenzyController : MonoBehaviourBase
{
    [SerializeField] private DoraRaycastController raycastController = null;
    [SerializeField] DoraGameplayData DoraGameplayData = null;

    #region PRIVATE API

    #endregion

    #region PUBLIC API

    public IEnumerator PlayFrenzyMode(AutoRotator i_autoRotator)
    {
        i_autoRotator.SetRotationSpeedX(DoraGameplayData.FrenzyRotationSpeed);
        raycastController.StartAutoRotation(false);

        raycastController.StartAutoMove(DoraGameplayData.CursorAutoMoveSpeed);

        yield return this.Wait(DoraGameplayData.FrenzyTime);
    }

    public void StopFrenzyMode()
    {
        raycastController.StopAutoMove();
    }

    #endregion
}
