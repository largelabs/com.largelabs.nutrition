using UnityEngine;

public class UIHarrankashSpawner : TransformSpawner<SpriteFrameSwapper, UIHarrankashTypes>
{
    private readonly static string ORANGE_HARRA_ANIM = "OrangeHarraAnim";

    protected override string getPrefab(UIHarrankashTypes i_prefabId)
    {
        if (i_prefabId == UIHarrankashTypes.Orange)
            return ORANGE_HARRA_ANIM;
        else
        {
            Debug.LogError("Invalid prefab Id! Returning null...");
            return null;
        }
    }

    protected override void resetComponent(SpriteFrameSwapper i_component)
    {
        i_component.Stop();
    }
}
