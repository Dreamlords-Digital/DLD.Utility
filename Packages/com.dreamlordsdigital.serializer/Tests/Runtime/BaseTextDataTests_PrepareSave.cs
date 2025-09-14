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

		Assert.AreEqual(PrepareSaveClass.ValueDefault, result.Value);

		Assert.AreNotEqual(PrepareSaveClass.ValueDefault, valueAfterPrepareSave);

		result.SetValueAfterPrepareSave(valueAfterPrepareSave);
		Assert.AreEqual(PrepareSaveClass.ValueDefault, result.Value);

		return result;
	}

	[Test(Description = "Ensure ITextData.PrepareSave really gets called after call to ITextDataIO.ToSerializedString.")]
	public void ToSerializedString_OnOneClass_PrepareSaveGetsCalled()
	{
		// -----------------------------------------------

		const string ValueAfterToSerializedString = "After ToSerializedString";
		var prep = CreatePrepClass(ValueAfterToSerializedString);

		// -----------------------------------------------

		_textDataIO.ToSerializedString(prep);

		// -----------------------------------------------

		Assert.AreEqual(ValueAfterToSerializedString, prep.Value);
	}

	[Test(Description = "Ensure ITextData.PrepareSave really gets called after call to ITextDataIO.SaveToLocal.")]
	public void SaveToLocal_OnOneClass_PrepareSaveGetsCalled()
	{
		// -----------------------------------------------

		const string ValueAfterTryLoadFromLocal = "After TryLoadFromLocal";
		var prep = CreatePrepClass(ValueAfterTryLoadFromLocal);

		// -----------------------------------------------

		PrepareSaveClass deserializedPrepFromFile;
		string prepSavePath;
		DoLoadFromLocal("Prep.txt", prep, out deserializedPrepFromFile, out prepSavePath);

		// -----------------------------------------------

		Assert.AreEqual(ValueAfterTryLoadFromLocal, prep.Value);

		// -----------------------------------------------

		// done with our temporary serialized class,
		// delete its file so it doesn't waste space
		File.Delete(prepSavePath);
	}
}

}
