// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

namespace DLD.Utility
{
	public static class ByteUtil
	{
		/// <summary>
		/// Given an int that represents a bitmask, find the position of the first bit that is set to 1.
		/// </summary>
		/// <param name="n">int value that we're checking.</param>
		/// <returns>A value from 0 to 31.</returns>
		/// <remarks>
		/// If specified int is not power-of-two, we'll end up returning
		/// the position of the first bit that is set,
		/// ignoring the rest.
		/// </remarks>
		public static int FindBitPosition(this int n)
		{
			if (n == 0)
			{
				// value of 0, no bits set
				return 0;
			}

			int tryValue = 1;
			int position = 1;

			while ((tryValue & n) == 0 && position <= 32)
			{
				// try next power-of-two value
				tryValue = 1 << position++;
			}

			return position - 1;
		}

		public static void SetFlag(this ref byte byteToChange, int index, bool value)
		{
			if (index < 0 || index > 7)
			{
				return;
			}

			if (value)
			{
				byteToChange = (byte) (byteToChange | (1 << index));
			}
			else
			{
				byteToChange = (byte) (byteToChange & ~(1 << index));
			}
		}

		public static bool GetFlag(this byte byteToGet, int index)
		{
			if (index < 0 || index > 7)
			{
				return false;
			}

			return ((byteToGet & (1 << index)) != 0);
		}

		static char GetHexValue(int i) => i < 10 ? (char) (i + 48) : (char) (i - 10 + 65);

		public static string Byte16ToString(this byte[] hashBytes)
		{
			char[] hashChars = new char[32];
			int count = System.Math.Min(16, hashBytes.Length);
			for (int i = 0; i < count; ++i)
			{
				hashChars[i*2] = GetHexValue(hashBytes[i] / 16); // 0, 2, 4,
				hashChars[(i*2)+1] = GetHexValue(hashBytes[i] % 16); // 1, 3, 5,
			}

			return new string(hashChars);
		}

		public static bool AllNull(this byte[] me)
		{
			if (me == null)
			{
				return true;
			}

			if (me.Length == 0)
			{
				return true;
			}

			for (int n = 0, len = me.Length; n < len; ++n)
			{
				if (me[n] != 0)
				{
					return false;
				}
			}

			return true;
		}
	}
}