// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	[CustomPropertyDrawer(typeof(FloatRangeAttribute))]
	public class FloatRangePropertyDrawer : PropertyDrawer
	{
		FloatRangeAttribute _floatRangeAttribute;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_floatRangeAttribute ??= attribute as FloatRangeAttribute;
			if (_floatRangeAttribute == null)
			{
				return;
			}

			DrawGUI(position, property, label, _floatRangeAttribute.UseAllAvailableSpace,
				label.text, _floatRangeAttribute.Label,
				_floatRangeAttribute.LowerLimitLabel, _floatRangeAttribute.LowerLimitPostLabel,
				_floatRangeAttribute.UpperLimitLabel, _floatRangeAttribute.UpperLimitPostLabel);
		}

		public static void DrawGUI(Rect position, SerializedProperty property, GUIContent label, bool useAllAvailableSpace,
			string originalLabel, string customLabel, string lowerLimitLabel, string lowerLimitPostLabel, string upperLimitLabel, string upperLimitPostLabel)
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
			bool hasLowerLimitLabel = !string.IsNullOrWhiteSpace(lowerLimitLabel);

			Vector2 lowerLimitLabelSize;
			if (hasLowerLimitLabel)
			{
				label.text = lowerLimitLabel;
				lowerLimitLabelSize = EditorStyles.label.CalcSize(label);
			}
			else
			{
				lowerLimitLabelSize = Vector2.zero;
			}

			// -----------------------
			bool hasLowerLimitPostLabel = !string.IsNullOrWhiteSpace(lowerLimitPostLabel);

			Vector2 lowerLimitPostLabelSize;
			if (hasLowerLimitPostLabel)
			{
				label.text = lowerLimitPostLabel;
				lowerLimitPostLabelSize = EditorStyles.label.CalcSize(label);
			}
			else
			{
				lowerLimitPostLabelSize = Vector2.zero;
			}

			// -----------------------
			bool hasUpperLimitLabel = !string.IsNullOrWhiteSpace(upperLimitLabel);

			label.text = hasUpperLimitLabel ? upperLimitLabel : "to";
			var upperLimitLabelSize = EditorStyles.label.CalcSize(label);

			bool hasUpperLimitPostLabel = !string.IsNullOrWhiteSpace(upperLimitPostLabel);
			Vector2 upperLimitPostLabelSize;
			if (hasUpperLimitPostLabel)
			{
				label.text = upperLimitPostLabel;
				upperLimitPostLabelSize = EditorStyles.label.CalcSize(label);
			}
			else
			{
				upperLimitPostLabelSize = Vector2.zero;
			}

			float allLabelsWidth = lowerLimitLabelSize.x + lowerLimitPostLabelSize.x + upperLimitLabelSize.x + upperLimitPostLabelSize.x;

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

			var lowerLimitProperty = property.FindPropertyRelative(nameof(FloatRange.LowerLimit));
			var upperLimitProperty = property.FindPropertyRelative(nameof(FloatRange.UpperLimit));

			// -----------------------

			const float SPACE_BETWEEN_LOWER_LIMIT_AND_TO = 8;
			const float SPACE_BETWEEN_FIELD_AND_POST_LABEL = 2;

			float end;

			if (hasLowerLimitLabel)
			{
				var labelRect = new Rect(position);
				labelRect.width = labelSize.x;
				var indentedRect = EditorGUI.IndentedRect(labelRect);

				label.text = hasLabel ? customLabel : originalLabel;
				GUI.Label(indentedRect, label, EditorStyles.label);

				var lowerLimitRect = new Rect(position);
				lowerLimitRect.x = indentedRect.xMax;
				lowerLimitRect.width = lowerLimitLabelSize.x + inputFieldWidth - SPACE_BETWEEN_LOWER_LIMIT_AND_TO;

				EditorGUIUtility.labelWidth = lowerLimitLabelSize.x + 3;

				EditorGUI.indentLevel = 0;
				label.text = lowerLimitLabel;
				EditorGUI.PropertyField(lowerLimitRect, lowerLimitProperty, label);

				end = lowerLimitRect.xMax;
			}
			else
			{
				var lowerLimitRect = new Rect(position);
				lowerLimitRect.width = labelSize.x + inputFieldWidth - SPACE_BETWEEN_LOWER_LIMIT_AND_TO;

				label.text = hasLabel ? customLabel : originalLabel;
				EditorGUI.PropertyField(lowerLimitRect, lowerLimitProperty, label);

				end = lowerLimitRect.xMax;
			}

			// -----------------------

			if (hasLowerLimitPostLabel)
			{
				var lowerLimitPostRect = new Rect(position);
				lowerLimitPostRect.x = end + SPACE_BETWEEN_FIELD_AND_POST_LABEL;
				lowerLimitPostRect.width = lowerLimitPostLabelSize.x;

				label.text = lowerLimitPostLabel;
				GUI.Label(lowerLimitPostRect, label, EditorStyles.label);

				end = lowerLimitPostRect.xMax;
			}
			else
			{
				end += SPACE_BETWEEN_LOWER_LIMIT_AND_TO;
			}

			// -----------------------

			int prevIndentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			var upperLimitRect = new Rect(position);
			upperLimitRect.x = end;
			upperLimitRect.width = upperLimitLabelSize.x + inputFieldWidth - SPACE_BETWEEN_LOWER_LIMIT_AND_TO;

			EditorGUIUtility.labelWidth = upperLimitLabelSize.x + 3;

			label.text = hasUpperLimitLabel ? upperLimitLabel : "to";
			EditorGUI.PropertyField(upperLimitRect, upperLimitProperty, label);

			EditorGUI.indentLevel = prevIndentLevel;

			// -----------------------

			if (hasUpperLimitPostLabel)
			{
				var upperLimitPostLabelRect = new Rect(position);
				upperLimitPostLabelRect.x = upperLimitRect.xMax + SPACE_BETWEEN_FIELD_AND_POST_LABEL;
				upperLimitPostLabelRect.width = upperLimitPostLabelSize.x;

				label.text = upperLimitPostLabel;
				GUI.Label(upperLimitPostLabelRect, label, EditorStyles.label);
			}
		}
	}
}
