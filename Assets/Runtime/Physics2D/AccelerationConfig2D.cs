
using UnityEngine;

[CreateAssetMenu(fileName = "AccelerationConfig2D", menuName = "com.largelabs.nutrition/AccelerationConfig2D", order = 1)]
public class AccelerationConfig2D : ScriptableObject, IAccelerationConfig
{
    [Header("Acceleration")]
    [SerializeField] float accX = 0f;
    [SerializeField] float accY = 0f;
    [SerializeField] float desX = 0f;
    [SerializeField] float desY = 0f;

    [Header("Velocity")]
    [SerializeField] float maxVelX = 0f;
    [SerializeField] float maxVelY = 0f;
    [SerializeField] float minVelX = 0f;
    [SerializeField] float minVelY = 0f;

    #region IAccelerationConfig2D

    public float AccelerationX { get { return accX; } }

    public float AccelerationY { get { return accY; } }

    public float AccelerationZ { get { return 0f; } }

    public float DescelerationX { get { return desX; } }

    public float DescelerationY { get { return desY; } }

    public float DescelerationZ { get { return 0f; } }

    public float MaxVelocityX { get { return maxVelX; } }

    public float MaxVelocityY { get { return maxVelY; } }

    public float MaxVelocityZ { get { return 0f; } }

    public float MinVelocityX { get { return minVelX; } }

    public float MinVelocityY { get { return minVelY; } }

    public float MinVelocityZ { get { return 0f; } }

    #endregion
}

