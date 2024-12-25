// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	[CustomPropertyDrawer(typeof(RelabelIfAttribute))]
	public class RelabelIfPropertyDrawer : PropertyDrawer
	{
		RelabelIfAttribute _relabelIf;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			_relabelIf ??= attribute as RelabelIfAttribute;
			if (_relabelIf == null)
			{
				return EditorGUI.GetPropertyHeight(property, label, true);
			}

			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_relabelIf ??= attribute as RelabelIfAttribute;
			if (_relabelIf == null)
			{
				EditorGUI.PropertyField(position, property, label, true);
				return;
			}

			if (ShowIfPropertyDrawer.IsConditionMet(property, _relabelIf.PropertyToCheck, _relabelIf.ValueToCheckAgainst, _relabelIf.ComparisonType))
			{
				label.text = _relabelIf.NewLabel;
			}

			EditorGUI.PropertyField(position, property, label, true);
		}
	}
}