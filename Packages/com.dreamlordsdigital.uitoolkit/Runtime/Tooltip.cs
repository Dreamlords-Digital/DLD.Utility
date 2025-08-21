using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public interface ITooltip
	{
		void ShowTooltipAtMouse(string text, string iconClassName, Vector2 mousePos);
		void HideTooltip();
	}

	public static class TooltipUtil
	{
		public static readonly EventCallback<PointerEnterEvent, ITooltip> ShowFromUserData = _ShowTooltipFromUserData;
		public static readonly EventCallback<PointerLeaveEvent, ITooltip> Hide = _HideTooltip;

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

			t.ShowTooltipAtMouse(tooltip, iconClassName, e.position);
		}

		static void _HideTooltip(PointerLeaveEvent _, ITooltip t)
		{
			t.HideTooltip();
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

		string _lastIconStyleNameUsed;

		readonly VisualElement _icon;
		readonly Label _text;

		readonly EventCallback<PointerMoveEvent> _onPointerMove;

		public Tooltip()
		{
			var asset = Resources.Load<VisualTreeAsset>(TEMPLATE_RESOURCES_PATH);
			asset.CloneTree(this);
			this.RemoveTemplateContainer("Tooltip");

			_icon = this.Q<VisualElement>("Icon");
			_text = this.Q<Label>("Text");

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

		public void Set(string text, string iconClassName = null)
		{
			_text.text = text;

			if (!string.IsNullOrWhiteSpace(iconClassName))
			{
				if (!string.IsNullOrWhiteSpace(_lastIconStyleNameUsed))
				{
					_icon.RemoveFromClassList(_lastIconStyleNameUsed);
				}
				_icon.style.display = DisplayStyle.Flex;
				_icon.AddToClassList(iconClassName);
				_lastIconStyleNameUsed = iconClassName;
			}
			else
			{
				_icon.style.display = DisplayStyle.None;
			}
		}

		public void ShowAtMouseCursor(string text, string iconClassName = null)
		{
			Set(text, iconClassName);
			_showType = ShowType.FollowMouseCursor;
			AddToClassList(FOLLOW_MOUSE_STYLE_CLASS);
			style.display = DisplayStyle.Flex;
		}

		public void ShowAtMouseCursor(string text, string iconClassName, Vector2 mousePos)
		{
			ShowAtMouseCursor(text, iconClassName);
			this.SetPosition(mousePos);
		}

		public void Hide()
		{
			_showType = ShowType.None;
			RemoveFromClassList(FOLLOW_MOUSE_STYLE_CLASS);
			style.display = DisplayStyle.None;
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
