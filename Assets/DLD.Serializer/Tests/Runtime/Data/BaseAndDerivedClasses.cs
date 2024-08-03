// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;

namespace DLD.Serializer.Tests
{
	[Serializable]
	public class BaseClass : ITextData
	{
		public virtual string MyType
		{
			get { return "Base"; }
		}

		// -----------------------------

		[Serialized]
		[SerializedName("ID")]
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

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}


	public class Derived1 : BaseClass
	{
		public const string DERIVED_1_TYPE = "Derived1";

		public override string MyType
		{
			get { return DERIVED_1_TYPE; }
		}

		// -----------------------------

		[Serialized]
		[SerializedName("MyField")]
		int _derived1Field;

		public int MyField
		{
			get { return _derived1Field; }
		}

		public void SetMyField(int value)
		{
			_derived1Field = value;
		}
	}

	public class Derived2 : BaseClass
	{
		public const string DERIVED_2_TYPE = "Derived2";

		public override string MyType
		{
			get { return DERIVED_2_TYPE; }
		}
	}

	public class Derived3 : BaseClass
	{
		public const string DERIVED_3_TYPE = "Derived3";

		public override string MyType
		{
			get { return DERIVED_3_TYPE; }
		}
	}



	[Serializable]
	public class BaseAndDerivedClassUser : ITextData
	{
		// -----------------------------

		[Serialized]
		[SerializedName("List")]
		readonly List<BaseClass> _list = new List<BaseClass>();

		public int Count
		{
			get { return _list.Count; }
		}

		public BaseClass this[int idx]
		{
			get { return _list[idx]; }
		}

		public void Add(BaseClass newObject)
		{
			_list.Add(newObject);
		}

		// -----------------------------

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}
}