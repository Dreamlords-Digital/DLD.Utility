using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public class TabbedContent
	{
		const string TAB_BODY_STYLE_CLASS = "dld-tab__body";

		public TabbedContent()
		{
			// todo: probably need some sort of custom UI for grid (GraphView is in UnityEditor)
			Body = new VisualElement();

			Body.style.display = DisplayStyle.None; // hide at first
			Body.AddToClassList(TAB_BODY_STYLE_CLASS);
		}

		public void Init(RadioButton newTabButton)
		{
			Tab = newTabButton;
			Body.name = $"TabBody:{newTabButton.label}";
			_modifiedIndicator = newTabButton.Q<VisualElement>("ModifiedIndicator");
			_modifiedIndicator.style.display = DisplayStyle.None;
		}

		public string TabLabel
		{
			get => Tab?.label;
			set
			{
				if (Tab != null)
				{
					Tab.label = value;
				}
			}
		}

		public bool IsTabSelected
		{
			get => Tab?.value ?? false;
			set
			{
				if (Tab != null)
				{
					Tab.value = value;
				}
			}
		}

		public bool ShowModifiedIndicator
		{
			get => _modifiedIndicator.style.display == DisplayStyle.Flex;
			set => _modifiedIndicator.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
		}

		/// <summary>
		/// Where the NodeViews for this Behaviour Tree are parented to.
		/// </summary>
		public readonly VisualElement Body;

		public RadioButton Tab { get; private set; }

		VisualElement _modifiedIndicator;
	}

	[UxmlElement]
	public partial class Pane<T> : VisualElement where T : TabbedContent, new()
	{
		const string TEMPLATE_RESOURCES_PATH = "DLD UIToolkit/Pane";

		readonly VisualElement _tabContainer;
		readonly List<T> _tabList = new();
		T _currentTab;

		/// <summary>
		/// Where tab bodies will be shown.
		/// </summary>
		readonly VisualElement _tabBodyContainer;

		ContextMenu _contextMenu;

		readonly EventCallback<ChangeEvent<bool>> _onPressTab;
		readonly EventCallback<ContextClickEvent> _onPressTabContext;

		System.Action<T> _onTabShown;
		System.Action<ContextMenu, T> _onTabContext;

		// =====================================================================

		public Pane()
		{
			var asset = Resources.Load<VisualTreeAsset>(TEMPLATE_RESOURCES_PATH);
			asset.CloneTree(this);

			var clonedRoot = this.Q<VisualElement>("Pane");
			foreach (string rootStyleClass in clonedRoot.GetClasses())
			{
				AddToClassList(rootStyleClass);
			}

			for (int n = clonedRoot.childCount - 1; n >= 0; --n)
			{
				Insert(0, clonedRoot[n]);
			}

			clonedRoot.RemoveFromHierarchy();

			// -----------------------------------

			const string TABS_LIST_ELEMENT_NAME = "Tabs";
			_tabContainer = this.Q<VisualElement>(TABS_LIST_ELEMENT_NAME);

			const string TAB_BODY_CONTAINER_ELEMENT_NAME = "TabBodyContainer";
			_tabBodyContainer = this.Q<VisualElement>(TAB_BODY_CONTAINER_ELEMENT_NAME);

			// -----------------------------------

			_onPressTab = OnPressTab;
			_onPressTabContext = OnPressTabContext;
		}

		public VisualElement TabBodyContainer => _tabBodyContainer;

		public T CurrentTab => _currentTab;

		public int Count => _tabList.Count;

		public new T this[int idx] => _tabList[idx];

		// =====================================================================

		public void SetContextMenu(ContextMenu contextMenu)
		{
			_contextMenu = contextMenu;
		}

		public void SetOnTabShown(System.Action<T> onTabShown)
		{
			_onTabShown = onTabShown;
		}

		public void SetOnTabContext(Action<ContextMenu, T> onTabContext)
		{
			_onTabContext = onTabContext;
		}

		public T CreateTab(string tabName)
		{
			const string TAB_TEMPLATE_RESOURCES_PATH = "DLD UIToolkit/Tab";
			var tabTemplate = Resources.Load<VisualTreeAsset>(TAB_TEMPLATE_RESOURCES_PATH);
			var newTabFromTemplate = tabTemplate.Instantiate();
			const string TAB_ELEMENT_NAME = "Tab";
			var newTabButton = newTabFromTemplate.Q<RadioButton>(TAB_ELEMENT_NAME);
			newTabButton.label = tabName;
			newTabButton.labelElement.pickingMode = PickingMode.Ignore;
			_tabContainer.Add(newTabButton);

			var newTabbedContent = new T();
			newTabbedContent.Init(newTabButton);

			newTabButton.userData = newTabbedContent;
			_tabList.Add(newTabbedContent);

			_tabBodyContainer.Add(newTabbedContent.Body);

			newTabButton.RegisterCallback(_onPressTab);
			newTabButton.RegisterCallback(_onPressTabContext);

			return newTabbedContent;
		}

		void CloseTab(T tabToClose)
		{
			if (tabToClose.ShowModifiedIndicator)
			{
				// todo: Ask user if they want to save, discard, or cancel
			}

			// If the tab to close is currently selected,
			// then switch to a different tab if possible.
			if (tabToClose.IsTabSelected)
			{
				int tabToCloseIdx = _tabList.IndexOf(tabToClose);
				if (tabToCloseIdx >= 0)
				{
					// Favor switching to the tab that is to the left of the tab to close.
					// If not possible (tab to close is leftmost) then switch to the tab to the right.
					if (tabToCloseIdx > 0)
					{
						_tabList[tabToCloseIdx - 1].IsTabSelected = true;
					}
					else if (_tabList.Count > 1)
					{
						_tabList[1].IsTabSelected = true;
					}
				}
			}

			_tabContainer.Remove(tabToClose.Tab);
			_tabBodyContainer.Remove(tabToClose.Body);
			_tabList.Remove(tabToClose);

			if (_tabList.Count == 0)
			{
				// todo: show splash screen?
				Debug.Log("Tabs now empty");
			}
		}

		void OnPressTab(ChangeEvent<bool> e)
		{
			if (e.target is not RadioButton { userData: T clickedTabContent })
			{
				return;
			}

			clickedTabContent.Body.style.display = e.newValue ? DisplayStyle.Flex : DisplayStyle.None;

			if (!e.previousValue && e.newValue)
			{
				_currentTab = clickedTabContent;
				_onTabShown?.Invoke(clickedTabContent);
			}
		}

		void OnPressTabContext(ContextClickEvent e)
		{
			if (e.target is not RadioButton { userData: T clickedTabContent })
			{
				return;
			}

			_contextMenu.ClearMenu();
			_contextMenu.AddMenu("Close", () => CloseTab(clickedTabContent));
			_contextMenu.AddMenu("Close Other Tabs", () => throw new NotImplementedException());
			_contextMenu.AddMenu("Close All Tabs", () => throw new NotImplementedException());
			_onTabContext?.Invoke(_contextMenu, clickedTabContent);
			_contextMenu.Show(e);
		}
	}
}
