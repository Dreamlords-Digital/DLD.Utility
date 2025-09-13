// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using System.Text;
using DLD.Utility;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public partial class Dropdown
	{
		public void SetItemsFromEnum(ulong[] values, DropdownItem[] items, EnumDropdownItem[] obsoleteItems, bool enumUsesFlags,
			DropdownEnumFlagHandling newEnumFlagHandling, bool includeObsoleteEnums)
		{
			_enumValues = values;
			_enumItems = items;
			_enumObsoleteItems = obsoleteItems;
			_enumFlagHandling = newEnumFlagHandling;
			_includeObsoleteEnums = includeObsoleteEnums;
			_currentMode = Mode.Enum;

			_noneIndex = _enumValues.IndexOf(0u);

			if (enumUsesFlags)
			{
				// enum has flags
				if (_enumFlagHandling == DropdownEnumFlagHandling.Auto)
				{
					_currentMode = Mode.EnumFlag;
					if (_currentValueDisplayType == DropdownCurrentValueDisplayType.Icons)
					{
						InitializeCurrentDisplayIcons(_enumValues.Length);
					}
				}
			}
		}

		public void SetValueWithoutNotify(ulong currentValue)
		{
			_currentEnumValue = currentValue;

			switch (_currentMode)
			{
				case Mode.Enum:
					UpdateCurrentEnumValueDisplayed(currentValue);
					break;
				case Mode.EnumFlag:
					UpdateCurrentEnumFlagValueDisplayed(currentValue);
					break;
			}
		}

		// ==================================================================================

		/// <summary>
		///    Called when user clicks on the dropdown box.
		/// </summary>
		void Open()
		{
			if (_currentMode == Mode.EnumFlag)
			{
				InitializeCurrentDisplayIcons(_enumValues.Length);
			}

			_contextMenu.DoAltBgStyling(_doAltBgStyling);
			_contextMenu.SetAlwaysLeaveSpaceForSelectedIndicator(_currentMode == Mode.EnumFlag);
			_contextMenu.ClearMenu();
			for (int n = 0; n < _enumValues.Length; ++n)
			{
				ulong enumInt = _enumValues[n];
				var obsoleteType = _enumObsoleteItems?[n].Obsolete ?? EnumObsoleteType.None;

				if (obsoleteType != EnumObsoleteType.None && !_includeObsoleteEnums)
				{
					continue;
				}

				var itemTooltips = new List<TooltipMessage>();

				bool enumValueIsSelected = _currentMode switch
				{
					Mode.Enum => enumInt == _currentEnumValue,
					Mode.EnumFlag => _currentEnumValue.HasFlag(enumInt) &&
					                 (enumInt != 0 || _currentEnumValue == 0),
					_ => false
				};

				ContextMenuItemStyle menuItemStyle = ContextMenuItemStyle.Standard;
				if (enumValueIsSelected)
				{
					menuItemStyle |= ContextMenuItemStyle.Selected;
				}

				if (obsoleteType == EnumObsoleteType.ObsoleteWarning)
				{
					menuItemStyle |= ContextMenuItemStyle.Warning;
				}
				else if (obsoleteType == EnumObsoleteType.ObsoleteError)
				{
					menuItemStyle |= ContextMenuItemStyle.Error;
				}

				if (obsoleteType != EnumObsoleteType.None)
				{
					if (!string.IsNullOrEmpty(_enumItems[n].Tooltip))
					{
						// two tooltip messages immediately: the tooltip from the _enumItems, and the message from _enumObsoleteItems

						itemTooltips.Add(new TooltipMessage(BaseIcons.GENERIC_INFO, _enumItems[n].Tooltip));
						itemTooltips.Add(TooltipUtil.CreateObsoleteTooltipMessage(_enumObsoleteItems?[n] ?? EnumDropdownItem.Empty));

						_contextMenu.AddMenu(_enumItems[n].Label, itemTooltips, _enumItems[n].IconClassName, menuItemStyle,
							listener: this, userArg1: enumInt);
					}
					else
					{
						// tooltip is only from the ObsoleteAttribute
						itemTooltips.Add(TooltipUtil.CreateObsoleteTooltipMessage(_enumObsoleteItems?[n] ?? EnumDropdownItem.Empty));

						_contextMenu.AddMenu(_enumItems[n].Label, itemTooltips, _enumItems[n].IconClassName, menuItemStyle,
							listener: this, userArg1: enumInt);
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(_enumItems[n].Tooltip))
					{
						itemTooltips.Add(new TooltipMessage(BaseIcons.GENERIC_INFO, _enumItems[n].Tooltip));
					}

					_contextMenu.AddMenu(_enumItems[n].Label, itemTooltips, _enumItems[n].IconClassName, menuItemStyle,
						listener: this, userArg1: enumInt);
				}
			}

			_contextMenu.Show(_toggle, listener: this);
		}

		/// <summary>
		///    Called when user chooses an item inside the dropdown box.
		/// </summary>
		void OnDropdownEnumChosen(int chosenEnumIndex, Label chosenItemLabel, ulong newEnumValueChosen)
		{
			if (newEnumValueChosen == _currentEnumValue)
			{
				// same as currently chosen value, nothing to do
				return;
			}

			_contextMenu.ChangeSelected(chosenEnumIndex);

			using var changeEvent = ChangeEvent<ulong>.GetPooled(_currentEnumValue, newEnumValueChosen);
			changeEvent.target = this;

			_currentEnumValue = newEnumValueChosen;
			_toggle.label = chosenItemLabel.text;
			UpdateCurrentValueIcon(chosenEnumIndex);
			UpdateTooltip(chosenEnumIndex);

			_toggle.RemoveFromClassList(DROPDOWN_BOX_ERROR_STYLE_CLASS);
			_toggle.RemoveFromClassList(DROPDOWN_BOX_WARNING_STYLE_CLASS);
			if (chosenItemLabel.ClassListContains(ContextMenu.ERROR_ENTRY_LABEL_STYLE_CLASS))
			{
				_toggle.AddToClassList(DROPDOWN_BOX_ERROR_STYLE_CLASS);
			}
			else if (chosenItemLabel.ClassListContains(ContextMenu.WARNING_ENTRY_LABEL_STYLE_CLASS))
			{
				_toggle.AddToClassList(DROPDOWN_BOX_WARNING_STYLE_CLASS);
			}

			panel.visualTree.SendEvent(changeEvent);
		}

		void UpdateCurrentEnumValueDisplayed(ulong currentValue)
		{
			int currentValueIdx = _enumValues.IndexOf(currentValue);
			if (currentValueIdx == -1)
			{
				return;
			}

			_toggle.label = _enumItems[currentValueIdx].Label;
			UpdateCurrentValueIcon(currentValueIdx);
			UpdateTooltip(currentValueIdx);

			_toggle.RemoveFromClassList(DROPDOWN_BOX_ERROR_STYLE_CLASS);
			_toggle.RemoveFromClassList(DROPDOWN_BOX_WARNING_STYLE_CLASS);

			var obsoleteType = _enumObsoleteItems?[currentValueIdx].Obsolete ?? EnumObsoleteType.None;
			switch (obsoleteType)
			{
				case EnumObsoleteType.ObsoleteError:
					_toggle.AddToClassList(DROPDOWN_BOX_ERROR_STYLE_CLASS);
					break;
				case EnumObsoleteType.ObsoleteWarning:
					_toggle.AddToClassList(DROPDOWN_BOX_WARNING_STYLE_CLASS);
					break;
			}
		}

		void UpdateCurrentValueIcon(int index)
		{
			_icon.ClearClassList();
			_icon.AddToClassList(BaseIcons.ICON_STYLE_CLASS);
			if (!string.IsNullOrEmpty(_enumItems[index].IconClassName))
			{
				_icon.style.display = DisplayStyle.Flex;
				_icon.AddToClassList(_enumItems[index].IconClassName);
			}
			else
			{
				_icon.style.display = DisplayStyle.None;
			}
		}

		void UpdateTooltip(int index)
		{
			var obsoleteType = _enumObsoleteItems?[index].Obsolete ?? EnumObsoleteType.None;

			if (_enumObsoleteItems != null && obsoleteType != EnumObsoleteType.None)
			{
				if (!string.IsNullOrWhiteSpace(_enumItems[index].Tooltip))
				{
					// two tooltip messages immediately: the tooltip from the EnumUI, and the message from ObsoleteAttribute
					_enumValueTooltip.Text = _enumItems[index].Tooltip;
				}
				else
				{
					_enumValueTooltip.Text = null;
				}
				_enumObsoleteTooltip.IconClassName = TooltipUtil.CreateObsoleteTooltipIcon(obsoleteType);
				_enumObsoleteTooltip.Text = TooltipUtil.CreateObsoleteTooltipText(_enumObsoleteItems[index]);
			}
			else
			{
				_enumValueTooltip.Text = _enumItems[index].Tooltip;
				_enumObsoleteTooltip.Text = null;
			}
		}

		// ==================================================================================

		void OnDropdownEnumFlagChosen(int index, ulong newEnumValueChosen)
		{
			ulong previousValue = _currentEnumValue;

			if (newEnumValueChosen == 0)
			{
				// User purposefully chose the "None" item.
				// Clear all values.
				_currentEnumValue = 0;
				_contextMenu.ChangeSelected(index);
			}
			else
			{
				_currentEnumValue.ToggleFlag(newEnumValueChosen);

				if (previousValue == 0)
				{
					// Value used to be 0. That means this is the first assignment.
					// Get rid of the selected indicator on the "None" item.
					_contextMenu.ChangeSelected(index);
				}
				else if (_currentEnumValue == 0)
				{
					// User unchecked the last flag that was giving it a value, and now it's become 0.
					// Need to show the selected indicator on the "None" item.
					_contextMenu.ChangeSelected(_noneIndex);
				}
				else
				{
					bool added = _currentEnumValue.HasFlag(newEnumValueChosen);
					_contextMenu.SetSelected(index, added);
				}
			}

			using var changeEvent = ChangeEvent<ulong>.GetPooled(previousValue, _currentEnumValue);
			changeEvent.target = this;

			UpdateCurrentEnumFlagValueDisplayed(_currentEnumValue);

			panel.visualTree.SendEvent(changeEvent);
		}

		void UpdateCurrentEnumFlagValueDisplayed(ulong currentEnumFlagValue)
		{
			if (_currentValueDisplayType == DropdownCurrentValueDisplayType.Icons)
			{
				// toggle needs to display a row of icons

				if (currentEnumFlagValue == 0)
				{
					// current enum value is zero

					if (_noneIndex != -1)
					{
						_icon.style.display = DisplayStyle.Flex;
						_icon.ClearClassList();
						_icon.AddToClassList(BaseIcons.ICON_STYLE_CLASS);
						_icon.AddToClassList(_enumItems[_noneIndex].IconClassName);

						_enumValueTooltip.Text = _enumItems[_noneIndex].Tooltip;

						// Note: Assigning to _toggle.label actually causes the label to be re-inserted
						// back as the first child of _toggle.
						_toggle.label = _enumItems[_noneIndex].Label ?? _enumItems[_noneIndex].ShortLabel ?? _labelToDisplayWhenNoneSelected;

						// So we move the single-value icon back to its intended position
						// (to the left of the label).
						_toggle.Remove(_icon);
						_toggle.Insert(0, _icon);
					}
					else
					{
						_icon.style.display = DisplayStyle.None;
						_toggle.label = _labelToDisplayWhenNoneSelected;
						_enumValueTooltip.Text = null;
					}
					_enumObsoleteTooltip.Text = null;

					// clear other icons
					for (int n = 0; n < _currentValueIcons.Count; ++n)
					{
						_currentValueIcons[n].ClearClassList();
					}
				}
				else
				{
					// current enum value isn't zero

					_icon.style.display = DisplayStyle.None;
					_toggle.label = null;

					for (int n = 0; n < _enumValues.Length; ++n)
					{
						ulong enumValue = _enumValues[n];

						_currentValueIcons[n].ClearClassList();
						if (currentEnumFlagValue.HasFlag(enumValue) && enumValue != 0)
						{
							_currentValueIcons[n].AddToClassList(BaseIcons.ICON_STYLE_CLASS);
							_currentValueIcons[n].AddToClassList(_enumItems[n].IconClassName);
						}
					}

					// Get the long label and use it as the tooltip
					_enumValueTooltip.Text = GetEnumFlagAsLabel(_enumValues, _enumItems, currentEnumFlagValue, _noneIndex, DropdownCurrentValueDisplayType.Labels);
				}
			}
			else
			{
				_toggle.label = GetEnumFlagAsLabel(_enumValues, _enumItems, currentEnumFlagValue, _noneIndex, _currentValueDisplayType);

				if (string.IsNullOrEmpty(_toggle.label))
				{
					_toggle.label = _labelToDisplayWhenNoneSelected;
				}
			}
		}

		static string GetEnumFlagAsLabel(ulong[] enumValues, DropdownItem[] enumItems, ulong currentValue, int noneIndex, DropdownCurrentValueDisplayType displayType)
		{
			if (currentValue == 0)
			{
				if (noneIndex != -1)
				{
					switch (displayType)
					{
						case DropdownCurrentValueDisplayType.ShortLabels:
							// Try ShortLabel, if that's null, try Label.
							return enumItems[noneIndex].ShortLabel ?? enumItems[noneIndex].Label;
						default: // default using DropdownCurrentValueDisplayType.Labels
							// Try Label, if that's null, try ShortLabel.
							return enumItems[noneIndex].Label ?? enumItems[noneIndex].ShortLabel;
					}
				}

				return null;
			}

			var s = new StringBuilder();
			bool once = false;

			for (int n = 0; n < enumValues.Length; ++n)
			{
				ulong enumValue = enumValues[n];

				if (currentValue.HasFlag(enumValue) && enumValue != 0)
				{
					if (once)
					{
						s.Append(", ");
					}

					{
						switch (displayType)
						{
							case DropdownCurrentValueDisplayType.ShortLabels:
								// Try ShortLabel, if that's null, try Label.
								s.Append(enumItems[n].ShortLabel ?? enumItems[n].Label);
								break;
							default: // default using DropdownCurrentValueDisplayType.Labels
								// Try Label, if that's null, try ShortLabel.
								s.Append(enumItems[n].Label ?? enumItems[n].ShortLabel);
								break;
						}
					}

					once = true;
				}
			}

			return s.ToString();
		}

		// ==================================================================================
	}
}
