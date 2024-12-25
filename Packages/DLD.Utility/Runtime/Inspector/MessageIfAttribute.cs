// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEngine;

namespace DLD.Utility.Inspector
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class MessageIfAttribute : PropertyAttribute
	{
		public readonly string Message;
		public string PropertyName1 { get; }
		public string PropertyName2 { get; }

		public string PropertyToCheck { get; }
		public ComparisonType ComparisonType { get; }
		public object ValueToCheckAgainst { get; }

		public MessageIfAttribute(string propertyToCheck, object valueToCheckAgainst, string message)
		{
			PropertyToCheck = propertyToCheck;
			ComparisonType = ComparisonType.Equals;
			ValueToCheckAgainst = valueToCheckAgainst;

			Message = message;
		}

		public MessageIfAttribute(string propertyToCheck, object valueToCheckAgainst, string message, string propertyName1)
		{
			PropertyToCheck = propertyToCheck;
			ComparisonType = ComparisonType.Equals;
			ValueToCheckAgainst = valueToCheckAgainst;

			Message = message;
			PropertyName1 = propertyName1;
			PropertyName2 = null;
		}

		public MessageIfAttribute(string propertyToCheck, object valueToCheckAgainst, string message, string propertyName1, string propertyName2)
		{
			PropertyToCheck = propertyToCheck;
			ComparisonType = ComparisonType.Equals;
			ValueToCheckAgainst = valueToCheckAgainst;

			Message = message;
			PropertyName1 = propertyName1;
			PropertyName2 = propertyName2;
		}
	}
}