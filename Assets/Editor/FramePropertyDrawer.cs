using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Frame<Sprite>))]
public class FramePropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//label = EditorGUI.BeginProperty(position, label, property);
		position.width /= 2.45f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("frameObject"), GUIContent.none);
		position.width *= 1.8f;
		position.x *= 2;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("screenTime"), new GUIContent(" "));
		//EditorGUI.EndProperty();
	}
}
