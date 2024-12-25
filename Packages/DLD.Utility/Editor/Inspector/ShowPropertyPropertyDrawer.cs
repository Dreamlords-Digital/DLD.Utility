// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	[CustomPropertyDrawer(typeof(ShowPropertyAttribute))]
	public class ShowPropertyPropertyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var a = attribute as ShowPropertyAttribute;
			if (a == null)
			{
				return EditorGUI.GetPropertyHeight(property, label, true);
			}

			bool show = Utility.GetPropertyReturnValue<bool>(property, a.PropertyName);

			if (!show && a.HideType == HideType.DoNotDraw)
			{
				return -EditorGUIUtility.standardVerticalSpacing;
			}

			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var a = attribute as ShowPropertyAttribute;
			if (a == null)
			{
				return;
			}

			bool show = Utility.GetPropertyReturnValue<bool>(property, a.PropertyName);

			if (show)
			{
				EditorGUI.PropertyField(position, property, label, true);
			}
			else if (a.HideType == HideType.ReadOnly)
			{
				bool prevEnabled = GUI.enabled;
				GUI.enabled = false;
				EditorGUI.PropertyField(position, property, label, true);
				GUI.enabled = prevEnabled;
			}
		}
	}
}