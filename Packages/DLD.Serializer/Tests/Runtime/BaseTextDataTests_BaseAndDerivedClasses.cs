// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace DLD.Serializer.Tests
{
	public abstract partial class BaseTextDataTests
	{
		static BaseAndDerivedClassUser SetUpBaseAndDerivedList(int derived1FieldValue, int derived2FieldValue, int derived3FieldValue)
		{
			var stuff = new BaseAndDerivedClassUser();

			var derived11 = new Derived1();
			var derived12 = new Derived1();
			var derived13 = new Derived1();
			var derived14 = new Derived1();

			// add 8 elements to the List
			stuff.Add(new Derived2()); // 0
			stuff.Add(derived11);      // 1
			stuff.Add(new Derived2()); // 2
			stuff.Add(derived12);      // 3
			stuff.Add(new Derived3()); // 4
			stuff.Add(new Derived2()); // 5
			stuff.Add(derived13);      // 6
			stuff.Add(derived14);      // 7

			Assert.AreEqual(8, stuff.Count);

			derived11.SetMyField(derived1FieldValue);
			derived12.SetMyField(derived2FieldValue);
			derived13.SetMyField(derived3FieldValue);

			return stuff;
		}

		static void AssertBaseAndDerivedList(BaseAndDerivedClassUser original, BaseAndDerivedClassUser deserialized,
			int derived1FieldValue, int derived2FieldValue, int derived3FieldValue)
		{
			Assert.AreEqual(8, deserialized.Count);

			Assert.AreEqual(Derived2.DERIVED_2_TYPE, deserialized[0].MyType);
			Assert.AreEqual(Derived1.DERIVED_1_TYPE, deserialized[1].MyType);
			Assert.AreEqual(Derived2.DERIVED_2_TYPE, deserialized[2].MyType);
			Assert.AreEqual(Derived1.DERIVED_1_TYPE, deserialized[3].MyType);
			Assert.AreEqual(Derived3.DERIVED_3_TYPE, deserialized[4].MyType);
			Assert.AreEqual(Derived2.DERIVED_2_TYPE, deserialized[5].MyType);
			Assert.AreEqual(Derived1.DERIVED_1_TYPE, deserialized[6].MyType);
			Assert.AreEqual(Derived1.DERIVED_1_TYPE, deserialized[7].MyType);

			Assert.IsInstanceOf<Derived2>(deserialized[0]);
			Assert.IsInstanceOf<Derived1>(deserialized[1]);
			Assert.IsInstanceOf<Derived2>(deserialized[2]);
			Assert.IsInstanceOf<Derived1>(deserialized[3]);
			Assert.IsInstanceOf<Derived3>(deserialized[4]);
			Assert.IsInstanceOf<Derived2>(deserialized[5]);
			Assert.IsInstanceOf<Derived1>(deserialized[6]);
			Assert.IsInstanceOf<Derived1>(deserialized[7]);

			// ------------------

			var derived11 = (Derived1) original[1];
			var deserializedDerived11 = (Derived1) deserialized[1];
			Assert.NotNull(deserializedDerived11);

			Assert.AreEqual(derived1FieldValue, derived11.MyField);
			Assert.AreEqual(derived1FieldValue, deserializedDerived11.MyField);

			// ------------------

			var derived12 = (Derived1) original[3];
			var deserializedDerived12 = (Derived1) deserialized[3];
			Assert.NotNull(deserializedDerived12);

			Assert.AreEqual(derived2FieldValue, derived12.MyField);
			Assert.AreEqual(derived2FieldValue, deserializedDerived12.MyField);

			// ------------------

			var derived13 = (Derived1) original[6];
			var deserializedDerived13 = (Derived1) deserialized[6];
			Assert.NotNull(deserializedDerived13);

			Assert.AreEqual(derived3FieldValue, derived13.MyField);
			Assert.AreEqual(derived3FieldValue, deserializedDerived13.MyField);
		}


		[Test(Description = "Ensure that a polymorphic list (with 8 elements) gets deserialized properly when using ITextDataIO.FromSerializedString.")]
		public void FromSerializedString_OnLargePolymorphicList_DeserializesProperly()
		{
			// ensure that a List of base type, but whose elements are
			// different kinds of derived types, will get deserialized
			// properly (using ITextDataIO.FromSerializedString)

			const int DERIVED11_NEW_FIELD_VALUE = 34;
			const int DERIVED12_NEW_FIELD_VALUE = 7;
			const int DERIVED13_NEW_FIELD_VALUE = -56;

			var stuff = SetUpBaseAndDerivedList(
				DERIVED11_NEW_FIELD_VALUE, DERIVED12_NEW_FIELD_VALUE, DERIVED13_NEW_FIELD_VALUE);

			// --------------------------------------------

			var serialized = _textDataIO.ToSerializedString(stuff);
			Debug.Log($"Serialized:\n{serialized}");
			var deserialized = _textDataIO.FromSerializedString<BaseAndDerivedClassUser>(serialized);

			// --------------------------------------------

			AssertBaseAndDerivedList(stuff, deserialized,
				DERIVED11_NEW_FIELD_VALUE, DERIVED12_NEW_FIELD_VALUE, DERIVED13_NEW_FIELD_VALUE);
		}


		[Test(Description = "Ensure that a List of base type, but whose elements are different kinds of derived types, will get deserialized properly (using ITextDataIO.TryLoadFromLocal).")]
		public void TryLoadFromLocal_OnPolymorphicList_DeserializesProperly()
		{
			const int DERIVED11_NEW_FIELD_VALUE = 48;
			const int DERIVED12_NEW_FIELD_VALUE = -547;
			const int NICE = 69;

			var stuff = SetUpBaseAndDerivedList(
				DERIVED11_NEW_FIELD_VALUE, DERIVED12_NEW_FIELD_VALUE, NICE);

			// --------------------------------------------

			BaseAndDerivedClassUser deserialized;
			string savePath;
			DoLoadFromLocal("List.txt", stuff, out deserialized, out savePath);

			// --------------------------------------------

			AssertBaseAndDerivedList(stuff, deserialized,
				DERIVED11_NEW_FIELD_VALUE, DERIVED12_NEW_FIELD_VALUE, NICE);

			// done with our temporary serialized class,
			// delete its file so it doesn't waste space
			File.Delete(savePath);
		}

		[Test(Description = "Try serializing then deserializing a list of base class, where the elements are different derived types.")]
		public void FromSerializedString_OnPolymorphicList_DeserializesProperly()
		{
			BaseAndDerivedClassUser toSave = new BaseAndDerivedClassUser();
			toSave.Add(new Derived2());

			var element2 = new Derived1();
			element2.SetMyField(32);
			toSave.Add(element2);


			var serializedText = _textDataIO.ToSerializedString(toSave);
			Debug.Log(serializedText);
			var deserialized = _textDataIO.FromSerializedString<BaseAndDerivedClassUser>(serializedText);

			Assert.IsNotNull(deserialized);
			Assert.AreEqual(2, deserialized.Count);

			Assert.IsInstanceOf<Derived2>(deserialized[0]);
			Assert.IsInstanceOf<Derived1>(deserialized[1]);

			var deserializedDerived1 = (Derived1) deserialized[1];
			Assert.IsNotNull(deserializedDerived1);

			Assert.AreEqual(32, deserializedDerived1.MyField);
		}

		[Test(Description = "Try deserializing a list of base class, where the elements are different derived types. A full type hint is included for each.")]
		public void FromSerializedString_OnPolymorphicListWithFullTypeHint_DeserializesProperly()
		{
			const string SERIALIZED = @"
{
	""" + BaseTextDataIO.TYPE_HINT_NAME + @""": ""DLD.Serializer.Tests.BaseAndDerivedClassUser, DLD.Serializer.Tests"",
	""List"":
	[
		 {
			 """ + BaseTextDataIO.TYPE_HINT_NAME + @""": ""DLD.Serializer.Tests.Derived2, DLD.Serializer.Tests"",
			 ""ID"": null
		 },
		 {
			 """ + BaseTextDataIO.TYPE_HINT_NAME + @""": ""DLD.Serializer.Tests.Derived1, DLD.Serializer.Tests"",
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