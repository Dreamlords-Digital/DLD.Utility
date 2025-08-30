// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEngine;
#if DLD_UTILITY_UNITY_MATHS_AVAILABLE
using Unity.Mathematics;
#endif

namespace DLD.Utility
{
	/// <summary>
	/// Used to express an inclusive range for an int value.
	/// </summary>
	/// <remarks>
	/// Primarily intended for expressing damage (or energy cost, or cooldown rates, etc.) ranges in games.
	/// Has optional validation methods to prevent Lower Limit from going above Upper Limit.
	/// </remarks>
	[Serializable]
	public struct IntRange : IEquatable<IntRange>
	{
		/// <summary>
		/// The inclusive lower limit to the range.
		/// </summary>
		public int Min;

		/// <summary>
		/// The inclusive upper limit to the range.
		/// </summary>
		public int Max;

		public IntRange(int min, int max, bool allowSameValues = false)
		{
			Min = min;
			Max = max;
			Min = ValidateMin(Min, allowSameValues);
			Max = ValidateMax(Max, allowSameValues);
		}

		public IntRange(int singleNumber)
		{
			Min = singleNumber;
			Max = singleNumber;
			Min = ValidateMin(Min, true);
			Max = ValidateMax(Max, true);
		}

		/// <summary>
		/// Sets a limit that a value can reach.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="validate">Do not allow Lower Limit to be higher than the Upper Limit,
		/// and do not allow Upper Limit to be lower than the Lower Limit.</param>
		/// <param name="allowSameValues">Allow Lower Limit to be same value as Upper Limit.</param>
		public void SetLimit(IntRange value, bool validate = true, bool allowSameValues = false)
		{
			Min = value.Min;
			Max = value.Max;

			if (validate)
			{
				Min = ValidateMin(Min, allowSameValues);
				Max = ValidateMax(Max, allowSameValues);
			}
		}

		/// <summary>
		/// Sets a limit that a value can reach.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="validate">Do not allow Lower Limit to be higher than the Upper Limit,
		/// and do not allow Upper Limit to be lower than the Lower Limit.</param>
		/// <param name="allowSameValues">Allow Lower Limit to be same value as Upper Limit.</param>
		public void SetLimit(int min, int max, bool validate = true, bool allowSameValues = false)
		{
			Min = min;
			Max = max;

			if (validate)
			{
				Min = ValidateMin(min, allowSameValues);
				Max = ValidateMax(max, allowSameValues);
			}
		}

		/// <summary>
		/// Sets the Upper Limit that a value can reach.
		/// </summary>
		/// <param name="newMax"></param>
		/// <param name="validate">Do not allow Upper Limit to become lower than the current Lower Limit.</param>
		/// <param name="allowSameValues">Allow Lower Limit to be same value as Upper Limit.</param>
		public void SetMax(int newMax, bool validate = true, bool allowSameValues = false)
		{
			Max = validate ? ValidateMax(newMax, allowSameValues) : newMax;
		}

		/// <summary>
		/// Sets the Lower Limit that a value can drop to.
		/// </summary>
		/// <param name="newMin"></param>
		/// <param name="validate">Do not allow Lower Limit to become higher than the current Upper Limit.</param>
		/// <param name="allowSameValues">Allow Lower Limit to be same value as Upper Limit.</param>
		public void SetMin(int newMin, bool validate = true, bool allowSameValues = false)
		{
			Min = validate ? ValidateMin(newMin, allowSameValues) : newMin;
		}

		/// <summary>
		/// Modify an upper limit by moving it via an offset from current upper limit.
		/// </summary>
		public void ModifyMax(int offsetToUpperLimit, bool allowSameValues = false)
		{
			Max = ValidateMax(Max + offsetToUpperLimit, allowSameValues);
		}

		/// <summary>
		/// Modify a lower limit by moving it via an offset from current lower limit.
		/// </summary>
		public void ModifyLowerLimit(int offsetToLowerLimit, bool allowSameValues = false)
		{
			Min = ValidateMin(Min + offsetToLowerLimit, allowSameValues);
		}

		int ValidateMax(int value, bool allowSameValues = false)
		{
			int newUpperLimit = value;

			if (allowSameValues)
			{
				if (newUpperLimit < Min)
				{
					newUpperLimit = Min;
				}
			}
			else
			{
				if (newUpperLimit <= Min)
				{
					newUpperLimit = Min + 1;
				}
			}


			return newUpperLimit;
		}

		int ValidateMin(int value, bool allowSameValues = false)
		{
			int newLowerLimit = value;

			if (allowSameValues)
			{
				if (newLowerLimit > Max)
				{
					newLowerLimit = Max;
				}
			}
			else
			{
				if (newLowerLimit >= Max)
				{
					newLowerLimit = Max - 1;
				}
			}

			return newLowerLimit;
		}

		public void DisallowNonPositiveValues(bool allowSameValues = false)
		{
			if (Max < 1)
			{
				// We're about to set Upper Limit to 1,
				// which necessitates setting the Lower Limit to 1 as well (1 to 1),
				// because Lower Limit can't be above Upper Limit.
				//
				// But if same values aren't allowed,
				// then the closest we can change the
				// values to is Lower Limit 1, and Upper Limit 2.

				if (allowSameValues)
				{
					Min = 1;
					Max = 1;
				}
				else
				{
					Min = 1;
					Max = 2;
				}
			}
			else if (Min < 1)
			{
				Min = 1;
				if (Max == 1 && !allowSameValues)
				{
					Max = 2;
				}
			}
		}

		public bool IsWithinRange(int value)
		{
			return value >= Min && value <= Max;
		}

		public bool IsOutsideRange(int value)
		{
			return value < Min || value > Max;
		}

		public bool IsZero => Min == 0 && Max == 0;

		public bool IsMinAndMaxSame => Min == Max;

		public bool IsMinAndMaxSameAndPositive => Min == Max && Min > 0;

		/// <summary>
		/// Uses Unity's <see cref="UnityEngine.Random"/> to generate a random value within the range.
		/// </summary>
		public int Random()
		{
			return UnityEngine.Random.Range(Min, Max+1);
		}

		/// <summary>
		/// Uses a <see cref="System.Random"/> to generate a random value within the range.
		/// </summary>
		public int Random(System.Random random)
		{
			return random.Next(Min, Max+1);
		}

		public float Lerp(float t)
		{
			return Mathf.Lerp(Min, Max, t);
		}

		public float LerpUnclamped(float t)
		{
			return Mathf.LerpUnclamped(Min, Max, t);
		}

		public float InverseLerp(float v)
		{
			return Mathf.InverseLerp(Min, Max, v);
		}

		public float Clamp(float v)
		{
			return Mathf.Clamp(v, Min, Max);
		}

		public int Clamp(int v)
		{
			if (v < Min)
			{
				return Min;
			}

			if (v > Max)
			{
				return Max;
			}

			return v;
		}

		public static bool operator ==(IntRange a, IntRange b)
		{
			return (a.Max == b.Max) && (a.Min == b.Min);
		}

		public static bool operator !=(IntRange a, IntRange b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj is not IntRange other)
			{
				return false;
			}

			return Max == other.Max && Min == other.Min;
		}

		public bool Equals(IntRange other)
		{
			return Min == other.Min && Max == other.Max;
		}

		public override int GetHashCode()
		{
			int l = (Min << 16) | (Min >> 16);
			return Max ^ l;
		}

		public override string ToString()
		{
			return Min == Max
				? Min.ToString()
				: $"{Min.ToString()} to {Max.ToString()}";
		}

		public string ToString(string format)
		{
			return Min == Max
				? Min.ToString(format)
				: $"{Min.ToString(format)} to {Max.ToString(format)}";
		}

		public string ToString(int offset, string format = "N0")
		{
			return Min == Max
				? (Min + offset).ToString(format)
				: $"{(Min + offset).ToString(format)} to {(Max + offset).ToString(format)}";
		}

		/// <summary>
		/// Range of 0 to 1
		/// </summary>
		public static IntRange MinValue => new IntRange(0, 1);

		/// <summary>
		/// Range of 0 to 0
		/// </summary>
		public static IntRange Zero => new IntRange(0, 0, true);

		/// <summary>
		/// Range of 1 to 1
		/// </summary>
		public static IntRange One => new IntRange(1, 1, true);

		public static IntRange operator +(IntRange a, IntRange b) =>
			new IntRange(a.Min + b.Min, a.Max + b.Max, true);

		public static IntRange operator +(IntRange a, int offset) =>
			new IntRange(a.Min + offset, a.Max + offset, true);

		public static IntRange operator -(IntRange a, IntRange b) =>
			new IntRange(a.Min - b.Min, a.Max - b.Max, true);

		public static IntRange operator -(IntRange a, int offset) =>
			new IntRange(a.Min - offset, a.Max - offset, true);

#if DLD_UTILITY_UNITY_MATHS_AVAILABLE
		public static implicit operator IntRange(int2 i) => new(i.x, i.y);
		public static implicit operator int2(IntRange intRange) => new(intRange.Min, intRange.Max);
#endif
	}
}
