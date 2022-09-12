using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraFrenzyController : MonoBehaviourBase
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float cursorAutoMoveSpeed = 1f;
    [SerializeField] private float frenzyTime = 8f;
    [SerializeField] private DoraRaycastController raycastController = null;

    [SerializeField] private DoraMover doraMover = null;

    #region PRIVATE API

    #endregion

    #region PUBLIC API

    public IEnumerator PlayFrenzyMode(AutoRotator i_autoRotator)
    {

        i_autoRotator.SetRotationSpeedX(rotationSpeed);
        raycastController.StartAutoRotation(false);
        raycastController.StopAutoMove();

        raycastController.StartAutoMove(cursorAutoMoveSpeed);

        yield return this.Wait(frenzyTime);

        raycastController.StopAutoMove();

    }

    public void StopFrenzyMode()
    {
        raycastController.StopAutoMove();
    }

    #endregion
}
