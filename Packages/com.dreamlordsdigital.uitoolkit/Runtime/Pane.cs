// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

public enum SplitDirection : byte
{
	Down,
	Right
}

public class TabbedContent
{
	const string TabBodyStyleClass = "dld-tab__body";

	readonly ITooltip _tooltip;
	DialogBox _dialogBox;

	public TabbedContent(ITooltip tooltip)
	{
		_tooltip = tooltip;

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

	public bool IsDialogBoxShown => _dialogBox != null && _dialogBox.IsShown;

	VisualElement _modifiedIndicator;

	public virtual void OnClose()
	{
	}

	public void ShowAsFocused()
	{
		// todo: remove the unfocused style
	}

	public void ShowAsUnfocused()
	{
		// todo: add the unfocused style
	}

	public void ShowDialogBox(IDialogBoxListener listener, string title = null, string description = null,
		bool showNo = false, bool showCancel = false,
		string okTooltipText = null, string noTooltipText = null, string cancelTooltipText = null,
		string okTooltipIcon = BaseIcons.GenericInfo, string noTooltipIcon = BaseIcons.GenericError, string cancelTooltipIcon = BaseIcons.GenericInfo,
		string okArg = DialogBox.GenericOk, string noArg = DialogBox.GenericNo, string cancelArg = DialogBox.GenericCancel, string userArg1 = null)
	{
		if (_dialogBox == null)
		{
			_dialogBox = new DialogBox();
			_dialogBox.SetTooltip(_tooltip);
			Body.Add(_dialogBox);
		}

		_dialogBox.Show(listener, title, description, showNo, showCancel,
			okTooltipText, noTooltipText, cancelTooltipText,
			okTooltipIcon, noTooltipIcon, cancelTooltipIcon,
			okArg, noArg, cancelArg, userArg1);
	}
}

/// <summary>
///    Receives events when the user interacts with a tab.
/// </summary>
public interface ITabListener
{
	/// <summary>
	///    Called when user switches to a different tab.
	///    Also called when a new tab is created
	///    (since the newly created tab is switched to automatically).
	/// </summary>
	/// <param name="pane"></param>
	/// <param name="shownTab"></param>
	void OnTabShown(Pane pane, TabbedContent shownTab);

	/// <summary>
	///    Called when user right-clicks on a tab and the context menu is about to show up.
	///    Use this to add further entries into the context menu.
	/// </summary>
	/// <param name="contextMenu"></param>
	/// <param name="tab"></param>
	void OnTabContextMenu(IContextMenu contextMenu, TabbedContent tab);

	/// <summary>
	///    Called when user closes a tab. Return true to force the tab to stay open.
	///    Use this to show a modal dialog box asking the user to confirm the closing.
	/// </summary>
	/// <param name="tabToClose"></param>
	/// <returns></returns>
	bool NeedToAskConfirmationToClose(TabbedContent tabToClose);

	/// <summary>
	///    Called a tab is finally closed.
	/// </summary>
	/// <param name="closedTab"></param>
	void OnTabClosed(TabbedContent closedTab);
}

[UxmlElement]
public partial class Pane : VisualElement, IContextMenuListener
{
	const string TemplateResourcesPath = "DLD UIToolkit/Pane";

	const string ContextMenuCloseTab = "CloseTab";
	const string ContextMenuCloseOtherTabs = "CloseOtherTabs";
	const string ContextMenuCloseAllTabs = "CloseAllTabs";

	readonly VisualElement _tabContainer;
	readonly List<TabbedContent> _tabList = new();
	TabbedContent _currentTab;

	/// <summary>
	///    Where tab bodies will be shown. This is where all tab bodies are parented to.
	/// </summary>
	readonly VisualElement _tabBodyContainer;

	IContextMenu _contextMenu;
	ITabListener _tabListener;

	readonly EventCallback<ChangeEvent<bool>> _onPressTab;
	readonly EventCallback<PointerDownEvent> _onPressTabContext;

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

	/// <inheritdoc cref="_tabBodyContainer"/>
	public VisualElement TabBodyContainer => _tabBodyContainer;

	public TabbedContent CurrentTab => _currentTab;

	public int Count => _tabList.Count;

	public new TabbedContent this[int idx] => _tabList[idx];

	public int IndexOf(TabbedContent tab) => _tabList.IndexOf(tab);

	// =====================================================================

	public void SetContextMenu(IContextMenu contextMenu)
	{
		_contextMenu = contextMenu;
	}

	public void SetTabListener(ITabListener newListener)
	{
		_tabListener = newListener;
	}

	// =====================================================================

	public TabbedContent CreateTab(string tabName, string iconStyleName, TabbedContent newTabbedContent)
	{
		const string TabTemplateResourcesPath = "DLD UIToolkit/Tab";
		var tabTemplate = Resources.Load<VisualTreeAsset>(TabTemplateResourcesPath);
		var newTabFromTemplate = tabTemplate.Instantiate();
		const string TabElementName = "Tab";
		var newTabButton = newTabFromTemplate.Q<RadioButton>(TabElementName);
		newTabButton.label = tabName;
		newTabButton.labelElement.pickingMode = PickingMode.Ignore;

		var icon = newTabButton.Q<VisualElement>("Icon");
		if (!string.IsNullOrEmpty(iconStyleName))
		{
			newTabButton.Insert(0, icon);
			icon.AddToClassList(iconStyleName);
		}
		else
		{
			icon.style.display = DisplayStyle.None;
		}

		_tabContainer.Add(newTabButton);

		newTabbedContent.Init(newTabButton);

		newTabButton.userData = newTabbedContent;
		_tabList.Add(newTabbedContent);

		_tabBodyContainer.Add(newTabbedContent.Body);

		newTabButton.RegisterCallback(_onPressTab);
		newTabButton.RegisterCallback(_onPressTabContext);

		return newTabbedContent;
	}

	// =====================================================================

	public TReturn GetTabFromBody<TReturn>(VisualElement body) where TReturn : TabbedContent
	{
		for (int n = 0; n < _tabList.Count; ++n)
		{
			if (_tabList[n].Body == body)
			{
				return _tabList[n] as TReturn;
			}
		}

		return null;
	}

	void CloseTab(TabbedContent tabToClose)
	{
		// Ask user if they want to save, discard, or cancel.
		if (_tabListener != null && _tabListener.NeedToAskConfirmationToClose(tabToClose))
		{
			// Can't continue, need to wait for user to confirm.
			return;
		}

		ProceedToCloseTab(tabToClose);
	}

	public void ProceedToCloseTab(TabbedContent tabToClose)
	{
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
		_tabListener?.OnTabClosed(tabToClose);

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
		if (e.target is not RadioButton { userData: TabbedContent clickedTabContent })
		{
			return;
		}

		clickedTabContent.Body.style.display = e.newValue ? DisplayStyle.Flex : DisplayStyle.None;

		if (!e.previousValue && e.newValue)
		{
			_currentTab = clickedTabContent;
			_tabListener?.OnTabShown(this, clickedTabContent);
		}
	}

	public void ShowCurrentTabAsUnfocused()
	{
		_currentTab?.ShowAsUnfocused();
	}

	public void ShowCurrentTabAsFocused()
	{
		_currentTab?.ShowAsFocused();
	}

	// =====================================================================

	void OnPressTabContext(PointerDownEvent e)
	{
		if (e.target is not RadioButton { userData: TabbedContent clickedTabContent })
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
		_tabListener?.OnTabContextMenu(_contextMenu, clickedTabContent);
		_contextMenu.Show(e);
	}

	public void OnContextMenuChosen(ContextMenuParams parameters)
	{
		switch (parameters.UserArg1 as string)
		{
			case ContextMenuCloseTab:
				CloseTab(parameters.UserArg2 as TabbedContent);
				break;
			case ContextMenuCloseOtherTabs:
				var thisTab = parameters.UserArg2 as TabbedContent;
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
