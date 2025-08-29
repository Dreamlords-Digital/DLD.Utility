// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
#if DLD_UTILITY_UNITY_MATHS_AVAILABLE
using Unity.Mathematics;
#endif

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
		public float LowerLimit;

		/// <summary>
		/// The inclusive upper limit to the range.
		/// </summary>
		public float UpperLimit;

		public FloatRange(float lower, float upper)
		{
			LowerLimit = lower;
			UpperLimit = upper;
		}

		public bool IsWithinLimit(float value)
		{
			return value >= LowerLimit && value <= UpperLimit;
		}

		public bool IsOutsideLimit(float value)
		{
			return value < LowerLimit || value > UpperLimit;
		}

		public bool IsZero => Mathf.Abs(LowerLimit) < float.Epsilon && Mathf.Abs(UpperLimit) < float.Epsilon;

		public bool IsLowerAndUpperLimitSame => Mathf.Approximately(LowerLimit, UpperLimit);

		public bool IsLowerAndUpperLimitSameAndPositive => Mathf.Approximately(LowerLimit, UpperLimit) && LowerLimit > 0;

		/// <summary>
		/// Uses Unity's <see cref="UnityEngine.Random"/> to generate a random value within the range.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Random()
		{
			return UnityEngine.Random.Range(LowerLimit, UpperLimit);
		}

		/// <summary>
		/// Uses a <see cref="System.Random"/> to generate a random value within the range.
		/// </summary>
		public float Random(System.Random random)
		{
			return LowerLimit + ((float)NextDoubleInclusive(random) * (UpperLimit - LowerLimit));
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
			return Mathf.Approximately(a.LowerLimit, b.LowerLimit) && Mathf.Approximately(a.UpperLimit, b.UpperLimit);
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

			return Mathf.Approximately(LowerLimit, other.LowerLimit) && Mathf.Approximately(UpperLimit, other.UpperLimit);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(LowerLimit, UpperLimit);
		}

		public bool Equals(FloatRange other)
		{
			return Mathf.Approximately(LowerLimit, other.LowerLimit) && Mathf.Approximately(UpperLimit, other.UpperLimit);
		}

		public string ToString(string format)
		{
			return Mathf.Approximately(LowerLimit, UpperLimit)
				? LowerLimit.ToString(format)
				: $"{LowerLimit.ToString(format)} to {UpperLimit.ToString(format)}";
		}

		public string ToString(int offset, string format = "N0")
		{
			return Mathf.Approximately(LowerLimit, UpperLimit)
				? (LowerLimit + offset).ToString(format)
				: $"{(LowerLimit + offset).ToString(format)} to {(UpperLimit + offset).ToString(format)}";
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Lerp(float t)
		{
			return Mathf.Lerp(LowerLimit, UpperLimit, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float LerpUnclamped(float t)
		{
			return Mathf.LerpUnclamped(LowerLimit, UpperLimit, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float InverseLerp(float v)
		{
			return Mathf.InverseLerp(LowerLimit, UpperLimit, v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Clamp(float v)
		{
			return Mathf.Clamp(v, LowerLimit, UpperLimit);
		}

		public static FloatRange operator +(FloatRange a, FloatRange b) =>
			new FloatRange(a.LowerLimit + b.LowerLimit, a.UpperLimit + b.UpperLimit);

		public static FloatRange operator +(FloatRange a, int offset) =>
			new FloatRange(a.LowerLimit + offset, a.UpperLimit + offset);

		public static FloatRange operator +(FloatRange a, float offset) =>
			new FloatRange(a.LowerLimit + offset, a.UpperLimit + offset);

		public static FloatRange operator -(FloatRange a, FloatRange b) =>
			new FloatRange(a.LowerLimit - b.LowerLimit, a.UpperLimit - b.UpperLimit);

		public static FloatRange operator -(FloatRange a, int offset) =>
			new FloatRange(a.LowerLimit - offset, a.UpperLimit - offset);

		public static FloatRange operator -(FloatRange a, float offset) =>
			new FloatRange(a.LowerLimit - offset, a.UpperLimit - offset);

#if DLD_UTILITY_UNITY_MATHS_AVAILABLE
		public static implicit operator FloatRange(float2 f) => new(f.x, f.y);
		public static implicit operator float2(FloatRange floatRange) => new(floatRange.LowerLimit, floatRange.UpperLimit);
#endif
	}
}
