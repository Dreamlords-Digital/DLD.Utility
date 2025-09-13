// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;

namespace DLD.UIToolkit
{

public class EnumUIAttribute : Attribute
{
	public readonly string Label;
	public readonly string ShortLabel;
	public readonly string IconStyleClass;

	public EnumUIAttribute(string label = null, string shortLabel = null, string iconStyleClass = null)
	{
		Label = label;
		ShortLabel = shortLabel;
		IconStyleClass = iconStyleClass;
	}
}

}
