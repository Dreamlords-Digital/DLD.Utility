// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

[UxmlElement]
public partial class HueSlider : SliderInt
{
	const string ClassStyleName = "dld-color-picker--hue-slider";
	const string DraggerClassStyleName = "dld-slider__dragger-image";
	const string DraggerBorderClassStyleName = "dld-slider__dragger-border-image";

	const string DraggerElementName = "unity-dragger";
	const string DraggerBorderElementName = "unity-dragger-border";

	public HueSlider()
	{
		AddToClassList(ClassStyleName);
		lowValue = 0;
		highValue = 255;

		var draggerImage = new VisualElement();
		draggerImage.AddToClassList(DraggerClassStyleName);
		this.Q<VisualElement>(DraggerElementName)?.Add(draggerImage);

		var draggerBorderImage = new VisualElement();
		draggerBorderImage.AddToClassList(DraggerBorderClassStyleName);
		this.Q<VisualElement>(DraggerBorderElementName)?.Add(draggerBorderImage);
	}
}

}
