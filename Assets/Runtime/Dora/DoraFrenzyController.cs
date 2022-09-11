using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraFrenzyController : MonoBehaviourBase
{
    [SerializeField] private float rotationSpeed = 100f;

    #region PRIVATE API

    #endregion

    #region PUBLIC API

    public void SetFrenzyRotationSpeed(AutoRotator i_autoRotator)
    {
        i_autoRotator.SetRotationSpeedX(rotationSpeed);
    }

    #endregion
}
