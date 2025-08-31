using System.Collections.Generic;
using System.Linq;
using DLD.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public interface IContextMenu
	{
		VisualElement Root { get; }

		void ClearMenu();
		void DoAltBgStyling(bool doAltBgStyling);
		void SetAlwaysLeaveSpaceForSelectedIndicator(bool alwaysLeaveSpaceForSelectedIndicator);
		void AddSeparator();

		VisualElement AddMenu(string label, string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
			IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null);

		void AddMenu(string label, string tooltip, string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
			IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null);

		void AddMenu(string label, TooltipMessage[] menuTooltip, string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
			IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null);

		void AddMenu(string label, List<TooltipMessage> menuTooltip, string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
			IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null);

		/// <summary>
		/// Show the selected indicator only on the specified item.
		/// All other items will have their selected indicators cleared.
		/// </summary>
		void ChangeSelected(int newSelectedIdx);

		/// <summary>
		/// Find the menu item that has the specified userArg1 and
		/// show the selected indicator on it only.
		/// All other items will have their selected indicators cleared.
		/// </summary>
		void ChangeSelected(object userArg1);

		/// <summary>
		/// Show/hide the selected indicator of the specified item.
		/// Other items are not edited.
		/// </summary>
		/// <param name="menuIdx">The item to edit, by index.</param>
		/// <param name="selected">Whether to show the selected indicator or not.</param>
		void SetSelected(int menuIdx, bool selected);

		void Show(Vector2 position, IContextMenuListener listener = null);
		void Show(ContextClickEvent e, IContextMenuListener listener = null);
		void Show(PointerDownEvent e, IContextMenuListener listener = null);
		void Show(VisualElement ve, IContextMenuListener listener = null);

		bool WasLastShownOn(VisualElement ve);
	}

	public interface IContextMenuListener
	{
		void OnContextMenuChosen(int index, Label label, object itemTooltip, object userArg1, object userArg2);
		void OnContextMenuCanceled();
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
		const string TEMPLATE_RESOURCES_PATH = "DLD UIToolkit/ContextMenu";
		const string ENTRY_TEMPLATE_RESOURCES_PATH = "DLD UIToolkit/ContextMenuEntry";
		const string SEPARATOR_TEMPLATE_RESOURCES_PATH = "DLD UIToolkit/ContextMenuEntrySeparator";
		const string MENU_NAME = "ContextMenuBg";
		const string SELECTED_INDICATOR_NAME = "SelectedIndicator";
		const string ICON_NAME = "Icon";

		const string ENTRY_ALT_STYLE_CLASS = "dld-context-menu-entry-container--alt-bg";
		const string PRESSED_ENTRY_STYLE_CLASS = "dld-context-menu-entry-container--active";
		const string DISABLED_ENTRY_STYLE_CLASS = "dld-context-menu-entry-container--disabled";
		const string MENU_AS_DROPDOWN_STYLE_CLASS = "dld-context-menu--as-dropdown";
		const string MENU_AS_DROPDOWN_LONGER_THAN_BUTTON_STYLE_CLASS = "dld-context-menu--as-dropdown--longer";

		const string SELECTED_ENTRY_LABEL_STYLE_CLASS = "dld-context-menu-entry__label--selected";
		public const string ERROR_ENTRY_LABEL_STYLE_CLASS = "dld-context-menu-entry__label--error";
		public const string WARNING_ENTRY_LABEL_STYLE_CLASS = "dld-context-menu-entry__label--warning";

		const float DEFAULT_MOUSE_MOVE_DISTANCE_FOR_INSTANT_CLOSE = 10;

		static readonly CustomStyleProperty<float> DropdownButtonFitWidthAdjust = new ("--dropdown--button-fit-width-adjust");

		// ==================================================================================

		float _dropdownButtonFitWidthAdjust;

		readonly VisualElement _menu;

		readonly System.Action _delayedFocus;

		readonly VisualTreeAsset _entryAsset;
		readonly VisualTreeAsset _separatorAsset;

		VisualElement _elementShownOn;

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

			_entryAsset = Resources.Load<VisualTreeAsset>(ENTRY_TEMPLATE_RESOURCES_PATH);
			_separatorAsset = Resources.Load<VisualTreeAsset>(SEPARATOR_TEMPLATE_RESOURCES_PATH);

			// -----------------------------------

			var asset = Resources.Load<VisualTreeAsset>(TEMPLATE_RESOURCES_PATH);
			asset.CloneTree(this);
			this.RemoveTemplateContainer("ContextMenu");

			// -----------------------------------

			style.display = DisplayStyle.None;

			RegisterCallback<MouseDownEvent, ContextMenu>((e, c) => c.OnPressOutside(e), this);
			RegisterCallback<MouseMoveEvent, ContextMenu>((e, c) => c.OnMouseMove(e), this);
			RegisterCallback<MouseUpEvent, ContextMenu>((e, c) => c.OnMouseUpOutside(e), this);

			_menu = this.Q<VisualElement>(MENU_NAME);
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

		public void AddSeparator()
		{
			var createdSeparator = _separatorAsset.Instantiate();

			var entryContainer = createdSeparator.Q<VisualElement>("Entry");
			_menu.Add(entryContainer);
		}

		public void AddMenu(string label, string menuTooltip = null,
			string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
			IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null)
		{
			var entryContainer = AddMenu(label, iconClassStyle, menuItemStyle, listener, userArg1, userArg2);
			entryContainer.Register(_tooltip, menuTooltip);
		}

		public void AddMenu(string label, TooltipMessage[] menuTooltip = null,
			string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
			IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null)
		{
			var entryContainer = AddMenu(label, iconClassStyle, menuItemStyle, listener, userArg1, userArg2);

			entryContainer.userData = menuTooltip;
			entryContainer.Register(_tooltip);
		}

		public void AddMenu(string label, List<TooltipMessage> menuTooltip = null,
			string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
			IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null)
		{
			var entryContainer = AddMenu(label, iconClassStyle, menuItemStyle, listener, userArg1, userArg2);

			entryContainer.userData = menuTooltip;
			entryContainer.Register(_tooltip);
		}

		// ==================================================================================

		public void ChangeMenuIcon(object userArg1, string iconClassStyleToAdd = null, string iconClassStyleToRemove = null)
		{
			for (int n = 0; n < _menu.childCount; ++n)
			{
				var gotIcon = _menu[n].Q<VisualElement>(ICON_NAME);
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
				var selectedIndicator = _menu[n].Q<VisualElement>(SELECTED_INDICATOR_NAME);
				if (selectedIndicator == null)
				{
					// we're at a separator, not a menu entry (separators don't have a selected indicator)
					continue;
				}

				var entryLabel = _menu[n].Q<Label>();
				if (n == menuIdx)
				{
					selectedIndicator.AddToClassList(BaseIcons.SELECTED_IN_DROPDOWN);
					entryLabel.AddToClassList(SELECTED_ENTRY_LABEL_STYLE_CLASS);
				}
				else
				{
					selectedIndicator.RemoveFromClassList(BaseIcons.SELECTED_IN_DROPDOWN);
					entryLabel.RemoveFromClassList(SELECTED_ENTRY_LABEL_STYLE_CLASS);
				}
			}
		}

		public void ChangeSelected(object userArg1)
		{
			for (int n = 0; n < _menu.childCount; ++n)
			{
				var selectedIndicator = _menu[n].Q<VisualElement>(SELECTED_INDICATOR_NAME);
				if (selectedIndicator == null)
				{
					// we're at a separator, not a menu entry (separators don't have a selected indicator)
					continue;
				}

				var entryLabel = _menu[n].Q<Label>();
				var gotIcon = _menu[n].Q<VisualElement>(ICON_NAME);
				if (gotIcon?.userData != null && gotIcon.userData.Equals(userArg1))
				{
					selectedIndicator.AddToClassList(BaseIcons.SELECTED_IN_DROPDOWN);
					entryLabel.AddToClassList(SELECTED_ENTRY_LABEL_STYLE_CLASS);
				}
				else
				{
					selectedIndicator.RemoveFromClassList(BaseIcons.SELECTED_IN_DROPDOWN);
					entryLabel.RemoveFromClassList(SELECTED_ENTRY_LABEL_STYLE_CLASS);
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

			var selectedIndicator = _menu[menuIdx].Q<VisualElement>(SELECTED_INDICATOR_NAME);

			if (selected)
			{
				selectedIndicator?.AddToClassList(BaseIcons.SELECTED_IN_DROPDOWN);
				entryLabel.AddToClassList(SELECTED_ENTRY_LABEL_STYLE_CLASS);
			}
			else
			{
				selectedIndicator?.RemoveFromClassList(BaseIcons.SELECTED_IN_DROPDOWN);
				entryLabel.RemoveFromClassList(SELECTED_ENTRY_LABEL_STYLE_CLASS);
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

		public void Show(VisualElement ve, IContextMenuListener listener = null)
		{
			UpdateIconVisibility();
			_listener = listener;
			_elementShownOn = ve;

			var veLayout = ve.layout;
			var veWorldPos = ve.LocalToWorld(new Vector2(0, veLayout.height));
			var localPos = this.WorldToLocal(veWorldPos);
			_menu.SetPosition(localPos);
			_menu.style.width = StyleKeyword.Null;
			_menu.AddToClassList(MENU_AS_DROPDOWN_STYLE_CLASS);
			_menu.RemoveFromClassList(MENU_AS_DROPDOWN_LONGER_THAN_BUTTON_STYLE_CLASS);

			style.display = DisplayStyle.Flex;
			focusable = true;
			_mouseMovedDuringMouseDown = false;

			schedule.Execute(_delayedFocus).ExecuteLater(0);
		}

		public bool WasLastShownOn(VisualElement ve) => ve == _elementShownOn;

		public void Hide()
		{
			style.display = DisplayStyle.None;
			_menu.style.width = StyleKeyword.Null;
			Blur();
			_menu.RemoveFromClassList(MENU_AS_DROPDOWN_STYLE_CLASS);
			if (_focusTargetAfterClose != null)
			{
				_focusTargetAfterClose.Focus();
			}

			if (_listener != null)
			{
				_listener.OnContextMenuCanceled();
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
			float elementWidth = _elementShownOn.layout.width;

			if (menuWidth <= 0)
			{
				return;
			}

			if (menuWidth <= elementWidth)
			{
				_menu.style.width = elementWidth + _dropdownButtonFitWidthAdjust;
			}
			else
			{
				_menu.AddToClassList(MENU_AS_DROPDOWN_LONGER_THAN_BUTTON_STYLE_CLASS);
			}
		}

		void OnMouseMove(MouseMoveEvent _)
		{
			_mouseMovedDuringMouseDown = true;
		}

		void OnMouseUpOutside(MouseUpEvent e)
		{
			var mouseDelta = e.localMousePosition - _menu.GetPosition2();
			bool mouseMovedFarEnough = mouseDelta.sqrMagnitude > (DEFAULT_MOUSE_MOVE_DISTANCE_FOR_INSTANT_CLOSE * DEFAULT_MOUSE_MOVE_DISTANCE_FOR_INSTANT_CLOSE);
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
							var gotIcon = _menu[focusedMenuIdx].Q<VisualElement>(ICON_NAME);
							var gotLabel = _menu[focusedMenuIdx].Q<Label>();
							gotListener.OnContextMenuChosen(focusedMenuIdx, gotLabel, _menu[focusedMenuIdx].userData, gotIcon.userData, gotLabel.userData);
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

		public VisualElement AddMenu(string label, string iconClassStyle = null, ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard,
			IContextMenuListener listener = null, object userArg1 = null, object userArg2 = null)
		{
			var createdEntry = _entryAsset.Instantiate();

			var entryContainer = createdEntry.Q<VisualElement>("Entry");

			bool showAsDisabled = (menuItemStyle & ContextMenuItemStyle.Disabled) != 0;
			if (showAsDisabled)
			{
				entryContainer.AddToClassList(DISABLED_ENTRY_STYLE_CLASS);
			}

			var selectedIndicator = entryContainer.Q<VisualElement>(SELECTED_INDICATOR_NAME);
			selectedIndicator.userData = listener;

			var entryIcon = entryContainer.Q<VisualElement>(ICON_NAME);
			entryIcon.userData = userArg1;

			var entryLabel = entryContainer.Q<Label>();
			entryLabel.text = label;
			entryLabel.userData = userArg2;

			if ((menuItemStyle & ContextMenuItemStyle.Error) != 0)
			{
				entryLabel.AddToClassList(ERROR_ENTRY_LABEL_STYLE_CLASS);
			}
			else if ((menuItemStyle & ContextMenuItemStyle.Warning) != 0)
			{
				entryLabel.AddToClassList(WARNING_ENTRY_LABEL_STYLE_CLASS);
			}

			if ((menuItemStyle & ContextMenuItemStyle.Selected) != 0)
			{
				selectedIndicator.style.display = DisplayStyle.Flex;
				selectedIndicator.AddToClassList(BaseIcons.SELECTED_IN_DROPDOWN);
				entryLabel.AddToClassList(SELECTED_ENTRY_LABEL_STYLE_CLASS);
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
						v.AddToClassList(PRESSED_ENTRY_STYLE_CLASS);
					}
				});

				entryContainer.RegisterCallback<PointerUpEvent>(e =>
				{
					if (e.currentTarget is not VisualElement targetElement)
					{
						return;
					}
					var gotSelectedIndicator = targetElement.Q<VisualElement>(SELECTED_INDICATOR_NAME);

					if (gotSelectedIndicator.userData is IContextMenuListener gotListener)
					{
						targetElement.Focus();
						var gotIcon = targetElement.Q<VisualElement>(ICON_NAME);
						var gotLabel = targetElement.Q<Label>();
						gotListener.OnContextMenuChosen(targetElement.parent.IndexOf(targetElement), gotLabel, targetElement.userData, gotIcon.userData, gotLabel.userData);
					}
					else
					{
						e.StopPropagation();
					}
				});
			}

			_menu.Add(entryContainer);

			if (_doAltBgStyling && _menu.childCount % 2 == 0)
			{
				entryContainer.AddToClassList(ENTRY_ALT_STYLE_CLASS);
			}

			if (!string.IsNullOrEmpty(iconClassStyle))
			{
				entryIcon.AddToClassList(iconClassStyle);
			}

			return entryContainer;
		}

		void UpdateIconVisibility()
		{
			bool iconsShown = false;
			bool selectedIndicatorShown = false;

			// -----------------------------------------------------------------
			// First Pass
			// Check whether any menu entry has icon and selected indicator showing

			for (int n = 0; n < _menu.childCount; ++n)
			{
				var gotSelectedIndicator = _menu[n].Q<VisualElement>(SELECTED_INDICATOR_NAME);
				if (gotSelectedIndicator == null)
				{
					// we're at a separator, not a menu entry (separators don't have a selected indicator)
					continue;
				}

				if (gotSelectedIndicator.resolvedStyle.display == DisplayStyle.Flex)
				{
					selectedIndicatorShown = true;
				}
				var gotIcon = _menu[n].Q<VisualElement>(ICON_NAME);
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
				var gotSelectedIndicator = _menu[n].Q<VisualElement>(SELECTED_INDICATOR_NAME);
				if (gotSelectedIndicator == null)
				{
					// we're at a separator, not a menu entry (separators don't have a selected indicator)
					continue;
				}

				gotSelectedIndicator.style.display = selectedIndicatorShown || _alwaysLeaveSpaceForSelectedIndicator ? DisplayStyle.Flex : DisplayStyle.None;

				var gotIcon = _menu[n].Q<VisualElement>(ICON_NAME);
				gotIcon.style.display = iconsShown ? DisplayStyle.Flex : DisplayStyle.None;
			}
		}

		void DelayedFocus()
		{
			Focus();
		}
	}
}
