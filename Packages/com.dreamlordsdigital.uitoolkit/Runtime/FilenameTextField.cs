// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

/// <summary>
///    TextField that displays a non-user-editable file type extension label at the end.
/// </summary>
[UxmlElement]
public partial class FilenameTextField : TextField
{
	readonly Label _fileExtensionLabel;

	System.Action<string> _editCallback;
	System.Action _confirmCallback;

	public FilenameTextField()
	{
		_fileExtensionLabel = new Label();

		// The file type extension label is a display only, so stop it from sending/receiving events.
		_fileExtensionLabel.focusable = false;
		_fileExtensionLabel.pickingMode = PickingMode.Ignore;
		_fileExtensionLabel.RegisterCallback((ChangeEvent<string> e) => e.StopPropagation());

		// ---------------------------------------------------------------------

		// When you click on the textbox (the background that contains the text element)
		// it just makes the text cursor go to the beginning.
		// We fix that by making it so that if the mouse click is at the right of the textbox,
		// we move the text cursor to the end instead.

		var textInput = this.Q("unity-text-input");
		textInput.delegatesFocus = true;
		textInput.Add(_fileExtensionLabel);

		textInput.RegisterCallback((MouseDownEvent e) =>
		{
			var me = (VisualElement)e.currentTarget; // TextInput
			var textField = (TextField)me.parent;

			if (textField.focusController.focusedElement != textField)
			{
				// the textfield isn't focused
				return;
			}

			if (textField.cursorIndex == 0 && textField.selectIndex == textField.text.Length)
			{
				// all text was selected, we shouldn't do anything
				return;
			}

			// clicked on textfield, and all is not selected
			if (e.localMousePosition.x > me.resolvedStyle.paddingLeft)
			{
				// clicked at the right of textfield
				// move cursor to end
				textField.cursorIndex = textField.text.Length;
				textField.selectIndex = textField.cursorIndex;
			}
			else
			{
				// clicked at the left of textfield
				// move cursor to beginning
				textField.cursorIndex = 0;
				textField.selectIndex = 0;
			}
		});

		RegisterCallback((ChangeEvent<string> e, FilenameTextField f) => f.OnEditFileTextField(e), this);
		RegisterCallback((KeyDownEvent e, FilenameTextField f) =>
		{
			if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter)
			{
				// user wants to commit to the typed value
				f.OnConfirm();
			}
		}, this, TrickleDown.TrickleDown);

		// ---------------------------------------------------------------------

		var textElement = textInput.Q<TextElement>();
		textElement.AddToClassList("dld-label--filename");
		_fileExtensionLabel.AddToClassList("dld-label--filename-extension");
	}

	public string Extension
	{
		get => _fileExtensionLabel.text;
		set => _fileExtensionLabel.text = value;
	}

	public string FullValue { get; private set; }

	public void SetFilename(string fullFilename, string filenameNoExtension)
	{
		FullValue = fullFilename;
		value = filenameNoExtension;
	}

	public void SetFilename(string fullFilename, string filenameNoExtension, string extension)
	{
		FullValue = fullFilename;
		value = filenameNoExtension;
		Extension = extension;
	}

	public void SetEditCallback(System.Action<string> newCallback)
	{
		_editCallback = newCallback;
	}

	public void SetConfirmCallback(System.Action newCallback)
	{
		_confirmCallback = newCallback;
	}

	void OnEditFileTextField(ChangeEvent<string> e)
	{
		FullValue = $"{e.newValue}{Extension}";
		_editCallback?.Invoke(FullValue);
	}

	void OnConfirm()
	{
		_confirmCallback?.Invoke();
	}
}

}
