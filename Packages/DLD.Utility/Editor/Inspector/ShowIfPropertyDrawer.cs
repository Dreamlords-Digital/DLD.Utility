// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	[CustomPropertyDrawer(typeof(ShowIfAttribute))]
	public class ShowIfPropertyDrawer : PropertyDrawer
	{
		ShowIfAttribute _showIf;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			_showIf ??= attribute as ShowIfAttribute;
			if (_showIf == null)
			{
				return EditorGUI.GetPropertyHeight(property, label, true);
			}

			if (!IsConditionMet(property, _showIf.PropertyToCheck, _showIf.ValueToCheckAgainst, _showIf.ComparisonType) &&
			    _showIf.HideType == HideType.DoNotDraw)
			{
				return -EditorGUIUtility.standardVerticalSpacing;
			}

			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		const string ARRAY_START_MARKER = ".Array.data[";

		public static bool IsConditionMet(SerializedProperty property, string propertyNameToCheck, object valueToCheckAgainst, ComparisonType comparisonType)
		{
			SerializedProperty fieldToCheck;

			if (property.propertyPath.Contains(ARRAY_START_MARKER))
			{
				string relativePath =
					property.propertyPath.Substring(0, property.propertyPath.LastIndexOf(".", StringComparison.Ordinal));
				string propertyPathToCheck = $"{relativePath}.{propertyNameToCheck}";
				fieldToCheck = property.serializedObject.FindProperty(propertyPathToCheck);
			}
			else
			{
				fieldToCheck = property.serializedObject.FindProperty(propertyNameToCheck);
			}

			if (fieldToCheck == null)
			{
				Debug.LogError($"Property {propertyNameToCheck} not found in {property.propertyPath}");
				return false;
			}

			// Is the condition met? Should the field be drawn?
			bool conditionMet = false;

			switch (fieldToCheck.propertyType)
			{
				case SerializedPropertyType.Boolean:
				{
					switch (comparisonType)
					{
						case ComparisonType.NotEqual:
							conditionMet = !fieldToCheck.boolValue.Equals(valueToCheckAgainst);
							break;
						default: // assume we are checking for equality
							conditionMet = fieldToCheck.boolValue.Equals(valueToCheckAgainst);
							break;
					}

					break;
				}
				case SerializedPropertyType.Enum:
				{
					int intValueToCheckAgainst = (int) valueToCheckAgainst;
					switch (comparisonType)
					{
						case ComparisonType.Equals:
							conditionMet = fieldToCheck.intValue == intValueToCheckAgainst;
							break;
						case ComparisonType.NotEqual:
							conditionMet = fieldToCheck.intValue != intValueToCheckAgainst;
							break;
						case ComparisonType.FlagCheckLenient:
							conditionMet = (fieldToCheck.intValue & intValueToCheckAgainst) > 0;
							break;
						case ComparisonType.FlagCheckStrict:
							conditionMet = (fieldToCheck.intValue & intValueToCheckAgainst) == intValueToCheckAgainst;
							break;
					}

					break;
				}
				case SerializedPropertyType.Integer:
				{
					int intValueToCheckAgainst = (int) valueToCheckAgainst;
					switch (comparisonType)
					{
						case ComparisonType.Equals:
							conditionMet = fieldToCheck.intValue == intValueToCheckAgainst;
							break;
						case ComparisonType.NotEqual:
							conditionMet = fieldToCheck.intValue != intValueToCheckAgainst;
							break;
						case ComparisonType.GreaterThan:
							conditionMet = fieldToCheck.intValue > intValueToCheckAgainst;
							break;
						case ComparisonType.LesserThan:
							conditionMet = fieldToCheck.intValue < intValueToCheckAgainst;
							break;
						case ComparisonType.LesserThanOrEqual:
							conditionMet = fieldToCheck.intValue <= intValueToCheckAgainst;
							break;
						case ComparisonType.GreaterThanOrEqual:
							conditionMet = fieldToCheck.intValue >= intValueToCheckAgainst;
							break;
						case ComparisonType.FlagCheckLenient:
							conditionMet = (fieldToCheck.intValue & intValueToCheckAgainst) > 0;
							break;
						case ComparisonType.FlagCheckStrict:
							conditionMet = (fieldToCheck.intValue & intValueToCheckAgainst) == intValueToCheckAgainst;
							break;
					}

					break;
				}
				case SerializedPropertyType.Float:
				{
					float floatValueToCheckAgainst = (float) valueToCheckAgainst;
					switch (comparisonType)
					{
						case ComparisonType.Equals:
							conditionMet = Math.Abs(fieldToCheck.floatValue - floatValueToCheckAgainst) < float.Epsilon;
							break;
						case ComparisonType.NotEqual:
							conditionMet = Math.Abs(fieldToCheck.floatValue - floatValueToCheckAgainst) > float.Epsilon;
							break;
						case ComparisonType.GreaterThan:
							conditionMet = fieldToCheck.floatValue > floatValueToCheckAgainst;
							break;
						case ComparisonType.LesserThan:
							conditionMet = fieldToCheck.floatValue < floatValueToCheckAgainst;
							break;
						case ComparisonType.LesserThanOrEqual:
							conditionMet = fieldToCheck.floatValue <= floatValueToCheckAgainst;
							break;
						case ComparisonType.GreaterThanOrEqual:
							conditionMet = fieldToCheck.floatValue >= floatValueToCheckAgainst;
							break;
					}

					break;
				}
				case SerializedPropertyType.String:
				{
					string stringValueToCheckAgainst = (string) valueToCheckAgainst;
					switch (comparisonType)
					{
						case ComparisonType.Equals:
							conditionMet = !fieldToCheck.stringValue.Equals(stringValueToCheckAgainst, StringComparison.Ordinal);
							break;
						case ComparisonType.NotEqual:
							conditionMet = fieldToCheck.stringValue.Equals(stringValueToCheckAgainst, StringComparison.Ordinal);
							break;
						case ComparisonType.GreaterThan:
							conditionMet =
								string.Compare(fieldToCheck.stringValue, stringValueToCheckAgainst,
									StringComparison.Ordinal) > 0;
							break;
						case ComparisonType.LesserThan:
							conditionMet =
								string.Compare(fieldToCheck.stringValue, stringValueToCheckAgainst,
									StringComparison.Ordinal) < 0;
							break;
						case ComparisonType.LesserThanOrEqual:
							conditionMet =
								string.Compare(fieldToCheck.stringValue, stringValueToCheckAgainst,
									StringComparison.Ordinal) <= 0;
							break;
						case ComparisonType.GreaterThanOrEqual:
							conditionMet =
								string.Compare(fieldToCheck.stringValue, stringValueToCheckAgainst,
									StringComparison.Ordinal) >= 0;
							break;
					}

					break;
				}
				case SerializedPropertyType.ManagedReference:
				{
					switch (comparisonType)
					{
						case ComparisonType.NotEqual:
							conditionMet = !fieldToCheck.managedReferenceValue.Equals(valueToCheckAgainst);
							break;
						default: // assume we are checking for equality
							conditionMet = fieldToCheck.managedReferenceValue.Equals(valueToCheckAgainst);
							break;
					}

					break;
				}
				case SerializedPropertyType.ObjectReference:
				{
					if (fieldToCheck.objectReferenceValue == null && valueToCheckAgainst == null)
					{
						if (comparisonType == ComparisonType.NotEqual)
						{
							return false;
						}
						else // assume we are checking for equality
						{
							return true;
						}
					}

					switch (comparisonType)
					{
						case ComparisonType.NotEqual:
							conditionMet = !fieldToCheck.objectReferenceValue.Equals(valueToCheckAgainst);
							break;
						default: // assume we are checking for equality
							conditionMet = fieldToCheck.objectReferenceValue.Equals(valueToCheckAgainst);
							break;
					}

					break;
				}
			}

			return conditionMet;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_showIf ??= attribute as ShowIfAttribute;
			if (_showIf == null)
			{
				EditorGUI.PropertyField(position, property, label, true);
				return;
			}

			if (IsConditionMet(property, _showIf.PropertyToCheck, _showIf.ValueToCheckAgainst, _showIf.ComparisonType))
			{
				EditorGUI.PropertyField(position, property, label, true);
			}
			else if (_showIf.HideType == HideType.ReadOnly)
			{
				bool prevEnabled = GUI.enabled;
				GUI.enabled = false;
				EditorGUI.PropertyField(position, property, label, true);
				GUI.enabled = prevEnabled;
			}
		}
	}
}