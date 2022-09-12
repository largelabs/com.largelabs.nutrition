using System.Collections;
using System.Collections.Generic;
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
        raycastController.StopAutoMove();

        raycastController.StartAutoMove(DoraGameplayData.CursorAutoMoveSpeed);

        yield return this.Wait(DoraGameplayData.FrenzyTime);

        raycastController.StopAutoMove();

    }

    public void StopFrenzyMode()
    {
        raycastController.StopAutoMove();
    }

    #endregion
}
