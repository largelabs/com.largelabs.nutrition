
using System.Collections;
using UnityEngine;

public class AutoRotator : MonoBehaviourBase
{
    [SerializeField] Vector3 rotationSpeed = new Vector3(50f, 0f, 0f);
    [SerializeField] Vector3 maxRotationSpeed = new Vector3(50f, 0f, 0f);

    Vector3 currentRotationSpeed = MathConstants.VECTOR_3_ZERO;
    float rotationRatio = 0f;

    Coroutine autoRotationRoutine = null;

    #region UNITY AND CORE

    protected override void Awake()
    {
        base.Awake();
        updateRotationSpeed();
    }

    #endregion

    #region PUBLIC API

    public void SetRotationSpeedRatio(float i_ratio)
    {
        rotationRatio = Mathf.Clamp01(i_ratio);
        updateRotationSpeed();
    }

    void updateRotationSpeed()
    {
        currentRotationSpeed = Vector3.Lerp(rotationSpeed, maxRotationSpeed, rotationRatio);
    }

    public bool IsAutoRotating => null != autoRotationRoutine;

    [ExposePublicMethod]
    public void SetRotationSpeed(Vector3 i_rotationSpeed)
    {
        currentRotationSpeed = i_rotationSpeed;
        updateRotationSpeed();
    }

    [ExposePublicMethod]
    public void SetRotationSpeedX(float i_rotationSpeedX)
    {
        rotationSpeed.x = i_rotationSpeedX;
        updateRotationSpeed();
    }

    [ExposePublicMethod]
    public void SetRotationSpeedY(float i_rotationSpeedY)
    {
        rotationSpeed.y = i_rotationSpeedY;
        updateRotationSpeed();
    }

    [ExposePublicMethod]
    public void SetRotationSpeedZ(float i_rotationSpeedZ)
    {
        rotationSpeed.z = i_rotationSpeedZ;
        updateRotationSpeed();
    }

    [ExposePublicMethod]
    public void SetMaxRotationSpeedX(float i_rotationSpeedX)
    {
        maxRotationSpeed.x = i_rotationSpeedX;
        updateRotationSpeed();
    }

    [ExposePublicMethod]
    public void SetMaxRotationSpeedY(float i_rotationSpeedY)
    {
        maxRotationSpeed.y = i_rotationSpeedY;
        updateRotationSpeed();
    }

    [ExposePublicMethod]
    public void SetMaxRotationSpeedZ(float i_rotationSpeedZ)
    {
        maxRotationSpeed.z = i_rotationSpeedZ;
        updateRotationSpeed();
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
            transform.Rotate(Time.deltaTime * currentRotationSpeed);
            yield return null;
        }
    } 

    #endregion
}

