// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

namespace DLD.UIToolkit
{

public class TooltipMessage
{
	public string IconClassName;
	public string Text;

	public TooltipMessage(string iconClassName = null, string text = null)
	{
		IconClassName = iconClassName;
		Text = text;
	}

	public static readonly TooltipMessage JumpToSourceFile = new(BaseIcons.JumpToSourceFile, "<i>Left-Click to jump to source file.</i>");
	public static readonly TooltipMessage OpenLinkInWebBrowser = new(BaseIcons.OpenLinkInWebBrowser, "<i>Ctrl + Left-Click to open link.</i>");
}

}
