// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;

namespace DLD.Serializer.Tests
{
	[Serializable]
	public class SimpleClass : ITextData
	{
		// -----------------------------

		[Serialized("TestString")]
		string _serializedPrivateString;

		public string SerializedPrivateString
		{
			get { return _serializedPrivateString; }
		}

		public void SetSerializedPrivateString(string newValue)
		{
			_serializedPrivateString = newValue;
		}

		// -----------------------------

		public string SerializedPublicString;

		// -----------------------------

		public int SerializedPublicInt;

		// -----------------------------

		[NotSerialized]
		string _nonSerializedPrivateString;

		public string NonSerializedPrivateString
		{
			get { return _nonSerializedPrivateString; }
		}

		public void SetNonSerializedPrivateString(string newValue)
		{
			_nonSerializedPrivateString = newValue;
		}

		// -----------------------------

		[NotSerialized]
		public string NonSerializedPublicString;

		// -----------------------------

		public const string SER_PRV_STR_DEFAULT = "Serialized Private String Default Value";
		public const string SER_PUB_STR_DEFAULT = "Serialized Public String Default Value";

		public const string NSR_PRV_STR_DEFAULT = "NonSerialized Private String Default Value";
		public const string NSR_PUB_STR_DEFAULT = "NonSerialized Public String Default Value";

		public const int SER_PUB_INT_DEFAULT = -123;

		public SimpleClass()
		{
			_serializedPrivateString = SER_PRV_STR_DEFAULT;
			SerializedPublicString = SER_PUB_STR_DEFAULT;

			_nonSerializedPrivateString = NSR_PRV_STR_DEFAULT;
			NonSerializedPublicString = NSR_PUB_STR_DEFAULT;

			SerializedPublicInt = SER_PUB_INT_DEFAULT;
		}

		// -----------------------------

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}
}