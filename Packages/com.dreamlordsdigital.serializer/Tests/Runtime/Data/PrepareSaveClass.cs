// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;

namespace DLD.Serializer.Tests
{

[Serializable]
public class PrepareSaveClass : ITextData
{
	string _valueAfterPrepareSave;

	string _value;

	public string Value
	{
		get
		{
			return _value;
		}
	}

	public void SetValueAfterPrepareSave(string newValue)
	{
		_valueAfterPrepareSave = newValue;
	}

	// -----------------------------

	public const string ValueDefault = "Nothing";
	public const string ValueAfterPrepareSaveDefault = "qw";

	public PrepareSaveClass()
	{
		_value = ValueDefault;
		_valueAfterPrepareSave = ValueAfterPrepareSaveDefault;
	}

	// -----------------------------

	public void PostLoad(string fullPath, string filename)
	{
	}

	public void PrepareSave()
	{
		_value = _valueAfterPrepareSave;
	}
}

}
