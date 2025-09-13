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
	///    Used to express an inclusive range for a float value.
	/// </summary>
	/// <remarks>
	///    Primarily intended for expressing damage (or energy cost, or cooldown rates, etc.) ranges in games.
	/// </remarks>
	[Serializable]
	[JetBrains.Annotations.PublicAPI]
	public struct FloatRange : IEquatable<FloatRange>
	{
		/// <summary>
		///    The inclusive lower limit to the range.
		/// </summary>
		public float Min;

		/// <summary>
		///    The inclusive upper limit to the range.
		/// </summary>
		public float Max;

		public FloatRange(float lower, float upper)
		{
			Min = lower;
			Max = upper;
		}

		public bool IsWithinRange(float value)
		{
			return value >= Min && value <= Max;
		}

		public bool IsOutsideRange(float value)
		{
			return value < Min || value > Max;
		}

		public bool IsZero => Mathf.Abs(Min) < float.Epsilon && Mathf.Abs(Max) < float.Epsilon;

		public bool IsMinAndMaxSame => Mathf.Approximately(Min, Max);

		public bool IsMinAndMaxSameAndPositive => Mathf.Approximately(Min, Max) && Min > 0;

		/// <summary>
		///    Uses Unity's <see cref="UnityEngine.Random"/> to generate a random value within the range.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Random()
		{
			return UnityEngine.Random.Range(Min, Max);
		}

		/// <summary>
		///    Uses a <see cref="System.Random"/> to generate a random value within the range.
		/// </summary>
		public float Random(System.Random random)
		{
			return Min + ((float)NextDoubleInclusive(random) * (Max - Min));
		}

		/// <summary>
		///    Returns a random floating-point number that is greater than or equal to 0.0,
		///    and less than or equal to 1.0.
		/// </summary>
		/// <remarks>
		///    From https://stackoverflow.com/a/66681312/1377948
		/// </remarks>
		public static double NextDoubleInclusive(System.Random random)
		{
			return (random.Next() * (1.0 / (int.MaxValue - 1)));
		}

		public static bool operator ==(FloatRange a, FloatRange b)
		{
			return Mathf.Approximately(a.Min, b.Min) && Mathf.Approximately(a.Max, b.Max);
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

			return Mathf.Approximately(Min, other.Min) && Mathf.Approximately(Max, other.Max);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Min, Max);
		}

		public bool Equals(FloatRange other)
		{
			return Mathf.Approximately(Min, other.Min) && Mathf.Approximately(Max, other.Max);
		}

		public override string ToString()
		{
			return ToString("0.###");
		}

		public string ToString(string format)
		{
			return Mathf.Approximately(Min, Max)
				? Min.ToString(format)
				: $"{Min.ToString(format)} to {Max.ToString(format)}";
		}

		public string ToString(int offset, string format = "0.###")
		{
			return Mathf.Approximately(Min, Max)
				? (Min + offset).ToString(format)
				: $"{(Min + offset).ToString(format)} to {(Max + offset).ToString(format)}";
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Lerp(float t)
		{
			return Mathf.Lerp(Min, Max, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float LerpUnclamped(float t)
		{
			return Mathf.LerpUnclamped(Min, Max, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float InverseLerp(float v)
		{
			return Mathf.InverseLerp(Min, Max, v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Clamp(float v)
		{
			return Mathf.Clamp(v, Min, Max);
		}

		public static FloatRange operator +(FloatRange a, FloatRange b) =>
			new FloatRange(a.Min + b.Min, a.Max + b.Max);

		public static FloatRange operator +(FloatRange a, int offset) =>
			new FloatRange(a.Min + offset, a.Max + offset);

		public static FloatRange operator +(FloatRange a, float offset) =>
			new FloatRange(a.Min + offset, a.Max + offset);

		public static FloatRange operator -(FloatRange a, FloatRange b) =>
			new FloatRange(a.Min - b.Min, a.Max - b.Max);

		public static FloatRange operator -(FloatRange a, int offset) =>
			new FloatRange(a.Min - offset, a.Max - offset);

		public static FloatRange operator -(FloatRange a, float offset) =>
			new FloatRange(a.Min - offset, a.Max - offset);

#if DLD_UTILITY_UNITY_MATHS_AVAILABLE
		public static implicit operator FloatRange(float2 f) => new(f.x, f.y);
		public static implicit operator float2(FloatRange floatRange) => new(floatRange.Min, floatRange.Max);
#endif
	}
}
