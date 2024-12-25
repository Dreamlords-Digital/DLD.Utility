// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DLD.Utility
{
	[Serializable]
	public struct RangeFloat
	{
		public float Start;
		public float End;

		public RangeFloat(float start, float end)
		{
			Start = start;
			End = end;
		}

		public bool IsZero => Mathf.Abs(Start) < float.Epsilon && Mathf.Abs(End) < float.Epsilon;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Random()
		{
			return UnityEngine.Random.Range(Start, End);
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
	}
}