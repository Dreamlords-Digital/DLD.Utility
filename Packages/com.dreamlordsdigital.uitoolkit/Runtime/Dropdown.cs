using System;
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

	public enum DropdownCurrentValueDisplayType
	{
		Labels,
		ShortLabels,
		Icons
	}

	[UxmlElement]
	public partial class Dropdown : VisualElement, IContextMenuListener
	{
		const string TEMPLATE_RESOURCES_PATH = "DLD UIToolkit/Dropdown";
		const string ARROW_STYLE_CLASS = "dld-dropdown__arrow";

		// ==================================================================================

		readonly Label _label;
		readonly Toggle _toggle;
		readonly VisualElement _icon;

		IContextMenu _contextMenu;
		bool _doAltBgStyling;

		enum Mode
		{
			/// <summary>
			/// Dropdown will show all values of the specified Enum in <see cref="SetEnumTypeOnOpen"/>.
			/// </summary>
			Enum,

			/// <summary>
			/// Dropdown will show a list of <see cref="DropdownItem"/>
			/// that is meant to represent the values of a byte mask
			/// specified in <see cref="SetValueWithoutNotify(byte)"/>.
			/// </summary>
			/// <remarks>
			/// In this mode, the user is allowed to select multiple dropdown items.
			/// </remarks>
			ByteMask
		}

		Mode _currentMode;

		// ==================================================================================

		System.Type _enumTypeOnOpen;
		System.Type _lastEnumTypeUsedOnOpen;
		Enum _currentEnumValue;

		// ==================================================================================

		DropdownCurrentValueDisplayType _currentValueDisplayType;
		List<DropdownItem> _dropdownItems;

		/// <summary>
		/// true: DropdownItems that are not assigned a label will not be shown in the choices.<br/>
		/// false: DropdownItems that are not assigned a label will still be shown in the choices.
		/// </summary>
		bool _skipBlankValues;

		byte _currentByteValue;

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

		public void DoAltBgStyling(bool doAltBgStyling)
		{
			_doAltBgStyling = doAltBgStyling;
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

		// ==================================================================================

		/// <summary>
		/// Called when user clicks on the dropdown box.
		/// </summary>
		void OnOpen()
		{
			switch (_currentMode)
			{
				case Mode.Enum:
					if (_enumTypeOnOpen != null)
					{
						Open(_enumTypeOnOpen);
					}
					break;
				case Mode.ByteMask:
					if (_dropdownItems != null && _dropdownItems.Count > 0)
					{
						Open(_dropdownItems, _currentByteValue);
					}
					break;
			}
		}

		// ==================================================================================

		public void OnContextMenuChosen(int index, string label, object newValue, object userArg2)
		{
			switch (_currentMode)
			{
				case Mode.Enum:
					if (_enumTypeOnOpen != null)
					{
						OnDropdownEnumChosen(index, label, newValue);
					}
					break;
				case Mode.ByteMask:
					if (_dropdownItems != null && _dropdownItems.Count > 0)
					{
						OnDropdownByteMaskChosen((int)newValue);
					}
					break;
			}
		}

		public void OnContextMenuCanceled()
		{
			_toggle.value = false;
			_toggle.Focus();
		}
	}
}
