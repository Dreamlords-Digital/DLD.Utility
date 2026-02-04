// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UIElements.Toggle;

namespace DLD.UIToolkit
{
	public enum SearchType : byte
	{
		Simple,
		SimpleIgnoreCase,
		Fuzzy,
		FuzzyIgnoreCase,
		Regex,
		RegexIgnoreCase
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
		readonly Button _clearButton;

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

			_clearButton = new Button();
			_clearButton.RegisterCallback<ClickEvent, SearchTextField>((e, me) => me.OnClearButtonPressed(), this);
			_clearButton.AddToClassList("dld-search-text-field__clear-button");

			var textBox = this.Q<VisualElement>("unity-text-input");
			textBox.Insert(0, _searchButton);
			textBox.Add(_clearButton);

			if (isDelayed)
			{
				RegisterCallback<ChangeEvent<string>, SearchTextField>((e, me) => me.OnDelayedChange(e), this);
			}
			else
			{
				RegisterCallback<ChangeEvent<string>, SearchTextField>((e, me) => me.OnImmediateChange(e), this);
				RegisterCallback<NavigationSubmitEvent, SearchTextField>((e, me) => me.OnImmediateSubmitText(e), this);
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
			if (string.IsNullOrEmpty(e.newValue) && !string.IsNullOrEmpty(e.previousValue))
			{
				// just cleared out the text
				_searchDelayRemaining = 0;
				_listener?.OnSearchTextChanged(_searchType, value);
			}
			else
			{
				_searchDelayRemaining = DefaultSearchDelay;
			}
		}

		void OnImmediateSubmitText(NavigationSubmitEvent e)
		{
			if (_searchDelayRemaining > 0)
			{
				_searchDelayRemaining = 0;
				_listener?.OnSearchTextChanged(_searchType, value);
				e.StopPropagation();
			}
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

			bool searchTypeIsSimple = _searchType is SearchType.Simple or SearchType.SimpleIgnoreCase;
			bool searchTypeIsFuzzy = _searchType is SearchType.Fuzzy or SearchType.FuzzyIgnoreCase;
			bool searchTypeIsRegex = _searchType is SearchType.Regex or SearchType.RegexIgnoreCase;

			_contextMenu.ClearMenu();
			_contextMenu.AddLabel("Search Type:");
			_contextMenu.AddMenu("Simple", tooltip: "Use * as wildcard.",
				menuItemStyle: searchTypeIsSimple ? ContextMenuItemStyle.Selected : ContextMenuItemStyle.Standard,
				listener: this, userArg1: SearchTypeSimple);
			_contextMenu.AddMenu("Fuzzy", tooltip: "Use fuzzy search algorithm. Fuzzy search is lenient on wrong spellings.",
				menuItemStyle: searchTypeIsFuzzy ? ContextMenuItemStyle.Selected : ContextMenuItemStyle.Standard,
				listener: this, userArg1: SearchTypeFuzzy);
			_contextMenu.AddMenu("Regex", tooltip: "Use regular expression.",
				menuItemStyle: searchTypeIsRegex ? ContextMenuItemStyle.Selected : ContextMenuItemStyle.Standard,
				listener: this, userArg1: SearchTypeRegex);
			_contextMenu.AddSeparator();
			_contextMenu.AddMenu("Match Case", tooltip: "Case sensitive or not.",
				menuItemStyle: _matchCase ? ContextMenuItemStyle.Selected : ContextMenuItemStyle.Standard,
				listener: this, userArg1: MatchCase);
			_contextMenu.Show(this, ElementAnchorPoint.LowerLeft, listener: this);
		}

		public void OnContextMenuChosen(ContextMenuParams parameters)
		{
			switch (parameters.UserArg1 as string)
			{
				case SearchTypeSimple:
					if (_searchType is SearchType.SimpleIgnoreCase or SearchType.Simple)
					{
						return;
					}
					_searchType = _matchCase ? SearchType.Simple : SearchType.SimpleIgnoreCase;
					break;
				case SearchTypeFuzzy:
					if (_searchType is SearchType.Fuzzy or SearchType.FuzzyIgnoreCase)
					{
						return;
					}
					_searchType = _matchCase ? SearchType.Fuzzy : SearchType.FuzzyIgnoreCase;
					break;
				case SearchTypeRegex:
					if (_searchType is SearchType.Regex or SearchType.RegexIgnoreCase)
					{
						return;
					}
					_searchType = _matchCase ? SearchType.Regex : SearchType.RegexIgnoreCase;
					break;
				case MatchCase:
					_matchCase = !_matchCase;
					if (_searchType is SearchType.Simple or SearchType.SimpleIgnoreCase)
					{
						_searchType = _matchCase ? SearchType.Simple : SearchType.SimpleIgnoreCase;
					}
					else if (_searchType is SearchType.Fuzzy or SearchType.FuzzyIgnoreCase)
					{
						_searchType = _matchCase ? SearchType.Fuzzy : SearchType.FuzzyIgnoreCase;
					}
					else if (_searchType is SearchType.Regex or SearchType.RegexIgnoreCase)
					{
						_searchType = _matchCase ? SearchType.Regex : SearchType.RegexIgnoreCase;
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

		void OnClearButtonPressed()
		{
			SetValueWithoutNotify("");
			_searchDelayRemaining = 0;
			_listener?.OnSearchTextChanged(_searchType, value);
		}

		// =====================================================================
	}
}
