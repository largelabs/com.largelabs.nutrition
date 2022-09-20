using UnityEngine;

public abstract class HaraPlatformAbstract : MonoBehaviourBase
{
    [SerializeField] private AccelerationConfig2D accelerationConfig;
    [SerializeField][Range(1f, 30f)] private float maxJumpHeight = 8f;

    public AccelerationConfig2D AccelerationConfig => accelerationConfig;
    public float MaxJumpHeight => maxJumpHeight;

    public virtual void onCollision()
    {
        //Play animations, sound, etc..
    }
}