
using System.Collections;
using UnityEngine;

public class AutoRotator : MonoBehaviourBase
{
    [SerializeField] Vector3 rotationSpeed = new Vector3(50f, 0f, 0f);

    Coroutine autoRotationRoutine = null;

    #region PUBLIC API

    public bool IsAutoRotating => null != autoRotationRoutine;

    [ExposePublicMethod]
    public void SetRotationSpeed(Vector3 i_rotationSpeed)
    {
        rotationSpeed = i_rotationSpeed;
    }

    [ExposePublicMethod]
    public void SetRotationSpeedX(float i_rotationSpeedX)
    {
        rotationSpeed.x = i_rotationSpeedX;
    }

    [ExposePublicMethod]
    public void SetRotationSpeedY(float i_rotationSpeedY)
    {
        rotationSpeed.y = i_rotationSpeedY;
    }

    [ExposePublicMethod]
    public void SetRotationSpeedZ(float i_rotationSpeedZ)
    {
        rotationSpeed.z = i_rotationSpeedZ;
    }

    [ExposePublicMethod]
    public void StartAutoRotation()
    {
        if (null != autoRotationRoutine) return;
        autoRotationRoutine = StartCoroutine(autoRotationLoop());
    }

    [ExposePublicMethod]
    public void StopAutoRotation()
    {
        if (null == autoRotationRoutine) return;
        this.DisposeCoroutine(ref autoRotationRoutine);
    }

    #endregion

    #region PRIVATE

    IEnumerator autoRotationLoop()
    {
        while (true)
        {
            transform.Rotate(Time.deltaTime * rotationSpeed);
            yield return null;
        }
    } 

    #endregion
}

