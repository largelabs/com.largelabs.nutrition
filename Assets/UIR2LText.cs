using UnityEngine;
using UnityEngine.UI;

public class UIR2LText : MonoBehaviourBase
{
    [SerializeField] bool isArabic = false;
    [SerializeField] string englishString = null;
    [SerializeField] string arabicString = null;

    [SerializeField] Font englishFont = null;
    [SerializeField] Font arabicFont = null;

    [SerializeField] Text uiText = null;

    string fixedArabicString = null;

    private void OnValidate()
    {
        init();
    }

    void Start()
    {
        init();
    }

    private void init()
    {
        if (null == uiText) uiText = GetComponent<Text>();
        updateText();
    }

    void updateText()
    {
        if (null == uiText) return;
        
        if(true == isArabic)
        {
            fixedArabicString = ArabicFixerTool.FixLine(arabicString);
            uiText.text = fixedArabicString;
            uiText.font = arabicFont;
        }
        else
        {
            uiText.text = englishString;
            uiText.font = englishFont;
        }
    }
}
