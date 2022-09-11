using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraFrenzyController : MonoBehaviourBase
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float cursorAutoMoveTime = 1f;
    [SerializeField] private float frenzyTime = 8f;
    [SerializeField] private DoraRaycastController raycastController = null;

    #region PRIVATE API

    #endregion

    #region PUBLIC API

    public IEnumerator PlayFrenzyMode(AutoRotator i_autoRotator)
    {
        i_autoRotator.SetRotationSpeedX(rotationSpeed);

        raycastController.StartAutoMove(cursorAutoMoveTime);

        yield return this.Wait(frenzyTime);

        raycastController.StopAutoMove();
    }

    #endregion
}
