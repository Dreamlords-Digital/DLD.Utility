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

		IContextMenu _contextMenu;

		System.Type _enumTypeOnOpen;
		System.Type _lastEnumTypeUsedOnOpen;
		object _currentEnumValue;

		public Dropdown()
		{
			var asset = Resources.Load<VisualTreeAsset>(TEMPLATE_RESOURCES_PATH);
			asset.CloneTree(this);
			this.RemoveTemplateContainer("Dropdown");

			_toggle = this.Q<Toggle>();
			_toggle.labelElement.focusable = true;

			var arrow = new VisualElement();
			arrow.AddToClassList(ARROW_STYLE_CLASS);
			_toggle.Add(arrow);

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
			_toggle.label = currentValue.ToString();
			_currentEnumValue = currentValue;
		}

		public void SetEnumTypeOnOpen(Type enumType)
		{
			_enumTypeOnOpen = enumType;
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
				_contextMenu.Show(_toggle, this);
				return;
			}

			var enumValues = Enum.GetValues(enumType);

			_contextMenu.ClearMenu();
			for (int n = 0; n < enumValues.Length; ++n)
			{
				object enumValue = enumValues.GetValue(n);

				bool match = enumValue.Equals(_currentEnumValue);
				string iconClassName = match ? BaseIcons.SELECTED_IN_DROPDOWN : null;
				_contextMenu.AddMenu(enumValue.ToString(), iconClassName, this, enumValue);
			}
			_contextMenu.Show(_toggle, this);

			_lastEnumTypeUsedOnOpen = enumType;
		}

		public void OnContextMenuChosen(int index, object enumValue, object userArg2)
		{
			if (enumValue.Equals(_currentEnumValue))
			{
				// same as currently chosen value, nothing to do
				return;
			}

			_contextMenu.ChangeMenuIcon(_currentEnumValue, iconClassStyleToRemove: BaseIcons.SELECTED_IN_DROPDOWN);
			_contextMenu.ChangeMenuIcon(enumValue, iconClassStyleToAdd: BaseIcons.SELECTED_IN_DROPDOWN);

			using var changeEvent = ChangeEvent<object>.GetPooled(_currentEnumValue, enumValue);
			changeEvent.target = this;

			_currentEnumValue = enumValue;
			_toggle.label = enumValue.ToString();

			panel.visualTree.SendEvent(changeEvent);
		}

		public void OnContextMenuCanceled()
		{
			_toggle.value = false;
			_toggle.Focus();
		}
	}
}
