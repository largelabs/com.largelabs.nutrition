using UnityEngine;
using UnityEngine.UI;

public class UIHarrankashSpawner : TransformSpawner<Image, HarraEnumReference.UIHarrankashTypes>
{
    private readonly static string ORANGE_HARRA_ANIM = "UIHarra";

    protected override string getPrefab(HarraEnumReference.UIHarrankashTypes i_prefabId)
    {
        if (i_prefabId == HarraEnumReference.UIHarrankashTypes.Orange)
            return ORANGE_HARRA_ANIM;
        else
        {
            Debug.LogError("Invalid prefab Id! Returning null...");
            return null;
        }
    }

    protected override void resetComponent(Image i_component)
    {
      
    }
}
