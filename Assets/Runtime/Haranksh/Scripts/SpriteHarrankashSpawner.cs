using UnityEngine;

public class SpriteHarrankashSpawner : TransformSpawner<SpriteFrameSwapper, SpriteHarrankashTypes>
{
    private readonly static string ORANGE_HARRA_ANIM = "SpriteHarra"; 

    protected override string getPrefab(SpriteHarrankashTypes i_prefabId)
    {
        if (i_prefabId == SpriteHarrankashTypes.Orange)
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
