// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DLD.Utility.Inspector.Editor
{
	public static class Utility
	{
		const string ARRAY_START_MARKER = ".Array.data[";

		static object FindElementInEnumerable(SerializedProperty property)
		{
			// This is most likely the MonoBehaviour or ScriptableObject that the SerializedProperty did the serializing on:
			var unityObject  = property.serializedObject.targetObject;

			string propertyPath = property.propertyPath;

			if (propertyPath.Contains(ARRAY_START_MARKER))
			{
				// We are in an array/list element, so we need to go through the object and
				// find that array/list element to invoke the property on that specific element.
				// Note: This assumes we are only inside one, no nested arrays/lists.

				//Debug.LogError($"arrayElementType: {parentObject.arrayElementType}");

				//string relativePath =
				//	propertyPath.Substring(0, propertyPath.LastIndexOf(".", StringComparison.Ordinal));
				//SerializedProperty parentObject = property.serializedObject.FindProperty(relativePath);
				//Debug.LogError($"Parent Object: {parentObject.name} ({parentObject.propertyPath}) Type: {parentObject.type} Serialized Object: {parentObject.serializedObject.targetObject.name}");

				// Get the array/list variable name
				string fieldName = propertyPath.Substring(0, propertyPath.IndexOf(".", StringComparison.Ordinal));

				// We do this to get the actual array/list, but as a System.Object
				var gotFieldInfo = unityObject.GetType().GetField(fieldName,
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				//Debug.LogError($"fieldName: {fieldName} fieldInfo: {gotFieldInfo != null}");
				if (gotFieldInfo != null)
				{
					// This is the actual instance of the array/list, but only as a System.Object
					var arrayObject = gotFieldInfo.GetValue(unityObject);

					// Using the PropertyPath, we get the element index value we are in
					int arraySta = propertyPath.LastIndexOf(ARRAY_START_MARKER, StringComparison.Ordinal);
					int arrayEnd = propertyPath.IndexOf("]", arraySta + ARRAY_START_MARKER.Length, StringComparison.Ordinal);
					string idxString = propertyPath.Substring(arraySta + ARRAY_START_MARKER.Length, arrayEnd - (arraySta + ARRAY_START_MARKER.Length));
					//Debug.LogError($"arraySta: {arraySta} arrayEnd: {arrayEnd} idxString: {idxString}");

					int idxDesired = int.Parse(idxString);
					//Debug.LogError($"idxDesired: {idxDesired}");

					// Loop through the array/list and get the element index we want
					int idxLooped = 0;
					var arrayEnumerable = (IEnumerable) arrayObject;
					foreach (var element in arrayEnumerable)
					{
						if (idxLooped == idxDesired)
						{
							// Finally found the element we were looking for.
							return element;
						}
						++idxLooped;
					}
				}
			}

			return null;
		}

		public static T GetPropertyReturnValue<T>(SerializedProperty property, string propertyName)
		{
			// This is most likely the MonoBehaviour or ScriptableObject that the SerializedProperty did the serializing on:
			var unityObject  = property.serializedObject.targetObject;

			var propertyInfo = unityObject.GetType().GetProperty(propertyName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (propertyInfo != null)
			{
				return (T)propertyInfo.GetMethod.Invoke(unityObject, null);
			}

			// Did not find property, could be that the property is in a regular C# class that was serialized.
			// There's no way to get the actual instance (the System.Object) of the regular C# class where the
			// SerializedProperty is serializing from, so we have to go the long way around and get it
			// from the targetObject (i.e. the MonoBehaviour or ScriptableObject).

			// Try seeing if this object is an element in an array/list:
			//
			// The property path of the SerializedProperty would be:
			//
			//   variableName.Array.data[0].fieldNameInElement
			//
			// variableName = the list/array name
			// fieldNameInElement = the name of the SerializedProperty we are in
			// Array.data[0] = this means we are the first element in the array
			//
			// Even if the field is a list, Unity serializes it as if it was an array,
			// so it will still have the "Array.data" in the property path.
			//
			string propertyPath = property.propertyPath;

			if (propertyPath.Contains(ARRAY_START_MARKER))
			{
				// We are in an array/list element, so we need to go through the object and
				// find that array/list element to invoke the property on that specific element.
				// Note: This assumes we are only inside one, no nested arrays/lists.

				var elementObject = FindElementInEnumerable(property);
				if (elementObject != null)
				{
					propertyInfo = elementObject.GetType().GetProperty(propertyName,
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					if (propertyInfo != null)
					{
						return (T) propertyInfo.GetMethod.Invoke(elementObject, null);
					}
				}
			}

			// was not able to find the property
			Debug.LogError($"Property {propertyName} not found in {unityObject.GetType().Name} SerializedProperty.path: {propertyPath}");
			return default;
		}

		public static T GetMethodReturnValue<T>(SerializedProperty property, string methodName)
		{
			// This is most likely the MonoBehaviour or ScriptableObject that the SerializedProperty did the serializing on:
			var unityObject  = property.serializedObject.targetObject;

			var propertyInfo = unityObject.GetType().GetMethod(methodName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (propertyInfo != null)
			{
				return (T)propertyInfo.Invoke(unityObject, null);
			}

			// Did not find property, could be that the property is in a regular C# class that was serialized.
			// There's no way to get the actual instance (the System.Object) of the regular C# class where the
			// SerializedProperty is serializing from, so we have to go the long way around and get it
			// from the targetObject (i.e. the MonoBehaviour or ScriptableObject).

			// Try seeing if this object is an element in an array/list:
			//
			// The property path of the SerializedProperty would be:
			//
			//   variableName.Array.data[0].fieldNameInElement
			//
			// variableName = the list/array name
			// fieldNameInElement = the name of the SerializedProperty we are in
			// Array.data[0] = this means we are the first element in the array
			//
			// Even if the field is a list, Unity serializes it as if it was an array,
			// so it will still have the "Array.data" in the property path.
			//
			string propertyPath = property.propertyPath;

			if (propertyPath.Contains(ARRAY_START_MARKER))
			{
				// We are in an array/list element, so we need to go through the object and
				// find that array/list element to invoke the property on that specific element.
				// Note: This assumes we are only inside one, no nested arrays/lists.

				var elementObject = FindElementInEnumerable(property);
				if (elementObject != null)
				{
					propertyInfo = elementObject.GetType().GetMethod(methodName,
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					if (propertyInfo != null)
					{
						return (T) propertyInfo.Invoke(elementObject, null);
					}
				}
			}

			// was not able to find the property
			Debug.LogError($"Method {methodName} not found in {unityObject.GetType().Name} SerializedProperty.path: {propertyPath}");
			return default;
		}
	}
}