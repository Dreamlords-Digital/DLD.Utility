// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;
using System.Linq;
using FuzzyString;
using UnityEngine;

namespace DLD.IMGUI
{
	public static class StringSearchPickerUtility
	{
		public static int OpenStringSearchPickers;
	}

	public class StringSearchPicker<TReturnType>
	{
		public struct Entry
		{
			readonly GUIContent _label;
			readonly GUIContent _tooltip;
			readonly TReturnType _returnValue;

			public GUIContent Label => _label;

			public TReturnType ReturnValue => _returnValue;

			public GUIContent Tooltip => _tooltip;

			public bool HasTooltip => _tooltip != null;


			public Entry(string label)
			{
				_label = string.IsNullOrEmpty(label) ? GUIContent.none : new GUIContent(label);
				_returnValue = default(TReturnType);
				_tooltip = null;
			}

			public Entry(string label, TReturnType returnValue)
			{
				_label = string.IsNullOrEmpty(label) ? GUIContent.none : new GUIContent(label);
				_returnValue = returnValue;
				_tooltip = null;
			}

			public Entry(string label, string tooltipText, TReturnType returnValue)
			{
				_label = string.IsNullOrEmpty(label) ? GUIContent.none : new GUIContent(label);
				_returnValue = returnValue;

				if (!string.IsNullOrEmpty(tooltipText))
				{
					_tooltip = new GUIContent(tooltipText);
				}
				else
				{
					_tooltip = null;
				}
			}

			public Entry(string label, Texture labelIcon, TReturnType returnValue)
			{
				_label = new GUIContent(label, labelIcon);
				_returnValue = returnValue;
				_tooltip = null;
			}

			public Entry(string label, Texture labelIcon, string tooltipText, TReturnType returnValue)
			{
				_label = new GUIContent(label, labelIcon);
				_returnValue = returnValue;

				if (!string.IsNullOrEmpty(tooltipText))
				{
					_tooltip = new GUIContent(tooltipText);
				}
				else
				{
					_tooltip = null;
				}
			}

			public Entry(string label, Texture labelIcon, string tooltipText, string tooltipIcon, TReturnType returnValue)
			{
				_label = new GUIContent(label, labelIcon);
				_returnValue = returnValue;
				_tooltip = new GUIContent(tooltipText, tooltipIcon);
			}

			public Entry(GUIContent labelGuiContent, TReturnType returnValue)
			{
				_label = labelGuiContent;
				_returnValue = returnValue;
				_tooltip = null;
			}

			public Entry(GUIContent labelGuiContent, string tooltipText, TReturnType returnValue)
			{
				_label = labelGuiContent;
				_returnValue = returnValue;

				if (!string.IsNullOrEmpty(tooltipText))
				{
					_tooltip = new GUIContent(tooltipText);
				}
				else
				{
					_tooltip = null;
				}
			}

			public Entry(GUIContent labelGuiContent, GUIContent tooltipGuiContent, TReturnType returnValue)
			{
				_label = labelGuiContent;
				_returnValue = returnValue;
				_tooltip = tooltipGuiContent;
			}
		}

		bool _showList;
		int _currentlySelectedIdx;
		Rect _textfieldRect;

		Entry[] _originalEntries;
		Entry[] _filteredEntries;

		string _tempSearchString = string.Empty;

		GUIStyle _textFieldGuiStyle;
		GUIStyle _entryGuiStyle;
		GUIStyle _entryTooltipGuiStyle;

		GUIStyle _backgroundGuiStyle;

		const string DEFAULT_TEXTFIELD_STYLE_NAME = "TextField";
		const string DEFAULT_ENTRY_STYLE_NAME = "PopupEntry";
		const string DEFAULT_DISABLED_ENTRY_STYLE_NAME = "PopupEntryDisabled";
		const string DEFAULT_SEPARATOR_ENTRY_STYLE_NAME = "PopupEntrySeparator";
		const string DEFAULT_TOOLTIP_STYLE_NAME = "PopupTooltip";
		const string DEFAULT_BG_STYLE_NAME = "Box";
		const string DEFAULT_BG_HIDDEN_STYLE_NAME = "PopupBoxHidden";

		const float DEFAULT_TOOLTIP_WIDTH = 200;

		string _textFieldGuiStyleName = DEFAULT_TEXTFIELD_STYLE_NAME;
		string _entryGuiStyleName = DEFAULT_ENTRY_STYLE_NAME;
		string _entryTooltipStyleName = DEFAULT_TOOLTIP_STYLE_NAME;

		string _bgGuiStyleName = DEFAULT_BG_STYLE_NAME;

		// ----------------------------------------------------------

		int _openedPickerId = -1;


		bool AreWeBeingSignaledToClose
		{
			get
			{
				return _showList &&
				       StringSearchPickerUtility.OpenStringSearchPickers > 1 &&
				       (_openedPickerId < StringSearchPickerUtility.OpenStringSearchPickers);
			}
		}

		bool IsThisPickerOpen
		{
			get
			{
				return _showList;
			}
		}

		void OpenThisPicker()
		{
			++StringSearchPickerUtility.OpenStringSearchPickers;

			_showList = true;
			_openedPickerId = StringSearchPickerUtility.OpenStringSearchPickers;

			_filteredEntries = _originalEntries.Where(o => IsANearStringMatch(o.ReturnValue.ToString(), _tempSearchString))
				.ToArray();
		}

		void CloseThisPicker()
		{
			--StringSearchPickerUtility.OpenStringSearchPickers;

			_showList = false;
			_openedPickerId = -1;

			_filteredEntries = _originalEntries;
		}

		// ----------------------------------------------------------

		bool IsANearStringMatch(string source, string target)
		{
			// Choose which algorithms should weigh in for the comparison
			var options = new FuzzyStringComparisonOptions[]
			{
				FuzzyStringComparisonOptions.UseOverlapCoefficient,
				FuzzyStringComparisonOptions.UseLongestCommonSubsequence,
				FuzzyStringComparisonOptions.UseLongestCommonSubstring
			};

			// Choose the relative strength of the comparison - is it almost exactly equal? or is it just close?
			var tolerance = FuzzyStringComparisonTolerance.Strong;

			// Get a boolean determination of approximate equality
			try
			{
				return source.ApproximatelyEquals(target, tolerance, options);
			}
			catch (NullReferenceException)
			{
				return false;
			}
		}

		// ----------------------------------------------------------

		string DrawTextField(Rect textFieldRect = default(Rect), bool shouldUseGUILayout = false)
		{
			if (_originalEntries == null || _originalEntries.Length == 0)
			{
				return string.Empty;
			}

			if (_currentlySelectedIdx > _originalEntries.Length - 1)
			{
				_currentlySelectedIdx = _originalEntries.Length - 1;
			}

			_tempSearchString = (shouldUseGUILayout)
				? GUILayout.TextField(_tempSearchString, _textFieldGuiStyle)
				: GUI.TextField(textFieldRect, _tempSearchString, _textFieldGuiStyle);

			if (string.IsNullOrEmpty(_tempSearchString))
			{
				return string.Empty;
			}
			else
			{
				if (IsThisPickerOpen)
				{
					CloseThisPicker();
				}
				else
				{
					OpenThisPicker();
				}
			}

			// ------------------------------------------------------------------------------
			// record the textfield's Rect to _buttonRect only if it's valid

			var lastRect = GUILayoutUtility.GetLastRect();

			if (Mathf.Approximately(lastRect.width, 1) && Mathf.Approximately(lastRect.height, 1))
			{
				return string.Empty;
			}

			_textfieldRect = lastRect;

			return _tempSearchString;
		}

		TReturnType DrawList(TReturnType selectedValue, bool isOverlay = false, Vector2 offset = default(Vector2))
		{
			//GUI.enabled = true;

			if (AreWeBeingSignaledToClose)
			{
				// we're being forcibly closed
				// so abort whole function and hide the list
				CloseThisPicker();
				return selectedValue;
			}


			if (StringSearchPickerUtility.OpenStringSearchPickers == 1 && _openedPickerId > 1 && _showList)
			{
				_openedPickerId = 1;
			}


			Rect listRect = new Rect(_textfieldRect.x + offset.x, _textfieldRect.y + offset.y + _textfieldRect.height - 1,
				_textfieldRect.width, 0);

			if (_filteredEntries != null)
			{
				for (int n = 0; n < _filteredEntries.Length; ++n)
				{
					if (_filteredEntries[n].Label == null)
					{
						continue;
					}

					listRect.height += _entryGuiStyle.CalcHeight(_filteredEntries[n].Label, listRect.width);
				}
			}


			bool done = false;

			if ((Event.current.type == EventType.MouseUp) && _showList && !listRect.Contains(Event.current.mousePosition))
			{
				done = true;
			}

			if (_showList && _filteredEntries != null)
			{
				if (isOverlay)
				{
					GUI.Box(listRect, string.Empty, _backgroundGuiStyle);
				}

				//var debugText = new StringBuilder();

				//debugText.AppendFormat("_buttonRect: {0} mousePosition: {1}\n", _buttonRect, Event.current.mousePosition);

				Rect thisButtonRect = new Rect();
				thisButtonRect.x = _textfieldRect.x + offset.x;
				thisButtonRect.width = _textfieldRect.width;

				float accumulatedY = _textfieldRect.height - 1;
				for (int n = 0; n < _filteredEntries.Length; ++n)
				{
					thisButtonRect.y = accumulatedY + _textfieldRect.y + offset.y;
					thisButtonRect.height = _entryGuiStyle.CalcHeight(_filteredEntries[n].Label, listRect.width);

					accumulatedY += thisButtonRect.height;

					//debugText.AppendFormat("n: {0} thisButtonRect: {1} mousePosition: {2} contains? {3} has tool tip? {4}\n", n, thisButtonRect, Event.current.mousePosition, thisButtonRect.Contains(Event.current.mousePosition), _entries[n].HasTooltip);

					if (thisButtonRect.Contains(Event.current.mousePosition) && _filteredEntries[n].HasTooltip)
					{
						Rect tooltipRect = new Rect(thisButtonRect);
						tooltipRect.x += thisButtonRect.width + _entryTooltipGuiStyle.margin.left;
						tooltipRect.y += _entryTooltipGuiStyle.margin.top;
						tooltipRect.width = Mathf.Min(DEFAULT_TOOLTIP_WIDTH,
							_entryTooltipGuiStyle.CalcSize(_filteredEntries[n].Tooltip).x);
						tooltipRect.height = _entryTooltipGuiStyle.CalcHeight(_filteredEntries[n].Tooltip, tooltipRect.width);

						//debugText.AppendFormat(">> n: {0} tooltipRect: {1}\n", n, tooltipRect);

						GUI.Label(tooltipRect, _filteredEntries[n].Tooltip, _entryTooltipGuiStyle);
					}

					bool shouldDisplayHover = isOverlay &&
					                          thisButtonRect.Contains(Event.current.mousePosition) &&
					                          Event.current.type != EventType.MouseDown;

					if (shouldDisplayHover)
					{
						//GUI.contentColor = _entryGuiStyle.hover.textColor;
						if (Event.current.type == EventType.Repaint)
						{
							//GUIContent hoverContent = new GUIContent(_entries[n].Label);
							//hoverContent.text = string.Format("<color=white>{0}</color>", hoverContent.text);

							_entryGuiStyle.Draw(thisButtonRect, _filteredEntries[n].Label, true,
								Event.current.type == EventType.MouseDown, false, false);
						}
					}

					if (!shouldDisplayHover && GUI.Button(thisButtonRect, _filteredEntries[n].Label, _entryGuiStyle))
					{
						_tempSearchString = string.Empty;
						_currentlySelectedIdx = n;
						selectedValue = _filteredEntries[n].ReturnValue;
						//debugText.AppendFormat("picked {0}\n", _entries[n].ReturnValue);
						CloseThisPicker();
					}
				}

				//Debug.Log(debugText.ToString());
			}

			if (done)
			{
				CloseThisPicker();
			}

			return selectedValue;
		}

		// -----------------------------------------------------------------------------

		int GetIndexFromValue(TReturnType initialValue)
		{
			if (_originalEntries == null)
			{
				return -1;
			}

			for (int n = 0; n < _originalEntries.Length; ++n)
			{
				if (EqualityComparer<TReturnType>.Default.Equals(_originalEntries[n].ReturnValue, initialValue))
				{
					return n;
				}
			}

			// error, specified initialValue not found in entry list
			return 0;
		}

		public void SetEntries(Entry[] entries)
		{
			_originalEntries = entries;
		}

		public void SetSelectedIndex(TReturnType initialValue)
		{
			_currentlySelectedIdx = GetIndexFromValue(initialValue);
		}

		public void SetSelectedIndexToFirstEntry()
		{
			_currentlySelectedIdx = 0;
		}

		public void SetGuiStyles(string buttonStyle, string entryStyle, string tooltipStyle, string bgStyle)
		{
			_textFieldGuiStyleName = buttonStyle;
			_entryGuiStyleName = entryStyle;
			_entryTooltipStyleName = tooltipStyle;

			_bgGuiStyleName = bgStyle;
		}

		void InitGuiStylesIfNeeded()
		{
			if (_textFieldGuiStyle == null && !string.IsNullOrEmpty(_textFieldGuiStyleName))
			{
				_textFieldGuiStyle = _textFieldGuiStyleName;
				_entryGuiStyle = _entryGuiStyleName;
				_entryTooltipGuiStyle = _entryTooltipStyleName;

				_backgroundGuiStyle = _bgGuiStyleName;
			}
		}

		public bool HasEntries
		{
			get
			{
				return _originalEntries != null && _originalEntries.Length > 0;
			}
		}

		public int EntryCount
		{
			get
			{
				if (_originalEntries == null)
				{
					return 0;
				}

				return _originalEntries.Length;
			}
		}

		// -----------------------------------------------------------------------------

		public TReturnType Draw(TReturnType selectedValue)
		{
			InitGuiStylesIfNeeded();
			DrawTextField(default(Rect), true);
			return DrawList(selectedValue);
		}

		public TReturnType Draw(Rect textFieldRect, TReturnType selectedValue)
		{
			InitGuiStylesIfNeeded();
			DrawTextField(textFieldRect);
			return DrawList(selectedValue);
		}

		public void DrawOverlayIfNeeded(Vector2 offset = default(Vector2))
		{
			InitGuiStylesIfNeeded();
			DrawList(default(TReturnType), true, offset);
		}
	}
}