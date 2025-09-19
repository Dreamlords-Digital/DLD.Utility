// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using System.Text;
using DLD.Utility;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

public partial class Dropdown
{
	public void InitializeCurrentDisplayIcons(int iconCount)
	{
		if (_currentValueDisplayType != DropdownCurrentValueDisplayType.Icons)
		{
			return;
		}

		while (_currentValueIcons.Count < iconCount)
		{
			var newIcon = new VisualElement();
			newIcon.AddToClassList(BaseIcons.IconStyleClass);
			_currentValueIcons.Add(newIcon);
			newIcon.name = $"Icon{_currentValueIcons.Count}";
		}

		// Remove all of them first.
		for (int i = 0; i < _currentValueIcons.Count; ++i)
		{
			if (_toggle.Contains(_currentValueIcons[i]))
			{
				_toggle.Remove(_currentValueIcons[i]);
			}
		}

		// Add only up to the ones we need.
		for (int i = iconCount - 1; i >= 0; --i)
		{
			// The icons need to be inserted to the left
			// because we need to keep the dropdown arrow that's inside the toggle to be the last.
			// That's why we keep inserting at the beginning instead of using Add().
			_toggle.Insert(0, _currentValueIcons[i]);
		}
	}

	public void SetValueWithoutNotify(byte currentByteValue)
	{
		UpdateCurrentMaskValueDisplayed(currentByteValue);

		_currentMode = Mode.ByteMask;
		_currentByteValue = currentByteValue;
		_currentEnumValue = 0;
	}

	// ==================================================================================

	public void ClearItems()
	{
		if (_dropdownItems != null)
		{
			_dropdownItems.Clear();
		}
		else
		{
			_dropdownItems = new List<DropdownItem>();
		}
	}

	public void AddDropdownItem(string label, string shortLabel = null, string tooltipText = null, string iconClassName = null)
	{
		_dropdownItems.Add(new DropdownItem()
		{
			Label = label,
			ShortLabel = shortLabel,
			Tooltip = tooltipText,
			IconClassName = iconClassName
		});
	}

	/// <summary>
	///    Called when user clicks on the dropdown box.
	/// </summary>
	void Open(List<DropdownItem> dropdownItems, byte value)
	{
		_contextMenu.DoAltBgStyling(_doAltBgStyling);
		_contextMenu.SetAlwaysLeaveSpaceForSelectedIndicator(true);
		_contextMenu.ClearMenu();
		for (int n = 0; n < dropdownItems.Count; ++n)
		{
			var item = dropdownItems[n];
			string label = item.Label ?? item.ShortLabel;
			if (string.IsNullOrEmpty(label))
			{
				if (_skipBlankValues)
				{
					continue;
				}

				label = (n + 1).ToString();
			}

			var menuItemStyle = value.GetFlag(n) ? ContextMenuItemStyle.Selected : ContextMenuItemStyle.Standard;

			_contextMenu.AddMenu(label, item.Tooltip, item.IconClassName, menuItemStyle,
				listener: this, userArg1: n);
		}

		_contextMenu.Show(_toggle, listener: this);
	}

	/// <summary>
	///    Called when user chooses an item inside the dropdown box.
	/// </summary>
	void OnDropdownByteMaskChosen(int chosenIndex)
	{
		byte previousValue = _currentByteValue;

		// calculate new byte value
		_currentByteValue.ToggleFlag(chosenIndex);

		using var changeEvent = ChangeEvent<byte>.GetPooled(previousValue, _currentByteValue);
		changeEvent.target = this;

		UpdateCurrentMaskValueDisplayed(_currentByteValue);

		panel.visualTree.SendEvent(changeEvent);
	}

	// ==================================================================================

	void UpdateCurrentMaskValueDisplayed(byte currentByteValue)
	{
		if (_currentValueDisplayType == DropdownCurrentValueDisplayType.Icons)
		{
			// toggle needs to display a row of icons

			_toggle.label = null;

			for (int i = 0; i < 8; ++i)
			{
				_currentValueIcons[i].ClearClassList();
				if (currentByteValue.GetFlag(i))
				{
					_currentValueIcons[i].AddToClassList(BaseIcons.IconStyleClass);
					_currentValueIcons[i].AddToClassList(_dropdownItems[i].IconClassName);
				}
			}

			// Get the long label and use it as the tooltip
			ClearEnumValueTooltips();
			string valueText;
			if (currentByteValue > 0)
			{
				valueText = GetByteMaskAsLabel(_dropdownItems, currentByteValue, DropdownCurrentValueDisplayType.Labels);
			}
			else
			{
				valueText = _labelToDisplayWhenNoneSelected;
			}

			AddEnumValueTooltip(valueText);
		}
		else
		{
			_toggle.label = GetByteMaskAsLabel(_dropdownItems, currentByteValue, _currentValueDisplayType);

			if (string.IsNullOrEmpty(_toggle.label))
			{
				_toggle.label = _labelToDisplayWhenNoneSelected;
			}
		}
	}

	static string GetByteMaskAsLabel(List<DropdownItem> items, byte value, DropdownCurrentValueDisplayType displayType)
	{
		if (value == 0)
		{
			return null;
		}

		StringBuilder s = new StringBuilder();
		bool once = false;
		for (int i = 0; i < 8; ++i)
		{
			if (value.GetFlag(i))
			{
				if (once)
				{
					s.Append(", ");
				}

				switch (displayType)
				{
					case DropdownCurrentValueDisplayType.ShortLabels:
						// Try ShortLabel, if that's null, try Label.
						// If that's still null, just show the bit position number.
						s.Append(items[i].ShortLabel ?? items[i].Label ?? (i + 1).ToString());
						break;
					default: // default using DropdownCurrentValueDisplayType.Labels
						// Try Label, if that's null, try ShortLabel.
						// If that's still null, just show the bit position number.
						s.Append(items[i].Label ?? items[i].ShortLabel ?? (i + 1).ToString());
						break;
				}

				once = true;
			}
		}

		return s.ToString();
	}
}

}
