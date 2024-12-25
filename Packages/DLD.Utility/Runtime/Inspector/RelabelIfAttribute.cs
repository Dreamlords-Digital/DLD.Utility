// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEngine;

namespace DLD.Utility.Inspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class RelabelIfAttribute : PropertyAttribute
	{
		public string PropertyToCheck { get; }
		public ComparisonType ComparisonType { get; }
		public object ValueToCheckAgainst { get; }

		public string NewLabel { get; }

		public RelabelIfAttribute(string propertyToCheck, ComparisonType comparisonType,
			object valueToCheckAgainst, string newLabel)
		{
			PropertyToCheck = propertyToCheck;
			ComparisonType = comparisonType;
			ValueToCheckAgainst = valueToCheckAgainst;
			NewLabel = newLabel;
		}
	}
}