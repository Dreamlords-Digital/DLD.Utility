// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEngine;

namespace DLD.Utility.Inspector
{
	/// <summary>
	/// Show a RangeFloat in a compact format (with two input fields in one line), but only if the specified property returns true.
	/// </summary>
	/// <remarks>
	/// <para>This is referring to a C# property, not a <see cref="UnityEditor.SerializedProperty"/>.</para>
	/// <para>This assumes the property returns a boolean. The property does not have to be public.</para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class RangeFloatShowPropertyAttribute : PropertyAttribute
	{
		/// <summary>
		/// Name of property to use for determining if the RangeFloat will be shown.
		/// </summary>
		/// <remarks>
		/// <para>This is referring to a C# property, not a <see cref="UnityEditor.SerializedProperty"/>.</para>
		/// <para>This assumes the property returns a boolean. The property does not have to be public.</para>
		/// </remarks>
		public readonly string PropertyName;

		/// <summary>
		/// Whether to completely hide the RangeFloat, or still show it but with read-only disabled controls.
		/// </summary>
		public HideType HideType { get; } = HideType.DoNotDraw;

		/// <summary>
		/// Custom label on the RangeFloat. Can be left at null to use the default (just use the variable name).
		/// </summary>
		public readonly string Label;

		/// <summary>
		/// Optional label just before the Range Start's input field.
		/// </summary>
		public readonly string StartLabel;

		/// <summary>
		/// Optional label to the right of the Range Start's input field.
		/// </summary>
		public readonly string StartPostLabel;

		/// <summary>
		/// Optional label just before the Range End's input field.
		/// </summary>
		public readonly string EndLabel;

		/// <summary>
		/// Optional label to the right of the Range End's input field.
		/// </summary>
		public readonly string EndPostLabel;

		/// <summary>
		/// Make the control use all the available width, instead of lining up with other controls.
		/// </summary>
		public readonly bool UseAllAvailableSpace;

		public readonly float StartMin;
		public readonly float StartMax;

		public readonly float EndMin;
		public readonly float EndMax;

		public RangeFloatShowPropertyAttribute()
		{
			PropertyName = null;

			Label = null;
			StartLabel = null;
			StartPostLabel = null;
			EndLabel = null;
			EndPostLabel = null;

			UseAllAvailableSpace = false;

			StartMin = float.MinValue;
			StartMax = float.MaxValue;

			EndMin = float.MinValue;
			EndMax = float.MaxValue;
		}

		public RangeFloatShowPropertyAttribute(string propertyName, HideType hideType,
			string startLabel, string endLabel, bool useAllAvailableSpace = false,
			float startMin = float.MinValue, float startMax = float.MaxValue, float endMin = float.MinValue, float endMax = float.MaxValue)
		{
			PropertyName = propertyName;
			HideType = hideType;

			Label = null;
			StartLabel = startLabel;
			StartPostLabel = null;
			EndLabel = endLabel;
			EndPostLabel = null;

			UseAllAvailableSpace = useAllAvailableSpace;

			StartMin = startMin;
			StartMax = startMax;

			EndMin = endMin;
			EndMax = endMax;
		}

		public RangeFloatShowPropertyAttribute(string propertyName, HideType hideType,
			string label, string startLabel, string startPostLabel, string endLabel, string endPostLabel, bool useAllAvailableSpace,
			float startMin = float.MinValue, float startMax = float.MaxValue, float endMin = float.MinValue, float endMax = float.MaxValue)
		{
			PropertyName = propertyName;
			HideType = hideType;

			Label = label;
			StartLabel = startLabel;
			StartPostLabel = startPostLabel;
			EndLabel = endLabel;
			EndPostLabel = endPostLabel;

			UseAllAvailableSpace = useAllAvailableSpace;

			StartMin = startMin;
			StartMax = startMax;

			EndMin = endMin;
			EndMax = endMax;
		}
	}
}