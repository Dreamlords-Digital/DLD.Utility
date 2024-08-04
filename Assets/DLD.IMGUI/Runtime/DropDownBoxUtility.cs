// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

namespace DLD.IMGUI
{
	public static class DropDownBoxUtility
	{
		public const string SEPARATOR = "----";

		/// <summary>
		/// The number of opened dropdown boxes. Starts at 1. This is normally only 1.
		/// And dropdown boxes of different kinds use this to ensure that
		/// only one is open at any time.
		/// </summary>
		static int _openDropDownBoxes;

		public static int OpenDropDownBoxes => _openDropDownBoxes;

		public static int PushDropDownBox()
		{
			++_openDropDownBoxes;

			return _openDropDownBoxes;
		}

		public static void PopDropDownBox()
		{
			--_openDropDownBoxes;

			if (_openDropDownBoxes < 0)
			{
				_openDropDownBoxes = 0;
			}
		}

		public static bool IsNoDropDownBoxOpen
		{
			get
			{
				return _openDropDownBoxes == 0;
			}
		}

		public static bool IsAnyDropDownBoxOpen
		{
			get
			{
				return _openDropDownBoxes > 0;
			}
		}

		public static bool IsOnlyOneDropDownBoxOpen
		{
			get
			{
				return _openDropDownBoxes == 1;
			}
		}

		public static bool IsThisDropDownBoxObscured(int dropdownBoxId)
		{
			return _openDropDownBoxes > 1 && (dropdownBoxId < _openDropDownBoxes);
		}

		public static void CloseAllDropDownBoxes()
		{
			_openDropDownBoxes = 0;
		}

		// =========================================
	}
}