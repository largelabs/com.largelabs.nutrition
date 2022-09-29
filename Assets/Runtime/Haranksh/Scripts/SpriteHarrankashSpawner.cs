using UnityEngine;

public class SpriteHarrankashSpawner : TransformSpawner<SpriteRenderer, HarraEnumReference.SpriteHarrankashTypes>
{
    private readonly static string ORANGE_PLAT = "SpriteHarra"; 
    private readonly static string ORANGE_PLAYER = "SpriteHarraPlayer"; 

    protected override string getPrefab(HarraEnumReference.SpriteHarrankashTypes i_prefabId)
    {
        if (i_prefabId == HarraEnumReference.SpriteHarrankashTypes.OrangePlat)
            return ORANGE_PLAT;
        else if (i_prefabId == HarraEnumReference.SpriteHarrankashTypes.OrangePlayer)
            return ORANGE_PLAYER;
        else
        {
            Debug.LogError("Invalid prefab Id! Returning null...");
            return null;
        }
    }

    protected override void resetComponent(SpriteRenderer i_component)
    {
    }
}
