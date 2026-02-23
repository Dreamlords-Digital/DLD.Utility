// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using DLD.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

public interface IColorPickerListener
{
	void OnColorChangeUpdated(Color32 updatedColor);
	void OnColorChangeConfirmed(Color32 confirmedColor);
	void OnColorChangeCanceled();
}

[UxmlElement]
public partial class ColorPicker : VisualElement
{
	const string TemplateResourcesPath = "DLD UIToolkit/ColorPicker";
	const int DefaultColorMapSize = 64;

	const string BigColorCapturedPointer = "dld-color-picker--big-color--dragging";
	const string ColorSwatchStyleName = "dld-color-picker--color-swatch";

	// =========================================================================================

	readonly VisualElement _colorMap;
	readonly VisualElement _colorMapCursor;
	readonly HueSlider _hueSlider;
	readonly VisualElement _colorSwatch;
	readonly TextField _colorHex;

	readonly SliderInt _redSlider;
	readonly SliderInt _greenSlider;
	readonly SliderInt _blueSlider;

	readonly Texture2D _colorMapTexture;
	readonly byte[] _colorMapPixels;

	Color32 _currentColor;
	HSBColor _currentHSBColor = new HSBColor(1, 1, 1, 1);

	IColorPickerListener _listener;

	// =========================================================================================

	public ColorPicker()
	{
		var asset = Resources.Load<VisualTreeAsset>(TemplateResourcesPath);
		asset.CloneTree(this);
		this.RemoveTemplateContainer();

		_colorMapPixels = new byte[3 * DefaultColorMapSize * DefaultColorMapSize];
		_colorMapTexture = new Texture2D(DefaultColorMapSize, DefaultColorMapSize, TextureFormat.RGB24, false);
		_colorMapTexture.wrapMode = TextureWrapMode.Clamp;

		SetColorMap(_colorMapTexture, _colorMapPixels, _currentHSBColor.ToColor());

		_colorMap = this.Q<VisualElement>(className: "dld-color-picker--big-color");
		_colorMap.style.backgroundImage = _colorMapTexture;

		_colorMap.RegisterCallback<PointerDownEvent, ColorPicker>((e, c) => c.OnColorMapDown(e), this);
		_colorMap.RegisterCallback<PointerMoveEvent, ColorPicker>((e, c) => c.OnColorMapDrag(e), this);
		_colorMap.RegisterCallback<PointerUpEvent, ColorPicker>((e, c) => c.OnColorMapUp(e), this);
		_colorMap.RegisterCallback<PointerCaptureOutEvent, ColorPicker>((e, c) => c.OnColorMapOut(e), this);

		_colorMapCursor = this.Q<VisualElement>(className: "dld-color-picker--color-cursor");

		_hueSlider = this.Q<HueSlider>();
		_hueSlider.style.backgroundImage = MakeVerticalHueBand();
		_hueSlider.RegisterCallback<ChangeEvent<int>, ColorPicker>((i, c) => c.OnHueSliderChange(i), this);

		_colorSwatch = this.Q<VisualElement>(className: ColorSwatchStyleName);
		_colorHex = this.Q<TextField>("HexField");

		_redSlider = this.Q<SliderInt>("SliderR");
		_greenSlider = this.Q<SliderInt>("SliderG");
		_blueSlider = this.Q<SliderInt>("SliderB");

		_redSlider.RegisterCallback<ChangeEvent<int>, ColorPicker>((i, c) => c.OnRedSliderChange(i), this);
		_greenSlider.RegisterCallback<ChangeEvent<int>, ColorPicker>((i, c) => c.OnGreenSliderChange(i), this);
		_blueSlider.RegisterCallback<ChangeEvent<int>, ColorPicker>((i, c) => c.OnBlueSliderChange(i), this);

		var okButton = this.Q<Button>("OkButton");
		var cancelButton = this.Q<Button>("CancelButton");
		okButton.RegisterCallback<ClickEvent, ColorPicker>((e, c) => c.OnOkPressed(e), this);
		cancelButton.RegisterCallback<ClickEvent, ColorPicker>((e, c) => c.OnCancelPressed(e), this);
	}

	~ColorPicker()
	{
		Object.Destroy(_colorMapTexture);
	}

	// =========================================================================================

	public void SetListener(IColorPickerListener newListener)
	{
		_listener = newListener;
	}

	public void SetColor(Color32 newColor)
	{
		_currentColor = newColor;
		UpdateFromRGB();
	}

	public void SetColorWithoutNotify(Color32 newColor)
	{
		_currentColor = newColor;
		UpdateFromRGB(sendNotify: false);
	}

	// =========================================================================================
	// UI Callbacks

	void OnColorMapDown(PointerDownEvent e)
	{
		_colorMap.CapturePointer(e.pointerId);
		_colorMap.AddToClassList(BigColorCapturedPointer);
		// e.localPosition.x = saturation
		// e.localPosition.y = brightness
		UpdateColorFromMap(e.localPosition.x, e.localPosition.y);
	}

	void OnColorMapDrag(PointerMoveEvent e)
	{
		if (_colorMap.HasPointerCapture(e.pointerId))
		{
			// e.localPosition.x = saturation
			// e.localPosition.y = brightness
			UpdateColorFromMap(e.localPosition.x, e.localPosition.y);
		}
	}

	void OnColorMapUp(PointerUpEvent e)
	{
		_colorMap.ReleasePointer(e.pointerId);
		_colorMap.RemoveFromClassList(BigColorCapturedPointer);
		UpdateColorFromMap(e.localPosition.x, e.localPosition.y);
	}

	void OnColorMapOut(PointerCaptureOutEvent e)
	{
	}

	void OnHueSliderChange(ChangeEvent<int> e)
	{
		float newHue = Mathf.Clamp01(e.newValue / 255.0f);
		_currentHSBColor.H = newHue;
		_currentColor = _currentHSBColor.ToColor();

		_redSlider.SetValueWithoutNotify(_currentColor.r);
		_greenSlider.SetValueWithoutNotify(_currentColor.g);
		_blueSlider.SetValueWithoutNotify(_currentColor.b);

		_colorSwatch.style.backgroundColor = new StyleColor(_currentColor);
		_colorHex.SetValueWithoutNotify(_currentColor.ColorToHashHex());

		SetColorMap(_colorMapTexture, _colorMapPixels, new HSBColor(newHue, 1, 1).ToColor());

		_listener?.OnColorChangeUpdated(_currentColor);
	}

	void OnRedSliderChange(ChangeEvent<int> e)
	{
		_currentColor.r = (byte)e.newValue;
		UpdateFromRGB();
	}

	void OnGreenSliderChange(ChangeEvent<int> e)
	{
		_currentColor.g = (byte)e.newValue;
		UpdateFromRGB();
	}

	void OnBlueSliderChange(ChangeEvent<int> e)
	{
		_currentColor.b = (byte)e.newValue;
		UpdateFromRGB();
	}

	void OnOkPressed(ClickEvent e)
	{
		_listener?.OnColorChangeConfirmed(_currentColor);
	}

	void OnCancelPressed(ClickEvent e)
	{
		_listener?.OnColorChangeCanceled();
	}

	// =========================================================================================

	void UpdateFromRGB(bool sendNotify = true)
	{
		_currentHSBColor = HSBColor.FromColor(_currentColor);

		float newX = Mathf.RoundToInt(_currentHSBColor.S * 255.0f);
		float newY = Mathf.RoundToInt((1-_currentHSBColor.B) * 255.0f);
		_colorMapCursor.SetPosition(newX-7, newY-7);

		int newHue = Mathf.RoundToInt(_currentHSBColor.H * 255.0f);
		_hueSlider.SetValueWithoutNotify(newHue);

		_colorSwatch.style.backgroundColor = new StyleColor(_currentColor);
		_colorHex.SetValueWithoutNotify(_currentColor.ColorToHashHex());

		SetColorMap(_colorMapTexture, _colorMapPixels, new HSBColor(_currentHSBColor.H, 1, 1).ToColor());

		if (sendNotify)
		{
			_listener?.OnColorChangeUpdated(_currentColor);
		}
	}

	void UpdateColorFromMap(float x, float y)
	{
		float newX = Mathf.RoundToInt(Mathf.Clamp(x, 0.0f, 255.0f));
		float newY = Mathf.RoundToInt(Mathf.Clamp(y, 0.0f, 255.0f));
		_colorMapCursor.SetPosition(newX-7, newY-7);

		float newSaturation = Mathf.Clamp01(x / 255.0f);
		float newBrightness = 1.0f - Mathf.Clamp01(y / 255.0f);
		_currentHSBColor.S = newSaturation;
		_currentHSBColor.B = newBrightness;
		_currentColor = _currentHSBColor.ToColor();

		_redSlider.SetValueWithoutNotify(_currentColor.r);
		_greenSlider.SetValueWithoutNotify(_currentColor.g);
		_blueSlider.SetValueWithoutNotify(_currentColor.b);

		_colorSwatch.style.backgroundColor = new StyleColor(_currentColor);
		_colorHex.SetValueWithoutNotify(_currentColor.ColorToHashHex());

		_listener?.OnColorChangeUpdated(_currentColor);
	}

	// =========================================================================================

	static void SetColorMap(Texture2D ret, byte[] colors, Color color, int size = DefaultColorMapSize)
	{
		// top: white to hue
		// left: white to black
		// right hue to black
		// bottom: black

		float sizeF = size;

		for (int y = 0; y < size; ++y)
		{
			Color32 leftSideColor = Color32.Lerp(Color.black, Color.white, y / sizeF);
			Color32 rightSideColor = Color32.Lerp(Color.black, color, y / sizeF);
			HSBColor leftSideHSB = HSBColor.FromColor(leftSideColor);
			HSBColor rightSideHSB = HSBColor.FromColor(rightSideColor);
			for (int x = 0; x < size; ++x)
			{
				int idx = 3*((y * size) + x);
				Color32 newColor = HSBColor.Lerp(leftSideHSB, rightSideHSB, x / sizeF).ToColor();
				colors[idx] = newColor.r;
				colors[idx+1] = newColor.g;
				colors[idx+2] = newColor.b;
			}
		}

		ret.SetPixelData(colors, 0);
		ret.Apply(false);
	}

	static void SetColorMap(Texture2D ret, Color32[] colors, Color color, int size = DefaultColorMapSize)
	{
		// top: white to hue
		// left: white to black
		// right hue to black
		// bottom: black

		for (int y = 0; y < size; ++y)
		{
			Color32 leftSideColor = Color32.Lerp(Color.black, Color.white, y / 255.0f);
			Color32 rightSideColor = Color32.Lerp(Color.black, color, y / 255.0f);
			for (int x = 0; x < size; ++x)
			{
				int idx = (y * size) + x;
				colors[idx] =
					HSBColor.Lerp(HSBColor.FromColor(leftSideColor), HSBColor.FromColor(rightSideColor), x / 255.0f)
						.ToColor();
			}
		}

		ret.SetPixelData(colors, 0);
		ret.Apply(false);
	}

	static Texture2D MakeVerticalHueBand(int width = 2, int height = DefaultColorMapSize, bool invertDirection = false)
	{
		Texture2D ret = new Texture2D(width, height, TextureFormat.RGB24, false);

		Color32[] colors = new Color32[width * height];
		for (int y = 0; y < height; ++y)
		{
			float hue;
			if (invertDirection)
			{
				hue = 1.0f - (float)y / height;
			}
			else
			{
				hue = (float)y / height;
			}

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
}

}
