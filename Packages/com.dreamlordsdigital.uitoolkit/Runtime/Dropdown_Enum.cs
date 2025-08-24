using System;
using DLD.Utility;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public partial class Dropdown
	{
		public void SetValueWithoutNotify(Enum currentValue)
		{
			_toggle.label = currentValue.ToStringLabel();
			_currentEnumValue = currentValue;

			UpdateCurrentValueIcon(currentValue);
		}

		public void SetEnumTypeOnOpen(Type enumType)
		{
			_enumTypeOnOpen = enumType;
			_currentMode = Mode.Enum;
		}

		// ==================================================================================

		void Open(System.Type enumType)
		{
			if (_lastEnumTypeUsedOnOpen == enumType && _contextMenu.WasLastShownOn(_toggle))
			{
				// Context Menu has the same entries as what we want,
				// so no need to add them again. Just reuse it as-is.
				_contextMenu.Show(_toggle, this);
				return;
			}

			var enumValues = Enum.GetValues(enumType);

			_contextMenu.DoAltBgStyling(_doAltBgStyling);
			_contextMenu.ClearMenu();
			for (int n = 0; n < enumValues.Length; ++n)
			{
				object enumValue = enumValues.GetValue(n);

				bool enumValueIsCurrent = enumValue.Equals(_currentEnumValue);

				string label = null;
				string iconClassName = null;
				string tooltipDesc = null;
				var enumUI = enumValue.GetEnumValueAttribute<EnumUI>();
				if (enumUI != null)
				{
					label = enumUI.Label;
					iconClassName = enumUI.IconStyleClass;
					tooltipDesc = enumUI.Tooltip;
				}
				if (string.IsNullOrEmpty(label))
				{
					label = enumValue.ToStringLabel();
				}

				_contextMenu.AddMenu(label, iconClassName, enumValueIsCurrent, tooltipDesc,
					listener: this, userArg1: enumValue);
			}
			_contextMenu.Show(_toggle, this);

			_lastEnumTypeUsedOnOpen = enumType;
		}

		void OnDropdownEnumChosen(int index, string label, object newValue)
		{
			if (newValue.Equals(_currentEnumValue))
			{
				// same as currently chosen value, nothing to do
				return;
			}

			_contextMenu.ChangeSelected(index);

			using var changeEvent = ChangeEvent<object>.GetPooled(_currentEnumValue, newValue);
			changeEvent.target = this;

			_currentEnumValue = (Enum)newValue;
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
	}
}
