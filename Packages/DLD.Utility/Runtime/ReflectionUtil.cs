// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Reflection;

namespace DLD.Utility
{
	public static class ReflectionUtil
	{
		/// <summary>
		/// Based on: https://stackoverflow.com/a/1799401
		/// </summary>
		public static T GetEnumValueAttribute<T>(this object enumValue) where T : Attribute
		{
			Type enumType = enumValue.GetType();
			var memberInfos = enumType.GetMember(enumValue.ToString());

			for (int n = 0; n < memberInfos.Length; ++n)
			{
				if (memberInfos[n].DeclaringType == enumType)
				{
					return memberInfos[n].GetCustomAttribute<T>(false);
				}
			}

			return null;
		}
	}
}
