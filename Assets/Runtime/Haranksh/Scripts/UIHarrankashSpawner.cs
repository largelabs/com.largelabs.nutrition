using UnityEngine;

public class UIHarrankashSpawner : TransformSpawner<UIImageFrameSwapper, UIHarrankashTypes>
{
    private readonly static string ORANGE_HARRA_ANIM = "UIHarra";

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

    protected override void resetComponent(UIImageFrameSwapper i_component)
    {
        i_component.Stop();
    }
}
