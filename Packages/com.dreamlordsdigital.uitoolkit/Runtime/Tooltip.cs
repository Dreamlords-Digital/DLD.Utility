using System.Collections.Generic;
using DLD.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public interface ITooltip
	{
		void ShowTooltipAtMouse(IEventHandler context, string text, string iconClassName, Vector2 mousePos, bool pushToStack = false);
		void HideTooltip();
		void HideTooltipIfContextIs(IEventHandler context, bool popFromStack = false);
	}

	public static class TooltipUtil
	{
		public static readonly EventCallback<PointerEnterEvent, ITooltip> ShowFromUserData = _ShowTooltipFromUserData;
		public static readonly EventCallback<PointerLeaveEvent, ITooltip> Hide = _HideTooltip;

		static readonly EventCallback<PointerEnterEvent, ITooltip> ShowTooltipFromUserDataPushToStack = _ShowTooltipFromUserDataPushToStack;
		static readonly EventCallback<PointerLeaveEvent, ITooltip> HideTooltipIfContextIs = _HideTooltipIfContextIs;

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

		static void _ShowTooltipFromUserData(PointerEnterEvent e, ITooltip t)
		{
			var eventTarget = (VisualElement)e.target;
			string tooltip = (string)eventTarget.userData;

			if (string.IsNullOrWhiteSpace(tooltip))
			{
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

			t.ShowTooltipAtMouse(eventTarget, tooltip, iconClassName, e.position);
		}

		static void _ShowTooltipFromUserDataPushToStack(PointerEnterEvent e, ITooltip t)
		{
			var eventTarget = (VisualElement)e.target;
			string tooltip = (string)eventTarget.userData;

			if (string.IsNullOrWhiteSpace(tooltip))
			{
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

			t.ShowTooltipAtMouse(eventTarget, tooltip, iconClassName, e.position, true);
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

		public void Set(IEventHandler context, string text, string iconClassName = null, bool pushToStack = false)
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
						newIcon.AddToClassList(BaseIcons.ICON_STYLE_CLASS);
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
						nextAvailable.icon.AddToClassList(BaseIcons.ICON_STYLE_CLASS);
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
					_icon.AddToClassList(BaseIcons.ICON_STYLE_CLASS);
					_icon.AddToClassList(iconClassName);
				}
				else
				{
					_icon.style.display = DisplayStyle.None;
				}

				_originalRowUsed = true;
			}
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
	}
}
