// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using NUnit.Framework;

namespace DLD.Serializer.Tests
{
	[Serializable]
	public class RenameTest : ITextData
	{
		public int Field;

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}

	public abstract partial class BaseTextDataTests
	{
		[Test(Description = "The standard situation where no change to the type hint has been done.")]
		public void FromSerializedString_CorrectTypeHint_DeserializesProperly()
		{
			const string SERIALIZED = "{" +
			                          "	\"" + BaseTextDataIO.TYPE_HINT_NAME + "\": \"DLD.Serializer.Tests.RenameTest, Assembly-CSharp-Editor\"," +
			                          "	\"Field\": 10" +
			                          "}";
			var deserialized = _textDataIO.FromSerializedString<RenameTest>(SERIALIZED);

			Assert.IsNotNull(deserialized);
			Assert.AreEqual(10, deserialized.Field);
		}

		[Test(Description = "Try deserializing a class that has no type hint.")]
		public void FromSerializedString_NoTypeHint_DeserializesProperly()
		{
			const string SERIALIZED = "{" +
			                          "	\"Field\": 10" +
			                          "}";
			var deserialized = _textDataIO.FromSerializedString<RenameTest>(SERIALIZED);

			Assert.IsNotNull(deserialized);
			Assert.AreEqual(10, deserialized.Field);
		}

		[Test(Description = "Try deserializing a class where type hint is correct but does not specify an assembly name.")]
		public void FromSerializedString_TypeHintHasNoAssembly_DeserializesProperly()
		{
			const string SERIALIZED = "{" +
			                          "	\"" + BaseTextDataIO.TYPE_HINT_NAME + "\": \"DLD.Serializer.Tests.RenameTest\"," +
			                          "	\"Field\": 10" +
			                          "}";
			var deserialized = _textDataIO.FromSerializedString<RenameTest>(SERIALIZED);

			Assert.IsNotNull(deserialized);
			Assert.AreEqual(10, deserialized.Field);
		}

		[Test(Description = "Try deserializing a class even though the string is specifying an old assembly name that doesn't exist anymore.")]
		public void FromSerializedString_TypeHintHasNonExistentAssembly_DeserializesProperly()
		{
			const string SERIALIZED = "{" +
			                          "	\"" + BaseTextDataIO.TYPE_HINT_NAME + "\": \"DLD.Serializer.Tests.RenameTest, NonExistentAssembly\"," +
			                          "	\"Field\": 10" +
			                          "}";
			var deserialized = _textDataIO.FromSerializedString<RenameTest>(SERIALIZED);

			Assert.IsNotNull(deserialized);
			Assert.AreEqual(10, deserialized.Field);
		}

		[Test(Description = "Try deserializing a class even though the string is specifying an old namespace name that doesn't exist anymore.")]
		public void FromSerializedString_TypeHintHasNonExistentNamespace_DeserializesProperly()
		{
			const string SERIALIZED = "{" +
			                          "	\"" + BaseTextDataIO.TYPE_HINT_NAME + "\": \"NonExistentNamespace.RenameTest, Assembly-CSharp-Editor\"," +
			                          "	\"Field\": 10" +
			                          "}";
			var deserialized = _textDataIO.FromSerializedString<RenameTest>(SERIALIZED);

			Assert.IsNotNull(deserialized);
			Assert.AreEqual(10, deserialized.Field);
		}

		[Test(Description = "Try deserializing a class even though the string is specifying an old class name that doesn't exist anymore.")]
		public void FromSerializedString_TypeHintHasNonExistentClass_DeserializesProperly()
		{
			const string SERIALIZED = "{" +
			                          "	\"" + BaseTextDataIO.TYPE_HINT_NAME + "\": \"DLD.Utility.UnitTest.NonExistentClass, Assembly-CSharp-Editor\"," +
			                          "	\"Field\": 10" +
			                          "}";
			var deserialized = _textDataIO.FromSerializedString<RenameTest>(SERIALIZED);

			Assert.IsNotNull(deserialized);
			Assert.AreEqual(10, deserialized.Field);
		}

		// ===========================================================

		[Test(Description = "Try deserializing a list of base class, where the elements are different derived types. A full type hint is included for each, but with the assembly name is wrong.")]
		public void FromSerializedString_OnPolymorphicListButWrongAssemblyHint_DeserializesProperly()
		{
			const string SERIALIZED = @"
{
	""" + BaseTextDataIO.TYPE_HINT_NAME + @""": ""DLD.Serializer.Tests.BaseAndDerivedClassUser, Assembly-CSharp-Editor"",
	""List"":
	[
		 {
			 """ + BaseTextDataIO.TYPE_HINT_NAME + @""": ""DLD.Serializer.Tests.Derived2, Assembly-CSharp-Editor"",
			 ""ID"": null
		 },
		 {
			 """ + BaseTextDataIO.TYPE_HINT_NAME + @""": ""DLD.Serializer.Tests.Derived1, Assembly-CSharp-Editor"",
			 ""ID"": null,
			 ""MyField"": 32
		 }
	]
}";
			var deserialized = _textDataIO.FromSerializedString<BaseAndDerivedClassUser>(SERIALIZED);
			Assert.IsNotNull(deserialized);
			Assert.AreEqual(2, deserialized.Count);

			Assert.IsInstanceOf<Derived2>(deserialized[0]);
			Assert.IsInstanceOf<Derived1>(deserialized[1]);

			var deserializedDerived1 = (Derived1) deserialized[1];
			Assert.IsNotNull(deserializedDerived1);

			Assert.AreEqual(32, deserializedDerived1.MyField);
		}

		[Test(Description = "Try deserializing a list of base class, where the elements are different derived types. A full type hint is included for each, but with an assembly that doesn't even exist.")]
		public void FromSerializedString_OnPolymorphicListButNonExistentAssemblyHint_DeserializesProperly()
		{
			const string SERIALIZED = @"
{
	""" + BaseTextDataIO.TYPE_HINT_NAME + @""": ""DLD.Serializer.Tests.BaseAndDerivedClassUser, NonExistentAssembly"",
	""List"":
	[
		 {
			 """ + BaseTextDataIO.TYPE_HINT_NAME + @""": ""DLD.Serializer.Tests.Derived2, NonExistentAssembly"",
			 ""ID"": null
		 },
		 {
			 """ + BaseTextDataIO.TYPE_HINT_NAME + @""": ""DLD.Serializer.Tests.Derived1, NonExistentAssembly"",
			 ""ID"": null,
			 ""MyField"": 32
		 }
	]
}";
			var deserialized = _textDataIO.FromSerializedString<BaseAndDerivedClassUser>(SERIALIZED);
			Assert.IsNotNull(deserialized);
			Assert.AreEqual(2, deserialized.Count);

			Assert.IsInstanceOf<Derived2>(deserialized[0]);
			Assert.IsInstanceOf<Derived1>(deserialized[1]);

			var deserializedDerived1 = (Derived1) deserialized[1];
			Assert.IsNotNull(deserializedDerived1);

			Assert.AreEqual(32, deserializedDerived1.MyField);
		}

		[Test(Description = "Try deserializing a list of base class, where the elements are different derived types. The type hint included for each doesn't specify the assembly name.")]
		public void FromSerializedString_OnPolymorphicListButNoAssemblyHint_DeserializesProperly()
		{
			const string SERIALIZED = @"
{
	""" + BaseTextDataIO.TYPE_HINT_NAME + @""": ""DLD.Serializer.Tests.BaseAndDerivedClassUser"",
	""List"":
	[
		 {
			 """ + BaseTextDataIO.TYPE_HINT_NAME + @""": ""DLD.Serializer.Tests.Derived2"",
			 ""ID"": null
		 },
		 {
			 """ + BaseTextDataIO.TYPE_HINT_NAME + @""": ""DLD.Serializer.Tests.Derived1"",
			 ""ID"": null,
			 ""MyField"": 32
		 }
	]
}";
			var deserialized = _textDataIO.FromSerializedString<BaseAndDerivedClassUser>(SERIALIZED);
			Assert.IsNotNull(deserialized);
			Assert.AreEqual(2, deserialized.Count);

			Assert.IsInstanceOf<Derived2>(deserialized[0]);
			Assert.IsInstanceOf<Derived1>(deserialized[1]);

			var deserializedDerived1 = (Derived1) deserialized[1];
			Assert.IsNotNull(deserializedDerived1);

			Assert.AreEqual(32, deserializedDerived1.MyField);
		}

		/// <summary>
		/// Try deserializing a list of base class, where the elements are different derived types.
		/// No type hint is included.
		/// </summary>
		//
		// Note: I disabled this Test since I know JsonFx can't do this,
		// it's a nice-to-have, but we don't really need this yet.
		//[Test]
		public void TestListOfDerivedClass_NoTypeHint()
		{
			const string SERIALIZED = @"
{
	""List"":
	[
		 {
			 ""ID"": null
		 },
		 {
			 ""ID"": null,
			 ""MyField"": 32
		 }
	]
}";
			var deserialized = _textDataIO.FromSerializedString<BaseAndDerivedClassUser>(SERIALIZED);
			Assert.IsNotNull(deserialized);
			Assert.AreEqual(2, deserialized.Count);

			Assert.IsInstanceOf<Derived2>(deserialized[0]);
			Assert.IsInstanceOf<Derived1>(deserialized[1]);

			var deserializedDerived1 = (Derived1) deserialized[1];
			Assert.IsNotNull(deserializedDerived1);

			Assert.AreEqual(32, deserializedDerived1.MyField);
		}
	}
}