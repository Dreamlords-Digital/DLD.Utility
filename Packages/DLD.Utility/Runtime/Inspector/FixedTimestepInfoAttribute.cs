// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;

namespace DLD.Utility.Inspector
{
	public class FixedTimestepInfoAttribute : PropertyAttribute
	{
		public readonly string Message;
		public string PropertyToCheck { get; }
		public ComparisonType ComparisonType { get; }
		public object ValueToCheckAgainst { get; }

		public FixedTimestepInfoAttribute(string message) => Message = message;

		public FixedTimestepInfoAttribute(string propertyToCheck,
			object valueToCheckAgainst, string message)
		{
			PropertyToCheck = propertyToCheck;
			ComparisonType = ComparisonType.Equals;
			ValueToCheckAgainst = valueToCheckAgainst;
			Message = message;
		}

		public FixedTimestepInfoAttribute(string propertyToCheck, ComparisonType comparisonType,
			object valueToCheckAgainst, string message)
		{
			PropertyToCheck = propertyToCheck;
			ComparisonType = comparisonType;
			ValueToCheckAgainst = valueToCheckAgainst;
			Message = message;
		}
	}
}