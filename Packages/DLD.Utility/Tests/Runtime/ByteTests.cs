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
			const int NO_BIT_SET = 0;
			Assert.AreEqual(-1, NO_BIT_SET.FindBitIndex());

			const int FIRST_BIT_SET = 0b00000001;
			Assert.AreEqual(0, FIRST_BIT_SET.FindBitIndex());

			const int SECOND_BIT_SET = 0b00000010;
			Assert.AreEqual(1, SECOND_BIT_SET.FindBitIndex());

			const int THIRD_BIT_SET = 0b00000100;
			Assert.AreEqual(2, THIRD_BIT_SET.FindBitIndex());

			const int LAST_BIT = 1 << 31;
			Assert.AreEqual(31, LAST_BIT.FindBitIndex());
		}

		[Test(Description = "FindBitIndex returns correct value.")]
		public void FindBitIndex_OnNonPowerOfTwo_ReturnsFirstBitSet()
		{
			const int BIT_1_AND_3_SET = 0b00000101;
			Assert.AreEqual(0, BIT_1_AND_3_SET.FindBitIndex());

			const int BIT_5_AND_ABOVE_SET = 0b01110000;
			Assert.AreEqual(4, BIT_5_AND_ABOVE_SET.FindBitIndex());
		}

		[Test(Description = "GetFlag returns correct value for 8-bit integer (byte).")]
		public void GetFlag_For8BitInt_Works()
		{
			const byte BIT_1_AND_3_SET = 0b00000101;
			Assert.AreEqual(true, BIT_1_AND_3_SET.GetFlag(0));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(1));
			Assert.AreEqual(true, BIT_1_AND_3_SET.GetFlag(2));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(3));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(4));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(5));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(6));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(7));

			const byte LAST_BIT = 0b10000000;
			Assert.AreEqual(false, LAST_BIT.GetFlag(0));
			Assert.AreEqual(false, LAST_BIT.GetFlag(1));
			Assert.AreEqual(false, LAST_BIT.GetFlag(2));
			Assert.AreEqual(false, LAST_BIT.GetFlag(3));
			Assert.AreEqual(false, LAST_BIT.GetFlag(4));
			Assert.AreEqual(false, LAST_BIT.GetFlag(5));
			Assert.AreEqual(false, LAST_BIT.GetFlag(6));
			Assert.AreEqual(true, LAST_BIT.GetFlag(7));
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
			const int BIT_1_AND_3_SET = 0b0000101;
			Assert.AreEqual(true, BIT_1_AND_3_SET.GetFlag(0));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(1));
			Assert.AreEqual(true, BIT_1_AND_3_SET.GetFlag(2));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(3));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(4));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(5));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(6));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(7));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(8));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(9));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(10));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(11));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(12));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(13));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(14));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(15));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(16));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(17));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(18));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(19));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(20));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(21));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(22));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(23));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(24));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(25));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(26));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(27));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(28));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(29));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(30));
			Assert.AreEqual(false, BIT_1_AND_3_SET.GetFlag(31));

			const int LAST_BIT = 1 << 31;
			Assert.AreEqual(false, LAST_BIT.GetFlag(0));
			Assert.AreEqual(false, LAST_BIT.GetFlag(1));
			Assert.AreEqual(false, LAST_BIT.GetFlag(2));
			Assert.AreEqual(false, LAST_BIT.GetFlag(3));
			Assert.AreEqual(false, LAST_BIT.GetFlag(4));
			Assert.AreEqual(false, LAST_BIT.GetFlag(5));
			Assert.AreEqual(false, LAST_BIT.GetFlag(6));
			Assert.AreEqual(false, LAST_BIT.GetFlag(7));
			Assert.AreEqual(false, LAST_BIT.GetFlag(8));
			Assert.AreEqual(false, LAST_BIT.GetFlag(9));
			Assert.AreEqual(false, LAST_BIT.GetFlag(10));
			Assert.AreEqual(false, LAST_BIT.GetFlag(11));
			Assert.AreEqual(false, LAST_BIT.GetFlag(12));
			Assert.AreEqual(false, LAST_BIT.GetFlag(13));
			Assert.AreEqual(false, LAST_BIT.GetFlag(14));
			Assert.AreEqual(false, LAST_BIT.GetFlag(15));
			Assert.AreEqual(false, LAST_BIT.GetFlag(16));
			Assert.AreEqual(false, LAST_BIT.GetFlag(17));
			Assert.AreEqual(false, LAST_BIT.GetFlag(18));
			Assert.AreEqual(false, LAST_BIT.GetFlag(19));
			Assert.AreEqual(false, LAST_BIT.GetFlag(20));
			Assert.AreEqual(false, LAST_BIT.GetFlag(21));
			Assert.AreEqual(false, LAST_BIT.GetFlag(22));
			Assert.AreEqual(false, LAST_BIT.GetFlag(23));
			Assert.AreEqual(false, LAST_BIT.GetFlag(24));
			Assert.AreEqual(false, LAST_BIT.GetFlag(25));
			Assert.AreEqual(false, LAST_BIT.GetFlag(26));
			Assert.AreEqual(false, LAST_BIT.GetFlag(27));
			Assert.AreEqual(false, LAST_BIT.GetFlag(28));
			Assert.AreEqual(false, LAST_BIT.GetFlag(29));
			Assert.AreEqual(false, LAST_BIT.GetFlag(30));
			Assert.AreEqual(true, LAST_BIT.GetFlag(31));
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
	}
}