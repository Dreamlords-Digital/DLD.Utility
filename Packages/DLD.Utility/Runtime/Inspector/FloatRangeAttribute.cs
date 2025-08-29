// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEngine;

namespace DLD.Utility.Inspector
{
	/// <summary>
	/// Show a FloatRange in a compact format (with two input fields in one line),
	/// and optionally with custom labels for the input fields.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class FloatRangeAttribute : PropertyAttribute
	{
		/// <summary>
		/// Custom label on the FloatRange. Can be left at null to use the default (just use the variable name).
		/// </summary>
		public readonly string Label;

		/// <summary>
		/// Optional label just before the Range Start's input field.
		/// </summary>
		public readonly string LowerLimitLabel;

		/// <summary>
		/// Optional label to the right of the Range Start's input field.
		/// </summary>
		public readonly string LowerLimitPostLabel;

		/// <summary>
		/// Optional label just before the Range End's input field.
		/// </summary>
		public readonly string UpperLimitLabel;

		/// <summary>
		/// Optional label to the right of the Range End's input field.
		/// </summary>
		public readonly string UpperLimitPostLabel;

		/// <summary>
		/// Make the control use all the available width, instead of lining up with other controls.
		/// </summary>
		public readonly bool UseAllAvailableSpace;

		public readonly float LowerLimitMin;
		public readonly float LowerLimitMax;

		public readonly float UpperLimitMin;
		public readonly float UpperLimitMax;

		public FloatRangeAttribute()
		{
			Label = null;
			LowerLimitLabel = null;
			LowerLimitPostLabel = null;
			UpperLimitLabel = null;
			UpperLimitPostLabel = null;

			UseAllAvailableSpace = false;

			LowerLimitMin = float.MinValue;
			LowerLimitMax = float.MaxValue;

			UpperLimitMin = float.MinValue;
			UpperLimitMax = float.MaxValue;
		}

		public FloatRangeAttribute(string lowerLimitLabel, string upperLimitLabel, bool useAllAvailableSpace = false,
			float lowerLimitMin = float.MinValue, float lowerLimitMax = float.MaxValue, float upperLimitMin = float.MinValue, float upperLimitMax = float.MaxValue)
		{
			Label = null;
			LowerLimitLabel = lowerLimitLabel;
			LowerLimitPostLabel = null;
			UpperLimitLabel = upperLimitLabel;
			UpperLimitPostLabel = null;

			UseAllAvailableSpace = useAllAvailableSpace;

			LowerLimitMin = lowerLimitMin;
			LowerLimitMax = lowerLimitMax;

			UpperLimitMin = upperLimitMin;
			UpperLimitMax = upperLimitMax;
		}

		public FloatRangeAttribute(string label = null, string lowerLimitLabel = null, string lowerLimitPostLabel = null, string upperLimitLabel = null, string upperLimitPostLabel = null, bool useAllAvailableSpace = false,
			float lowerLimitMin = float.MinValue, float lowerLimitMax = float.MaxValue, float upperLimitMin = float.MinValue, float upperLimitMax = float.MaxValue)
		{
			Label = label;
			LowerLimitLabel = lowerLimitLabel;
			LowerLimitPostLabel = lowerLimitPostLabel;
			UpperLimitLabel = upperLimitLabel;
			UpperLimitPostLabel = upperLimitPostLabel;

			UseAllAvailableSpace = useAllAvailableSpace;

			LowerLimitMin = lowerLimitMin;
			LowerLimitMax = lowerLimitMax;

			UpperLimitMin = upperLimitMin;
			UpperLimitMax = upperLimitMax;
		}
	}
}
