// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;

namespace DLD.UIToolkit
{

[Flags]
public enum TooltipShowMode : byte
{
	Normal = 0,
	AppendToExisting = 1 << 0,
	DoNotShowWhenUserIsDragging = 1 << 1,
}

}
