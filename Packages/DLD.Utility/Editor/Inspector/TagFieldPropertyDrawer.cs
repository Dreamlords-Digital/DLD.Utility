// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	[CustomPropertyDrawer(typeof(TagFieldAttribute))]
	public class TagFieldPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			string displayedTag;
			if (string.IsNullOrEmpty(property.stringValue))
			{
				displayedTag = "Untagged";
			}
			else
			{
				displayedTag = property.stringValue;
			}

			property.stringValue = EditorGUI.TagField(position, label, displayedTag);

			EditorGUI.EndProperty();
		}
	}
}