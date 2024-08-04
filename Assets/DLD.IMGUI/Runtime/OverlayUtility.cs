// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

namespace DLD.IMGUI
{
	public static class OverlayUtility
	{
		public static bool IsAnyOverlayOpen => ColorPicker.IsOpen || DropDownBoxUtility.IsAnyDropDownBoxOpen;
	}
}