using UnityEngine;

public abstract class HaraPlatformAbstract : MonoBehaviourBase
{

    public AccelerationConfig2D AccelerationConfig;
    [SerializeField][Range(1f, 30f)] public float MaxJumpHeight = 8f;

    public void onCollision()
    {
        //Play animations, sound, etc..
    }
}
