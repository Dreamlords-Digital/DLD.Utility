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
	/// </summary>
	/// <param name="context">The thing that caused the tooltip to be shown.</param>
	/// <param name="text">Text to be shown as the tooltip message.</param>
	/// <param name="iconClassName">Optional icon drawn before the tooltip text. This is a USS style name.</param>
	/// <param name="mousePos">Initial mouse position. This ensures the tooltip is at the correct position the moment it appears.</param>
	/// <param name="append">
	///    When set to true, the specified tooltip text will be added below all other currently shown text already on the tooltip.
	///    When set to false, the specified tooltip text will replace all other currently shown text in the tooltip.
	/// </param>
	/// <remarks>
	///    Call <see cref="HideTooltip"/> or <see cref="HideTooltipIfContextIs"/> to hide it.
	/// </remarks>
	void ShowTooltipAtMouse(VisualElement context, string text, string iconClassName, Vector2 mousePos, bool append = false);

	/// <summary>
	///    Show a tooltip message that is anchored to a <see cref="VisualElement"/>.
	/// </summary>
	/// <param name="context">The thing that caused the tooltip to be shown.</param>
	/// <param name="anchorElement">Where to anchor the tooltip.</param>
	/// <param name="text">Text to be shown as the tooltip message.</param>
	/// <param name="iconClassName">Optional icon drawn before the tooltip text. This is a USS style name.</param>
	/// <param name="anchorPoint">In what position the tooltip should be in, relative to the anchor.</param>
	/// <param name="append">
	///    When set to true, the specified tooltip text will be added below all other currently shown text already on the tooltip.
	///    When set to false, the specified tooltip text will replace all other currently shown text in the tooltip.
	/// </param>
	/// <remarks>
	///    Call <see cref="HideTooltip"/> or <see cref="HideTooltipIfContextIs"/> to hide it.
	/// </remarks>
	void ShowTooltipAt(VisualElement context, VisualElement anchorElement, string text, string iconClassName, ElementAnchorPoint anchorPoint, bool append = false);

	/// <summary>
	///    Add another message to the tooltip.
	/// </summary>
	/// <remarks>
	///    The new message will be shown below all the existing messages.
	/// </remarks>
	/// <param name="context">The thing that caused the tooltip to be shown.</param>
	/// <param name="text"></param>
	/// <param name="iconClassName">Optional icon drawn before the tooltip text. This is a USS style name.</param>
	void AddToTooltip(VisualElement context, string text, string iconClassName = null);

	void HideTooltip();

	/// <summary>
	///    Remove all messages in the tooltip, without hiding it (assuming it's currently shown).
	/// </summary>
	void ClearTooltipMessages();

	/// <summary>
	///    Show the tooltip and make it follow the mouse cursor.
	/// </summary>
	/// <remarks>
	///    <para>
	///       This is meant to be called after calling <see cref="AddToTooltip"/> multiple times.
	///    </para>
	///    <para>
	///       Call <see cref="HideTooltip"/> or <see cref="HideTooltipIfContextIs"/> to hide it.
	///    </para>
	/// </remarks>
	void ShowAtMouseCursor(Vector2 mousePos);

	/// <summary>
	///    Show the tooltip and make it anchored to a <see cref="VisualElement"/>.
	/// </summary>
	/// <param name="context">The thing that caused the tooltip to be shown.</param>
	/// <param name="anchorPoint">In what position the tooltip should be in, relative to the anchor.</param>
	/// <remarks>
	///    <para>
	///       This is meant to be called after calling <see cref="AddToTooltip"/> multiple times.
	///    </para>
	///    <para>
	///       Call <see cref="HideTooltip"/> or <see cref="HideTooltipIfContextIs"/> to hide it.
	///    </para>
	///    <para>
	///       If the <paramref name="context"/> has a <see cref="TooltipCollection"/> assigned to its <see cref="VisualElement.userData"/>,
	///       then the anchor specified in <see cref="TooltipCollection.TooltipAnchor"/> will be used as the anchor.
	///       Otherwise, the <paramref name="context"/> will be used as the anchor.
	///    </para>
	/// </remarks>
	void ShowAt(VisualElement context, ElementAnchorPoint anchorPoint);

	/// <summary>
	///    Assign an "owner" to the tooltip even if no tooltip text is currently shown.
	/// </summary>
	/// <param name="context"></param>
	/// <remarks>
	///    <para>
	///       This is used when the mouse is currently on a <see cref="VisualElement"/> that usually would display
	///       a tooltip, but currently isn't showing any.
	///    </para>
	///    <para>
	///       For example, let's say the mouse cursor moves into a textbox that shows an error tooltip message
	///       only when the text inside fails a regex. If the text inside didn't fail the regex, then no
	///       error tooltip will be shown, but <see cref="SetContext"/> should still be called.
	///       <see cref="RefreshTooltipsOfContext"/> would then be called afterwards when the conditions
	///       are met (e.g. mouse cursor never left the textbox, but the text inside the textbox has changed)
	///       to make any relevant tooltips show up.
	///    </para>
	/// </remarks>
	void SetContext(VisualElement context);

	/// <summary>
	///    If the context that was last assigned to the tooltip matches the one specified, the tooltip is hidden.
	/// </summary>
	/// <param name="context">The thing that caused the tooltip to be shown with a specific text and icon.</param>
	/// <param name="removeOnlyLastMessagesWithMatchingContext">
	///    Only remove the most recent tooltip messages (if it's showing multiple tooltip messages)
	///    that the <paramref name="context"/> owns, instead of hiding the entire tooltip.
	/// </param>
	/// <remarks>
	///    Basically, this ensures a <see cref="VisualElement"/> that showed a tooltip
	///    will hide the tooltip only if the tooltip is still showing that VisualElement's message.
	///    If the tooltip happened to be showing the text given by a different VisualElement already at that point,
	///    then it won't be hidden.
	/// </remarks>
	void HideTooltipIfContextIs(VisualElement context, bool removeOnlyLastMessagesWithMatchingContext = false);

	/// <summary>
	///    Removes all currently displayed messages of the specified <paramref name="context"/>,
	///    then re-adds the up-to-date messages provided by the specified <paramref name="context"/>.
	/// </summary>
	/// <remarks>
	///    If the tooltip isn't shown on the specified context, then this aborts.<br/>
	///    If the tooltip ends up with no more messages, then the tooltip will be automatically hidden.<br/>
	///    If the tooltip ends up having messages, and it's currently hidden, then it will be automatically shown.
	/// </remarks>
	void RefreshTooltipsOfContext(VisualElement context);

	/// <summary>
	///    Whether the user is currently performing a drag-and-drop operation or not.
	/// </summary>
	/// <remarks>
	///    This is used by tooltip displayers that do not want their tooltip to be shown
	///    when the user is performing a drag-and-drop operation.
	/// </remarks>
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

	public void Append(VisualElement context, string text, string iconClassName = null)
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

	public void ShowAtMouseCursor(VisualElement context, string text, string iconClassName = null, bool append = false)
	{
		_lastContext = context;
		Set(context, text, iconClassName, append);

		if (append && style.display == DisplayStyle.Flex)
		{
			// tooltip is already shown
			// do not change where it is shown
			return;
		}
		ShowAtMouseCursor();
	}

	public void ShowAtMouseCursor(VisualElement context, string text, string iconClassName, Vector2 mousePos, bool append = false)
	{
		_lastContext = context;
		bool alreadyShown = append && style.display == DisplayStyle.Flex;

		ShowAtMouseCursor(context, text, iconClassName, append);

		if (alreadyShown)
		{
			return;
		}

		this.SetPosition(mousePos);
	}

	public void ShowAt(VisualElement context, VisualElement anchorElement, string text, string iconClassName, ElementAnchorPoint anchorPoint, bool append = false)
	{
		_lastContext = context;
		Set(context, text, iconClassName, append);

		if (append && style.display == DisplayStyle.Flex)
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

	public void HideIfContextIs(VisualElement context, bool removeOnlyLastMessagesWithMatchingContext = false)
	{
		if (removeOnlyLastMessagesWithMatchingContext)
		{
			// keep removing last tooltip message if context is same
			while (_messageRowCountUsed > 0 && _messageRows[_messageRowCountUsed - 1].Context == context)
			{
				Debug.Assert(ReferenceEquals(_messageRows[_messageRowCountUsed - 1].Container, this[_messageRowCountUsed - 1]),
					$"_messageRows[{_messageRowCountUsed - 1}]: \"{_messageRows[_messageRowCountUsed - 1].Text.text}\", this[0]: \"{this[_messageRowCountUsed - 1].Q<Label>().text}\"");

				RemoveAt(_messageRowCountUsed-1);
				--_messageRowCountUsed;
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

	/// <inheritdoc cref="ITooltip.RefreshTooltipsOfContext"/>
	public void RefreshTooltipsOfContext(VisualElement context)
	{
		Debug.Assert(context != null, "RefreshTooltipsOfContext: Passed context is null");

		bool foundInAtLeastOneContext = context == _lastContext;
		if (!foundInAtLeastOneContext)
		{
			for (int n = 0; n < _messageRowCountUsed; ++n)
			{
				if (context == _messageRows[n].Context)
				{
					foundInAtLeastOneContext = true;
					break;
				}
			}
		}

		if (!foundInAtLeastOneContext)
		{
			// no longer relevant because tooltip is now at a different context
			return;
		}

		if (_messageRowCountUsed > 0)
		{
			// remove all messages that match the specified context

			Debug.Assert(_messageRowCountUsed == childCount,
				$"Refreshing for {context.name ?? context.GetType().Name}: _messageRowCountUsed: {_messageRowCountUsed} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

			for (int n = _messageRowCountUsed - 1; n >= 0; --n)
			{
				if (_messageRows[n].Context == context)
				{
					Debug.Assert(ReferenceEquals(_messageRows[n].Container, this[n]),
						$"Refreshing for {context.name ?? context.GetType().Name}: _messageRows[{n}]: \"{_messageRows[n].Text.text}\" {_messageRows[n].Container.name}, this[{n}]: \"{this[n].Q<Label>().text}\" {this[n].name}");

					RemoveAt(n);
					--_messageRowCountUsed;
				}
			}
		}

		Debug.Assert(_messageRowCountUsed == childCount,
			$"Refreshing for {context.name ?? context.GetType().Name}: _messageRowCountUsed: {_messageRowCountUsed} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

		switch (context.userData)
		{
			case string tooltipText:
			{
				if (!string.IsNullOrWhiteSpace(tooltipText))
				{
					(string updatedTooltipText, string iconClassName) = TooltipUtil.GetTooltipText(tooltipText);
					Append(context, updatedTooltipText, iconClassName);
				}

				break;
			}
			case TooltipMessage tooltipMessage:
			{
				if (!string.IsNullOrWhiteSpace(tooltipMessage.Text))
				{
					Append(context, tooltipMessage.Text, tooltipMessage.IconClassName);
				}

				break;
			}
			case TooltipWithAnchor tooltipWithAnchor:
			{
				if (!string.IsNullOrWhiteSpace(tooltipWithAnchor.Tooltip.Text))
				{
					Append(context, tooltipWithAnchor.Tooltip.Text, tooltipWithAnchor.Tooltip.IconClassName);
				}
				break;
			}
			case TooltipCollection tooltipCollection:
			{
				TooltipMessage[] tooltipMessageArray = tooltipCollection.Tooltips;
				for (int i = 0; i < tooltipMessageArray.Length; ++i)
				{
					if (tooltipMessageArray[i] == null || string.IsNullOrWhiteSpace(tooltipMessageArray[i].Text))
					{
						continue;
					}

					Append(context, tooltipMessageArray[i].Text, tooltipMessageArray[i].IconClassName);
				}

				break;
			}
			case TooltipMessage[] tooltipMessageArray:
			{
				for (int i = 0; i < tooltipMessageArray.Length; ++i)
				{
					if (tooltipMessageArray[i] == null || string.IsNullOrWhiteSpace(tooltipMessageArray[i].Text))
					{
						continue;
					}

					Append(context, tooltipMessageArray[i].Text, tooltipMessageArray[i].IconClassName);
				}

				break;
			}
			case List<TooltipMessage> tooltipMessageList:
			{
				for (int i = 0; i < tooltipMessageList.Count; ++i)
				{
					if (tooltipMessageList[i] == null || string.IsNullOrWhiteSpace(tooltipMessageList[i].Text))
					{
						continue;
					}

					Append(context, tooltipMessageList[i].Text, tooltipMessageList[i].IconClassName);
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

	void Set(VisualElement context, string text, string iconClassName = null, bool append = false)
	{
		if (append)
		{
			// todo: check if a tooltip with same context, text, and icon is already in the list?

			if (_messageRows.Count == _messageRowCountUsed) // used up all existing rows, make a new one
			{
				var newTooltipRow = CreateNewTooltipRow(context, text, iconClassName);

				_messageRows.Add(newTooltipRow);
				_messageRowCountUsed += 1;
				newTooltipRow.Container.name = $"{_messageRowCountUsed}";
				Add(newTooltipRow.Container);
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

				Add(nextAvailable.Container);
				_messageRowCountUsed += 1;
				nextAvailable.Container.name = $"{_messageRowCountUsed}";
			}
		}
		else
		{
			// No append means we replace entire list of tooltip messages with only one.

			// Remove all existing TooltipRow.Containers currently parented to the Tooltip.
			Clear();

			// _messageRowCountUsed might have been 0,
			// so ensure it is now 1.
			_messageRowCountUsed = 1;

			// --------------------------------------------------------------------

			// Put in a single TooltipRow.Container into the Tooltip.
			if (_messageRows.Count == 0)
			{
				// first ever message to be added
				var newTooltipRow = CreateNewTooltipRow(context, text, iconClassName);
				_messageRows.Add(newTooltipRow);
				Add(newTooltipRow.Container);
				newTooltipRow.Container.name = "1";
			}
			else
			{
				// reuse existing first message struct
				Add(_messageRows[0].Container);
				_messageRows[0].Container.name = "1";
			}

			// --------------------------------------------------------------------

			var firstMessage = _messageRows[0];
			firstMessage.Context = context;
			firstMessage.Text.text = text;
			if (!string.IsNullOrWhiteSpace(iconClassName))
			{
				firstMessage.Icon.style.display = DisplayStyle.Flex;
				firstMessage.Icon.ClearClassList();
				firstMessage.Icon.AddToClassList(TooltipIconStyleClass);
				firstMessage.Icon.AddToClassList(iconClassName);
			}
			else
			{
				firstMessage.Icon.style.display = DisplayStyle.None;
			}

			// ensure our List is updated with the new values we assigned
			_messageRows[0] = firstMessage;
		}

		Debug.Assert(_messageRowCountUsed == childCount,
			$"_messageRowCountUsed: {_messageRowCountUsed} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");
	}
}

}
