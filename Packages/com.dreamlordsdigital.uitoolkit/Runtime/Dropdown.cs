using System;
using DLD.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	[UxmlElement]
	public partial class Dropdown : VisualElement, IContextMenuListener
	{
		const string TEMPLATE_RESOURCES_PATH = "DLD UIToolkit/Dropdown";
		const string ARROW_STYLE_CLASS = "dld-dropdown__arrow";

		readonly Label _label;
		readonly Toggle _toggle;
		readonly VisualElement _icon;

		IContextMenu _contextMenu;
		bool _doAltBgStyling;

		System.Type _enumTypeOnOpen;
		System.Type _lastEnumTypeUsedOnOpen;
		Enum _currentEnumValue;

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

		public void SetValueWithoutNotify(Enum currentValue)
		{
			_toggle.label = currentValue.ToStringLabel();
			_currentEnumValue = currentValue;

			UpdateIcon(currentValue);
		}

		public void SetEnumTypeOnOpen(Type enumType)
		{
			_enumTypeOnOpen = enumType;
		}

		public void DoAltBgStyling(bool doAltBgStyling)
		{
			_doAltBgStyling = doAltBgStyling;
		}

		void OnOpen()
		{
			if (_enumTypeOnOpen != null)
			{
				Open(_enumTypeOnOpen);
			}
		}

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

		public void OnContextMenuChosen(int index, string label, object enumValue, object userArg2)
		{
			if (enumValue.Equals(_currentEnumValue))
			{
				// same as currently chosen value, nothing to do
				return;
			}

			_contextMenu.ChangeSelected(index);

			using var changeEvent = ChangeEvent<object>.GetPooled(_currentEnumValue, enumValue);
			changeEvent.target = this;

			_currentEnumValue = (Enum)enumValue;
			_toggle.label = label;
			UpdateIcon(_currentEnumValue);

			panel.visualTree.SendEvent(changeEvent);
		}

		public void OnContextMenuCanceled()
		{
			_toggle.value = false;
			_toggle.Focus();
		}

		void UpdateIcon(Enum currentValue)
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
