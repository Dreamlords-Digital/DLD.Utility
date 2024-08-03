// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace DLD.Serializer.Tests
{
	public abstract partial class BaseTextDataTests
	{
		static TreeGraph.Root CreateTestTree()
		{
			var root = new TreeGraph.Root();
				root.Add(new TreeGraph.Leaf1());
				var comp1 = new TreeGraph.Composite();
				root.Add(comp1);
					comp1.Add(new TreeGraph.Leaf1());
					comp1.Add(new TreeGraph.Leaf2());
					comp1.Add(new TreeGraph.Leaf2());
				var comp2 = new TreeGraph.Composite();
				root.Add(comp2);
					comp2.Add(new TreeGraph.Leaf1());
					var comp3 = new TreeGraph.Composite();
					comp2.Add(comp3);
						comp3.Add(new TreeGraph.Leaf2());
			return root;
		}

		static void AssertTreeCorrect(TreeGraph.Root root)
		{
			Assert.AreEqual(3, root.Count);
			Assert.IsInstanceOf<TreeGraph.Root>(root);

			Assert.IsInstanceOf<TreeGraph.Leaf1>(root[0]);
			Assert.AreEqual(0, root[0].Count);
			Assert.IsInstanceOf<TreeGraph.Composite>(root[1]);
			Assert.AreEqual(3, root[1].Count);
			Assert.IsInstanceOf<TreeGraph.Composite>(root[2]);
			Assert.AreEqual(2, root[2].Count);


			Assert.IsInstanceOf<TreeGraph.Leaf1>(root[1][0]);
			Assert.IsInstanceOf<TreeGraph.Leaf2>(root[1][1]);
			Assert.IsInstanceOf<TreeGraph.Leaf2>(root[1][2]);


			Assert.IsInstanceOf<TreeGraph.Leaf1>(root[2][0]);
			Assert.IsInstanceOf<TreeGraph.Composite>(root[2][1]);

			Assert.IsInstanceOf<TreeGraph.Leaf2>(root[2][1][0]);
		}

		[Test(Description = "Ensure that a tree graph data structure, where each node is of a base type but can have children that are of derived types, will get deserialized properly (using ITextDataIO.FromSerializedString).")]
		public void FromSerializedString_OnTreeGraph_DeserializesProperly()
		{
			var root = CreateTestTree();
			AssertTreeCorrect(root);

			// -----------------------------------------------

			var serialized = _textDataIO.ToSerializedString(root);
			var deserialized = _textDataIO.FromSerializedString<TreeGraph.Root>(serialized);

			// -----------------------------------------------

			AssertTreeCorrect(root);
			AssertTreeCorrect(deserialized);
		}

		[Test(Description = "Ensure that a tree graph data structure, where each node is of a base type but can have children that are of derived types, will get deserialized properly (using ITextDataIO.TryLoadFromLocal).")]
		public void TryLoadFromLocal_OnTreeGraph_DeserializesProperly()
		{
			var root = CreateTestTree();
			AssertTreeCorrect(root);

			// -----------------------------------------------

			TreeGraph.Root deserialized;
			string savePath;
			DoLoadFromLocal("Root.txt", root, out deserialized, out savePath);

			// -----------------------------------------------

			AssertTreeCorrect(root);
			AssertTreeCorrect(deserialized);

			// -----------------------------------------------

			// done with our temporary serialized class,
			// delete its file so it doesn't waste space
			File.Delete(savePath);
		}


		[Test(Description = "Ensure that a tree graph data structure, where each node is of a base type but can have children that are of derived types, will get deserialized properly (using ITextDataIO.LoadAllFromLocal).")]
		public void LoadAllFromLocal_OnTreeGraph_DeserializesProperly()
		{
			var root1 = CreateTestTree();
			AssertTreeCorrect(root1);

			var root2 = CreateTestTree();
			AssertTreeCorrect(root2);

			var root3 = CreateTestTree();
			AssertTreeCorrect(root3);

			// -----------------------------------------------

			var savePath = string.Format("{0}TreeGraphs/", SavePath);
			const string ROOT1_FILENAME = "Root1.txt";
			const string ROOT2_FILENAME = "Root2.txt";
			const string ROOT3_FILENAME = "Root3.txt";

			var root1SavePath = string.Format("{0}{1}", savePath, ROOT1_FILENAME);
			var root2SavePath = string.Format("{0}{1}", savePath, ROOT2_FILENAME);
			var root3SavePath = string.Format("{0}{1}", savePath, ROOT3_FILENAME);

			_textDataIO.SaveToLocal(root1SavePath, root1);
			_textDataIO.SaveToLocal(root2SavePath, root2);
			_textDataIO.SaveToLocal(root3SavePath, root3);

			Assert.IsTrue(File.Exists(root1SavePath));
			Assert.IsTrue(File.Exists(root2SavePath));
			Assert.IsTrue(File.Exists(root3SavePath));

			var outputList = new List<TreeGraph.Root>();
			_textDataIO.LoadAllFromLocal(savePath, ".txt", outputList);

			// -----------------------------------------------

			Assert.AreEqual(3, outputList.Count);

			// there's no guaranteed order in the list we got,
			// so check each by their ID
			for (int n = 0, len = outputList.Count; n < len; ++n)
			{
				AssertTreeCorrect(outputList[n]);
			}
		}
	}
}