// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using DLD.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

public static class TooltipUtil
{
	public static readonly EventCallback<PointerEnterEvent, ITooltip> ShowAtMouse = _ShowAtMouse;
	public static readonly EventCallback<PointerEnterEvent, ITooltip> ShowAtRightNoDrag = _ShowAtRightNoDrag;
	public static readonly EventCallback<PointerEnterEvent, ITooltip> ShowAtRight = _ShowAtRight;
	public static readonly EventCallback<PointerLeaveEvent, ITooltip> Hide = _HideTooltip;
	public static readonly EventCallback<PointerLeaveEvent, ITooltip> RemoveTooltipsOfMatchingContext = _RemoveTooltipsOfMatchingContext;

	static readonly EventCallback<PointerEnterEvent, ITooltip> AppendAtMouse = _AppendAtMouse;
	static readonly EventCallback<PointerEnterEvent, ITooltip> AppendAtRightNoDrag = _AppendAtRightNoDrag;
	static readonly EventCallback<PointerEnterEvent, ITooltip> AppendAtRight = _AppendAtRight;

	const string DefaultObsoleteMessage = "Marked as obsolete";

	public static TooltipMessage[] CreateTooltipMessages(DropdownItemTooltip item, EnumDropdownItem obsoleteItem, string obsoleteMessageToUseIfNull = DefaultObsoleteMessage)
	{
		return new TooltipMessage[]
		{
			new(BaseIcons.GenericInfo, item.Tooltip),
			new(obsoleteItem.Obsolete == EnumObsoleteType.ObsoleteError ? BaseIcons.GenericError : BaseIcons.GenericWarning, obsoleteItem.ObsoleteMessage ?? obsoleteMessageToUseIfNull),
		};
	}

	public static string CreateObsoleteTooltip(EnumDropdownItem item, string obsoleteMessageToUseIfNull = DefaultObsoleteMessage)
	{
		return $"{(item.Obsolete == EnumObsoleteType.ObsoleteError ? BaseIcons.GenericError : BaseIcons.GenericWarning)};{item.ObsoleteMessage ?? obsoleteMessageToUseIfNull}";
	}

	public static string CreateObsoleteTooltipIcon(EnumObsoleteType obsoleteType)
	{
		return obsoleteType == EnumObsoleteType.ObsoleteError ? BaseIcons.GenericError : BaseIcons.GenericWarning;
	}

	public static string CreateObsoleteTooltipText(EnumDropdownItem item, string obsoleteMessageToUseIfNull = DefaultObsoleteMessage) =>
		!string.IsNullOrWhiteSpace(item.ObsoleteMessage) ? item.ObsoleteMessage : obsoleteMessageToUseIfNull;

	public static TooltipMessage CreateObsoleteTooltipMessage(EnumDropdownItem item, string obsoleteMessageToUseIfNull = DefaultObsoleteMessage)
	{
		return new TooltipMessage(CreateObsoleteTooltipIcon(item.Obsolete), CreateObsoleteTooltipText(item, obsoleteMessageToUseIfNull));
	}

	public static void RegisterTooltipDisplayer(this VisualElement tooltipDisplayer, ITooltip tooltip, string tooltipText, string iconClassName = BaseIcons.GenericInfo,
		TooltipShowMode tooltipShowMode = TooltipShowMode.Normal, ElementAnchorPoint anchorPoint = ElementAnchorPoint.Mouse)
	{
		if (tooltipDisplayer == null)
		{
			return;
		}

		if (string.IsNullOrWhiteSpace(tooltipText))
		{
			return;
		}

		tooltipDisplayer.PrepareTooltipText(tooltipText, iconClassName);
		tooltipDisplayer.RegisterTooltipDisplayer(tooltip, tooltipShowMode, anchorPoint);
	}

	public static void RegisterTooltipDisplayer(this VisualElement tooltipDisplayer, ITooltip tooltip,
		TooltipShowMode tooltipShowMode = TooltipShowMode.Normal, ElementAnchorPoint anchorPoint = ElementAnchorPoint.Mouse)
	{
		if (tooltipDisplayer == null)
		{
			return;
		}

		if (anchorPoint == ElementAnchorPoint.Mouse)
		{
			tooltipDisplayer.RegisterCallback(
				(tooltipShowMode | TooltipShowMode.AppendToExisting) == TooltipShowMode.AppendToExisting
					? AppendAtMouse
					: ShowAtMouse, tooltip);
		}
		else if (anchorPoint == ElementAnchorPoint.Right)
		{
			if ((tooltipShowMode | TooltipShowMode.DoNotShowWhenUserIsDragging) == TooltipShowMode.DoNotShowWhenUserIsDragging)
			{
				tooltipDisplayer.RegisterCallback(
					(tooltipShowMode | TooltipShowMode.AppendToExisting) == TooltipShowMode.AppendToExisting
						? AppendAtRightNoDrag
						: ShowAtRightNoDrag, tooltip);
			}
			else
			{
				tooltipDisplayer.RegisterCallback(
					(tooltipShowMode | TooltipShowMode.AppendToExisting) == TooltipShowMode.AppendToExisting
						? AppendAtRight
						: ShowAtRight, tooltip);
			}
		}

		tooltipDisplayer.RegisterCallback(
			(tooltipShowMode | TooltipShowMode.AppendToExisting) == TooltipShowMode.AppendToExisting
				? RemoveTooltipsOfMatchingContext
				: Hide, tooltip);
	}

	public static (string, TooltipMessage) CreateClickableLinkTooltip(string tooltipText, string iconClassName = BaseIcons.GenericInfo)
	{
		if (string.IsNullOrWhiteSpace(tooltipText))
		{
			return (null, null);
		}

		(string url, string formattedText) = tooltipText.ExtractHRef(replacementStartTag: BaseStyles.LinkStartTags, replacementEndTag: BaseStyles.LinkEndTags);
		if (!string.IsNullOrEmpty(url))
		{
			return (url, new TooltipMessage(iconClassName, formattedText));
		}
		else
		{
			return (null, new TooltipMessage(iconClassName, tooltipText));
		}
	}

	static string PrepareTooltipText(this VisualElement tooltipDisplayer, string tooltipText, string iconClassName = BaseIcons.GenericInfo)
	{
		string finalTooltipText;
		(string url, string formattedText) = tooltipText.ExtractHRef(replacementStartTag: BaseStyles.LinkStartTags, replacementEndTag: BaseStyles.LinkEndTags);
		if (!string.IsNullOrEmpty(url))
		{
			tooltipDisplayer.RegisterCallback<ClickEvent, string>((e, gotUrl) =>
			{
				if (e.ctrlKey && e.button == 0) // ctrl + left click
				{
					Application.OpenURL(gotUrl);
					e.StopImmediatePropagation();
				}
			}, url);

			tooltipDisplayer.userData = new[]
			{
				new TooltipMessage(BaseIcons.GenericInfo, formattedText),
				TooltipMessage.OpenLinkInWebBrowser,
			};

			finalTooltipText = formattedText;
		}
		else
		{
			// If passed tooltipText already has an icon inside, or user doesn't want an icon displayed,
			// then just use tooltipText as-is.
			finalTooltipText = tooltipText.Contains(';') || string.IsNullOrWhiteSpace(iconClassName) ? tooltipText : $"{iconClassName};{tooltipText}";

			tooltipDisplayer.userData = finalTooltipText;
		}

		return finalTooltipText;
	}

	static void _ShowAtMouse(PointerEnterEvent e, ITooltip t)
	{
		var eventTarget = (VisualElement)e.target;
		_Show(eventTarget, t, e.position, false, ElementAnchorPoint.Mouse);
	}

	static void _AppendAtMouse(PointerEnterEvent e, ITooltip t)
	{
		var eventTarget = (VisualElement)e.target;
		_Show(eventTarget, t, e.position, true, ElementAnchorPoint.Mouse);
	}

	static void _ShowAtRightNoDrag(PointerEnterEvent e, ITooltip t)
	{
		if (t.IsDragging)
		{
			return;
		}

		var eventTarget = (VisualElement)e.target;
		_Show(eventTarget, t, e.position, false, ElementAnchorPoint.Right);
	}

	static void _ShowAtRight(PointerEnterEvent e, ITooltip t)
	{
		var eventTarget = (VisualElement)e.target;
		_Show(eventTarget, t, e.position, false, ElementAnchorPoint.Right);
	}

	static void _AppendAtRightNoDrag(PointerEnterEvent e, ITooltip t)
	{
		if (t.IsDragging)
		{
			return;
		}

		var eventTarget = (VisualElement)e.target;
		_Show(eventTarget, t, e.position, true, ElementAnchorPoint.Right);
	}

	static void _AppendAtRight(PointerEnterEvent e, ITooltip t)
	{
		var eventTarget = (VisualElement)e.target;
		_Show(eventTarget, t, e.position, true, ElementAnchorPoint.Right);
	}

	public static (string, string) GetTooltipText(string tooltip)
	{
		string iconClassName;
		int semicolonIdx = tooltip.IndexOf(';');
		if (semicolonIdx != -1)
		{
			iconClassName = tooltip.Substring(0, semicolonIdx);
			tooltip = tooltip.Substring(semicolonIdx + 1);

			if (string.IsNullOrWhiteSpace(tooltip))
			{
				return (null, null);
			}
		}
		else
		{
			iconClassName = null;
		}

		return (tooltip, iconClassName);
	}

	static void _Show(VisualElement eventTarget, ITooltip t, Vector2 mousePos, bool append, ElementAnchorPoint anchorPoint)
	{
		switch (eventTarget.userData)
		{
			case string tooltip:
			{
				if (string.IsNullOrWhiteSpace(tooltip))
				{
					// nothing to show
					t.SetContext(eventTarget);
					return;
				}

				(string tooltipText, string iconClassName) = GetTooltipText(tooltip);

				switch (anchorPoint)
				{
					case ElementAnchorPoint.Mouse:
						t.ShowTooltipAtMouse(eventTarget, tooltipText, iconClassName, mousePos, append);
						break;
					default:
						t.ShowTooltipAt(eventTarget, eventTarget, tooltipText, iconClassName, anchorPoint, append);
						break;
				}

				break;
			}
			case TooltipMessage tooltipMessage:
			{
				if (string.IsNullOrWhiteSpace(tooltipMessage.Text))
				{
					// nothing to show
					t.SetContext(eventTarget);
					return;
				}

				switch (anchorPoint)
				{
					case ElementAnchorPoint.Mouse:
						t.ShowTooltipAtMouse(eventTarget, tooltipMessage.Text, tooltipMessage.IconClassName, mousePos, append);
						break;
					default:
						t.ShowTooltipAt(eventTarget, eventTarget, tooltipMessage.Text, tooltipMessage.IconClassName, anchorPoint, append);
						break;
				}

				break;
			}
			case TooltipWithAnchor tooltipWithAnchor:
			{
				if (string.IsNullOrWhiteSpace(tooltipWithAnchor.Tooltip.Text))
				{
					// nothing to show
					t.SetContext(anchorPoint == ElementAnchorPoint.Mouse ? eventTarget : tooltipWithAnchor.TooltipAnchor);
					return;
				}

				switch (anchorPoint)
				{
					case ElementAnchorPoint.Mouse:
						t.ShowTooltipAtMouse(eventTarget, tooltipWithAnchor.Tooltip.Text, tooltipWithAnchor.Tooltip.IconClassName, mousePos, append);
						break;
					default:
						t.ShowTooltipAt(eventTarget, tooltipWithAnchor.TooltipAnchor, tooltipWithAnchor.Tooltip.Text, tooltipWithAnchor.Tooltip.IconClassName, anchorPoint, append);
						break;
				}

				break;
			}
			case TooltipCollection tooltipCollection:
			{
				UseTooltipMessageArray(tooltipCollection.Tooltips, tooltipCollection.TooltipAnchor);
				break;
			}
			case TooltipMessage[] tooltipMessageArray:
			{
				UseTooltipMessageArray(tooltipMessageArray, eventTarget);
				break;
			}
			case List<TooltipMessage> tooltipMessageList:
			{
				if (tooltipMessageList.Count == 0)
				{
					t.SetContext(eventTarget);
					return;
				}

				if (!append)
				{
					t.ClearTooltipMessages();
				}

				bool addedAtLeastOne = false;
				for (int i = 0; i < tooltipMessageList.Count; ++i)
				{
					if (tooltipMessageList[i] == null || string.IsNullOrWhiteSpace(tooltipMessageList[i].Text))
					{
						continue;
					}

					addedAtLeastOne = true;
					t.AddToTooltip(eventTarget, tooltipMessageList[i].Text, tooltipMessageList[i].IconClassName);
				}

				if (!addedAtLeastOne)
				{
					t.SetContext(eventTarget);
					if (!append)
					{
						return;
					}
				}

				switch (anchorPoint)
				{
					case ElementAnchorPoint.Mouse:
						t.ShowAtMouseCursor(mousePos);
						break;
					default:
						t.ShowAt(eventTarget, anchorPoint);
						break;
				}

				break;
			}
		}

		return;

		void UseTooltipMessageArray(TooltipMessage[] tooltipMessageArray, VisualElement anchor)
		{
			if (tooltipMessageArray.Length == 0)
			{
				t.SetContext(anchorPoint == ElementAnchorPoint.Mouse ? eventTarget : anchor);
				return;
			}

			if (!append)
			{
				t.ClearTooltipMessages();
			}

			bool addedAtLeastOne = false;
			for (int i = 0; i < tooltipMessageArray.Length; ++i)
			{
				if (tooltipMessageArray[i] == null || string.IsNullOrWhiteSpace(tooltipMessageArray[i].Text))
				{
					continue;
				}

				addedAtLeastOne = true;
				t.AddToTooltip(eventTarget, tooltipMessageArray[i].Text, tooltipMessageArray[i].IconClassName);
			}

			if (!addedAtLeastOne)
			{
				t.SetContext(eventTarget);
				if (!append)
				{
					return;
				}
			}

			switch (anchorPoint)
			{
				case ElementAnchorPoint.Mouse:
					t.ShowAtMouseCursor(mousePos);
					break;
				default:
					t.ShowAt(anchor, anchorPoint);
					break;
			}
		}
	}

	static void _HideTooltip(PointerLeaveEvent e, ITooltip t)
	{
		t.HideTooltip();
	}

	static void _RemoveTooltipsOfMatchingContext(PointerLeaveEvent e, ITooltip t)
	{
		var eventTarget = (VisualElement)e.target;
		t.HideTooltipIfContextIs(eventTarget, true);
	}
}

}
