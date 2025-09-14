// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{

[CustomPropertyDrawer(typeof(MessageFuncAttribute))]
public class MessageFuncPropertyDrawer : PropertyDrawer
{
	const int Spacing = 3;
	const float IndentWidth = 16;

	static readonly GUIContent GUIContentBuffer = new();

	Vector2 _propertyLabelSize;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		var a = attribute as MessageFuncAttribute;
		if (a == null)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		string result = Utility.GetMethodReturnValue<string>(property, a.FuncName);
		if (string.IsNullOrEmpty(result))
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		if (GUIContentBuffer.image == null)
		{
			GUIContentBuffer.image = EditorGUIUtility.IconContent("console.infoicon").image;
		}

		GUIContentBuffer.text = result;
		float messageHeight = EditorStyles.helpBox.CalcHeight(GUIContentBuffer, EditorGUIUtility.currentViewWidth);

		return EditorGUI.GetPropertyHeight(property, label, true) + Spacing + messageHeight + Spacing;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var a = attribute as MessageFuncAttribute;
		if (a == null)
		{
			return;
		}

		string result = Utility.GetMethodReturnValue<string>(property, a.FuncName);
		if (string.IsNullOrEmpty(result))
		{
			EditorGUI.PropertyField(position, property, label, true);
			return;
		}

		GUIContentBuffer.text = result;
		float messageHeight = EditorStyles.helpBox.CalcHeight(GUIContentBuffer, EditorGUIUtility.currentViewWidth);

		if (Event.current.type == EventType.Repaint)
		{
			_propertyLabelSize = EditorStyles.label.CalcSize(label);
		}

		EditorGUIUtility.labelWidth = _propertyLabelSize.x + (EditorGUI.indentLevel * IndentWidth) + 5;

		Rect propertyRect = new Rect(position);
		propertyRect.height = position.height - Spacing - messageHeight - Spacing;
		EditorGUI.PropertyField(propertyRect, property, label, true);

		EditorGUIUtility.labelWidth = 0;

		Rect messageRect = new Rect(position);
		messageRect.y += propertyRect.height + Spacing;
		messageRect.height = messageHeight;
		GUI.Label(messageRect, GUIContentBuffer, EditorStyles.helpBox);
	}
}

}
