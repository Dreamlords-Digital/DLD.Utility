// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using UnityEngine;

namespace DLD.Serializer.Tests
{
	[Serializable]
	public class UnityDataTypesClass : ITextData
	{
		[Serialized]
		Color _color;

		[Serialized]
		Color32 _color32;

		[Serialized]
		Vector2 _vector2;

		[Serialized]
		Vector3 _vector3;

		[Serialized]
		Vector4 _vector4;

		[Serialized]
		Quaternion _quaternion;

		[Serialized]
		Matrix4x4 _matrix;

		// -----------------------------

		public Color Color
		{
			get { return _color; }
		}

		public Color32 IntColor
		{
			get { return _color32; }
		}

		public Vector2 Vector2
		{
			get { return _vector2; }
		}

		public Vector3 Vector3
		{
			get { return _vector3; }
		}

		public Vector4 Vector4
		{
			get { return _vector4; }
		}

		public Quaternion Quaternion
		{
			get { return _quaternion; }
		}

		public Matrix4x4 Matrix
		{
			get { return _matrix; }
		}

		// -----------------------------

		public void SetColor(Color newValue)
		{
			_color = newValue;
		}

		public void SetColor32(Color32 newValue)
		{
			_color32 = newValue;
		}

		public void SetVector2(Vector2 newValue)
		{
			_vector2 = newValue;
		}

		public void SetVector3(Vector3 newValue)
		{
			_vector3 = newValue;
		}

		public void SetVector4(Vector4 newValue)
		{
			_vector4 = newValue;
		}

		public void SetQuaternion(Quaternion newValue)
		{
			_quaternion = newValue;
		}

		public void SetMatrix(Matrix4x4 newValue)
		{
			_matrix = newValue;
		}

		// -----------------------------

		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}
	}
}