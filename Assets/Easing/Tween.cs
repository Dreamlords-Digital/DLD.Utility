using UnityEngine;

namespace Easing
{
	public static class Tween
	{
		// --------------------------------------------------------------------------------------

		public static Vector3 ReturnCatmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			Vector3 a = 0.5f * (2f * p1);
			Vector3 b = 0.5f * (p2 - p0);
			Vector3 c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
			Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);

			Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);

			return pos;
		}

		public static float CLerp(float start, float end, float value)
		{
			const float min = 0.0f;
			const float max = 360.0f;
			float half = Mathf.Abs((max - min) * 0.5f);
			float result;
			float diff;
			if ((end - start) < -half)
			{
				diff = ((max - start) + end) * value;
				result = start + diff;
			}
			else if ((end - start) > half)
			{
				diff = -((max - end) + start) * value;
				result = start + diff;
			}
			else
				result = start + (end - start) * value;

			return result;
		}

		public static float Spring(float start, float end, float value)
		{
			value = Mathf.Clamp01(value);
			value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) *
			        (1f + (1.2f * (1f - value)));
			return start + (end - start) * value;
		}

		// --------------------------------------------------------------------------------------

		public static float EaseInQuad(float start, float end, float value)
		{
			end -= start;
			return end * value * value + start;
		}

		public static float EaseOutQuad(float start, float end, float value)
		{
			end -= start;
			return -end * value * (value - 2) + start;
		}

		public static float EaseInOutQuad(float start, float end, float value)
		{
			value /= .5f;
			end -= start;
			if (value < 1)
				return end * 0.5f * value * value + start;
			value--;
			return -end * 0.5f * (value * (value - 2) - 1) + start;
		}

		// ----------------------------------------------

		public static Vector2 EaseInQuad(Vector2 start, Vector2 end, float value)
		{
			Vector2 result;
			result.x = EaseInQuad(start.x, end.x, value);
			result.y = EaseInQuad(start.y, end.y, value);
			return result;
		}

		public static Vector2 EaseOutQuad(Vector2 start, Vector2 end, float value)
		{
			Vector2 result;
			result.x = EaseOutQuad(start.x, end.x, value);
			result.y = EaseOutQuad(start.y, end.y, value);
			return result;
		}

		public static Vector2 EaseInOutQuad(Vector2 start, Vector2 end, float value)
		{
			Vector2 result;
			result.x = EaseInOutQuad(start.x, end.x, value);
			result.y = EaseInOutQuad(start.y, end.y, value);
			return result;
		}

		// ----------------------------------------------

		public static Vector3 EaseInQuad(Vector3 start, Vector3 end, float value)
		{
			Vector3 result;
			result.x = EaseInQuad(start.x, end.x, value);
			result.y = EaseInQuad(start.y, end.y, value);
			result.z = EaseInQuad(start.z, end.z, value);
			return result;
		}

		public static Vector3 EaseOutQuad(Vector3 start, Vector3 end, float value)
		{
			Vector3 result;
			result.x = EaseOutQuad(start.x, end.x, value);
			result.y = EaseOutQuad(start.y, end.y, value);
			result.z = EaseOutQuad(start.z, end.z, value);
			return result;
		}

		public static Vector3 EaseInOutQuad(Vector3 start, Vector3 end, float value)
		{
			Vector3 result;
			result.x = EaseInOutQuad(start.x, end.x, value);
			result.y = EaseInOutQuad(start.y, end.y, value);
			result.z = EaseInOutQuad(start.z, end.z, value);
			return result;
		}

		// --------------------------------------------------------------------------------------

		public static float EaseInCubic(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value + start;
		}

		public static float EaseOutCubic(float start, float end, float value)
		{
			value--;
			end -= start;
			return end * (value * value * value + 1) + start;
		}

		public static float EaseInOutCubic(float start, float end, float value)
		{
			value /= .5f;
			end -= start;
			if (value < 1)
				return end * 0.5f * value * value * value + start;
			value -= 2;
			return end * 0.5f * (value * value * value + 2) + start;
		}

		// --------------------------------------------------------------------------------------

		public static float EaseInQuart(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value + start;
		}

		public static float EaseOutQuart(float start, float end, float value)
		{
			value--;
			end -= start;
			return -end * (value * value * value * value - 1) + start;
		}

		public static float EaseInOutQuart(float start, float end, float value)
		{
			value /= .5f;
			end -= start;
			if (value < 1)
				return end * 0.5f * value * value * value * value + start;
			value -= 2;
			return -end * 0.5f * (value * value * value * value - 2) + start;
		}

		// --------------------------------------------------------------------------------------

		public static float EaseInQuint(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value * value + start;
		}

		public static float EaseOutQuint(float start, float end, float value)
		{
			value--;
			end -= start;
			return end * (value * value * value * value * value + 1) + start;
		}

		public static float EaseInOutQuint(float start, float end, float value)
		{
			value /= .5f;
			end -= start;
			if (value < 1)
				return end * 0.5f * value * value * value * value * value + start;
			value -= 2;
			return end * 0.5f * (value * value * value * value * value + 2) + start;
		}

		// ----------------------------------------------

		public static Vector3 EaseOutQuint(Vector3 start, Vector3 end, float value)
		{
			Vector3 result;
			result.x = EaseOutQuint(start.x, end.x, value);
			result.y = EaseOutQuint(start.y, end.y, value);
			result.z = EaseOutQuint(start.z, end.z, value);
			return result;
		}

		public static Vector3 EaseInOutQuint(Vector3 start, Vector3 end, float value)
		{
			Vector3 result;
			result.x = EaseInOutQuint(start.x, end.x, value);
			result.y = EaseInOutQuint(start.y, end.y, value);
			result.z = EaseInOutQuint(start.z, end.z, value);
			return result;
		}

		// --------------------------------------------------------------------------------------

		public static float EaseInSine(float start, float end, float value)
		{
			end -= start;
			return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
		}

		public static float EaseOutSine(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
		}

		public static float EaseInOutSine(float start, float end, float value)
		{
			end -= start;
			return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
		}

		// ----------------------------------------------

		public static Vector2 EaseInSine(Vector2 start, Vector2 end, float value)
		{
			Vector2 result;
			result.x = EaseInSine(start.x, end.x, value);
			result.y = EaseInSine(start.y, end.y, value);
			return result;
		}

		public static Vector2 EaseOutSine(Vector2 start, Vector2 end, float value)
		{
			Vector2 result;
			result.x = EaseOutSine(start.x, end.x, value);
			result.y = EaseOutSine(start.y, end.y, value);
			return result;
		}

		public static Vector2 EaseInOutSine(Vector2 start, Vector2 end, float value)
		{
			Vector2 result;
			result.x = EaseInOutSine(start.x, end.x, value);
			result.y = EaseInOutSine(start.y, end.y, value);
			return result;
		}

		// ----------------------------------------------

		public static Vector3 EaseInSine(Vector3 start, Vector3 end, float value)
		{
			Vector3 result;
			result.x = EaseInSine(start.x, end.x, value);
			result.y = EaseInSine(start.y, end.y, value);
			result.z = EaseInSine(start.z, end.z, value);
			return result;
		}

		public static Vector3 EaseOutSine(Vector3 start, Vector3 end, float value)
		{
			Vector3 result;
			result.x = EaseOutSine(start.x, end.x, value);
			result.y = EaseOutSine(start.y, end.y, value);
			result.z = EaseOutSine(start.z, end.z, value);
			return result;
		}

		public static Vector3 EaseInOutSine(Vector3 start, Vector3 end, float value)
		{
			Vector3 result;
			result.x = EaseInOutSine(start.x, end.x, value);
			result.y = EaseInOutSine(start.y, end.y, value);
			result.z = EaseInOutSine(start.z, end.z, value);
			return result;
		}

		// --------------------------------------------------------------------------------------

		public static float EaseInExpo(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Pow(2, 10 * (value - 1)) + start;
		}

		public static float EaseOutExpo(float start, float end, float value)
		{
			end -= start;
			return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
		}

		public static float EaseInOutExpo(float start, float end, float value)
		{
			value /= .5f;
			end -= start;
			if (value < 1)
				return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
			value--;
			return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
		}

		// --------------------------------------------------------------------------------------

		public static float EaseInCirc(float start, float end, float value)
		{
			end -= start;
			return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
		}

		public static float EaseOutCirc(float start, float end, float value)
		{
			value--;
			end -= start;
			return end * Mathf.Sqrt(1 - value * value) + start;
		}

		public static float EaseInOutCirc(float start, float end, float value)
		{
			value /= .5f;
			end -= start;
			if (value < 1)
				return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
			value -= 2;
			return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
		}

		// --------------------------------------------------------------------------------------

		/* GFX47 MOD START */

		public static float EaseInBounce(float start, float end, float value)
		{
			end -= start;
			float d = 1f;
			return end - EaseOutBounce(0, end, d - value) + start;
		}

		/* GFX47 MOD END */

		/* GFX47 MOD START */
		// float bounce(float start, float end, float value){
		public static float EaseOutBounce(float start, float end, float value)
		{
			value /= 1f;
			end -= start;
			if (value < (1 / 2.75f))
			{
				return end * (7.5625f * value * value) + start;
			}
			else if (value < (2 / 2.75f))
			{
				value -= (1.5f / 2.75f);
				return end * (7.5625f * (value) * value + .75f) + start;
			}
			else if (value < (2.5 / 2.75))
			{
				value -= (2.25f / 2.75f);
				return end * (7.5625f * (value) * value + .9375f) + start;
			}
			else
			{
				value -= (2.625f / 2.75f);
				return end * (7.5625f * (value) * value + .984375f) + start;
			}
		}

		/* GFX47 MOD END */

		/* GFX47 MOD START */

		public static float EaseInOutBounce(float start, float end, float value)
		{
			end -= start;
			float d = 1f;
			if (value < d * 0.5f)
				return EaseInBounce(0, end, value * 2) * 0.5f + start;
			else
				return EaseOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
		}

		/* GFX47 MOD END */

		public static float EaseInBack(float start, float end, float value)
		{
			end -= start;
			value /= 1;
			float s = 1.70158f;
			return end * (value) * value * ((s + 1) * value - s) + start;
		}

		public static float EaseOutBack(float start, float end, float value)
		{
			float s = 1.70158f;
			end -= start;
			value = (value) - 1;
			return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
		}

		public static float EaseInOutBack(float start, float end, float value)
		{
			float s = 1.70158f;
			end -= start;
			value /= .5f;
			if ((value) < 1)
			{
				s *= (1.525f);
				return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
			}

			value -= 2;
			s *= (1.525f);
			return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
		}

		public static float Punch(float amplitude, float value)
		{
			if (value == 0)
			{
				return 0;
			}
			else if (Mathf.Approximately(value, 1))
			{
				return 0;
			}

			const float period = 1 * 0.3f;
			float s = period / (2 * Mathf.PI) * Mathf.Asin(0);
			return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
		}

		/* GFX47 MOD START */

		public static float EaseInElastic(float start, float end, float value)
		{
			end -= start;

			const float d = 1f;
			const float p = d * .3f;
			float s;
			float a = 0;

			if (value == 0)
				return start;

			if (Mathf.Approximately((value /= d), 1))
				return start + end;

			if (a == 0f || a < Mathf.Abs(end))
			{
				a = end;
				s = p / 4;
			}
			else
			{
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
		}

		/* GFX47 MOD END */

		/* GFX47 MOD START */
		// float elastic(float start, float end, float value){
		public static float EaseOutElastic(float start, float end, float value)
		{
			/* GFX47 MOD END */
			//Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
			end -= start;

			const float d = 1f;
			const float p = d * .3f;
			float s;
			float a = 0;

			if (value == 0)
				return start;

			if (Mathf.Approximately((value /= d), 1))
				return start + end;

			if (a == 0f || a < Mathf.Abs(end))
			{
				a = end;
				s = p * 0.25f;
			}
			else
			{
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
		}

		/* GFX47 MOD START */

		public static float EaseInOutElastic(float start, float end, float value)
		{
			end -= start;

			const float d = 1f;
			const float p = d * .3f;
			float s;
			float a = 0;

			if (value == 0)
				return start;

			if (Mathf.Approximately((value /= d * 0.5f), 2))
				return start + end;

			if (a == 0f || a < Mathf.Abs(end))
			{
				a = end;
				s = p / 4;
			}
			else
			{
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			if (value < 1)
				return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
			return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
		}

		/* GFX47 MOD END */
	}
}