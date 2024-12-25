// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;
using UnityEditor;

namespace DLD.Utility.Inspector.Editor
{
	[CustomEditor(typeof(GameObjectNotes))]
	public class GameObjectNotesInspector : UnityEditor.Editor
	{
		SerializedProperty _notesProperty;

		bool _edit;

		readonly GUIContent _labelContent = new();
		GUIStyle _labelStyle;

		void OnEnable()
		{
			_notesProperty = serializedObject.FindProperty(nameof(GameObjectNotes.Notes));

			_labelStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Game).label);
			_labelStyle.wordWrap = true;
		}

		public override void OnInspectorGUI()
		{
			if (_edit)
			{
				if (GUILayout.Button("Done"))
				{
					_edit = false;
				}

				EditorGUIUtility.labelWidth = 0;
				EditorGUILayout.PropertyField(_notesProperty, _labelContent);
				serializedObject.ApplyModifiedProperties();
			}
			else
			{
				if (GUILayout.Button("Edit"))
				{
					_edit = true;
				}

				GUILayout.Label(((GameObjectNotes) target).Notes, _labelStyle);
			}
		}
	}
}