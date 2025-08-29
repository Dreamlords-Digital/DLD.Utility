// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DLD.Utility
{
	/// <summary>
	/// Used to express an inclusive range for a float value.
	/// </summary>
	/// <remarks>
	/// Primarily intended for expressing damage ranges in games.
	/// </remarks>
	[Serializable]
	public struct FloatRange : IEquatable<FloatRange>
	{
		/// <summary>
		/// The inclusive lower limit to the range.
		/// </summary>
		public float Start;

		/// <summary>
		/// The inclusive upper limit to the range.
		/// </summary>
		public float End;

		public FloatRange(float start, float end)
		{
			Start = start;
			End = end;
		}

		public bool IsWithinLimit(float value)
		{
			return value >= Start && value <= End;
		}

		public bool IsOutsideLimit(float value)
		{
			return value < Start || value > End;
		}

		public bool IsZero => Mathf.Abs(Start) < float.Epsilon && Mathf.Abs(End) < float.Epsilon;

		public bool IsLowerAndUpperLimitSame => Mathf.Approximately(Start, End);

		public bool IsLowerAndUpperLimitSameAndPositive => Mathf.Approximately(Start, End) && Start > 0;

		/// <summary>
		/// Uses Unity's <see cref="UnityEngine.Random"/> to generate a random value within the range.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Random()
		{
			return UnityEngine.Random.Range(Start, End);
		}

		/// <summary>
		/// Uses a <see cref="System.Random"/> to generate a random value within the range.
		/// </summary>
		public float Random(System.Random random)
		{
			return Start + ((float)NextDoubleInclusive(random) * (End - Start));
		}

		/// <summary>Returns a random floating-point number that is greater than or equal to 0.0,
		/// and less than or equal to 1.0.</summary>
		/// <remarks>
		/// From https://stackoverflow.com/a/66681312/1377948
		/// </remarks>
		public static double NextDoubleInclusive(System.Random random)
		{
			return (random.Next() * (1.0 / (int.MaxValue - 1)));
		}

		public static bool operator ==(FloatRange a, FloatRange b)
		{
			return Mathf.Approximately(a.Start, b.Start) && Mathf.Approximately(a.End, b.End);
		}

		public static bool operator !=(FloatRange a, FloatRange b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj is not FloatRange other)
			{
				return false;
			}

			return Mathf.Approximately(Start, other.Start) && Mathf.Approximately(End, other.End);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Start, End);
		}

		public bool Equals(FloatRange other)
		{
			return Mathf.Approximately(Start, other.Start) && Mathf.Approximately(End, other.End);
		}

		public string ToString(string format)
		{
			return Mathf.Approximately(Start, End)
				? Start.ToString(format)
				: $"{Start.ToString(format)} to {End.ToString(format)}";
		}

		public string ToString(int offset, string format = "N0")
		{
			return Mathf.Approximately(Start, End)
				? (Start + offset).ToString(format)
				: $"{(Start + offset).ToString(format)} to {(End + offset).ToString(format)}";
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Lerp(float t)
		{
			return Mathf.Lerp(Start, End, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float LerpUnclamped(float t)
		{
			return Mathf.LerpUnclamped(Start, End, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float InverseLerp(float v)
		{
			return Mathf.InverseLerp(Start, End, v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Clamp(float v)
		{
			return Mathf.Clamp(v, Start, End);
		}

		public static FloatRange operator +(FloatRange a, FloatRange b) =>
			new FloatRange(a.Start + b.Start, a.End + b.End);

		public static FloatRange operator +(FloatRange a, int offset) =>
			new FloatRange(a.Start + offset, a.End + offset);

		public static FloatRange operator +(FloatRange a, float offset) =>
			new FloatRange(a.Start + offset, a.End + offset);

		public static FloatRange operator -(FloatRange a, FloatRange b) =>
			new FloatRange(a.Start - b.Start, a.End - b.End);

		public static FloatRange operator -(FloatRange a, int offset) =>
			new FloatRange(a.Start - offset, a.End - offset);

		public static FloatRange operator -(FloatRange a, float offset) =>
			new FloatRange(a.Start - offset, a.End - offset);
	}
}
