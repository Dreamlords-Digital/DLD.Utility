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
	/// Primarily intended for expressing damage ranges in games.
	/// Has optional validation methods to prevent Lower Limit from going above Upper Limit.
	/// </remarks>
	public struct IntRange : IEquatable<IntRange>
	{
		/// <summary>
		/// The inclusive lower limit to the range.
		/// </summary>
		public int LowerLimit;

		/// <summary>
		/// The inclusive upper limit to the range.
		/// </summary>
		public int UpperLimit;

		public IntRange(int lowerLimit, int upperLimit, bool allowSameValues = false)
		{
			LowerLimit = lowerLimit;
			UpperLimit = upperLimit;
			LowerLimit = ValidateLowerLimit(LowerLimit, allowSameValues);
			UpperLimit = ValidateUpperLimit(UpperLimit, allowSameValues);
		}

		public IntRange(int singleNumber)
		{
			LowerLimit = singleNumber;
			UpperLimit = singleNumber;
			LowerLimit = ValidateLowerLimit(LowerLimit, true);
			UpperLimit = ValidateUpperLimit(UpperLimit, true);
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
			LowerLimit = value.LowerLimit;
			UpperLimit = value.UpperLimit;

			if (validate)
			{
				LowerLimit = ValidateLowerLimit(LowerLimit, allowSameValues);
				UpperLimit = ValidateUpperLimit(UpperLimit, allowSameValues);
			}
		}

		/// <summary>
		/// Sets a limit that a value can reach.
		/// </summary>
		/// <param name="lowerLimit"></param>
		/// <param name="upperLimit"></param>
		/// <param name="validate">Do not allow Lower Limit to be higher than the Upper Limit,
		/// and do not allow Upper Limit to be lower than the Lower Limit.</param>
		/// <param name="allowSameValues">Allow Lower Limit to be same value as Upper Limit.</param>
		public void SetLimit(int lowerLimit, int upperLimit, bool validate = true, bool allowSameValues = false)
		{
			LowerLimit = lowerLimit;
			UpperLimit = upperLimit;

			if (validate)
			{
				LowerLimit = ValidateLowerLimit(lowerLimit, allowSameValues);
				UpperLimit = ValidateUpperLimit(upperLimit, allowSameValues);
			}
		}

		/// <summary>
		/// Sets the Upper Limit that a value can reach.
		/// </summary>
		/// <param name="newUpperLimit"></param>
		/// <param name="validate">Do not allow Upper Limit to become lower than the current Lower Limit.</param>
		/// <param name="allowSameValues">Allow Lower Limit to be same value as Upper Limit.</param>
		public void SetUpperLimit(int newUpperLimit, bool validate = true, bool allowSameValues = false)
		{
			UpperLimit = validate ? ValidateUpperLimit(newUpperLimit, allowSameValues) : newUpperLimit;
		}

		/// <summary>
		/// Sets the Lower Limit that a value can drop to.
		/// </summary>
		/// <param name="newLowerLimit"></param>
		/// <param name="validate">Do not allow Lower Limit to become higher than the current Upper Limit.</param>
		/// <param name="allowSameValues">Allow Lower Limit to be same value as Upper Limit.</param>
		public void SetLowerLimit(int newLowerLimit, bool validate = true, bool allowSameValues = false)
		{
			LowerLimit = validate ? ValidateLowerLimit(newLowerLimit, allowSameValues) : newLowerLimit;
		}

		/// <summary>
		/// Modify an upper limit by moving it via an offset from current upper limit.
		/// </summary>
		public void ModifyUpperLimit(int offsetToUpperLimit, bool allowSameValues = false)
		{
			UpperLimit = ValidateUpperLimit(UpperLimit + offsetToUpperLimit, allowSameValues);
		}

		/// <summary>
		/// Modify a lower limit by moving it via an offset from current lower limit.
		/// </summary>
		public void ModifyLowerLimit(int offsetToLowerLimit, bool allowSameValues = false)
		{
			LowerLimit = ValidateLowerLimit(LowerLimit + offsetToLowerLimit, allowSameValues);
		}

		int ValidateUpperLimit(int value, bool allowSameValues = false)
		{
			int newUpperLimit = value;

			if (allowSameValues)
			{
				if (newUpperLimit < LowerLimit)
				{
					newUpperLimit = LowerLimit;
				}
			}
			else
			{
				if (newUpperLimit <= LowerLimit)
				{
					newUpperLimit = LowerLimit + 1;
				}
			}


			return newUpperLimit;
		}

		int ValidateLowerLimit(int value, bool allowSameValues = false)
		{
			int newLowerLimit = value;

			if (allowSameValues)
			{
				if (newLowerLimit > UpperLimit)
				{
					newLowerLimit = UpperLimit;
				}
			}
			else
			{
				if (newLowerLimit >= UpperLimit)
				{
					newLowerLimit = UpperLimit - 1;
				}
			}

			return newLowerLimit;
		}

		public void DisallowNonPositiveValues(bool allowSameValues = false)
		{
			if (UpperLimit < 1)
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
					LowerLimit = 1;
					UpperLimit = 1;
				}
				else
				{
					LowerLimit = 1;
					UpperLimit = 2;
				}
			}
			else if (LowerLimit < 1)
			{
				LowerLimit = 1;
				if (UpperLimit == 1 && !allowSameValues)
				{
					UpperLimit = 2;
				}
			}
		}

		public bool IsWithinLimit(int value)
		{
			return value >= LowerLimit && value <= UpperLimit;
		}

		public bool IsOutsideLimit(int value)
		{
			return value < LowerLimit || value > UpperLimit;
		}

		public bool IsZero => LowerLimit == 0 && UpperLimit == 0;

		public bool IsLowerAndUpperLimitSame => LowerLimit == UpperLimit;

		public bool IsLowerAndUpperLimitSameAndPositive => LowerLimit == UpperLimit && LowerLimit > 0;

		/// <summary>
		/// Uses Unity's <see cref="UnityEngine.Random"/> to generate a random value within the range.
		/// </summary>
		public int Random()
		{
			return UnityEngine.Random.Range(LowerLimit, UpperLimit+1);
		}

		/// <summary>
		/// Uses a <see cref="System.Random"/> to generate a random value within the range.
		/// </summary>
		public int Random(System.Random random)
		{
			return random.Next(LowerLimit, UpperLimit+1);
		}

		public float Lerp(float t)
		{
			return Mathf.Lerp(LowerLimit, UpperLimit, t);
		}

		public float LerpUnclamped(float t)
		{
			return Mathf.LerpUnclamped(LowerLimit, UpperLimit, t);
		}

		public float InverseLerp(float v)
		{
			return Mathf.InverseLerp(LowerLimit, UpperLimit, v);
		}

		public float Clamp(float v)
		{
			return Mathf.Clamp(v, LowerLimit, UpperLimit);
		}

		public int Clamp(int v)
		{
			if (v < LowerLimit)
			{
				return LowerLimit;
			}

			if (v > UpperLimit)
			{
				return UpperLimit;
			}

			return v;
		}

		public static bool operator ==(IntRange a, IntRange b)
		{
			return (a.UpperLimit == b.UpperLimit) && (a.LowerLimit == b.LowerLimit);
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

			return UpperLimit == other.UpperLimit && LowerLimit == other.LowerLimit;
		}

		public bool Equals(IntRange other)
		{
			return LowerLimit == other.LowerLimit && UpperLimit == other.UpperLimit;
		}

		public override int GetHashCode()
		{
			int l = (LowerLimit << 16) | (LowerLimit >> 16);
			return UpperLimit ^ l;
		}

		public override string ToString()
		{
			return LowerLimit == UpperLimit
				? LowerLimit.ToString()
				: $"{LowerLimit.ToString()} to {UpperLimit.ToString()}";
		}

		public string ToString(string format)
		{
			return LowerLimit == UpperLimit
				? LowerLimit.ToString(format)
				: $"{LowerLimit.ToString(format)} to {UpperLimit.ToString(format)}";
		}

		public string ToString(int offset, string format = "N0")
		{
			return LowerLimit == UpperLimit
				? (LowerLimit + offset).ToString(format)
				: $"{(LowerLimit + offset).ToString(format)} to {(UpperLimit + offset).ToString(format)}";
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
			new IntRange(a.LowerLimit + b.LowerLimit, a.UpperLimit + b.UpperLimit, true);

		public static IntRange operator +(IntRange a, int offset) =>
			new IntRange(a.LowerLimit + offset, a.UpperLimit + offset, true);

		public static IntRange operator -(IntRange a, IntRange b) =>
			new IntRange(a.LowerLimit - b.LowerLimit, a.UpperLimit - b.UpperLimit, true);

		public static IntRange operator -(IntRange a, int offset) =>
			new IntRange(a.LowerLimit - offset, a.UpperLimit - offset, true);

#if DLD_UTILITY_UNITY_MATHS_AVAILABLE
		public static implicit operator IntRange(int2 i) => new(i.x, i.y);
		public static implicit operator int2(IntRange intRange) => new(intRange.LowerLimit, intRange.UpperLimit);
#endif
	}
}
