// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	[CustomPropertyDrawer(typeof(RangeFloatShowPropertyAttribute))]
	public class RangeFloatShowPropertyPropertyDrawer : PropertyDrawer
	{
		RangeFloatShowPropertyAttribute _rangeShowIf;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			_rangeShowIf ??= attribute as RangeFloatShowPropertyAttribute;
			if (_rangeShowIf == null)
			{
				return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(RangeFloat.Start)), label);
			}

			bool show = Utility.GetPropertyReturnValue<bool>(property, _rangeShowIf.PropertyName);

			if (!show && _rangeShowIf.HideType == HideType.DoNotDraw)
			{
				return -EditorGUIUtility.standardVerticalSpacing;
			}

			return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(RangeFloat.Start)), label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_rangeShowIf ??= attribute as RangeFloatShowPropertyAttribute;
			if (_rangeShowIf == null)
			{
				return;
			}

			bool show = Utility.GetPropertyReturnValue<bool>(property, _rangeShowIf.PropertyName);

			if (show || _rangeShowIf.HideType == HideType.ReadOnly)
			{
				string originalLabel = label.text;
				string customLabel = _rangeShowIf.Label;
				string startLabel = _rangeShowIf.StartLabel;
				string startPostLabel = _rangeShowIf.StartPostLabel;
				string endLabel = _rangeShowIf.EndLabel;
				string endPostLabel = _rangeShowIf.EndPostLabel;
				bool useAllAvailableSpace = _rangeShowIf.UseAllAvailableSpace;

				bool prevEnabled = GUI.enabled;
				if (!show)
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