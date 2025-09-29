// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

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
	VisualElement _lastContext;
	VisualElement _lastAnchorElement;
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

	public void Append(VisualElement context, string text, string iconClassName = null, int indexToInsertInto = -1)
	{
		_lastContext = context;
		Set(context, text, iconClassName, true, indexToInsertInto);
	}

	public void ShowAtMouseCursor()
	{
		if (_messageRows.Count == 0)
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
		if (_messageRows.Count == 0)
		{
			return;
		}

		ShowAtMouseCursor();
		this.SetPosition(mousePos);
	}

	public void ShowAt(VisualElement anchorElement, ElementAnchorPoint anchorPoint)
	{
		_lastAnchorPoint = anchorPoint;
		_lastAnchorElement = anchorElement;

		if (_messageRows.Count == 0)
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
		_messageRows.Clear();
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

		if (_messageRows.Count > 0)
		{
			Clear();
			_messageRows.Clear();
		}
	}

	public void HideIfContextIs(VisualElement context, bool removeOnlyLastMessagesWithMatchingContext = false)
	{
		if (removeOnlyLastMessagesWithMatchingContext)
		{
			// remove all messages that match the specified context
			for (int n = _messageRows.Count - 1; n >= 0; --n)
			{
				if (_messageRows[n].Context == context)
				{
					Debug.Assert(ReferenceEquals(_messageRows[n].Container, this[n]),
						$"Refreshing for {context.name ?? context.GetType().Name}: _messageRows[{n}]: \"{_messageRows[n].Text.text}\" {_messageRows[n].Container.name}, this[{n}]: \"{this[n].Q<Label>().text}\" {this[n].name}");

					RemoveAt(n);
					_messageRows.RemoveAt(n);
				}
			}

			Debug.Assert(_messageRows.Count == childCount,
				$"_messageRows.Count: {_messageRows.Count} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

			if (_messageRows.Count == 0)
			{
				_Hide();
			}
		}
		else
		{
			// check only topmost message, if context matches, hide
			if (_messageRows.Count > 0 && _messageRows[^1].Context == context)
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
			for (int n = 0; n < _messageRows.Count; ++n)
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

		int firstIdxFound = -1;
		if (_messageRows.Count > 0)
		{
			// remove all messages that match the specified context

			Debug.Assert(_messageRows.Count == childCount,
				$"Refreshing for {context.name ?? context.GetType().Name}: _messageRows.Count: {_messageRows.Count} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

			for (int n = _messageRows.Count - 1; n >= 0; --n)
			{
				if (_messageRows[n].Context == context)
				{
					Debug.Assert(ReferenceEquals(_messageRows[n].Container, this[n]),
						$"Refreshing for {context.name ?? context.GetType().Name}: _messageRows[{n}]: \"{_messageRows[n].Text.text}\" {_messageRows[n].Container.name}, this[{n}]: \"{this[n].Q<Label>().text}\" {this[n].name}");

					RemoveAt(n);
					_messageRows.RemoveAt(n);
					firstIdxFound = n;
				}
			}
		}

		Debug.Assert(_messageRows.Count == childCount,
			$"Refreshing for {context.name ?? context.GetType().Name}: _messageRows.Count: {_messageRows.Count} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

		switch (context.userData)
		{
			case string tooltipText:
			{
				if (!string.IsNullOrWhiteSpace(tooltipText))
				{
					(string updatedTooltipText, string iconClassName) = TooltipUtil.GetTooltipText(tooltipText);
					Append(context, updatedTooltipText, iconClassName, firstIdxFound);
				}

				break;
			}
			case TooltipMessage tooltipMessage:
			{
				if (!string.IsNullOrWhiteSpace(tooltipMessage.Text))
				{
					Append(context, tooltipMessage.Text, tooltipMessage.IconClassName, firstIdxFound);
				}

				break;
			}
			case TooltipWithAnchor tooltipWithAnchor:
			{
				if (!string.IsNullOrWhiteSpace(tooltipWithAnchor.Tooltip.Text))
				{
					Append(context, tooltipWithAnchor.Tooltip.Text, tooltipWithAnchor.Tooltip.IconClassName, firstIdxFound);
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

					Append(context, tooltipMessageArray[i].Text, tooltipMessageArray[i].IconClassName, firstIdxFound++);
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

					Append(context, tooltipMessageArray[i].Text, tooltipMessageArray[i].IconClassName, firstIdxFound++);
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

					Append(context, tooltipMessageList[i].Text, tooltipMessageList[i].IconClassName, firstIdxFound++);
				}

				break;
			}
		}

		Debug.Assert(_messageRows.Count == childCount,
			$"_messageRows.Count: {_messageRows.Count} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

		if (_messageRows.Count == 0)
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
					ShowAt(_lastAnchorElement, _lastAnchorPoint);
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

	void Set(VisualElement context, string text, string iconClassName = null, bool append = false, int indexToInsertInto = -1)
	{
		if (append)
		{
			// todo: check if a tooltip with same context, text, and icon is already in the list?

			var newTooltipRow = CreateNewTooltipRow(context, text, iconClassName);

			if (indexToInsertInto == -1 || indexToInsertInto >= _messageRows.Count)
			{
				_messageRows.Add(newTooltipRow);
				Add(newTooltipRow.Container);
			}
			else
			{
				_messageRows.Insert(indexToInsertInto, newTooltipRow);
				Insert(indexToInsertInto, newTooltipRow.Container);
			}
		}
		else
		{
			// No append means we replace entire list of tooltip messages with only one.

			// Remove all existing TooltipRow.Containers currently parented to the Tooltip.
			Clear();
			_messageRows.Clear();

			// --------------------------------------------------------------------

			// Put in a single TooltipRow.Container into the Tooltip.

			var newTooltipRow = CreateNewTooltipRow(context, text, iconClassName);
			_messageRows.Add(newTooltipRow);
			Add(newTooltipRow.Container);

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

		Debug.Assert(_messageRows.Count == childCount,
			$"Mismatch on message count and child count. _messageRows.Count: {_messageRows.Count} childCount: {childCount} _messageRows.Count: {_messageRows.Count}\nWhen adding tooltip \"{text}\"");
	}
}

}
