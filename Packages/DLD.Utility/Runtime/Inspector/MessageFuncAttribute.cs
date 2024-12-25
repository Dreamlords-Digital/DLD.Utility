// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEngine;

namespace DLD.Utility.Inspector
{
	/// <summary>
	/// Use a method to determine a message to be drawn below the Serialized Field.
	/// </summary>
	/// <remarks>
	/// <para>This is assuming the method returns a string and that it requires no parameters. The method does not have to be public.</para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class MessageFuncAttribute : PropertyAttribute
	{
		public readonly string FuncName;
		public MessageFuncAttribute(string funcName) => FuncName = funcName;
	}
}