// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace DLD.IMGUI
{
	public enum DropDownBoxShowTooltipType
	{
		No,
		UnderlayOnly,
		OverlayOnly,
		Both
	}

	public class DropDownBox<TReturnType>
	{
		// ====================================================================

		public DropDownBox()
		{
		}

		public DropDownBox(List<Entry> choices)
		{
			AddChoicesAtEnd(choices);
			SetSelectedIndexToFirstEntry();
		}

		// ====================================================================

		public interface IEntry
		{
			GUIContent Label { get; }
			bool IsSeparator { get; }
			TReturnType ReturnValue { get; }
			bool Enabled { get; }

			GUIContent Tooltip { get; }
			bool HasTooltip { get; }

			// --------------------------------------------

			void SetEnabled(bool b);
		}

		public static bool IsReturnValueInEntryList(List<Entry> list, TReturnType returnValueToCheck)
		{
			for (int n = 0; n < list.Count; ++n)
			{
				if (EqualityComparer<TReturnType>.Default.Equals(list[n].ReturnValue, returnValueToCheck))
				{
					return true;
				}
			}

			return false;
		}

		public struct Entry : IEntry
		{
			readonly GUIContent _label;
			readonly GUIContent _tooltip;
			readonly TReturnType _returnValue;
			readonly bool _iconColorCustomized;
			readonly Color32 _iconColor;
			bool _enabled;

			// --------------------------------------------

			public GUIContent Label => _label;
			public bool IsSeparator => _label.text == DropDownBoxUtility.SEPARATOR;
			public bool IsIconColorCustomized => _iconColorCustomized;
			public Color32 IconColor => _iconColor;
			public TReturnType ReturnValue => _returnValue;
			public bool Enabled => _enabled;

			public GUIContent Tooltip => _tooltip;

			public bool HasTooltip =>
				_tooltip != null && _tooltip != GUIContent.none && !string.IsNullOrEmpty(_tooltip.text);

			// --------------------------------------------

			public void SetEnabled(bool enable)
			{
				_enabled = enable;
			}

			public void SetLabel(string label)
			{
				_label.text = label;
			}

			public void SetTooltip(string text)
			{
				if (_tooltip == null)
				{
					return;
				}

				_tooltip.text = text;
			}

			// --------------------------------------------

			public Entry(string label)
			{
				_label = string.IsNullOrEmpty(label) ? GUIContent.none : new GUIContent(label);
				_returnValue = default(TReturnType);
				_tooltip = null;
				_enabled = true;
				_iconColorCustomized = false;
				_iconColor = new Color32(255, 255, 255, 255);
			}

			public Entry(string label, TReturnType returnValue)
			{
				_label = string.IsNullOrEmpty(label) ? GUIContent.none : new GUIContent(label);
				_returnValue = returnValue;
				_tooltip = null;
				_enabled = true;
				_iconColorCustomized = false;
				_iconColor = new Color32(255, 255, 255, 255);
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

				_enabled = true;
				_iconColorCustomized = false;
				_iconColor = new Color32(255, 255, 255, 255);
			}

			public Entry(string label, string tooltipText, TReturnType returnValue, bool enabled)
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

				_enabled = enabled;
				_iconColorCustomized = false;
				_iconColor = new Color32(255, 255, 255, 255);
			}

			public Entry(string label, Texture labelIcon, TReturnType returnValue)
			{
				_label = new GUIContent(label, labelIcon);
				_returnValue = returnValue;
				_tooltip = null;
				_enabled = true;
				_iconColorCustomized = false;
				_iconColor = new Color32(255, 255, 255, 255);
			}

			public Entry(string label, Texture labelIcon, Color32 iconColor, TReturnType returnValue)
			{
				_label = new GUIContent(label, labelIcon);
				_returnValue = returnValue;
				_tooltip = null;
				_enabled = true;
				_iconColorCustomized = true;
				_iconColor = iconColor;
			}

			public Entry(string label, Texture labelIcon, Color32 iconColor, string tooltipText, TReturnType returnValue)
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

				_enabled = true;
				_iconColorCustomized = true;
				_iconColor = iconColor;
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

				_enabled = true;
				_iconColorCustomized = false;
				_iconColor = new Color32(255, 255, 255, 255);
			}

			public Entry(string label, Texture labelIcon, string tooltipText, string tooltipIcon, TReturnType returnValue)
			{
				_label = new GUIContent(label, labelIcon);
				_returnValue = returnValue;
				_tooltip = new GUIContent(tooltipText, tooltipIcon);
				_enabled = true;
				_iconColorCustomized = false;
				_iconColor = new Color32(255, 255, 255, 255);
			}

			public Entry(GUIContent labelGuiContent, TReturnType returnValue)
			{
				_label = labelGuiContent;
				_returnValue = returnValue;
				_tooltip = null;
				_enabled = true;
				_iconColorCustomized = false;
				_iconColor = new Color32(255, 255, 255, 255);
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

				_enabled = true;
				_iconColorCustomized = false;
				_iconColor = new Color32(255, 255, 255, 255);
			}

			public Entry(GUIContent labelGuiContent, GUIContent tooltipGuiContent, TReturnType returnValue)
			{
				_label = labelGuiContent;
				_returnValue = returnValue;
				_tooltip = tooltipGuiContent;
				_enabled = true;
				_iconColorCustomized = false;
				_iconColor = new Color32(255, 255, 255, 255);
			}
		}

		// ====================================================================

		/// <summary>
		/// True when the dropdown box has focus so it can show the choices.
		/// </summary>
		bool _showChoices;

		int _currentlySelectedIdx;

		/// <summary>
		/// Needed so the x, y position of the choices can be determined.
		/// </summary>
		Rect _buttonRect;

		// ----------------------------------
		// scrollbar properties

		/// <summary>
		/// If dropdown box is bigger than this, it will be forced to make use of the scrollbar.
		/// Set to 0 to disable. Default is 0.
		/// </summary>
		float _maxHeight;

		public void SetMaxHeight(float newMaxHeight)
		{
			_maxHeight = newMaxHeight;
		}

		bool _shouldAppearAbove;
		bool _needsScrollbar;
		Vector2 _scrollbarPos;

		/// <summary>
		/// The rect that's used instead of <see cref="_choicesBoxRect"/> if a scrollbar is used.
		/// </summary>
		Rect _scrollRect;

		// ----------------------------------

		/// <summary>
		/// Cached value of the choices Rect
		/// </summary>
		Rect _choicesBoxRect;

		float _choicesHeight;
		float _smallerHeight;

		bool _choicesBoxSizeUpdated;

		readonly List<Entry> _choices = new List<Entry>();

		Func<TReturnType, TReturnType, bool> _customEqualityFunc;

		public void SetEqualityComparer(Func<TReturnType, TReturnType, bool> customEqualityFunc)
		{
			_customEqualityFunc = customEqualityFunc;
		}

		bool _shouldShowButtonTooltip;

		// ----------------------------------------------------------

		GUIContent _noSelection = new GUIContent("<i>Choose...</i>");

		GUIStyle _buttonGuiStyle;
		GUIStyle _entryGuiStyle;
		GUIStyle _entrySelectedGuiStyle;
		GUIStyle _entryDisabledGuiStyle;
		GUIStyle _entrySeparatorGuiStyle;
		GUIStyle _entryTooltipGuiStyle;

		GUIStyle _backgroundGuiStyle;

		const string DEFAULT_BUTTON_STYLE_NAME = "Popup";
		const string DEFAULT_ENTRY_STYLE_NAME = "PopupEntry";
		const string DEFAULT_DISABLED_ENTRY_STYLE_NAME = "PopupEntryDisabled";
		const string DEFAULT_ENTRY_SELECTED_STYLE_NAME = "PopupEntrySelected";
		const string DEFAULT_SEPARATOR_ENTRY_STYLE_NAME = "PopupEntrySeparator";
		const string DEFAULT_TOOLTIP_STYLE_NAME = "PopupTooltip";
		const string DEFAULT_BG_STYLE_NAME = "Box";

		const float DEFAULT_TOOLTIP_WIDTH = 200;

		string _buttonGuiStyleName = DEFAULT_BUTTON_STYLE_NAME;
		string _entryGuiStyleName = DEFAULT_ENTRY_STYLE_NAME;
		string _entrySelectedGuiStyleName = DEFAULT_ENTRY_SELECTED_STYLE_NAME;
		string _entryDisabledGuiStyleName = DEFAULT_DISABLED_ENTRY_STYLE_NAME;
		string _entrySeparatorGuiStyleName = DEFAULT_SEPARATOR_ENTRY_STYLE_NAME;
		string _entryTooltipStyleName = DEFAULT_TOOLTIP_STYLE_NAME;

		string _bgGuiStyleName = DEFAULT_BG_STYLE_NAME;

		// ----------------------------------------------------------

		DropDownBoxShowTooltipType _showTooltips = DropDownBoxShowTooltipType.Both;

		public void ShowTooltipsOnUnderlayOnly(bool newVal)
		{
			if (newVal)
			{
				_showTooltips = DropDownBoxShowTooltipType.UnderlayOnly;
			}
		}

		public void ShowTooltips(DropDownBoxShowTooltipType showTooltipType)
		{
			_showTooltips = showTooltipType;
		}

		// ----------------------------------------------------------


		Action<TReturnType> _onChosenValueCallback;

		public void SetOnChosenValueCallback(Action<TReturnType> newCallback)
		{
			_onChosenValueCallback = newCallback;
		}

		int _openedDropDownBoxId = -1;


		bool AreWeBeingSignaledToClose
		{
			get
			{
				return _showChoices &&
				       (DropDownBoxUtility.IsThisDropDownBoxObscured(_openedDropDownBoxId) ||
				        DropDownBoxUtility.IsNoDropDownBoxOpen);
			}
		}


		public void SetOpenedState(bool newOpenedState)
		{
			_showChoices = newOpenedState;
		}

		public bool IsThisDropDownBoxOpen
		{
			get
			{
				return _showChoices;
			}
		}

		void OpenThisDropDownBox()
		{
			_showChoices = true;
			_openedDropDownBoxId = DropDownBoxUtility.PushDropDownBox();

			//BetterDebug.Log("DropDownBox: Opened {0}. with id {1}",
			//	typeof(TReturnType).Name, _openedDropDownBoxId);
		}

		void CloseThisDropDownBox()
		{
			DropDownBoxUtility.PopDropDownBox();

			//BetterDebug.Log("DropDownBox: Closed {0}. with id {1} DropDownBoxes still open: {2}",
			//	typeof(TReturnType).Name, _openedDropDownBoxId.ToString(), DropDownBoxUtility.IsAnyDropDownBoxOpen);

			_showChoices = false;
			_openedDropDownBoxId = -1;
		}

		public float PixelHeight
		{
			get
			{
				InitGuiStylesIfNeeded();
				return _buttonGuiStyle.lineHeight + _buttonGuiStyle.margin.vertical;
			}
		}

		// ----------------------------------------------------------

		/// <summary>
		/// If shortening the choices height (happens when the bottom part of the choices rect is beyond the bottom part of the screen)
		/// will be less than this, the dropdown choices will be shown above the button instead.
		/// </summary>
		const int MIN_HEIGHT_ALLOWED = 220;

		const int SCROLLBAR_BOTTOM_MARGIN = 20;

		const int
			VERTICAL_SCREEN_MARGIN =
				Utility.UNITY_TOP_BAR_HEIGHT + 15; // 15 is for the bottom scrollbar in a horizontal window stack

		TReturnType DrawList(TReturnType selectedValue, bool isOverlay = false, Vector2 offset = default(Vector2))
		{
			if (AreWeBeingSignaledToClose)
			{
				// we're being forcibly closed
				// so abort whole function and hide the list
				CloseThisDropDownBox();
				return selectedValue;
			}

			if (DropDownBoxUtility.IsOnlyOneDropDownBoxOpen && _openedDropDownBoxId > 1 && _showChoices)
			{
				_openedDropDownBoxId = 1;
				//Debug.LogFormat("changed id of {0} to {1}", typeof(T).Name, _openedDropDownBoxId);
			}

			if (_choices == null)
			{
				return selectedValue;
			}

			SetChoicesBoxSizeIfNeeded();

			_choicesBoxRect.x = _buttonRect.x + offset.x;

			// initially place it below the dropdown button
			_choicesBoxRect.y = _buttonRect.y + offset.y + _buttonRect.height - 1;

			// choices width is always snapped to the button's width
			_choicesBoxRect.width = _buttonRect.width;

			// reset choices height to the max first
			_choicesBoxRect.height = _choicesHeight;

			var usableScreenHeight = Screen.height - VERTICAL_SCREEN_MARGIN;

			if (isOverlay && Event.current.type == EventType.Repaint)
			{
				// bottom-most Y coordinate of the choices box rect if it was placed below the button
				var bottomY = _buttonRect.y + offset.y + _buttonRect.height + _choicesBoxRect.height;

				if (bottomY > usableScreenHeight || (_maxHeight > 0 && _choicesBoxRect.height > _maxHeight))
				{
					// if current bottom of the choices rect is beyond bottom part of screen
					// calculate what the height would be to shorten it to just at the bottom part of the screen
					// so that it fits
					_smallerHeight = _choicesBoxRect.height - (bottomY - usableScreenHeight) - SCROLLBAR_BOTTOM_MARGIN;

					if (_smallerHeight < MIN_HEIGHT_ALLOWED)
					{
						// if the resulting height is too small
						// don't use that
						// instead, show the choices rect above the button
						// because the assumption is, in this case,
						// there is more space above than below
						_shouldAppearAbove = true;

						// move the choices rect above the dropdown button, instead of below it
						_choicesBoxRect.y = _buttonRect.y + offset.y - _choicesBoxRect.height;

						// but still, cap it to the max height if it's too much
						if (_maxHeight > 0 && _choicesBoxRect.height > _maxHeight)
						{
							// since we shortened the choices rect, use a scrollbar for it
							_needsScrollbar = true;
							_scrollRect.x = _choicesBoxRect.x;
							_scrollRect.y = _buttonRect.y + offset.y - _maxHeight;
							_scrollRect.width = _choicesBoxRect.width;
							_scrollRect.height = _maxHeight;
						}
						else
						{
							_needsScrollbar = false;
						}
					}
					else
					{
						// keep the choices rect below the dropdown button
						// and use this shorter height instead
						_shouldAppearAbove = false;

						// but still, cap it to the max height if it's too much
						if (_maxHeight > 0 && _smallerHeight > _maxHeight)
						{
							_smallerHeight = _maxHeight;
						}

						// since we shortened the choices rect, use a scrollbar for it
						_needsScrollbar = true;

						_scrollRect.x = _choicesBoxRect.x;
						_scrollRect.y = _buttonRect.y + offset.y + _buttonRect.height - 1;
						_scrollRect.width = _choicesBoxRect.width;
						_scrollRect.height = _smallerHeight;
					}
				}
				else
				{
					// choices rect fits below the dropdown button
					// so no changes are needed
					_shouldAppearAbove = false;
					_needsScrollbar = false;
				}
			}


			// this is outside the if-check above so that it still works when not drawing overlay
			if (_shouldAppearAbove)
			{
				// move the choices rect above the dropdown button, instead of below it

				if (_maxHeight > 0 && _choicesHeight > _maxHeight)
				{
					//_choicesBoxRect.height = _maxHeight;

					_scrollRect.x = _choicesBoxRect.x;
					_scrollRect.y = _buttonRect.y + offset.y - _maxHeight;
					_scrollRect.width = _choicesBoxRect.width;
					_scrollRect.height = _maxHeight;
				}
				else
				{
					_choicesBoxRect.y = _buttonRect.y + offset.y - _choicesBoxRect.height;
				}
			}
			else
			{
				if (_needsScrollbar)
				{
					_scrollRect.x = _choicesBoxRect.x;
					_scrollRect.y = _buttonRect.y + offset.y + _buttonRect.height - 1;
					_scrollRect.width = _choicesBoxRect.width;
					_scrollRect.height = _smallerHeight;
				}
			}

			var done = (Event.current.type == EventType.MouseUp) &&
			           _showChoices &&
			           (_needsScrollbar
				           ? !_scrollRect.Contains(Event.current.mousePosition)
				           : !_choicesBoxRect.Contains(Event.current.mousePosition));

			Rect tooltipRect = new Rect();
			GUIContent tooltipContent = null;

			var showTooltip = isOverlay
				? (_showTooltips == DropDownBoxShowTooltipType.OverlayOnly ||
				   _showTooltips == DropDownBoxShowTooltipType.Both)
				: (_showTooltips == DropDownBoxShowTooltipType.UnderlayOnly ||
				   _showTooltips == DropDownBoxShowTooltipType.Both);
			if (_showTooltips == DropDownBoxShowTooltipType.No)
			{
				showTooltip = false;
			}

			if (_showChoices)
			{
				var scrollbarWidth = GUI.skin.verticalScrollbar.fixedWidth;
				if (_needsScrollbar)
				{
					var insideRect = new Rect(0, 0, _choicesBoxRect.width - scrollbarWidth, _choicesHeight);

					//var underlayScrollRect = new Rect(_scrollRect);
					//underlayScrollRect.x = _buttonRect.x + offset.x;
					//underlayScrollRect.y = _buttonRect.y + offset.y + _buttonRect.height - 1;

					GUI.Box(_scrollRect, string.Empty, _backgroundGuiStyle);

					_scrollbarPos = GUI.BeginScrollView(_scrollRect, _scrollbarPos, insideRect, false, true);
				}
				else
				{
					if (isOverlay)
					{
						GUI.Box(_choicesBoxRect, string.Empty, _backgroundGuiStyle);
					}

					GUI.BeginGroup(_choicesBoxRect);
				}

				//var debugText = new StringBuilder();
				//debugText.AppendFormat("DropDownBox<{0}> --------------------\n", typeof(T).Name);
				//debugText.AppendFormat("_buttonRect: {0} mousePosition: {1}\n", _buttonRect, Event.current.mousePosition);

				Rect thisButtonRect = new Rect();
				thisButtonRect.x = /*_needsScrollbar ?*/ 0 /*: _buttonRect.x + offset.x*/;
				thisButtonRect.width = _buttonRect.width - (_needsScrollbar ? scrollbarWidth : 0);

				float accumulatedY = /*_needsScrollbar ?*/ 1 /*: _buttonRect.height - 1*/;
				for (int n = 0; n < _choices.Count; ++n)
				{
					thisButtonRect.y = accumulatedY;

					GUIStyle thisButtonStyle = _entryGuiStyle;

					if (_choices[n].IsSeparator)
					{
						thisButtonRect.height = _entrySeparatorGuiStyle.fixedHeight;

						if (GUI.Button(thisButtonRect, string.Empty, _entrySeparatorGuiStyle))
						{
							done = false;
						}
					}
					else
					{
						if (!_choices[n].Enabled)
						{
							thisButtonStyle = _entryDisabledGuiStyle;
						}
						else if (n == _currentlySelectedIdx)
						{
							thisButtonStyle = _entrySelectedGuiStyle;
						}

						thisButtonRect.height = thisButtonStyle.CalcHeight(_choices[n].Label, _buttonRect.width);

						//debugText.AppendFormat("n: {0} thisButtonRect: {1} mousePosition: {2} contains? {3} has tool tip? {4}\n", n, thisButtonRect, Event.current.mousePosition, thisButtonRect.Contains(Event.current.mousePosition), _entries[n].HasTooltip);

						if (thisButtonRect.Contains(Event.current.mousePosition) && _choices[n].HasTooltip)
						{
							//tooltipRect = new Rect(thisButtonRect);
							tooltipRect.x = _buttonRect.x +
							                offset.x +
							                thisButtonRect.width +
							                _entryTooltipGuiStyle.margin.left;

							if (_shouldAppearAbove)
							{
								var usedHeight = _needsScrollbar ? _scrollRect.height : _choicesBoxRect.height;
								tooltipRect.y = _buttonRect.y +
								                offset.y +
								                accumulatedY -
								                usedHeight -
								                1 -
								                _scrollbarPos.y +
								                _entryTooltipGuiStyle.margin.top;
							}
							else
							{
								tooltipRect.y = _buttonRect.y +
								                offset.y +
								                accumulatedY +
								                _buttonRect.height -
								                1 -
								                _scrollbarPos.y +
								                _entryTooltipGuiStyle.margin.top;
							}

							tooltipRect.width = Mathf.Min(DEFAULT_TOOLTIP_WIDTH,
								_entryTooltipGuiStyle.CalcSize(_choices[n].Tooltip).x);
							tooltipRect.height = _entryTooltipGuiStyle.CalcHeight(_choices[n].Tooltip, tooltipRect.width);

							if (tooltipRect.yMax > usableScreenHeight)
							{
								tooltipRect.y = usableScreenHeight - tooltipRect.height;
							}

							if (_needsScrollbar)
							{
								tooltipRect.x += GUI.skin.verticalScrollbar.fixedWidth;
							}
							//debugText.AppendFormat(">> n: {0} tooltipRect: {1}\n", n, tooltipRect);

							tooltipContent = _choices[n].Tooltip;
						}

						bool isIconColorCustomized = _choices[n].IsIconColorCustomized;

						var prevColor = GUI.contentColor;

						if (isIconColorCustomized)
						{
							GUI.contentColor = new Color(0, 0, 0, 0);
						}

						bool shouldDisplayHover = isOverlay &&
						                          thisButtonRect.Contains(Event.current.mousePosition) &&
						                          Event.current.type != EventType.MouseDown;

						if (shouldDisplayHover)
						{
							//GUI.contentColor = thisButtionStyle.hover.textColor;
							if (Event.current.type == EventType.Repaint)
							{
								//GUIContent hoverContent = new GUIContent(_entries[n].Label);
								//hoverContent.text = string.Format("<color=white>{0}</color>", hoverContent.text);

								thisButtonStyle.Draw(thisButtonRect, _choices[n].Label, true,
									Event.current.type == EventType.MouseDown, false, false);
							}
						}

						if ( /*!shouldDisplayHover &&*/ GUI.Button(thisButtonRect, _choices[n].Label, thisButtonStyle) &&
						                                _choices[n].Enabled)
						{
							_currentlySelectedIdx = n;
							selectedValue = _choices[n].ReturnValue;
							//debugText.AppendFormat("picked {0}\n", _entries[n].ReturnValue);
							Debug.Log($"DropdownBox: picked {_choices[n].Label.text} ({selectedValue})");

							if (_onChosenValueCallback != null)
							{
								_onChosenValueCallback(_choices[n].ReturnValue);
							}

							CloseThisDropDownBox();
						}

						if (isIconColorCustomized)
						{
							GUI.contentColor = prevColor;

							Rect iconRect = new Rect(thisButtonRect);
							iconRect.x += 3;
							iconRect.width = _choices[n].Label.image.width;
							iconRect.height = _choices[n].Label.image.height;
							iconRect.y += (thisButtonRect.height - iconRect.height) / 2;
							Widget.DrawIcon(iconRect, _choices[n].Label.image, _choices[n].IconColor);

							Rect labelRect = new Rect(thisButtonRect);
							labelRect.xMin += iconRect.width + 2;
							GUI.Label(labelRect, _choices[n].Label.text, _entryGuiStyle);
						}
					}

					accumulatedY += thisButtonRect.height;
				}

				if (_needsScrollbar)
				{
					GUI.EndScrollView(true);
				}
				else
				{
					GUI.EndGroup();
				}

				// tooltip when mouse is over a choice
				//Debug.Log(debugText.ToString());
				if (showTooltip && (tooltipContent != null))
				{
					GUI.Label(tooltipRect, tooltipContent, _entryTooltipGuiStyle);
				}
			}
			else
			{
				// this dropdown box is closed
				// but check if mouse is over the dropdown box, if so, draw the tooltip for the currently selected choice
				//
				if (!isOverlay && !OverlayUtility.IsAnyOverlayOpen)
				{
					Rect adjustedButtonRect = new Rect(_buttonRect.x + offset.x, _buttonRect.y + offset.y, _buttonRect.width,
						_buttonRect.height);
					if (adjustedButtonRect.Contains(Event.current.mousePosition) &&
					    (_currentlySelectedIdx >= 0 && _currentlySelectedIdx < _choices.Count) &&
					    _choices[_currentlySelectedIdx].HasTooltip)
					{
						_shouldShowButtonTooltip = true;
					}
					else
					{
						_shouldShowButtonTooltip = false;
					}
				}

				// then actually draw the tooltip beside the button
				//
				if (isOverlay && _shouldShowButtonTooltip && _choices[_currentlySelectedIdx].HasTooltip)
				{
					// position to the right
					tooltipRect.x = _buttonRect.x + offset.x + _buttonRect.width + _entryTooltipGuiStyle.margin.left;
					tooltipRect.y = _buttonRect.y + offset.y - 5;

					tooltipContent = _choices[_currentlySelectedIdx].Tooltip;

					tooltipRect.width = Mathf.Min(DEFAULT_TOOLTIP_WIDTH, _entryTooltipGuiStyle.CalcSize(tooltipContent).x);
					tooltipRect.height = _entryTooltipGuiStyle.CalcHeight(tooltipContent, tooltipRect.width);

					if (tooltipRect.yMax > usableScreenHeight)
					{
						tooltipRect.y = usableScreenHeight - tooltipRect.height;
					}
					//Debug.LogFormat("tooltip on button: {0} size: {1}", tooltipContent.text, tooltipRect);

					GUI.Label(tooltipRect, tooltipContent, _entryTooltipGuiStyle);
				}
			}

			if (done)
			{
				CloseThisDropDownBox();
			}

			return selectedValue;
		}

		// -----------------------------------------------------------------------------

		public bool Contains(TReturnType returnValueToCheck)
		{
			return GetIndexFromValue(returnValueToCheck) > -1;
		}

		int GetIndexFromValue(TReturnType initialValue)
		{
			Assert.IsNotNull(_choices, $"DropDownBox `Entries` is null upon trying to get index of {initialValue}");
			if (_choices == null)
			{
				return -1;
			}

			if (_customEqualityFunc != null)
			{
				for (int n = 0; n < _choices.Count; ++n)
				{
					if (_customEqualityFunc(_choices[n].ReturnValue, initialValue))
					{
						return n;
					}
				}

				return -1;
			}

			for (int n = 0; n < _choices.Count; ++n)
			{
				if (EqualityComparer<TReturnType>.Default.Equals(_choices[n].ReturnValue, initialValue))
				{
					return n;
				}
			}

			return -1;
		}

		public void SetChoices(IReadOnlyList<Entry> entriesToUse)
		{
			_choices.Clear();

			if (entriesToUse != null)
			{
				_choices.AddRange(entriesToUse);
			}

			UpdateChoicesBoxSize();
		}

		public void SetChoices(DropDownBox<TReturnType> otherDropDownBox, int startIdx = 0, int endIdx = -1)
		{
			_choices.Clear();

			if (otherDropDownBox != null)
			{
				if (startIdx == 0 && endIdx == -1)
				{
					_choices.AddRange(otherDropDownBox._choices);
				}
				else
				{
					if (endIdx == -1)
					{
						endIdx = otherDropDownBox._choices.Count - 1;
					}

					if (startIdx < 0 || startIdx >= otherDropDownBox._choices.Count)
					{
						startIdx = 0;
					}

					if (endIdx < 0 || endIdx >= otherDropDownBox._choices.Count)
					{
						endIdx = otherDropDownBox._choices.Count - 1;
					}

					for (int n = startIdx; n <= endIdx; ++n)
					{
						_choices.Add(otherDropDownBox._choices[n]);
					}
				}
			}

			UpdateChoicesBoxSize();
		}

		public void ClearChoices()
		{
			_choices.Clear();
		}

		public void AddChoicesAtEnd(IReadOnlyList<Entry> entriesToAdd)
		{
			if (entriesToAdd == null)
			{
				return;
			}

			_choices.AddRange(entriesToAdd);
			UpdateChoicesBoxSize();
		}

		public void AddChoiceAtStart(Entry entryToAdd)
		{
			_choices.Insert(0, entryToAdd);
			UpdateChoicesBoxSize();
		}

		public void AddChoiceAtStart(string label, string tooltipText, TReturnType returnValue)
		{
			_choices.Insert(0, new Entry(label, tooltipText, returnValue));
			UpdateChoicesBoxSize();
		}

		public void AddChoiceAtEnd(Entry entryToAdd)
		{
			_choices.Add(entryToAdd);
			UpdateChoicesBoxSize();
		}

		public void AddChoiceAtEnd(string label, string tooltipText, TReturnType returnValue)
		{
			_choices.Add(new Entry(label, tooltipText, returnValue));
			UpdateChoicesBoxSize();
		}

		public void AddChoiceAtEndIfReturnValueNotInYet(Entry entryToAdd)
		{
			var foundIdx = GetIndexFromValue(entryToAdd.ReturnValue);
			if (foundIdx > -1)
			{
				// entryToAdd is already in our choices list, abort.
				return;
			}

			_choices.Add(entryToAdd);
			UpdateChoicesBoxSize();
		}

		void UpdateChoicesBoxSize()
		{
			_choicesBoxSizeUpdated = false;
		}

		void SetChoicesBoxSizeIfNeeded()
		{
			if (_choicesBoxSizeUpdated || _choices == null || _buttonRect.width <= 0)
			{
				return;
			}

			const int BOTTOM_PADDING = 2;

			InitGuiStylesIfNeeded();
			_choicesHeight = BOTTOM_PADDING;
			for (int n = 0; n < _choices.Count; ++n)
			{
				if (_choices[n].Label == null)
				{
					continue;
				}

				if (_choices[n].IsSeparator)
				{
					_choicesHeight += _entrySeparatorGuiStyle.fixedHeight;
				}
				else
				{
					_choicesHeight += _entryGuiStyle.CalcHeight(_choices[n].Label, _buttonRect.width);
				}
			}

			_choicesBoxRect.width = _buttonRect.width;
			_choicesBoxRect.height = _choicesHeight;
			_choicesBoxSizeUpdated = true;
		}

		public void SetChoiceEnabled(int idx, bool b)
		{
			var newChoice = _choices[idx];
			newChoice.SetEnabled(b);
			_choices[idx] = newChoice;
		}

		public void SetChoiceEnabled(TReturnType returnValue, bool b)
		{
			var idx = GetIndexFromValue(returnValue);
			if (idx > -1)
			{
				SetChoiceEnabled(idx, b);
			}
		}

		public void SetChoiceLabel(int idx, string newLabel)
		{
			var newChoice = _choices[idx];
			newChoice.SetLabel(newLabel);
			_choices[idx] = newChoice;
		}

		public void SetChoiceLabel(int idx, string newLabel, string tooltipText)
		{
			var newChoice = _choices[idx];
			newChoice.SetLabel(newLabel);
			newChoice.SetTooltip(tooltipText);
			_choices[idx] = newChoice;
		}

		public void SetAllChoicesEnabled(bool b)
		{
			for (int n = 0; n < _choices.Count; ++n)
			{
				var entryToModify = _choices[n];
				entryToModify.SetEnabled(b);
				_choices[n] = entryToModify;
			}
		}

		public void SetSelectedIndex(TReturnType valueToSelect)
		{
			UnityEngine.Profiling.Profiler.BeginSample("DropDownBox.SetSelectedIndex");

			_currentlySelectedIdx = GetIndexFromValue(valueToSelect);

			UnityEngine.Profiling.Profiler.EndSample();

			//if (_currentlySelectedIdx == -1)
			//{
			//	var errorMessage = new StringBuilder();
			//	errorMessage.AppendFormat(
			//		"DropDownBox: trying to get index of initialValue {0} but it's not found in the choices. choice count: {1}\n",
			//		initialValue, _choices.Count);

			//	for (int n = 0; n < _choices.Count; ++n)
			//	{
			//		if (_customEqualityFunc != null)
			//		{
			//			errorMessage.AppendFormat("{0}) {1} == {2} ? {3}\n", n, initialValue, _choices[n].ReturnValue,
			//				_customEqualityFunc(_choices[n].ReturnValue, initialValue));
			//		}
			//		else
			//		{
			//			errorMessage.AppendFormat("{0}) {1} == {2} ? {3}\n", n, initialValue, _choices[n].ReturnValue,
			//				EqualityComparer<TReturnType>.Default.Equals(_choices[n].ReturnValue, initialValue));
			//		}
			//	}
			//	Debug.LogErrorFormat(errorMessage.ToString());
			//}
		}

		public void SetSelectedIndex(Predicate<Entry> predicate)
		{
			_currentlySelectedIdx = _choices.FindIndex(predicate);
		}

		public void SetSelectedIndexToFirstEntry()
		{
			_currentlySelectedIdx = 0;
		}

		public void ClearSelection()
		{
			_currentlySelectedIdx = -1;
		}

		public void SetGuiStyles(string buttonStyle, string entryStyle, string entrySelectedStyle,
			string entryDisabledStyle, string entrySeparatorStyle, string tooltipStyle, string bgStyle)
		{
			_buttonGuiStyleName = buttonStyle;
			_entryGuiStyleName = entryStyle;
			_entrySelectedGuiStyleName = entrySelectedStyle;
			_entryDisabledGuiStyleName = entryDisabledStyle;
			_entrySeparatorGuiStyleName = entrySeparatorStyle;
			_entryTooltipStyleName = tooltipStyle;

			_bgGuiStyleName = bgStyle;
		}

		public void SetButtonGuiStyle(string buttonStyle)
		{
			_buttonGuiStyleName = buttonStyle;
		}

		void InitGuiStylesIfNeeded()
		{
			if (_buttonGuiStyle == null && !string.IsNullOrEmpty(_buttonGuiStyleName))
			{
				_buttonGuiStyle = _buttonGuiStyleName;
				_entryGuiStyle = _entryGuiStyleName;
				_entrySelectedGuiStyle = _entrySelectedGuiStyleName;
				_entryDisabledGuiStyle = _entryDisabledGuiStyleName;
				_entrySeparatorGuiStyle = _entrySeparatorGuiStyleName;
				_entryTooltipGuiStyle = _entryTooltipStyleName;

				_backgroundGuiStyle = _bgGuiStyleName;
			}
		}

		public bool HasChoices
		{
			get
			{
				return _choices != null && _choices.Count > 0;
			}
		}

		public int ChoiceCount
		{
			get
			{
				if (_choices == null)
				{
					return 0;
				}

				return _choices.Count;
			}
		}

		// -----------------------------------------------------------------------------

		public TReturnType GetFirstChoice()
		{
			if (_choices != null && _choices.Count > 0)
			{
				return _choices[0].ReturnValue;
			}

			return default(TReturnType);
		}

		public TReturnType GetChoice(int idx)
		{
			if (_choices != null && _choices.Count > 0 && idx >= 0 && idx < _choices.Count)
			{
				return _choices[idx].ReturnValue;
			}

			return default(TReturnType);
		}

		public void EditChoice(Predicate<Entry> match, Func<Entry, Entry> edit)
		{
			if (_choices == null || _choices.Count <= 0)
			{
				return;
			}

			for (int n = 0; n < _choices.Count; ++n)
			{
				if (match(_choices[n]))
				{
					_choices[n] = edit(_choices[n]);
				}
			}
		}

		public void RemoveChoices(Predicate<Entry> match)
		{
			if (_choices == null || _choices.Count <= 0)
			{
				return;
			}

			_choices.RemoveAll(match);
			UpdateChoicesBoxSize();
		}

		// --------------------------------------------------------

		public bool HasSelected
		{
			get
			{
				return _choices != null &&
				       _choices.Count > 0 &&
				       _currentlySelectedIdx >= 0 &&
				       _currentlySelectedIdx < _choices.Count;
			}
		}

		public TReturnType GetCurrentlySelectedChoice()
		{
			if (_choices != null &&
			    _choices.Count > 0 &&
			    _currentlySelectedIdx >= 0 &&
			    _currentlySelectedIdx < _choices.Count)
			{
				return _choices[_currentlySelectedIdx].ReturnValue;
			}

			return default(TReturnType);
		}

		public string GetCurrentlySelectedLabel()
		{
			if (_choices != null &&
			    _choices.Count > 0 &&
			    _currentlySelectedIdx >= 0 &&
			    _currentlySelectedIdx < _choices.Count)
			{
				return _choices[_currentlySelectedIdx].Label.text;
			}

			return null;
		}

		// ====================================================================
		// The 3 render functions: DrawUnderlay, DrawButton, and DrawOverlayIfNeeded

		/// <summary>
		/// This is needed to ensure Event input won't be intercepted by widgets above/underneath the dropdown box.
		/// </summary>
		/// <param name="selectedValue"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		public TReturnType DrawUnderlay(TReturnType selectedValue, Vector2 offset = default(Vector2))
		{
			InitGuiStylesIfNeeded();
			return DrawList(selectedValue, false, offset);
		}

		public TReturnType DrawUnderlay(Vector2 offset = default(Vector2))
		{
			InitGuiStylesIfNeeded();
			return DrawList(GetCurrentlySelectedChoice(), false, offset);
		}

		/// <summary>
		/// Draw the button that will show/hide the dropdown box.
		/// </summary>
		/// <param name="options"></param>
		public void DrawButton(params GUILayoutOption[] options)
		{
			InitGuiStylesIfNeeded();

			if (_choices == null || _choices.Count == 0)
			{
				GUILayout.Button("<i>No choices</i>", _buttonGuiStyle, options);
				return;
			}

			bool isIconColorCustomized;
			GUIContent buttonLabel;
			if (_currentlySelectedIdx == -1)
			{
				buttonLabel = _noSelection;
				isIconColorCustomized = false;
			}
			else
			{
				if (_currentlySelectedIdx > _choices.Count - 1)
				{
					_currentlySelectedIdx = _choices.Count - 1;
				}

				if (_currentlySelectedIdx < 0)
				{
					_currentlySelectedIdx = 0;
				}

				buttonLabel = _choices[_currentlySelectedIdx].Label;
				isIconColorCustomized = _choices[_currentlySelectedIdx].IsIconColorCustomized;
			}

			var prevColor = GUI.contentColor;

			if (isIconColorCustomized)
			{
				GUI.contentColor = new Color(0, 0, 0, 0);
			}

			//var buttonLabel = string.Format("{0}, {1}, {2}", _choices[_currentlySelectedIdx].Label.text,
			//	_needsScrollbar ? "scrollbar" : "no scrollbar", _shouldAppearAbove ? "above" : "below");

			if (!OverlayUtility.IsAnyOverlayOpen)
			{
				if (GUILayout.Button(buttonLabel, _buttonGuiStyle, options))
				{
					if (IsThisDropDownBoxOpen)
					{
						CloseThisDropDownBox();
					}
					else
					{
						OpenThisDropDownBox();
					}
				}

				// ------------------------------------------------------------------------------
				// record the button's Rect to _buttonRect only if it's valid

				if (Event.current.type == EventType.Repaint)
				{
					Rect newButtonRect = GUILayoutUtility.GetLastRect();
					if (!Mathf.Approximately(newButtonRect.width, _buttonRect.width)) // width has changed
					{
						_buttonRect = newButtonRect;
						_choicesBoxSizeUpdated = false; // force the choice box to update
						SetChoicesBoxSizeIfNeeded();
					}
					else
					{
						_buttonRect = newButtonRect;
					}
				}
			}
			else
			{
				// Drawing non-functioning button since another dropdown box is already open
				GUILayout.Label(buttonLabel, _buttonGuiStyle, options);
				if (Event.current.type == EventType.Repaint)
				{
					_buttonRect = GUILayoutUtility.GetLastRect();
				}
			}

			if (isIconColorCustomized)
			{
				GUI.contentColor = prevColor;

				Rect iconRect = new Rect(_buttonRect);
				iconRect.x += 3;
				iconRect.width = buttonLabel.image.width;
				iconRect.height = buttonLabel.image.height;
				iconRect.y += (_buttonRect.height - iconRect.height) / 2;
				Widget.DrawIcon(iconRect, buttonLabel.image, _choices[_currentlySelectedIdx].IconColor);

				Rect labelRect = new Rect(_buttonRect);
				labelRect.xMin += iconRect.width + 2;
				GUI.Label(labelRect, buttonLabel.text, _entryGuiStyle);
			}
		}

		/// <summary>
		/// This is needed to ensure dropdown box is always shown on top of other widgets.
		/// </summary>
		/// <param name="offset"></param>
		public TReturnType DrawOverlayIfNeeded(Vector2 offset = default(Vector2))
		{
			InitGuiStylesIfNeeded();
			return DrawList(default(TReturnType), true, offset);
		}

		public TReturnType DrawOverlayIfNeeded(TReturnType selectedValue, Vector2 offset = default(Vector2))
		{
			InitGuiStylesIfNeeded();
			return DrawList(selectedValue, true, offset);
		}
	}
}