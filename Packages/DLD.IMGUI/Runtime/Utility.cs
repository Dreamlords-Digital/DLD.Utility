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
		public static void GetAvailableSize(out int width, out int height)
		{
#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isPlaying)
			{
#endif
				width = Screen.width;
				height = Screen.height;
				return;
#if UNITY_EDITOR
			}
#endif

#if UNITY_EDITOR
			const int SCENE_VIEW_UPPER_BAR_HEIGHT = 17;
			if (UnityEditor.EditorWindow.focusedWindow != null && UnityEditor.EditorWindow.focusedWindow.GetType() == typeof(UnityEditor.SceneView))
			{
				//return Mathf.RoundToInt(EditorWindow.focusedWindow.position.height) + ALLOWANCE - UPPER_BAR_HEIGHT;

				var size = UnityEditor.EditorWindow.focusedWindow.position;
				width = Mathf.RoundToInt(size.width);
				height = Mathf.RoundToInt(size.height) - SCENE_VIEW_UPPER_BAR_HEIGHT;
				return;
			}
			else if (UnityEditor.SceneView.lastActiveSceneView != null)
			{
				// SceneView.lastActiveSceneView seems to be buggy, or we are misunderstanding what it means.
				// if you have two scene views as tabs in the same area, SceneView.lastActiveSceneView seems
				// to always point to the leftmost one, even if it's not the active/focused scene view.
				var size = UnityEditor.SceneView.lastActiveSceneView.position;
				width = Mathf.RoundToInt(size.width);
				height = Mathf.RoundToInt(size.height) - SCENE_VIEW_UPPER_BAR_HEIGHT;
				return;
			}
#endif
			width = Screen.width;
			height = Screen.height;
		}

		/// <summary>
		/// Fits Rect position into the Screen as much as possible. When in the Unity Editor during Edit Mode, this will use the SceneView window's size
		/// (minus the height of the SceneView's top toolbar).
		///
		/// The Rect's size is expected to be smaller than the screen. If not, set <see cref="allowChangingWidth"/> and <see cref="allowChangingHeight"/> to true to allow
		/// it to change the Rect's size to fit into the screen.
		///
		/// The Rect's initial position values are expected to be global screen coordinates,
		/// meaning (0, 0) is top-left of entire screen, (width, height) is bottom-right of entire screen.
		/// </summary>
		/// <param name="rectToAdjust">The Rect values to adjust.</param>
		/// <param name="screenMarginLeft">Distance from the left edge of the screen to give the tooltip some space, if it was adjusted to be anchored to the left edge of the screen.</param>
		/// <param name="screenMarginRight">Distance from the right edge of the screen to give the tooltip some space, if it was adjusted to be anchored to the right edge of the screen.</param>
		/// <param name="screenMarginTop">Distance from the top edge of the screen to give the tooltip some space, if it was adjusted to be anchored to the top edge of the screen.</param>
		/// <param name="screenMarginBottom">Distance from the bottom edge of the screen to give the tooltip some space, if it was adjusted to be anchored to the bottom edge of the screen.</param>
		/// <param name="allowChangingWidth">If the Rect is larger than the screen width, set this to true to allow it to shrink the Rect width to fit in the screen.</param>
		/// <param name="allowChangingHeight">If the Rect is larger than the screen height, set this to true to allow it to shrink the Rect height to fit in the screen.</param>
		/// <returns></returns>
		public static Rect FitInScreen(this Rect rectToAdjust,
			float screenMarginLeft, float screenMarginRight, float screenMarginTop, float screenMarginBottom,
			bool allowChangingWidth = false, bool allowChangingHeight = false)
		{
			GetAvailableSize(out var screenWidth, out var screenHeight);

			if (rectToAdjust.height > screenHeight && allowChangingHeight)
			{
				// rect is too big for screen height
				// just fill entire screen height
				rectToAdjust.y = screenMarginTop;
				rectToAdjust.height = screenHeight - screenMarginTop - screenMarginBottom;
			}
			else
			{
				bool tooFarUp = rectToAdjust.y < 0;
				bool tooFarDown = rectToAdjust.yMax > screenHeight;

				if (tooFarUp && !tooFarDown)
				{
					// if rect is too far up the screen, bring it down
					rectToAdjust.y = screenMarginTop;
				}
				else if (!tooFarUp && tooFarDown)
				{
					// if rect is too far down the screen, bring it up
					rectToAdjust.y = screenHeight - rectToAdjust.height - screenMarginBottom;
				}
			}

			if (rectToAdjust.width > screenWidth && allowChangingWidth)
			{
				// rect is too big for screen width
				// just fill entire screen width
				rectToAdjust.x = screenMarginLeft;
				rectToAdjust.width = screenWidth - screenMarginLeft - screenMarginRight;
			}
			else
			{
				bool tooFarToTheLeft = rectToAdjust.x < 0;
				bool tooFarToTheRight = rectToAdjust.xMax > screenWidth;

				if (tooFarToTheLeft && !tooFarToTheRight)
				{
					// if rect is too far to the left of the screen, move it to the right
					rectToAdjust.x = screenMarginLeft;
				}
				else if (!tooFarToTheLeft && tooFarToTheRight)
				{
					// if rect is too far to the right of the screen, move it to the left
					rectToAdjust.x = screenWidth - rectToAdjust.width - screenMarginRight;
				}
			}

			return rectToAdjust;
		}

		/// <summary>
		/// Fits Rect position into the Screen as much as possible. When in the Unity Editor during Edit Mode, this will use the SceneView window's size
		/// (minus the height of the SceneView's top toolbar).
		///
		/// The Rect's size is expected to be smaller than the screen. If not, set <see cref="allowChangingWidth"/> and <see cref="allowChangingHeight"/> to true to allow
		/// it to change the Rect's size to fit into the screen.
		///
		/// The Rect's initial position values are expected to be global screen coordinates,
		/// meaning (0, 0) is top-left of entire screen, (width, height) is bottom-right of entire screen.
		///
		/// If the Rect's X position needs adjustment, it will move the X position to the specified <see cref="alternativeXPos"/>.
		/// </summary>
		/// <param name="rectToAdjust">The Rect values to adjust.</param>
		/// <param name="alternativeXPos">If after being adjusted, the Rect ends up covering the mouse, it will move the Rect's X position to this value.</param>
		/// <param name="screenMarginLeft">Distance from the left edge of the screen to give the tooltip some space, if it was adjusted to be anchored to the left edge of the screen.</param>
		/// <param name="screenMarginRight">Distance from the right edge of the screen to give the tooltip some space, if it was adjusted to be anchored to the right edge of the screen.</param>
		/// <param name="screenMarginTop">Distance from the top edge of the screen to give the tooltip some space, if it was adjusted to be anchored to the top edge of the screen.</param>
		/// <param name="screenMarginBottom">Distance from the bottom edge of the screen to give the tooltip some space, if it was adjusted to be anchored to the bottom edge of the screen.</param>
		/// <param name="allowChangingWidth">If the Rect is larger than the screen width, set this to true to allow it to shrink the Rect width to fit in the screen.</param>
		/// <param name="allowChangingHeight">If the Rect is larger than the screen height, set this to true to allow it to shrink the Rect height to fit in the screen.</param>
		/// <returns></returns>
		public static Rect FitInScreenWithXFlip(this Rect rectToAdjust, float alternativeXPos,
			float screenMarginLeft, float screenMarginRight, float screenMarginTop, float screenMarginBottom, bool allowChangingWidth = false, bool allowChangingHeight = false)
		{
			float initialXPos = rectToAdjust.x;
			rectToAdjust = rectToAdjust.FitInScreen(screenMarginLeft, screenMarginRight, screenMarginTop, screenMarginBottom);

			if (!Mathf.Approximately(initialXPos, rectToAdjust.x))
			{
				rectToAdjust.x = alternativeXPos;
			}

			return rectToAdjust;
		}

		public static bool IsRepaint(this Event e)
		{
			return (e.type == EventType.Repaint);
		}

		public static bool IsLeftMouseButtonUsed(this Event e)
		{
			return (e.button == 0);
		}

		public static bool IsRightMouseButtonUsed(this Event e)
		{
			return (e.button == 1);
		}

		public static bool IsLeftMouseUp(this Event e)
		{
			return (e.type == EventType.MouseUp) && (e.button == 0);
		}

		public static bool IsRightMouseUp(this Event e)
		{
			return (e.type == EventType.MouseUp) && (e.button == 1);
		}

		public static bool IsLeftMouseDown(this Event e)
		{
			return (e.type == EventType.MouseDown) && (e.button == 0);
		}

		public static bool IsRightMouseDown(this Event e)
		{
			return (e.type == EventType.MouseDown) && (e.button == 1);
		}

		public static bool IsLeftMouseClick(this Event e)
		{
			return (e.type == EventType.MouseUp) && (e.button == 0) && (e.clickCount == 1);
		}

		public static bool IsRightMouseClick(this Event e)
		{
			return (e.type == EventType.MouseUp) && (e.button == 1) && (e.clickCount == 1);
		}

		public static bool IsLeftMouseDoubleClick(this Event e)
		{
			return (e.type == EventType.MouseUp) && (e.button == 0) && (e.clickCount == 2);
		}

		public static bool IsRightMouseDoubleClick(this Event e)
		{
			return (e.type == EventType.MouseUp) && (e.button == 1) && (e.clickCount == 2);
		}
	}
}