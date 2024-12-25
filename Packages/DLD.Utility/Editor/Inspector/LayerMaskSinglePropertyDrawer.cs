// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using DLD.Utility;
using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	/// <summary>
	/// <see cref="PropertyDrawer"/> for <see cref="LayerMask"/> to force it to be assigned with one layer only.
	/// </summary>
	[CustomPropertyDrawer(typeof(LayerMaskSingleAttribute))]
	public class LayerMaskSinglePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			int layerIndex = property.intValue.FindBitIndex();
			property.intValue = 1 << EditorGUI.LayerField(position, label, layerIndex);

			EditorGUI.EndProperty();
		}
	}
}