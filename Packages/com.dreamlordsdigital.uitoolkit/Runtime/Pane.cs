// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

public class TabbedContent
{
	const string TabBodyStyleClass = "dld-tab__body";

	public TabbedContent()
	{
		// todo: probably need some sort of custom UI for grid (GraphView is in UnityEditor)
		Body = new VisualElement();

		Body.style.display = DisplayStyle.None; // hide at first
		Body.AddToClassList(TabBodyStyleClass);
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
	///    Where the NodeViews for this Behaviour Tree are parented to.
	/// </summary>
	public readonly VisualElement Body;

	public RadioButton Tab { get; private set; }

	VisualElement _modifiedIndicator;

	public virtual void OnClose()
	{
	}
}

[UxmlElement]
public partial class Pane<T> : VisualElement, IContextMenuListener where T : TabbedContent, new()
{
	const string TemplateResourcesPath = "DLD UIToolkit/Pane";

	const string ContextMenuCloseTab = "CloseTab";
	const string ContextMenuCloseOtherTabs = "CloseOtherTabs";
	const string ContextMenuCloseAllTabs = "CloseAllTabs";

	readonly VisualElement _tabContainer;
	readonly List<T> _tabList = new();
	T _currentTab;

	/// <summary>
	///    Where tab bodies will be shown.
	/// </summary>
	readonly VisualElement _tabBodyContainer;

	IContextMenu _contextMenu;

	readonly EventCallback<ChangeEvent<bool>> _onPressTab;
	readonly EventCallback<PointerDownEvent> _onPressTabContext;

	Action<T> _onTabShown;
	Action<IContextMenu, T> _onTabContext;

	// =====================================================================

	public Pane()
	{
		var asset = Resources.Load<VisualTreeAsset>(TemplateResourcesPath);
		asset.CloneTree(this);
		this.RemoveTemplateContainer("Pane");

		// -----------------------------------

		const string TabsListElementName = "Tabs";
		_tabContainer = this.Q<VisualElement>(TabsListElementName);

		const string TabBodyContainerElementName = "TabBodyContainer";
		_tabBodyContainer = this.Q<VisualElement>(TabBodyContainerElementName);

		// -----------------------------------

		_onPressTab = OnPressTab;
		_onPressTabContext = OnPressTabContext;
	}

	public VisualElement TabBodyContainer => _tabBodyContainer;

	public T CurrentTab => _currentTab;

	public int Count => _tabList.Count;

	public new T this[int idx] => _tabList[idx];

	// =====================================================================

	public void SetContextMenu(IContextMenu contextMenu)
	{
		_contextMenu = contextMenu;
	}

	public void SetOnTabShown(Action<T> onTabShown)
	{
		_onTabShown = onTabShown;
	}

	public void SetOnTabContext(Action<IContextMenu, T> onTabContext)
	{
		_onTabContext = onTabContext;
	}

	public T CreateTab(string tabName)
	{
		const string TabTemplateResourcesPath = "DLD UIToolkit/Tab";
		var tabTemplate = Resources.Load<VisualTreeAsset>(TabTemplateResourcesPath);
		var newTabFromTemplate = tabTemplate.Instantiate();
		const string TabElementName = "Tab";
		var newTabButton = newTabFromTemplate.Q<RadioButton>(TabElementName);
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

	public T GetTabFromBody(VisualElement body)
	{
		for (int n = 0; n < _tabList.Count; ++n)
		{
			if (_tabList[n].Body == body)
			{
				return _tabList[n];
			}
		}

		return null;
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

		tabToClose.OnClose();

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

	void OnPressTabContext(PointerDownEvent e)
	{
		if (e.target is not RadioButton { userData: T clickedTabContent })
		{
			return;
		}

		_contextMenu.DoAltBgStyling(false);
		_contextMenu.SetAlwaysLeaveSpaceForSelectedIndicator(false);
		_contextMenu.ClearMenu();
		_contextMenu.AddMenu("Close", iconClassStyle: BaseIcons.Close,
			listener: this, userArg1: ContextMenuCloseTab, userArg2: clickedTabContent);
		_contextMenu.AddMenu("Close Other Tabs", iconClassStyle: BaseIcons.Close, menuItemStyle: _tabList.Count == 1 ? ContextMenuItemStyle.Disabled : ContextMenuItemStyle.Standard,
			listener: this, userArg1: ContextMenuCloseOtherTabs, userArg2: clickedTabContent);
		_contextMenu.AddMenu("Close All Tabs", iconClassStyle: BaseIcons.Close,
			listener: this, userArg1: ContextMenuCloseAllTabs);
		_onTabContext?.Invoke(_contextMenu, clickedTabContent);
		_contextMenu.Show(e);
	}

	public void OnContextMenuChosen(int index, Label label, object itemTooltip, object userArg1, object userArg2)
	{
		switch (userArg1 as string)
		{
			case ContextMenuCloseTab:
				CloseTab(userArg2 as T);
				break;
			case ContextMenuCloseOtherTabs:
				var thisTab = userArg2 as T;
				Debug.LogError($"Close Other Tabs for {thisTab?.TabLabel}, not yet implemented");
				break;
			case ContextMenuCloseAllTabs:
				Debug.LogError("Close All Tabs not yet implemented");
				break;
		}
	}

	public void OnContextMenuClosed(bool userCancelled)
	{
	}
}

}
