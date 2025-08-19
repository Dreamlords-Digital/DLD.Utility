using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public class ContextMenu : VisualElement
	{
		const string TEMPLATE_RESOURCES_PATH = "DLD UIToolkit/ContextMenu";
		const string MENU_NAME = "ContextMenuBg";

		readonly VisualElement _root;
		readonly VisualElement _menu;

		public VisualElement Root => _root;

		public ContextMenu()
		{
			var asset = Resources.Load<VisualTreeAsset>(TEMPLATE_RESOURCES_PATH);
			asset.CloneTree(this);

			_root = this.Q<VisualElement>("ContextMenu");
			_root.style.display = DisplayStyle.None;

			_root.RegisterCallback<MouseDownEvent, VisualElement>(OnPressOutside, _root);

			_menu = this.Q<VisualElement>(MENU_NAME);
			_menu.RegisterCallback<MouseDownEvent>(OnPressInside);

			_root.RegisterCallback<KeyDownEvent, VisualElement>(OnPressKey, _root);
			_root.focusable = true;
		}

		~ContextMenu()
		{
			_root.UnregisterCallback<MouseDownEvent, VisualElement>(OnPressOutside);
			_menu.UnregisterCallback<MouseDownEvent>(OnPressInside);
			_root.UnregisterCallback<KeyDownEvent, VisualElement>(OnPressKey);
		}

		public void ClearMenu()
		{
			_menu.Clear();
		}

		public void AddMenu(string label, System.Action callback)
		{
			var newButton = new Button
			{
				userData = callback,
				text = label
			};

			newButton.RegisterCallback<ClickEvent, VisualElement>((click, root) =>
			{
				if (click.currentTarget is VisualElement { userData: System.Action userCallback })
				{
					userCallback();
				}
				root.style.display = DisplayStyle.None;
			}, _root);

			_menu.Add(newButton);
		}

		public void Show(Vector2 position)
		{
			_menu.style.left = position.x;
			_menu.style.top = position.y;
			_root.style.display = DisplayStyle.Flex;
			_root.Focus();
		}

		public void Show(ContextClickEvent e)
		{
			var contextMenuMousePos = ((VisualElement)e.currentTarget).ChangeCoordinatesTo(_root, e.localMousePosition);
			Show(contextMenuMousePos);
		}

		public void Hide()
		{
			_root.style.display = DisplayStyle.None;
		}

		static void OnPressOutside(MouseDownEvent e, VisualElement root)
		{
			root.style.display = DisplayStyle.None;
			e.StopPropagation();
			root.Blur();
		}

		static void OnPressInside(MouseDownEvent e)
		{
			e.StopPropagation();
		}

		static void OnPressKey(KeyDownEvent e, VisualElement root)
		{
			if (root.style.display == DisplayStyle.None)
			{
				return;
			}

			switch (e.keyCode)
			{
				case KeyCode.Escape:
				{
					// Pressing Escape is considered a cancel.
					// Do the same thing as OnPressOutside.
					root.style.display = DisplayStyle.None;
					e.StopPropagation();
					root.Blur();
					break;
				}
				case KeyCode.DownArrow:
				{
					var menu = root.Q<VisualElement>(MENU_NAME);

					int focusedMenuIdx = GetFocusedMenuIdx(menu);
					if (focusedMenuIdx == -1 || focusedMenuIdx == menu.childCount - 1)
					{
						// wrap around and go to first menu entry
						menu[0].Focus();
					}
					else
					{
						// select menu entry below
						menu[focusedMenuIdx+1].Focus();
					}

					break;
				}
				case KeyCode.UpArrow:
				{
					var menu = root.Q<VisualElement>(MENU_NAME);

					int focusedMenuIdx = GetFocusedMenuIdx(menu);
					if (focusedMenuIdx <= 0)
					{
						// wrap around and go to final menu entry
						menu[menu.childCount - 1].Focus();
					}
					else
					{
						// select menu entry above
						menu[focusedMenuIdx-1].Focus();
					}

					break;
				}
				case KeyCode.Return:
				case KeyCode.KeypadEnter:
				{
					var menu = root.Q<VisualElement>(MENU_NAME);

					int focusedMenuIdx = GetFocusedMenuIdx(menu);
					if (focusedMenuIdx != -1)
					{
						// pressing enter will do the same thing that the menu entry's click callback does
						if (menu[focusedMenuIdx].userData is System.Action userCallback)
						{
							userCallback();
							root.style.display = DisplayStyle.None;
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
