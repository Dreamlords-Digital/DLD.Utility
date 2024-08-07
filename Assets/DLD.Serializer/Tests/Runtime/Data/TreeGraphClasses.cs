// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;

namespace DLD.Serializer.Tests.TreeGraph
{
	[Serializable]
	public class BaseNode : ITextData
	{
		public virtual string MyType
		{
			get { return "Base"; }
		}

		// -----------------------------

		[Serialized("ID")]
		string _id;

		public string ID
		{
			get { return _id; }
		}

		public void SetID(string value)
		{
			_id = value;
		}

		// -----------------------------

		[Serialized("Children")]
		List<BaseNode> _children = new List<BaseNode>();

		public int Count
		{
			get
			{
				return _children != null ? _children.Count : 0;
			}
		}

		public BaseNode this[int idx]
		{
			get
			{
				return _children != null ? _children[idx] : null;
			}
		}

		public void Add(BaseNode newObject)
		{
			if (_children == null)
			{
				_children = new List<BaseNode>();
			}

			_children.Add(newObject);
		}

		// -----------------------------

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}


	[Serializable]
	public class Root : BaseNode
	{
		public override string MyType
		{
			get { return "Root"; }
		}
	}

	[Serializable]
	public class Composite : BaseNode
	{
		public override string MyType
		{
			get { return "Composite"; }
		}
	}

	[Serializable]
	public class Leaf1 : BaseNode
	{
		public override string MyType
		{
			get { return "Leaf1"; }
		}
	}

	[Serializable]
	public class Leaf2 : BaseNode
	{
		public override string MyType
		{
			get { return "Leaf2"; }
		}
	}
}