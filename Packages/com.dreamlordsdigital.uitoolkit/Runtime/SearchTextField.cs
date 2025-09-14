using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public enum SearchType : byte
	{
		SimpleNotMatchCase,
		SimpleMatchCase,
		Fuzzy,
		Regex,
	}

	public interface ISearchTextFieldListener
	{
		void OnSearchTextChanged(SearchType searchType, string searchText);
	}

	[UxmlElement]
	public partial class SearchTextField : TextField, IContextMenuListener
	{
		const string SearchTypeSimple = "SearchTypeSimple";
		const string SearchTypeFuzzy = "SearchTypeFuzzy";
		const string SearchTypeRegex = "SearchTypeRegex";
		const string MatchCase = "MatchCase";

		const float DefaultSearchDelay = 0.33f;

		// =====================================================================

		SearchType _searchType = SearchType.Fuzzy;
		bool _matchCase;
		bool _wasInFocusBeforeClickingOnSearchButton;
		float _searchDelayRemaining;

		readonly Toggle _searchButton;

		IContextMenu _contextMenu;
		ISearchTextFieldListener _listener;

		// =====================================================================

		public SearchTextField()
		{
			keyboardType = TouchScreenKeyboardType.Search;
			AddToClassList("dld-search-text-field");
			RegisterCallback<FocusOutEvent, SearchTextField>((e, me) => me.OnFocusOut(e), this);

			_searchButton = new Toggle();
			_searchButton.RegisterCallback<ClickEvent, SearchTextField>((e, me) => me.OnSearchButtonPressed(), this);
			_searchButton.AddToClassList("dld-search-text-field__search-button");

			var textBox = this.Q<VisualElement>("unity-text-input");
			textBox.Add(_searchButton);

			if (isDelayed)
			{
				RegisterCallback<ChangeEvent<string>, SearchTextField>((e, me) => me.OnDelayedChange(e), this);
			}
			else
			{
				RegisterCallback<ChangeEvent<string>, SearchTextField>((e, me) => me.OnImmediateChange(e), this);
			}
		}

		public void SetContextMenu(IContextMenu newContextMenu)
		{
			_contextMenu = newContextMenu;
		}

		public void SetListener(ISearchTextFieldListener newListener)
		{
			_listener = newListener;
		}

		// =====================================================================

		public void Update(float deltaTimeSeconds)
		{
			if (_searchDelayRemaining > 0)
			{
				_searchDelayRemaining -= deltaTimeSeconds;
				if (_searchDelayRemaining <= 0)
				{
					_listener?.OnSearchTextChanged(_searchType, value);
				}
			}
		}

		void OnDelayedChange(ChangeEvent<string> e)
		{
			_listener?.OnSearchTextChanged(_searchType, e.newValue);
		}

		void OnImmediateChange(ChangeEvent<string> e)
		{
			_searchDelayRemaining = DefaultSearchDelay;
		}

		void OnFocusOut(FocusOutEvent e)
		{
			if (e.relatedTarget != null && e.relatedTarget is VisualElement elementThatGainedFocus)
			{
				if (IndexOf(elementThatGainedFocus) == -1)
				{
					// not ours
					_wasInFocusBeforeClickingOnSearchButton = false;
				}
			}
			else
			{
				_wasInFocusBeforeClickingOnSearchButton = true;
			}
		}

		void OnSearchButtonPressed()
		{
			Debug.Assert(_contextMenu != null, "_contextMenu should be assigned");

			bool searchTypeIsSimple = _searchType is SearchType.SimpleNotMatchCase or SearchType.SimpleMatchCase;

			ContextMenuItemStyle matchCaseStyle;
			string matchCaseTooltip;
			if (searchTypeIsSimple)
			{
				matchCaseTooltip = "Case sensitive or not.";
				if (_matchCase)
				{
					matchCaseStyle = ContextMenuItemStyle.Selected;
				}
				else
				{
					matchCaseStyle = ContextMenuItemStyle.Standard;
				}
			}
			else
			{
				matchCaseTooltip = "Only applies to Simple Search Type.";
				if (_matchCase)
				{
					matchCaseStyle = ContextMenuItemStyle.Selected | ContextMenuItemStyle.Disabled;
				}
				else
				{
					matchCaseStyle = ContextMenuItemStyle.Standard | ContextMenuItemStyle.Disabled;
				}
			}

			_contextMenu.ClearMenu();
			_contextMenu.AddLabel("Search Type:");
			_contextMenu.AddMenu("Simple", tooltip: "Use * as wildcard.",
				menuItemStyle: searchTypeIsSimple ? ContextMenuItemStyle.Selected : ContextMenuItemStyle.Standard,
				listener: this, userArg1: SearchTypeSimple);
			_contextMenu.AddMenu("Fuzzy", tooltip: "Use fuzzy search algorithm. Fuzzy search is lenient on wrong spellings.",
				menuItemStyle: _searchType == SearchType.Fuzzy ? ContextMenuItemStyle.Selected : ContextMenuItemStyle.Standard,
				listener: this, userArg1: SearchTypeFuzzy);
			_contextMenu.AddMenu("Regex", tooltip: "Use regular expression.",
				menuItemStyle: _searchType == SearchType.Regex ? ContextMenuItemStyle.Selected : ContextMenuItemStyle.Standard,
				listener: this, userArg1: SearchTypeRegex);
			_contextMenu.AddSeparator();
			_contextMenu.AddMenu("Match Case", tooltip: matchCaseTooltip,
				menuItemStyle: matchCaseStyle,
				listener: this, userArg1: MatchCase);
			_contextMenu.Show(this, ElementAnchorPoint.LowerRight, listener: this);
		}

		public void OnContextMenuChosen(int index, Label itemLabel, object itemTooltip, object userArg1, object userArg2)
		{
			switch (userArg1 as string)
			{
				case SearchTypeSimple:
					if (_searchType is SearchType.SimpleNotMatchCase or SearchType.SimpleMatchCase)
					{
						return;
					}
					_searchType = _matchCase ? SearchType.SimpleMatchCase : SearchType.SimpleNotMatchCase;
					break;
				case SearchTypeFuzzy:
					if (_searchType == SearchType.Fuzzy)
					{
						return;
					}
					_searchType = SearchType.Fuzzy;
					break;
				case SearchTypeRegex:
					if (_searchType == SearchType.Regex)
					{
						return;
					}
					_searchType = SearchType.Regex;
					break;
				case MatchCase:
					_matchCase = !_matchCase;
					if (_searchType is SearchType.SimpleNotMatchCase or SearchType.SimpleMatchCase)
					{
						_searchType = _matchCase ? SearchType.SimpleMatchCase : SearchType.SimpleNotMatchCase;
					}
					break;
			}
			_listener?.OnSearchTextChanged(_searchType, value);
		}

		public void OnContextMenuClosed(bool userCancelled)
		{
			_searchButton.SetValueWithoutNotify(false);

			if (_wasInFocusBeforeClickingOnSearchButton)
			{
				Focus();
				_wasInFocusBeforeClickingOnSearchButton = false;
			}
		}

		// =====================================================================
	}
}
