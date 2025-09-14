// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;

namespace DLD.Serializer.Tests
{

[Serializable]
public class PostLoadClass : ITextData
{
	public string ID;

	string _filenameGotFromPostLoad;

	public string FilenameGotFromPostLoad
	{
		get
		{
			return _filenameGotFromPostLoad;
		}
	}

	int _timesPostLoadGotCalled;

	public int TimesPostLoadGotCalled
	{
		get
		{
			return _timesPostLoadGotCalled;
		}
	}

	// -----------------------------

	public const string FilenameGotDefault = "No Value";

	public PostLoadClass()
	{
		_filenameGotFromPostLoad = FilenameGotDefault;
	}

	// -----------------------------

	public void PostLoad(string fullPath, string filename)
	{
		_filenameGotFromPostLoad = filename;
		++_timesPostLoadGotCalled;
	}

	public void PrepareSave()
	{
	}
}

}
