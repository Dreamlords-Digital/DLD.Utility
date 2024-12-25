// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	[CustomPropertyDrawer(typeof(RangeFloatAttribute))]
	public class RangeFloatPropertyDrawer : PropertyDrawer
	{
		RangeFloatAttribute _rangeFloatAttribute;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_rangeFloatAttribute ??= attribute as RangeFloatAttribute;
			if (_rangeFloatAttribute == null)
			{
				return;
			}

			string originalLabel = label.text;
			string customLabel = _rangeFloatAttribute.Label;
			string startLabel = _rangeFloatAttribute.StartLabel;
			string startPostLabel = _rangeFloatAttribute.StartPostLabel;
			string endLabel = _rangeFloatAttribute.EndLabel;
			string endPostLabel = _rangeFloatAttribute.EndPostLabel;
			bool useAllAvailableSpace = _rangeFloatAttribute.UseAllAvailableSpace;
			DrawGUI(position, property, label, useAllAvailableSpace,
				originalLabel, customLabel, startLabel, startPostLabel, endLabel, endPostLabel);
		}

		public static void DrawGUI(Rect position, SerializedProperty property, GUIContent label, bool useAllAvailableSpace,
			string originalLabel, string customLabel, string startLabel, string startPostLabel, string endLabel, string endPostLabel)
		{
			// -----------------------
			bool hasLabel = !string.IsNullOrWhiteSpace(customLabel);

			if (hasLabel)
			{
				label.text = customLabel;
			}

			Vector2 labelSize;
			if (useAllAvailableSpace)
			{
				labelSize = EditorStyles.label.CalcSize(label);
			}
			else
			{
				labelSize.x = EditorGUIUtility.labelWidth;
				labelSize.y = position.y;
			}

			// -----------------------
			bool hasStartLabel = !string.IsNullOrWhiteSpace(startLabel);

			Vector2 startLabelSize;
			if (hasStartLabel)
			{
				label.text = startLabel;
				startLabelSize = EditorStyles.label.CalcSize(label);
			}
			else
			{
				startLabelSize = Vector2.zero;
			}

			// -----------------------
			bool hasStartPostLabel = !string.IsNullOrWhiteSpace(startPostLabel);

			Vector2 startPostLabelSize;
			if (hasStartPostLabel)
			{
				label.text = startPostLabel;
				startPostLabelSize = EditorStyles.label.CalcSize(label);
			}
			else
			{
				startPostLabelSize = Vector2.zero;
			}

			// -----------------------
			bool hasEndLabel = !string.IsNullOrWhiteSpace(endLabel);

			label.text = hasEndLabel ? endLabel : "to";
			var endLabelSize = EditorStyles.label.CalcSize(label);

			bool hasEndPostLabel = !string.IsNullOrWhiteSpace(endPostLabel);
			Vector2 endPostLabelSize;
			if (hasEndPostLabel)
			{
				label.text = endPostLabel;
				endPostLabelSize = EditorStyles.label.CalcSize(label);
			}
			else
			{
				endPostLabelSize = Vector2.zero;
			}

			float allLabelsWidth = startLabelSize.x + startPostLabelSize.x + endLabelSize.x + endPostLabelSize.x;

			float inputFieldWidth;
			if (useAllAvailableSpace)
			{
				allLabelsWidth += labelSize.x;
				inputFieldWidth = (position.width - allLabelsWidth) * 0.5f;
			}
			else
			{
				inputFieldWidth = (position.width - EditorGUIUtility.labelWidth - allLabelsWidth) * 0.5f;
			}

			// -----------------------

			var startProperty = property.FindPropertyRelative(nameof(RangeFloat.Start));
			var endProperty = property.FindPropertyRelative(nameof(RangeFloat.End));

			// -----------------------

			const float SPACE_BETWEEN_START_AND_TO = 8;
			const float SPACE_BETWEEN_FIELD_AND_POST_LABEL = 2;

			float end;

			if (hasStartLabel)
			{
				var labelRect = new Rect(position);
				labelRect.width = labelSize.x;
				var indentedRect = EditorGUI.IndentedRect(labelRect);

				label.text = hasLabel ? customLabel : originalLabel;
				GUI.Label(indentedRect, label, EditorStyles.label);

				var startRect = new Rect(position);
				startRect.x = indentedRect.xMax;
				startRect.width = startLabelSize.x + inputFieldWidth - SPACE_BETWEEN_START_AND_TO;

				EditorGUIUtility.labelWidth = startLabelSize.x + 3;

				EditorGUI.indentLevel = 0;
				label.text = startLabel;
				EditorGUI.PropertyField(startRect, startProperty, label);

				end = startRect.xMax;
			}
			else
			{
				var startRect = new Rect(position);
				startRect.width = labelSize.x + inputFieldWidth - SPACE_BETWEEN_START_AND_TO;

				label.text = hasLabel ? customLabel : originalLabel;
				EditorGUI.PropertyField(startRect, startProperty, label);

				end = startRect.xMax;
			}

			// -----------------------

			if (hasStartPostLabel)
			{
				var startPostRect = new Rect(position);
				startPostRect.x = end + SPACE_BETWEEN_FIELD_AND_POST_LABEL;
				startPostRect.width = startPostLabelSize.x;

				label.text = startPostLabel;
				GUI.Label(startPostRect, label, EditorStyles.label);

				end = startPostRect.xMax;
			}
			else
			{
				end += SPACE_BETWEEN_START_AND_TO;
			}

			// -----------------------

			int prevIndentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			var endRect = new Rect(position);
			endRect.x = end;
			endRect.width = endLabelSize.x + inputFieldWidth - SPACE_BETWEEN_START_AND_TO;

			EditorGUIUtility.labelWidth = endLabelSize.x + 3;

			label.text = hasEndLabel ? endLabel : "to";
			EditorGUI.PropertyField(endRect, endProperty, label);

			EditorGUI.indentLevel = prevIndentLevel;

			// -----------------------

			if (hasEndPostLabel)
			{
				var endPostLabelRect = new Rect(position);
				endPostLabelRect.x = endRect.xMax + SPACE_BETWEEN_FIELD_AND_POST_LABEL;
				endPostLabelRect.width = endPostLabelSize.x;

				label.text = endPostLabel;
				GUI.Label(endPostLabelRect, label, EditorStyles.label);
			}
		}
	}
}