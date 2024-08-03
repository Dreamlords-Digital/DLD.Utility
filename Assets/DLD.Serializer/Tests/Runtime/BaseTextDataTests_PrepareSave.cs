// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.IO;
using NUnit.Framework;

namespace DLD.Serializer.Tests
{
	public abstract partial class BaseTextDataTests
	{
		static PrepareSaveClass CreatePrepClass(string valueAfterPrepareSave)
		{
			var result = new PrepareSaveClass();

			Assert.AreEqual(PrepareSaveClass.VALUE_DEFAULT, result.Value);

			Assert.AreNotEqual(PrepareSaveClass.VALUE_DEFAULT, valueAfterPrepareSave);

			result.SetValueAfterPrepareSave(valueAfterPrepareSave);
			Assert.AreEqual(PrepareSaveClass.VALUE_DEFAULT, result.Value);

			return result;
		}

		[Test(Description = "Ensure ITextData.PrepareSave really gets called after call to ITextDataIO.ToSerializedString.")]
		public void ToSerializedString_OnOneClass_PrepareSaveGetsCalled()
		{
			// -----------------------------------------------

			const string VALUE_AFTER_TO_SERIALIZED_STRING = "After ToSerializedString";
			var prep = CreatePrepClass(VALUE_AFTER_TO_SERIALIZED_STRING);

			// -----------------------------------------------

			_textDataIO.ToSerializedString(prep);

			// -----------------------------------------------

			Assert.AreEqual(VALUE_AFTER_TO_SERIALIZED_STRING, prep.Value);
		}

		[Test(Description = "Ensure ITextData.PrepareSave really gets called after call to ITextDataIO.SaveToLocal.")]
		public void SaveToLocal_OnOneClass_PrepareSaveGetsCalled()
		{
			// -----------------------------------------------

			const string VALUE_AFTER_TRY_LOAD_FROM_LOCAL = "After TryLoadFromLocal";
			var prep = CreatePrepClass(VALUE_AFTER_TRY_LOAD_FROM_LOCAL);

			// -----------------------------------------------

			PrepareSaveClass deserializedPrepFromFile;
			string prepSavePath;
			DoLoadFromLocal("Prep.txt", prep, out deserializedPrepFromFile, out prepSavePath);

			// -----------------------------------------------

			Assert.AreEqual(VALUE_AFTER_TRY_LOAD_FROM_LOCAL, prep.Value);

			// -----------------------------------------------

			// done with our temporary serialized class,
			// delete its file so it doesn't waste space
			File.Delete(prepSavePath);
		}
	}
}