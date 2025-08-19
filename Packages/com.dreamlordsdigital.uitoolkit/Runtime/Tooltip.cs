using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public interface ITooltip
	{
		void ShowTooltipAtMouse(string text, string iconClassName = null);
		void HideTooltip();
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

			var clonedRoot = this.Q<VisualElement>("Tooltip");
			foreach (string rootStyleClass in clonedRoot.GetClasses())
			{
				AddToClassList(rootStyleClass);
			}

			for (int n = clonedRoot.childCount - 1; n >= 0; --n)
			{
				if (clonedRoot[n].name == "Icon")
				{
					_icon = clonedRoot[n];
				}
				else if (clonedRoot[n].name == "Text")
				{
					_text = (Label)clonedRoot[n];
				}
				Insert(0, clonedRoot[n]);
			}

			clonedRoot.RemoveFromHierarchy();

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
