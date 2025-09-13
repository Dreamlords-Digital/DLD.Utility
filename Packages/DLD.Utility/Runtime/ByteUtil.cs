// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Text;

namespace DLD.Utility
{

public static class ByteUtil
{
	/// <summary>
	///    Given an int that represents a bitmask, find the position of the first bit that is set to 1.
	/// </summary>
	/// <param name="n">int value that we're checking.</param>
	/// <returns>
	///    A value from 0 to 31. 0 means 1st bit.
	///    If no bits are set to 1 (passed value is 0), this returns -1.
	/// </returns>
	/// <remarks>
	///    If specified int is not power-of-two, we'll end up returning
	///    the position of the first (lowest value) bit that is set,
	///    ignoring the rest.
	/// </remarks>
	public static int FindBitIndex(this int n)
	{
		if (n == 0)
		{
			// value of 0, no bits set
			return -1;
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

	// =================================================================

	public static void ToggleFlag(this ref byte byteToChange, int index)
	{
		if (index < 0 || index > 7)
		{
			return;
		}

		if ((byteToChange & (1 << index)) == 0)
		{
			byteToChange = (byte)(byteToChange | (1 << index));
		}
		else
		{
			byteToChange = (byte)(byteToChange & ~(1 << index));
		}
	}

	public static void ToggleFlag(ref System.Enum flags, System.Enum flag)
	{
		int flagsValue = System.Convert.ToInt32(flags);
		int flagValue = System.Convert.ToInt32(flag);

		if ((flagsValue & flagValue) == 0)
		{
			flags = (System.Enum)System.Enum.ToObject(flags.GetType(), flagsValue | flagValue);
		}
		else
		{
			flags = (System.Enum)System.Enum.ToObject(flags.GetType(), flagsValue & (~flagValue));
		}
	}

	public static void ToggleFlag(this ref ulong flags, ulong flag)
	{
		if ((flags & flag) == 0) // if currently does not have value
		{
			flags |= flag;
		}
		else
		{
			flags &= (~flag);
		}
	}

	// =================================================================

	public static void SetFlag(this ref byte byteToChange, int index, bool value)
	{
		if (index < 0 || index > 7)
		{
			return;
		}

		if (value)
		{
			byteToChange = (byte)(byteToChange | (1 << index));
		}
		else
		{
			byteToChange = (byte)(byteToChange & ~(1 << index));
		}
	}

	public static void SetFlag(this ref int intToChange, int index, bool value)
	{
		if (index < 0 || index > 31)
		{
			return;
		}

		if (value)
		{
			intToChange = (intToChange | (1 << index));
		}
		else
		{
			intToChange = (intToChange & ~(1 << index));
		}
	}

	public static void SetFlag(this ref uint intToChange, int index, bool value)
	{
		if (index < 0 || index > 31)
		{
			return;
		}

		if (value)
		{
			intToChange = (intToChange | (uint)(1 << index));
		}
		else
		{
			intToChange = (intToChange & (uint)~(1 << index));
		}
	}

	// =================================================================

	public static bool GetFlag(this byte byteToGet, int index)
	{
		if (index < 0 || index > 7)
		{
			return false;
		}

		return ((byteToGet & (1 << index)) != 0);
	}

	public static bool GetFlag(this int intToGet, int index)
	{
		if (index < 0 || index > 31)
		{
			return false;
		}

		return ((intToGet & (1 << index)) != 0);
	}

	public static bool GetFlag(this ulong intToGet, int index)
	{
		if (index < 0 || index > 31)
		{
			return false;
		}

		return ((intToGet & (1u << index)) != 0);
	}

	public static bool HasFlag(this ulong intToGet, ulong flagToCheck)
	{
		return (intToGet & flagToCheck) == flagToCheck;
	}

	// =================================================================

	public static string ToStringFlags(this byte byteToGet)
	{
		StringBuilder s = new StringBuilder();
		for (int i = 7; i >= 0; --i)
		{
			if ((byteToGet & (1 << i)) != 0)
			{
				s.Append("1");
			}
			else
			{
				s.Append("0");
			}
		}

		return s.ToString();
	}

	public static System.Enum Clone(this System.Enum value)
	{
		return (System.Enum)System.Enum.ToObject(value.GetType(), System.Convert.ToInt32(value));
	}

	static char GetHexValue(int i) => i < 10 ? (char)(i + 48) : (char)(i - 10 + 65);

	public static string Byte16ToString(this byte[] hashBytes)
	{
		char[] hashChars = new char[32];
		int count = System.Math.Min(16, hashBytes.Length);
		for (int i = 0; i < count; ++i)
		{
			hashChars[i * 2] = GetHexValue(hashBytes[i] / 16); // 0, 2, 4,
			hashChars[(i * 2) + 1] = GetHexValue(hashBytes[i] % 16); // 1, 3, 5,
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
