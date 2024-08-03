// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;
using NUnit.Framework;

namespace DLD.Serializer.Tests
{
	public abstract partial class BaseTextDataTests
	{
		static void AssertApproximately(string label, float a, float b, float tolerance = 0.0001f)
		{
			Assert.IsTrue(Mathf.Abs(a - b) <= tolerance,
				"for {0}: a: {1} b: {2} difference: {3} tolerance: {4}",
				label, a, b, Mathf.Abs(a - b), tolerance);
		}

		static void AssertColor32Equal(Color32 a, Color32 b)
		{
			Assert.AreEqual(a.r, b.r);
			Assert.AreEqual(a.g, b.g);
			Assert.AreEqual(a.b, b.b);
			Assert.AreEqual(a.a, b.a);
		}

		static void AssertColorEqual(Color a, Color b)
		{
			AssertApproximately("r", a.r, b.r);
			AssertApproximately("g", a.g, b.g);
			AssertApproximately("b", a.b, b.b);
			AssertApproximately("a", a.a, b.a);
		}

		static void AssertVector2Equal(Vector2 a, Vector2 b)
		{
			AssertApproximately("x", a.x, b.x);
			AssertApproximately("y", a.y, b.y);
		}

		static void AssertVector3Equal(Vector3 a, Vector3 b)
		{
			AssertApproximately("x", a.x, b.x);
			AssertApproximately("y", a.y, b.y);
			AssertApproximately("z", a.z, b.z);
		}

		static void AssertVector4Equal(Vector4 a, Vector4 b)
		{
			AssertApproximately("x", a.x, b.x);
			AssertApproximately("y", a.y, b.y);
			AssertApproximately("z", a.z, b.z);
			AssertApproximately("w", a.w, b.w);
		}

		static void AssertQuaternionEqual(Quaternion a, Quaternion b)
		{
			AssertApproximately("w", a.w, b.w);
			AssertApproximately("x", a.x, b.x);
			AssertApproximately("y", a.y, b.y);
			AssertApproximately("z", a.z, b.z);
		}

		static void AssertMatrixEqual(Matrix4x4 a, Matrix4x4 b)
		{
			AssertApproximately("m00", a.m00, b.m00);
			AssertApproximately("m01", a.m01, b.m01);
			AssertApproximately("m02", a.m02, b.m02);
			AssertApproximately("m03", a.m03, b.m03);

			AssertApproximately("m10", a.m10, b.m10);
			AssertApproximately("m11", a.m11, b.m11);
			AssertApproximately("m12", a.m12, b.m12);
			AssertApproximately("m13", a.m13, b.m13);

			AssertApproximately("m20", a.m20, b.m20);
			AssertApproximately("m21", a.m21, b.m21);
			AssertApproximately("m22", a.m22, b.m22);
			AssertApproximately("m23", a.m23, b.m23);

			AssertApproximately("m30", a.m30, b.m30);
			AssertApproximately("m31", a.m31, b.m31);
			AssertApproximately("m32", a.m32, b.m32);
			AssertApproximately("m33", a.m33, b.m33);
		}

		[Test(Description = "Ensure Unity structs Color and Color32 gets serialized/deserialized.")]
		public void FromSerializedString_ForColor_DeserializesProperly()
		{
			// -----------------------------------------------

			var unityClass = new UnityDataTypesClass();

			var newColorValue = new Color(0, 1, 0, 1);
			var newColor32Value = new Color32(123, 56, 12, 200);

			unityClass.SetColor(newColorValue);
			unityClass.SetColor32(newColor32Value);

			AssertColorEqual(newColorValue, unityClass.Color);
			AssertColor32Equal(newColor32Value, unityClass.IntColor);

			// -----------------------------------------------

			string serialized = _textDataIO.ToSerializedString(unityClass);
			Debug.Log($"Serialized:\n{serialized}");

			// after serializing, it shouldn't matter if we change
			// the object now, the deserialized will get a copy of
			// how it was before serialization.
			var originalNewColor32Value = new Color32(10, 250, 88, 150);
			unityClass.SetColor32(originalNewColor32Value);
			AssertColor32Equal(originalNewColor32Value, unityClass.IntColor);

			var deserialized = _textDataIO.FromSerializedString<UnityDataTypesClass>(serialized);

			AssertColor32Equal(originalNewColor32Value, unityClass.IntColor);

			// -----------------------------------------------

			Assert.NotNull(deserialized);

			AssertColorEqual(newColorValue, deserialized.Color);
			AssertColor32Equal(newColor32Value, deserialized.IntColor);

			AssertColor32Equal(originalNewColor32Value, unityClass.IntColor);
		}

		[Test(Description = "Ensure Unity struct Vector2 gets serialized/deserialized.")]
		public void FromSerializedString_ForVector2_DeserializesProperly()
		{
			// -----------------------------------------------

			var unityClass = new UnityDataTypesClass();
			unityClass.SetVector2(Vector2.right);

			// -----------------------------------------------

			var serialized = _textDataIO.ToSerializedString(unityClass);

			// after serializing, it shouldn't matter if we change
			// the object now, the deserialized will get a copy of
			// how it was before serialization.
			unityClass.SetVector2(Vector2.down);

			var deserialized = _textDataIO.FromSerializedString<UnityDataTypesClass>(serialized);

			// -----------------------------------------------

			Assert.NotNull(deserialized);

			AssertVector2Equal(Vector2.right, deserialized.Vector2);
			AssertVector2Equal(Vector2.down, unityClass.Vector2);
		}

		[Test(Description = "Ensure Unity struct Vector3 gets serialized/deserialized.")]
		public void FromSerializedString_ForVector3_DeserializesProperly()
		{
			// -----------------------------------------------

			var unityClass = new UnityDataTypesClass();

			var newVector3Value = new Vector3(0.356f, 23.45f, 0);
			unityClass.SetVector3(newVector3Value);

			// -----------------------------------------------

			var serialized = _textDataIO.ToSerializedString(unityClass);

			// after serializing, it shouldn't matter if we change
			// the object now, the deserialized will get a copy of
			// how it was before serialization.
			unityClass.SetVector3(Vector3.down);

			var deserialized = _textDataIO.FromSerializedString<UnityDataTypesClass>(serialized);

			// -----------------------------------------------

			Assert.NotNull(deserialized);

			AssertVector3Equal(newVector3Value, deserialized.Vector3);
			AssertVector3Equal(Vector3.down, unityClass.Vector3);
		}

		[Test(Description = "Ensure Unity struct Vector4 gets serialized/deserialized.")]
		public void FromSerializedString_ForVector4_DeserializesProperly()
		{
			// -----------------------------------------------

			var unityClass = new UnityDataTypesClass();

			var newVector4Value = new Vector4(0.12f, 84.005f, 0, 1);
			unityClass.SetVector4(newVector4Value);

			// -----------------------------------------------

			var serialized = _textDataIO.ToSerializedString(unityClass);

			// after serializing, it shouldn't matter if we change
			// the object now, the deserialized will get a copy of
			// how it was before serialization.
			unityClass.SetVector4(Vector4.one);

			var deserialized = _textDataIO.FromSerializedString<UnityDataTypesClass>(serialized);

			// -----------------------------------------------

			Assert.NotNull(deserialized);

			AssertVector4Equal(newVector4Value, deserialized.Vector4);
			AssertVector4Equal(Vector4.one, unityClass.Vector4);
		}

		[Test(Description = "Ensure Unity struct Quaternion gets serialized/deserialized.")]
		public void FromSerializedString_ForQuaternion_DeserializesProperly()
		{
			// -----------------------------------------------

			var unityClass = new UnityDataTypesClass();

			var newQuatValue = new Quaternion(1.12f, 7.07f, 0.06f, 1);
			unityClass.SetQuaternion(newQuatValue);

			// -----------------------------------------------

			var serialized = _textDataIO.ToSerializedString(unityClass);

			// after serializing, it shouldn't matter if we change
			// the object now, the deserialized will get a copy of
			// how it was before serialization.
			unityClass.SetQuaternion(Quaternion.identity);

			var deserialized = _textDataIO.FromSerializedString<UnityDataTypesClass>(serialized);

			// -----------------------------------------------

			Assert.NotNull(deserialized);

			AssertQuaternionEqual(newQuatValue, deserialized.Quaternion);
			AssertQuaternionEqual(Quaternion.identity, unityClass.Quaternion);
		}

		[Test(Description = "Ensure Unity struct Matrix4x4 gets serialized/deserialized.")]
		public void FromSerializedString_ForMatrix_DeserializesProperly()
		{
			// -----------------------------------------------

			var unityClass = new UnityDataTypesClass();

			var newMatrixValue = new Matrix4x4();
			newMatrixValue.SetTRS(new Vector3(2,5,8), Quaternion.identity, new Vector3(1, 1, 1.5f));
			unityClass.SetMatrix(newMatrixValue);

			// -----------------------------------------------

			var serialized = _textDataIO.ToSerializedString(unityClass);

			// after serializing, it shouldn't matter if we change
			// the object now, the deserialized will get a copy of
			// how it was before serialization.
			var newMatrixValueAfter = new Matrix4x4();
			const float COS_45 = 0.70710678118654752440084436210485f;
			newMatrixValueAfter.SetTRS(new Vector3(1, 1, 0), new Quaternion(0, COS_45, 0, COS_45), new Vector3(2, 1, 1));
			unityClass.SetMatrix(newMatrixValueAfter);

			var deserialized = _textDataIO.FromSerializedString<UnityDataTypesClass>(serialized);

			// -----------------------------------------------

			Assert.NotNull(deserialized);

			AssertMatrixEqual(newMatrixValue, deserialized.Matrix);
			AssertMatrixEqual(newMatrixValueAfter, unityClass.Matrix);
		}
	}
}