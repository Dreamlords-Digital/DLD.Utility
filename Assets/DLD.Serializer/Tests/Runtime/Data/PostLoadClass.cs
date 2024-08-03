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
			get { return _filenameGotFromPostLoad; }
		}

		int _timesPostLoadGotCalled;

		public int TimesPostLoadGotCalled
		{
			get { return _timesPostLoadGotCalled; }
		}

		// -----------------------------

		public const string FILENAME_GOT_DEFAULT = "No Value";

		public PostLoadClass()
		{
			_filenameGotFromPostLoad = FILENAME_GOT_DEFAULT;
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
