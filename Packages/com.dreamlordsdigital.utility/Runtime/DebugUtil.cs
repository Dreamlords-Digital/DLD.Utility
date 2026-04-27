// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;

namespace DLD.Utility
{

public static class DebugUtil
{
	// =================================================================

	public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Debug.DrawRay(pos, direction);

		Quaternion rotation = Quaternion.LookRotation(direction);

		Vector3 right = rotation * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
		Vector3 left = rotation * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
		Debug.DrawRay(pos + direction, right * arrowHeadLength);
		Debug.DrawRay(pos + direction, left * arrowHeadLength);
	}

	public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Debug.DrawRay(pos, direction, color);

		Quaternion rotation = Quaternion.LookRotation(direction);

		Vector3 right = rotation * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
		Vector3 left = rotation * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
		Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
		Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
	}

	// =================================================================

	public static void DrawX(Vector3 pos, float size)
	{
		Vector3 topLeft = pos + (new Vector3(-0.707f, 0, 0.707f) * size);
		Vector3 topRight = pos + (new Vector3(0.707f, 0, 0.707f) * size);
		Vector3 bottomLeft = pos + (new Vector3(-0.707f, 0, -0.707f) * size);
		Vector3 bottomRight = pos + (new Vector3(0.707f, 0, -0.707f) * size);

		Debug.DrawLine(topLeft, bottomRight);
		Debug.DrawLine(topRight, bottomLeft);
	}

	public static void DrawX(Vector3 pos, float size, Color color)
	{
		Vector3 topLeft = pos + (new Vector3(-0.707f, 0, 0.707f) * size);
		Vector3 topRight = pos + (new Vector3(0.707f, 0, 0.707f) * size);
		Vector3 bottomLeft = pos + (new Vector3(-0.707f, 0, -0.707f) * size);
		Vector3 bottomRight = pos + (new Vector3(0.707f, 0, -0.707f) * size);

		Debug.DrawLine(topLeft, bottomRight, color);
		Debug.DrawLine(topRight, bottomLeft, color);
	}

	// ------------------------------------------------------------

	public static void DrawX(Vector3 pos, float size, Vector3 direction)
	{
		Quaternion rotation = Quaternion.LookRotation(direction);
		DrawX(pos, size, rotation);
	}

	public static void DrawX(Vector3 pos, float size, Vector3 direction, Color color)
	{
		Quaternion rotation = Quaternion.LookRotation(direction);
		DrawX(pos, size, rotation, color);
	}

	// ------------------------------------------------------------

	public static void DrawX(Vector3 pos, float size, Quaternion rotation)
	{
		Vector3 topLeft = pos + (rotation * new Vector3(-0.707f, 0, 0.707f) * size);
		Vector3 topRight = pos + (rotation * new Vector3(0.707f, 0, 0.707f) * size);
		Vector3 bottomLeft = pos + (rotation * new Vector3(-0.707f, 0, -0.707f) * size);
		Vector3 bottomRight = pos + (rotation * new Vector3(0.707f, 0, -0.707f) * size);

		Debug.DrawLine(topLeft, bottomRight);
		Debug.DrawLine(topRight, bottomLeft);
	}

	public static void DrawX(Vector3 pos, float size, Quaternion rotation, Color color)
	{
		Vector3 topLeft = pos + (rotation * new Vector3(-0.707f, 0, 0.707f) * size);
		Vector3 topRight = pos + (rotation * new Vector3(0.707f, 0, 0.707f) * size);
		Vector3 bottomLeft = pos + (rotation * new Vector3(-0.707f, 0, -0.707f) * size);
		Vector3 bottomRight = pos + (rotation * new Vector3(0.707f, 0, -0.707f) * size);

		Debug.DrawLine(topLeft, bottomRight, color);
		Debug.DrawLine(topRight, bottomLeft, color);
	}

	// =================================================================

	public static void DrawCircle(Vector3 pos, float radius)
	{
		float stepSize = (0.1f / radius);
		float angle = 0;

		Vector3 pt1 = new Vector3(pos.x + radius, 0, pos.z);
		angle += stepSize;

		Vector3 start = pt1;
		Vector3 end = pt1;
		while (angle <= ((2 * Mathf.PI) ))
		{
			Vector3 pt2 = new Vector3(pos.x + radius * Mathf.Cos(angle), 0, pos.z + radius * Mathf.Sin(angle));
			Debug.DrawLine(pt1, pt2);
			pt1 = pt2;

			angle += stepSize;
			end = pt2;
		}

		Debug.DrawLine(end, start);
	}

	public static void DrawCircle(Vector3 pos, float radius, Color color)
	{
		float stepSize = (0.1f / radius);
		float angle = 0;

		Vector3 pt1 = new Vector3(pos.x + radius, 0, pos.z);
		angle += stepSize;

		Vector3 start = pt1;
		Vector3 end = pt1;
		while (angle <= ((2 * Mathf.PI) ))
		{
			Vector3 pt2 = new Vector3(pos.x + radius * Mathf.Cos(angle), 0, pos.z + radius * Mathf.Sin(angle));
			Debug.DrawLine(pt1, pt2, color);
			pt1 = pt2;

			angle += stepSize;
			end = pt2;
		}

		Debug.DrawLine(end, start, color);
	}

	public static void DrawCircle(Vector3 pos, float radius, float gap, Vector3 direction)
	{
		Quaternion rotation = Quaternion.LookRotation(direction);

		float stepSize = (0.1f / radius);
		float angle = (gap/2) + (Mathf.PI/2);

		Vector3 pt1 = pos + rotation * new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
		angle += stepSize;

		Vector3 start = pt1;
		Vector3 end = pt1;
		while (angle <= ((2 * Mathf.PI) - (gap/2) + (Mathf.PI/2)))
		{
			Vector3 pt2 = pos + rotation * new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));

			Debug.DrawLine(pt1, pt2);
			pt1 = pt2;

			angle += stepSize;
			end = pt2;
		}

		if (gap <= 0)
		{
			Debug.DrawLine(end, start);
		}
	}

	public static void DrawCircle(Vector3 pos, float radius, float gap, Vector3 direction, Color color)
	{
		Quaternion rotation = Quaternion.LookRotation(direction);

		float stepSize = (0.1f / radius);
		float angle = (gap/2) + (Mathf.PI/2);

		Vector3 pt1 = pos + rotation * new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
		angle += stepSize;

		Vector3 start = pt1;
		Vector3 end = pt1;
		while (angle <= ((2 * Mathf.PI) - (gap/2) + (Mathf.PI/2)))
		{
			Vector3 pt2 = pos + rotation * new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));

			Debug.DrawLine(pt1, pt2, color);
			pt1 = pt2;

			angle += stepSize;
			end = pt2;
		}

		if (gap <= 0)
		{
			Debug.DrawLine(end, start, color);
		}
	}

	// =================================================================
}

}
