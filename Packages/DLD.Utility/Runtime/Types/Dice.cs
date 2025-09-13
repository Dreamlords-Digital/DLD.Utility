// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;

namespace DLD.Utility
{
	[Serializable]
	[JetBrains.Annotations.PublicAPI]
	public struct Dice : IEquatable<Dice>
	{
		public int Quantity;
		public int NumberOfSides;
		public int Bonus;

		public Dice(int quantity, int numberOfSides, int bonus)
		{
			Quantity = quantity;
			NumberOfSides = numberOfSides;
			Bonus = bonus;
		}

		// =====================================================================================

		public bool HasValue => (Quantity > 0 && NumberOfSides > 0) || Bonus != 0;

		public bool IsPlural => (Quantity > 1 || NumberOfSides > 1 || Bonus > 1) ||
		                        (Quantity == 1 && NumberOfSides == 1 && Bonus == 1);

		public bool IsOne => (Quantity == 1 && NumberOfSides == 1 && Bonus == 0) ||
		                     (Quantity == 0 && NumberOfSides == 0 && Bonus == 1);

		public bool IsStaticNumberOnly => Quantity == 0 && NumberOfSides == 0 && Bonus != 0;
		public bool IsStaticPositiveNumberOnly => Quantity == 0 && NumberOfSides == 0 && Bonus > 0;
		public bool IsStaticNegativeNumberOnly => Quantity == 0 && NumberOfSides == 0 && Bonus < 0;
		public bool WillResultInNegativeNumber => Max < 0;
		public bool CanResultInNegativeNumber => Min < 0 || Max < 0;

		// =====================================================================================

		/// <summary>
		///    Uses Unity's <see cref="UnityEngine.Random"/> to roll the dice and generate a result.
		/// </summary>
		public int Roll()
		{
			int result = 0;
			for (int n = 0; n < Quantity; ++n)
			{
				result += UnityEngine.Random.Range(1, NumberOfSides+1);
			}
			return result + Bonus;
		}

		/// <summary>
		///    Uses a <see cref="System.Random"/> to roll the dice and generate a result.
		/// </summary>
		public int Roll(Random random)
		{
			int result = 0;
			for (int n = 0; n < Quantity; ++n)
			{
				result += random.Next(NumberOfSides+1);
			}
			return result + Bonus;
		}

		// =====================================================================================

		/// <summary>
		///    Get the minimum possible result of the dice roll.
		/// </summary>
		public int Min => Quantity + Bonus;

		/// <summary>
		///    Get the max possible result of the dice roll.
		/// </summary>
		public int Max => GetMax(Quantity, NumberOfSides, Bonus);

		/// <summary>
		///    Get the max possible result of a dice roll.
		/// </summary>
		/// <param name="quantity"></param>
		/// <param name="numberOfSides"></param>
		/// <param name="bonus"></param>
		/// <returns></returns>
		public static int GetMax(int quantity, int numberOfSides, int bonus = 0)
		{
			return (quantity * numberOfSides) + bonus;
		}

		// =====================================================================================

		public override string ToString()
		{
			return ToString(Quantity, NumberOfSides, Bonus);
		}

		public static string ToString(int quantity, int numberOfSides, int bonus = 0)
		{
			if (quantity == 1 && numberOfSides == 1)
			{
				if (bonus == 0)
				{
					// 1d1
					return "1";
				}
				else
				{
					// with bonus
					return (1+bonus).ToString();
				}
			}

			if (quantity > 0 && numberOfSides > 0)
			{
				if (bonus == 0)
				{
					// no bonus value (example: "2d6")
					return $"{quantity}d{numberOfSides}";
				}
				else
				{
					// with bonus (example: "2d6+3", or "2d6-3")
					return $"{quantity}d{numberOfSides}{bonus:+0;-#}";
				}
			}

			// bonus value only, no dice roll
			if (bonus != 0)
			{
				return bonus.ToString();
			}

			// note: negative value in dice roll ignored, treat it as zero
			return "0";
		}

		// =====================================================================================

		public override bool Equals(object obj)
		{
			if (obj is not Dice other)
			{
				return false;
			}

			return Equals(other);
		}

		public bool Equals(Dice other)
		{
			return Quantity == other.Quantity && NumberOfSides == other.NumberOfSides && Bonus == other.Bonus;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Quantity, NumberOfSides, Bonus);
		}

		// =====================================================================================
	}
}
