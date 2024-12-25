// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEngine;

namespace DLD.Utility.Inspector
{
	/// <summary>
	/// Forces a string to be assigned with scene paths only.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class ScenePathAttribute : PropertyAttribute
	{
	}
}