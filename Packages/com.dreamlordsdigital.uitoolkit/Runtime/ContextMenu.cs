using DLD.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public interface IContextMenu
	{
		VisualElement Root { get; }

		void ClearMenu();

		VisualElement AddMenu(string label, System.Action callback);
		void AddMenu(string label, string iconClassStyle, System.Action callback);

		void AddSeparator();

		void Show(Vector2 position);
		void Show(ContextClickEvent e);
		void Show(PointerDownEvent e);
	}

	public class ContextMenu : VisualElement, IContextMenu
	{
		const string TEMPLATE_RESOURCES_PATH = "DLD UIToolkit/ContextMenu";
		const string ENTRY_TEMPLATE_RESOURCES_PATH = "DLD UIToolkit/ContextMenuEntry";
		const string SEPARATOR_TEMPLATE_RESOURCES_PATH = "DLD UIToolkit/ContextMenuEntrySeparator";
		const string MENU_NAME = "ContextMenuBg";

		const string ENTRY_ALT_CLASS_NAME = "dld-context-menu-entry-container--alt-bg";
		const string PRESSED_ENTRY_CLASS_NAME = "dld-context-menu-entry-container--active";

		const float DEFAULT_MOUSE_MOVE_DISTANCE_FOR_INSTANT_CLOSE = 10;

		readonly VisualElement _menu;

		readonly System.Action _delayedFocus;

		readonly VisualTreeAsset _entryAsset;
		readonly VisualTreeAsset _separatorAsset;

		bool _doAltBgStyling;
		bool _mouseMovedDuringMouseDown;

		Focusable _focusTargetAfterClose;

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

			RegisterCallback<KeyDownEvent, ContextMenu>((e, c) => c.OnPressKey(e), this);
		}

		public void SetFocusTargetAfterClose(Focusable newFocusTarget)
		{
			_focusTargetAfterClose = newFocusTarget;
		}

		public void ClearMenu()
		{
			_menu.Clear();
		}

		public void DoAltBgStyling(bool doAltBgStyling)
		{
			_doAltBgStyling = doAltBgStyling;
		}

		public void AddSeparator()
		{
			var createdSeparator = _separatorAsset.Instantiate();

			var entryContainer = createdSeparator.Q<VisualElement>("Entry");
			_menu.Add(entryContainer);
		}

		public VisualElement AddMenu(string label, System.Action callback)
		{
			var createdEntry = _entryAsset.Instantiate();

			var entryContainer = createdEntry.Q<VisualElement>("Entry");
			entryContainer.userData = callback;

			var entryLabel = entryContainer.Q<Label>();
			entryLabel.text = label;

			entryContainer.RegisterCallback<PointerDownEvent>(e =>
			{
				if (e.currentTarget is VisualElement v)
				{
					v.AddToClassList(PRESSED_ENTRY_CLASS_NAME);
				}
			});

			entryContainer.RegisterCallback<PointerUpEvent, ContextMenu>((e, contextMenu) =>
			{
				if (e.currentTarget is VisualElement { userData: System.Action userCallback } v)
				{
					v.Focus();
					userCallback();
				}
				contextMenu.Hide();
			}, this);

			_menu.Add(entryContainer);

			if (_doAltBgStyling && _menu.childCount % 2 == 0)
			{
				entryContainer.AddToClassList(ENTRY_ALT_CLASS_NAME);
			}

			return entryContainer;
		}

		public void AddMenu(string label, string iconClassStyle, System.Action callback)
		{
			var entryContainer = AddMenu(label, callback);

			var entryIcon = entryContainer.Q<VisualElement>("Icon");
			entryIcon.AddToClassList(iconClassStyle);
		}

		public void Show(Vector2 position)
		{
			_menu.SetPosition(position);
			style.display = DisplayStyle.Flex;
			focusable = true;
			_mouseMovedDuringMouseDown = false;

			schedule.Execute(_delayedFocus).ExecuteLater(0);
		}

		public void Show(ContextClickEvent e)
		{
			var contextMenuMousePos = ((VisualElement)e.currentTarget).ChangeCoordinatesTo(this, e.localMousePosition);
			Show(contextMenuMousePos);
		}

		public void Show(PointerDownEvent e)
		{
			var mousePos = ((VisualElement)e.currentTarget).ChangeCoordinatesTo(this, e.localPosition);
			Show(mousePos);
		}

		public void Hide()
		{
			style.display = DisplayStyle.None;
			Blur();
			if (_focusTargetAfterClose != null)
			{
				_focusTargetAfterClose.Focus();
			}
		}

		void DelayedFocus()
		{
			Focus();
		}

		void OnMouseMove(MouseMoveEvent e)
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
						_menu[focusedMenuIdx+1].Focus();
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
						_menu[focusedMenuIdx-1].Focus();
					}

					break;
				}
				case KeyCode.Return:
				case KeyCode.KeypadEnter:
				{
					var menu = this.Q<VisualElement>(MENU_NAME);

					int focusedMenuIdx = GetFocusedMenuIdx(menu);
					if (focusedMenuIdx != -1)
					{
						// pressing enter will do the same thing that the menu entry's click callback does
						if (menu[focusedMenuIdx].userData is System.Action userCallback)
						{
							userCallback();
							Hide();
							e.StopPropagation();
						}
					}
					break;
				}
			}

			return;

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
	}
}
