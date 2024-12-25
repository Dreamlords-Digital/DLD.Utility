// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	[CustomPropertyDrawer(typeof(MessageIfAttribute))]
	public class MessageIfPropertyDrawer : PropertyDrawer
	{
		const int SPACING = 3;
		const float INDENT_WIDTH = 16;

		static readonly GUIContent GUIContentBuffer = new();

		Vector2 _propertyLabelSize;

		string GetMessage(SerializedObject o)
		{
			var a = attribute as MessageIfAttribute;
			if (a == null)
			{
				return null;
			}

			object property1 = MessagePropertyDrawer.GetValue(a.PropertyName1, o);
			object property2 = MessagePropertyDrawer.GetValue(a.PropertyName2, o);

			string message;
			if (property1 != null && property2 != null)
			{
				message = string.Format(a.Message, property1, property2);
			}
			else if (property1 != null)
			{
				message = string.Format(a.Message, property1);
			}
			else
			{
				message = a.Message;
			}

			return message;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var a = attribute as MessageIfAttribute;
			if (a == null)
			{
				return EditorGUI.GetPropertyHeight(property, label, true);
			}

			if (!string.IsNullOrEmpty(a.PropertyToCheck) &&
			    !ShowIfPropertyDrawer.IsConditionMet(property, a.PropertyToCheck, a.ValueToCheckAgainst, a.ComparisonType))
			{
				return EditorGUI.GetPropertyHeight(property, label, true);
			}

			if (GUIContentBuffer.image == null)
			{
				GUIContentBuffer.image = EditorGUIUtility.IconContent("console.infoicon").image;
			}

			GUIContentBuffer.text = GetMessage(property.serializedObject);
			float messageHeight = EditorStyles.helpBox.CalcHeight(GUIContentBuffer, EditorGUIUtility.currentViewWidth);

			return EditorGUI.GetPropertyHeight(property, label, true) + SPACING + messageHeight + SPACING;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var a = attribute as MessageIfAttribute;
			if (a == null)
			{
				return;
			}

			if (!string.IsNullOrEmpty(a.PropertyToCheck) &&
			    !ShowIfPropertyDrawer.IsConditionMet(property, a.PropertyToCheck, a.ValueToCheckAgainst, a.ComparisonType))
			{
				EditorGUI.PropertyField(position, property, label, true);
				return;
			}

			GUIContentBuffer.text = GetMessage(property.serializedObject);
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
		}
	}
}