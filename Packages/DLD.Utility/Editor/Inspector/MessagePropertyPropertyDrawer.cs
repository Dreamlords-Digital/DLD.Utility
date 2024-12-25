// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	[CustomPropertyDrawer(typeof(MessagePropertyAttribute))]
	public class MessagePropertyPropertyDrawer : PropertyDrawer
	{
		const int SPACING = 3;
		const float INDENT_WIDTH = 16;

		static readonly GUIContent GUIContentBuffer = new();

		Vector2 _propertyLabelSize;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var a = attribute as MessagePropertyAttribute;
			if (a == null)
			{
				return EditorGUI.GetPropertyHeight(property, label, true);
			}

			string result = Utility.GetPropertyReturnValue<string>(property, a.PropertyName);
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

			return EditorGUI.GetPropertyHeight(property, label, true) + SPACING + messageHeight + SPACING;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var a = attribute as MessagePropertyAttribute;
			if (a == null)
			{
				return;
			}

			string result = Utility.GetPropertyReturnValue<string>(property, a.PropertyName);
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

			EditorGUIUtility.labelWidth = _propertyLabelSize.x + (EditorGUI.indentLevel * INDENT_WIDTH) + 5;

			Rect propertyRect = new Rect(position);
			propertyRect.height = position.height - SPACING - messageHeight - SPACING;
			EditorGUI.PropertyField(propertyRect, property, label, true);

			EditorGUIUtility.labelWidth = 0;

			Rect messageRect = new Rect(position);
			messageRect.y += propertyRect.height + SPACING;
			messageRect.height = messageHeight;
			GUI.Label(messageRect, GUIContentBuffer, EditorStyles.helpBox);

			if (a.RepaintEveryFrame)
			{
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}
		}
	}
}