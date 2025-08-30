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
				_floatRangeAttribute.MinLabel, _floatRangeAttribute.MinPostLabel,
				_floatRangeAttribute.MaxLabel, _floatRangeAttribute.MaxPostLabel);
		}

		public static void DrawGUI(Rect position, SerializedProperty property, GUIContent label, bool useAllAvailableSpace,
			string originalLabel, string customLabel, string minLabel, string minPostLabel, string maxLabel, string maxPostLabel)
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
			bool hasMinLabel = !string.IsNullOrWhiteSpace(minLabel);

			Vector2 minLabelSize;
			if (hasMinLabel)
			{
				label.text = minLabel;
				minLabelSize = EditorStyles.label.CalcSize(label);
			}
			else
			{
				minLabelSize = Vector2.zero;
			}

			// -----------------------
			bool hasMinPostLabel = !string.IsNullOrWhiteSpace(minPostLabel);

			Vector2 minPostLabelSize;
			if (hasMinPostLabel)
			{
				label.text = minPostLabel;
				minPostLabelSize = EditorStyles.label.CalcSize(label);
			}
			else
			{
				minPostLabelSize = Vector2.zero;
			}

			// -----------------------
			bool hasMaxLabel = !string.IsNullOrWhiteSpace(maxLabel);

			label.text = hasMaxLabel ? maxLabel : "to";
			var maxLabelSize = EditorStyles.label.CalcSize(label);

			bool hasMaxPostLabel = !string.IsNullOrWhiteSpace(maxPostLabel);
			Vector2 maxPostLabelSize;
			if (hasMaxPostLabel)
			{
				label.text = maxPostLabel;
				maxPostLabelSize = EditorStyles.label.CalcSize(label);
			}
			else
			{
				maxPostLabelSize = Vector2.zero;
			}

			float allLabelsWidth = minLabelSize.x + minPostLabelSize.x + maxLabelSize.x + maxPostLabelSize.x;

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

			var minProperty = property.FindPropertyRelative(nameof(FloatRange.Min));
			var maxProperty = property.FindPropertyRelative(nameof(FloatRange.Max));

			// -----------------------

			const float SPACE_BETWEEN_LOWER_LIMIT_AND_TO = 8;
			const float SPACE_BETWEEN_FIELD_AND_POST_LABEL = 2;

			float end;

			if (hasMinLabel)
			{
				var labelRect = new Rect(position);
				labelRect.width = labelSize.x;
				var indentedRect = EditorGUI.IndentedRect(labelRect);

				label.text = hasLabel ? customLabel : originalLabel;
				GUI.Label(indentedRect, label, EditorStyles.label);

				var minRect = new Rect(position);
				minRect.x = indentedRect.xMax;
				minRect.width = minLabelSize.x + inputFieldWidth - SPACE_BETWEEN_LOWER_LIMIT_AND_TO;

				EditorGUIUtility.labelWidth = minLabelSize.x + 3;

				EditorGUI.indentLevel = 0;
				label.text = minLabel;
				EditorGUI.PropertyField(minRect, minProperty, label);

				end = minRect.xMax;
			}
			else
			{
				var minRect = new Rect(position);
				minRect.width = labelSize.x + inputFieldWidth - SPACE_BETWEEN_LOWER_LIMIT_AND_TO;

				label.text = hasLabel ? customLabel : originalLabel;
				EditorGUI.PropertyField(minRect, minProperty, label);

				end = minRect.xMax;
			}

			// -----------------------

			if (hasMinPostLabel)
			{
				var minPostRect = new Rect(position);
				minPostRect.x = end + SPACE_BETWEEN_FIELD_AND_POST_LABEL;
				minPostRect.width = minPostLabelSize.x;

				label.text = minPostLabel;
				GUI.Label(minPostRect, label, EditorStyles.label);

				end = minPostRect.xMax;
			}
			else
			{
				end += SPACE_BETWEEN_LOWER_LIMIT_AND_TO;
			}

			// -----------------------

			int prevIndentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			var maxRect = new Rect(position);
			maxRect.x = end;
			maxRect.width = maxLabelSize.x + inputFieldWidth - SPACE_BETWEEN_LOWER_LIMIT_AND_TO;

			EditorGUIUtility.labelWidth = maxLabelSize.x + 3;

			label.text = hasMaxLabel ? maxLabel : "to";
			EditorGUI.PropertyField(maxRect, maxProperty, label);

			EditorGUI.indentLevel = prevIndentLevel;

			// -----------------------

			if (hasMaxPostLabel)
			{
				var maxPostLabelRect = new Rect(position);
				maxPostLabelRect.x = maxRect.xMax + SPACE_BETWEEN_FIELD_AND_POST_LABEL;
				maxPostLabelRect.width = maxPostLabelSize.x;

				label.text = maxPostLabel;
				GUI.Label(maxPostLabelRect, label, EditorStyles.label);
			}
		}
	}
}
