using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public class EnumUIAttribute : Attribute
	{
		public readonly string Tooltip;
		public readonly string Label;
		public readonly string ShortLabel;
		public readonly string IconStyleClass;

		public EnumUIAttribute(string tooltip = null, string label = null, string shortLabel = null, string iconStyleClass = null)
		{
			Tooltip = tooltip;
			Label = label;
			ShortLabel = shortLabel;
			IconStyleClass = iconStyleClass;
		}
	}
}
