// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;
using DLD.Utility;
using UnityEngine;

namespace DLD.IMGUI
{
	public interface IGuiLabel
	{
		Color LabelTint { get; }
		GUIContent Label { get; }
	}

	public static class SelectionList
	{
		public delegate void SelectCallback(int index, string label);

		public delegate void DoubleClickCallback(int index, string label);

		const string DEFAULT_LIST_ITEM_STYLE_NAME = "ListItem";

		// ====================================================================================================

		public static int Draw(int selected, IList<IGuiLabel> list)
		{
			return Draw(selected, list, DEFAULT_LIST_ITEM_STYLE_NAME, null, null, null, null, 50);
		}

		public static int Draw(int selected, IList<IGuiLabel> list, Func<int, bool> visibilityPredicate)
		{
			return Draw(selected, list, DEFAULT_LIST_ITEM_STYLE_NAME, null, null, visibilityPredicate, null, 50);
		}

		public static int Draw(int selected, IList<IGuiLabel> list, GUIStyle elementStyle,
			Func<int, bool> visibilityPredicate)
		{
			return Draw(selected, list, elementStyle, null, null, visibilityPredicate, null, 50);
		}

		public static int Draw(int selected, IList<IGuiLabel> list, GUIStyle elementStyle,
			Func<int, bool> visibilityPredicate, Action<Rect, int> callbackOnHover)
		{
			return Draw(selected, list, elementStyle, null, null, visibilityPredicate, callbackOnHover, 50);
		}

		public static int Draw(int selected, IList<IGuiLabel> list, GUIStyle elementStyle)
		{
			return Draw(selected, list, elementStyle, null, null, null, null, 50);
		}

		public static int Draw(int selected, IList<IGuiLabel> list, DoubleClickCallback onDoubleClick)
		{
			return Draw(selected, list, DEFAULT_LIST_ITEM_STYLE_NAME, null, onDoubleClick, null, null, 50);
		}

		public static int Draw(int selected, IList<IGuiLabel> list, DoubleClickCallback onDoubleClick,
			Func<int, bool> visibilityPredicate)
		{
			return Draw(selected, list, DEFAULT_LIST_ITEM_STYLE_NAME, null, onDoubleClick, visibilityPredicate, null, 50);
		}

		public static int Draw(int selected, IList<IGuiLabel> list, GUIStyle elementStyle,
			DoubleClickCallback onDoubleClick, Func<int, bool> visibilityPredicate)
		{
			return Draw(selected, list, elementStyle, null, onDoubleClick, visibilityPredicate, null, 50);
		}

		public static int Draw(int selected, IList<IGuiLabel> list, GUIStyle elementStyle,
			DoubleClickCallback onDoubleClick, Func<int, bool> visibilityPredicate, Action<Rect, int> onHover)
		{
			return Draw(selected, list, elementStyle, null, onDoubleClick, visibilityPredicate, onHover, 50);
		}

		public static int Draw(int selected,
			IList<IGuiLabel> list,
			SelectCallback selectCallback,
			DoubleClickCallback onDoubleClick)
		{
			return Draw(selected, list, DEFAULT_LIST_ITEM_STYLE_NAME, selectCallback, onDoubleClick, null, null, 50);
		}

		public static int Draw(int selected,
			IList<IGuiLabel> list,
			GUIStyle elementStyle,
			SelectCallback selectCallback,
			DoubleClickCallback chooseCallback,
			Func<int, bool> visibilityPredicate,
			Action<Rect, int> hoveringOnElement,
			int maxHeight)
		{
			var prevColor = GUI.color;
			var firstVisible = -1;

			for (int i = 0; i < list.Count; ++i)
			{
				if (visibilityPredicate != null && !visibilityPredicate(i))
				{
					continue;
				}

				if (firstVisible == -1)
				{
					firstVisible = i;
				}

				Rect elementRect;
				if (list[i].Label.image != null)
				{
					elementRect = GUILayoutUtility.GetRect(list[i].Label, elementStyle, GUILayout.Height(maxHeight));
				}
				else
				{
					elementRect = GUILayoutUtility.GetRect(list[i].Label, elementStyle);
				}


				bool hover = false;

				// note: we disallow interactivity if there is a dropdown box or modal window open elsewhere
				if (!OverlayUtility.IsAnyOverlayOpen)
				{
					hover = elementRect.Contains(Event.current.mousePosition);

					if (hover && hoveringOnElement != null)
					{
						hoveringOnElement(elementRect, i);
					}

					if (hover && Event.current.type == EventType.MouseDown && Event.current.clickCount == 1)
					{
						selected = i;
						if (selectCallback != null)
						{
							selectCallback(i, list[i].Label.text);
						}

						Utility.ForceGuiPassive();
						Utility.EatEvent();
					}
					else if (hover &&
					         chooseCallback != null &&
					         Event.current.type == EventType.MouseDown &&
					         Event.current.clickCount == 2)
					{
						chooseCallback(i, list[i].Label.text);

						Utility.ForceGuiPassive();
						Utility.EatEvent();
					}
					else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.UpArrow)
					{
						if (selected > 0 && (visibilityPredicate == null || selected > firstVisible))
						{
							if (visibilityPredicate != null)
							{
								do
								{
									if (selected > 0)
									{
										--selected;
									}
									else
									{
										break;
									}
								} while (!visibilityPredicate(selected));
							}
							else
							{
								--selected;
							}
						}

						Utility.EatEvent();
					}
					else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.DownArrow)
					{
						if (selected < list.Count - 1)
						{
							if (visibilityPredicate != null)
							{
								var selectedBeforeAdvancing = selected;
								do
								{
									if (selected < list.Count - 1)
									{
										++selected;
									}
									else
									{
										// did not find any visible
										selected = selectedBeforeAdvancing;
										break;
									}
								} while (!visibilityPredicate(selected));
							}
							else
							{
								++selected;
							}
						}

						Utility.EatEvent();
					}
					else if (chooseCallback != null &&
					         Event.current.type == EventType.KeyDown &&
					         Event.current.keyCode == KeyCode.Return &&
					         (i == selected))
					{
						chooseCallback(i, list[i].Label.text);

						Utility.EatEvent();
					}
				}


				if (Event.current.type == EventType.Repaint)
				{
					if (list[i].Label.image != null && !IsWhite(list[i].LabelTint))
					{
						var imageWidth = list[i].Label.image.width;
						var leftPadding = elementStyle.padding.left;

						var prevContentOffset = elementStyle.contentOffset;
						elementStyle.contentOffset = new Vector2(imageWidth + 3, 0);
						elementStyle.Draw(
							//new Rect(elementRect.x + imageWidth, elementRect.y, elementRect.width - imageWidth, elementRect.height),
							elementRect,
							list[i].Label.text, hover, i == selected && Event.current.type == EventType.MouseDown,
							i == selected, false);
						elementStyle.contentOffset = prevContentOffset;


						GUI.color = list[i].LabelTint;
						GUI.DrawTexture(
							new Rect(elementRect.x + leftPadding, elementRect.y, imageWidth, elementRect.height),
							list[i].Label.image, ScaleMode.ScaleToFit);
						GUI.color = prevColor;
					}
					else
					{
						elementStyle.Draw(elementRect, list[i].Label, hover,
							i == selected && Event.current.type == EventType.MouseDown,
							i == selected, false);
					}
				}
			}

			GUI.color = prevColor;

			return selected;
		}

		static bool IsWhite(Color c)
		{
			return c.r >= 0.9f && c.g >= 0.9f && c.b >= 0.9f && c.a >= 0.9f;
		}

		// ====================================================================================================

		public static int Draw(int selected, IList<GUIContent> list)
		{
			return Draw(selected, list, DEFAULT_LIST_ITEM_STYLE_NAME, -1, null, null);
		}

		public static int Draw(int selected, IList<GUIContent> list, GUIStyle elementStyle)
		{
			return Draw(selected, list, elementStyle, -1, null, null);
		}

		public static int Draw(int selected, IList<GUIContent> list, DoubleClickCallback onDoubleClick)
		{
			return Draw(selected, list, DEFAULT_LIST_ITEM_STYLE_NAME, -1, null, onDoubleClick);
		}

		public static int Draw(int selected, IList<GUIContent> list, GUIStyle elementStyle,
			DoubleClickCallback onDoubleClick)
		{
			return Draw(selected, list, elementStyle, -1, null, onDoubleClick);
		}

		public static int Draw(int selected,
			IList<GUIContent> list,
			SelectCallback selectCallback,
			DoubleClickCallback onDoubleClick)
		{
			return Draw(selected, list, DEFAULT_LIST_ITEM_STYLE_NAME, -1, selectCallback, onDoubleClick);
		}

		public static int Draw(int selected,
			IList<GUIContent> list,
			GUIStyle elementStyle,
			int width,
			SelectCallback selectCallback,
			DoubleClickCallback chooseCallback)
		{
			for (int i = 0; i < list.Count; ++i)
			{
				Rect elementRect;
				if (width < 0)
				{
					elementRect = GUILayoutUtility.GetRect(list[i], elementStyle);
				}
				else
				{
					elementRect = GUILayoutUtility.GetRect(list[i], elementStyle, GUILayout.Width(width));
				}

				bool hover = elementRect.Contains(Event.current.mousePosition);

				if (hover && Event.current.type == EventType.MouseDown && Event.current.clickCount == 1)
				{
					selected = i;
					if (selectCallback != null)
					{
						selectCallback(i, list[i].text);
					}

					Utility.ForceGuiPassive();
					Utility.EatEvent();
				}
				else if (hover &&
				         chooseCallback != null &&
				         Event.current.type == EventType.MouseDown &&
				         Event.current.clickCount == 2)
				{
					chooseCallback(i, list[i].text);

					Utility.ForceGuiPassive();
					Utility.EatEvent();
				}
				else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.UpArrow)
				{
					Utility.EatEvent();

					if (selected > 0)
					{
						--selected;
					}
				}
				else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.DownArrow)
				{
					Utility.EatEvent();

					if (selected < list.Count - 1)
					{
						++selected;
					}
				}
				else if (chooseCallback != null &&
				         Event.current.type == EventType.KeyDown &&
				         Event.current.keyCode == KeyCode.Return &&
				         (i == selected))
				{
					chooseCallback(i, list[i].text);

					Utility.EatEvent();
				}
				else if (Event.current.type == EventType.Repaint)
				{
					elementStyle.Draw(elementRect, list[i], hover,
						i == selected && Event.current.type == EventType.MouseDown, i == selected, false);
				}
			}

			return selected;
		}

		// ====================================================================================================

		public static (int, int, Rect) Draw(int selected, int hovered, GUIContent[] list)
		{
			return Draw(selected, hovered, list, DEFAULT_LIST_ITEM_STYLE_NAME, -1, null, null);
		}

		public static (int, int, Rect) Draw(int selected, int hovered, GUIContent[] list, GUIStyle elementStyle,
			float width)
		{
			return Draw(selected, hovered, list, elementStyle, width, null, null);
		}

		public static (int, int, Rect) Draw(int selected, int hovered, GUIContent[] list,
			DoubleClickCallback onDoubleClick)
		{
			return Draw(selected, hovered, list, DEFAULT_LIST_ITEM_STYLE_NAME, -1, null, onDoubleClick);
		}

		public static (int, int, Rect) Draw(int selected, int hovered,
			GUIContent[] list,
			SelectCallback selectCallback,
			DoubleClickCallback onDoubleClick)
		{
			return Draw(selected, hovered, list, DEFAULT_LIST_ITEM_STYLE_NAME, -1, selectCallback, onDoubleClick);
		}

		public static (int, int, Rect) Draw(int selected, int hovered,
			GUIContent[] list,
			GUIStyle elementStyle,
			float width,
			SelectCallback selectCallback,
			DoubleClickCallback chooseCallback)
		{
			if (list == null)
			{
				return (-1, -1, Rect.zero);
			}

			Rect hoveredRect = Rect.zero;
			int newHovered = (Event.current.type == EventType.Repaint || Event.current.type == EventType.Layout)
				? hovered
				: -1;

			for (int i = 0; i < list.Length; ++i)
			{
				Rect elementRect;
				if (width < 0)
				{
					elementRect = GUILayoutUtility.GetRect(list[i], elementStyle);
				}
				else
				{
					elementRect = GUILayoutUtility.GetRect(list[i], elementStyle, GUILayout.Width(width));
				}

				bool hover = elementRect.Contains(Event.current.mousePosition);

				if (hover && (Event.current.type == EventType.Repaint || Event.current.type == EventType.MouseMove))
				{
					newHovered = i;
					hoveredRect = elementRect;
				}

				if (hover && Event.current.type == EventType.MouseDown && Event.current.clickCount == 1)
				{
					selected = i;
					selectCallback?.Invoke(i, list[i].text);

					Utility.ForceGuiPassive();
					Utility.EatEvent();
				}
				else if (hover &&
				         chooseCallback != null &&
				         Event.current.type == EventType.MouseDown &&
				         Event.current.clickCount == 2)
				{
					chooseCallback(i, list[i].text);

					Utility.ForceGuiPassive();
					Utility.EatEvent();
				}
				else if (hover &&
				         chooseCallback != null &&
				         Event.current.type == EventType.MouseUp &&
				         Event.current.clickCount == 2)
				{
					Utility.ForceGuiPassive();
					Utility.EatEvent();
				}
				else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.UpArrow)
				{
					Utility.EatEvent();

					if (selected > 0)
					{
						--selected;
					}
				}
				else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.DownArrow)
				{
					Utility.EatEvent();

					if (selected < list.Length - 1)
					{
						++selected;
					}
				}
				else if (chooseCallback != null &&
				         Event.current.type == EventType.KeyDown &&
				         Event.current.keyCode == KeyCode.Return &&
				         (i == selected))
				{
					chooseCallback(i, list[i].text);

					Utility.EatEvent();
				}
				else if (Event.current.type == EventType.Repaint)
				{
					var prevBgColor = GUI.backgroundColor;
					if (list[i].tooltip.IsAllHexadecimals())
					{
						GUI.backgroundColor = list[i].tooltip.HexToColor();
					}

					elementStyle.Draw(elementRect, list[i], hover,
						i == selected && Event.current.type == EventType.MouseDown, i == selected, false);
					GUI.backgroundColor = prevBgColor;
				}
			}

			return (selected, newHovered, hoveredRect);
		}

		// ====================================================================================================

		public static int Draw(int selected, string[] list)
		{
			return Draw(selected, list, DEFAULT_LIST_ITEM_STYLE_NAME, null, null);
		}

		public static int Draw(int selected, string[] list, GUIStyle elementStyle)
		{
			return Draw(selected, list, elementStyle, null, null);
		}

		public static int Draw(int selected, string[] list, DoubleClickCallback onDoubleClick)
		{
			return Draw(selected, list, DEFAULT_LIST_ITEM_STYLE_NAME, null, onDoubleClick);
		}

		public static int Draw(int selected,
			string[] list,
			SelectCallback selectCallback,
			DoubleClickCallback onDoubleClick)
		{
			return Draw(selected, list, DEFAULT_LIST_ITEM_STYLE_NAME, selectCallback, onDoubleClick);
		}

		public static int Draw(int selected,
			string[] list,
			GUIStyle elementStyle,
			SelectCallback selectCallback,
			DoubleClickCallback chooseCallback)
		{
			GUIContent c = new GUIContent();
			for (int i = 0; i < list.Length; ++i)
			{
				c.text = list[i];
				Rect elementRect = GUILayoutUtility.GetRect(c, elementStyle);
				bool hover = elementRect.Contains(Event.current.mousePosition);
				if (hover && Event.current.type == EventType.MouseDown && Event.current.clickCount == 1)
				{
					selected = i;
					if (selectCallback != null)
					{
						selectCallback(i, list[i]);
					}

					Utility.ForceGuiPassive();
					Utility.EatEvent();
				}
				else if (hover &&
				         chooseCallback != null &&
				         Event.current.type == EventType.MouseDown &&
				         Event.current.clickCount == 2)
				{
					chooseCallback(i, list[i]);

					Utility.ForceGuiPassive();
					Utility.EatEvent();
				}
				else if (Event.current.type == EventType.Repaint)
				{
					elementStyle.Draw(elementRect, list[i], hover, false, i == selected, false);
				}
			}

			return selected;
		}
	}
}