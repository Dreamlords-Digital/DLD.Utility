using System;
using System.Reflection;
using System.Text;
using DLD.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public partial class Dropdown
	{
		public void SetEnumTypeOnOpen(Type enumType, DropdownEnumFlagHandling newEnumFlagHandling, bool includeObsoleteEnums)
		{
			_enumTypeOnOpen = enumType;
			_enumFlagHandling = newEnumFlagHandling;
			_includeObsoleteEnums = includeObsoleteEnums;
			_currentMode = Mode.Enum;

			if (enumType.GetCustomAttribute<FlagsAttribute>(false) != null)
			{
				// enum has flags
				if (_enumFlagHandling == DropdownEnumFlagHandling.Auto)
				{
					_currentMode = Mode.EnumFlag;
					if (_currentValueDisplayType == DropdownCurrentValueDisplayType.Icons)
					{
						Array enumValues = Enum.GetValues(enumType);
						InitializeCurrentDisplayIcons(enumValues.Length);
					}
				}
			}
		}

		public void SetValueWithoutNotify(Enum currentValue)
		{
			_toggle.label = currentValue.ToStringLabel();
			_currentEnumValue = currentValue;

			switch (_currentMode)
			{
				case Mode.Enum:
					UpdateCurrentValueIcon(currentValue);
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
		void Open(Type enumType)
		{
			if (_lastEnumTypeUsedOnOpen == enumType && _contextMenu.WasLastShownOn(_toggle))
			{
				// Context Menu has the same entries as what we want,
				// so no need to add them again. Just reuse it as-is.
				_contextMenu.Show(_toggle, this);
				return;
			}

			Array enumValues = Enum.GetValues(enumType);

			if (_currentMode == Mode.EnumFlag)
			{
				InitializeCurrentDisplayIcons(enumValues.Length);
			}

			_noneEnumValue = null;

			_contextMenu.DoAltBgStyling(_doAltBgStyling);
			_contextMenu.SetAlwaysLeaveSpaceForSelectedIndicator(_currentMode == Mode.EnumFlag);
			_contextMenu.ClearMenu();
			for (int n = 0; n < enumValues.Length; ++n)
			{
				var enumValue = (Enum)enumValues.GetValue(n);

				var obsoleteAttribute = enumValue.GetEnumValueAttribute<ObsoleteAttribute>();
				if (obsoleteAttribute != null && !_includeObsoleteEnums)
				{
					continue;
				}

				int enumValueInt = Convert.ToInt32(enumValue);
				if (enumValueInt == 0 && _noneEnumValue == null)
				{
					_noneEnumValue = enumValue;
				}

				bool enumValueIsSelected = _currentMode switch
				{
					Mode.Enum => enumValue.Equals(_currentEnumValue),
					Mode.EnumFlag => _currentEnumValue.HasFlag(enumValue) &&
					                 (enumValueInt != 0 || Convert.ToInt32(_currentEnumValue) == 0),
					_ => false
				};

				string label;
				string iconClassName;
				string tooltipDesc;

				var enumUI = enumValue.GetEnumValueAttribute<EnumUI>();
				if (obsoleteAttribute != null && enumUI != null)
				{
					label = enumUI.Label ?? enumValue.ToStringLabel();
					iconClassName = enumUI.IconStyleClass;

					if (!string.IsNullOrWhiteSpace(enumUI.Tooltip))
					{
						// two tooltip messages immediately: the tooltip from the EnumUI, and the message from ObsoleteAttribute
						var tooltipMessages = new TooltipMessage[]
						{
							new(BaseIcons.GENERIC_INFO, enumUI.Tooltip),
							new(obsoleteAttribute.IsError ? BaseIcons.GENERIC_ERROR : BaseIcons.GENERIC_WARNING, obsoleteAttribute.Message ?? "Marked as obsolete"),
						};

						_contextMenu.AddMenu(label, iconClassName, enumValueIsSelected, tooltipMessages,
							listener: this, userArg1: enumValue);
					}
					else
					{
						// tooltip is only from the ObsoleteAttribute
						tooltipDesc = $"{(obsoleteAttribute.IsError ? BaseIcons.GENERIC_ERROR : BaseIcons.GENERIC_WARNING)};{obsoleteAttribute.Message}";

						_contextMenu.AddMenu(label, iconClassName, enumValueIsSelected, tooltipDesc,
							listener: this, userArg1: enumValue);
					}
				}
				else if (obsoleteAttribute != null)
				{
					label = enumValue.ToStringLabel();
					tooltipDesc = $"{(obsoleteAttribute.IsError ? BaseIcons.GENERIC_ERROR : BaseIcons.GENERIC_WARNING)};{obsoleteAttribute.Message}";

					_contextMenu.AddMenu(label, null, enumValueIsSelected, tooltipDesc,
						listener: this, userArg1: enumValue);
				}
				else if (enumUI != null)
				{
					label = enumUI.Label ?? enumValue.ToStringLabel();
					iconClassName = enumUI.IconStyleClass;
					tooltipDesc = enumUI.Tooltip;

					_contextMenu.AddMenu(label, iconClassName, enumValueIsSelected, tooltipDesc,
						listener: this, userArg1: enumValue);
				}
				else
				{
					label = enumValue.ToStringLabel();

					_contextMenu.AddMenu(label, null, enumValueIsSelected, tooltip: null,
						listener: this, userArg1: enumValue);
				}
			}

			_contextMenu.Show(_toggle, this);

			_lastEnumTypeUsedOnOpen = enumType;
		}

		/// <summary>
		///    Called when user chooses an item inside the dropdown box.
		/// </summary>
		void OnDropdownEnumChosen(int index, string label, Enum newEnumValueChosen)
		{
			if (newEnumValueChosen.Equals(_currentEnumValue))
			{
				// same as currently chosen value, nothing to do
				return;
			}

			_contextMenu.ChangeSelected(index);

			using var changeEvent = ChangeEvent<Enum>.GetPooled(_currentEnumValue, newEnumValueChosen);
			changeEvent.target = this;

			_currentEnumValue = newEnumValueChosen;
			_toggle.label = label;
			UpdateCurrentValueIcon(_currentEnumValue);

			panel.visualTree.SendEvent(changeEvent);
		}

		void UpdateCurrentValueIcon(Enum currentValue)
		{
			_icon.ClearClassList();
			_icon.AddToClassList(BaseIcons.ICON_STYLE_CLASS);
			var enumUI = currentValue.GetEnumValueAttribute<EnumUI>();
			if (enumUI != null && !string.IsNullOrEmpty(enumUI.IconStyleClass))
			{
				_icon.style.display = DisplayStyle.Flex;
				_icon.AddToClassList(enumUI.IconStyleClass);
			}
			else
			{
				_icon.style.display = DisplayStyle.None;
			}
		}

		// ==================================================================================

		void OnDropdownEnumFlagChosen(int index, string label, Enum newEnumValueChosen)
		{
			Enum previousValue = _currentEnumValue;

			int newEnumChosenInt = Convert.ToInt32(newEnumValueChosen);
			if (newEnumChosenInt == 0)
			{
				// User purposefully chose the "None" item.
				// Clear all values.
				_currentEnumValue = (Enum)Enum.ToObject(_enumTypeOnOpen, 0);
				_contextMenu.ChangeSelected(index);
			}
			else
			{
				ByteUtil.ToggleFlag(ref _currentEnumValue, newEnumValueChosen);

				if (Convert.ToInt32(previousValue) == 0)
				{
					// Value used to be 0. That means this is the first assignment.
					// Get rid of the selected indicator on the "None" item.
					_contextMenu.ChangeSelected(index);
				}
				else if (Convert.ToInt32(_currentEnumValue) == 0)
				{
					// User unchecked the last flag that was giving it a value, and now it's become 0.
					// Need to show the selected indicator on the "None" item.
					_contextMenu.ChangeSelected(_noneEnumValue);
				}
				else
				{
					bool added = _currentEnumValue.HasFlag(newEnumValueChosen);
					_contextMenu.SetSelected(index, added);
				}
			}

			using var changeEvent = ChangeEvent<Enum>.GetPooled(previousValue, _currentEnumValue);
			changeEvent.target = this;

			UpdateCurrentEnumFlagValueDisplayed(_currentEnumValue);

			panel.visualTree.SendEvent(changeEvent);
		}

		void UpdateCurrentEnumFlagValueDisplayed(Enum currentEnumFlagValue)
		{
			if (_currentValueDisplayType == DropdownCurrentValueDisplayType.Icons)
			{
				// toggle needs to display a row of icons

				int currentEnumInt = Convert.ToInt32(currentEnumFlagValue);
				if (currentEnumInt == 0)
				{
					// current enum value is zero

					var enumUI = _noneEnumValue.GetEnumValueAttribute<EnumUI>();
					if (enumUI != null)
					{
						_icon.style.display = DisplayStyle.Flex;
						_icon.ClearClassList();
						_icon.AddToClassList(BaseIcons.ICON_STYLE_CLASS);
						_icon.AddToClassList(enumUI.IconStyleClass);
						_toggle.userData = enumUI.Tooltip;

						// Note: Assigning to _toggle.label actually causes the label to be re-inserted
						// back as the first child of _toggle.
						_toggle.label = enumUI.Label ?? enumUI.ShortLabel ?? _labelToDisplayWhenNoneSelected;

						// So we move the single-value icon back to its intended position
						// (to the left of the label).
						_toggle.Remove(_icon);
						_toggle.Insert(0, _icon);
					}
					else
					{
						_icon.style.display = DisplayStyle.None;
						_toggle.label = _labelToDisplayWhenNoneSelected;
						_toggle.userData = null;
					}

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

					Array enumValues = Enum.GetValues(_enumTypeOnOpen);
					for (int n = 0; n < enumValues.Length; ++n)
					{
						var enumValue = (Enum)enumValues.GetValue(n);
						_currentValueIcons[n].ClearClassList();
						if (currentEnumFlagValue.HasFlag(enumValue) && Convert.ToInt32(enumValue) != 0)
						{
							var enumUI = enumValue.GetEnumValueAttribute<EnumUI>();
							if (enumUI != null)
							{
								_currentValueIcons[n].AddToClassList(BaseIcons.ICON_STYLE_CLASS);
								_currentValueIcons[n].AddToClassList(enumUI.IconStyleClass);
							}
						}
					}

					// Get the long label and use it as the tooltip
					_toggle.userData = BaseIcons.GENERIC_INFO + ";" + GetEnumFlagAsLabel(_enumTypeOnOpen, currentEnumFlagValue, _noneEnumValue, DropdownCurrentValueDisplayType.Labels);
				}
			}
			else
			{
				_toggle.label = GetEnumFlagAsLabel(_enumTypeOnOpen, currentEnumFlagValue, _noneEnumValue, _currentValueDisplayType);

				if (string.IsNullOrEmpty(_toggle.label))
				{
					_toggle.label = _labelToDisplayWhenNoneSelected;
				}
			}
		}

		static string GetEnumFlagAsLabel(Type enumType, Enum currentValue, Enum noneValue, DropdownCurrentValueDisplayType displayType)
		{
			int currentValueAsInt = Convert.ToInt32(currentValue);

			if (currentValueAsInt == 0)
			{
				var enumUI = noneValue.GetEnumValueAttribute<EnumUI>();
				if (enumUI != null)
				{
					switch (displayType)
					{
						case DropdownCurrentValueDisplayType.ShortLabels:
							// Try ShortLabel, if that's null, try Label.
							return enumUI.ShortLabel ?? enumUI.Label;
						default: // default using DropdownCurrentValueDisplayType.Labels
							// Try Label, if that's null, try ShortLabel.
							return enumUI.Label ?? enumUI.ShortLabel;
					}
				}

				return null;
			}

			var s = new StringBuilder();
			bool once = false;

			Array enumValues = Enum.GetValues(enumType);
			for (int n = 0; n < enumValues.Length; ++n)
			{
				var enumValue = (Enum)enumValues.GetValue(n);

				if (currentValue.HasFlag(enumValue) && Convert.ToInt32(enumValue) != 0)
				{
					if (once)
					{
						s.Append(", ");
					}

					var enumUI = enumValue.GetEnumValueAttribute<EnumUI>();
					if (enumUI != null)
					{
						switch (displayType)
						{
							case DropdownCurrentValueDisplayType.ShortLabels:
								// Try ShortLabel, if that's null, try Label.
								// If that's still null, just show the enumValue.
								s.Append(enumUI.ShortLabel ?? enumUI.Label ?? enumValue.ToStringLabel());
								break;
							default: // default using DropdownCurrentValueDisplayType.Labels
								// Try Label, if that's null, try ShortLabel.
								// If that's still null, just show the enumValue.
								s.Append(enumUI.Label ?? enumUI.ShortLabel ?? enumValue.ToStringLabel());
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
