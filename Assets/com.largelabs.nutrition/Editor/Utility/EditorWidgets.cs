using UnityEditor;
using UnityEngine;

public enum InfoLabelMode
{
    Invalid = 0, 
}

public static class EditorWidgets
{
    public enum InfoLabelType
    {
        Normal,
        Valid,
        Invalid
    }


    public static void TitleUI(string i_title, bool i_subtitle = false)
    {
        Color col = GUI.color;
        GUI.color = Color.cyan;

        if (false == i_subtitle)
            GUILayout.Space(10);

        SeparatorUI(i_subtitle);

        if (false == i_subtitle)
            GUILayout.Label(i_title, EditorStyles.largeLabel);
        else
            GUILayout.Label(i_title, EditorStyles.label);

        GUI.color = col;

        GUILayout.Space(10);
    }

    public static void InfoLabelUI(string i_info, InfoLabelType i_labelType)
    {
        GUILayout.Space(10);

        Color col = GUI.color;
        GUI.color = Color.cyan;

        if (i_labelType == InfoLabelType.Valid)
        {
            GUI.color = Color.green;
        }
        else if (i_labelType == InfoLabelType.Invalid)
        {
            GUI.color = Color.red;
        }

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label(i_info, style);
        GUI.color = col;

        GUILayout.Space(10);
    }

    public static void SeparatorUI(bool i_mini = false)
    {
        if (false == i_mini)
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        else
            EditorGUILayout.LabelField("________________________________________________________________________________________________________________________________________________________________");
    }
}

