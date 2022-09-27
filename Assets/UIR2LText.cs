using UnityEngine.UI;


public class UIR2LText : MonoBehaviourBase
{
    Text uiText = null;

    string originaltext = null;
    string fixedText = null;

    #region PUBLIC API

    [ExposePublicMethod(false)]
    public void FixText()
    {
        if (null == uiText) uiText = GetComponent<Text>();
        updateText(false);
    }

    [ExposePublicMethod(false)]
    public void Reset()
    {
        originaltext = null;
        fixedText = null;
    }

    #endregion

    void updateText(bool i_reset)
    {
        if (null == uiText) return;
        if (null == originaltext) originaltext = uiText.text;

        if (false == i_reset)
        {
            if (null == fixedText) fixedText = ArabicFixerTool.FixLine(originaltext);
        }
        else 
            fixedText = null;


        uiText.text = i_reset ? originaltext : fixedText;
    }
}
