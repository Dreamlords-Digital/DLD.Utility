// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;

namespace DLD.Utility.Inspector
{
	public class MessageAttribute : PropertyAttribute
	{
		public readonly string Message;
		public string PropertyName1 { get; }
		public string PropertyName2 { get; }

		public MessageAttribute(string message) => Message = message;

		public MessageAttribute(string message, string propertyName1)
		{
			Message = message;
			PropertyName1 = propertyName1;
			PropertyName2 = null;
		}

		public MessageAttribute(string message, string propertyName1, string propertyName2)
		{
			Message = message;
			PropertyName1 = propertyName1;
			PropertyName2 = propertyName2;
		}
	}
}