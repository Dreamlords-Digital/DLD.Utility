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
			get { return _value; }
		}

		public void SetValueAfterPrepareSave(string newValue)
		{
			_valueAfterPrepareSave = newValue;
		}

		// -----------------------------

		public const string VALUE_DEFAULT = "Nothing";
		public const string VALUE_AFTER_PREPARE_SAVE_DEFAULT = "qw";

		public PrepareSaveClass()
		{
			_value = VALUE_DEFAULT;
			_valueAfterPrepareSave = VALUE_AFTER_PREPARE_SAVE_DEFAULT;
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
