// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.IO;
using DLD.Utility;
using NUnit.Framework;

namespace DLD.Serializer.Tests
{
	public abstract partial class BaseTextDataTests
	{
		// ==========================================================

		const string GAME_MOD_PACKAGE_SUB_PATH = "Dreamlords Digital/GwP/UnitTest/";

		string _pathInCommonFolder;

		public string SavePath
		{
			get
			{
				if (string.IsNullOrEmpty(_pathInCommonFolder))
				{
					_pathInCommonFolder = $"{FileUtil.CommonDataFolder}/{GAME_MOD_PACKAGE_SUB_PATH}";
				}

				return _pathInCommonFolder;
			}
		}

		[SetUp]
		public void SetUp()
		{
			_textDataIO = GetTextDataIOInstance();
			if (Directory.Exists(SavePath))
			{
				Directory.Delete(SavePath, true);
			}
		}

		[TearDown]
		public void TearDown()
		{
		}

		// ==========================================================

		ITextDataIO _textDataIO;
		protected abstract ITextDataIO GetTextDataIOInstance();

		// ==========================================================

		void DoLoadFromLocal<T>(string filename, T data, out T deserialized, out string savePath) where T : ITextData
		{
			savePath = string.Format("{0}{1}", SavePath, filename);
			_textDataIO.SaveToLocal(savePath, data);

			deserialized = default(T);
			var loadResult = _textDataIO.TryLoadFromLocal(savePath, ref deserialized);

			Assert.True(loadResult.result == LoadResult.Success, "Loading from serialized class failed: {0}", savePath);

			Assert.IsNotNull(deserialized);
		}
	}
}
