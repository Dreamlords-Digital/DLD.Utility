// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;

namespace DLD.Utility
{
	public enum DerivativeType
	{
		ErrorRateOfChange,
		ValueRateOfChange,
	}

	public static class MathUtil
	{
		// Thanks to Rory Driscoll
		// http://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
		/// <summary>
		///    Creates dampened motion between a and b that is framerate independent.
		/// </summary>
		/// <param name="a">Initial parameter</param>
		/// <param name="b">Target parameter</param>
		/// <param name="lambda">Smoothing factor</param>
		/// <param name="dt">Time since last damp call</param>
		/// <returns></returns>
		public static Quaternion DampS(Quaternion a, Quaternion b, float lambda, float dt)
		{
			return Quaternion.Slerp(a, b, 1 - Mathf.Exp(-lambda * dt));
		}

		public static float DampS(float a, float b, float lambda, float dt)
		{
			return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
		}

		public static Vector3 DampS(Vector3 a, Vector3 b, float lambda, float dt)
		{
			return Vector3.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
		}

		public static Vector3 DirectionDampS(Vector3 a, Vector3 b, float lambda, float dt)
		{
			return Vector3.Slerp(a, b, 1 - Mathf.Exp(-lambda * dt));
		}

		public static Vector3 Predict(Vector3 projectilePos, Vector3 targetPos, Vector3 targetLastPos, float projectileSpeed)
		{
			// Target velocity
			Vector3 targetVelocity = (targetPos - targetLastPos) / Time.deltaTime;

			// Time to reach the target
			float flyTime = GetTimeUntilProjectileImpact(targetPos - projectilePos, targetVelocity, projectileSpeed);

			if (flyTime > 0)
			{
				return targetPos + flyTime * targetVelocity;
			}

			return targetPos;
		}

		static float GetTimeUntilProjectileImpact(Vector3 dist, Vector3 targetVelocity, float projectileSpeed)
		{
			float a = Vector3.Dot(targetVelocity, targetVelocity) - (projectileSpeed * projectileSpeed);
			float b = 2.0f * Vector3.Dot(targetVelocity, dist);
			float c = Vector3.Dot(dist, dist);

			float det = b * b - 4 * a * c;

			if (det > 0)
			{
				return 2 * c / (Mathf.Sqrt(det) - b);
			}

			return -1;
		}

		/// <summary>
		/// Projects a vector onto a plane. The output is not normalized.
		/// </summary>
		/// <param name="planeNormal"></param>
		/// <param name="vector"></param>
		/// <remarks>Code is from Forge3D Sci-fi Effects asset.</remarks>
		/// <returns></returns>
		public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
		{
			return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
		}

		public static float UpdatePID(float pFactor, float iFactor, float dFactor,
			float currentValue, float targetValue, float deltaTime,
			DerivativeType dType, float integralSaturation, float outputMin, float outputMax,
			ref float lastValue, ref bool derivativeInitialized, ref float storedIntegration)
		{
			// ----------------------------------

			float error = targetValue - currentValue;
			float p = error * pFactor;

			// ----------------------------------

			storedIntegration =
				Mathf.Clamp(storedIntegration + error * deltaTime, -integralSaturation, integralSaturation);
			float i = storedIntegration * iFactor;

			// ----------------------------------

			float dValue;
			if (derivativeInitialized)
			{
				switch (dType)
				{
					case DerivativeType.ErrorRateOfChange:
					{
						dValue = (error - lastValue) / deltaTime;
						lastValue = error;
						break;
					}
					default: // default is DerivativeType.ValueRateOfChange
					{
						dValue = -(currentValue - lastValue) / deltaTime;
						lastValue = currentValue;
						break;
					}
				}
			}
			else
			{
				dValue = 0;
				derivativeInitialized = true;
			}

			float d = dValue * dFactor;

			// ----------------------------------

			return Mathf.Clamp(p + i + d, outputMin, outputMax);
		}

		/// <summary>
		/// Given coordinates that represent a 2d direction whose length is at most 1.0,
		/// this will convert the values so that a direction pointing to
		/// top-right at its maximum, meaning (0.707, 0.707), becomes (1, 1).
		/// It will allow the X and Y axes to be able to reach the full value of 1.0 independently of each other.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="xMin"></param>
		/// <param name="xMax"></param>
		/// <param name="yMin"></param>
		/// <param name="yMax"></param>
		/// <remarks>From http://mathproofs.blogspot.com/2005/07/mapping-square-to-circle.html?showComment=1318897124368#c7349449939503611348</remarks>
		/// <returns></returns>
		public static Vector2 RectangularToRadial(float x, float y, float xMin = -1, float xMax = 1, float yMin = -1, float yMax = 1)
		{
			Vector2 r;

			bool xTooSmall = Mathf.Abs(x) < float.Epsilon;
			bool yTooSmall = Mathf.Abs(y) < float.Epsilon;

			float t;
			if (xTooSmall && yTooSmall)
			{
				t = 1;
			}
			else
			{
				float x2 = x * x;
				float y2 = y * y;
				t = Mathf.Sqrt((x2 + y2) / Mathf.Max(x2, y2));
			}

			r.x = Mathf.Clamp(x * t, xMin, xMax);
			r.y = Mathf.Clamp(y * t, yMin, yMax);

			return r;
		}

		/// <summary>
		/// Check if point is inside an ellipse.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="ellipsePos"></param>
		/// <param name="ellipseWidth"></param>
		/// <param name="ellipseHeight"></param>
		/// <remarks>From https://math.stackexchange.com/a/76463</remarks>
		/// <returns></returns>
		public static bool CheckIfInsideEllipse(Vector2 point, Vector2 ellipsePos, float ellipseWidth, float ellipseHeight)
		{
			float xh = point.x - ellipsePos.x;
			float yk = point.y - ellipsePos.y;
			float xh2 = xh * xh;
			float yk2 = yk * yk;

			float w2 = ellipseWidth * ellipseWidth;
			float h2 = ellipseHeight * ellipseHeight;

			return (xh2 / w2) + (yk2 / h2) <= 1;
		}

		/// <summary>
		/// Given a point that could be anywhere, this returns the point on the ellipse's border that
		/// would create a line towards the ellipse's center using those two points.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="ellipsePos"></param>
		/// <param name="ellipseWidth"></param>
		/// <param name="ellipseHeight"></param>
		/// <remarks>From https://www.reddit.com/r/askmath/comments/k6uwxq/how_to_find_the_coordinates_of_a_point_on_an/</remarks>
		/// <returns></returns>
		public static Vector2 GetPointOnEllipse(Vector2 point, Vector2 ellipsePos, float ellipseWidth, float ellipseHeight)
		{
			float theta = Vector2.SignedAngle(new Vector2(ellipseWidth, 0), point - ellipsePos) * Mathf.Deg2Rad;
			float tanTheta = Mathf.Tan(theta);
			float x = (ellipseWidth * ellipseHeight) / Mathf.Sqrt((ellipseHeight * ellipseHeight) + (ellipseWidth * ellipseWidth) * (tanTheta * tanTheta));

			if (Mathf.PI * -0.5f < theta && theta < Mathf.PI * 0.5f)
			{
				x = Mathf.Abs(x);
			}
			else
			{
				x = -Mathf.Abs(x);
			}

			float y = x * tanTheta;
			return new Vector2(x, y);
		}

		/// <summary>
		/// Check if line defined by lineSta and lineEnd passes through
		/// sphere whose center is at spherePos with the given radius.
		/// </summary>
		/// <remarks>
		/// From https://paulbourke.net/geometry/circlesphere/raysphere.c<br/>
		/// Explained in https://paulbourke.net/geometry/circlesphere/index.html#linesphere
		/// </remarks>
		/// <param name="lineSta"></param>
		/// <param name="lineEnd"></param>
		/// <param name="spherePos"></param>
		/// <param name="sphereRadius"></param>
		/// <returns></returns>
		public static bool LineIntersectsSphere(Vector3 lineSta, Vector3 lineEnd, Vector3 spherePos, float sphereRadius)
		{
			Vector3 dp = lineEnd - lineSta;

			float a = dp.x * dp.x + dp.y * dp.y + dp.z * dp.z;

			if (Mathf.Abs(a) < float.Epsilon)
			{
				// lineSta and lineEnd are too close to each other
				return false;
			}

			float b = 2 * (dp.x * (lineSta.x - spherePos.x) + dp.y * (lineSta.y - spherePos.y) + dp.z * (lineSta.z - spherePos.z));
			float c = spherePos.x * spherePos.x + spherePos.y * spherePos.y + spherePos.z * spherePos.z;
			c += lineSta.x * lineSta.x + lineSta.y * lineSta.y + lineSta.z * lineSta.z;
			c -= 2 * (spherePos.x * lineSta.x + spherePos.y * lineSta.y + spherePos.z * lineSta.z);
			c -= sphereRadius * sphereRadius;

			float bb4ac = b * b - 4 * a * c;

			return bb4ac >= 0;
		}

		/// <summary>
		/// Get top attainable speed based on force values and rigidbody properties.
		/// </summary>
		/// <remarks>
		/// Formula from: https://forum.unity.com/threads/terminal-velocity.34667/#post-1053927
		/// </remarks>
		/// <returns>Speed in meters per second (multiply by 3.6 to get kilometers per hour).</returns>
		public static float GetSpeed(float force, float drag, float mass) => ((force / drag) - Time.fixedDeltaTime * force) / mass;

		/// <summary>
		/// Get top attainable speed based on force values and rigidbody properties.
		/// </summary>
		/// <remarks>
		/// Formula from: https://forum.unity.com/threads/terminal-velocity.34667/#post-1053927
		/// </remarks>
		/// <returns>Speed in meters per second (multiply by 3.6 to get kilometers per hour).</returns>
		public static float GetSpeed(float force, Rigidbody rigidbody) => ((force / rigidbody.drag) - Time.fixedDeltaTime * force) / rigidbody.mass;
	}
}