// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace DLD.Serializer.Tests
{

public abstract partial class BaseTextDataTests
{
	[Test(Description = "Ensure that fields set to be serialized really do get serialized, and fields that are marked to not be serialized really don't.")]
	public void FromSerializedString_OnStringReturnedByToSerializedString_IsCorrectData()
	{
		// -----------------------------------------------

		var simple1 = new SimpleClass();

		Assert.AreEqual(SimpleClass.SerializedPublicStringDefault, simple1.SerializedPublicString);
		Assert.AreEqual(SimpleClass.SerializedPrivateStringDefault, simple1.SerializedPrivateString);

		Assert.AreEqual(SimpleClass.NonSerializedPublicStringDefault, simple1.NonSerializedPublicString);
		Assert.AreEqual(SimpleClass.NonSerializedPrivateStringDefault, simple1.NonSerializedPrivateString);

		const string NSR_PRV_STR_NEW_1 = "This should not get saved";
		simple1.SetNonSerializedPrivateString(NSR_PRV_STR_NEW_1);

		const string NSR_PUB_STR_NEW_1 = "This should also not be saved";
		simple1.NonSerializedPublicString = NSR_PUB_STR_NEW_1;

		const int SER_PUB_INT_NEW_1 = 90;
		simple1.SerializedPublicInt = SER_PUB_INT_NEW_1;

		const string SER_PRV_STR_NEW_1 = "This should get saved";
		simple1.SetSerializedPrivateString(SER_PRV_STR_NEW_1);

		const string SER_PUB_STR_NEW_1 = "This should also be saved";
		simple1.SerializedPublicString = SER_PUB_STR_NEW_1;

		// -----------------------------------------------

		Assert.AreNotEqual(SimpleClass.SerializedPublicStringDefault, SER_PUB_STR_NEW_1);
		Assert.AreNotEqual(SimpleClass.SerializedPrivateStringDefault, SER_PRV_STR_NEW_1);

		Assert.AreNotEqual(SimpleClass.NonSerializedPublicStringDefault, NSR_PUB_STR_NEW_1);
		Assert.AreNotEqual(SimpleClass.NonSerializedPrivateStringDefault, NSR_PRV_STR_NEW_1);

		// -----------------------------------------------

		string simple1Text = _textDataIO.ToSerializedString(simple1);

		Debug.Log($"Serialized:\n{simple1Text}");

		var deserializedSimple1 = _textDataIO.FromSerializedString<SimpleClass>(simple1Text);

		// -----------------------------------------------

		Assert.AreNotSame(simple1, deserializedSimple1);

		Assert.AreEqual(SER_PUB_STR_NEW_1, simple1.SerializedPublicString);
		Assert.AreEqual(SER_PUB_STR_NEW_1, deserializedSimple1.SerializedPublicString);
		Assert.AreEqual(simple1.SerializedPublicString, deserializedSimple1.SerializedPublicString);

		Assert.AreEqual(SER_PRV_STR_NEW_1, simple1.SerializedPrivateString);
		Assert.AreEqual(SER_PRV_STR_NEW_1, deserializedSimple1.SerializedPrivateString);
		Assert.AreEqual(simple1.SerializedPrivateString, deserializedSimple1.SerializedPrivateString);

		Assert.AreEqual(SER_PUB_INT_NEW_1, simple1.SerializedPublicInt);
		Assert.AreEqual(SER_PUB_INT_NEW_1, deserializedSimple1.SerializedPublicInt);
		Assert.AreEqual(simple1.SerializedPublicInt, deserializedSimple1.SerializedPublicInt);

		Assert.AreEqual(NSR_PUB_STR_NEW_1, simple1.NonSerializedPublicString);
		Assert.AreEqual(SimpleClass.NonSerializedPublicStringDefault, deserializedSimple1.NonSerializedPublicString);
		Assert.AreNotEqual(simple1.NonSerializedPublicString, deserializedSimple1.NonSerializedPublicString);

		Assert.AreEqual(NSR_PRV_STR_NEW_1, simple1.NonSerializedPrivateString);
		Assert.AreEqual(SimpleClass.NonSerializedPrivateStringDefault, deserializedSimple1.NonSerializedPrivateString);
		Assert.AreNotEqual(simple1.NonSerializedPrivateString, deserializedSimple1.NonSerializedPrivateString);

		// -----------------------------------------------

		SimpleClass deserializedFromFileSimple1;
		string simple1SavePath;
		DoLoadFromLocal("Simple1.txt", simple1, out deserializedFromFileSimple1, out simple1SavePath);

		// -----------------------------------------------

		Assert.AreNotSame(deserializedFromFileSimple1, deserializedSimple1);
		Assert.AreNotSame(deserializedFromFileSimple1, simple1);

		Assert.AreEqual(SER_PUB_STR_NEW_1, simple1.SerializedPublicString);
		Assert.AreEqual(SER_PUB_STR_NEW_1, deserializedFromFileSimple1.SerializedPublicString);
		Assert.AreEqual(simple1.SerializedPublicString, deserializedFromFileSimple1.SerializedPublicString);

		Assert.AreEqual(SER_PRV_STR_NEW_1, simple1.SerializedPrivateString);
		Assert.AreEqual(SER_PRV_STR_NEW_1, deserializedFromFileSimple1.SerializedPrivateString);
		Assert.AreEqual(simple1.SerializedPrivateString, deserializedFromFileSimple1.SerializedPrivateString);

		Assert.AreEqual(SER_PUB_INT_NEW_1, simple1.SerializedPublicInt);
		Assert.AreEqual(SER_PUB_INT_NEW_1, deserializedFromFileSimple1.SerializedPublicInt);
		Assert.AreEqual(simple1.SerializedPublicInt, deserializedFromFileSimple1.SerializedPublicInt);

		Assert.AreEqual(NSR_PUB_STR_NEW_1, simple1.NonSerializedPublicString);
		Assert.AreEqual(SimpleClass.NonSerializedPublicStringDefault, deserializedFromFileSimple1.NonSerializedPublicString);
		Assert.AreNotEqual(simple1.NonSerializedPublicString, deserializedFromFileSimple1.NonSerializedPublicString);

		Assert.AreEqual(NSR_PRV_STR_NEW_1, simple1.NonSerializedPrivateString);
		Assert.AreEqual(SimpleClass.NonSerializedPrivateStringDefault, deserializedFromFileSimple1.NonSerializedPrivateString);
		Assert.AreNotEqual(simple1.NonSerializedPrivateString, deserializedFromFileSimple1.NonSerializedPrivateString);

		// -----------------------------------------------

		// done with our temporary serialized class, delete its file so it doesn't waste space
		File.Delete(simple1SavePath);
	}
}

}
