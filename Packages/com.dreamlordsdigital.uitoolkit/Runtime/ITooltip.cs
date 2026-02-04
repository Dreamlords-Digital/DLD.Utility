// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

/// <summary>
///    Allows you to show a tooltip.
/// </summary>
public interface ITooltip
{
	/// <summary>
	///    Show a tooltip message that follows the mouse cursor.
	/// </summary>
	/// <param name="context">The thing that caused the tooltip to be shown.</param>
	/// <param name="text">Text to be shown as the tooltip message.</param>
	/// <param name="iconClassName">Optional icon drawn before the tooltip text. This is a USS style name.</param>
	/// <param name="mousePos">
	///    Initial mouse position. This ensures the tooltip is at the correct position the moment it
	///    appears.
	/// </param>
	/// <param name="append">
	///    When set to true, the specified tooltip text will be added below all other currently shown text already on the
	///    tooltip.
	///    When set to false, the specified tooltip text will replace all other currently shown text in the tooltip.
	/// </param>
	/// <remarks>
	///    Call <see cref="HideTooltip"/> or <see cref="HideTooltipIfContextIs"/> to hide it.
	/// </remarks>
	void ShowTooltipAtMouse(VisualElement context, string text, string iconClassName, Vector2 mousePos, bool append = false);

	/// <summary>
	///    Show a tooltip message that is anchored to a <see cref="VisualElement"/>.
	/// </summary>
	/// <param name="context">The thing that caused the tooltip to be shown.</param>
	/// <param name="anchorElement">Where to anchor the tooltip.</param>
	/// <param name="text">Text to be shown as the tooltip message.</param>
	/// <param name="iconClassName">Optional icon drawn before the tooltip text. This is a USS style name.</param>
	/// <param name="anchorPoint">In what position the tooltip should be in, relative to the anchor.</param>
	/// <param name="append">
	///    When set to true, the specified tooltip text will be added below all other currently shown text already on the
	///    tooltip.
	///    When set to false, the specified tooltip text will replace all other currently shown text in the tooltip.
	/// </param>
	/// <remarks>
	///    Call <see cref="HideTooltip"/> or <see cref="HideTooltipIfContextIs"/> to hide it.
	/// </remarks>
	void ShowTooltipAt(VisualElement context, VisualElement anchorElement, string text, string iconClassName, ElementAnchorPoint anchorPoint, bool append = false);

	/// <summary>
	///    Add another message to the tooltip.
	/// </summary>
	/// <remarks>
	///    The new message will be shown below all the existing messages.
	/// </remarks>
	/// <param name="context">The thing that caused the tooltip to be shown.</param>
	/// <param name="text"></param>
	/// <param name="iconClassName">Optional icon drawn before the tooltip text. This is a USS style name.</param>
	void AddToTooltip(VisualElement context, string text, string iconClassName = null);

	/// <summary>
	///    Remove all messages in the tooltip, without hiding it (assuming it's currently shown).
	/// </summary>
	void ClearTooltipMessages();

	/// <summary>
	///    Show the tooltip and make it follow the mouse cursor.
	/// </summary>
	/// <remarks>
	///    <para>
	///       This is meant to be called after calling <see cref="AddToTooltip"/> multiple times.
	///    </para>
	///    <para>
	///       Call <see cref="HideTooltip"/> or <see cref="HideTooltipIfContextIs"/> to hide it.
	///    </para>
	/// </remarks>
	void ShowAtMouseCursor(Vector2 mousePos);

	/// <summary>
	///    Show the tooltip and make it anchored to a <see cref="VisualElement"/>.
	/// </summary>
	/// <param name="context">The thing that caused the tooltip to be shown.</param>
	/// <param name="anchorPoint">In what position the tooltip should be in, relative to the anchor.</param>
	/// <remarks>
	///    <para>
	///       This is meant to be called after calling <see cref="AddToTooltip"/> multiple times.
	///    </para>
	///    <para>
	///       Call <see cref="HideTooltip"/> or <see cref="HideTooltipIfContextIs"/> to hide it.
	///    </para>
	///    <para>
	///       If the <paramref name="context"/> has a <see cref="TooltipCollection"/> or <see cref="TooltipWithAnchor"/>
	///       assigned to its <see cref="VisualElement.userData"/>, then the anchor specified in
	///       <see cref="TooltipCollection.TooltipAnchor"/> (or <see cref="TooltipWithAnchor.TooltipAnchor"/>)
	///       will be used as the anchor.
	///    </para>
	///    <para>
	///       Otherwise, the <paramref name="context"/> will be used as the anchor.
	///    </para>
	/// </remarks>
	void ShowAt(VisualElement context, ElementAnchorPoint anchorPoint);

	/// <summary>
	///    Assign an "owner" to the tooltip even if no tooltip text is currently shown.
	/// </summary>
	/// <param name="context"></param>
	/// <remarks>
	///    <para>
	///       This is used when the mouse is currently on a <see cref="VisualElement"/> that usually would display
	///       a tooltip, but currently isn't showing any.
	///    </para>
	///    <para>
	///       For example, let's say the mouse cursor moves into a textbox that shows an error tooltip message
	///       only when the text inside fails a regex. If the text inside didn't fail the regex, then no
	///       error tooltip will be shown, but <see cref="SetContext"/> should still be called.
	///       <see cref="RefreshTooltipsOfContext"/> would then be called afterwards when the conditions
	///       are met (e.g. mouse cursor never left the textbox, but the text inside the textbox has changed)
	///       to make any relevant tooltips show up.
	///    </para>
	/// </remarks>
	void SetContext(VisualElement context);

	void HideTooltip();

	/// <summary>
	///    If the context that was last assigned to the tooltip matches the one specified, the tooltip is hidden.
	/// </summary>
	/// <param name="context">The thing that caused the tooltip to be shown with a specific text and icon.</param>
	/// <param name="removeOnlyLastMessagesWithMatchingContext">
	///    Only remove the most recent tooltip messages (if it's showing multiple tooltip messages)
	///    that the <paramref name="context"/> owns, instead of hiding the entire tooltip.
	/// </param>
	/// <remarks>
	///    Basically, this ensures a <see cref="VisualElement"/> that showed a tooltip
	///    will hide the tooltip only if the tooltip is still showing that VisualElement's message.
	///    If the tooltip happened to be showing the text given by a different VisualElement already at that point,
	///    then it won't be hidden.
	/// </remarks>
	void HideTooltipIfContextIs(VisualElement context, bool removeOnlyLastMessagesWithMatchingContext = false);

	/// <summary>
	///    Removes all currently displayed messages belonging to the specified <paramref name="context"/>,
	///    then re-adds the up-to-date messages provided by the specified <paramref name="context"/>.
	/// </summary>
	/// <remarks>
	///    If the tooltip isn't shown on the specified context, then this aborts.<br/>
	///    If the tooltip ends up with no more messages, then the tooltip will be automatically hidden.<br/>
	///    If the tooltip ends up having messages, and it's currently hidden, then it will be automatically shown.
	///    Whether it shows up anchored to a VisualElement or following the mouse cursor, it just reuses what was last used.
	/// </remarks>
	void RefreshTooltipsOfContext(VisualElement context);

	/// <summary>
	/// While in a keypress event, it's possible that the mouse is currently under a VisualElement that is providing
	/// a tooltip, and the keypress event causes that VisualElement to be removed from the panel.
	/// So we need to forcibly remove the tooltip if the mouse happens to be under such a VisualElement.
	/// For example, the mouse could be under a search result that shows a tooltip, and typing something different
	/// in the search text-field causes the search results to change. The tooltip needs to refresh.
	/// </summary>
	void HideTooltipIfUnderMouse<T>() where T : VisualElement;

	/// <summary>
	/// While in a keypress event, it's possible that the mouse is currently under a VisualElement that is providing
	/// a tooltip, and the keypress event causes that VisualElement to be removed from the panel.
	/// So we need to forcibly remove the tooltip if the mouse happens to be under such a VisualElement.
	/// For example, the mouse could be under a search result that shows a tooltip, and typing something different
	/// in the search text-field causes the search results to change. The tooltip needs to refresh.
	/// </summary>
	void RefreshTooltipIfUnderMouse<T>() where T : VisualElement;

	/// <summary>
	///    Whether the user is currently performing a drag-and-drop operation or not.
	/// </summary>
	/// <remarks>
	///    This is used by tooltip displayers that do not want their tooltip to be shown
	///    when the user is performing a drag-and-drop operation.
	/// </remarks>
	bool IsDragging { get; }
}

}
