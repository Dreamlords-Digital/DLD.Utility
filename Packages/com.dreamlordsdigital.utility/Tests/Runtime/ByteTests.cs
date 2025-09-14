// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using NUnit.Framework;

namespace DLD.Utility.Tests
{

[TestFixture]
public class ByteTests
{
	[Test(Description = "FindBitIndex returns correct value.")]
	public void FindBitIndex_Works()
	{
		const int NoBitSet = 0;
		Assert.AreEqual(-1, NoBitSet.FindBitIndex());

		const int FirstBitSet = 0b00000001;
		Assert.AreEqual(0, FirstBitSet.FindBitIndex());

		const int SecondBitSet = 0b00000010;
		Assert.AreEqual(1, SecondBitSet.FindBitIndex());

		const int ThirdBitSet = 0b00000100;
		Assert.AreEqual(2, ThirdBitSet.FindBitIndex());

		const int LastBit = 1 << 31;
		Assert.AreEqual(31, LastBit.FindBitIndex());
	}

	[Test(Description = "FindBitIndex returns correct value.")]
	public void FindBitIndex_OnNonPowerOfTwo_ReturnsFirstBitSet()
	{
		const int Bit1And3Set = 0b00000101;
		Assert.AreEqual(0, Bit1And3Set.FindBitIndex());

		const int Bit5AndAboveSet = 0b01110000;
		Assert.AreEqual(4, Bit5AndAboveSet.FindBitIndex());
	}

	[Test(Description = "GetFlag returns correct value for 8-bit integer (byte).")]
	public void GetFlag_For8BitInt_Works()
	{
		const byte Bit1And3Set = 0b00000101;
		Assert.AreEqual(true, Bit1And3Set.GetFlag(0));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(1));
		Assert.AreEqual(true, Bit1And3Set.GetFlag(2));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(3));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(4));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(5));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(6));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(7));

		const byte LastBit = 0b10000000;
		Assert.AreEqual(false, LastBit.GetFlag(0));
		Assert.AreEqual(false, LastBit.GetFlag(1));
		Assert.AreEqual(false, LastBit.GetFlag(2));
		Assert.AreEqual(false, LastBit.GetFlag(3));
		Assert.AreEqual(false, LastBit.GetFlag(4));
		Assert.AreEqual(false, LastBit.GetFlag(5));
		Assert.AreEqual(false, LastBit.GetFlag(6));
		Assert.AreEqual(true, LastBit.GetFlag(7));
	}

	[Test(Description = "SetFlag returns correct value for unsigned 8-bit integer (byte).")]
	public void SetFlag_For8BitInt_Works()
	{
		byte testValue = 0b00000101;
		Assert.AreEqual(5, testValue);

		testValue.SetFlag(0, false); // now 0b00000100
		Assert.AreEqual(4, testValue);

		testValue.SetFlag(7, true); // now 0b10000100
		Assert.AreEqual(132, testValue);
	}

	[Test(Description = "GetFlag returns correct value for 32-bit integer (int).")]
	public void GetFlag_For32BitInt_Works()
	{
		const int Bit1And3Set = 0b0000101;
		Assert.AreEqual(true, Bit1And3Set.GetFlag(0));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(1));
		Assert.AreEqual(true, Bit1And3Set.GetFlag(2));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(3));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(4));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(5));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(6));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(7));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(8));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(9));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(10));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(11));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(12));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(13));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(14));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(15));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(16));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(17));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(18));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(19));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(20));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(21));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(22));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(23));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(24));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(25));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(26));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(27));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(28));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(29));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(30));
		Assert.AreEqual(false, Bit1And3Set.GetFlag(31));

		const int LastBit = 1 << 31;
		Assert.AreEqual(false, LastBit.GetFlag(0));
		Assert.AreEqual(false, LastBit.GetFlag(1));
		Assert.AreEqual(false, LastBit.GetFlag(2));
		Assert.AreEqual(false, LastBit.GetFlag(3));
		Assert.AreEqual(false, LastBit.GetFlag(4));
		Assert.AreEqual(false, LastBit.GetFlag(5));
		Assert.AreEqual(false, LastBit.GetFlag(6));
		Assert.AreEqual(false, LastBit.GetFlag(7));
		Assert.AreEqual(false, LastBit.GetFlag(8));
		Assert.AreEqual(false, LastBit.GetFlag(9));
		Assert.AreEqual(false, LastBit.GetFlag(10));
		Assert.AreEqual(false, LastBit.GetFlag(11));
		Assert.AreEqual(false, LastBit.GetFlag(12));
		Assert.AreEqual(false, LastBit.GetFlag(13));
		Assert.AreEqual(false, LastBit.GetFlag(14));
		Assert.AreEqual(false, LastBit.GetFlag(15));
		Assert.AreEqual(false, LastBit.GetFlag(16));
		Assert.AreEqual(false, LastBit.GetFlag(17));
		Assert.AreEqual(false, LastBit.GetFlag(18));
		Assert.AreEqual(false, LastBit.GetFlag(19));
		Assert.AreEqual(false, LastBit.GetFlag(20));
		Assert.AreEqual(false, LastBit.GetFlag(21));
		Assert.AreEqual(false, LastBit.GetFlag(22));
		Assert.AreEqual(false, LastBit.GetFlag(23));
		Assert.AreEqual(false, LastBit.GetFlag(24));
		Assert.AreEqual(false, LastBit.GetFlag(25));
		Assert.AreEqual(false, LastBit.GetFlag(26));
		Assert.AreEqual(false, LastBit.GetFlag(27));
		Assert.AreEqual(false, LastBit.GetFlag(28));
		Assert.AreEqual(false, LastBit.GetFlag(29));
		Assert.AreEqual(false, LastBit.GetFlag(30));
		Assert.AreEqual(true, LastBit.GetFlag(31));
	}

	[Test(Description = "SetFlag returns correct value for signed 32-bit integer (int).")]
	public void SetFlag_For32BitInt_Works()
	{
		// 1
		// 4
		// 256
		// 1024
		// 65536
		// 262144
		int testValue = 0b00000000000001010000010100000101;
		Assert.AreEqual(328965, testValue);

		testValue.SetFlag(0, false);
		Assert.AreEqual(328964, testValue);

		testValue.SetFlag(30, true);
		Assert.AreEqual(1074070788, testValue);
	}

	[Test(Description = "SetFlag returns correct value for unsigned 32-bit integer (uint).")]
	public void SetFlag_For32BitUnsignedInt_Works()
	{
		// 1
		// 4
		// 256
		// 1024
		// 65536
		// 262144
		uint testValue = 0b00000000000001010000010100000101;
		Assert.AreEqual(328965U, testValue);

		testValue.SetFlag(0, false);
		Assert.AreEqual(328964U, testValue);

		testValue.SetFlag(30, true);
		Assert.AreEqual(1074070788U, testValue);

		testValue.SetFlag(31, true);
		Assert.AreEqual(3221554436U, testValue);
	}

	enum TestEnum : byte
	{
		None = 0,
		First = 1,
		Second = 2,
		Third = 3,
	}

	[Test(Description = "Clone for a System.Enum returns a new instance of a System.Enum")]
	public void Clone_ForEnum_ReturnsNewInstance()
	{
		System.Enum a = TestEnum.First;
		System.Enum b = TestEnum.First;

		// two different instances, despite having same enum value
		Assert.IsFalse(ReferenceEquals(a, b));
		Assert.IsTrue(a.Equals(b));

		// assigned by reference
		System.Enum fromA = a;
		Assert.IsTrue(ReferenceEquals(a, fromA));
		Assert.IsTrue(a.Equals(fromA));

		// assign by copy using Clone()
		System.Enum copyA = a.Clone();
		Assert.IsFalse(ReferenceEquals(a, copyA));
		Assert.IsTrue(a.Equals(copyA));
	}
}

}
