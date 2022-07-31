using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Frame<>))]
public class FramePropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);
		position.width = 150;
		EditorGUI.LabelField(position, "frameObject");
		position.x += 80;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("frameObject"), GUIContent.none);
		position.x += 160;
		EditorGUI.LabelField(position, "screenTime");
		position.x += 80;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("screenTime"), GUIContent.none);
		EditorGUI.EndProperty();
	}
}
