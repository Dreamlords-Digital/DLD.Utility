// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;
using DLD.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

public interface ITooltip
{
	/// <summary>
	///    Show a tooltip message that follows the mouse cursor.
	///    It will not hide until <see cref="HideTooltip"/> or <see cref="HideTooltipIfContextIs"/> is called.
	/// </summary>
	/// <param name="context">The thing that caused the tooltip to be shown.</param>
	/// <param name="text"></param>
	/// <param name="iconClassName">Optional icon drawn before the tooltip text. This is a USS style name.</param>
	/// <param name="mousePos">Initial mouse position. This ensures the tooltip is at the correct position at the start.</param>
	/// <param name="pushToStack">Whether the tooltip text specified will be added to the existing text already on the tooltip, or not.</param>
	void ShowTooltipAtMouse(VisualElement context, string text, string iconClassName, Vector2 mousePos, bool pushToStack = false);

	void ShowTooltipAt(VisualElement context, VisualElement anchorElement, string text, string iconClassName, ElementAnchorPoint anchorPoint, bool pushToStack = false);

	/// <summary>
	///    Add another message to the tooltip, assuming it's already shown.
	/// </summary>
	/// <remarks>
	///    The new message will be shown above and all the existing messages will be moved downward.
	/// </remarks>
	/// <param name="context">The thing that caused the tooltip to be shown.</param>
	/// <param name="text"></param>
	/// <param name="iconClassName">Optional icon drawn before the tooltip text. This is a USS style name.</param>
	void AddToTooltip(VisualElement context, string text, string iconClassName = null);

	void HideTooltip();

	/// <summary>
	///    Remove all messages in the tooltip, without hiding it (if it's shown).
	/// </summary>
	void ClearTooltipMessages();

	void ShowAtMouseCursor(Vector2 mousePos);

	void ShowAt(VisualElement context, ElementAnchorPoint anchorPoint);

	void SetContext(VisualElement eventTarget);

	/// <summary>
	///    If the context that was last assigned to the tooltip matches the one specified, the tooltip is hidden.
	/// </summary>
	/// <remarks>
	///    Basically, this ensures a VisualElement that showed a tooltip
	///    will hide it only if the tooltip is still showing its message.
	/// </remarks>
	/// <param name="context"></param>
	/// <param name="popFromStack">
	///    Only remove the most recent tooltip message (if it's showing multiple tooltip messages),
	///    instead of hiding the entire tooltip.
	/// </param>
	void HideTooltipIfContextIs(VisualElement context, bool popFromStack = false);

	/// <summary>
	///    Removes all currently displayed messages of the specified context,
	///    then re-adds the up-to-date messages given by the specified context.
	/// </summary>
	/// <remarks>
	///    If the tooltip isn't shown on the specified context, then this aborts.<br/>
	///    If the tooltip ends up with no more messages, then the tooltip will be automatically hidden.<br/>
	///    If the tooltip ends up having messages, and it's currently hidden, then it will be automatically shown.
	/// </remarks>
	void RefreshTooltipsOfContext(VisualElement context);

	bool IsDragging { get; }
}

public class TooltipMessage
{
	public string IconClassName;
	public string Text;

	public TooltipMessage(string iconClassName = null, string text = null)
	{
		IconClassName = iconClassName;
		Text = text;
	}

	public static readonly TooltipMessage JumpToSourceFile = new(BaseIcons.JumpToSourceFile, "<i>Left-Click to jump to source file.</i>");
	public static readonly TooltipMessage OpenLinkInWebBrowser = new(BaseIcons.OpenLinkInWebBrowser, "<i>Ctrl + Left-Click to open link.</i>");
}

public class TooltipCollection
{
	public TooltipMessage[] Tooltips;
	public VisualElement TooltipAnchor;
}

public class TooltipWithAnchor
{
	public TooltipMessage Tooltip;
	public VisualElement TooltipAnchor;
}

[Flags]
public enum TooltipShowMode
{
	Normal = 0,
	AppendToExisting = 1 << 0,
	DoNotShowWhenUserIsDragging = 1 << 1,
}

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

	static void _Show(VisualElement eventTarget, ITooltip t, Vector2 mousePos, bool pushToStack, ElementAnchorPoint anchorPoint)
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
						t.ShowTooltipAtMouse(eventTarget, tooltipText, iconClassName, mousePos, pushToStack);
						break;
					default:
						t.ShowTooltipAt(eventTarget, eventTarget, tooltipText, iconClassName, anchorPoint, pushToStack);
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
						t.ShowTooltipAtMouse(eventTarget, tooltipMessage.Text, tooltipMessage.IconClassName, mousePos, pushToStack);
						break;
					default:
						t.ShowTooltipAt(eventTarget, eventTarget, tooltipMessage.Text, tooltipMessage.IconClassName, anchorPoint, pushToStack);
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
						t.ShowTooltipAtMouse(eventTarget, tooltipWithAnchor.Tooltip.Text, tooltipWithAnchor.Tooltip.IconClassName, mousePos, pushToStack);
						break;
					default:
						t.ShowTooltipAt(eventTarget, tooltipWithAnchor.TooltipAnchor, tooltipWithAnchor.Tooltip.Text, tooltipWithAnchor.Tooltip.IconClassName, anchorPoint, pushToStack);
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

				if (!pushToStack)
				{
					t.ClearTooltipMessages();
				}

				bool addedAtLeastOne = false;
				for (int i = tooltipMessageList.Count - 1; i >= 0; --i)
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

			if (!pushToStack)
			{
				t.ClearTooltipMessages();
			}

			bool addedAtLeastOne = false;
			for (int i = tooltipMessageArray.Length - 1; i >= 0; --i)
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

[UxmlElement]
public partial class Tooltip : VisualElement
{
	const string TemplateResourcesPath = "DLD UIToolkit/Tooltip";
	const string FollowMouseStyleClass = "dld-tooltip__bg--follow-mouse";
	const string FollowElementStyleClass = "dld-tooltip__bg--follow-element";
	const string TooltipIconStyleClass = "dld-tooltip__icon";

	enum ShowType
	{
		None,
		FollowMouseCursor,
		AttachToVisualElement,
	}

	ShowType _showType = ShowType.None;

	struct TooltipRow
	{
		public VisualElement Context;
		public VisualElement Container;
		public VisualElement Icon;
		public Label Text;
	}

	readonly List<TooltipRow> _messageRows = new(10);
	int _messageRowCountUsed;
	VisualElement _lastContext;
	ElementAnchorPoint _lastAnchorPoint;

	readonly EventCallback<PointerMoveEvent> _onPointerMove;

	IDragStatus _dragStatus;

	public Tooltip()
	{
		var asset = Resources.Load<VisualTreeAsset>(TemplateResourcesPath);
		asset.CloneTree(this);
		this.RemoveTemplateContainer("Tooltip");

		// -----------------------------------

		style.display = DisplayStyle.None; // hide at first

		_onPointerMove = OnPointerMove;

		RegisterCallback<AttachToPanelEvent, Tooltip>((e, t) => e.destinationPanel.visualTree.RegisterCallback(t._onPointerMove), this);
		RegisterCallback<DetachFromPanelEvent, Tooltip>((e, t) => e.originPanel?.visualTree.UnregisterCallback(t._onPointerMove), this);
	}

	// ==================================================================================================

	public void SetDragStatus(IDragStatus newDragStatus)
	{
		_dragStatus = newDragStatus;
	}

	public bool IsDragging => _dragStatus != null && _dragStatus.IsDragging;

	public void PushToStack(VisualElement context, string text, string iconClassName = null)
	{
		_lastContext = context;
		Set(context, text, iconClassName, true);
	}

	public void ShowAtMouseCursor()
	{
		if (_messageRowCountUsed == 0)
		{
			return;
		}

		_lastAnchorPoint = ElementAnchorPoint.Mouse;
		_showType = ShowType.FollowMouseCursor;
		RemoveFromClassList(FollowElementStyleClass);
		AddToClassList(FollowMouseStyleClass);
		style.display = DisplayStyle.Flex;
	}

	public void ShowAtMouseCursor(Vector2 mousePos)
	{
		if (_messageRowCountUsed == 0)
		{
			return;
		}

		ShowAtMouseCursor();
		this.SetPosition(mousePos);
	}

	public void ShowAt(VisualElement anchorElement, ElementAnchorPoint anchorPoint)
	{
		_lastAnchorPoint = anchorPoint;

		if (_messageRowCountUsed == 0)
		{
			return;
		}

		Rect anchorRect = anchorElement.layout;
		var anchorPos = anchorPoint switch
		{
			ElementAnchorPoint.LowerRight => new Vector2(anchorRect.width, anchorRect.height),
			ElementAnchorPoint.Left => new Vector2(0, 0),
			ElementAnchorPoint.Right => new Vector2(anchorRect.width, 0),
			_ => new Vector2(0, anchorRect.height), // default is Bottom
		};
		var anchorWorldPos = anchorElement.LocalToWorld(anchorPos);
		var localPos = parent.WorldToLocal(anchorWorldPos);
		this.SetPosition(localPos);

		_showType = ShowType.AttachToVisualElement;
		RemoveFromClassList(FollowMouseStyleClass);
		AddToClassList(FollowElementStyleClass);
		style.display = DisplayStyle.Flex;
	}

	public void ShowAtMouseCursor(VisualElement context, string text, string iconClassName = null, bool pushToStack = false)
	{
		_lastContext = context;
		Set(context, text, iconClassName, pushToStack);

		if (pushToStack && style.display == DisplayStyle.Flex)
		{
			// tooltip is already shown
			// do not change where it is shown
			return;
		}
		ShowAtMouseCursor();
	}

	public void ShowAtMouseCursor(VisualElement context, string text, string iconClassName, Vector2 mousePos, bool pushToStack = false)
	{
		_lastContext = context;
		bool alreadyShown = pushToStack && style.display == DisplayStyle.Flex;

		ShowAtMouseCursor(context, text, iconClassName, pushToStack);

		if (alreadyShown)
		{
			return;
		}

		this.SetPosition(mousePos);
	}

	public void ShowAt(VisualElement context, VisualElement anchorElement, string text, string iconClassName, ElementAnchorPoint anchorPoint, bool pushToStack = false)
	{
		_lastContext = context;
		Set(context, text, iconClassName, pushToStack);

		if (pushToStack && style.display == DisplayStyle.Flex)
		{
			// tooltip is already shown
			// do not change where it is shown
			return;
		}
		ShowAt(anchorElement, anchorPoint);
	}

	public void ClearTooltipMessages()
	{
		Clear();
		_messageRowCountUsed = 0;
	}

	public void SetContext(VisualElement context)
	{
		_lastContext = context;
	}

	public void Hide()
	{
		_lastContext = null;

		_Hide();
	}

	void _Hide()
	{
		_showType = ShowType.None;
		RemoveFromClassList(FollowMouseStyleClass);
		RemoveFromClassList(FollowElementStyleClass);
		style.display = DisplayStyle.None;

		if (_messageRowCountUsed > 0)
		{
			Clear();
			_messageRowCountUsed = 0;
		}
	}

	public void HideIfContextIs(VisualElement context, bool popFromStack = false)
	{
		if (popFromStack)
		{
			while (_messageRowCountUsed > 0 && _messageRows[_messageRowCountUsed - 1].Context == context)
			{
				Debug.Assert(ReferenceEquals(_messageRows[_messageRowCountUsed - 1].Container, this[0]),
					$"_messageRows[{_messageRowCountUsed - 1}]: \"{_messageRows[_messageRowCountUsed - 1].Text.text}\", this[0]: \"{this[0].Q<Label>().text}\"");

				RemoveAt(0);
				_messageRowCountUsed -= 1;
			}

			Debug.Assert(_messageRowCountUsed == childCount,
				$"_messageRowCountUsed: {_messageRowCountUsed} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

			if (_messageRowCountUsed == 0)
			{
				_Hide();
			}
		}
		else
		{
			// check only topmost message, if context matches, hide
			if (_messageRowCountUsed > 0 && _messageRows[_messageRowCountUsed - 1].Context == context)
			{
				_Hide();
			}
		}
	}

	public void RefreshTooltipsOfContext(VisualElement context)
	{
		Debug.Assert(context != null, "RefreshTooltipsOfContext: Passed context is null");
		if (_lastContext != context)
		{
			return;
		}

		if (_messageRowCountUsed > 0)
		{
			Debug.Assert(_messageRowCountUsed == childCount,
				$"_messageRowCountUsed: {_messageRowCountUsed} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

			for (int n = _messageRowCountUsed - 1; n >= 0; --n)
			{
				if (_messageRows[n].Context == context)
				{
					int reversedIndex = _messageRowCountUsed - (n + 1);
					Debug.Assert(ReferenceEquals(_messageRows[n].Container, this[reversedIndex]),
						$"_messageRows[{n}]: \"{_messageRows[n].Text.text}\", this[{reversedIndex}]: \"{this[reversedIndex].Q<Label>().text}\"");

					RemoveAt(reversedIndex);
					--_messageRowCountUsed;
				}
			}
		}

		Debug.Assert(_messageRowCountUsed == childCount,
			$"_messageRowCountUsed: {_messageRowCountUsed} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

		switch (context.userData)
		{
			case string tooltipText:
			{
				if (!string.IsNullOrWhiteSpace(tooltipText))
				{
					(string updatedTooltipText, string iconClassName) = TooltipUtil.GetTooltipText(tooltipText);
					PushToStack(context, updatedTooltipText, iconClassName);
				}

				break;
			}
			case TooltipMessage tooltipMessage:
			{
				if (!string.IsNullOrWhiteSpace(tooltipMessage.Text))
				{
					PushToStack(context, tooltipMessage.Text, tooltipMessage.IconClassName);
				}

				break;
			}
			case TooltipWithAnchor tooltipWithAnchor:
			{
				if (!string.IsNullOrWhiteSpace(tooltipWithAnchor.Tooltip.Text))
				{
					PushToStack(context, tooltipWithAnchor.Tooltip.Text, tooltipWithAnchor.Tooltip.IconClassName);
				}
				break;
			}
			case TooltipCollection tooltipCollection:
			{
				TooltipMessage[] tooltipMessageArray = tooltipCollection.Tooltips;
				for (int i = tooltipMessageArray.Length - 1; i >= 0; --i)
				{
					if (tooltipMessageArray[i] == null || string.IsNullOrWhiteSpace(tooltipMessageArray[i].Text))
					{
						continue;
					}

					PushToStack(context, tooltipMessageArray[i].Text, tooltipMessageArray[i].IconClassName);
				}

				break;
			}
			case TooltipMessage[] tooltipMessageArray:
			{
				for (int i = tooltipMessageArray.Length - 1; i >= 0; --i)
				{
					if (tooltipMessageArray[i] == null || string.IsNullOrWhiteSpace(tooltipMessageArray[i].Text))
					{
						continue;
					}

					PushToStack(context, tooltipMessageArray[i].Text, tooltipMessageArray[i].IconClassName);
				}

				break;
			}
			case List<TooltipMessage> tooltipMessageList:
			{
				for (int i = tooltipMessageList.Count - 1; i >= 0; --i)
				{
					if (tooltipMessageList[i] == null || string.IsNullOrWhiteSpace(tooltipMessageList[i].Text))
					{
						continue;
					}

					PushToStack(context, tooltipMessageList[i].Text, tooltipMessageList[i].IconClassName);
				}

				break;
			}
		}

		Debug.Assert(_messageRowCountUsed == childCount,
			$"_messageRowCountUsed: {_messageRowCountUsed} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

		if (_messageRowCountUsed == 0)
		{
			_Hide();
		}
		else
		{
			switch (_lastAnchorPoint)
			{
				case ElementAnchorPoint.Mouse:
					ShowAtMouseCursor();
					break;
				default:
					ShowAt(context, _lastAnchorPoint);
					break;
			}
		}
	}

	// ==================================================================================================

	void OnPointerMove(PointerMoveEvent e)
	{
		if (_showType is ShowType.FollowMouseCursor or ShowType.None)
		{
			this.SetPosition(e.position);
		}
	}

	static TooltipRow CreateNewTooltipRow(VisualElement context, string text, string iconClassName = null)
	{
		var newContainer = new VisualElement();
		newContainer.AddToClassList("dld-tooltip__row");

		var newText = new Label(text);
		newText.AddToClassList("dld-tooltip__text");

		var newIcon = new VisualElement();
		if (!string.IsNullOrEmpty(iconClassName))
		{
			newIcon.AddToClassList(TooltipIconStyleClass);
			newIcon.AddToClassList(iconClassName);
		}

		newContainer.Add(newIcon);
		newContainer.Add(newText);

		return new TooltipRow
		{
			Context = context,
			Container = newContainer,
			Icon = newIcon,
			Text = newText
		};
	}

	void Set(VisualElement context, string text, string iconClassName = null, bool pushToStack = false)
	{
		if (pushToStack)
		{
			// todo: check if this tooltip is already in the stack

			if (_messageRows.Count == _messageRowCountUsed) // used up all existing rows, make a new one
			{
				var newTooltipRow = CreateNewTooltipRow(context, text, iconClassName);

				_messageRows.Add(newTooltipRow);
				_messageRowCountUsed += 1;
				Insert(0, newTooltipRow.Container);
			}
			else // reuse an existing row
			{
				var nextAvailable = _messageRows[_messageRowCountUsed];
				{
					// update context
					nextAvailable.Context = context;
					_messageRows[_messageRowCountUsed] = nextAvailable;
				}
				nextAvailable.Text.text = text;
				if (!string.IsNullOrEmpty(iconClassName))
				{
					nextAvailable.Icon.style.display = DisplayStyle.Flex;
					nextAvailable.Icon.ClearClassList();
					nextAvailable.Icon.AddToClassList(TooltipIconStyleClass);
					nextAvailable.Icon.AddToClassList(iconClassName);
				}
				else
				{
					nextAvailable.Icon.style.display = DisplayStyle.None;
				}

				Insert(0, nextAvailable.Container);
				_messageRowCountUsed += 1;
			}
		}
		else
		{
			while (_messageRowCountUsed > 1)
			{
				RemoveAt(0);
				--_messageRowCountUsed;
			}

			_messageRowCountUsed = 1;
			if (_messageRows.Count == 0)
			{
				var newTooltipRow = CreateNewTooltipRow(context, text, iconClassName);
				_messageRows.Add(newTooltipRow);
				Insert(0, newTooltipRow.Container);
			}
			else
			{
				Insert(0, _messageRows[0].Container);
			}

			var lastMessage = _messageRows[0];
			{
				// update context
				lastMessage.Context = context;
				_messageRows[0] = lastMessage;
			}

			lastMessage.Text.text = text;
			if (!string.IsNullOrWhiteSpace(iconClassName))
			{
				lastMessage.Icon.style.display = DisplayStyle.Flex;
				lastMessage.Icon.ClearClassList();
				lastMessage.Icon.AddToClassList(TooltipIconStyleClass);
				lastMessage.Icon.AddToClassList(iconClassName);
			}
			else
			{
				lastMessage.Icon.style.display = DisplayStyle.None;
			}
		}

		Debug.Assert(_messageRowCountUsed == childCount,
			$"_messageRowCountUsed: {_messageRowCountUsed} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");
	}
}

}
