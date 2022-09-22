using UnityEngine;
using UnityEngine.UI;

public class UIHarrankashSpawner : TransformSpawner<UIImageFrameSwapper, HarraEnumReference.UIHarrankashTypes>
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

    protected override void resetComponent(UIImageFrameSwapper i_component)
    {
        i_component.Stop();
        Image img = i_component.GetComponent<Image>();
        if (img != null)
            img.color = Color.white;
    }
}
