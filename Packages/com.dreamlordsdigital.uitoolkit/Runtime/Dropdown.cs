// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

public struct DropdownItem
{
	public string Label;
	public string ShortLabel;
	public string Tooltip;
	public string IconClassName;
}

public struct EnumDropdownItem
{
	public EnumObsoleteType Obsolete;
	public string ObsoleteMessage;

	public static readonly EnumDropdownItem Empty = new EnumDropdownItem();
}

public enum EnumObsoleteType : byte
{
	None,
	ObsoleteWarning,
	ObsoleteError,
}

public enum DropdownCurrentValueDisplayType : byte
{
	Labels,
	ShortLabels,
	Icons
}

/// <summary>
///    Whether an Enum that has the <see cref="System.FlagsAttribute"/> is treated like a bitmask
///    where we allow the user to select multiple values, or we ignore it and
///    enforce that only one value is selected.
/// </summary>
public enum DropdownEnumFlagHandling : byte
{
	/// <summary>
	///    Make the dropdown behave like a ToggleButtonGroup (i.e. allow multiple values to be selected)
	///    if the Enum has the <see cref="System.FlagsAttribute"/>.
	/// </summary>
	Auto,

	/// <summary>
	///    Even if the Enum has the <see cref="System.FlagsAttribute"/>, ignore it.
	///    Dropdown will then enforce that only one value is selected.
	/// </summary>
	ForceIgnore,
}

[UxmlElement]
public partial class Dropdown : VisualElement, IContextMenuListener
{
	const string TEMPLATE_RESOURCES_PATH = "DLD UIToolkit/Dropdown";
	const string ARROW_STYLE_CLASS = "dld-dropdown__arrow";
	const string DROPDOWN_BOX_WARNING_STYLE_CLASS = "dld-box--warning";
	const string DROPDOWN_BOX_ERROR_STYLE_CLASS = "dld-box--error";

	// ==================================================================================

	readonly Label _label;

	/// <summary>
	///    Behaves as our "dropdown box".
	/// </summary>
	readonly Toggle _toggle;

	readonly TooltipMessage _enumValueTooltip = new(BaseIcons.GENERIC_INFO);
	readonly TooltipMessage _enumObsoleteTooltip = new(BaseIcons.GENERIC_ERROR);

	/// <summary>
	///    Icon displayed when the dropdown box is displaying a single value.
	///    Only used if the value actually has an icon assigned to it.
	/// </summary>
	readonly VisualElement _icon;

	IContextMenu _contextMenu;
	ITooltip _tooltip;

	bool _doAltBgStyling;

	string _labelToDisplayWhenNoneSelected = "None";

	enum Mode : byte
	{
		/// <summary>
		///    Dropdown will show all values of the specified Enum in <see cref="SetItemsFromEnum"/>.
		/// </summary>
		Enum,

		/// <summary>
		///    Dropdown will show all values of the specified Enum in <see cref="SetItemsFromEnum"/>.
		/// </summary>
		/// <remarks>
		///    In this mode, the user is allowed to select multiple dropdown items.
		/// </remarks>
		EnumFlag,

		/// <summary>
		///    Dropdown will show a list of <see cref="DropdownItem"/>
		///    that is meant to represent the values of a byte mask
		///    specified in <see cref="SetValueWithoutNotify(byte)"/>.
		/// </summary>
		/// <remarks>
		///    In this mode, the user is allowed to select multiple dropdown items.
		/// </remarks>
		ByteMask
	}

	Mode _currentMode;

	// ==================================================================================

	/// <summary>
	///    When <see cref="_currentMode"/> is <see cref="Mode.Enum"/> or <see cref="Mode.EnumFlag"/>,
	///    this is the current Enum value.
	///    This is the value that we send when we dispatch a ChangeEvent.
	/// </summary>
	ulong _currentEnumValue;

	/// <inheritdoc cref="DropdownEnumFlagHandling"/>
	DropdownEnumFlagHandling _enumFlagHandling = DropdownEnumFlagHandling.Auto;

	/// <summary>
	///    Whether enum values that have the <see cref="System.ObsoleteAttribute"/>
	///    are included in the dropdown choices or not.
	/// </summary>
	bool _includeObsoleteEnums;

	ulong[] _enumValues;
	DropdownItem[] _enumItems;
	EnumDropdownItem[] _enumObsoleteItems;

	/// <summary>
	///    Which element in the <see cref="_enumValues"/> has the 0 value, if any.
	///    This is usually the first in the list of enum items, but that isn't a guarantee.
	/// </summary>
	int _noneIndex;

	// ==================================================================================

	/// <summary>
	///    Determines whether dropdown will display the
	///    currently selected values as either labels, shortened labels, or icons.
	///    Only applicable when dropdown is showing <see cref="Mode.EnumFlag"/> or <see cref="Mode.ByteMask"/>.
	/// </summary>
	DropdownCurrentValueDisplayType _currentValueDisplayType;

	/// <summary>
	///    Only used when the Dropdown <see cref="_currentMode"/> is in <see cref="Mode.ByteMask"/>.
	/// </summary>
	List<DropdownItem> _dropdownItems;

	/// <summary>
	///    Only used when <see cref="_currentMode"/> is <see cref="Mode.ByteMask"/>.<br/><br/>
	///    true: DropdownItems that are not assigned a label will not be shown in the choices.<br/><br/>
	///    false: DropdownItems that are not assigned a label will still be shown in the choices.
	///    Their ordinal positions will be used as their label.
	/// </summary>
	bool _skipBlankValues;

	/// <summary>
	///    When <see cref="_currentMode"/> is <see cref="Mode.ByteMask"/>,
	///    this is the current byte value.
	///    This is the value that we send when we dispatch a ChangeEvent.
	/// </summary>
	byte _currentByteValue;

	/// <summary>
	///    When <see cref="_currentValueDisplayType"/> is
	///    <see cref="DropdownCurrentValueDisplayType.Icons"/>,
	///    these hold the icons being displayed.
	/// </summary>
	List<VisualElement> _currentValueIcons;

	// ==================================================================================

	public Dropdown()
	{
		var asset = Resources.Load<VisualTreeAsset>(TEMPLATE_RESOURCES_PATH);
		asset.CloneTree(this);
		this.RemoveTemplateContainer("Dropdown");

		_toggle = this.Q<Toggle>();
		_toggle.labelElement.focusable = true;

		_icon = new VisualElement();
		_icon.name = "SingleValueIcon";

		var arrow = new VisualElement();
		arrow.AddToClassList(ARROW_STYLE_CLASS);
		_toggle.Add(arrow);

		_toggle.Insert(0, _icon);
		_icon.style.display = DisplayStyle.None;

		_label = this.Q<Label>(null, "dld-dropdown__label");
		_label.RegisterCallback<MouseDownEvent, Dropdown>((e, d) =>
		{
			d._toggle.value = true;
			e.StopPropagation();
		}, this);

		_toggle.RegisterCallback<ChangeEvent<bool>, Dropdown>((e, d) =>
		{
			if (!e.previousValue && e.newValue)
			{
				d.OnOpen();
			}
		}, this);
	}

	public string Label
	{
		get => _label.text;
		set => _label.text = value;
	}

	public Label LabelElement => _label;

	public void SetContextMenu(IContextMenu contextMenu)
	{
		_contextMenu = contextMenu;
	}

	public void SetTooltip(ITooltip newTooltip, TooltipMessage additionalTooltip = null)
	{
		// Prepare the dropdown box to allow it to display tooltips.
		_toggle.RegisterCallback(TooltipUtil.ShowFromUserData, newTooltip);
		_toggle.RegisterCallback(TooltipUtil.Hide, newTooltip);

		_toggle.userData = new[] { additionalTooltip, _enumValueTooltip, _enumObsoleteTooltip };
	}

	public bool HasAnyErrorTooltip => !string.IsNullOrWhiteSpace(_enumObsoleteTooltip.Text);

	public void DoAltBgStyling(bool doAltBgStyling)
	{
		_doAltBgStyling = doAltBgStyling;
	}

	public void SetLabelToDisplayWhenNoneSelected(string labelToDisplayWhenNoneSelected)
	{
		_labelToDisplayWhenNoneSelected = labelToDisplayWhenNoneSelected;
	}

	public void SetSkipBlankValues(bool skipBlankValues)
	{
		_skipBlankValues = skipBlankValues;
	}

	public void SetCurrentValueDisplayType(DropdownCurrentValueDisplayType newDisplayType)
	{
		_currentValueDisplayType = newDisplayType;

		if (_currentValueDisplayType == DropdownCurrentValueDisplayType.Icons)
		{
			if (_currentValueIcons != null)
			{
				_currentValueIcons.Clear();
			}
			else
			{
				_currentValueIcons = new List<VisualElement>();
			}
		}
	}

	public void ShowErrorIndication()
	{
		_toggle.AddToClassList(DROPDOWN_BOX_ERROR_STYLE_CLASS);

		// error indication is higher priority than warning
		_toggle.RemoveFromClassList(DROPDOWN_BOX_WARNING_STYLE_CLASS);
	}

	public void ShowWarningIndication()
	{
		_toggle.AddToClassList(DROPDOWN_BOX_WARNING_STYLE_CLASS);
	}

	public void HideErrorIndication()
	{
		_toggle.RemoveFromClassList(DROPDOWN_BOX_ERROR_STYLE_CLASS);
	}

	// ==================================================================================

	/// <summary>
	///    Called when user clicks on the dropdown box.
	/// </summary>
	void OnOpen()
	{
		switch (_currentMode)
		{
			case Mode.Enum:
			case Mode.EnumFlag:
				Open();
				break;
			case Mode.ByteMask:
				Debug.Assert(_dropdownItems != null && _dropdownItems.Count > 0);
				Open(_dropdownItems, _currentByteValue);
				break;
		}
	}

	// ==================================================================================

	public void OnContextMenuChosen(int index, Label label, object menuTooltip, object newValue, object userArg2)
	{
		switch (_currentMode)
		{
			case Mode.Enum:
				OnDropdownEnumChosen(index, label, (ulong)newValue);
				break;
			case Mode.EnumFlag:
				OnDropdownEnumFlagChosen(index, (ulong)newValue);
				break;
			case Mode.ByteMask:
				Debug.Assert(_dropdownItems != null && _dropdownItems.Count > 0);
				OnDropdownByteMaskChosen((int)newValue);
				break;
		}
	}

	public void OnContextMenuClosed(bool userCancelled)
	{
		_toggle.value = false;
		_toggle.Focus();
	}
}

}
