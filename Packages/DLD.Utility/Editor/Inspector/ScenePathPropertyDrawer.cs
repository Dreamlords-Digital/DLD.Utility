// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	/// <summary>
	/// <see cref="PropertyDrawer"/> for a string to force it to be assigned with scene paths only.
	/// </summary>
	[CustomPropertyDrawer(typeof(ScenePathAttribute))]
	public class ScenePathPropertyDrawer : PropertyDrawer
	{
		const float ERROR_MESSAGE_SPACING = 1;
		const float ERROR_MESSAGE_HEIGHT = 25;

		ScenePathAttribute _scenePathAttribute;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float baseHeight = base.GetPropertyHeight(property, label);

			var currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
			if (currentScene == null)
			{
				return baseHeight + ERROR_MESSAGE_SPACING + ERROR_MESSAGE_HEIGHT;
			}

			return baseHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_scenePathAttribute ??= attribute as ScenePathAttribute;
			bool showErrorIfUnassigned = _scenePathAttribute?.ShowErrorIfUnassigned ?? true;

			bool unassigned = string.IsNullOrEmpty(property.stringValue);
			var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);

			bool sceneNotFound = !unassigned && oldScene == null;
			bool showError = sceneNotFound || (unassigned && showErrorIfUnassigned);

			var prevColor = GUI.color;
			if (showError)
			{
				GUI.color = Color.red;
			}

			EditorGUI.BeginChangeCheck();

			Rect fieldRect = new Rect(position);
			fieldRect.height = base.GetPropertyHeight(property, label);
			var newScene = EditorGUI.ObjectField(fieldRect, label, oldScene, typeof(SceneAsset), false) as SceneAsset;

			if (showError)
			{
				GUI.color = prevColor;

				Rect errorMessageRect = new Rect(position);
				errorMessageRect.y += fieldRect.height + ERROR_MESSAGE_SPACING;
				errorMessageRect.height = ERROR_MESSAGE_HEIGHT;
				errorMessageRect.xMin += EditorGUIUtility.labelWidth;

				string message;
				if (sceneNotFound)
				{
					message = $"Not found: \"{property.stringValue}\"";
				}
				else
				{
					message = $"No assigned scene";
				}
				EditorGUI.HelpBox(errorMessageRect, message, MessageType.Error);
			}

			if (EditorGUI.EndChangeCheck())
			{
				string newPath = AssetDatabase.GetAssetPath(newScene);
				property.stringValue = newPath;
			}
		}
	}
}