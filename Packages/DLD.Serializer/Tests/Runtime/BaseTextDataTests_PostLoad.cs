// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace DLD.Serializer.Tests
{
	public abstract partial class BaseTextDataTests
	{
		static PostLoadClass CreatePostLoadClass(string id)
		{
			var post = new PostLoadClass();

			post.ID = id;
			Assert.AreEqual(0, post.TimesPostLoadGotCalled);
			Assert.AreEqual(PostLoadClass.FILENAME_GOT_DEFAULT, post.FilenameGotFromPostLoad);

			return post;
		}

		[Test(Description = "Ensure ITextData.PostLoad really gets called on deserialized objects after call to ITextDataIO.FromSerializedString.")]
		public void FromSerializedString_OnText_PostLoadIsCalledCorrectly()
		{
			// -----------------------------------------------

			var post = CreatePostLoadClass("post FromSerializedString");

			// -----------------------------------------------

			var serialized = _textDataIO.ToSerializedString(post);
			var deserialized = _textDataIO.FromSerializedString<PostLoadClass>(serialized);

			// -----------------------------------------------

			Assert.AreEqual(1, deserialized.TimesPostLoadGotCalled);
			Assert.AreEqual(string.Empty, deserialized.FilenameGotFromPostLoad);

			// PostLoad() was called on the deserialized copy, not the original
			// the original should not have had their PostLoad() called
			Assert.AreEqual(0, post.TimesPostLoadGotCalled);
			Assert.AreEqual(PostLoadClass.FILENAME_GOT_DEFAULT, post.FilenameGotFromPostLoad);
		}

		[Test(Description = "Ensure ITextData.PostLoad really gets called on deserialized objects after call to ITextDataIO.TryLoadFromLocal.")]
		public void TryLoadFromLocal_OnFile_PostLoadIsCalledCorrectly()
		{
			// -----------------------------------------------

			var post = CreatePostLoadClass("post TryLoadFromLocal");

			// -----------------------------------------------

			PostLoadClass deserializedFromFile;
			string postSavePath;
			DoLoadFromLocal("Post.txt", post, out deserializedFromFile, out postSavePath);

			// -----------------------------------------------

			Assert.AreEqual(1, deserializedFromFile.TimesPostLoadGotCalled);
			Assert.AreEqual("Post.txt", deserializedFromFile.FilenameGotFromPostLoad);

			// PostLoad() was called on the deserialized copy, not the original
			// the original should not have had their PostLoad() called
			Assert.AreEqual(0, post.TimesPostLoadGotCalled);
			Assert.AreEqual(PostLoadClass.FILENAME_GOT_DEFAULT, post.FilenameGotFromPostLoad);

			// -----------------------------------------------

			// done with our temporary serialized class,
			// delete its file so it doesn't waste space
			File.Delete(postSavePath);
		}

		[Test(Description = "Ensure ITextData.PostLoad really gets called on deserialized objects after call to ITextDataIO.LoadAllFromLocal.")]
		public void LoadAllFromLocal_OnThreeFiles_PostLoadIsCalledCorrectly()
		{
			// -----------------------------------------------

			const string POST1 = "post1";
			const string POST2 = "post2";
			const string POST3 = "post3";

			var post1 = CreatePostLoadClass(POST1);
			var post2 = CreatePostLoadClass(POST2);
			var post3 = CreatePostLoadClass(POST3);

			// -----------------------------------------------

			var savePath = string.Format("{0}PostFiles/", SavePath);

			const string POST1_FILENAME = "postLoadA1.txt";
			const string POST2_FILENAME = "postLoad2B.txt";
			const string POST3_FILENAME = "postLoad9C.txt";
			var post1SavePath = string.Format("{0}{1}", savePath, POST1_FILENAME);
			var post2SavePath = string.Format("{0}{1}", savePath, POST2_FILENAME);
			var post3SavePath = string.Format("{0}{1}", savePath, POST3_FILENAME);

			_textDataIO.SaveToLocal(post1SavePath, post1);
			_textDataIO.SaveToLocal(post2SavePath, post2);
			_textDataIO.SaveToLocal(post3SavePath, post3);

			// -----------------------------------------------
			// Check that the files do exist and they have contents inside.

			Assert.IsTrue(File.Exists(post1SavePath));
			Assert.IsTrue(File.Exists(post2SavePath));
			Assert.IsTrue(File.Exists(post3SavePath));

			int txtFileCount = 0;
			foreach (string file in Directory.EnumerateFiles(savePath, $"*.txt"))
			{
				string text = File.ReadAllText(file);
				Assert.IsTrue(text.Length > 0);

				++txtFileCount;
			}
			Assert.AreEqual(3, txtFileCount);

			// -----------------------------------------------

			var outputList = new List<PostLoadClass>();

			_textDataIO.LoadAllFromLocal(savePath, ".txt", outputList);

			// -----------------------------------------------

			Assert.AreEqual(3, outputList.Count);

			// there's no guaranteed order in the list we got,
			// so check each by their ID
			for (int n = 0, len = outputList.Count; n < len; ++n)
			{
				Assert.AreEqual(1, outputList[n].TimesPostLoadGotCalled);

				if (outputList[n].ID == POST1)
				{
					Assert.AreEqual(POST1_FILENAME, outputList[n].FilenameGotFromPostLoad);
				}
				else if (outputList[n].ID == POST2)
				{
					Assert.AreEqual(POST2_FILENAME, outputList[n].FilenameGotFromPostLoad);
				}
				else if (outputList[n].ID == POST3)
				{
					Assert.AreEqual(POST3_FILENAME, outputList[n].FilenameGotFromPostLoad);
				}
			}

			// PostLoad() was called on the deserialized copies, not the original objects
			// the originals should not have had their PostLoad() called
			Assert.AreEqual(0, post1.TimesPostLoadGotCalled);
			Assert.AreEqual(PostLoadClass.FILENAME_GOT_DEFAULT, post1.FilenameGotFromPostLoad);

			Assert.AreEqual(0, post2.TimesPostLoadGotCalled);
			Assert.AreEqual(PostLoadClass.FILENAME_GOT_DEFAULT, post2.FilenameGotFromPostLoad);

			Assert.AreEqual(0, post3.TimesPostLoadGotCalled);
			Assert.AreEqual(PostLoadClass.FILENAME_GOT_DEFAULT, post3.FilenameGotFromPostLoad);

			// -----------------------------------------------

			// done with our temporary serialized classes,
			// delete their files so it doesn't waste space
			File.Delete(post1SavePath);
			File.Delete(post2SavePath);
			File.Delete(post3SavePath);
		}
	}
}