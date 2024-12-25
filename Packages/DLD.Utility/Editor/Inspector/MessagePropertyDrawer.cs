// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	[CustomPropertyDrawer(typeof(MessageAttribute))]
	public class MessagePropertyDrawer : PropertyDrawer
	{
		const int SPACING = 3;
		const float INDENT_WIDTH = 16;

		static readonly GUIContent GUIContentBuffer = new();

		Vector2 _propertyLabelSize;

		public static object GetValue(string propertyName, SerializedObject o)
		{
			if (propertyName == "Time.fixedDeltaTime")
			{
				return Time.fixedDeltaTime;
			}
			else if (!string.IsNullOrEmpty(propertyName))
			{
				SerializedProperty p = o.FindProperty(propertyName);
				if (p != null)
				{
					return p.propertyType switch
					{
						SerializedPropertyType.Integer => p.intValue,
						SerializedPropertyType.Boolean => p.boolValue,
						SerializedPropertyType.Float => p.floatValue,
						SerializedPropertyType.String => p.stringValue,
						SerializedPropertyType.Color => p.colorValue,
						SerializedPropertyType.ObjectReference => p.objectReferenceValue,
						SerializedPropertyType.LayerMask => p.intValue,
						SerializedPropertyType.Vector2 => p.vector2Value,
						SerializedPropertyType.Vector3 => p.vector3Value,
						SerializedPropertyType.Vector4 => p.vector4Value,
						SerializedPropertyType.Rect => p.rectValue,
						SerializedPropertyType.Character => p.stringValue,
						SerializedPropertyType.AnimationCurve => p.animationCurveValue,
						SerializedPropertyType.Bounds => p.boundsValue,
						SerializedPropertyType.Quaternion => p.quaternionValue,
						SerializedPropertyType.ManagedReference => p.managedReferenceValue,
						SerializedPropertyType.ExposedReference => p.exposedReferenceValue,
						_ => null,
					};
				}
			}

			return null;
		}

		string GetMessage(SerializedObject o)
		{
			var a = attribute as MessageAttribute;
			if (a == null)
			{
				return null;
			}

			object property1 = GetValue(a.PropertyName1, o);
			object property2 = GetValue(a.PropertyName2, o);

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
			var a = attribute as MessageAttribute;
			if (a == null)
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
			var a = attribute as MessageAttribute;
			if (a == null)
			{
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