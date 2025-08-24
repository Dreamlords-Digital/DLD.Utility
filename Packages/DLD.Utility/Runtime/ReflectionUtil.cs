// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;

namespace DLD.Utility
{
	public static class ReflectionUtil
	{
		/// <summary>
		/// Based on https://stackoverflow.com/a/1799401
		/// </summary>
		public static T GetEnumValueAttribute<T>(this object enumValue) where T : Attribute
		{
			Type enumType = enumValue.GetType();
			var memberInfos = enumType.GetMember(enumValue.ToString());

			for (int n = 0; n < memberInfos.Length; ++n)
			{
				if (memberInfos[n].DeclaringType == enumType)
				{
					object[] gotAttributes = memberInfos[n].GetCustomAttributes(typeof(T), false);
					if (gotAttributes.Length > 0)
					{
						return (T)gotAttributes[0];
					}

					return null;
				}
			}

			return null;
		}
	}
}
