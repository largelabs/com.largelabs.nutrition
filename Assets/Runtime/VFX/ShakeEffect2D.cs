using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeEffect2D : MonoBehaviourBase
{
    public enum ShakeType
    {
        Sin = 0,
        Random = 1
    }

    public enum ShakeDirection
    {
        Horizontal = 0,
        Vertical = 1,
        Both = 2
    }

    public enum ActivationType
    {
        On_Enable = 0,
        Manual = 1
    }


    [Header("Objects")]
    [Tooltip("If no transform is set, the transform of the object holding this script will be used by default.")]
    [SerializeField] private Transform transformToShake = null;

    [Header("Settings")]
    [SerializeField] private ActivationType activationType = ActivationType.Manual;
    [SerializeField] [Range(0.0f, 250.0f)] private float speed = 20f;
    [SerializeField] [Range(0.0f, 20.0f)] private float intensity = 0.5f;
    [SerializeField] private ShakeType shakeType = ShakeType.Sin;
    [SerializeField] private ShakeDirection shakeDirection = ShakeDirection.Both;
    [SerializeField] private bool continuousShaking = false;
    [SerializeField] private float shakeDuration = 0.0f;
    [SerializeField] private bool loop = false;
    [SerializeField] private float waitTimeBetweenLoops = 0.0f;
    [SerializeField] private bool globalPosition = false;
    [SerializeField] private bool lockX = false;
    [SerializeField] private bool lockY = false;

    Vector3 origin;
    private float newX = 0f;
    private float newY = 0f;
    private float newZ = 0f;
    private float firstX = 0f;
    private float firstY = 0f;
    private float firstZ = 0f;
    private float currentOriginX = 0f;
    private float currentOriginY = 0f;
    private float currentOriginZ = 0f;
    private float remainingTime = 0.0f;
    private float loopTimer = 0.0f;
    private bool canShake = false;
    

    #region UNITY AND CORE

    protected override void Awake()
    {
        base.Awake();
        if (transformToShake == null)
            transformToShake = this.transform;
    }

    private void OnEnable()
    {
        if(activationType == ActivationType.On_Enable)
            canShake = true;

        if (globalPosition)
        {
            origin.x = transformToShake.position.x;
            origin.y = transformToShake.position.y;
            origin.z = transformToShake.position.z;
        }
        else
        {
            origin.x = transformToShake.localPosition.x;
            origin.y = transformToShake.localPosition.y;
            origin.z = transformToShake.localPosition.z;
        }

        firstX = origin.x;
        firstY = origin.y;
        firstZ = origin.z;

        remainingTime = shakeDuration;
    }

    private void OnDisable()
    {
        if (activationType == ActivationType.On_Enable)
            canShake = false;
    }

    private void Update()
    {
        if (false == canShake)
            return;

        if (continuousShaking)
        {
            generateNewCoords();
            shake();
        }
        else
        {
            if (remainingTime > 0.0f)
            {
                generateNewCoords();
                shake();
                remainingTime -= Time.deltaTime;

                if (remainingTime <= 0 && loop)
                    loopTimer = waitTimeBetweenLoops;

            }
            else if (loop)
            {
                if (loopTimer > 0)
                    loopTimer -= Time.deltaTime;
                else
                    resetShake();

            }
            else
            {
                canShake = false;
                resetPosition();
            }
        }

    }

    #endregion

    #region PUBLIC API
    public bool IsShaking { get { return canShake; } }

    public void SetContinuousShake(bool i_isContinuous)
    {
        continuousShaking = i_isContinuous;
    }

    #region START 

    [ExposePublicMethod]
    public void StartShake(bool i_newOrigin = false)
    {
        if (canShake)
            return;

        if(i_newOrigin)
        {
            if(globalPosition)
            {
                origin.x = transformToShake.position.x;
                origin.y = transformToShake.position.y;
            }
            else
            {
                origin.x = transformToShake.localPosition.x;
                origin.y = transformToShake.localPosition.y;
            }
            
        }

        remainingTime = shakeDuration;
        canShake = true;
    }

    public void SetIntensity(float i_intensity)
    {
        intensity = i_intensity;
    }

    public void StartShake(Vector3 i_newOrigin)
    {
        UpdateOrigin(i_newOrigin);

        remainingTime = shakeDuration;
        canShake = true;
    }

    public void StartShake(ShakeType i_type, ShakeDirection i_direction, bool i_newOrigin = false)
    {
        if(i_newOrigin)
        {
            if (globalPosition)
            {
                origin.x = transformToShake.position.x;
                origin.y = transformToShake.position.y;
            }
            else
            {
                origin.x = transformToShake.localPosition.x;
                origin.y = transformToShake.localPosition.y;
            }
        }

        remainingTime = shakeDuration;
        canShake = true;
        shakeType = i_type;
        shakeDirection = i_direction;
    }

    public void StartShake(float i_spd, float i_intense, bool i_newOrigin = false)
    {
        if (i_newOrigin)
        {
            if (globalPosition)
            {
                origin.x = transformToShake.position.x;
                origin.y = transformToShake.position.y;
            }
            else
            {
                origin.x = transformToShake.localPosition.x;
                origin.y = transformToShake.localPosition.y;
            }
        }

        remainingTime = shakeDuration;
        canShake = true;
        speed = i_spd;
        intensity = i_intense;
    }

    public void StartShake(float i_spd, float i_intense, ShakeType i_type, ShakeDirection i_direction, bool i_newOrigin = false)
    {
        if (i_newOrigin)
        {
            if (globalPosition)
            {
                origin.x = transformToShake.position.x;
                origin.y = transformToShake.position.y;
            }
            else
            {
                origin.x = transformToShake.localPosition.x;
                origin.y = transformToShake.localPosition.y;
            }
        }

        remainingTime = shakeDuration;
        canShake = true;
        speed = i_spd;
        intensity = i_intense;
        shakeType = i_type;
        shakeDirection = i_direction;
    }

    #endregion

    public void PauseShake()
    {
        canShake = false;
    }

    public void StopShake()
    {
        PauseShake();
        ResetShake();
    }

    public void StopShake(Vector3 i_newOrigin)
    {
        UpdateOrigin(i_newOrigin);
        PauseShake();
        ResetShake();
    }

    public void ChangeShakeTarget(Transform i_targetTransform)
    {
        transformToShake = i_targetTransform;
        origin.x = transformToShake.localPosition.x;
        origin.y = transformToShake.localPosition.y;
    }

    public void SetShakeDuration(float i_duration)
    {
        shakeDuration = i_duration;
        remainingTime = shakeDuration;
    }
    
    public float GetShakeDuration()
    {
        return shakeDuration;
    }

    public void ResetShake()
    {
        resetShake();
    }

    public void UpdateOrigin(Vector3 i_origin)
    {
        origin = i_origin;
    }
    #endregion

    #region PRIVATE API
    private void generateNewCoords()
    {
        currentOriginX = (globalPosition) ? transformToShake.position.x : transformToShake.localPosition.x;
        currentOriginY = (globalPosition) ? transformToShake.position.y : transformToShake.localPosition.y;
        currentOriginZ = (globalPosition) ? transformToShake.position.z : transformToShake.localPosition.z;

        newZ = currentOriginZ;

        if (shakeType == ShakeType.Sin)
        {
            if (shakeDirection == ShakeDirection.Horizontal)
            {
                newX = origin.x + Mathf.Sin(Time.time * speed) * intensity;
                newY = currentOriginY;
            }
            else if (shakeDirection == ShakeDirection.Vertical)
            {
                newX = currentOriginX;
                newY = origin.y + Mathf.Sin(Time.time * speed) * intensity;
            }
            else if (shakeDirection == ShakeDirection.Both)
            {
                newX = origin.x + Mathf.Sin(Time.time * speed) * intensity;
                newY = origin.y + Mathf.Sin(Time.time * speed) * intensity;
            }
        }
        else if(shakeType == ShakeType.Random)
        {
            Vector3 random = Random.insideUnitSphere;

            if (shakeDirection == ShakeDirection.Horizontal)
            {
                newX = origin.x + random.x * intensity;
                newY = currentOriginY;
            }
            else if (shakeDirection == ShakeDirection.Vertical)
            {
                newX = currentOriginX;
                newY = origin.y + random.y * intensity;
            }
            else if (shakeDirection == ShakeDirection.Both)
            {
                newX = origin.x + random.x * intensity;
                newY = origin.y + random.y * intensity;
            }
        }
    }

    private void shake()
    {
        transformToShake.localPosition = new Vector3(newX, newY, newZ);
    }

    private void resetShake()
    {
        resetPosition();

        remainingTime = shakeDuration;
        loopTimer = 0.0f;
    }

    private void resetPosition()
    {
        float newX = 0f;
        float newY = 0f;

        float newZ = origin.z;

        if (shakeDirection == ShakeDirection.Both)
        {
            newX = (lockX) ? firstX : origin.x;
            newY = (lockY) ? firstY : origin.y;
        }
        else if(shakeDirection == ShakeDirection.Horizontal)
        {
            newX = (lockX) ? firstX : origin.x;
            newY = currentOriginY;
        }
        else if(shakeDirection == ShakeDirection.Vertical)
        {
            newX = currentOriginX;
            newY = (lockY) ? firstY : origin.y; 
        }

        if (globalPosition)
            transformToShake.position = new Vector3(newX, newY, newZ);
        else
            transformToShake.localPosition = new Vector3(newX, newY, newZ);
    }

    #endregion
}
