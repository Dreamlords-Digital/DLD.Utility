// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEngine;

namespace DLD.Utility.Inspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class RangeFloatShowIfAttribute : PropertyAttribute
	{
		public string PropertyToCheck { get; }
		public ComparisonType ComparisonType { get; }
		public object ValueToCheckAgainst { get; }
		public HideType HideType { get; }

		public readonly string Label;

		public readonly string StartLabel;
		public readonly string StartPostLabel;
		public readonly string EndLabel;
		public readonly string EndPostLabel;

		public readonly bool UseAllAvailableSpace;

		public readonly float StartMin;
		public readonly float StartMax;

		public readonly float EndMin;
		public readonly float EndMax;

		public RangeFloatShowIfAttribute()
		{
			PropertyToCheck = null;
			ComparisonType = ComparisonType.Equals;
			ValueToCheckAgainst = null;
			HideType = HideType.DoNotDraw;

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

		public RangeFloatShowIfAttribute(string propertyToCheck,
			object valueToCheckAgainst,
			string startLabel, string endLabel, bool useAllAvailableSpace = false,
			float startMin = float.MinValue, float startMax = float.MaxValue, float endMin = float.MinValue, float endMax = float.MaxValue)
		{
			PropertyToCheck = propertyToCheck;
			ComparisonType = ComparisonType.Equals;
			ValueToCheckAgainst = valueToCheckAgainst;
			HideType = HideType.DoNotDraw;

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

		public RangeFloatShowIfAttribute(string propertyToCheck, ComparisonType comparisonType,
			object valueToCheckAgainst, HideType hideType,
			string startLabel, string endLabel, bool useAllAvailableSpace = false,
			float startMin = float.MinValue, float startMax = float.MaxValue, float endMin = float.MinValue, float endMax = float.MaxValue)
		{
			PropertyToCheck = propertyToCheck;
			ComparisonType = comparisonType;
			ValueToCheckAgainst = valueToCheckAgainst;
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

		public RangeFloatShowIfAttribute(string propertyToCheck, ComparisonType comparisonType,
			object valueToCheckAgainst, HideType hideType,
			string label, string startLabel, string startPostLabel, string endLabel, string endPostLabel, bool useAllAvailableSpace,
			float startMin = float.MinValue, float startMax = float.MaxValue, float endMin = float.MinValue, float endMax = float.MaxValue)
		{
			PropertyToCheck = propertyToCheck;
			ComparisonType = comparisonType;
			ValueToCheckAgainst = valueToCheckAgainst;
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