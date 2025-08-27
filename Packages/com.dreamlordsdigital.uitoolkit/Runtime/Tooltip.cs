using System;
using System.Collections.Generic;
using DLD.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public interface ITooltip
	{
		/// <summary>
		///    Show a tooltip message that follows the mouse cursor.
		///    It will not hide until <see cref="HideTooltip"/> or <see cref="HideTooltipIfContextIs"/> is called.
		/// </summary>
		/// <param name="context">The thing that caused the tooltip to be shown.</param>
		/// <param name="text"></param>
		/// <param name="iconClassName">Optional icon drawn before the tooltip text. This is a USS style name.</param>
		/// <param name="mousePos">Initial mouse position. This ensures the tooltip is at the correct position at the start.</param>
		/// <param name="pushToStack">Whether the tooltip text specified will be added to the existing text already on the tooltip, or not.</param>
		void ShowTooltipAtMouse(IEventHandler context, string text, string iconClassName, Vector2 mousePos, bool pushToStack = false);

		/// <summary>
		///    Add another message to the tooltip, assuming it's already shown.
		/// </summary>
		/// <remarks>
		///    The new message will be shown above and all the existing messages will be moved downward.
		/// </remarks>
		/// <param name="context">The thing that caused the tooltip to be shown.</param>
		/// <param name="text"></param>
		/// <param name="iconClassName">Optional icon drawn before the tooltip text. This is a USS style name.</param>
		void AddToTooltip(IEventHandler context, string text, string iconClassName = null);

		void HideTooltip();

		/// <summary>
		///    If the context that was last assigned to the tooltip matches the one specified, the tooltip is hidden.
		/// </summary>
		/// <remarks>
		///    Basically, this ensures a VisualElement that showed a tooltip
		///    will hide it only if the tooltip is still showing its message.
		/// </remarks>
		/// <param name="context"></param>
		/// <param name="popFromStack">
		///    Only remove the most recent tooltip message (if it's showing multiple tooltip messages),
		///    instead of hiding the entire tooltip.
		/// </param>
		void HideTooltipIfContextIs(IEventHandler context, bool popFromStack = false);
	}

	public class TooltipMessage
	{
		public string IconClassName;
		public string Text;

		public TooltipMessage(string iconClassName, string text)
		{
			IconClassName = iconClassName;
			Text = text;
		}
	}

	public static class TooltipUtil
	{
		public static readonly EventCallback<PointerEnterEvent, ITooltip> ShowFromUserData = _ShowTooltipFromUserData;
		public static readonly EventCallback<PointerLeaveEvent, ITooltip> Hide = _HideTooltip;

		static readonly EventCallback<PointerEnterEvent, ITooltip> ShowTooltipFromUserDataPushToStack = _ShowTooltipFromUserDataPushToStack;
		static readonly EventCallback<PointerLeaveEvent, ITooltip> HideTooltipIfContextIs = _HideTooltipIfContextIs;

		const string DEFAULT_OBSOLETE_MESSAGE = "Marked as obsolete";

		public static TooltipMessage[] CreateTooltipMessages(EnumUIAttribute enumUI, ObsoleteAttribute obsoleteAttribute, string obsoleteMessageToUseIfNull = DEFAULT_OBSOLETE_MESSAGE)
		{
			return new TooltipMessage[]
			{
				new(BaseIcons.GENERIC_INFO, enumUI.Tooltip),
				new(obsoleteAttribute.IsError ? BaseIcons.GENERIC_ERROR : BaseIcons.GENERIC_WARNING, obsoleteAttribute.Message ?? obsoleteMessageToUseIfNull),
			};
		}

		public static string CreateObsoleteTooltip(ObsoleteAttribute obsoleteAttribute, string obsoleteMessageToUseIfNull = DEFAULT_OBSOLETE_MESSAGE)
		{
			return $"{(obsoleteAttribute.IsError ? BaseIcons.GENERIC_ERROR : BaseIcons.GENERIC_WARNING)};{obsoleteAttribute.Message ?? obsoleteMessageToUseIfNull}";
		}

		public static string Register(this VisualElement tooltipDisplayer, ITooltip tooltip, string tooltipText, string iconClassName = BaseIcons.GENERIC_INFO, bool pushToStack = false)
		{
			if (tooltipDisplayer == null)
			{
				return null;
			}
			if (string.IsNullOrWhiteSpace(tooltipText))
			{
				return null;
			}

			// If passed tooltipText already has an icon inside, or user doesn't want an icon displayed,
			// then just use tooltipText as-is.
			string finalTooltipText = tooltipText.Contains(';') || string.IsNullOrWhiteSpace(iconClassName) ? tooltipText : $"{iconClassName};{tooltipText}";

			tooltipDisplayer.userData = finalTooltipText;

			if (pushToStack)
			{
				tooltipDisplayer.RegisterCallback(ShowTooltipFromUserDataPushToStack, tooltip);
				tooltipDisplayer.RegisterCallback(HideTooltipIfContextIs, tooltip);
			}
			else
			{
				tooltipDisplayer.RegisterCallback(ShowFromUserData, tooltip);
				tooltipDisplayer.RegisterCallback(Hide, tooltip);
			}

			return finalTooltipText;
		}

		public static void Register(this VisualElement tooltipDisplayer, ITooltip tooltip, bool pushToStack = false)
		{
			if (tooltipDisplayer == null)
			{
				return;
			}

			if (pushToStack)
			{
				tooltipDisplayer.RegisterCallback(ShowTooltipFromUserDataPushToStack, tooltip);
				tooltipDisplayer.RegisterCallback(HideTooltipIfContextIs, tooltip);
			}
			else
			{
				tooltipDisplayer.RegisterCallback(ShowFromUserData, tooltip);
				tooltipDisplayer.RegisterCallback(Hide, tooltip);
			}
		}

		static void _ShowTooltipFromUserData(PointerEnterEvent e, ITooltip t)
		{
			var eventTarget = (VisualElement)e.target;
			_Show(eventTarget, t, e.position, false);
		}

		static void _ShowTooltipFromUserDataPushToStack(PointerEnterEvent e, ITooltip t)
		{
			var eventTarget = (VisualElement)e.target;
			_Show(eventTarget, t, e.position, true);
		}

		static void _Show(VisualElement eventTarget, ITooltip t, Vector2 mousePos, bool pushToStack)
		{
			switch (eventTarget.userData)
			{
				case string tooltip:
				{
					if (string.IsNullOrWhiteSpace(tooltip))
					{
						// nothing to show
						return;
					}
					string iconClassName;
					int semicolonIdx = tooltip.IndexOf(';');
					if (semicolonIdx != -1)
					{
						iconClassName = tooltip.Substring(0, semicolonIdx);
						tooltip = tooltip.Substring(semicolonIdx+1);

						if (string.IsNullOrWhiteSpace(tooltip))
						{
							return;
						}
					}
					else
					{
						iconClassName = null;
					}

					t.ShowTooltipAtMouse(eventTarget, tooltip, iconClassName, mousePos, pushToStack);
					break;
				}
				case TooltipMessage tooltipMessage:
				{
					t.ShowTooltipAtMouse(eventTarget, tooltipMessage.Text, tooltipMessage.IconClassName, mousePos, pushToStack);
					break;
				}
				case TooltipMessage[] tooltipMessageArray:
				{
					t.ShowTooltipAtMouse(eventTarget, tooltipMessageArray[0].Text, tooltipMessageArray[0].IconClassName, mousePos, pushToStack);
					if (tooltipMessageArray.Length > 1)
					{
						for (int i = 1; i < tooltipMessageArray.Length; ++i)
						{
							t.AddToTooltip(eventTarget, tooltipMessageArray[i].Text, tooltipMessageArray[i].IconClassName);
						}
					}
					break;
				}
				case List<TooltipMessage> tooltipMessageList:
				{
					t.ShowTooltipAtMouse(eventTarget, tooltipMessageList[0].Text, tooltipMessageList[0].IconClassName, mousePos, pushToStack);
					if (tooltipMessageList.Count > 1)
					{
						for (int i = 1; i < tooltipMessageList.Count; ++i)
						{
							t.AddToTooltip(eventTarget, tooltipMessageList[i].Text, tooltipMessageList[i].IconClassName);
						}
					}
					break;
				}
			}
		}

		static void _HideTooltip(PointerLeaveEvent e, ITooltip t)
		{
			t.HideTooltip();
		}

		static void _HideTooltipIfContextIs(PointerLeaveEvent e, ITooltip t)
		{
			var eventTarget = (VisualElement)e.target;
			t.HideTooltipIfContextIs(eventTarget, true);
		}
	}

	[UxmlElement]
	public partial class Tooltip : VisualElement
	{
		const string TEMPLATE_RESOURCES_PATH = "DLD UIToolkit/Tooltip";
		const string FOLLOW_MOUSE_STYLE_CLASS = "dld-tooltip__bg--follow-mouse";
		const string TOOLTIP_ICON_STYLE_CLASS = "dld-tooltip__icon";

		enum ShowType
		{
			None,
			FollowMouseCursor,
			AttachToVisualElement,
		}

		ShowType _showType = ShowType.None;

		bool _originalRowUsed;
		readonly VisualElement _row;
		readonly VisualElement _icon;
		readonly Label _text;

		IEventHandler _context;
		readonly List<(IEventHandler context, VisualElement row, VisualElement icon, Label text)> _additionalRows = new();
		int _additionalRowCountUsed;

		readonly EventCallback<PointerMoveEvent> _onPointerMove;

		public Tooltip()
		{
			var asset = Resources.Load<VisualTreeAsset>(TEMPLATE_RESOURCES_PATH);
			asset.CloneTree(this);
			this.RemoveTemplateContainer("Tooltip");

			_row = this.Q<VisualElement>("Row");
			_icon = _row.Q<VisualElement>("Icon");
			_text = _row.Q<Label>("Text");

			// -----------------------------------

			style.display = DisplayStyle.None; // hide at first

			_onPointerMove = OnPointerMove;

			RegisterCallback<AttachToPanelEvent, Tooltip>((e, t) =>
			{
				e.destinationPanel.visualTree.RegisterCallback(t._onPointerMove);
			}, this);
			RegisterCallback<DetachFromPanelEvent, Tooltip>((e, t) =>
			{
				e.originPanel?.visualTree.UnregisterCallback(t._onPointerMove);
			}, this);
		}

		// ==================================================================================================

		public void PushToStack(IEventHandler context, string text, string iconClassName = null)
		{
			Set(context, text, iconClassName, true);
		}

		public void ShowAtMouseCursor(IEventHandler context, string text, string iconClassName = null, bool pushToStack = false)
		{
			Set(context, text, iconClassName, pushToStack);
			_showType = ShowType.FollowMouseCursor;
			AddToClassList(FOLLOW_MOUSE_STYLE_CLASS);
			style.display = DisplayStyle.Flex;
		}

		public void ShowAtMouseCursor(IEventHandler context, string text, string iconClassName, Vector2 mousePos, bool pushToStack = false)
		{
			ShowAtMouseCursor(context, text, iconClassName, pushToStack);
			this.SetPosition(mousePos);
		}

		public void Hide()
		{
			_originalRowUsed = false;
			_showType = ShowType.None;
			RemoveFromClassList(FOLLOW_MOUSE_STYLE_CLASS);
			style.display = DisplayStyle.None;

			if (_additionalRowCountUsed > 0)
			{
				for (int n = 0; n < _additionalRowCountUsed; ++n)
				{
					RemoveAt(0);
				}
				_additionalRowCountUsed = 0;
			}
		}

		public void HideIfContextIs(IEventHandler context, bool popFromStack = false)
		{
			if (popFromStack && _originalRowUsed && _additionalRowCountUsed > 0 && _additionalRows[_additionalRowCountUsed-1].Item1 == context)
			{
				RemoveAt(0);
				_additionalRowCountUsed -= 1;
			}
			else if (_context == context)
			{
				Hide();
			}
		}

		// ==================================================================================================

		void OnPointerMove(PointerMoveEvent e)
		{
			if (_showType == ShowType.FollowMouseCursor)
			{
				this.SetPosition(e.position);
			}
		}

		void Set(IEventHandler context, string text, string iconClassName = null, bool pushToStack = false)
		{
			if (_originalRowUsed && pushToStack)
			{
				if (_additionalRows.Count == _additionalRowCountUsed)
				{
					var newRow = new VisualElement();
					newRow.AddStyleClassesFrom(_row);

					var newText = new Label(text);
					newText.AddStyleClassesFrom(_text);

					var newIcon = new VisualElement();
					if (!string.IsNullOrEmpty(iconClassName))
					{
						newIcon.AddToClassList(TOOLTIP_ICON_STYLE_CLASS);
						newIcon.AddToClassList(iconClassName);
					}

					newRow.Add(newIcon);
					newRow.Add(newText);

					_additionalRows.Add((context, newRow, newIcon, newText));
					_additionalRowCountUsed += 1;
					Insert(0, newRow);
				}
				else
				{
					// reuse
					var nextAvailable = _additionalRows[_additionalRowCountUsed];
					{
						// update context
						nextAvailable.context = context;
						_additionalRows[_additionalRowCountUsed] = nextAvailable;
					}
					nextAvailable.text.text = text;
					nextAvailable.icon.ClearClassList();
					if (!string.IsNullOrEmpty(iconClassName))
					{
						nextAvailable.icon.AddToClassList(TOOLTIP_ICON_STYLE_CLASS);
						nextAvailable.icon.AddToClassList(iconClassName);
					}

					Insert(0, nextAvailable.row);
					_additionalRowCountUsed += 1;
				}
			}
			else
			{
				_context = context;
				_text.text = text;

				if (!string.IsNullOrWhiteSpace(iconClassName))
				{
					_icon.style.display = DisplayStyle.Flex;
					_icon.ClearClassList();
					_icon.AddToClassList(TOOLTIP_ICON_STYLE_CLASS);
					_icon.AddToClassList(iconClassName);
				}
				else
				{
					_icon.style.display = DisplayStyle.None;
				}

				_originalRowUsed = true;
			}
		}
	}
}
