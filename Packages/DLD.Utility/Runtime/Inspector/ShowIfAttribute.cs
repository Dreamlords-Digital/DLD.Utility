// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEngine;

namespace DLD.Utility.Inspector
{
	public enum ComparisonType
	{
		Equals,
		NotEqual,

		/// <summary>
		/// Require that only the value indicated is present in the bit flag property.
		/// Only applicable to enums/ints with bit flag values.
		/// </summary>
		FlagCheckStrict,

		/// <summary>
		/// Require that the value indicated is present in the bit flag  property, but allow other values to be present.
		/// Only applicable to enums/ints with bit flag values.
		/// </summary>
		FlagCheckLenient,

		GreaterThan,
		LesserThan,
		GreaterThanOrEqual,
		LesserThanOrEqual,
	}

	public enum HideType
	{
		ReadOnly,
		DoNotDraw
	}

	/// <summary>
	/// Show field/property if condition is true.
	/// </summary>
	/// <remarks>
	/// This will not work properly if used on an array/list.
	/// PropertyDrawers can't hide the field for editing size/count.
	/// See https://forum.unity.com/threads/propertydrawers-for-easy-inspector-customization.150337/#post-1031443
	/// The (ugly) workaround is to wrap the array/list into a serializable class/struct then use ShowIf on that instead.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class ShowIfAttribute : PropertyAttribute
	{
		public string PropertyToCheck { get; }
		public ComparisonType ComparisonType { get; }
		public object ValueToCheckAgainst { get; }
		public HideType HideType { get; }

		/// <summary>
		/// Show field/property if condition is true.
		/// </summary>
		/// <param name="propertyToCheck">The name of the property that is being compared (case sensitive).
		/// Use nameof operator to prevent spelling mistakes.</param>
		/// <param name="comparisonType">How to compare against value.</param>
		/// <param name="valueToCheckAgainst">The value that the property needs to be.</param>
		/// <param name="hideType">Whether we hide the property or just turn it read-only when the condition fails.</param>
		public ShowIfAttribute(string propertyToCheck, ComparisonType comparisonType,
			object valueToCheckAgainst, HideType hideType = HideType.DoNotDraw)
		{
			PropertyToCheck = propertyToCheck;
			ComparisonType = comparisonType;
			ValueToCheckAgainst = valueToCheckAgainst;
			HideType = hideType;
		}

		public ShowIfAttribute(string propertyToCheck, object valueToCheckAgainst, HideType hideType = HideType.DoNotDraw)
		{
			PropertyToCheck = propertyToCheck;
			ComparisonType = ComparisonType.Equals;
			ValueToCheckAgainst = valueToCheckAgainst;
			HideType = hideType;
		}
	}
}