using UnityEngine;

public class PlatformID : MonoBehaviour
{
    public enum PlatformType
    {
        Orange,
        Yellow,
        Green
    }

    [SerializeField] private PlatformType platformType = PlatformType.Green;

    public PlatformType PType => platformType;
}
