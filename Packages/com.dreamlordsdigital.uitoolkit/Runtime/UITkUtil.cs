// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

public static class CommandName
{
	/// <summary>
	///    Default shortcut: Ctrl + F
	/// </summary>
	public const string Find = "Find";

	/// <summary>
	///    Default shortcut: Ctrl + X
	/// </summary>
	public const string Cut = "Cut";

	/// <summary>
	///    Default shortcut: Ctrl + C
	/// </summary>
	public const string Copy = "Copy";

	/// <summary>
	///    Default shortcut: Ctrl + V
	/// </summary>
	public const string Paste = "Paste";

	/// <summary>
	///    Default shortcut: Ctrl + D
	/// </summary>
	public const string Duplicate = "Duplicate";

	/// <summary>
	///    Default shortcut: Ctrl + A
	/// </summary>
	public const string SelectAll = "SelectAll";

	/// <summary>
	///    Default shortcut: Shift + D
	/// </summary>
	public const string DeselectAll = "DeselectAll";

	/// <summary>
	///    Default shortcut: Shift + Delete
	/// </summary>
	/// <remarks>
	///    This is meant to be a delete command that bypasses the Recycle Bin/Trash.
	/// </remarks>
	public const string Delete = "Delete";

	/// <summary>
	///    Default shortcut: Delete
	/// </summary>
	/// <remarks>
	///    This is meant to be a delete command that, in reality, moves whatever was deleted to the Recycle Bin/Trash.
	/// </remarks>
	public const string SoftDelete = "SoftDelete";

	public const string FrameSelected = "FrameSelected";
	public const string FrameSelectedWithLock = "FrameSelectedWithLock";
	public const string FocusProjectWindow = "FocusProjectWindow";
}

public static class UITkUtil
{
	public const int LeftMouseButton = 0;
	public const int RightMouseButton = 1;

	public const string MouseCursorPanStyleClass = "dld-mouse-cursor--pan";
	public const string MouseCursorPanDragStyleClass = "dld-mouse-cursor--pan-drag";
	public const string MouseCursorZoomInStyleClass = "dld-mouse-cursor--zoom-in";
	public const string MouseCursorZoomOutStyleClass = "dld-mouse-cursor--zoom-out";
	public const string MouseCursorDragStyleClass = "dld-mouse-cursor--normal-drag";

	/// <summary>
	///    <para>Default reference zoom level.</para>
	/// </summary>
	public static readonly float DefaultReferenceScale = 1f;

	/// <summary>
	///    <para>Default min zoom level.</para>
	/// </summary>
	public static readonly float DefaultMinScale = 0.25f;

	/// <summary>
	///    <para>Default max zoom level.</para>
	/// </summary>
	public static readonly float DefaultMaxScale = 1f;

	/// <summary>
	///    <para>Default zoom step.</para>
	/// </summary>
	public static readonly float DefaultScaleStep = 0.15f;

	static readonly EventCallback<PointerDownEvent> OnPointerDownBypass = _OnPointerDownBypass;

	static void _OnPointerDownBypass(PointerDownEvent e)
	{
		e.StopPropagation();
	}

	static readonly EventCallback<PointerEnterEvent> OnPointerEnterBypass = _OnPointerEnterBypass;

	static void _OnPointerEnterBypass(PointerEnterEvent e)
	{
		e.StopPropagation();
	}

	static readonly EventCallback<PointerLeaveEvent> OnPointerLeaveBypass = _OnPointerLeaveBypass;

	static void _OnPointerLeaveBypass(PointerLeaveEvent e)
	{
		e.StopPropagation();
	}

	public static void RegisterPointerBypassCallbacks(this VisualElement ve)
	{
		ve?.RegisterCallback(OnPointerDownBypass);
		ve?.RegisterCallback(OnPointerEnterBypass);
		ve?.RegisterCallback(OnPointerLeaveBypass);
	}

	public static void UnregisterPointerBypassCallbacks(this VisualElement ve)
	{
		ve?.UnregisterCallback(OnPointerDownBypass);
		ve?.UnregisterCallback(OnPointerEnterBypass);
		ve?.UnregisterCallback(OnPointerLeaveBypass);
	}

	public static void AddStyleSheetsFrom(this VisualElement destination, VisualElement source)
	{
		for (int n = 0; n < source.styleSheets.count; ++n)
		{
			if (destination.styleSheets.Contains(source.styleSheets[n]))
			{
				continue;
			}

			destination.styleSheets.Add(source.styleSheets[n]);
		}
	}

	public static void AddStyleClassesFrom(this VisualElement destination, VisualElement source)
	{
		destination.ClearClassList();
		foreach (string styleClass in source.GetClasses())
		{
			if (destination.ClassListContains(styleClass))
			{
				continue;
			}

			destination.AddToClassList(styleClass);
		}
	}

	/// <summary>
	///    Call <see cref="VisualElement.AddToClassList"/> or <see cref="VisualElement.RemoveFromClassList"/>
	///    depending on value of <paramref name="set"/>.
	/// </summary>
	/// <param name="me"></param>
	/// <param name="set"></param>
	/// <param name="className"></param>
	public static void SetInClassList(this VisualElement me, bool set, string className)
	{
		if (set)
		{
			me.AddToClassList(className);
		}
		else
		{
			me.RemoveFromClassList(className);
		}
	}

	public static void RemoveTemplateContainer(this VisualElement me, string childRootName = "TemplateContainer")
	{
		var clonedRoot = me.Q<VisualElement>(childRootName);
		if (clonedRoot == null)
		{
			return;
		}

		me.pickingMode = clonedRoot.pickingMode;
		me.focusable = clonedRoot.focusable;

		foreach (string rootStyleClass in clonedRoot.GetClasses())
		{
			me.AddToClassList(rootStyleClass);
		}

		for (int n = clonedRoot.childCount - 1; n >= 0; --n)
		{
			me.Insert(0, clonedRoot[n]);
		}

		clonedRoot.RemoveFromHierarchy();
	}

	/// <summary>
	///    Like Q, but only performing the search through the VisualElement's ancestry.
	/// </summary>
	/// <param name="e"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static T Qup<T>(this VisualElement e) where T : VisualElement
	{
		if (e is T foundSelf)
		{
			return foundSelf;
		}

		VisualElement search = e.parent;
		while (search != null)
		{
			if (search is T found)
			{
				return found;
			}
			search = search.parent;
		}

		return null;
	}

	public static T Qup<T>(this VisualElement e, string name) where T : VisualElement
	{
		if (e is T foundSelf && foundSelf.name == name)
		{
			return foundSelf;
		}

		VisualElement search = e.parent;
		while (search != null)
		{
			if (search is T found && found.name == name)
			{
				return found;
			}
			search = search.parent;
		}

		return null;
	}

	public static T QupStartsWith<T>(this VisualElement e, string name) where T : VisualElement
	{
		if (e is T foundSelf && foundSelf.name.StartsWith(name))
		{
			return foundSelf;
		}

		VisualElement search = e.parent;
		while (search != null)
		{
			if (search is T found && found.name.StartsWith(name))
			{
				return found;
			}
			search = search.parent;
		}

		return null;
	}

	public static bool IsAncestorOf(this VisualElement ancestor, VisualElement child)
	{
		if (child == null)
		{
			return false;
		}

		for (VisualElement parent = child.hierarchy.parent; parent != null; parent = parent.hierarchy.parent)
		{
			if (ReferenceEquals(parent, ancestor))
			{
				return true;
			}
		}

		return false;
	}

	public static bool IsOrAncestorOf(this VisualElement ancestor, VisualElement child)
	{
		if (child == null)
		{
			return false;
		}

		if (ReferenceEquals(ancestor, child))
		{
			return true;
		}

		for (VisualElement parent = child.hierarchy.parent; parent != null; parent = parent.hierarchy.parent)
		{
			if (ReferenceEquals(parent, ancestor))
			{
				return true;
			}
		}

		return false;
	}

	public static string GetDebugAncestry(this VisualElement me)
	{
		var sb = new StringBuilder();

		var parent = me;
		int i = 0;
		while (parent != null)
		{
			if (i == 0)
			{
				sb.Append("(this). ");
			}
			else
			{
				sb.Append(i).Append(". ");
			}
			sb.AppendLine(parent.name);

			parent = parent.parent;
			++i;
		}

		return sb.ToString();
	}

	public static void Set(this Button button, string label = null, string iconClassName = null)
	{
		if (string.IsNullOrWhiteSpace(iconClassName))
		{
			// only label is available
			button.text = label;
			return;
		}

		var icon = new VisualElement();
		icon.AddToClassList(BaseIcons.IconStyleClass);
		icon.AddToClassList(iconClassName);

		button.Add(icon);
		button.AddToClassList(BaseStyles.ToggleWithIconStyleClass);

		if (!string.IsNullOrWhiteSpace(label))
		{
			var buttonLabel = new Label(label);
			button.Add(buttonLabel);
		}

		button.text = "";
	}

	public static void SetPosition(this VisualElement visualElement, Vector2 newPos)
	{
		visualElement.style.translate = newPos;
	}

	public static void SetPosition(this VisualElement visualElement, float newX, float newY)
	{
		visualElement.style.translate = new Translate(newX, newY);
	}

	public static void SetPositionX(this VisualElement visualElement, float newX)
	{
		var pos = visualElement.resolvedStyle.translate;
		pos.x = newX;
		visualElement.style.translate = pos;
	}

	public static void SetPositionY(this VisualElement visualElement, float newY)
	{
		var pos = visualElement.resolvedStyle.translate;
		pos.y = newY;
		visualElement.style.translate = pos;
	}

	public static Vector3 GetPositionXYZ(this VisualElement visualElement) => visualElement.resolvedStyle.translate;
	public static Vector2 GetPositionXY(this VisualElement visualElement) => visualElement.resolvedStyle.translate;

	/// <summary>
	///    Add the specified delta value to the transform's x and y.
	/// </summary>
	public static void AddToPosition(this VisualElement visualElement, Vector2 delta)
	{
		var pos = visualElement.resolvedStyle.translate;
		pos.x += delta.x;
		pos.y += delta.y;
		visualElement.style.translate = pos;
	}

	public static void SubtractFromPosition(this VisualElement visualElement, Vector2 delta)
	{
		var pos = visualElement.resolvedStyle.translate;
		pos.x -= delta.x;
		pos.y -= delta.y;
		visualElement.style.translate = pos;
	}

	public static void ResetPositionAndScale(this VisualElement visualElement)
	{
		visualElement.style.translate = Vector3.zero;
		visualElement.style.scale = Vector3.one;
	}

	public static void ResetScale(this VisualElement visualElement)
	{
		visualElement.style.scale = Vector3.one;
	}

	public static void SetScaleByZoom(this VisualElement visualElement, float zoomStep)
	{
		Vector3 scale = visualElement.resolvedStyle.scale.value;

		float newZoom = CalculateNewZoom(scale.y, zoomStep,
			DefaultScaleStep, DefaultReferenceScale, DefaultMinScale, DefaultMaxScale);
		scale.x = newZoom;
		scale.y = newZoom;
		scale.z = 1f;

		visualElement.style.scale = scale;
	}

	public static void SetScaleByZoom(this VisualElement visualElement, float zoomStep, VisualElement parent, Vector2 zoomFocusPoint)
	{
		Vector3 position = visualElement.resolvedStyle.translate;
		Vector3 scale = visualElement.resolvedStyle.scale.value;

		Vector2 focusPointLocal = parent.ChangeCoordinatesTo(visualElement, zoomFocusPoint);
		float x = focusPointLocal.x + visualElement.layout.x;
		float y = focusPointLocal.y + visualElement.layout.y;
		Vector3 mousePosBeforeScale = position + Vector3.Scale(new Vector3(x, y, 0.0f), scale);

		float newZoom = CalculateNewZoom(scale.y, zoomStep,
			DefaultScaleStep, DefaultReferenceScale, DefaultMinScale, DefaultMaxScale);
		scale.x = newZoom;
		scale.y = newZoom;
		scale.z = 1f;

		Vector3 newPosition = mousePosBeforeScale - Vector3.Scale(new Vector3(x, y, 0.0f), scale);

		visualElement.style.translate = newPosition;
		visualElement.style.scale = scale;
	}

	static float CalculateNewZoom(
		float currentZoom,
		float wheelDelta,
		float zoomStep,
		float referenceZoom,
		float minZoom,
		float maxZoom)
	{
		if (minZoom <= 0.0)
		{
			Debug.LogError($"The minimum zoom ({minZoom}) must be greater than zero.");
			return currentZoom;
		}

		if (referenceZoom < (double)minZoom)
		{
			Debug.LogError($"The reference zoom ({referenceZoom}) must be greater than or equal to the minimum zoom ({minZoom}).");
			return currentZoom;
		}

		if (referenceZoom > (double)maxZoom)
		{
			Debug.LogError($"The reference zoom ({referenceZoom}) must be less than or equal to the maximum zoom ({maxZoom}).");
			return currentZoom;
		}

		if (zoomStep < 0.0)
		{
			Debug.LogError($"The zoom step ({zoomStep}) must be greater than or equal to zero.");
			return currentZoom;
		}

		currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
		if (Mathf.Approximately(wheelDelta, 0.0f))
		{
			return currentZoom;
		}

		double y = Math.Log(referenceZoom, 1.0 + zoomStep);
		double num1 = referenceZoom - Math.Pow(1.0 + zoomStep, y);
		double num2 = Math.Log(minZoom - num1, 1.0 + zoomStep) - y;
		double num3 = Math.Log(maxZoom - num1, 1.0 + zoomStep) - y;
		double num4 = Math.Log(currentZoom - num1, 1.0 + zoomStep) - y;
		wheelDelta = Math.Sign(wheelDelta);
		double a = num4 + wheelDelta;

		if (a > num3 - 0.5)
		{
			return maxZoom;
		}

		if (a < num2 + 0.5)
		{
			return minZoom;
		}

		double num5 = Math.Round(a);
		return (float)(Math.Pow(1.0 + zoomStep, num5 + y) + num1);
	}
}

}
