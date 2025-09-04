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
		void ShowTooltipAtMouse(VisualElement context, string text, string iconClassName, Vector2 mousePos, bool pushToStack = false);

		void ShowTooltipAt(VisualElement context, string text, string iconClassName, ElementAnchorPoint anchorPoint, bool pushToStack = false);

		/// <summary>
		///    Add another message to the tooltip, assuming it's already shown.
		/// </summary>
		/// <remarks>
		///    The new message will be shown above and all the existing messages will be moved downward.
		/// </remarks>
		/// <param name="context">The thing that caused the tooltip to be shown.</param>
		/// <param name="text"></param>
		/// <param name="iconClassName">Optional icon drawn before the tooltip text. This is a USS style name.</param>
		void AddToTooltip(VisualElement context, string text, string iconClassName = null);

		void HideTooltip();

		/// <summary>
		///    Remove all messages in the tooltip, without hiding it (if it's shown).
		/// </summary>
		void ClearTooltipMessages();

		void ShowAtMouseCursor(Vector2 mousePos);

		void ShowAt(VisualElement context, ElementAnchorPoint anchorPoint);

		void SetContext(VisualElement eventTarget);

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
		void HideTooltipIfContextIs(VisualElement context, bool popFromStack = false);

		/// <summary>
		///    Removes all currently displayed messages of the specified context,
		///    then re-adds the up-to-date messages given by the specified context.
		/// </summary>
		/// <remarks>
		///    If the tooltip isn't shown on the specified context, then this aborts.<br/>
		///    If the tooltip ends up with no more messages, then the tooltip will be automatically hidden.<br/>
		///    If the tooltip ends up having messages, and it's currently hidden, then it will be automatically shown.
		/// </remarks>
		void RefreshTooltipsOfContext(VisualElement context);
	}

	public class TooltipMessage
	{
		public string IconClassName;
		public string Text;

		public TooltipMessage(string iconClassName = null, string text = null)
		{
			IconClassName = iconClassName;
			Text = text;
		}
	}

	public static class TooltipUtil
	{
		public static readonly EventCallback<PointerEnterEvent, ITooltip> ShowFromUserData = _ShowTooltipFromUserData;
		public static readonly EventCallback<PointerEnterEvent, ITooltip> ShowAtRightFromUserData = _ShowTooltipAtRightFromUserData;
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

		public static string CreateObsoleteTooltipIcon(ObsoleteAttribute obsoleteAttribute)
		{
			return obsoleteAttribute.IsError ? BaseIcons.GENERIC_ERROR : BaseIcons.GENERIC_WARNING;
		}

		public static string CreateObsoleteTooltipText(ObsoleteAttribute obsoleteAttribute, string obsoleteMessageToUseIfNull = DEFAULT_OBSOLETE_MESSAGE)
		{
			if (!string.IsNullOrWhiteSpace(obsoleteAttribute.Message))
			{
				return obsoleteAttribute.Message;
			}
			return obsoleteMessageToUseIfNull;
		}

		public static TooltipMessage CreateObsoleteTooltipMessage(ObsoleteAttribute obsoleteAttribute, string obsoleteMessageToUseIfNull = DEFAULT_OBSOLETE_MESSAGE)
		{
			return new TooltipMessage(CreateObsoleteTooltipIcon(obsoleteAttribute), CreateObsoleteTooltipText(obsoleteAttribute, obsoleteMessageToUseIfNull));
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
				tooltipDisplayer.RegisterCallback(Hide, tooltip);
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
				tooltipDisplayer.RegisterCallback(Hide, tooltip);
			}
			else
			{
				tooltipDisplayer.RegisterCallback(ShowFromUserData, tooltip);
				tooltipDisplayer.RegisterCallback(Hide, tooltip);
			}
		}

		public static void RegisterToRight(this VisualElement tooltipDisplayer, ITooltip tooltip)
		{
			if (tooltipDisplayer == null)
			{
				return;
			}

			tooltipDisplayer.RegisterCallback(ShowAtRightFromUserData, tooltip);
			tooltipDisplayer.RegisterCallback(Hide, tooltip);
		}

		static void _ShowTooltipFromUserData(PointerEnterEvent e, ITooltip t)
		{
			var eventTarget = (VisualElement)e.target;
			_Show(eventTarget, t, e.position, false, ElementAnchorPoint.Mouse);
		}

		static void _ShowTooltipFromUserDataPushToStack(PointerEnterEvent e, ITooltip t)
		{
			var eventTarget = (VisualElement)e.target;
			_Show(eventTarget, t, e.position, true, ElementAnchorPoint.Mouse);
		}

		static void _ShowTooltipAtRightFromUserData(PointerEnterEvent e, ITooltip t)
		{
			var eventTarget = (VisualElement)e.target;
			_Show(eventTarget, t, e.position, false, ElementAnchorPoint.Right);
		}

		public static (string, string) GetTooltipText(string tooltip)
		{
			string iconClassName;
			int semicolonIdx = tooltip.IndexOf(';');
			if (semicolonIdx != -1)
			{
				iconClassName = tooltip.Substring(0, semicolonIdx);
				tooltip = tooltip.Substring(semicolonIdx+1);

				if (string.IsNullOrWhiteSpace(tooltip))
				{
					return (null, null);
				}
			}
			else
			{
				iconClassName = null;
			}

			return (tooltip, iconClassName);
		}

		static void _Show(VisualElement eventTarget, ITooltip t, Vector2 mousePos, bool pushToStack, ElementAnchorPoint anchorPoint)
		{
			switch (eventTarget.userData)
			{
				case string tooltip:
				{
					if (string.IsNullOrWhiteSpace(tooltip))
					{
						// nothing to show
						t.SetContext(eventTarget);
						return;
					}

					(string tooltipText, string iconClassName) = GetTooltipText(tooltip);

					switch (anchorPoint)
					{
						case ElementAnchorPoint.Mouse:
							t.ShowTooltipAtMouse(eventTarget, tooltipText, iconClassName, mousePos, pushToStack);
							break;
						default:
							t.ShowTooltipAt(eventTarget, tooltipText, iconClassName, anchorPoint, pushToStack);
							break;
					}
					break;
				}
				case TooltipMessage tooltipMessage:
				{
					if (string.IsNullOrWhiteSpace(tooltipMessage.Text))
					{
						// nothing to show
						t.SetContext(eventTarget);
						return;
					}
					switch (anchorPoint)
					{
						case ElementAnchorPoint.Mouse:
							t.ShowTooltipAtMouse(eventTarget, tooltipMessage.Text, tooltipMessage.IconClassName, mousePos, pushToStack);
							break;
						default:
							t.ShowTooltipAt(eventTarget, tooltipMessage.Text, tooltipMessage.IconClassName, anchorPoint, pushToStack);
							break;
					}
					break;
				}
				case TooltipMessage[] tooltipMessageArray:
				{
					if (tooltipMessageArray.Length == 0)
					{
						t.SetContext(eventTarget);
						return;
					}
					if (!pushToStack)
					{
						t.ClearTooltipMessages();
					}

					bool addedAtLeastOne = false;
					for (int i = tooltipMessageArray.Length - 1; i >= 0; --i)
					{
						if (tooltipMessageArray[i] == null || string.IsNullOrWhiteSpace(tooltipMessageArray[i].Text))
						{
							continue;
						}

						addedAtLeastOne = true;
						t.AddToTooltip(eventTarget, tooltipMessageArray[i].Text, tooltipMessageArray[i].IconClassName);
					}

					if (!addedAtLeastOne)
					{
						t.SetContext(eventTarget);
					}

					switch (anchorPoint)
					{
						case ElementAnchorPoint.Mouse:
							t.ShowAtMouseCursor(mousePos);
							break;
						default:
							t.ShowAt(eventTarget, anchorPoint);
							break;
					}
					break;
				}
				case List<TooltipMessage> tooltipMessageList:
				{
					if (tooltipMessageList.Count == 0)
					{
						t.SetContext(eventTarget);
						return;
					}
					if (!pushToStack)
					{
						t.ClearTooltipMessages();
					}

					bool addedAtLeastOne = false;
					for (int i = tooltipMessageList.Count - 1; i >= 0; --i)
					{
						if (tooltipMessageList[i] == null || string.IsNullOrWhiteSpace(tooltipMessageList[i].Text))
						{
							continue;
						}

						addedAtLeastOne = true;
						t.AddToTooltip(eventTarget, tooltipMessageList[i].Text, tooltipMessageList[i].IconClassName);
					}

					if (!addedAtLeastOne)
					{
						t.SetContext(eventTarget);
					}

					switch (anchorPoint)
					{
						case ElementAnchorPoint.Mouse:
							t.ShowAtMouseCursor(mousePos);
							break;
						default:
							t.ShowAt(eventTarget, anchorPoint);
							break;
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
		const string FOLLOW_ELEMENT_STYLE_CLASS = "dld-tooltip__bg--follow-element";
		const string TOOLTIP_ICON_STYLE_CLASS = "dld-tooltip__icon";

		enum ShowType
		{
			None,
			FollowMouseCursor,
			AttachToVisualElement,
		}

		ShowType _showType = ShowType.None;

		struct TooltipRow
		{
			public VisualElement Context;
			public VisualElement Container;
			public VisualElement Icon;
			public Label Text;
		}

		readonly List<TooltipRow> _messageRows = new(10);
		int _messageRowCountUsed;
		VisualElement _lastContext;
		ElementAnchorPoint _lastAnchorPoint;

		readonly EventCallback<PointerMoveEvent> _onPointerMove;

		public Tooltip()
		{
			var asset = Resources.Load<VisualTreeAsset>(TEMPLATE_RESOURCES_PATH);
			asset.CloneTree(this);
			this.RemoveTemplateContainer("Tooltip");

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

		public void PushToStack(VisualElement context, string text, string iconClassName = null)
		{
			_lastContext = context;
			Set(context, text, iconClassName, true);
		}

		public void ShowAtMouseCursor()
		{
			if (_messageRowCountUsed == 0)
			{
				return;
			}

			_lastAnchorPoint = ElementAnchorPoint.Mouse;
			_showType = ShowType.FollowMouseCursor;
			RemoveFromClassList(FOLLOW_ELEMENT_STYLE_CLASS);
			AddToClassList(FOLLOW_MOUSE_STYLE_CLASS);
			style.display = DisplayStyle.Flex;
		}

		public void ShowAtMouseCursor(Vector2 mousePos)
		{
			if (_messageRowCountUsed == 0)
			{
				return;
			}
			ShowAtMouseCursor();
			this.SetPosition(mousePos);
		}

		public void ShowAt(VisualElement context, ElementAnchorPoint anchorPoint)
		{
			_lastAnchorPoint = anchorPoint;
			Rect contextRect = context.layout;
			var anchorPos = anchorPoint switch
			{
				ElementAnchorPoint.LowerRight => new Vector2(contextRect.width, contextRect.height),
				ElementAnchorPoint.Left => new Vector2(0, 0),
				ElementAnchorPoint.Right => new Vector2(contextRect.width, 0),
				_ => new Vector2(0, contextRect.height), // default is Bottom
			};
			var contextWorldPos = context.LocalToWorld(anchorPos);
			var localPos = parent.WorldToLocal(contextWorldPos);
			this.SetPosition(localPos);

			_showType = ShowType.AttachToVisualElement;
			RemoveFromClassList(FOLLOW_MOUSE_STYLE_CLASS);
			AddToClassList(FOLLOW_ELEMENT_STYLE_CLASS);
			style.display = DisplayStyle.Flex;
		}

		public void ShowAtMouseCursor(VisualElement context, string text, string iconClassName = null, bool pushToStack = false)
		{
			_lastContext = context;
			Set(context, text, iconClassName, pushToStack);
			ShowAtMouseCursor();
		}

		public void ShowAtMouseCursor(VisualElement context, string text, string iconClassName, Vector2 mousePos, bool pushToStack = false)
		{
			_lastContext = context;
			ShowAtMouseCursor(context, text, iconClassName, pushToStack);
			this.SetPosition(mousePos);
		}

		public void ShowAt(VisualElement context, string text, string iconClassName, ElementAnchorPoint anchorPoint, bool pushToStack = false)
		{
			_lastContext = context;
			Set(context, text, iconClassName, pushToStack);
			ShowAt(context, anchorPoint);
		}

		public void ClearTooltipMessages()
		{
			Clear();
			_messageRowCountUsed = 0;
		}

		public void SetContext(VisualElement context)
		{
			_lastContext = context;
		}

		public void Hide()
		{
			_lastContext = null;

			_Hide();
		}

		void _Hide()
		{
			_showType = ShowType.None;
			RemoveFromClassList(FOLLOW_MOUSE_STYLE_CLASS);
			RemoveFromClassList(FOLLOW_ELEMENT_STYLE_CLASS);
			style.display = DisplayStyle.None;

			if (_messageRowCountUsed > 0)
			{
				Clear();
				_messageRowCountUsed = 0;
			}
		}

		public void HideIfContextIs(VisualElement context, bool popFromStack = false)
		{
			if (popFromStack)
			{
				while (_messageRowCountUsed > 0 && _messageRows[_messageRowCountUsed - 1].Context == context)
				{
					Debug.Assert(ReferenceEquals(_messageRows[_messageRowCountUsed - 1].Container, this[0]),
						$"_messageRows[{_messageRowCountUsed - 1}]: \"{_messageRows[_messageRowCountUsed - 1].Text.text}\", this[0]: \"{this[0].Q<Label>().text}\"");

					RemoveAt(0);
					_messageRowCountUsed -= 1;
				}
				Debug.Assert(_messageRowCountUsed == childCount,
					$"_messageRowCountUsed: {_messageRowCountUsed} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

				if (_messageRowCountUsed == 0)
				{
					_Hide();
				}
			}
			else
			{
				// check only topmost message, if context matches, hide
				if (_messageRowCountUsed > 0 && _messageRows[_messageRowCountUsed - 1].Context == context)
				{
					_Hide();
				}
			}
		}

		public void RefreshTooltipsOfContext(VisualElement context)
		{
			Debug.Assert(context != null, "RefreshTooltipsOfContext: Passed context is null");
			if (_lastContext != context)
			{
				return;
			}

			if (_messageRowCountUsed > 0)
			{
				Debug.Assert(_messageRowCountUsed == childCount,
					$"_messageRowCountUsed: {_messageRowCountUsed} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

				for (int n = _messageRowCountUsed - 1; n >= 0; --n)
				{
					if (_messageRows[n].Context == context)
					{
						int reversedIndex = _messageRowCountUsed - (n + 1);
						Debug.Assert(ReferenceEquals(_messageRows[n].Container, this[reversedIndex]),
							$"_messageRows[{n}]: \"{_messageRows[n].Text.text}\", this[{reversedIndex}]: \"{this[reversedIndex].Q<Label>().text}\"");

						RemoveAt(reversedIndex);
						--_messageRowCountUsed;
					}
				}
			}

			Debug.Assert(_messageRowCountUsed == childCount,
				$"_messageRowCountUsed: {_messageRowCountUsed} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

			switch (context.userData)
			{
				case string tooltipText:
				{
					if (!string.IsNullOrWhiteSpace(tooltipText))
					{
						(string updatedTooltipText, string iconClassName) = TooltipUtil.GetTooltipText(tooltipText);
						PushToStack(context, updatedTooltipText, iconClassName);
					}
					break;
				}
				case TooltipMessage tooltipMessage:
				{
					if (!string.IsNullOrWhiteSpace(tooltipMessage.Text))
					{
						PushToStack(context, tooltipMessage.Text, tooltipMessage.IconClassName);
					}
					break;
				}
				case TooltipMessage[] tooltipMessageArray:
				{
					for (int i = tooltipMessageArray.Length - 1; i >= 0; --i)
					{
						if (tooltipMessageArray[i] == null || string.IsNullOrWhiteSpace(tooltipMessageArray[i].Text))
						{
							continue;
						}
						PushToStack(context, tooltipMessageArray[i].Text, tooltipMessageArray[i].IconClassName);
					}
					break;
				}
				case List<TooltipMessage> tooltipMessageList:
				{
					for (int i = tooltipMessageList.Count - 1; i >= 0; --i)
					{
						if (tooltipMessageList[i] == null || string.IsNullOrWhiteSpace(tooltipMessageList[i].Text))
						{
							continue;
						}
						PushToStack(context, tooltipMessageList[i].Text, tooltipMessageList[i].IconClassName);
					}
					break;
				}
			}

			Debug.Assert(_messageRowCountUsed == childCount,
				$"_messageRowCountUsed: {_messageRowCountUsed} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");

			if (_messageRowCountUsed == 0)
			{
				_Hide();
			}
			else
			{
				switch (_lastAnchorPoint)
				{
					case ElementAnchorPoint.Mouse:
						ShowAtMouseCursor();
						break;
					default:
						ShowAt(context, _lastAnchorPoint);
						break;
				}
			}
		}

		// ==================================================================================================

		void OnPointerMove(PointerMoveEvent e)
		{
			if (_showType is ShowType.FollowMouseCursor or ShowType.None)
			{
				this.SetPosition(e.position);
			}
		}

		static TooltipRow CreateNewTooltipRow(VisualElement context, string text, string iconClassName = null)
		{
			var newContainer = new VisualElement();
			newContainer.AddToClassList("dld-tooltip__row");

			var newText = new Label(text);
			newText.AddToClassList("dld-tooltip__text");

			var newIcon = new VisualElement();
			if (!string.IsNullOrEmpty(iconClassName))
			{
				newIcon.AddToClassList(TOOLTIP_ICON_STYLE_CLASS);
				newIcon.AddToClassList(iconClassName);
			}

			newContainer.Add(newIcon);
			newContainer.Add(newText);

			return new TooltipRow
			{
				Context = context,
				Container = newContainer,
				Icon = newIcon,
				Text = newText
			};
		}

		void Set(VisualElement context, string text, string iconClassName = null, bool pushToStack = false)
		{
			if (pushToStack)
			{
				if (_messageRows.Count == _messageRowCountUsed) // used up all existing rows, make a new one
				{
					var newTooltipRow = CreateNewTooltipRow(context, text, iconClassName);

					_messageRows.Add(newTooltipRow);
					_messageRowCountUsed += 1;
					Insert(0, newTooltipRow.Container);
				}
				else // reuse an existing row
				{
					var nextAvailable = _messageRows[_messageRowCountUsed];
					{
						// update context
						nextAvailable.Context = context;
						_messageRows[_messageRowCountUsed] = nextAvailable;
					}
					nextAvailable.Text.text = text;
					if (!string.IsNullOrEmpty(iconClassName))
					{
						nextAvailable.Icon.style.display = DisplayStyle.Flex;
						nextAvailable.Icon.ClearClassList();
						nextAvailable.Icon.AddToClassList(TOOLTIP_ICON_STYLE_CLASS);
						nextAvailable.Icon.AddToClassList(iconClassName);
					}
					else
					{
						nextAvailable.Icon.style.display = DisplayStyle.None;
					}

					Insert(0, nextAvailable.Container);
					_messageRowCountUsed += 1;
				}
			}
			else
			{
				while (_messageRowCountUsed > 1)
				{
					RemoveAt(0);
					--_messageRowCountUsed;
				}

				_messageRowCountUsed = 1;
				if (_messageRows.Count == 0)
				{
					var newTooltipRow = CreateNewTooltipRow(context, text, iconClassName);
					_messageRows.Add(newTooltipRow);
					Insert(0, newTooltipRow.Container);
				}
				else
				{
					Insert(0, _messageRows[0].Container);
				}

				var lastMessage = _messageRows[0];
				{
					// update context
					lastMessage.Context = context;
					_messageRows[0] = lastMessage;
				}

				lastMessage.Text.text = text;
				if (!string.IsNullOrWhiteSpace(iconClassName))
				{
					lastMessage.Icon.style.display = DisplayStyle.Flex;
					lastMessage.Icon.ClearClassList();
					lastMessage.Icon.AddToClassList(TOOLTIP_ICON_STYLE_CLASS);
					lastMessage.Icon.AddToClassList(iconClassName);
				}
				else
				{
					lastMessage.Icon.style.display = DisplayStyle.None;
				}
			}

			Debug.Assert(_messageRowCountUsed == childCount,
				$"_messageRowCountUsed: {_messageRowCountUsed} childCount: {childCount} _messageRows.Count: {_messageRows.Count}");
		}
	}
}
