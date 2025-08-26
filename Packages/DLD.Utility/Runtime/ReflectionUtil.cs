// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

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

		/// <summary>
		/// Based on:
		/// https://stackoverflow.com/a/2210327
		/// https://stackoverflow.com/a/16506710
		/// </summary>
		public static bool IsAutoGetProperty(this PropertyInfo propertyInfo)
		{
			if (!propertyInfo.CanRead)
			{
				// property does not even have a get
				return false;
			}

			// see if property getter has the [CompilerGenerated] attribute
			var getMethod = propertyInfo.GetGetMethod(true);
			if (getMethod == null)
			{
				return false;
			}

			var compilerGeneratedGetters = getMethod.GetCustomAttribute<CompilerGeneratedAttribute>(true);
			if (compilerGeneratedGetters == null)
			{
				return false;
			}

			if (propertyInfo.DeclaringType != null)
			{
				// See if class/struct that owns this property has any field with the name:
				// "<PropertyName>k__BackingField"
				var fields = propertyInfo.DeclaringType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
				for (int n = 0; n < fields.Length; ++n)
				{
					if (fields[n].Name == $"<{propertyInfo.Name}>k__BackingField")
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Based on:
		/// https://stackoverflow.com/a/2210327
		/// https://stackoverflow.com/a/16506710
		/// </summary>
		/// <remarks>
		/// Note: Unless the property is declared abstract or extern, it's impossible for a property to be only an auto-set with no get.
		/// It's only either:
		/// 1. Property has a non-auto set, and no get (basically like a method that accepts 1 parameter).
		/// 2. Property has an auto set, and auto get (the typical auto property).
		/// </remarks>
		public static bool IsAutoSetProperty(this PropertyInfo propertyInfo)
		{
			if (!propertyInfo.CanWrite)
			{
				// property does not even have a set
				return false;
			}

			// see if property setter has the [CompilerGenerated] attribute
			var setMethod = propertyInfo.GetSetMethod(true);
			if (setMethod == null)
			{
				return false;
			}

			var compilerGeneratedSetters = setMethod.GetCustomAttribute<CompilerGeneratedAttribute>(true);
			if (compilerGeneratedSetters == null)
			{
				return false;
			}

			if (propertyInfo.DeclaringType != null)
			{
				// See if class/struct that owns this property has any private field with the name:
				// "<PropertyName>k__BackingField"
				var fields = propertyInfo.DeclaringType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
				for (int n = 0; n < fields.Length; ++n)
				{
					if (fields[n].Name == $"<{propertyInfo.Name}>k__BackingField")
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
