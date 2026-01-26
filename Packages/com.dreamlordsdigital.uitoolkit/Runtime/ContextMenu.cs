// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

/// <summary>
///    Allows setting up the items in the context menu and what happens when the user clicks on them.
///    Also allows showing that context menu.
/// </summary>
public interface IContextMenu
{
	VisualElement Root { get; }

	void ClearMenu();
	void DoAltBgStyling(bool doAltBgStyling);
	void SetAlwaysLeaveSpaceForSelectedIndicator(bool alwaysLeaveSpaceForSelectedIndicator);
	void AddLabel(string text);
	void AddSeparator();

	VisualElement AddMenu(
		string label, string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
		IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null);

	void AddMenu(
		string label, string tooltip, string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
		IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null);

	void AddMenu(
		string label, TooltipMessage[] menuTooltip, string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
		IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null);

	void AddMenu(
		string label, List<TooltipMessage> menuTooltip, string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
		IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null);

	/// <summary>
	///    Show the selected indicator only on the specified item.
	///    All other items will have their selected indicators cleared.
	/// </summary>
	void ChangeSelected(int newSelectedIdx);

	/// <summary>
	///    Find the menu item that has the specified userArg1 and
	///    show the selected indicator on it only.
	///    All other items will have their selected indicators cleared.
	/// </summary>
	void ChangeSelected(object userArg1);

	/// <summary>
	///    Show/hide the selected indicator of the specified item.
	///    Other items are not edited.
	/// </summary>
	/// <param name="menuIdx">The item to edit, by index.</param>
	/// <param name="selected">Whether to show the selected indicator or not.</param>
	void SetSelected(int menuIdx, bool selected);

	void Show(Vector2 position, IContextMenuListener listener = null);
	void Show(ContextClickEvent e, IContextMenuListener listener = null);
	void Show(PointerDownEvent e, IContextMenuListener listener = null);
	void Show(VisualElement ve, ElementAnchorPoint anchorPoint = ElementAnchorPoint.Bottom, IContextMenuListener listener = null);

	bool WasLastShownOn(VisualElement ve);
}

public struct ContextMenuParams
{
	public Vector2 ContextMenuPosition;
	public int MenuItemIndex;
	public Label MenuItemLabel;
	public object MenuItemTooltip;
	public object UserArg1;
	public object UserArg2;
}

public interface IContextMenuListener
{
	void OnContextMenuChosen(ContextMenuParams parameters);
	void OnContextMenuClosed(bool userCancelled);
}

[System.Flags]
public enum ContextMenuItemStyle : byte
{
	Standard = 0,
	Selected = 1,
	Disabled = 2,
	Warning = 4,
	Error = 8,
}

public class ContextMenu : VisualElement, IContextMenu
{
	const string TemplateResourcesPath = "DLD UIToolkit/ContextMenu";
	const string EntryTemplateResourcesPath = "DLD UIToolkit/ContextMenuEntry";
	const string SeparatorTemplateResourcesPath = "DLD UIToolkit/ContextMenuEntrySeparator";
	const string MenuName = "ContextMenuBg";
	const string SelectedIndicatorName = "SelectedIndicator";
	const string IconName = "Icon";

	const string LabelStyleClass = "dld-context-menu-entry-label";
	const string EntryAltStyleClass = "dld-context-menu-entry-container--alt-bg";
	const string PressedEntryStyleClass = "dld-context-menu-entry-container--active";
	const string DisabledEntryStyleClass = "dld-context-menu-entry-container--disabled";
	const string MenuAsDropdownStyleClass = "dld-context-menu--as-dropdown";
	const string MenuAsDropdownLongerThanButtonStyleClass = "dld-context-menu--as-dropdown--longer";

	const string SelectedEntryLabelStyleClass = "dld-context-menu-entry__label--selected";
	public const string ErrorEntryLabelStyleClass = "dld-context-menu-entry__label--error";
	public const string WarningEntryLabelStyleClass = "dld-context-menu-entry__label--warning";

	const float DefaultMouseMoveDistanceForInstantClose = 10;

	static readonly CustomStyleProperty<float> DropdownButtonFitWidthAdjust = new("--dropdown--button-fit-width-adjust");

	// ==================================================================================

	float _dropdownButtonFitWidthAdjust;

	readonly VisualElement _menu;

	readonly System.Action _delayedFocus;

	readonly VisualTreeAsset _entryAsset;
	readonly VisualTreeAsset _separatorAsset;

	VisualElement _elementShownOn;
	ElementAnchorPoint _elementAnchorPoint;

	bool _doAltBgStyling;
	bool _mouseMovedDuringMouseDown;
	bool _alwaysLeaveSpaceForSelectedIndicator;

	Focusable _focusTargetAfterClose;

	ITooltip _tooltip;
	IContextMenuListener _listener;

	// ==================================================================================

	public VisualElement Root => this;

	public ContextMenu()
	{
		_delayedFocus = DelayedFocus;

		_entryAsset = Resources.Load<VisualTreeAsset>(EntryTemplateResourcesPath);
		_separatorAsset = Resources.Load<VisualTreeAsset>(SeparatorTemplateResourcesPath);

		// -----------------------------------

		var asset = Resources.Load<VisualTreeAsset>(TemplateResourcesPath);
		asset.CloneTree(this);
		this.RemoveTemplateContainer("ContextMenu");

		// -----------------------------------

		style.display = DisplayStyle.None;

		RegisterCallback<MouseDownEvent, ContextMenu>((e, c) => c.OnPressOutside(e), this);
		RegisterCallback<MouseMoveEvent, ContextMenu>((e, c) => c.OnMouseMove(e), this);
		RegisterCallback<MouseUpEvent, ContextMenu>((e, c) => c.OnMouseUpOutside(e), this);

		_menu = this.Q<VisualElement>(MenuName);
		_menu.RegisterCallback<MouseDownEvent>(e => e.StopPropagation());
		_menu.RegisterCallback<GeometryChangedEvent, ContextMenu>((e, c) => c.OnMenuResized(e), this);

		RegisterCallback<KeyDownEvent, ContextMenu>((e, c) => c.OnPressKey(e), this);

		RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
	}

	void OnCustomStyleResolved(CustomStyleResolvedEvent evt)
	{
		if (evt.customStyle.TryGetValue(DropdownButtonFitWidthAdjust, out float gotFloatValue))
		{
			_dropdownButtonFitWidthAdjust = gotFloatValue;
		}
		else
		{
			_dropdownButtonFitWidthAdjust = 0;
		}
	}

	public void SetFocusTargetAfterClose(Focusable newFocusTarget)
	{
		_focusTargetAfterClose = newFocusTarget;
	}

	public void SetTooltip(ITooltip newTooltip)
	{
		_tooltip = newTooltip;
	}

	public void DoAltBgStyling(bool doAltBgStyling)
	{
		_doAltBgStyling = doAltBgStyling;
	}

	public void SetAlwaysLeaveSpaceForSelectedIndicator(bool alwaysLeaveSpaceForSelectedIndicator)
	{
		_alwaysLeaveSpaceForSelectedIndicator = alwaysLeaveSpaceForSelectedIndicator;
	}

	// ==================================================================================

	public void ClearMenu()
	{
		_menu.Clear();
	}

	public void AddLabel(string text)
	{
		var newLabel = new Label(text);
		newLabel.AddToClassList(LabelStyleClass);
		_menu.Add(newLabel);
	}

	public void AddSeparator()
	{
		var createdSeparator = _separatorAsset.Instantiate();

		var entryContainer = createdSeparator.Q<VisualElement>("Entry");
		_menu.Add(entryContainer);
	}

	public VisualElement AddMenu(
		string label, string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
		IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null)
	{
		var createdEntry = _entryAsset.Instantiate();

		var entryContainer = createdEntry.Q<VisualElement>("Entry");

		bool showAsDisabled = (menuItemStyle & ContextMenuItemStyle.Disabled) != 0;
		if (showAsDisabled)
		{
			entryContainer.AddToClassList(DisabledEntryStyleClass);
		}

		var selectedIndicator = entryContainer.Q<VisualElement>(SelectedIndicatorName);
		selectedIndicator.userData = listener;

		var entryIcon = entryContainer.Q<VisualElement>(IconName);
		entryIcon.userData = userArg1;

		var entryLabel = entryContainer.Q<Label>();
		entryLabel.text = label;
		entryLabel.userData = userArg2;

		if ((menuItemStyle & ContextMenuItemStyle.Error) != 0)
		{
			entryLabel.AddToClassList(ErrorEntryLabelStyleClass);
		}
		else if ((menuItemStyle & ContextMenuItemStyle.Warning) != 0)
		{
			entryLabel.AddToClassList(WarningEntryLabelStyleClass);
		}

		if ((menuItemStyle & ContextMenuItemStyle.Selected) != 0)
		{
			selectedIndicator.style.display = DisplayStyle.Flex;
			selectedIndicator.AddToClassList(BaseIcons.SelectedInDropdown);
			entryLabel.AddToClassList(SelectedEntryLabelStyleClass);
		}
		else
		{
			selectedIndicator.style.display = DisplayStyle.None;
		}

		if (!showAsDisabled)
		{
			entryContainer.RegisterCallback<PointerDownEvent>(e =>
			{
				if (e.currentTarget is VisualElement v)
				{
					v.AddToClassList(PressedEntryStyleClass);
				}
			});

			entryContainer.RegisterCallback<PointerUpEvent, ContextMenu>((e, me) =>
			{
				if (e.currentTarget is not VisualElement targetElement)
				{
					return;
				}

				var gotSelectedIndicator = targetElement.Q<VisualElement>(SelectedIndicatorName);

				if (gotSelectedIndicator.userData is IContextMenuListener gotListener)
				{
					targetElement.Focus();
					var gotIcon = targetElement.Q<VisualElement>(IconName);
					var gotLabel = targetElement.Q<Label>();
					int menuIdx = targetElement.parent.IndexOf(targetElement);

					ContextMenuParams parameters = new()
					{
						ContextMenuPosition = me.LocalToWorld(me._menu.GetPositionXY()),
						MenuItemIndex = menuIdx,
						MenuItemLabel = gotLabel,
						MenuItemTooltip = targetElement.userData,
						UserArg1 = gotIcon.userData,
						UserArg2 = gotLabel.userData
					};
					gotListener.OnContextMenuChosen(parameters);
				}

				e.StopPropagation();
				me.Hide(false);
			}, this);
		}

		_menu.Add(entryContainer);

		if (_doAltBgStyling && _menu.childCount % 2 == 0)
		{
			entryContainer.AddToClassList(EntryAltStyleClass);
		}

		if (!string.IsNullOrEmpty(iconClassStyle))
		{
			entryIcon.AddToClassList(iconClassStyle);
		}

		return entryContainer;
	}

	public void AddMenu(
		string label, string menuTooltip = null,
		string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
		IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null)
	{
		var entryContainer = AddMenu(label, iconClassStyle, menuItemStyle, listener, userArg1, userArg2);
		entryContainer.RegisterTooltipDisplayer(_tooltip, menuTooltip, anchorPoint: ElementAnchorPoint.Right);
	}

	public void AddMenu(
		string label, TooltipMessage[] menuTooltip = null,
		string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
		IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null)
	{
		var entryContainer = AddMenu(label, iconClassStyle, menuItemStyle, listener, userArg1, userArg2);

		entryContainer.userData = menuTooltip;
		entryContainer.RegisterTooltipDisplayer(_tooltip, anchorPoint: ElementAnchorPoint.Right);
	}

	public void AddMenu(
		string label, List<TooltipMessage> menuTooltip = null,
		string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
		IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null)
	{
		var entryContainer = AddMenu(label, iconClassStyle, menuItemStyle, listener, userArg1, userArg2);

		entryContainer.userData = menuTooltip;
		entryContainer.RegisterTooltipDisplayer(_tooltip, anchorPoint: ElementAnchorPoint.Right);
	}

	// ==================================================================================

	public void ChangeMenuIcon(object userArg1, string iconClassStyleToAdd = null, string iconClassStyleToRemove = null)
	{
		for (int n = 0; n < _menu.childCount; ++n)
		{
			var gotIcon = _menu[n].Q<VisualElement>(IconName);
			if (gotIcon?.userData != null && gotIcon.userData.Equals(userArg1))
			{
				if (!string.IsNullOrEmpty(iconClassStyleToRemove))
				{
					gotIcon.RemoveFromClassList(iconClassStyleToRemove);
				}

				if (!string.IsNullOrEmpty(iconClassStyleToAdd))
				{
					gotIcon.AddToClassList(iconClassStyleToAdd);
				}

				// we assume the userArg1 value only appears once in the entire menu
				return;
			}
		}
	}

	public void ChangeSelected(int menuIdx)
	{
		for (int n = 0; n < _menu.childCount; ++n)
		{
			var selectedIndicator = _menu[n].Q<VisualElement>(SelectedIndicatorName);
			if (selectedIndicator == null)
			{
				// we're at a separator, not a menu entry (separators don't have a selected indicator)
				continue;
			}

			var entryLabel = _menu[n].Q<Label>();
			if (n == menuIdx)
			{
				selectedIndicator.AddToClassList(BaseIcons.SelectedInDropdown);
				entryLabel.AddToClassList(SelectedEntryLabelStyleClass);
			}
			else
			{
				selectedIndicator.RemoveFromClassList(BaseIcons.SelectedInDropdown);
				entryLabel.RemoveFromClassList(SelectedEntryLabelStyleClass);
			}
		}
	}

	public void ChangeSelected(object userArg1)
	{
		for (int n = 0; n < _menu.childCount; ++n)
		{
			var selectedIndicator = _menu[n].Q<VisualElement>(SelectedIndicatorName);
			if (selectedIndicator == null)
			{
				// we're at a separator, not a menu entry (separators don't have a selected indicator)
				continue;
			}

			var entryLabel = _menu[n].Q<Label>();
			var gotIcon = _menu[n].Q<VisualElement>(IconName);
			if (gotIcon?.userData != null && gotIcon.userData.Equals(userArg1))
			{
				selectedIndicator.AddToClassList(BaseIcons.SelectedInDropdown);
				entryLabel.AddToClassList(SelectedEntryLabelStyleClass);
			}
			else
			{
				selectedIndicator.RemoveFromClassList(BaseIcons.SelectedInDropdown);
				entryLabel.RemoveFromClassList(SelectedEntryLabelStyleClass);
			}
		}
	}

	public void SetSelected(int menuIdx, bool selected)
	{
		var entryLabel = _menu[menuIdx].Q<Label>();
		if (entryLabel == null)
		{
			return;
		}

		var selectedIndicator = _menu[menuIdx].Q<VisualElement>(SelectedIndicatorName);

		if (selected)
		{
			selectedIndicator?.AddToClassList(BaseIcons.SelectedInDropdown);
			entryLabel.AddToClassList(SelectedEntryLabelStyleClass);
		}
		else
		{
			selectedIndicator?.RemoveFromClassList(BaseIcons.SelectedInDropdown);
			entryLabel.RemoveFromClassList(SelectedEntryLabelStyleClass);
		}
	}

	// ==================================================================================

	public void Show(Vector2 position, IContextMenuListener listener = null)
	{
		UpdateIconVisibility();
		_listener = listener;
		_elementShownOn = null;

		_menu.SetPosition(position);
		style.display = DisplayStyle.Flex;
		focusable = true;
		_mouseMovedDuringMouseDown = false;

		schedule.Execute(_delayedFocus).ExecuteLater(0);
	}

	public void Show(ContextClickEvent e, IContextMenuListener listener = null)
	{
		var contextMenuMousePos = ((VisualElement)e.currentTarget).ChangeCoordinatesTo(this, e.localMousePosition);
		Show(contextMenuMousePos, listener);
	}

	public void Show(PointerDownEvent e, IContextMenuListener listener = null)
	{
		var mousePos = ((VisualElement)e.currentTarget).ChangeCoordinatesTo(this, e.localPosition);
		Show(mousePos, listener);
	}

	public void Show(VisualElement ve, ElementAnchorPoint anchorPoint = ElementAnchorPoint.Bottom, IContextMenuListener listener = null)
	{
		UpdateIconVisibility();
		_listener = listener;
		_elementShownOn = ve;
		_elementAnchorPoint = anchorPoint;

		var veLayout = ve.layout;
		var anchorPos = anchorPoint switch
		{
			ElementAnchorPoint.LowerRight => new Vector2(veLayout.width, veLayout.height),
			ElementAnchorPoint.Left => new Vector2(0, 0),
			ElementAnchorPoint.Right => new Vector2(veLayout.width, 0),
			_ => new Vector2(0, veLayout.height), // default is Bottom
		};
		var veWorldPos = ve.LocalToWorld(anchorPos);
		var localPos = this.WorldToLocal(veWorldPos);
		_menu.SetPosition(localPos);

		_menu.style.width = StyleKeyword.Null;
		_menu.AddToClassList(MenuAsDropdownStyleClass);
		_menu.RemoveFromClassList(MenuAsDropdownLongerThanButtonStyleClass);

		style.display = DisplayStyle.Flex;
		focusable = true;
		_mouseMovedDuringMouseDown = false;

		schedule.Execute(_delayedFocus).ExecuteLater(0);
	}

	public bool WasLastShownOn(VisualElement ve) => ve == _elementShownOn;

	public void Hide(bool userCancelled = true)
	{
		style.display = DisplayStyle.None;
		_menu.style.width = StyleKeyword.Null;
		Blur();
		_menu.RemoveFromClassList(MenuAsDropdownStyleClass);
		if (_focusTargetAfterClose != null)
		{
			_focusTargetAfterClose.Focus();
		}

		if (_listener != null)
		{
			_listener.OnContextMenuClosed(userCancelled);
			_listener = null;
		}
	}

	// ==================================================================================

	void OnMenuResized(GeometryChangedEvent evt)
	{
		if (evt.newRect.width == 0 || evt.newRect.height == 0 || _elementShownOn == null)
		{
			return;
		}

		float menuWidth = _menu.layout.width - _dropdownButtonFitWidthAdjust;
		float menuHeight = _menu.layout.height;
		float elementWidth = _elementShownOn.layout.width;

		if (menuWidth <= 0)
		{
			return;
		}

		switch (_elementAnchorPoint)
		{
			// -------------------------------------------------------------------------------------
			case ElementAnchorPoint.Bottom:
				//
				//                ********
				//                ********
				//                +------+
				//                |      |
				//                +------+
				//
				if (menuWidth <= elementWidth)
				{
					// fit context menu width to element
					_menu.style.width = elementWidth + _dropdownButtonFitWidthAdjust;
				}
				else
				{
					_menu.AddToClassList(MenuAsDropdownLongerThanButtonStyleClass);
				}

				break;

			// -------------------------------------------------------------------------------------
			case ElementAnchorPoint.LowerLeft:
				//
				//                ********
				//                ********
				//                +----------+
				//                |          |
				//                +----------+
				//
				_menu.AddToClassList(MenuAsDropdownLongerThanButtonStyleClass);
				break;

			// -------------------------------------------------------------------------------------
			case ElementAnchorPoint.LowerRight:
				//
				//                ********
				//                ********
				//            +----------+
				//            |          |
				//            +----------+
				//
				_menu.AddToPosition(new Vector2(-menuWidth, 0));
				break;

			// -------------------------------------------------------------------------------------
			case ElementAnchorPoint.UpperLeft:
				//
				//                +----------+
				//                |          |
				//                +----------+
				//                ********
				//                ********
				//
				_menu.AddToPosition(new Vector2(0, -menuHeight));
				break;

			// -------------------------------------------------------------------------------------
			case ElementAnchorPoint.UpperRight:
				//
				//            +----------+
				//            |          |
				//            +----------+
				//                ********
				//                ********
				//
				_menu.AddToPosition(new Vector2(-menuWidth, -menuHeight));
				break;

			// -------------------------------------------------------------------------------------
			case ElementAnchorPoint.Left:
				//
				//    +----------+********
				//    |          |********
				//    +----------+
				//
				_menu.AddToPosition(new Vector2(-menuWidth, 0));
				break;

			// -------------------------------------------------------------------------------------
			case ElementAnchorPoint.Right:
				//
				//                ********+----------+
				//                ********|          |
				//                        +----------+
				//
				break;

			// -------------------------------------------------------------------------------------
		}
	}

	void OnMouseMove(MouseMoveEvent _)
	{
		_mouseMovedDuringMouseDown = true;
	}

	void OnMouseUpOutside(MouseUpEvent e)
	{
		var mouseDelta = e.localMousePosition - _menu.GetPositionXY();
		bool mouseMovedFarEnough = mouseDelta.sqrMagnitude > (DefaultMouseMoveDistanceForInstantClose * DefaultMouseMoveDistanceForInstantClose);
		if (style.display == DisplayStyle.Flex && _mouseMovedDuringMouseDown && mouseMovedFarEnough)
		{
			Hide();
			e.StopPropagation();
		}
	}

	void OnPressOutside(MouseDownEvent e)
	{
		Hide();
		e.StopPropagation();
	}

	void OnPressKey(KeyDownEvent e)
	{
		if (style.display == DisplayStyle.None)
		{
			return;
		}

		switch (e.keyCode)
		{
			case KeyCode.Escape:
			{
				// Pressing Escape is considered a cancel.
				// Do the same thing as OnPressOutside.
				Hide();
				e.StopPropagation();
				break;
			}
			case KeyCode.DownArrow:
			{
				int focusedMenuIdx = GetFocusedMenuIdx(_menu);
				if (focusedMenuIdx == -1 || focusedMenuIdx == _menu.childCount - 1)
				{
					// wrap around and go to first menu entry
					_menu[0].Focus();
				}
				else
				{
					// select menu entry below
					++focusedMenuIdx;
					while (!_menu[focusedMenuIdx].focusable)
					{
						++focusedMenuIdx;
						if (focusedMenuIdx >= _menu.childCount)
						{
							focusedMenuIdx = 0;
						}
					}

					_menu[focusedMenuIdx].Focus();
				}

				break;
			}
			case KeyCode.UpArrow:
			{
				int focusedMenuIdx = GetFocusedMenuIdx(_menu);
				if (focusedMenuIdx <= 0)
				{
					// wrap around and go to final menu entry
					_menu[_menu.childCount - 1].Focus();
				}
				else
				{
					// select menu entry above
					--focusedMenuIdx;
					while (!_menu[focusedMenuIdx].focusable)
					{
						--focusedMenuIdx;
						if (focusedMenuIdx < 0)
						{
							focusedMenuIdx = _menu.childCount - 1;
						}
					}

					_menu[focusedMenuIdx].Focus();
				}

				break;
			}
			case KeyCode.Return:
			case KeyCode.KeypadEnter:
			{
				int focusedMenuIdx = GetFocusedMenuIdx(_menu);
				if (focusedMenuIdx != -1)
				{
					// pressing enter will do the same thing that the menu entry's click callback does
					var gotSelectedIndicator = _menu[focusedMenuIdx].Q<VisualElement>("SelectedIndicator");
					if (gotSelectedIndicator.userData is IContextMenuListener gotListener)
					{
						var gotIcon = _menu[focusedMenuIdx].Q<VisualElement>(IconName);
						var gotLabel = _menu[focusedMenuIdx].Q<Label>();
						ContextMenuParams parameters = new()
						{
							ContextMenuPosition = this.LocalToWorld(_menu.GetPositionXY()),
							MenuItemIndex = focusedMenuIdx,
							MenuItemLabel = gotLabel,
							MenuItemTooltip = _menu[focusedMenuIdx].userData,
							UserArg1 = gotIcon.userData,
							UserArg2 = gotLabel.userData
						};
						gotListener.OnContextMenuChosen(parameters);
						Hide();
						e.StopPropagation();
					}
				}

				break;
			}
		}

		return;

		// Get index of which child of m is focused
		int GetFocusedMenuIdx(VisualElement m)
		{
			for (int i = 0; i < m.childCount; i++)
			{
				if (m.panel.focusController.focusedElement == m[i])
				{
					return i;
				}
			}

			return -1;
		}
	}

	// ==================================================================================

	void UpdateIconVisibility()
	{
		bool iconsShown = false;
		bool selectedIndicatorShown = false;

		// -----------------------------------------------------------------
		// First Pass
		// Check whether any menu entry has icon and selected indicator showing

		for (int n = 0; n < _menu.childCount; ++n)
		{
			var gotSelectedIndicator = _menu[n].Q<VisualElement>(SelectedIndicatorName);
			if (gotSelectedIndicator == null)
			{
				// we're at a separator, not a menu entry (separators don't have a selected indicator)
				continue;
			}

			if (gotSelectedIndicator.resolvedStyle.display == DisplayStyle.Flex)
			{
				selectedIndicatorShown = true;
			}

			var gotIcon = _menu[n].Q<VisualElement>(IconName);
			if (gotIcon.GetClasses().Count() > 1)
			{
				iconsShown = true;
			}
		}

		// -----------------------------------------------------------------
		// Second Pass
		// Enable icons and selected indicator spaces if at least one menu entry is using them

		for (int n = 0; n < _menu.childCount; ++n)
		{
			var gotSelectedIndicator = _menu[n].Q<VisualElement>(SelectedIndicatorName);
			if (gotSelectedIndicator == null)
			{
				// we're at a separator, not a menu entry (separators don't have a selected indicator)
				continue;
			}

			gotSelectedIndicator.style.display = selectedIndicatorShown || _alwaysLeaveSpaceForSelectedIndicator ? DisplayStyle.Flex : DisplayStyle.None;

			var gotIcon = _menu[n].Q<VisualElement>(IconName);
			gotIcon.style.display = iconsShown ? DisplayStyle.Flex : DisplayStyle.None;
		}
	}

	void DelayedFocus()
	{
		Focus();
	}
}

}
