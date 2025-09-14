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
		get
		{
			return "Base";
		}
	}

	// -----------------------------

	[Serialized("ID")]
	string _id;

	public string ID
	{
		get
		{
			return _id;
		}
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
	public const string Derived1Type = "Derived1";

	public override string MyType
	{
		get
		{
			return Derived1Type;
		}
	}

	// -----------------------------

	[Serialized("MyField")]
	int _derived1Field;

	public int MyField
	{
		get
		{
			return _derived1Field;
		}
	}

	public void SetMyField(int value)
	{
		_derived1Field = value;
	}
}

public class Derived2 : BaseClass
{
	public const string Derived2Type = "Derived2";

	public override string MyType
	{
		get
		{
			return Derived2Type;
		}
	}
}

public class Derived3 : BaseClass
{
	public const string Derived3Type = "Derived3";

	public override string MyType
	{
		get
		{
			return Derived3Type;
		}
	}
}


[Serializable]
public class BaseAndDerivedClassUser : ITextData
{
	// -----------------------------

	[Serialized("List")]
	readonly List<BaseClass> _list = new List<BaseClass>();

	public int Count
	{
		get
		{
			return _list.Count;
		}
	}

	public BaseClass this[int idx]
	{
		get
		{
			return _list[idx];
		}
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
