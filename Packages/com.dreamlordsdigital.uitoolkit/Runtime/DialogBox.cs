// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

public interface IDialogBoxListener
{
	void OnDialogBoxChosen(string chosenArg, string userArg1);
}

[UxmlElement]
public partial class DialogBox : VisualElement
{
	const string TemplateResourcesPath = "DLD UIToolkit/DialogBox";

	public const string GenericOk = "GenericOk";
	public const string GenericYes = "GenericYes";
	public const string GenericNo = "GenericNo";
	public const string GenericCancel = "GenericCancel";

	ITooltip _tooltip;

	IDialogBoxListener _listener;

	readonly Label _title;
	readonly Label _description;
	readonly Button _okButton;
	readonly Button _noButton;
	readonly Button _cancelButton;

	string _okTooltipText;
	string _noTooltipText;
	string _cancelTooltipText;

	string _okTooltipIcon = BaseIcons.GenericInfo;
	string _noTooltipIcon = BaseIcons.GenericError;
	string _cancelTooltipIcon = BaseIcons.GenericInfo;

	string _okArg;
	string _noArg;
	string _cancelArg;

	string _userArg1;

	public DialogBox()
	{
		var asset = Resources.Load<VisualTreeAsset>(TemplateResourcesPath);
		asset.CloneTree(this);
		this.RemoveTemplateContainer("Root");

		_title = this.Q<Label>("Title");
		_description = this.Q<Label>("Description");

		_okButton = this.Q<Button>("OK");
		_okButton.RegisterCallback<PointerEnterEvent, DialogBox>((e, me) => me.OnPointerEnterOk(e), this);
		_okButton.RegisterCallback<PointerLeaveEvent, DialogBox>((e, me) => me.OnPointerLeaveOk(e), this);
		_okButton.clicked += OnOkClicked;

		_noButton = this.Q<Button>("No");
		_noButton.RegisterCallback<PointerEnterEvent, DialogBox>((e, me) => me.OnPointerEnterNo(e), this);
		_noButton.RegisterCallback<PointerLeaveEvent, DialogBox>((e, me) => me.OnPointerLeaveNo(e), this);
		_noButton.clicked += OnNoClicked;

		_cancelButton = this.Q<Button>("Cancel");
		_cancelButton.RegisterCallback<PointerEnterEvent, DialogBox>((e, me) => me.OnPointerEnterCancel(e), this);
		_cancelButton.RegisterCallback<PointerLeaveEvent, DialogBox>((e, me) => me.OnPointerLeaveCancel(e), this);
		_cancelButton.clicked += OnCancelClicked;
	}

	// =====================================================================

	public bool IsShown => style.display == DisplayStyle.Flex;

	public void SetTooltip(ITooltip newTooltip)
	{
		_tooltip = newTooltip;
	}

	public void Show(IDialogBoxListener listener, string title = null, string description = null, bool showNo = false, bool showCancel = false,
		string okTooltipText = null, string noTooltipText = null, string cancelTooltipText = null,
		string okTooltipIcon = BaseIcons.GenericInfo, string noTooltipIcon = BaseIcons.GenericError, string cancelTooltipIcon = BaseIcons.GenericInfo,
		string okArg = GenericOk, string noArg = GenericNo, string cancelArg = GenericCancel, string userArg1 = null)
	{
		_listener = listener;

		_title.text = title;
		_description.text = description;

		_noButton.style.display = showNo ? DisplayStyle.Flex : DisplayStyle.None;
		_cancelButton.style.display = showCancel ? DisplayStyle.Flex : DisplayStyle.None;

		_okTooltipText = okTooltipText;
		_noTooltipText = noTooltipText;
		_cancelTooltipText = cancelTooltipText;

		_okArg = okArg;
		_noArg = noArg;
		_cancelArg = cancelArg;

		_okTooltipIcon = okTooltipIcon;
		_noTooltipIcon = noTooltipIcon;
		_cancelTooltipIcon = cancelTooltipIcon;

		_userArg1 = userArg1;

		style.display = DisplayStyle.Flex;
	}

	// =====================================================================

	void OnPointerEnterOk(PointerEnterEvent e)
	{
		if (_tooltip != null && !string.IsNullOrEmpty(_okTooltipText))
		{
			_tooltip.ShowTooltipAt(_okButton, _okButton, _okTooltipText, _okTooltipIcon, ElementAnchorPoint.Right);
		}
	}

	void OnPointerLeaveOk(PointerLeaveEvent e)
	{
		_tooltip?.HideTooltipIfContextIs(_okButton);
	}

	void OnOkClicked()
	{
		style.display = DisplayStyle.None;
		_listener?.OnDialogBoxChosen(_okArg, _userArg1);
		_tooltip?.HideTooltipIfContextIs(_okButton);
	}

	// -----------------------------------------------------------------

	void OnPointerEnterNo(PointerEnterEvent e)
	{
		if (_tooltip != null && !string.IsNullOrEmpty(_noTooltipText))
		{
			_tooltip.ShowTooltipAt(_noButton, _noButton, _noTooltipText, _noTooltipIcon, ElementAnchorPoint.Right);
		}
	}

	void OnPointerLeaveNo(PointerLeaveEvent e)
	{
		_tooltip?.HideTooltipIfContextIs(_noButton);
	}

	void OnNoClicked()
	{
		style.display = DisplayStyle.None;
		_listener?.OnDialogBoxChosen(_noArg, _userArg1);
		_tooltip?.HideTooltipIfContextIs(_noButton);
	}

	// -----------------------------------------------------------------

	void OnPointerEnterCancel(PointerEnterEvent e)
	{
		if (_tooltip != null && !string.IsNullOrEmpty(_cancelTooltipText))
		{
			_tooltip.ShowTooltipAt(_cancelButton, _cancelButton, _cancelTooltipText, _cancelTooltipIcon, ElementAnchorPoint.Right);
		}
	}

	void OnPointerLeaveCancel(PointerLeaveEvent e)
	{
		_tooltip?.HideTooltipIfContextIs(_cancelButton);
	}

	void OnCancelClicked()
	{
		style.display = DisplayStyle.None;
		_listener?.OnDialogBoxChosen(_cancelArg, _userArg1);
		_tooltip?.HideTooltipIfContextIs(_cancelButton);
	}

	// =====================================================================
}

}
