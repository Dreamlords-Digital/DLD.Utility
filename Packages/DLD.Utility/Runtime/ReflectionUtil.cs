// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;

namespace DLD.Utility
{
	public static class ReflectionUtil
	{
		public static T GetEnumValueAttribute<T>(this object enumValue) where T : Attribute
		{
			System.Type enumType = enumValue.GetType();
			var memberInfos = enumType.GetMember(enumValue.ToString());

			for (int n = 0; n < memberInfos.Length; ++n)
			{
				if (memberInfos[n].DeclaringType == enumType)
				{
					object[] tooltips = memberInfos[n].GetCustomAttributes(typeof(T), false);
					if (tooltips.Length > 0)
					{
						return (T)tooltips[0];
					}

					return null;
				}
			}

			return null;
		}
	}
}
