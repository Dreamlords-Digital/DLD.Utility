// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;

namespace DLD.UIToolkit
{

[Flags]
public enum DragAndDropMode : byte
{
	/// <summary>
	///    Regular behaviour. User intends to parent.
	/// </summary>
	Normal = 0,

	/// <summary>
	///    User doesn't intend to parent the node, even if it is on top of a valid destination.
	///    It will just be moved (and unparented, if it isn't already).
	/// </summary>
	ForceMove = 1 << 0,

	/// <summary>
	///    User wants dragged node to be duplicated into the drop destination.
	/// </summary>
	CopyInto = 1 << 1,
}

public static class DragAndDropModeUtility
{
	public static bool Has(this DragAndDropMode valueToCheck, DragAndDropMode flagWanted)
	{
		return (valueToCheck & flagWanted) == flagWanted;
	}

	public static bool DoesNotHave(this DragAndDropMode valueToCheck, DragAndDropMode flagNotWanted)
	{
		return !valueToCheck.Has(flagNotWanted);
	}
}

}
