// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	[CustomPropertyDrawer(typeof(RangeFloatShowIfAttribute))]
	public class RangeFloatShowIfPropertyDrawer : PropertyDrawer
	{
		RangeFloatShowIfAttribute _rangeShowIf;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			_rangeShowIf ??= attribute as RangeFloatShowIfAttribute;
			if (_rangeShowIf == null)
			{
				return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(RangeFloat.Start)), label);
			}

			if (!ShowIfPropertyDrawer.IsConditionMet(property, _rangeShowIf.PropertyToCheck, _rangeShowIf.ValueToCheckAgainst, _rangeShowIf.ComparisonType) &&
			    _rangeShowIf.HideType == HideType.DoNotDraw)
			{
				return -EditorGUIUtility.standardVerticalSpacing;
			}

			return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(RangeFloat.Start)), label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_rangeShowIf ??= attribute as RangeFloatShowIfAttribute;
			if (_rangeShowIf == null)
			{
				return;
			}

			bool conditionMet = ShowIfPropertyDrawer.IsConditionMet(property, _rangeShowIf.PropertyToCheck,
				_rangeShowIf.ValueToCheckAgainst, _rangeShowIf.ComparisonType);
			if (conditionMet || _rangeShowIf.HideType == HideType.ReadOnly)
			{
				string originalLabel = label.text;
				string customLabel = _rangeShowIf.Label;
				string startLabel = _rangeShowIf.StartLabel;
				string startPostLabel = _rangeShowIf.StartPostLabel;
				string endLabel = _rangeShowIf.EndLabel;
				string endPostLabel = _rangeShowIf.EndPostLabel;
				bool useAllAvailableSpace = _rangeShowIf.UseAllAvailableSpace;

				bool prevEnabled = GUI.enabled;
				if (!conditionMet)
				{
					GUI.enabled = false;
				}

				RangeFloatPropertyDrawer.DrawGUI(position, property, label, useAllAvailableSpace,
					originalLabel, customLabel, startLabel, startPostLabel, endLabel, endPostLabel);

				GUI.enabled = prevEnabled;
			}
		}
	}
}