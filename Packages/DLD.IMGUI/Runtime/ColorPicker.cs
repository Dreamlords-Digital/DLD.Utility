// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEngine;
using DLD.Utility;

namespace DLD.IMGUI
{
	public delegate void OnUserChoseColor(Color chosenColor);

	public delegate void OnUserCancelColor();

	public static class ColorPicker
	{
		const int COLOR_MAP_SIZE = 256;
		const int SLIDER_TOP_MARGIN = 5;
		const int SWATCH_TOP_MARGIN = SLIDER_TOP_MARGIN + 5;
		const int SLIDER_RIGHT_MARGIN = 5;
		const int SLIDER_LEFT = 14;
		const int SLIDER_SPACING = 10;
		const int SLIDER_HEIGHT = 10;
		const int SLIDER_WIDTH = 157;
		const int TEXT_ENTRY_WIDTH = 40;
		const int TEXT_ENTRY_HEIGHT = 17;
		const int SWATCH_SIZE = 60;
		const int SWATCH_HEIGHT = SWATCH_SIZE - 23;
		const int SLIDER_LABEL_TOP_ADJUST = 0;
		const int SLIDER_TEXT_TOP_ADJUST = -3;

		static Texture2D _colorMap;
		static Texture2D _hueBand;
		static Color32[] _colorMapPixels = new Color32[COLOR_MAP_SIZE * COLOR_MAP_SIZE];
		static float _hue;
		static float _sat;
		static float _brt;
		static float _lastHue;
		static float _lastSat;
		static float _lastBrt;

		static Vector2 _crosshairPos;
		static Texture2D _crosshair;
		static Vector2 _crosshairHotspot;

		static Texture2D _hueLevelIndicator;
		static Vector2 _hueLevelIndicatorHotspot;

		static bool _draggingColorMap = false;
		static bool _draggingHueBand = false;

		static bool _initialized = false;

		const string CROSSHAIR_IMAGE_DEFAULT_STYLE_NAME = "ColorPicker.Icon.ColorCrosshair";
		const string HUE_LEVEL_IMAGE_DEFAULT_STYLE_NAME = "ColorPicker.Icon.HueLevel";
		const string COLOR_SWATCH_DEFAULT_STYLE_NAME = "ColorPicker.Swatch";

		static void Init()
		{
			_colorMap = new Texture2D(COLOR_MAP_SIZE, COLOR_MAP_SIZE, TextureFormat.RGB24, false);
			MakeColorMap(_colorMap, _colorMapPixels, new HSBColor(_hue, 1, 1).ToColor());
			_hueBand = MakeVerticalHueBand(20, COLOR_MAP_SIZE);

			GUIStyle crosshairStyle = GUI.skin.GetStyle(CROSSHAIR_IMAGE_DEFAULT_STYLE_NAME);
			_crosshair = crosshairStyle.normal.background;
			_crosshairHotspot = crosshairStyle.contentOffset;

			var hueLevelIndicatorStyle = GUI.skin.GetStyle(HUE_LEVEL_IMAGE_DEFAULT_STYLE_NAME);
			_hueLevelIndicator = hueLevelIndicatorStyle.normal.background;
			_hueLevelIndicatorHotspot = hueLevelIndicatorStyle.contentOffset;

			_initialized = true;
		}

		static void InitIfNeeded()
		{
			if (!_initialized)
			{
				Init();
			}
		}

		static void MakeColorMap(Texture2D ret, Color32[] colors, Color color)
		{
			// top: white to hue
			// left: white to black
			// right hue to black
			// bottom: black

			for (int y = 0; y < COLOR_MAP_SIZE; ++y)
			{
				Color32 leftSideColor = Color32.Lerp(Color.black, Color.white, y / 255.0f);
				Color32 rightSideColor = Color32.Lerp(Color.black, color, y / 255.0f);
				for (int x = 0; x < COLOR_MAP_SIZE; ++x)
				{
					int idx = (y * COLOR_MAP_SIZE) + x;
					colors[idx] =
						HSBColor.Lerp(HSBColor.FromColor(leftSideColor), HSBColor.FromColor(rightSideColor), x / 255.0f)
							.ToColor();
				}
			}

			ret.SetPixels32(colors);
			ret.Apply(false);
		}

		static Texture2D MakeVerticalHueBand(int width, int height)
		{
			Texture2D ret = new Texture2D(width, height, TextureFormat.RGB24, false);

			Color32[] colors = new Color32[width * height];
			for (int y = 0; y < height; ++y)
			{
				float hue = 1.0f - (float)y / height;
				Color hueColor = new HSBColor(hue, 1, 1).ToColor();

				for (int x = 0; x < width; ++x)
				{
					int idx = (y * width) + x;
					colors[idx] = hueColor;
				}
			}

			ret.SetPixels32(colors);
			ret.Apply(false);

			return ret;
		}

		public static bool IsOpen { get; set; }

		static OnUserChoseColor _chooseColorCallback;
		static OnUserCancelColor _cancelColorCallback;

		public static void Open(OnUserChoseColor okCallback, OnUserCancelColor cancelCallback)
		{
			_chooseColorCallback = okCallback;
			_cancelColorCallback = cancelCallback;
			IsOpen = true;
			//Debug.Log("color picker open");
		}

		public static void ConfirmAndClose()
		{
			_chooseColorCallback?.Invoke(GetColor());
			IsOpen = false;
		}

		public static void CancelAndClose()
		{
			_cancelColorCallback?.Invoke();
			IsOpen = false;
		}

		public static void SetColor(Color c)
		{
			HSBColor hsb = HSBColor.FromColor(c);
			_hue = hsb.h;
			_sat = hsb.s;
			_brt = hsb.b;
		}

		public static Color GetColor()
		{
			HSBColor theColor;
			theColor.h = _hue;
			theColor.s = _sat;
			theColor.b = _brt;
			theColor.a = 1;

			return theColor.ToColor();
		}

		public static Color32 GetColor32()
		{
			HSBColor theColor;
			theColor.h = _hue;
			theColor.s = _sat;
			theColor.b = _brt;
			theColor.a = 1;

			return theColor.ToColor();
		}

		public static bool DrawControls(Vector2 pos)
		{
			InitIfNeeded();

			Rect tempRect = Rect.MinMaxRect(0, 0, 0, 0);
			tempRect.Set(pos.x, pos.y, Mathf.Infinity, Mathf.Infinity);
			GUI.BeginGroup(tempRect);

			tempRect.Set(0, 0, COLOR_MAP_SIZE, COLOR_MAP_SIZE);
			GUI.DrawTexture(tempRect, _colorMap);

			tempRect.Set(COLOR_MAP_SIZE + SLIDER_RIGHT_MARGIN, 0, _hueBand.width, _hueBand.height);
			GUI.DrawTexture(tempRect, _hueBand);


			float labelsY = COLOR_MAP_SIZE + SLIDER_TOP_MARGIN + SLIDER_LABEL_TOP_ADJUST;

			float secondRow = (SLIDER_HEIGHT + SLIDER_SPACING);
			float thirdRow = (2 * (SLIDER_HEIGHT + SLIDER_SPACING));

			tempRect.Set(0, labelsY, 30, 30);
			GUI.Label(tempRect, "H");

			tempRect.Set(0, labelsY + secondRow, 30, 30);
			GUI.Label(tempRect, "S");

			tempRect.Set(0, labelsY + thirdRow, 30, 30);
			GUI.Label(tempRect, "B");

			float slidersY = COLOR_MAP_SIZE + SLIDER_TOP_MARGIN + SLIDER_SPACING;

			tempRect.Set(SLIDER_LEFT, slidersY, SLIDER_WIDTH, SLIDER_HEIGHT);
			_hue = GUI.HorizontalSlider(tempRect, _hue, 0.0f, 1.0f);

			tempRect.Set(SLIDER_LEFT, slidersY + secondRow, SLIDER_WIDTH, SLIDER_HEIGHT);
			_sat = GUI.HorizontalSlider(tempRect, _sat, 0.0f, 1.0f);

			tempRect.Set(SLIDER_LEFT, slidersY + thirdRow, SLIDER_WIDTH, SLIDER_HEIGHT);
			_brt = GUI.HorizontalSlider(tempRect, _brt, 0.0f, 1.0f);

			int hueInt = (int)Mathf.Round(_hue * 255);
			int satInt = (int)Mathf.Round(_sat * 255);
			int brtInt = (int)Mathf.Round(_brt * 255);


			// draw crosshair

			tempRect.Set(0, 0, COLOR_MAP_SIZE, COLOR_MAP_SIZE);
			GUI.BeginGroup(tempRect);

			tempRect.Set(satInt + _crosshairHotspot.x, 255 - brtInt + _crosshairHotspot.y, _crosshair.width,
				_crosshair.height);
			GUI.DrawTexture(tempRect, _crosshair);
			GUI.EndGroup();


			// draw hue level indicator

			tempRect.Set(
				COLOR_MAP_SIZE + SLIDER_RIGHT_MARGIN + _hueBand.width + SLIDER_RIGHT_MARGIN + _hueLevelIndicatorHotspot.x,
				hueInt + _hueLevelIndicatorHotspot.y, _hueLevelIndicator.width, _hueLevelIndicator.height);
			GUI.DrawTexture(tempRect, _hueLevelIndicator);


			float textX = SLIDER_LEFT + SLIDER_WIDTH + SLIDER_RIGHT_MARGIN;
			float textY = COLOR_MAP_SIZE + SLIDER_TOP_MARGIN + SLIDER_SPACING + SLIDER_TEXT_TOP_ADJUST;

			tempRect.Set(textX, textY, TEXT_ENTRY_WIDTH, TEXT_ENTRY_HEIGHT);
			hueInt = int.Parse(GUI.TextField(tempRect, hueInt.ToString()));

			tempRect.Set(textX, textY + secondRow, TEXT_ENTRY_WIDTH, TEXT_ENTRY_HEIGHT);
			satInt = int.Parse(GUI.TextField(tempRect, satInt.ToString()));

			tempRect.Set(textX, textY + thirdRow, TEXT_ENTRY_WIDTH, TEXT_ENTRY_HEIGHT);
			brtInt = int.Parse(GUI.TextField(tempRect, brtInt.ToString()));


			_hue = hueInt / 255.0f;
			_sat = satInt / 255.0f;
			_brt = brtInt / 255.0f;

			HSBColor producedHSB;
			producedHSB.h = _hue;
			producedHSB.s = _sat;
			producedHSB.b = _brt;
			producedHSB.a = 1;

			Color producedColor = producedHSB.ToColor();

			GUI.backgroundColor = producedColor;
			tempRect.Set(SLIDER_LEFT + SLIDER_WIDTH + SLIDER_RIGHT_MARGIN + TEXT_ENTRY_WIDTH + SLIDER_RIGHT_MARGIN,
				COLOR_MAP_SIZE + SWATCH_TOP_MARGIN,
				SWATCH_SIZE,
				SWATCH_HEIGHT);
			GUI.Box(tempRect, "", COLOR_SWATCH_DEFAULT_STYLE_NAME);
			GUI.backgroundColor = Color.white;

			tempRect.Set(SLIDER_LEFT + SLIDER_WIDTH + SLIDER_RIGHT_MARGIN + TEXT_ENTRY_WIDTH + SLIDER_RIGHT_MARGIN,
				COLOR_MAP_SIZE + SWATCH_TOP_MARGIN + SWATCH_HEIGHT + SLIDER_RIGHT_MARGIN,
				SWATCH_SIZE,
				TEXT_ENTRY_HEIGHT);
			GUI.TextField(tempRect, ColorUtil.ColorToHex(producedColor));

			const int BUTTON_WIDTH = 100;
			const int BUTTON_HEIGHT = 30;

			float buttonY = COLOR_MAP_SIZE +
			                SWATCH_TOP_MARGIN +
			                SWATCH_HEIGHT +
			                SLIDER_RIGHT_MARGIN +
			                TEXT_ENTRY_HEIGHT +
			                SWATCH_TOP_MARGIN;
			float buttonRight = COLOR_MAP_SIZE + SLIDER_RIGHT_MARGIN + _hueBand.width;

			tempRect.Set(buttonRight - BUTTON_WIDTH, buttonY, BUTTON_WIDTH, BUTTON_HEIGHT);
			if (GUI.Button(tempRect, "Cancel"))
			{
				IsOpen = false;
				if (_cancelColorCallback != null)
				{
					_cancelColorCallback();
				}
			}

			tempRect.Set(buttonRight - BUTTON_WIDTH - SLIDER_RIGHT_MARGIN - BUTTON_WIDTH, buttonY, BUTTON_WIDTH,
				BUTTON_HEIGHT);
			if (GUI.Button(tempRect, "OK"))
			{
				IsOpen = false;
				if (_chooseColorCallback != null)
				{
					_chooseColorCallback(producedColor);
				}
			}

			GUI.EndGroup();


			bool retVal = false;

			Event e = Event.current;
			if (e.isMouse && (e.type == EventType.MouseDrag || e.type == EventType.MouseDown))
			{
				Vector2 mPos = e.mousePosition;


				if (!_draggingHueBand &&
				    ((mPos.x > pos.x &&
				      mPos.x <= pos.x + COLOR_MAP_SIZE &&
				      mPos.y > pos.y &&
				      mPos.y <= pos.y + COLOR_MAP_SIZE) ||
				     _draggingColorMap))
				{
					_sat = (mPos.x - pos.x - 1) / 255.0f;
					_brt = 1 - ((mPos.y - pos.y - 1) / 255.0f);

					_sat = Mathf.Clamp01(_sat);
					_brt = Mathf.Clamp01(_brt);

					_draggingColorMap = true;
					_draggingHueBand = false;

					retVal = true;
					//Event.current.Use();
				}

				float hueLeft = pos.x + COLOR_MAP_SIZE + SLIDER_RIGHT_MARGIN;
				float hueRight = hueLeft + _hueBand.width;
				float hueTop = pos.y;
				float hueBtm = hueTop + _hueBand.height;
				if (!_draggingColorMap &&
				    ((mPos.x >= hueLeft && mPos.x <= hueRight && mPos.y > hueTop && mPos.y <= hueBtm) || _draggingHueBand))
				{
					_hue = (mPos.y - hueTop - 1) / 255.0f;

					_hue = Mathf.Clamp01(_hue);

					_draggingColorMap = false;
					_draggingHueBand = true;

					retVal = true;
					//Event.current.Use();
				}
			}
			else if (e.isMouse && (e.type == EventType.MouseUp))
			{
				_draggingColorMap = false;
				_draggingHueBand = false;
			}

			const float CHANGE_THRESHOLD = 0.001f;

			if (Math.Abs(_lastHue - _hue) > CHANGE_THRESHOLD)
			{
				HSBColor newHue;
				newHue.h = _hue;
				newHue.s = 1;
				newHue.b = 1;
				newHue.a = 1;

				MakeColorMap(_colorMap, _colorMapPixels, newHue.ToColor());
				_lastHue = _hue;
				retVal = true;
			}

			if (Math.Abs(_lastSat - _sat) > CHANGE_THRESHOLD)
			{
				_lastSat = _sat;
				retVal = true;
			}

			if (Math.Abs(_lastBrt - _brt) > CHANGE_THRESHOLD)
			{
				_lastBrt = _brt;
				retVal = true;
			}

			return retVal;
		}
	}
}