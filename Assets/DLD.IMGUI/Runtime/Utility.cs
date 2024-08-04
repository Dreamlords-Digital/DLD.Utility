// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using DLD.Utility;
using UnityEngine.Assertions;

namespace DLD.IMGUI
{

	public static class Utility
	{
		public static void ForceGuiPassive()
		{
#if UNITY_EDITOR
			// Workaround for bug in Unity 2018 to fix the problem
			// where mouse input doesn't work anymore for any EditorWindow
			// once the Character/Map Editor is opened.
			//
			// For some reason, CanvasObjectInspector.OnSceneGUI gets
			// called for each opened EditorWindow in the Unity Editor.
			// So we had to add this code to make sure we're only
			// doing this to the Scene View Window (where the Character/Map Editor
			// is drawn).
			//
			var mouseOverWindow = EditorWindow.mouseOverWindow;
			if (!EditorApplication.isPlaying && mouseOverWindow != null && mouseOverWindow.GetType() != typeof(SceneView))
			{
				//BetterDebug.LogError(this, "OnSceneGUI: Not in SceneView. Currently at: {0}", EditorWindow.mouseOverWindow.ToString());
				return;
			}

			//BetterDebug.Log("ForceGuiPassive. Event: {0} Currently at: {1}", Event.current.type, mouseOverWindow != null ? mouseOverWindow.ToString() : "null");

			// 0 signifies no control is active anymore,
			// and that we allow other controls to change selection
			const int ZERO_CONTROL_ID = 0;

			// This effectively prevents GameObject selection in the Scene View.
			GUIUtility.hotControl = ZERO_CONTROL_ID;
			GUIUtility.keyboardControl = ZERO_CONTROL_ID;
			HandleUtility.AddDefaultControl(ZERO_CONTROL_ID);
#endif
		}

		public static void LoseTextFocus()
		{
			GUIUtility.keyboardControl = 0;
		}

		public static void EatEvent()
		{
			EatEvent(Event.current);
		}

		public static void EatEvent(Event eventToEat)
		{
			//ForceGuiPassive();

			// Eat the event to prevent unity from acting like normal
			eventToEat.Use();
		}

		static GUISkin _defaultGUISkin;

		public static void RefreshGUISkin()
		{
			_defaultGUISkin = Resources.Load("EditorUI/NativeLookDark", typeof(GUISkin)) as GUISkin;
			Assert.IsNotNull(_defaultGUISkin,
				"could not load default GUI skin at Resources path: \"EditorUI/NativeLookDark\"");

			//DLD.Profiler.Instance.IndicateCall("Loaded NativeLookDark GUISkin from Resources");
		}

		public static GUISkin GetDefaultGUISkin()
		{
			if (_defaultGUISkin == null)
			{
				RefreshGUISkin();
			}

			return _defaultGUISkin;
		}

		public const int UNITY_TOP_BAR_HEIGHT = 38;

		/// <summary>
		/// Converts from a 3d world point to the 2d GUI point.
		/// Works properly in both Editor and in runtime.
		/// </summary>
		public static Vector2 WorldToGuiPoint(Vector3 worldPos)
		{
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				return HandleUtility.WorldToGUIPoint(worldPos);
			}
#endif
			Camera cameraUsed = Camera.main;
			return WorldToGuiPoint(worldPos, cameraUsed);
		}

		/// <summary>
		/// Converts from a 3d world point to the 2d GUI point.
		/// Works properly in both Editor and in runtime.
		/// Use this overload only if you need to specify a camera.
		/// </summary>
		/// <param name="cameraUsed">Camera used to determine world to GUI conversion, instead of the default used (Camera.main in runtime)</param>
		public static Vector2 WorldToGuiPoint(Vector3 worldPos, Camera cameraUsed)
		{
			if (cameraUsed == null)
			{
				return Vector2.zero;
			}

			Vector2 result = cameraUsed.WorldToScreenPoint(worldPos);

#if UNITY_EDITOR
			result.y = Screen.height - result.y - UNITY_TOP_BAR_HEIGHT;
#else
			result.y = Screen.height - result.y;
#endif

			return result;
		}

		public static string GetProperTextFieldGUIStyle(bool hasFocus)
		{
			return hasFocus ? "SearchTextField.Focused" : "SearchTextField";
		}

		public static string GetProperClearButtonGUIStyle(bool hasFocus)
		{
			return hasFocus ? "SearchClearButton.Focused" : "SearchClearButton";
		}
	}

}