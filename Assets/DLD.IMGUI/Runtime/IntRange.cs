// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using DLD.Serializer;

namespace DLD.Utility
{
	/// <summary>
	/// Used to express an inclusive range for an int value.
	/// </summary>
	/// <remarks>
	/// It has validation methods to prevent Lower Limit from going above Upper Limit.
	/// This validation is enabled by default but can be optionally disabled.
	/// </remarks>
	public struct IntRange
	{
		/// <inheritdoc cref="_lowerLimit"/>
		public int LowerLimit => _lowerLimit;

		/// <inheritdoc cref="_upperLimit"/>
		public int UpperLimit => _upperLimit;

		/// <summary>
		/// The inclusive lower limit to the range.
		/// </summary>
		[Serialized]
		[SerializedName("LowerLimit")]
		int _lowerLimit;

		/// <summary>
		/// The inclusive upper limit to the range.
		/// </summary>
		[Serialized]
		[SerializedName("UpperLimit")]
		int _upperLimit;

		public IntRange(int lowerLimit, int upperLimit, bool allowSameValues = false)
		{
			_lowerLimit = lowerLimit;
			_upperLimit = upperLimit;
			_lowerLimit = ValidateLowerLimit(_lowerLimit, allowSameValues);
			_upperLimit = ValidateUpperLimit(_upperLimit, allowSameValues);
		}

		public IntRange(int singleNumber)
		{
			_lowerLimit = singleNumber;
			_upperLimit = singleNumber;
			_lowerLimit = ValidateLowerLimit(_lowerLimit, true);
			_upperLimit = ValidateUpperLimit(_upperLimit, true);
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
			_lowerLimit = value._lowerLimit;
			_upperLimit = value._upperLimit;

			if (validate)
			{
				_lowerLimit = ValidateLowerLimit(_lowerLimit, allowSameValues);
				_upperLimit = ValidateUpperLimit(_upperLimit, allowSameValues);
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
			_lowerLimit = lowerLimit;
			_upperLimit = upperLimit;

			if (validate)
			{
				_lowerLimit = ValidateLowerLimit(lowerLimit, allowSameValues);
				_upperLimit = ValidateUpperLimit(upperLimit, allowSameValues);
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
			_upperLimit = validate ? ValidateUpperLimit(newUpperLimit, allowSameValues) : newUpperLimit;
		}

		/// <summary>
		/// Sets the Lower Limit that a value can drop to.
		/// </summary>
		/// <param name="newLowerLimit"></param>
		/// <param name="validate">Do not allow Lower Limit to become higher than the current Upper Limit.</param>
		/// <param name="allowSameValues">Allow Lower Limit to be same value as Upper Limit.</param>
		public void SetLowerLimit(int newLowerLimit, bool validate = true, bool allowSameValues = false)
		{
			_lowerLimit = validate ? ValidateLowerLimit(newLowerLimit, allowSameValues) : newLowerLimit;
		}

		/// <summary>
		/// Modify an upper limit by moving it via an offset from current upper limit.
		/// </summary>
		public void ModifyUpperLimit(int offsetToUpperLimit, bool allowSameValues = false)
		{
			_upperLimit = ValidateUpperLimit(_upperLimit + offsetToUpperLimit, allowSameValues);
		}

		/// <summary>
		/// Modify a lower limit by moving it via an offset from current lower limit.
		/// </summary>
		public void ModifyLowerLimit(int offsetToLowerLimit, bool allowSameValues = false)
		{
			_lowerLimit = ValidateLowerLimit(_lowerLimit + offsetToLowerLimit, allowSameValues);
		}

		int ValidateUpperLimit(int value, bool allowSameValues = false)
		{
			int newUpperLimit = value;

			if (allowSameValues)
			{
				if (newUpperLimit < _lowerLimit)
				{
					newUpperLimit = _lowerLimit;
				}
			}
			else
			{
				if (newUpperLimit <= _lowerLimit)
				{
					newUpperLimit = _lowerLimit + 1;
				}
			}


			return newUpperLimit;
		}

		int ValidateLowerLimit(int value, bool allowSameValues = false)
		{
			int newLowerLimit = value;

			if (allowSameValues)
			{
				if (newLowerLimit > _upperLimit)
				{
					newLowerLimit = _upperLimit;
				}
			}
			else
			{
				if (newLowerLimit >= _upperLimit)
				{
					newLowerLimit = _upperLimit - 1;
				}
			}

			return newLowerLimit;
		}

		public void DisallowNonPositiveValues(bool allowSameValues = false)
		{
			if (_upperLimit < 1)
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
					_lowerLimit = 1;
					_upperLimit = 1;
				}
				else
				{
					_lowerLimit = 1;
					_upperLimit = 2;
				}
			}
			else if (_lowerLimit < 1)
			{
				_lowerLimit = 1;
				if (_upperLimit == 1 && !allowSameValues)
				{
					_upperLimit = 2;
				}
			}
		}

		public bool IsWithinLimit(int value)
		{
			return value >= _lowerLimit && value <= _upperLimit;
		}

		public bool IsOutsideLimit(int value)
		{
			return value < _lowerLimit || value > _upperLimit;
		}

		public bool IsZero => _lowerLimit == 0 && _upperLimit == 0;

		public bool IsLowerAndUpperLimitSame => _lowerLimit == _upperLimit;

		public bool IsLowerAndUpperLimitSameAndPositive => _lowerLimit == _upperLimit && _lowerLimit > 0;

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
			if (!(obj is IntRange other))
			{
				return false;
			}

			return _upperLimit == other.UpperLimit && _lowerLimit == other.LowerLimit;
		}

		public override int GetHashCode()
		{
			var l = (_lowerLimit << 16) | (_lowerLimit >> 16);
			return _upperLimit ^ l;
			//const int N1 = 99999997;
			//return (_lowerLimit % N1) ^ _upperLimit;
		}

		public override string ToString()
		{
			return _lowerLimit == _upperLimit
				? _lowerLimit.ToString()
				: $"{_lowerLimit.ToString()} to {_upperLimit.ToString()}";
		}

		public string ToString(string format)
		{
			return _lowerLimit == _upperLimit
				? _lowerLimit.ToString(format)
				: $"{_lowerLimit.ToString(format)} to {_upperLimit.ToString(format)}";
		}

		public string ToString(int offset, string format = "N0")
		{
			return _lowerLimit == _upperLimit
				? (_lowerLimit + offset).ToString(format)
				: $"{(_lowerLimit + offset).ToString(format)} to {(_upperLimit + offset).ToString(format)}";
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
	}
}