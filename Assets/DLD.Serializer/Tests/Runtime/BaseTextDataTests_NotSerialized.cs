// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using NUnit.Framework;
using UnityEngine;

namespace DLD.Serializer.Tests
{
	[Serializable]
	public class RegularSerializedTest : ITextData
	{
		public int PublicField;

		int _privateField;

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}

	[Serializable]
	public class NotSerializedTest : ITextData
	{
		[NotSerialized]
		public int PublicField;

		[Serialized()]
		int _privateField;

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}

	[Serializable]
	public class SerializedTest : ITextData
	{
		[Serialized("PrivateField")]
		int _privateField;

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}

	[Serializable]
	public class PublicPropertyTest : ITextData
	{
		public int IntProperty { get; set; }

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}

	[Serializable]
	public class PublicPropertyNotSerializedTest : ITextData
	{
		[NotSerialized]
		public int IntProperty { get; set; }

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}

	[Serializable]
	public class PrivatePropertyNotSerializedTest : ITextData
	{
		int IntProperty { get; set; }

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}

	[Serializable]
	public class PublicPropertyWithPrivateSetNotSerializedTest : ITextData
	{
		public int IntProperty { get; private set; }

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}

	[Serializable]
	public class PublicPropertyWithPrivateGetNotSerializedTest : ITextData
	{
		public int IntProperty { private get; set; }

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}

	[Serializable]
	public class PrivatePropertySerializedTest : ITextData
	{
		[Serialized]
		int IntProperty { get; set; }

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}

	public abstract partial class BaseTextDataTests
	{
		[Test(Description = "Test that public properties are serialized")]
		public void ToSerializedString_HasPublicProperty_PublicPropertyShouldAppearInString()
		{
			var testObject = new PublicPropertyTest();
			testObject.IntProperty = 1337;

			string serialized = _textDataIO.ToSerializedString(testObject);

			Debug.Log(serialized);

			Assert.IsTrue(serialized.Contains("IntProperty"));
		}

		[Test(Description = "Test that public properties with NotSerializedAttribute are not serialized")]
		public void ToSerializedString_HasPublicPropertyWithNotSerialized_PublicPropertyShouldNotAppearInString()
		{
			var testObject = new PublicPropertyNotSerializedTest();
			testObject.IntProperty = 1337;

			string serialized = _textDataIO.ToSerializedString(testObject);

			Debug.Log(serialized);

			Assert.IsFalse(serialized.Contains("IntProperty"));
		}

		[Test(Description = "Test that private properties are not serialized.")]
		public void ToSerializedString_HasPrivateProperty_PrivatePropertyShouldNotAppearInString()
		{
			var testObject = new PrivatePropertyNotSerializedTest();

			string serialized = _textDataIO.ToSerializedString(testObject);

			Debug.Log(serialized);

			Assert.IsFalse(serialized.Contains("IntProperty"));
		}

		[Test(Description = "Test that public properties with a private set are not serialized")]
		public void ToSerializedString_HasPublicPropertyWithPrivateSet_PropertyShouldNotAppearInString()
		{
			var testObject = new PublicPropertyWithPrivateSetNotSerializedTest();

			string serialized = _textDataIO.ToSerializedString(testObject);

			Debug.Log(serialized);

			Assert.IsFalse(serialized.Contains("IntProperty"));
		}

		[Test(Description = "Test that public properties with a private get are not serialized")]
		public void ToSerializedString_HasPublicPropertyWithPrivateGet_PropertyShouldNotAppearInString()
		{
			var testObject = new PublicPropertyWithPrivateGetNotSerializedTest();

			string serialized = _textDataIO.ToSerializedString(testObject);

			Debug.Log(serialized);

			Assert.IsFalse(serialized.Contains("IntProperty"));
		}

		[Test(Description = "Test that private properties with SerializedAttribute are serialized")]
		public void ToSerializedString_HasPrivatePropertyWithSerialized_PrivatePropertyShouldAppearInString()
		{
			var testObject = new PrivatePropertySerializedTest();

			string serialized = _textDataIO.ToSerializedString(testObject);

			Debug.Log(serialized);

			Assert.IsTrue(serialized.Contains("IntProperty"));
		}

		[Test(Description = "Test that public fields are serialized")]
		public void ToSerializedString_HasPublicField_PublicFieldShouldAppearInString()
		{
			var testObject = new RegularSerializedTest();

			string serialized = _textDataIO.ToSerializedString(testObject);

			Debug.Log(serialized);

			Assert.IsTrue(serialized.Contains("PublicField"));
		}

		[Test(Description = "Test that public fields with the NotSerializedAttribute are not serialized")]
		public void ToSerializedString_HasPublicFieldWithNotSerialized_PublicFieldShouldNotAppearInString()
		{
			var testObject = new NotSerializedTest();

			string serialized = _textDataIO.ToSerializedString(testObject);

			Debug.Log(serialized);

			Assert.IsFalse(serialized.Contains("PublicField"));
		}

		[Test(Description = "Test that private fields are not serialized")]
		public void ToSerializedString_HasPrivateField_PrivateFieldShouldNotAppearInString()
		{
			var testObject = new RegularSerializedTest();

			string serialized = _textDataIO.ToSerializedString(testObject);

			Debug.Log(serialized);

			Assert.IsFalse(serialized.Contains("_privateField"));
		}

		[Test(Description = "Test that private fields with the SerializedAttribute are serialized")]
		public void ToSerializedString_HasPrivateFieldWithSerialized_PrivateFieldShouldAppearInString()
		{
			var testObject = new NotSerializedTest();

			string serialized = _textDataIO.ToSerializedString(testObject);

			Debug.Log(serialized);

			Assert.IsTrue(serialized.Contains("_privateField"));
		}

		[Test(Description = "Test that private fields with the SerializedAttribute and have a custom name are serialized with that name")]
		public void ToSerializedString_HasPrivateFieldWithSerializedName_PrivateFieldNameShouldAppearInString()
		{
			var testObject = new SerializedTest();

			string serialized = _textDataIO.ToSerializedString(testObject);

			Debug.Log(serialized);

			Assert.IsTrue(serialized.Contains("PrivateField"));
		}
	}
}