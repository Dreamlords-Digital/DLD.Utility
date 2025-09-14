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
		get
		{
			return _serializedPrivateString;
		}
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
		get
		{
			return _nonSerializedPrivateString;
		}
	}

	public void SetNonSerializedPrivateString(string newValue)
	{
		_nonSerializedPrivateString = newValue;
	}

	// -----------------------------

	[NotSerialized]
	public string NonSerializedPublicString;

	// -----------------------------

	public const string SerializedPrivateStringDefault = "Serialized Private String Default Value";
	public const string SerializedPublicStringDefault = "Serialized Public String Default Value";

	public const string NonSerializedPrivateStringDefault = "NonSerialized Private String Default Value";
	public const string NonSerializedPublicStringDefault = "NonSerialized Public String Default Value";

	public const int SerializedPublicIntDefault = -123;

	public SimpleClass()
	{
		_serializedPrivateString = SerializedPrivateStringDefault;
		SerializedPublicString = SerializedPublicStringDefault;

		_nonSerializedPrivateString = NonSerializedPrivateStringDefault;
		NonSerializedPublicString = NonSerializedPublicStringDefault;

		SerializedPublicInt = SerializedPublicIntDefault;
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
