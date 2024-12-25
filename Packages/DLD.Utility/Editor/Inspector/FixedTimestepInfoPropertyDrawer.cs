// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	[CustomPropertyDrawer(typeof(FixedTimestepInfoAttribute))]
	public class FixedTimestepInfoPropertyDrawer : PropertyDrawer
	{
		const int SPACING = 3;

		static readonly GUIContent GUIContentBuffer = new();

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var a = attribute as FixedTimestepInfoAttribute;
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

			string message = string.Format(a.Message, Time.fixedDeltaTime, (property.intValue * (1/Time.fixedDeltaTime)));
			GUIContentBuffer.text = message;
			float messageHeight = EditorStyles.helpBox.CalcHeight(GUIContentBuffer, EditorGUIUtility.currentViewWidth);

			return EditorGUI.GetPropertyHeight(property, label, true) + SPACING + messageHeight + SPACING;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var a = attribute as FixedTimestepInfoAttribute;
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

			string message = string.Format(a.Message, Time.fixedDeltaTime, (property.intValue * (1/Time.fixedDeltaTime)));
			GUIContentBuffer.text = message;
			float messageHeight = EditorStyles.helpBox.CalcHeight(GUIContentBuffer, EditorGUIUtility.currentViewWidth);


			Rect propertyRect = new Rect(position);
			propertyRect.height = position.height - SPACING - messageHeight - SPACING;
			EditorGUI.PropertyField(propertyRect, property, label, true);


			Rect messageRect = new Rect(position);
			messageRect.y += propertyRect.height + SPACING;
			messageRect.height = messageHeight;
			GUI.Label(messageRect, GUIContentBuffer, EditorStyles.helpBox);
		}
	}
}