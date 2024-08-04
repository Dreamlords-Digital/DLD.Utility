// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using DLD.Utility;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DLD.IMGUI
{
	public static class Widget
	{
		public const string LABEL_STYLE_NAME = "PropertyLabel";
		public const string LABEL_2_LINES_STYLE_NAME = "PropertyLabel2Lines";
		const string LABEL_MESSAGE_STYLE_NAME = "HelpfulMessage";
		public const string LABEL_INFO_MESSAGE_STYLE_NAME = "Label.Info";
		public const string LABEL_INFO_MEDIUM_MESSAGE_STYLE_NAME = "Label.Info.Med";

		public const string ASSET_LABEL_STYLE_NAME = "AssetMiniLabel";
		const string ASSET_LABEL_FOR_VERTICAL_STYLE_NAME = "AssetMiniLabel.Vertical";

		public const string ASSET_LABEL_BG_STYLE_NAME = "AssetMiniLabel.Bg";
		public const string ASSET_LABEL_BG_ERROR_STYLE_NAME = "AssetMiniLabel.Bg.Error";
		public const string ASSET_LABEL_TEXT_STYLE_NAME = "AssetMiniLabel.Text";

		const string LABEL_FOR_WARN_STYLE_NAME = "PropertyLabelWarn";
		const string DESCRIPTION_STYLE_NAME = "TinyLabel";
		public const string WARNING_LABEL_STYLE_NAME = "TinyLabel-Warning";
		public const string ERROR_LABEL_STYLE_NAME = "TinyLabel-Warn";
		public const string DEFAULT_TEXT_FIELD_STYLE_NAME = "TextField";
		const string HIGHLIGHTED_TEXT_FIELD_STYLE_NAME = "TextField-Highlighted";
		const string WARNING_TEXT_FIELD_STYLE_NAME = "TextField-Warn";
		public const string DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME = "TextFieldMultiLine";
		const string NON_WORD_WRAP_TEXT_FIELD_STYLE_NAME = "TextField-LongText";
		const string DEFAULT_SELECTABLE_LABEL_STYLE_NAME = "TextFieldSingleLine";
		const string DEFAULT_CHECKBOX_STYLE_NAME = "PropertyCheckbox";

		public const string RADIO_GROUP_LEFT_STYLE_NAME = "DropDownBox.MiniButtonWithText.Left";
		public const string RADIO_GROUP_MID_STYLE_NAME = "DropDownBox.MiniButtonWithText.Mid";
		public const string RADIO_GROUP_RIGHT_STYLE_NAME = "DropDownBox.MiniButtonWithText.Right";

		const string DEFAULT_COLOR_SWATCH_STYLE_NAME = "ColorSwatch";

		public const string DROP_DOWN_BOX_LABEL_STYLE_NAME = "DropDownBoxLabel";
		const string DROP_DOWN_BOX_BUTTON_STYLE_NAME = "DropDownBoxButton";

		const string PLUS_MINUS_BUTTON_STYLE_NAME = "PlusMinusButton";
		public const string DEFAULT_BG_HIDDEN_STYLE_NAME = "PopupBoxHidden";

		const int DEFAULT_MULTILINE_LABEL_HEIGHT = 50;

		public static readonly GUILayoutOption[] None = { };
		public static readonly GUILayoutOption[] NoExpand = { GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false) };
		public static readonly GUILayoutOption[] NoExpandHeight = { GUILayout.ExpandHeight(false) };
		public static readonly GUILayoutOption[] NoExpandWidth = { GUILayout.ExpandWidth(false) };
		public static readonly GUILayoutOption[] ExpandHeight = { GUILayout.ExpandHeight(true) };
		public static readonly GUILayoutOption[] ExpandWidth = { GUILayout.ExpandWidth(true) };

		const float DEBUG_TOOLBAR_BUTTON_HEIGHT = 32;

		public static readonly GUILayoutOption[] Width15LayoutOption = { GUILayout.Width(15) };
		public static readonly GUILayoutOption[] Width18LayoutOption = { GUILayout.Width(18) };
		public static readonly GUILayoutOption[] Width20LayoutOption = { GUILayout.Width(20) };
		public static readonly GUILayoutOption[] Width28LayoutOption = { GUILayout.Width(28) };
		public static readonly GUILayoutOption[] Width30LayoutOption = { GUILayout.Width(30) };
		public static readonly GUILayoutOption[] Width50LayoutOption = { GUILayout.Width(50) };
		public static readonly GUILayoutOption[] Width60LayoutOption = { GUILayout.Width(60) };
		public static readonly GUILayoutOption[] Width100LayoutOption = { GUILayout.Width(100) };
		public static readonly GUILayoutOption[] Width220LayoutOption = { GUILayout.Width(220) };
		public static readonly GUILayoutOption[] Width380LayoutOption = { GUILayout.Width(380) };
		public static readonly GUILayoutOption[] Width400LayoutOption = { GUILayout.Width(400) };

		public static readonly GUILayoutOption[] DebugButtonLayoutOption = { GUILayout.Height(DEBUG_TOOLBAR_BUTTON_HEIGHT) };
		public static readonly GUILayoutOption[] Height20LayoutOption = { GUILayout.Height(20) };
		public static readonly GUILayoutOption[] Height23LayoutOption = { GUILayout.Height(23) };
		public static readonly GUILayoutOption[] Height24LayoutOption = { GUILayout.Height(24) };
		public static readonly GUILayoutOption[] Height25LayoutOption = { GUILayout.Height(25) };
		public static readonly GUILayoutOption[] Height30LayoutOption = { GUILayout.Height(30) };
		public static readonly GUILayoutOption[] Height35LayoutOption = { GUILayout.Height(35) };
		public static readonly GUILayoutOption[] Height40LayoutOption = { GUILayout.Height(40) };
		public static readonly GUILayoutOption[] Height73LayoutOption = { GUILayout.Height(73) };
		public static readonly GUILayoutOption[] Height74LayoutOption = { GUILayout.Height(74) };
		public static readonly GUILayoutOption[] Height82LayoutOption = { GUILayout.Height(82) };
		public static readonly GUILayoutOption[] Height180LayoutOption = { GUILayout.Height(180) };
		public static readonly GUILayoutOption[] Height200ExpandLayoutOption = { GUILayout.Height(200), GUILayout.ExpandHeight(true) };
		public static readonly GUILayoutOption[] Height250ExpandLayoutOption = { GUILayout.Height(250), GUILayout.ExpandHeight(true) };

		public static readonly GUILayoutOption[] HeightAtLeast145ExpandLayoutOption = { GUILayout.MinHeight(145), GUILayout.ExpandHeight(true) };

		public static readonly GUILayoutOption[] Height100To450LayoutOption = { GUILayout.MinHeight(100), GUILayout.MaxHeight(450) };
		public static readonly GUILayoutOption[] Height30To120LayoutOption = { GUILayout.MinHeight(30), GUILayout.MaxHeight(120) };

		public static readonly GUILayoutOption[] MinWidth100LayoutOption = { GUILayout.MinWidth(100) };

		public static readonly GUILayoutOption[] MinHeight200LayoutOption = { GUILayout.MinHeight(200), GUILayout.ExpandHeight(true) };
		public static readonly GUILayoutOption[] MinHeight350LayoutOption = { GUILayout.MinHeight(350), GUILayout.ExpandHeight(true) };
		//public static readonly GUILayoutOption[] Height30To120LayoutOption = { GUILayout.MinHeight(30), GUILayout.MaxHeight(120) };
		public static readonly GUILayoutOption[] Height100To330LayoutOption = { GUILayout.MinHeight(100), GUILayout.MaxHeight(330), GUILayout.ExpandHeight(true) };

		public static readonly GUILayoutOption[] Width50Height30LayoutOption = { GUILayout.Width(50), GUILayout.Height(30) };
		const int ITEM_SPACING = 20;
		const int RELATED_ITEM_SPACING = 10;

		public static void DrawItemSpacing()
		{
			GUILayout.Space(ITEM_SPACING);
		}

		public static void DrawRelatedItemSpacing()
		{
			GUILayout.Space(RELATED_ITEM_SPACING);
		}

		// ================================================================================================

		public static GUISkin LoadGuiSkin(string resourcesPath)
		{
#if UNITY_EDITOR
			return EditorGUIUtility.Load(resourcesPath) as GUISkin;
#else
			return Resources.Load(resourcesPath) as GUISkin;
#endif
		}

		// ================================================================================================


		const string SECTION_HEADER = "Header2";

		public static void DrawSectionHeader(string title)
		{
			if (string.IsNullOrEmpty(title))
			{
				return;
			}
			GUILayout.Label(title, SECTION_HEADER);
		}

		public static void DrawMessage(string label, params GUILayoutOption[] options)
		{
			DrawLabel(label, LABEL_MESSAGE_STYLE_NAME, options);
		}

		public static void DrawMessage(string label)
		{
			DrawLabel(label, LABEL_MESSAGE_STYLE_NAME);
		}

		public static void DrawInfoMessage(string label, params GUILayoutOption[] options)
		{
			DrawLabel(label, LABEL_INFO_MESSAGE_STYLE_NAME, options);
		}

		public static void DrawInfoMessage(string label)
		{
			DrawLabel(label, LABEL_INFO_MESSAGE_STYLE_NAME);
		}

		public static void DrawMediumInfoMessage(string label)
		{
			DrawLabel(label, LABEL_INFO_MEDIUM_MESSAGE_STYLE_NAME);
		}

		public static void DrawLabel(string label, params GUILayoutOption[] options)
		{
			DrawLabel(label, LABEL_STYLE_NAME, options);
		}

		public static void DrawLabel(string label)
		{
			DrawLabel(label, LABEL_STYLE_NAME);
		}

		public static void DrawLabel2Lines(string label)
		{
			DrawLabel(label, LABEL_2_LINES_STYLE_NAME);
		}

		public static void DrawLabel(string label, GUIStyle labelStyle)
		{
			if (!string.IsNullOrEmpty(label))
			{
				GUILayout.Label(label, labelStyle);
			}
		}

		public static void DrawLabel(GUIContent label)
		{
			GUILayout.Label(label, LABEL_STYLE_NAME);
		}

		public static void DrawLabel(GUIContent label, GUIStyle labelStyle)
		{
			if (!string.IsNullOrEmpty(label.text) || label.image != null)
			{
				GUILayout.Label(label, labelStyle);
			}
		}

		public static void DrawLabel(string label, GUIStyle labelStyle, params GUILayoutOption[] options)
		{
			if (!string.IsNullOrEmpty(label))
			{
				GUILayout.Label(label, labelStyle, options);
			}
		}

		public static void DrawDescription(string description, params GUILayoutOption[] options)
		{
			if (!string.IsNullOrEmpty(description))
			{
				GUILayout.Label(description, DESCRIPTION_STYLE_NAME, options);
			}
		}

		public static void DrawDescription(string description)
		{
			if (!string.IsNullOrEmpty(description))
			{
				GUILayout.Label(description, DESCRIPTION_STYLE_NAME);
			}
		}

		public static void DrawLabelWithDescription(string label, string description, params GUILayoutOption[] options)
		{
			if (string.IsNullOrEmpty(description))
			{
				DrawLabel(label, options);
			}
			else
			{
				GUILayout.BeginVertical();
				DrawLabel(label, options);
				DrawDescription(description, options);
				GUILayout.EndVertical();
			}
		}

		public static void DrawLabelWithDescription(string label, string description)
		{
			if (string.IsNullOrEmpty(description))
			{
				DrawLabel(label);
			}
			else
			{
				GUILayout.BeginVertical();
				DrawLabel(label);
				DrawDescription(description);
				GUILayout.EndVertical();
			}
		}

		public static void DrawLabelWithDescription(GUIContent label, string description)
		{
			if (string.IsNullOrEmpty(description))
			{
				DrawLabel(label);
			}
			else
			{
				GUILayout.BeginVertical();
				DrawLabel(label);
				DrawDescription(description);
				GUILayout.EndVertical();
			}
		}

		public static void DrawLabelWithDescriptionOneLine(string label, string description, params GUILayoutOption[] options)
		{
			GUILayout.BeginHorizontal();
			DrawLabel(label, options);
			DrawDescription(description, options);
			GUILayout.EndHorizontal();
		}

		public static void DrawLabelWithDescriptionOneLine(string label, string description)
		{
			GUILayout.BeginHorizontal();
			DrawLabel(label);
			DrawDescription(description);
			GUILayout.EndHorizontal();
		}

		public static void DrawLabelWithValidation(string label, Func<string> getWarningMessages,
			params GUILayoutOption[] options)
		{
			DrawLabel(label, LABEL_STYLE_NAME, options);

			string warningMessage = getWarningMessages();
			bool validInput = string.IsNullOrEmpty(warningMessage);

			if (!validInput)
			{
				DrawErrorMessage(warningMessage);
			}
		}

		public static void DrawLabelWithValidation(string label, Func<string> getWarningMessages)
		{
			DrawLabel(label, LABEL_STYLE_NAME);

			string warningMessage = getWarningMessages();
			bool validInput = string.IsNullOrEmpty(warningMessage);

			if (!validInput)
			{
				DrawErrorMessage(warningMessage);
			}
		}

		public static void DrawLabelWithValidationHorizontal(string label, Func<string> getWarningMessages,
			params GUILayoutOption[] options)
		{
			GUILayout.BeginHorizontal();

			DrawLabel(label, LABEL_FOR_WARN_STYLE_NAME, options);

			string warningMessage = getWarningMessages();
			bool validInput = string.IsNullOrEmpty(warningMessage);

			if (!validInput)
			{
				DrawErrorMessage(warningMessage);
			}
			GUILayout.EndHorizontal();
		}

		public static void DrawLabelWithValidationHorizontal(string label, Func<string> getWarningMessages)
		{
			GUILayout.BeginHorizontal();

			DrawLabel(label, LABEL_FOR_WARN_STYLE_NAME);

			string warningMessage = getWarningMessages();
			bool validInput = string.IsNullOrEmpty(warningMessage);

			if (!validInput)
			{
				DrawErrorMessage(warningMessage);
			}

			GUILayout.EndHorizontal();
		}

		public static void DrawErrorMessage(string message)
		{
			GUILayout.Label(message, ERROR_LABEL_STYLE_NAME);
		}

		public static void DrawWarningMessage(string message)
		{
			GUILayout.Label(message, WARNING_LABEL_STYLE_NAME);
		}


		// ================================================================================================

		public static bool Button(GUIContent label, bool enabled, GUIStyle style, params GUILayoutOption[] options)
		{
			if (enabled)
			{
				return GUILayout.Button(label, style, options);
			}

			GUILayout.Label(label, style, options);
			return false;
		}

		public static bool Button(GUIContent label, bool enabled, GUIStyle style)
		{
			if (enabled)
			{
				return GUILayout.Button(label, style);
			}

			GUILayout.Label(label, style);
			return false;
		}

		// ================================================================================================

		public static T DrawEnumHorizontalRadioGroup<T>(GUIContent label, T currentValue,
			T choice1, GUIContent choice1Label, T choice2, GUIContent choice2Label)
		{
			bool currentValueIs1 = EqualityComparer<T>.Default.Equals(currentValue, choice1);
			bool currentValueIs2 = EqualityComparer<T>.Default.Equals(currentValue, choice2);

			GUILayout.BeginHorizontal();
			DrawDropDownBoxLabel(label);
			bool pressedChoice1 = GUILayout.Toggle(currentValueIs1, choice1Label, RADIO_GROUP_LEFT_STYLE_NAME);
			bool pressedChoice2 = GUILayout.Toggle(currentValueIs2, choice2Label, RADIO_GROUP_RIGHT_STYLE_NAME);
			GUILayout.EndHorizontal();

			if (pressedChoice1 && !currentValueIs1)
			{
				return choice1;
			}
			else if (pressedChoice2 && !currentValueIs2)
			{
				return choice2;
			}

			return currentValue;
		}

		public static T DrawEnumHorizontalRadioGroup<T>(GUIContent label, T currentValue,
			T choice1, GUIContent choice1Label, T choice2, GUIContent choice2Label, T choice3, GUIContent choice3Label)
		{
			bool currentValueIs1 = EqualityComparer<T>.Default.Equals(currentValue, choice1);
			bool currentValueIs2 = EqualityComparer<T>.Default.Equals(currentValue, choice2);
			bool currentValueIs3 = EqualityComparer<T>.Default.Equals(currentValue, choice3);

			GUILayout.BeginHorizontal();
			DrawDropDownBoxLabel(label);
			bool pressedChoice1 = GUILayout.Toggle(currentValueIs1, choice1Label, RADIO_GROUP_LEFT_STYLE_NAME);
			bool pressedChoice2 = GUILayout.Toggle(currentValueIs2, choice2Label, RADIO_GROUP_MID_STYLE_NAME);
			bool pressedChoice3 = GUILayout.Toggle(currentValueIs3, choice3Label, RADIO_GROUP_RIGHT_STYLE_NAME);
			GUILayout.EndHorizontal();

			if (pressedChoice1 && !currentValueIs1)
			{
				return choice1;
			}
			else if (pressedChoice2 && !currentValueIs2)
			{
				return choice2;
			}
			else if (pressedChoice3 && !currentValueIs3)
			{
				return choice3;
			}

			return currentValue;
		}

		public static T DrawEnumHorizontalRadioGroup<T>(string label, T currentValue,
			T choice1, string choice1Label, T choice2, string choice2Label)
		{
			bool currentValueIs1 = EqualityComparer<T>.Default.Equals(currentValue, choice1);
			bool currentValueIs2 = EqualityComparer<T>.Default.Equals(currentValue, choice2);

			GUILayout.BeginHorizontal();
			if (label != null && label.Contains("\n"))
			{
				DrawLabel2Lines(label);
			}
			else
			{
				DrawDropDownBoxLabel(label);
			}

			bool pressedChoice1 = GUILayout.Toggle(currentValueIs1, choice1Label, RADIO_GROUP_LEFT_STYLE_NAME);
			bool pressedChoice2 = GUILayout.Toggle(currentValueIs2, choice2Label, RADIO_GROUP_RIGHT_STYLE_NAME);
			GUILayout.EndHorizontal();

			if (pressedChoice1 && !currentValueIs1)
			{
				return choice1;
			}
			else if (pressedChoice2 && !currentValueIs2)
			{
				return choice2;
			}

			return currentValue;
		}

		public static T DrawEnumHorizontalRadioGroup<T>(string label, T currentValue,
			T choice1, string choice1Label, T choice2, string choice2Label, T choice3, string choice3Label)
		{
			bool currentValueIs1 = EqualityComparer<T>.Default.Equals(currentValue, choice1);
			bool currentValueIs2 = EqualityComparer<T>.Default.Equals(currentValue, choice2);
			bool currentValueIs3 = EqualityComparer<T>.Default.Equals(currentValue, choice3);

			GUILayout.BeginHorizontal();
			if (label != null && label.Contains("\n"))
			{
				DrawLabel2Lines(label);
			}
			else
			{
				DrawDropDownBoxLabel(label);
			}

			bool pressedChoice1 = GUILayout.Toggle(currentValueIs1, choice1Label, RADIO_GROUP_LEFT_STYLE_NAME);
			bool pressedChoice2 = GUILayout.Toggle(currentValueIs2, choice2Label, RADIO_GROUP_MID_STYLE_NAME);
			bool pressedChoice3 = GUILayout.Toggle(currentValueIs3, choice3Label, RADIO_GROUP_RIGHT_STYLE_NAME);
			GUILayout.EndHorizontal();

			if (pressedChoice1 && !currentValueIs1)
			{
				return choice1;
			}
			else if (pressedChoice2 && !currentValueIs2)
			{
				return choice2;
			}
			else if (pressedChoice3 && !currentValueIs3)
			{
				return choice3;
			}

			return currentValue;
		}

		// -----------------------------------------------------

		public static T DrawEnumHorizontalRadioGroup<T>(string label, T currentValue,
			T choice1, string choice1Label,
			T choice2, string choice2Label,
			T choice3, string choice3Label,
			T choice4, string choice4Label,
			T choice5, string choice5Label)
		{
			bool currentValueIs1 = EqualityComparer<T>.Default.Equals(currentValue, choice1);
			bool currentValueIs2 = EqualityComparer<T>.Default.Equals(currentValue, choice2);
			bool currentValueIs3 = EqualityComparer<T>.Default.Equals(currentValue, choice3);
			bool currentValueIs4 = EqualityComparer<T>.Default.Equals(currentValue, choice4);
			bool currentValueIs5 = EqualityComparer<T>.Default.Equals(currentValue, choice5);

			GUILayout.BeginHorizontal();
			if (label != null && label.Contains("\n"))
			{
				DrawLabel2Lines(label);
			}
			else
			{
				DrawDropDownBoxLabel(label);
			}

			bool pressedChoice1 = GUILayout.Toggle(currentValueIs1, choice1Label, RADIO_GROUP_LEFT_STYLE_NAME);
			bool pressedChoice2 = GUILayout.Toggle(currentValueIs2, choice2Label, RADIO_GROUP_MID_STYLE_NAME);
			bool pressedChoice3 = GUILayout.Toggle(currentValueIs3, choice3Label, RADIO_GROUP_MID_STYLE_NAME);
			bool pressedChoice4 = GUILayout.Toggle(currentValueIs4, choice4Label, RADIO_GROUP_MID_STYLE_NAME);
			bool pressedChoice5 = GUILayout.Toggle(currentValueIs5, choice5Label, RADIO_GROUP_RIGHT_STYLE_NAME);
			GUILayout.EndHorizontal();

			if (pressedChoice1 && !currentValueIs1)
			{
				return choice1;
			}
			else if (pressedChoice2 && !currentValueIs2)
			{
				return choice2;
			}
			else if (pressedChoice3 && !currentValueIs3)
			{
				return choice3;
			}
			else if (pressedChoice4 && !currentValueIs4)
			{
				return choice4;
			}
			else if (pressedChoice5 && !currentValueIs5)
			{
				return choice5;
			}

			return currentValue;
		}

		public static T DrawEnumHorizontalRadioGroup<T>(string label, T currentValue,
			T choice1, string choice1Label,
			T choice2, string choice2Label,
			T choice3, string choice3Label,
			T choice4, string choice4Label)
		{
			bool currentValueIs1 = EqualityComparer<T>.Default.Equals(currentValue, choice1);
			bool currentValueIs2 = EqualityComparer<T>.Default.Equals(currentValue, choice2);
			bool currentValueIs3 = EqualityComparer<T>.Default.Equals(currentValue, choice3);
			bool currentValueIs4 = EqualityComparer<T>.Default.Equals(currentValue, choice4);

			GUILayout.BeginHorizontal();
			if (label != null && label.Contains("\n"))
			{
				DrawLabel2Lines(label);
			}
			else
			{
				DrawDropDownBoxLabel(label);
			}

			bool pressedChoice1 = GUILayout.Toggle(currentValueIs1, choice1Label, RADIO_GROUP_LEFT_STYLE_NAME);
			bool pressedChoice2 = GUILayout.Toggle(currentValueIs2, choice2Label, RADIO_GROUP_MID_STYLE_NAME);
			bool pressedChoice3 = GUILayout.Toggle(currentValueIs3, choice3Label, RADIO_GROUP_MID_STYLE_NAME);
			bool pressedChoice4 = GUILayout.Toggle(currentValueIs4, choice4Label, RADIO_GROUP_RIGHT_STYLE_NAME);
			GUILayout.EndHorizontal();

			if (pressedChoice1 && !currentValueIs1)
			{
				return choice1;
			}
			else if (pressedChoice2 && !currentValueIs2)
			{
				return choice2;
			}
			else if (pressedChoice3 && !currentValueIs3)
			{
				return choice3;
			}
			else if (pressedChoice4 && !currentValueIs4)
			{
				return choice4;
			}

			return currentValue;
		}

		public static T DrawEnumRadioGroup<T>(string label, T currentValue,
			T choice1, string choice1Label,
			T choice2, string choice2Label,
			T choice3, string choice3Label, bool rightAlign)
		{
			bool currentValueIs1 = EqualityComparer<T>.Default.Equals(currentValue, choice1);
			bool currentValueIs2 = EqualityComparer<T>.Default.Equals(currentValue, choice2);
			bool currentValueIs3 = EqualityComparer<T>.Default.Equals(currentValue, choice3);

			DrawLabel(label, "RadioGroupLabel");
			GUILayout.BeginHorizontal();
			if (rightAlign)
			{
				GUILayout.FlexibleSpace();
			}
			else
			{
				GUILayout.Space(10);
			}
			bool pressedChoice1 = GUILayout.Toggle(currentValueIs1, choice1Label, RADIO_GROUP_LEFT_STYLE_NAME);
			bool pressedChoice2 = GUILayout.Toggle(currentValueIs2, choice2Label, RADIO_GROUP_MID_STYLE_NAME);
			bool pressedChoice3 = GUILayout.Toggle(currentValueIs3, choice3Label, RADIO_GROUP_RIGHT_STYLE_NAME);
			GUILayout.EndHorizontal();

			if (pressedChoice1 && !currentValueIs1)
			{
				return choice1;
			}
			else if (pressedChoice2 && !currentValueIs2)
			{
				return choice2;
			}
			else if (pressedChoice3 && !currentValueIs3)
			{
				return choice3;
			}

			return currentValue;
		}

		public static T DrawEnumRadioGroup<T>(string label, T currentValue,
			T choice1, string choice1Label,
			T choice2, string choice2Label,
			T choice3, string choice3Label,
			T choice4, string choice4Label)
		{
			bool currentValueIs1 = EqualityComparer<T>.Default.Equals(currentValue, choice1);
			bool currentValueIs2 = EqualityComparer<T>.Default.Equals(currentValue, choice2);
			bool currentValueIs3 = EqualityComparer<T>.Default.Equals(currentValue, choice3);
			bool currentValueIs4 = EqualityComparer<T>.Default.Equals(currentValue, choice4);

			DrawLabel(label, "RadioGroupLabel");
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			bool pressedChoice1 = GUILayout.Toggle(currentValueIs1, choice1Label, RADIO_GROUP_LEFT_STYLE_NAME);
			bool pressedChoice2 = GUILayout.Toggle(currentValueIs2, choice2Label, RADIO_GROUP_MID_STYLE_NAME);
			bool pressedChoice3 = GUILayout.Toggle(currentValueIs3, choice3Label, RADIO_GROUP_MID_STYLE_NAME);
			bool pressedChoice4 = GUILayout.Toggle(currentValueIs4, choice4Label, RADIO_GROUP_RIGHT_STYLE_NAME);
			GUILayout.EndHorizontal();

			if (pressedChoice1 && !currentValueIs1)
			{
				return choice1;
			}
			else if (pressedChoice2 && !currentValueIs2)
			{
				return choice2;
			}
			else if (pressedChoice3 && !currentValueIs3)
			{
				return choice3;
			}
			else if (pressedChoice4 && !currentValueIs4)
			{
				return choice4;
			}

			return currentValue;
		}

		// ================================================================================================

		public static string DrawTextField(Rect contentRect, string inputtedText)
		{
			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = string.Empty;
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inputtedText = EditorGUI.TextField(contentRect, inputtedText, GUI.skin.GetStyle(DEFAULT_TEXT_FIELD_STYLE_NAME));
			}
			else
			{
#endif
				inputtedText = GUI.TextField(contentRect, inputtedText, GUI.skin.GetStyle(DEFAULT_TEXT_FIELD_STYLE_NAME));
#if UNITY_EDITOR
			}
#endif
			return inputtedText;
		}

		public static void DrawNonInteractiveTextField(Rect contentRect, string inputtedText)
		{
			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = string.Empty;
			}

			GUI.Label(contentRect, inputtedText, GUI.skin.GetStyle(DEFAULT_TEXT_FIELD_STYLE_NAME));
		}

		public static string DrawTextField(Rect contentRect, string label, string description, string inputtedText)
		{
			DrawLabelWithDescription(label, description);
			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = string.Empty;
			}
			inputtedText = DrawTextField(contentRect, inputtedText);
			return inputtedText;
		}



		public static string DrawTextField(string inputtedText, params GUILayoutOption[] options)
		{
			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = string.Empty;
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inputtedText = EditorGUILayout.TextField(string.Empty, inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME, options);
			}
			else
			{
#endif
				inputtedText = GUILayout.TextField(inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME, options);
#if UNITY_EDITOR
			}
#endif

			return inputtedText;
		}

		public static string DrawTextField(string inputtedText)
		{
			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = string.Empty;
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inputtedText =
					EditorGUILayout.TextField(string.Empty, inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME);
			}
			else
			{
#endif
				inputtedText = GUILayout.TextField(inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif

			return inputtedText;
		}

		public static string DrawNonWordWrapTextField(string inputtedText)
		{
			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = string.Empty;
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inputtedText =
					EditorGUILayout.TextField(string.Empty, inputtedText, NON_WORD_WRAP_TEXT_FIELD_STYLE_NAME);
			}
			else
			{
#endif
				inputtedText = GUILayout.TextField(inputtedText, NON_WORD_WRAP_TEXT_FIELD_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif

			return inputtedText;
		}

		public static string DrawTextField(string label, string description, string inputtedText,
			params GUILayoutOption[] options)
		{
			DrawLabelWithDescription(label, description, GUILayout.ExpandWidth(false));

			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = string.Empty;
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inputtedText =
					EditorGUILayout.TextField(string.Empty, inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME, options);
			}
			else
			{
#endif
				inputtedText = GUILayout.TextField(inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME, options);
#if UNITY_EDITOR
			}
#endif

			return inputtedText;
		}

		public static string DrawTextField(string label, string description, string inputtedText)
		{
			DrawLabelWithDescription(label, description);
			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = string.Empty;
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inputtedText = EditorGUILayout.TextField(string.Empty, inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME);
			}
			else
			{
#endif
				inputtedText = GUILayout.TextField(inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif
			return inputtedText;
		}

		public static string DrawTextField(GUIContent label, string description, string inputtedText)
		{
			DrawLabelWithDescription(label, description);
			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = string.Empty;
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inputtedText = EditorGUILayout.TextField(string.Empty, inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME);
			}
			else
			{
#endif
				inputtedText = GUILayout.TextField(inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif
			return inputtedText;
		}

		public static string DrawTextFieldHorizontal(string label, string description, string inputtedText, params GUILayoutOption[] options)
		{
			var hasDescription = !string.IsNullOrEmpty(description);
			if (hasDescription)
			{
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				DrawLabel(label);
			}
			else
			{
				GUILayout.BeginHorizontal();
				DrawLabel(label);
			}

			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = "";
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inputtedText = EditorGUILayout.TextField(string.Empty, inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME, options);
			}
			else
			{
#endif
				inputtedText = GUILayout.TextField(inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME, options);
#if UNITY_EDITOR
			}
#endif
			GUILayout.EndHorizontal();

			if (hasDescription)
			{
				DrawDescription(description, options);
				GUILayout.EndVertical();
			}

			return inputtedText;
		}

		public static string DrawTextFieldHorizontal(string label, string description, string inputtedText)
		{
			var hasDescription = !string.IsNullOrEmpty(description);
			if (hasDescription)
			{
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				DrawLabel(label);
			}
			else
			{
				GUILayout.BeginHorizontal();
				DrawLabel(label);
			}

			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = "";
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inputtedText =
					EditorGUILayout.TextField(string.Empty, inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME);
			}
			else
			{
#endif
				inputtedText = GUILayout.TextField(inputtedText, DEFAULT_TEXT_FIELD_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif
			GUILayout.EndHorizontal();

			if (hasDescription)
			{
				DrawDescription(description);
				GUILayout.EndVertical();
			}

			return inputtedText;
		}

		public static string DrawTextFieldWithValidation(string label, string description, Func<string, string> getWarningMessages, string inputtedText)
		{
			DrawLabelWithDescription(label, description);

			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = string.Empty;
			}

			string warningMessage = getWarningMessages(inputtedText);
			bool validInput = string.IsNullOrEmpty(warningMessage);

#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inputtedText = EditorGUILayout.TextField(string.Empty, inputtedText, validInput ? DEFAULT_TEXT_FIELD_STYLE_NAME : WARNING_TEXT_FIELD_STYLE_NAME);
			}
			else
			{
#endif
				inputtedText = GUILayout.TextField(inputtedText, validInput ? DEFAULT_TEXT_FIELD_STYLE_NAME : WARNING_TEXT_FIELD_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif

			if (!validInput)
			{
				GUILayout.Label(warningMessage, ERROR_LABEL_STYLE_NAME);
			}

			return inputtedText;
		}

		public static string DrawMultiLineTextField(string label, string description, string inputtedText)
		{
			return DrawMultiLineTextField(label, description, inputtedText, DEFAULT_MULTILINE_LABEL_HEIGHT);
		}

		public static string DrawMultiLineTextField(string label, string description, string inputtedText, float height)
		{
			DrawLabelWithDescription(label, description);
			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = "";
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inputtedText = EditorGUILayout.TextArea(inputtedText, GUI.skin.GetStyle(DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME), GUILayout.Height(height));
			}
			else
			{
#endif
				inputtedText = GUILayout.TextArea(inputtedText, DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME, GUILayout.Height(height));
#if UNITY_EDITOR
			}
#endif
			return inputtedText;
		}

		public static string DrawMultiLineTextFieldFillHeight(string label, string description, string inputtedText)
		{
			DrawLabelWithDescription(label, description);
			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = "";
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inputtedText = EditorGUILayout.TextArea(inputtedText, GUI.skin.GetStyle(DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME), ExpandHeight);
			}
			else
			{
#endif
				inputtedText = GUILayout.TextArea(inputtedText, DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME, ExpandHeight);
#if UNITY_EDITOR
			}
#endif
			return inputtedText;
		}

		public static string DrawMultiLineTextField(string inputtedText, float height)
		{
			if (string.IsNullOrEmpty(inputtedText))
			{
				inputtedText = "";
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inputtedText = EditorGUILayout.TextArea(inputtedText, GUI.skin.GetStyle(DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME), GUILayout.Height(height));
			}
			else
			{
#endif
				inputtedText = GUILayout.TextArea(inputtedText, DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME, GUILayout.Height(height));
#if UNITY_EDITOR
			}
#endif
			return inputtedText;
		}

		public static string DrawMultiLineTextField(Rect rect, string inputtedText)
		{
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				return EditorGUI.TextArea(rect, inputtedText, GUI.skin.GetStyle(DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME));
			}
			else
			{
#endif
				return GUI.TextArea(rect, inputtedText, GUI.skin.GetStyle(DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME));
#if UNITY_EDITOR
			}
#endif
		}

		public static void DrawSelectableLabel(string val, params GUILayoutOption[] options)
		{
			if (string.IsNullOrEmpty(val))
			{
				val = "";
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				//EditorGUILayout.TextField(string.Empty, val, DEFAULT_SELECTABLE_LABEL_STYLE_NAME, options);
				EditorGUILayout.SelectableLabel(val, GUI.skin.GetStyle(DEFAULT_SELECTABLE_LABEL_STYLE_NAME), options);
			}
			else
			{
#endif
				GUILayout.TextField(val, DEFAULT_SELECTABLE_LABEL_STYLE_NAME, options);
#if UNITY_EDITOR
			}
#endif
		}

		public static void DrawSelectableLabel(string val)
		{
			if (string.IsNullOrEmpty(val))
			{
				val = "";
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				//EditorGUILayout.TextField(string.Empty, val, DEFAULT_SELECTABLE_LABEL_STYLE_NAME, options);
				EditorGUILayout.SelectableLabel(val, GUI.skin.GetStyle(DEFAULT_SELECTABLE_LABEL_STYLE_NAME));
			}
			else
			{
#endif
				GUILayout.TextField(val, DEFAULT_SELECTABLE_LABEL_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif
		}

		public static void DrawSelectableLabel(string label, string description, string val)
		{
			DrawLabelWithDescription(label, description);
			if (string.IsNullOrEmpty(val))
			{
				val = "";
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				//EditorGUILayout.TextField(string.Empty, val, DEFAULT_SELECTABLE_LABEL_STYLE_NAME);
				EditorGUILayout.SelectableLabel(val, GUI.skin.GetStyle(DEFAULT_SELECTABLE_LABEL_STYLE_NAME));
			}
			else
			{
#endif
				GUILayout.TextField(val, DEFAULT_SELECTABLE_LABEL_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif
		}

		public static void DrawSelectableLabelWithCopyButton(string label, string description, string val, GUIContent copyButtonGUIContent)
		{
			DrawLabelWithDescription(label, description);
			if (string.IsNullOrEmpty(val))
			{
				val = "";
			}

			GUILayout.BeginHorizontal();
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				EditorGUILayout.SelectableLabel(val, GUI.skin.GetStyle(DEFAULT_SELECTABLE_LABEL_STYLE_NAME));
			}
			else
			{
#endif
				GUILayout.TextField(val, DEFAULT_SELECTABLE_LABEL_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif

			if (GUILayout.Button(copyButtonGUIContent, NoExpand))
			{
				GUIUtility.systemCopyBuffer = val;
			}

			GUILayout.EndHorizontal();
		}

		public static void DrawSelectableLabel(GUIContent label, string description, string val)
		{
			DrawLabelWithDescription(label, description);
			if (string.IsNullOrEmpty(val))
			{
				val = "";
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				//EditorGUILayout.TextField(string.Empty, val, DEFAULT_SELECTABLE_LABEL_STYLE_NAME);
				EditorGUILayout.SelectableLabel(val, GUI.skin.GetStyle(DEFAULT_SELECTABLE_LABEL_STYLE_NAME));
			}
			else
			{
#endif
				GUILayout.TextField(val, DEFAULT_SELECTABLE_LABEL_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif
		}


		public static void DrawSelectableLabel(string label, string val)
		{
			DrawLabel(label);
			if (string.IsNullOrEmpty(val))
			{
				val = "";
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				//EditorGUILayout.TextField(string.Empty, val, DEFAULT_SELECTABLE_LABEL_STYLE_NAME);
				EditorGUILayout.SelectableLabel(val, GUI.skin.GetStyle(DEFAULT_SELECTABLE_LABEL_STYLE_NAME));
			}
			else
			{
#endif
				GUILayout.TextField(val, DEFAULT_SELECTABLE_LABEL_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif
		}

		public static void DrawSelectableMultilineLabel(string label, string description, string val, params GUILayoutOption[] options)
		{
			DrawLabelWithDescription(label, description);
			if (string.IsNullOrEmpty(val))
			{
				val = "";
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				EditorGUILayout.TextArea(val, DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME, options);
			}
			else
			{
#endif
				GUILayout.TextArea(val, DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME, options);
#if UNITY_EDITOR
			}
#endif
		}

		public static void DrawSelectableMultilineLabel(string label, string description, string val)
		{
			DrawLabelWithDescription(label, description);
			if (string.IsNullOrEmpty(val))
			{
				val = "";
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				EditorGUILayout.TextArea(val, DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME);
			}
			else
			{
#endif
				GUILayout.TextArea(val, DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif
		}

		public static void DrawNonSelectableLabel(string label, string description, string val, params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			DrawLabel(label, options);
			DrawLabel(val, options);
			GUILayout.EndHorizontal();
			DrawDescription(description, options);
			GUILayout.EndVertical();
		}

		public static void DrawNonSelectableLabel(string label, string description, string val)
		{
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			DrawLabel(label);
			DrawLabel(val);
			GUILayout.EndHorizontal();
			DrawDescription(description);
			GUILayout.EndVertical();
		}

		public static void DrawNonSelectableAssetLabel(string label, string description, string val, params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical();
			DrawLabel(label, options);
			DrawLabel(val, ASSET_LABEL_FOR_VERTICAL_STYLE_NAME, options);
			DrawDescription(description, options);
			GUILayout.EndVertical();
		}

		public static void DrawNonSelectableAssetLabel(string label, string description, string val, string displayedValIfNull = "<i>Null</i>")
		{
			GUILayout.BeginVertical();
			DrawLabel(label);
			DrawLabel(string.IsNullOrEmpty(val) ? displayedValIfNull : val, ASSET_LABEL_FOR_VERTICAL_STYLE_NAME);
			DrawDescription(description);
			GUILayout.EndVertical();
		}

		public static void DrawNonSelectableAssetLabel(string val)
		{
			DrawLabel(val, ASSET_LABEL_STYLE_NAME);
		}

		public static void DrawNonSelectableAssetLabel(GUIContent val)
		{
			GUILayout.Label(val, ASSET_LABEL_STYLE_NAME);
		}

		public static void DrawNonSelectableAssetLabel(string label, string description, GUIContent val, params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical();
			DrawLabel(label, options);

			if (!string.IsNullOrEmpty(val.text))
			{
				GUILayout.Label(val, ASSET_LABEL_FOR_VERTICAL_STYLE_NAME);
			}

			DrawDescription(description, options);
			GUILayout.EndVertical();
		}

		public static void DrawNonSelectableAssetLabel(GUIContent label, string description, GUIContent val, params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical();
			GUILayout.Label(label, LABEL_STYLE_NAME, options);

			if (!string.IsNullOrEmpty(val.text))
			{
				GUILayout.Label(val, ASSET_LABEL_FOR_VERTICAL_STYLE_NAME);
			}

			DrawDescription(description, options);
			GUILayout.EndVertical();
		}

		public static void DrawNonSelectableAssetLabelHorizontal(string label, string description, GUIContent val, params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			DrawLabel(label, options);

			if (!string.IsNullOrEmpty(val.text))
			{
				GUILayout.Label(val, ASSET_LABEL_STYLE_NAME);
			}
			GUILayout.EndHorizontal();

			DrawDescription(description, options);
			GUILayout.EndVertical();
		}

		public static void DrawNonSelectableAssetLabelHorizontal(string label, string description, GUIContent val)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			DrawLabel(label);

			if (!string.IsNullOrEmpty(val.text))
			{
				GUILayout.Label(val, ASSET_LABEL_STYLE_NAME);
			}

			GUILayout.EndHorizontal();

			DrawDescription(description);
			GUILayout.EndVertical();
		}

		public static void DrawNonSelectableAssetLabelHorizontal(GUIContent label, string description, GUIContent val)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			DrawLabel(label);

			if (!string.IsNullOrEmpty(val.text))
			{
				GUILayout.Label(val, ASSET_LABEL_STYLE_NAME);
			}

			GUILayout.EndHorizontal();

			DrawDescription(description);
			GUILayout.EndVertical();
		}

		public static void DrawNonSelectableAssetLabelHorizontal(string label, GUIContent val)
		{
			GUILayout.BeginHorizontal();

			if (!string.IsNullOrEmpty(label))
			{
				GUILayout.Label(label, "AssetMiniLabel.Label");
			}

			if (!string.IsNullOrEmpty(val.text))
			{
				GUILayout.Label(val, ASSET_LABEL_STYLE_NAME);
			}

			GUILayout.EndHorizontal();
		}

		public static void DrawNonSelectableAssetLabelHorizontal(string label, GUIContent val,
			params GUILayoutOption[] options)
		{
			DrawNonSelectableAssetLabelHorizontal(label, string.Empty, val, options);
		}

		// --------------

		public static void DrawHoverableNonSelectableAssetLabelHorizontal(string label, string description, GUIContent val, ref Rect assetRect, params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			DrawLabel(label, options);

			if (!string.IsNullOrEmpty(val.text))
			{
				GUILayout.Label(val, ASSET_LABEL_STYLE_NAME);
				if (Event.current.type == EventType.Repaint)
				{
					assetRect = GUILayoutUtility.GetLastRect();
				}
			}
			GUILayout.EndHorizontal();

			DrawDescription(description, options);
			GUILayout.EndVertical();
		}

		public static void DrawHoverableNonSelectableAssetLabelHorizontal(string label, string description,
			GUIContent val, ref Rect assetRect)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			DrawLabel(label);

			if (!string.IsNullOrEmpty(val.text))
			{
				GUILayout.Label(val, ASSET_LABEL_STYLE_NAME);
				if (Event.current.type == EventType.Repaint)
				{
					assetRect = GUILayoutUtility.GetLastRect();
				}
			}

			GUILayout.EndHorizontal();

			DrawDescription(description);
			GUILayout.EndVertical();
		}

		public static void DrawHoverableNonSelectableAssetLabelHorizontal(string label, GUIContent val, ref Rect assetRect,
			params GUILayoutOption[] options)
		{
			DrawHoverableNonSelectableAssetLabelHorizontal(label, string.Empty, val, ref assetRect, options);
		}

		public static void DrawHoverableNonSelectableAssetLabelHorizontal(string label, GUIContent val,
			ref Rect assetRect)
		{
			DrawHoverableNonSelectableAssetLabelHorizontal(label, string.Empty, val, ref assetRect);
		}

		public static void DrawNonSelectableAssetLabelHorizontal(string label, string description, string val, params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			DrawLabel(label, options);

			if (!string.IsNullOrEmpty(val))
			{
				GUILayout.Label(val, ASSET_LABEL_STYLE_NAME);
			}
			GUILayout.EndHorizontal();

			DrawDescription(description, options);
			GUILayout.EndVertical();
		}

		public static void DrawNonSelectableAssetLabelHorizontal(string label, string description, string val)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			DrawLabel(label);

			if (!string.IsNullOrEmpty(val))
			{
				GUILayout.Label(val, ASSET_LABEL_STYLE_NAME);
			}

			GUILayout.EndHorizontal();

			DrawDescription(description);
			GUILayout.EndVertical();
		}

		public static void DrawLabelHorizontal(string label, string description, string val, params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			DrawLabel(label, options);

			if (!string.IsNullOrEmpty(val))
			{
				GUILayout.Label(val, LABEL_STYLE_NAME, GUILayout.ExpandWidth(false));
			}
			GUILayout.EndHorizontal();

			DrawDescription(description, options);
			GUILayout.EndVertical();
		}

		public static void DrawLabelHorizontal(string label, string description, string val)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			DrawLabel(label);

			if (!string.IsNullOrEmpty(val))
			{
				GUILayout.Label(val, LABEL_STYLE_NAME, GUILayout.ExpandWidth(false));
			}

			GUILayout.EndHorizontal();

			DrawDescription(description);
			GUILayout.EndVertical();
		}

		public static void DrawSelectableLabel(string label, string description, int val)
		{
			DrawLabelWithDescription(label, description);

#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				EditorGUILayout.TextField(string.Empty, val.ToString(), DEFAULT_SELECTABLE_LABEL_STYLE_NAME);
			}
			else
			{
#endif
				GUILayout.TextField(val.ToString(), DEFAULT_SELECTABLE_LABEL_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif
		}

		public static void DrawSelectableMultiLineLabel(string label, string description, string val)
		{
			DrawSelectableMultiLineLabel(label, description, val, DEFAULT_MULTILINE_LABEL_HEIGHT);
		}

		public static void DrawSelectableMultiLineLabel(string label, string description, string val, float height)
		{
			DrawLabelWithDescription(label, description);
			if (string.IsNullOrEmpty(val))
			{
				val = "";
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				EditorGUILayout.TextField(string.Empty, val, DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME, GUILayout.Height(height));
			}
			else
			{
#endif
				GUILayout.TextField(val, DEFAULT_MULTI_LINE_TEXT_FIELD_STYLE_NAME, GUILayout.Height(height));
#if UNITY_EDITOR
			}
#endif
		}

		// ================================================================================================

		const int DEFAULT_COLOR_SWATCH_WIDTH = 50;
		const int DEFAULT_COLOR_SWATCH_HEIGHT = 30;

		static readonly GUILayoutOption[] ColorSwatchLayout =
			{ GUILayout.ExpandWidth(false), GUILayout.Width(DEFAULT_COLOR_SWATCH_WIDTH) };

		static readonly GUILayoutOption[] ColorSwatchButton =
			{ GUILayout.Width(DEFAULT_COLOR_SWATCH_WIDTH), GUILayout.Height(DEFAULT_COLOR_SWATCH_HEIGHT) };

		public static bool DrawColorSwatch(string label, string description, Color32 inColor)
		{
			var returnValue = false;
			GUILayout.BeginVertical(ColorSwatchLayout);
			//DrawLabelWithDescription(label, description);

			//GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
			//GUILayout.EndVertical();

			var prevColor = GUI.backgroundColor;
			GUI.backgroundColor = inColor;
			if (GUILayout.Button(string.Empty, DEFAULT_COLOR_SWATCH_STYLE_NAME, ColorSwatchButton))
			{
				returnValue = true;
			}
			GUI.backgroundColor = prevColor;

			if (!string.IsNullOrEmpty(label))
			{
				DrawLabel(label, NoExpandWidth);
			}
			if (!string.IsNullOrEmpty(description))
			{
				DrawDescription(description, NoExpandWidth);
			}

			GUILayout.EndVertical();

			return returnValue;
		}

		// ================================================================================================

		public static Vector3 DrawVector3Field(string label, string inputIdX, string inputIdY, string inputIdZ, Vector3 inValue)
		{
			var labelStyle = GUI.skin.GetStyle(LABEL_STYLE_NAME);

			GUILayout.BeginHorizontal(ExpandWidth);
			GUILayout.Label(label, labelStyle);

			var inOutTextX = inValue.x.ToString(CultureInfo.InvariantCulture);
			var inOutTextY = inValue.y.ToString(CultureInfo.InvariantCulture);
			var inOutTextZ = inValue.z.ToString(CultureInfo.InvariantCulture);

			if (InputStrings.ContainsKey(inputIdX))
			{
				inOutTextX = InputStrings[inputIdX];
			}
			if (InputStrings.ContainsKey(inputIdY))
			{
				inOutTextY = InputStrings[inputIdY];
			}
			if (InputStrings.ContainsKey(inputIdZ))
			{
				inOutTextZ = InputStrings[inputIdZ];
			}

			var textfieldStyle = GUI.skin.GetStyle(DEFAULT_TEXT_FIELD_STYLE_NAME);

#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				GUILayout.Label("X", labelStyle);
				inOutTextX = EditorGUILayout.TextField(inOutTextX, textfieldStyle);
				GUILayout.Label("Y", labelStyle);
				inOutTextY = EditorGUILayout.TextField(inOutTextY, textfieldStyle);
				GUILayout.Label("Z", labelStyle);
				inOutTextZ = EditorGUILayout.TextField(inOutTextZ, textfieldStyle);
			}
			else
			{
#endif
				GUILayout.Label("X", labelStyle);
				inOutTextX = GUILayout.TextField(inOutTextX, textfieldStyle);
				GUILayout.Label("Y", labelStyle);
				inOutTextY = GUILayout.TextField(inOutTextY, textfieldStyle);
				GUILayout.Label("Z", labelStyle);
				inOutTextZ = GUILayout.TextField(inOutTextZ, textfieldStyle);
#if UNITY_EDITOR
			}
#endif

			bool enterWasPressed = Event.current.isKey && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
			bool tabWasPressed = Event.current.isKey && Event.current.keyCode == KeyCode.Tab;
			bool mouseWasClicked = Event.current.isMouse && Event.current.type == EventType.MouseDown;

			if (GUI.enabled && (enterWasPressed || tabWasPressed || mouseWasClicked))
			{
				//const string NUMBER_PATTERN = @"-?\d+";
				//const string ZERO_OR_POSITIVE_NUMBER_PATTERN = @"[0-9]+(\.[0-9]+)?";
				const string NUMBER_PATTERN = @"-?[0-9]*(\.[0-9]+)?";

				//Debug.LogFormat("raw input ({0}, {1}, {2})", inOutTextX, inOutTextY, inOutTextZ);
				inOutTextX = Regex.Match(inOutTextX, NUMBER_PATTERN).Value;
				inOutTextY = Regex.Match(inOutTextY, NUMBER_PATTERN).Value;
				inOutTextZ = Regex.Match(inOutTextZ, NUMBER_PATTERN).Value;

				//Debug.LogFormat("after regex input ({0}, {1}, {2})", inOutTextX, inOutTextY, inOutTextZ);
				float.TryParse(inOutTextX, NumberStyles.Number, CultureInfo.InvariantCulture, out inValue.x);
				float.TryParse(inOutTextY, NumberStyles.Number, CultureInfo.InvariantCulture, out inValue.y);
				float.TryParse(inOutTextZ, NumberStyles.Number, CultureInfo.InvariantCulture, out inValue.z);

				//Debug.LogFormat("after parsing to float {0}", inValue);
				inOutTextX = inValue.x.ToString(CultureInfo.InvariantCulture);
				inOutTextY = inValue.y.ToString(CultureInfo.InvariantCulture);
				inOutTextZ = inValue.z.ToString(CultureInfo.InvariantCulture);
			}

			InputStrings[inputIdX] = inOutTextX;
			InputStrings[inputIdY] = inOutTextY;
			InputStrings[inputIdZ] = inOutTextZ;

			GUILayout.EndHorizontal();

			return inValue;
		}

		public static Vector2 DrawVector2Field(string label, string inputIdX, string inputIdY, Vector2 inValue)
		{
			return DrawVector2Field(label, inputIdX, inputIdY, "X", "Y", inValue);
		}

		public static Vector2 DrawVector2Field(string label, string inputIdX, string inputIdY, string labelX, string labelY, Vector2 inValue)
		{
			GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			GUILayout.Label(label, LABEL_STYLE_NAME);


			var inOutTextX = inValue.x.ToString(CultureInfo.InvariantCulture);
			var inOutTextY = inValue.y.ToString(CultureInfo.InvariantCulture);

			if (InputStrings.ContainsKey(inputIdX))
			{
				inOutTextX = InputStrings[inputIdX];
			}
			if (InputStrings.ContainsKey(inputIdY))
			{
				inOutTextY = InputStrings[inputIdY];
			}

			var textfieldStyle = GUI.skin.GetStyle(DEFAULT_TEXT_FIELD_STYLE_NAME);

#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				GUILayout.Label(labelX, LABEL_STYLE_NAME);
				inOutTextX = EditorGUILayout.TextField(inOutTextX, textfieldStyle);
				GUILayout.Label(labelY, LABEL_STYLE_NAME);
				inOutTextY = EditorGUILayout.TextField(inOutTextY, textfieldStyle);
			}
			else
			{
#endif
				GUILayout.Label(labelX, LABEL_STYLE_NAME);
				inOutTextX = GUILayout.TextField(inOutTextX, textfieldStyle);
				GUILayout.Label(labelY, LABEL_STYLE_NAME);
				inOutTextY = GUILayout.TextField(inOutTextY, textfieldStyle);
#if UNITY_EDITOR
			}
#endif

			bool enterWasPressed = Event.current.isKey && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
			bool tabWasPressed = Event.current.isKey && Event.current.keyCode == KeyCode.Tab;
			bool mouseWasClicked = Event.current.isMouse && Event.current.type == EventType.MouseDown;

			if (GUI.enabled && (enterWasPressed || tabWasPressed || mouseWasClicked))
			{
				//const string NUMBER_PATTERN = @"-?\d+";
				//const string ZERO_OR_POSITIVE_NUMBER_PATTERN = @"[0-9]+(\.[0-9]+)?";
				const string NUMBER_PATTERN = @"-?[0-9]*(\.[0-9]+)?";

				//Debug.LogFormat("raw input ({0}, {1})", inOutTextX, inOutTextY);
				inOutTextX = Regex.Match(inOutTextX, NUMBER_PATTERN).Value;
				inOutTextY = Regex.Match(inOutTextY, NUMBER_PATTERN).Value;

				//Debug.LogFormat("after regex input ({0}, {1})", inOutTextX, inOutTextY);
				float.TryParse(inOutTextX, NumberStyles.Number, CultureInfo.InvariantCulture, out inValue.x);
				float.TryParse(inOutTextY, NumberStyles.Number, CultureInfo.InvariantCulture, out inValue.y);

				//Debug.LogFormat("after parsing to float {0}", inValue);
				inOutTextX = inValue.x.ToString(CultureInfo.InvariantCulture);
				inOutTextY = inValue.y.ToString(CultureInfo.InvariantCulture);
			}

			InputStrings[inputIdX] = inOutTextX;
			InputStrings[inputIdY] = inOutTextY;


			GUILayout.EndHorizontal();

			return inValue;
		}

		// ================================================================================================

		public static bool DrawBoolField(string label, bool inputtedBool)
		{
			inputtedBool = GUILayout.Toggle(inputtedBool, label, DEFAULT_CHECKBOX_STYLE_NAME);
			return inputtedBool;
		}

		public static bool DrawBoolField(GUIContent label, bool inputtedBool)
		{
			inputtedBool = GUILayout.Toggle(inputtedBool, label, DEFAULT_CHECKBOX_STYLE_NAME);
			return inputtedBool;
		}

		public static bool DrawBoolField(GUIContent label, string description, bool inputtedBool)
		{
			GUILayout.BeginVertical();
			inputtedBool = GUILayout.Toggle(inputtedBool, label, DEFAULT_CHECKBOX_STYLE_NAME);
			DrawDescription(description);
			GUILayout.EndVertical();
			return inputtedBool;
		}

		public static bool DrawBoolField(string label, string description, bool inputtedBool)
		{
			GUILayout.BeginVertical();
			inputtedBool = GUILayout.Toggle(inputtedBool, label, DEFAULT_CHECKBOX_STYLE_NAME);
			DrawDescription(description);
			GUILayout.EndVertical();
			return inputtedBool;
		}

		public static bool DrawBoolField(string label, string description, bool inputtedBool, GUIStyle style)
		{
			GUILayout.BeginVertical(style);
			inputtedBool = GUILayout.Toggle(inputtedBool, label, DEFAULT_CHECKBOX_STYLE_NAME);
			DrawDescription(description);
			GUILayout.EndVertical();
			return inputtedBool;
		}

		// ================================================================================================

		static readonly Dictionary<string, string> InputStrings = new Dictionary<string, string>();

		public static void ClearInputStrings()
		{
			InputStrings.Clear();
		}

		public static void ClearInputString(string inputId)
		{
			InputStrings.Remove(inputId);
		}

		public static bool HasInputString(string inputId)
		{
			return InputStrings.ContainsKey(inputId);
		}

		public static string GetInputString(string inputId)
		{
			if (InputStrings.ContainsKey(inputId))
			{
				return InputStrings[inputId];
			}
			return string.Empty;
		}

		// ================================================================================================

		public static int DrawIntField(int inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(string.Empty, string.Empty, string.Empty, inputId, null, inValue, null, int.MinValue, int.MaxValue-1, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, false, false, 0, 0, 0, null, options);
		}

		public static int DrawIntField(string label, int inValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, inValue, options);
		}

		public static int DrawIntField(string label, Action<int> onValueChangeCallback, int inValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, label, onValueChangeCallback, inValue, null, int.MinValue, int.MaxValue-1, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, false, false, 0, 0, 0, null, options);
		}

		public static int DrawIntField(string label, string description, int inValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, description, label + description, null, inValue, null, int.MinValue, int.MaxValue-1, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, false, false, 0, 0, 0, null, options);
		}

		public static int DrawIntField(string label, string description, int inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, description, inputId, null, inValue, null, int.MinValue, int.MaxValue-1, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, false, false, 0, 0, 0, null, options);
		}

		public static int DrawIntField(string label, string description, int inValue, Action additionalGuiBeside, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, description, label + description, null, inValue, null, int.MinValue, int.MaxValue-1, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, false, false, 0, 0, 0, additionalGuiBeside, options);
		}

		public static int DrawIntField(string label, string description, Action<int> onValueChangeCallback, int inValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, description, label + description, onValueChangeCallback, inValue, null, int.MinValue, int.MaxValue-1, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, false, false, 0, 0, 0, null, options);
		}

		public static int DrawIntField(string label, string description, int inValue, Func<int, string> getWarningMessages, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, description, label + description, null, inValue, getWarningMessages, int.MinValue, int.MaxValue-1, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, false, false, 0, 0, 0, null, options);
		}

		// --------------------------------------------------------


		public static int DrawIntFieldWithPlusMinus(string label, int inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, inputId, null, inValue, null, int.MinValue, int.MaxValue-1, true,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawIntFieldWithPlusMinus(string label, int inValue, string inputId, bool allowZeroOrNegative, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, inputId, null, inValue, null, allowZeroOrNegative ? int.MinValue : 1, int.MaxValue-1, allowZeroOrNegative,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawIntFieldWithPostLabelAndPlusMinus(string label, string postLabel, int inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, string.Empty, inputId, null, inValue, null, int.MinValue, int.MaxValue-1, true,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawIntFieldWithPostLabelAndPlusMinus(string label, string postLabel, string desc, int inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, desc, inputId, null, inValue, null, int.MinValue, int.MaxValue-1, true,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawIntFieldWithPostLabelAndPlusMinus(string label, string postLabel, string desc, int inValue, int minValue, int maxValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, desc, inputId, null, inValue, null, minValue, maxValue, minValue <= 0,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawIntFieldWithPostLabelAndPlusMinus(string label, string postLabel, int inValue, int minValue, int maxValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, string.Empty, inputId, null, inValue, null, minValue, maxValue, minValue <= 0,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawIntFieldWithPlusMinus(int inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(string.Empty, string.Empty, string.Empty, inputId, null, inValue, null, int.MinValue, int.MaxValue-1, true,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawIntFieldWithPlusMinus(int inValue, string inputId, GUIStyle textfieldStyle, GUIStyle buttonStyle, params GUILayoutOption[] options)
		{
			return DrawIntField(string.Empty, string.Empty, string.Empty, inputId, null, inValue, null, int.MinValue, int.MaxValue-1, true, textfieldStyle, textfieldStyle, WARNING_TEXT_FIELD_STYLE_NAME, buttonStyle, true, false, 0, 0, 0, null, options);
		}

		/// <summary>
		/// Int Field with plus/minus buttons. Allows positive and negative values, and zero.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="inValue"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static int DrawIntFieldWithPlusMinus(string label,
													int inValue,
													params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, label, null, inValue, null, int.MinValue, int.MaxValue-1, true,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		/// <summary>
		/// Int Field with plus/minus buttons. Allows positive and negative values, and zero. Max value is restrained by a parameter.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="inValue"></param>
		/// <param name="maxValue"></param>
		/// <param name="inputId"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static int DrawIntFieldWithPlusMinus(string label, int inValue, int maxValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, inputId, null, inValue, null, int.MinValue, maxValue, false, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawIntFieldWithPlusMinus(string label, int inValue, int minValue, int maxValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, inputId, null, inValue, null, minValue, maxValue, false, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static IntRange DrawValueRangeWithPlusMinus(string label, IntRange current, string lowerLimitInputId, string upperLimitInputId)
		{
			GUILayout.BeginHorizontal();
			int newLowerLimitValue = DrawIntFieldWithPlusMinus(label, current.LowerLimit,
				1, int.MaxValue - 1, lowerLimitInputId);

			int newUpperLimitValue;
			if (newLowerLimitValue != current.LowerLimit)
			{
				if (newLowerLimitValue >= current.UpperLimit)
				{
					// User input pushed the lower limit above the upper limit.
					// We allow this, but we increase the upper limit, so that the range stays valid.
					newUpperLimitValue = newLowerLimitValue + 1;
					current.SetUpperLimit(newUpperLimitValue, false);
				}
				current.SetLowerLimit(newLowerLimitValue, false);
				ClearInputStrings();
			}

			newUpperLimitValue = DrawIntFieldWithPlusMinus(" to ", current.UpperLimit,
				2, int.MaxValue - 1, upperLimitInputId);

			if (newUpperLimitValue != current.UpperLimit)
			{
				if (newUpperLimitValue <= newLowerLimitValue)
				{
					// User input pushed the upper limit below the lower limit.
					// We allow this, but we decrease the lower limit, so that the range stays valid.
					newLowerLimitValue = newUpperLimitValue - 1;
					current.SetLowerLimit(newLowerLimitValue, false);
				}
				current.SetUpperLimit(newUpperLimitValue, false);
				ClearInputStrings();
			}
			GUILayout.EndHorizontal();

			return current;
		}

		public static IntRange DrawValueRangeWithPlusMinus(string label, string postLabel, IntRange current, string lowerLimitInputId, string upperLimitInputId)
		{
			GUILayout.BeginHorizontal();
			int newLowerLimitValue = DrawIntFieldWithPlusMinus(label, current.LowerLimit,
				1, int.MaxValue - 1, lowerLimitInputId);

			int newUpperLimitValue;
			if (newLowerLimitValue != current.LowerLimit)
			{
				if (newLowerLimitValue >= current.UpperLimit)
				{
					// User input pushed the lower limit above the upper limit.
					// We allow this, but we increase the upper limit, so that the range stays valid.
					newUpperLimitValue = newLowerLimitValue + 1;
					current.SetUpperLimit(newUpperLimitValue, false);
				}
				current.SetLowerLimit(newLowerLimitValue, false);
				ClearInputStrings();
			}

			newUpperLimitValue = DrawIntFieldWithPostLabelAndPlusMinus(" to ", postLabel, current.UpperLimit,
				2, int.MaxValue - 1, upperLimitInputId);

			if (newUpperLimitValue != current.UpperLimit)
			{
				if (newUpperLimitValue <= newLowerLimitValue)
				{
					// User input pushed the upper limit below the lower limit.
					// We allow this, but we decrease the lower limit, so that the range stays valid.
					newLowerLimitValue = newUpperLimitValue - 1;
					current.SetLowerLimit(newLowerLimitValue, false);
				}
				current.SetUpperLimit(newUpperLimitValue, false);
				ClearInputStrings();
			}
			GUILayout.EndHorizontal();

			return current;
		}

		/// <summary>
		/// Int Field with plus/minus buttons. Allows positive values, and zero. Max value is restrained by a parameter.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="inValue"></param>
		/// <param name="maxValue"></param>
		/// <param name="inputId"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static int DrawPositiveOrZeroIntFieldWithPlusMinus(string label, int inValue, int maxValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, inputId, null, inValue, null, 0, maxValue, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawPositiveOrZeroIntFieldWithPlusMinus(string label, int inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, inputId, null, inValue, null, 0, int.MaxValue, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawPositiveOrZeroIntFieldWithPostLabelAndPlusMinus(string label, string postLabel,
			string inputId, int inValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, string.Empty, inputId, null, inValue, null,
				0, int.MaxValue, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME,
				WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0,
				0, null, options);
		}

		public static int DrawPositiveOrZeroIntFieldWithPlusMinus(string label, int inValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, label, null, inValue, null, 0, int.MaxValue, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawPositiveOrZeroIntFieldWithPlusMinus(string label, string postLabel, string description,
			int inValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, description, $"{label}{postLabel}{description}", null, inValue, null, 0, int.MaxValue, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawPositiveOrZeroIntFieldWithPlusMinus(string label, string postLabel, string description, int inValue, int maxValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, description, $"{label}{postLabel}{description}", null, inValue, null, 0, maxValue, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawPositiveIntFieldWithPlusMinus(string label, string postLabel, string description,
			int inValue, int maxValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, description, $"{label}{postLabel}{description}",
				null, inValue, null, 1, maxValue, false, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME,
				WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawPositiveIntFieldWithPlusMinus(string label, string postLabel, string description, string inputId,
			int inValue, int maxValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, description, inputId,
				null, inValue, null, 1, maxValue, false, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME,
				WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawPositiveIntFieldWithPlusMinus(string label, string postLabel, string description, string inputId,
			int inValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, description, inputId,
				null, inValue, null, 1, int.MaxValue-1, false, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME,
				WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawPositiveOrZeroIntFieldWithPlusMinus(string label, string description, int inValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, description, $"{label}{description}", null, inValue, null, 0, int.MaxValue, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawPositiveOrZeroIntFieldWithPlusMinus(string label, string description, int inValue, Action additionGuiBeside, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, description, $"{label}{description}", null, inValue, null, 0, int.MaxValue, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, additionGuiBeside, options);
		}

		public static int DrawPositiveOrZeroIntFieldWithPlusMinus(int inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(string.Empty, string.Empty, string.Empty, inputId, null, inValue, null, 0, int.MaxValue, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawPositiveOrZeroIntFieldWithPlusMinus(int inValue, string inputId, GUIStyle textfieldStyle, GUIStyle buttonStyle, params GUILayoutOption[] options)
		{
			return DrawIntField(string.Empty, string.Empty, string.Empty, inputId, null, inValue, null, 0, int.MaxValue, true, textfieldStyle, textfieldStyle, WARNING_TEXT_FIELD_STYLE_NAME, buttonStyle, true, false, 0, 0, 0, null, options);
		}

		/// <summary>
		/// Int Field with plus/minus buttons. Allows any number except zero.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="inValue"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static int DrawNonZeroIntFieldWithPlusMinus(string label,
													int inValue,
													params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, label, null, inValue, null, int.MinValue, int.MaxValue-1, false, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}
		public static int DrawNonZeroIntFieldWithPlusMinus(string label, int inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, inputId, null, inValue, null, int.MinValue, int.MaxValue-1, false, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawNonZeroIntFieldWithPostLabelAndPlusMinus(string label, string postLabel, string desc, int inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, desc, inputId, null, inValue, null, int.MinValue, int.MaxValue-1, false,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		/// <summary>
		/// Int Field with plus/minus buttons. Only allows positive numbers. Zero not allowed.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="inValue"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static int DrawPositiveIntFieldWithPlusMinus(string label,
													int inValue,
													params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, label, null, inValue, null, 1, int.MaxValue-1, false, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawPositiveIntFieldWithPlusMinus(string label, string postLabel, string description,
													int inValue,
													params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, description, $"{label}{postLabel}{description}", null, inValue, null, 1, int.MaxValue-1, false, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		/// <summary>
		/// Int Field with plus/minus buttons. Only allows positive numbers. Zero not allowed.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="inValue"></param>
		/// <param name="inputId"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static int DrawPositiveIntFieldWithPlusMinus(string label, int inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, inputId, null, inValue, null, 1, int.MaxValue-1, false, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		/// <summary>
		/// Int Field with plus/minus buttons. Only allows positive numbers. Zero not allowed.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="inValue"></param>
		/// <param name="maxValue"></param>
		/// <param name="inputId"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static int DrawPositiveIntFieldWithPlusMinus(string label, int inValue, int maxValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, string.Empty, inputId, null, inValue, null, 1, maxValue, false, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawPositiveIntFieldWithPostLabelAndPlusMinus(string label, string postLabel, string desc,
			string inputId, int inValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, desc, inputId, null, inValue, null, 1, int.MaxValue-1, false,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawPositiveIntFieldWithPostLabelAndPlusMinus(string label, string postLabel, string desc,
			string inputId, int inValue, int maxValue, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, desc, inputId, null, inValue, null, 1, maxValue, false,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		// --------------------------------------------------------------------

		public static int DrawIntFieldWithPlusMinusAndDiceRoll(string label, string description,
													int inValue, int numberOfDice, int numberOfSides, int bonusToRoll,
													params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, description, label + description, null, inValue, i =>
			{
				if (i < numberOfDice + bonusToRoll)
				{
					return "Roll result too low";
				}
				if (i > ((numberOfDice * numberOfSides) + bonusToRoll))
				{
					return "Roll result too high";
				}
				return string.Empty;
			}, 1, int.MaxValue-1, false, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, true, numberOfDice, numberOfSides, bonusToRoll, null, options);
		}

		public static int DrawIntFieldWithPlusMinusAndDiceRoll(string label, string description,
													int inValue, int numberOfDice, int numberOfSides,
													params GUILayoutOption[] options)
		{
			return DrawIntFieldWithPlusMinusAndDiceRoll(label, description, inValue, numberOfDice, numberOfSides, 0, options);
		}

		public static int DrawIntFieldWithPlusMinusAndDiceRoll(string label,
													int inValue, int numberOfDice, int numberOfSides, int bonusToRoll,
													params GUILayoutOption[] options)
		{
			return DrawIntFieldWithPlusMinusAndDiceRoll(label, string.Empty, inValue, numberOfDice, numberOfSides, bonusToRoll, options);
		}

		public static int DrawIntFieldWithPlusMinusAndDiceRoll(string label,
													int inValue, int numberOfDice, int numberOfSides,
													params GUILayoutOption[] options)
		{
			return DrawIntFieldWithPlusMinusAndDiceRoll(label, string.Empty, inValue, numberOfDice, numberOfSides, 0, options);
		}

		// --------------------------------------------------------------------

		public static int DrawIntFieldWithPlusMinus(string label, string description, int inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, description, inputId, null, inValue, null, int.MinValue, int.MaxValue-1, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawIntFieldWithPlusMinus(string label,
													string description,
													int inValue,
													params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, description, label + description, null, inValue, null, int.MinValue, int.MaxValue-1, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawIntFieldWithPlusMinus(string label,
													string description,
													int inValue,
													Action additionalGuiBeside,
													params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, description, label + description, null, inValue, null, int.MinValue, int.MaxValue-1, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, additionalGuiBeside, options);
		}

		public static int DrawIntFieldWithPlusMinus(string label, string postLabel,
													string description,
													int inValue,
													params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, description, label + postLabel + description, null, inValue, null, int.MinValue, int.MaxValue-1, true, DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawIntFieldWithPlusMinus(string label,
													string description,
													Action<int> onValueChangeCallback,
													int inValue,
													Func<int, string> getWarningMessages,
													GUIStyle normalGuiStyle,
													GUIStyle highlightGuiStyle,
													GUIStyle warningGuiStyle,
													params GUILayoutOption[] options)
		{
			return DrawIntField(label, string.Empty, description, label + description, onValueChangeCallback, inValue, getWarningMessages, int.MinValue, int.MaxValue-1, true, normalGuiStyle, highlightGuiStyle, warningGuiStyle, PLUS_MINUS_BUTTON_STYLE_NAME, true, false, 0, 0, 0, null, options);
		}

		public static int DrawIntField(string label, string postLabel, string description, string inputId, Action<int> onValueChangeCallback,
			int inValue, Func<int, string> getWarningMessages, int minValueAllowed, int maxValueAllowed, bool allowZero, bool withPlusMinus, bool withDiceRoll,
			int numberOfDice, int numberOfSides, int bonusToRoll, Action additionalGuiBeside, params GUILayoutOption[] options)
		{
			return DrawIntField(label, postLabel, description, inputId, onValueChangeCallback,
				inValue, getWarningMessages, minValueAllowed, maxValueAllowed, allowZero,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, PLUS_MINUS_BUTTON_STYLE_NAME,
				withPlusMinus, withDiceRoll, numberOfDice, numberOfSides, bonusToRoll, additionalGuiBeside, options);
		}

		static readonly GUIContent DiceButtonLabel = new GUIContent();
		static readonly GUILayoutOption[] NoLayoutOption = { };

		public static int DrawIntField(string label, string postLabel, string description, string inputId, Action<int> onValueChangeCallback,
			int inValue, Func<int, string> getWarningMessages, int minValueAllowed, int maxValueAllowed, bool allowZero, GUIStyle normalGuiStyle,
			GUIStyle highlightGuiStyle, GUIStyle warningGuiStyle, GUIStyle buttonStyle, bool withPlusMinus, bool withDiceRoll,
			int numberOfDice, int numberOfSides, int bonusToRoll, Action additionalGuiBeside, params GUILayoutOption[] options)
		{
			if (!allowZero && inValue == 0)
			{
				inValue = 1;
			}

			string warningMessage = null;
			bool validInput = true;
			if (getWarningMessages != null)
			{
				warningMessage = getWarningMessages(inValue);
				validInput = string.IsNullOrEmpty(warningMessage);
			}

			var styleToUse = validInput ? (inValue == 0 ? normalGuiStyle : highlightGuiStyle) : warningGuiStyle;

			if (styleToUse == normalGuiStyle && !string.IsNullOrEmpty(postLabel))
			{
				styleToUse = "PropertyTextfield";
			}
			string inOutText = inValue.ToString(CultureInfo.InvariantCulture);

			if (InputStrings.ContainsKey(inputId))
			{
				inOutText = InputStrings[inputId];
			}

			GUILayout.BeginHorizontal(NoLayoutOption);
			DrawLabel(label);

			if (withPlusMinus || withDiceRoll)
			{
				GUILayout.BeginHorizontal(NoLayoutOption);
			}
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inOutText = EditorGUILayout.TextField(inOutText, styleToUse, options);
			}
			else
			{
#endif
				inOutText = GUILayout.TextField(inOutText, styleToUse, options);
#if UNITY_EDITOR
			}
#endif

			if (!string.IsNullOrEmpty(postLabel))
			{
				DrawLabel(postLabel, "PropertyPostLabel");
			}

			if (!string.IsNullOrEmpty(postLabel) && withPlusMinus)
			{
				GUILayout.Space(10);
			}

			if (withPlusMinus)
			{
				if (withDiceRoll)
				{
					maxValueAllowed = (numberOfDice * numberOfSides) + bonusToRoll;
				}

				// -----------------------------------------------------------------
				var prevGuiEnabled = GUI.enabled;

				var canStillIncrease = (inValue + 1 <= maxValueAllowed);
				GUI.enabled = prevGuiEnabled && canStillIncrease;
				if (GUILayout.Button(GetIcon("Icon.Plus"), buttonStyle, NoLayoutOption) && canStillIncrease)
				{
					++inValue;
					if (!allowZero && inValue == 0)
					{
						inValue = 1;
					}
					inOutText = inValue.ToString();
				}

				var canStillDecrease = (inValue - 1 >= minValueAllowed);
				GUI.enabled = prevGuiEnabled && canStillDecrease;
				if (GUILayout.Button(GetIcon("Icon.Minus"), buttonStyle, NoLayoutOption) && canStillDecrease)
				{
					--inValue;
					if (!allowZero && inValue == 0)
					{
						inValue = -1;
					}
					inOutText = inValue.ToString();
				}

				GUI.enabled = prevGuiEnabled;
				// -----------------------------------------------------------------
			}
			if (withDiceRoll)
			{
				DiceButtonLabel.text = $"{numberOfDice.ToString()}d{numberOfSides.ToString()}";
				DiceButtonLabel.image = GetIcon("Icon.Dice");
				if (GUILayout.Button(DiceButtonLabel, buttonStyle, NoLayoutOption))
				{
					inValue = RandomUtil.RollDice(numberOfDice, numberOfSides, bonusToRoll);
					inOutText = inValue.ToString();
				}
			}
			if (withPlusMinus || withDiceRoll)
			{
				GUILayout.EndHorizontal();
			}

			if (additionalGuiBeside != null)
			{
				additionalGuiBeside();
			}

			GUILayout.EndHorizontal();
			DrawDescription(description);


			if (!validInput)
			{
				GUILayout.Label(warningMessage, ERROR_LABEL_STYLE_NAME, NoLayoutOption);
			}

			// note: enter gets intercepted before we have a chance to detect this event, so the EventType becomes Used.
			// that means this gets called more times than necessary, but should still work correctly
			bool enterWasPressed = Event.current.type == EventType.Used && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);

			bool tabWasPressed = Event.current.isKey && Event.current.keyCode == KeyCode.Tab;
			bool mouseWasClicked = Event.current.isMouse && Event.current.type == EventType.MouseDown;

			if (enterWasPressed || tabWasPressed || mouseWasClicked)
			{
				inOutText = Regex.Match(inOutText, @"-?\d+").Value;

				int.TryParse(inOutText, out inValue);
				inOutText = inValue.ToString();

				if (inValue < minValueAllowed)
				{
					inValue = minValueAllowed;
					inOutText = inValue.ToString();
				}
				else if (inValue > maxValueAllowed)
				{
					inValue = maxValueAllowed;
					inOutText = inValue.ToString();
				}

				if (onValueChangeCallback != null)
				{
					onValueChangeCallback(inValue);
				}

				//BetterDebug.Log("DrawIntField: inOutText now \"{0}\". inValue: {1} (Event.current.type: {2}, Event.current.rawType: {3}, Event.current.commandName: {4})",
				//	inOutText, inValue, Event.current.type, Event.current.rawType, Event.current.commandName);
			}


			InputStrings[inputId] = inOutText;

			return inValue;
		}


		public static int DrawIntField(Rect rect, string inputId, Action<int> onValueChangeCallback,
			int inValue, int minValueAllowed, int maxValueAllowed, bool allowZero, GUIStyle normalGuiStyle)
		{
			if (!allowZero && inValue == 0)
			{
				inValue = 1;
			}

			string inOutText = inValue.ToString(CultureInfo.InvariantCulture);

			if (InputStrings.ContainsKey(inputId))
			{
				inOutText = InputStrings[inputId];
			}

#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inOutText = EditorGUI.TextField(rect, inOutText, normalGuiStyle);
			}
			else
			{
#endif
				inOutText = GUI.TextField(rect, inOutText, normalGuiStyle);
#if UNITY_EDITOR
			}
#endif

			bool enterWasPressed = Event.current.isKey && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
			bool tabWasPressed = Event.current.isKey && Event.current.keyCode == KeyCode.Tab;
			bool mouseWasClicked = Event.current.isMouse && Event.current.type == EventType.MouseDown;

			if (enterWasPressed || tabWasPressed || mouseWasClicked)
			{
				inOutText = Regex.Match(inOutText, @"-?\d+").Value;

				int.TryParse(inOutText, out inValue);
				inOutText = inValue.ToString();

				if (inValue < minValueAllowed)
				{
					inValue = -inValue;
					if (inValue < minValueAllowed)
					{
						inValue = minValueAllowed;
					}
					else if (inValue > maxValueAllowed)
					{
						inValue = minValueAllowed;
					}

					inOutText = inValue.ToString();
				}
				else if (inValue > maxValueAllowed)
				{
					inValue = maxValueAllowed;
					inOutText = inValue.ToString();
				}

				if (onValueChangeCallback != null)
				{
					onValueChangeCallback(inValue);
				}
			}

			InputStrings[inputId] = inOutText;

			return inValue;
		}

		public static int DrawIntField(Rect rect, string inputId, int inValue)
		{
			return DrawIntField(rect, inputId, null, inValue, int.MinValue, int.MaxValue, true, "textfield");
		}

		public static int DrawIntField(Rect rect, string inputId, int inValue, int minAllowed, int maxAllowed)
		{
			return DrawIntField(rect, inputId, null, inValue, minAllowed, maxAllowed, true, "textfield");
		}

		public static int DrawPositiveIntField(Rect rect, string inputId, int inValue)
		{
			return DrawIntField(rect, inputId, null, inValue, 0, int.MaxValue, true, "textfield");
		}

		public static int DrawPositiveIntField(int inValue, params GUILayoutOption[] options)
		{
			return DrawPositiveIntField(string.Empty, string.Empty, inValue, DEFAULT_TEXT_FIELD_STYLE_NAME, options);
		}

		public static int DrawPositiveIntField(string label, int inValue, params GUILayoutOption[] options)
		{
			return DrawPositiveIntField(label, string.Empty, inValue, DEFAULT_TEXT_FIELD_STYLE_NAME, options);
		}

		public static int DrawPositiveIntField(string label, string description, int inValue, GUIStyle styleUsed, params GUILayoutOption[] options)
		{
			GUILayout.BeginHorizontal();

			if (!string.IsNullOrEmpty(label))
			{
				DrawLabel(label);
			}

			string inText;
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inText = EditorGUILayout.TextField(inValue.ToString(), styleUsed, options);
			}
			else
			{
#endif
				inText = GUILayout.TextField(inValue.ToString(), styleUsed, options);
#if UNITY_EDITOR
			}
#endif
			GUILayout.EndHorizontal();

			if (!string.IsNullOrEmpty(description))
			{
				DrawDescription(description);
			}

			inText = Regex.Match(inText, @"\d+").Value;
			int inputAsInt;
			int.TryParse(inText, out inputAsInt);

			return inputAsInt;
		}

		public static int DrawPositiveIntField(Rect contentRect, int inValue, GUIStyle styleUsed)
		{
			string inText;
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inText = EditorGUI.TextField(contentRect, inValue.ToString(), styleUsed);
			}
			else
			{
#endif
				inText = GUI.TextField(contentRect, inValue.ToString(), styleUsed);
#if UNITY_EDITOR
			}
#endif

			inText = Regex.Match(inText, @"\d+").Value;
			int inputAsInt;
			int.TryParse(inText, out inputAsInt);

			return inputAsInt;
		}

		public static int DrawPositiveIntFieldWithHighlight(Rect contentRect, int inValue, GUIStyle styleUsed)
		{
			return DrawPositiveIntFieldWithHighlight(contentRect, inValue, styleUsed, HIGHLIGHTED_TEXT_FIELD_STYLE_NAME);
		}

		public static int DrawPositiveIntFieldWithHighlight(Rect contentRect, int inValue, GUIStyle styleUsed, GUIStyle styleUsedForHighlight)
		{
			var styleToUse = inValue == 0 ? styleUsed : styleUsedForHighlight;

			string inText;
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inText = EditorGUI.TextField(contentRect, inValue.ToString(), styleToUse);
			}
			else
			{
#endif
				inText = GUI.TextField(contentRect, inValue.ToString(), styleToUse);
#if UNITY_EDITOR
			}
#endif

			inText = Regex.Match(inText, @"\d+").Value;
			int inputAsInt;
			int.TryParse(inText, out inputAsInt);

			return inputAsInt;
		}

		// ================================================================================================

		public static double DrawNumberField(string label, string description, double inValue, params GUILayoutOption[] options)
		{
			return DrawNumberField(label, string.Empty, description, label+description, null, inValue, null, double.MinValue, double.MaxValue, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, null, options);
		}

		public static double DrawNumberField(string label, string description, double inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawNumberField(label, string.Empty, description, inputId, null, inValue, null, double.MinValue, double.MaxValue, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, null, options);
		}

		public static double DrawNumberField(double inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawNumberField(string.Empty, string.Empty, string.Empty, inputId, null, inValue, null, double.MinValue, double.MaxValue, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, null, options);
		}

		public static double DrawNumberField(string label, string postLabel, string description, double inValue, params GUILayoutOption[] options)
		{
			return DrawNumberField(label, postLabel, description, label+postLabel+description, null, inValue, null, double.MinValue, double.MaxValue, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, null, options);
		}

		public static float DrawNumberField(string label, string postLabel, string description, float inValue, params GUILayoutOption[] options)
		{
			return (float)DrawNumberField(label, postLabel, description, label+postLabel+description, null, inValue, null, double.MinValue, double.MaxValue, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, null, options);
		}

		public static double DrawNumberField(string label, string postLabel, string description, double inValue, Action additionalGuiBeside, params GUILayoutOption[] options)
		{
			return DrawNumberField(label, postLabel, description, label+postLabel+description, null, inValue, null, double.MinValue, double.MaxValue, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, additionalGuiBeside, options);
		}

		public static double DrawZeroOrPositiveNumberField(string label, string postLabel, string description, double inValue, Action additionalGuiBeside, params GUILayoutOption[] options)
		{
			return DrawNumberField(label, postLabel, description, label+postLabel+description, null, inValue, null, 0, double.MaxValue, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, additionalGuiBeside, options);
		}

		public static double DrawZeroOrPositiveNumberField(string label, string postLabel, string description, double inValue, double maxValue, Action additionalGuiBeside, params GUILayoutOption[] options)
		{
			return DrawNumberField(label, postLabel, description, label+postLabel+description, null, inValue, null, 0, maxValue, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, additionalGuiBeside, options);
		}

		public static double DrawZeroOrPositiveNumberField(string label, string postLabel, string description, double inValue, double maxValue, Action additionalGuiBeside)
		{
			return DrawNumberField(label, postLabel, description, label+postLabel+description, null, inValue, null, 0, maxValue, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, additionalGuiBeside, null);
		}

		public static double DrawZeroOrPositiveNumberField(string label, string postLabel, string description, double inValue, double maxValue, params GUILayoutOption[] options)
		{
			return DrawNumberField(label, postLabel, description, label+postLabel+description, null, inValue, null, 0, maxValue, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, null, options);
		}

		public static double DrawZeroOrPositiveNumberField(string label, string postLabel, string description, double inValue, double maxValue)
		{
			return DrawNumberField(label, postLabel, description, label+postLabel+description, null, inValue, null, 0, maxValue, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, null, null);
		}

		public static double DrawPositiveNumberField(string label, string postLabel, string description,
			double inValue, double minValue, Action additionalGuiBeside, params GUILayoutOption[] options)
		{
			return DrawNumberField(label, postLabel, description, label + postLabel + description, null, inValue, null,
				minValue, double.MaxValue, false, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				additionalGuiBeside, options);
		}

		public static double DrawNumberField(string label, string postLabel, string description, string inputId, Action<double> onValueChangeCallback, double inValue, Func<double, string> getWarningMessages, double minValueAllowed, double maxValueAllowed, bool allowZero, bool roundNumber, int decimalPlaces, GUIStyle normalGuiStyle, GUIStyle highlightGuiStyle, GUIStyle warningGuiStyle, Action additionalGuiBeside, params GUILayoutOption[] options)
		{
			string warningMessage = null;
			bool validInput = true;
			if (getWarningMessages != null)
			{
				warningMessage = getWarningMessages(inValue);
				validInput = string.IsNullOrEmpty(warningMessage);
			}

			var styleToUse = validInput ? (Math.Abs(inValue) < 0.0001 ? normalGuiStyle : highlightGuiStyle) : warningGuiStyle;

			string inOutText = inValue.ToString(CultureInfo.InvariantCulture);

			if (InputStrings.ContainsKey(inputId))
			{
				inOutText = InputStrings[inputId];
			}

			GUILayout.BeginHorizontal();
			DrawLabel(label);
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inOutText = EditorGUILayout.TextField(inOutText, styleToUse, options);
			}
			else
			{
#endif
				inOutText = GUILayout.TextField(inOutText, styleToUse, options);
#if UNITY_EDITOR
			}
#endif

			if (!string.IsNullOrEmpty(postLabel))
			{
				DrawLabel(postLabel, "PropertyPostLabel");
			}
			if (additionalGuiBeside != null)
			{
				additionalGuiBeside();
			}
			GUILayout.EndHorizontal();

			DrawDescription(description);


			if (!validInput)
			{
				GUILayout.Label(warningMessage, ERROR_LABEL_STYLE_NAME);
			}

			bool enterWasPressed = Event.current.isKey && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
			bool tabWasPressed = Event.current.isKey && Event.current.keyCode == KeyCode.Tab;
			bool mouseWasClicked = Event.current.isMouse && Event.current.type == EventType.MouseDown;

			if (enterWasPressed || tabWasPressed || mouseWasClicked)
			{
				//Debug.LogFormat("raw input {0}", inOutText);
				inOutText = Regex.Match(inOutText, @"-?[0-9]*(\.[0-9]+)?").Value;

				//Debug.LogFormat("parsing input {0}", inOutText);
				double.TryParse(inOutText, NumberStyles.Number, CultureInfo.InvariantCulture, out inValue);

				if (inValue < minValueAllowed)
				{
					inValue = minValueAllowed;
				}

				if (inValue > maxValueAllowed)
				{
					inValue = maxValueAllowed;
				}

				if (roundNumber)
				{
					inValue = Math.Round(inValue, decimalPlaces);
				}

				inOutText = inValue.ToString(CultureInfo.InvariantCulture);
				//Debug.LogFormat("got {0}", inValue);

				if (onValueChangeCallback != null)
				{
					onValueChangeCallback(inValue);
				}
			}

			InputStrings[inputId] = inOutText;

			return inValue;
		}

		// ================================================================================================

		public static double DrawNumberField(Rect rect, string inputId, double inValue)
		{
			return DrawNumberField(rect, string.Empty, string.Empty, inputId, null, inValue, null, float.MinValue, float.MaxValue, true, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, null);
		}

		public static double DrawNumberField(Rect rect, string label, string description, string inputId, Action<double> onValueChangeCallback, double inValue, Func<double, string> getWarningMessages, double minValueAllowed, double maxValueAllowed, bool allowZero, bool useRect, bool roundNumber, int decimalPlaces, GUIStyle normalGuiStyle, GUIStyle highlightGuiStyle, GUIStyle warningGuiStyle, params GUILayoutOption[] options)
		{
			string warningMessage = null;
			bool validInput = true;
			if (getWarningMessages != null)
			{
				warningMessage = getWarningMessages(inValue);
				validInput = string.IsNullOrEmpty(warningMessage);
			}

			var styleToUse = validInput ? (Math.Abs(inValue) < 0.0001f ? normalGuiStyle : highlightGuiStyle) : warningGuiStyle;

			string inOutText = inValue.ToString(CultureInfo.InvariantCulture);

			if (InputStrings.ContainsKey(inputId))
			{
				inOutText = InputStrings[inputId];
			}

			if (useRect)
			{
#if UNITY_EDITOR
				if (!EditorApplication.isPlaying)
				{
					inOutText = EditorGUI.TextField(rect, inOutText, normalGuiStyle);
				}
				else
				{
#endif
					inOutText = GUI.TextField(rect, inOutText, normalGuiStyle);
#if UNITY_EDITOR
				}
#endif
			}
			else
			{
				GUILayout.BeginHorizontal();
				DrawLabel(label);
#if UNITY_EDITOR
				if (!EditorApplication.isPlaying)
				{
					inOutText = EditorGUILayout.TextField(inOutText, styleToUse, options);
				}
				else
				{
#endif
					inOutText = GUILayout.TextField(inOutText, styleToUse, options);
#if UNITY_EDITOR
				}
#endif
				GUILayout.EndHorizontal();

				DrawDescription(description);

				if (!validInput)
				{
					GUILayout.Label(warningMessage, ERROR_LABEL_STYLE_NAME);
				}
			}

			bool enterWasPressed = Event.current.isKey && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
			bool tabWasPressed = Event.current.isKey && Event.current.keyCode == KeyCode.Tab;
			bool mouseWasClicked = Event.current.isMouse && Event.current.type == EventType.MouseDown;

			if (enterWasPressed || tabWasPressed || mouseWasClicked)
			{
				//Debug.LogFormat("raw input {0}", inOutText);
				inOutText = Regex.Match(inOutText, @"-?[0-9]*(\.[0-9]+)?").Value;

				//Debug.LogFormat("parsing input {0}", inOutText);
				double.TryParse(inOutText, NumberStyles.Number, CultureInfo.InvariantCulture, out inValue);

				if (inValue < minValueAllowed)
				{
					inValue = minValueAllowed;
				}

				if (inValue > maxValueAllowed)
				{
					inValue = maxValueAllowed;
				}

				if (roundNumber)
				{
					inValue = Math.Round(inValue, decimalPlaces);
				}

				inOutText = inValue.ToString(CultureInfo.InvariantCulture);
				//Debug.LogFormat("got {0}", newValue);

				if (onValueChangeCallback != null)
				{
					onValueChangeCallback(inValue);
				}
			}

			InputStrings[inputId] = inOutText;

			return inValue;
		}

		// ================================================================================================

		public static float DrawPositiveNumberField(string label, string postLabel, string description,
			float inValue, float minValue, Action additionalGuiBeside, params GUILayoutOption[] options)
		{
			return DrawNumberField(label, postLabel, description, label + postLabel + description, null, inValue, null,
				minValue, float.MaxValue, false, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				additionalGuiBeside, options);
		}

		public static float DrawNumberField(string label, string description, string inputId, float inValue)
		{
			return DrawNumberField(label, null, description, inputId, null, inValue, null,
				float.MinValue, float.MaxValue, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				null, null);
		}

		public static float DrawPositiveNumberField(string label, string description, string inputId, float inValue)
		{
			return DrawNumberField(label, null, description, inputId, null, inValue, null,
				0, float.MaxValue, false, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				null, null);
		}

		public static float DrawNumberField(string label, string description, string inputId,
			float inValue, float minValueAllowed, float maxValueAllowed, bool allowZero)
		{
			return DrawNumberField(label, null, description, inputId, null, inValue, null,
				minValueAllowed, maxValueAllowed, allowZero, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME,
				null, null);
		}

		public static float DrawNumberField(string label, string postLabel, string description, string inputId,
			Action<float> onValueChangeCallback, float inValue, Func<float, string> getWarningMessages,
			float minValueAllowed, float maxValueAllowed, bool allowZero, bool roundNumber, int decimalPlaces,
			GUIStyle normalGuiStyle, GUIStyle highlightGuiStyle, GUIStyle warningGuiStyle, Action additionalGuiBeside,
			params GUILayoutOption[] options)
		{
			if (!allowZero && inValue == 0)
			{
				inValue = 0.0001f;
			}

			string warningMessage = null;
			bool validInput = true;
			if (getWarningMessages != null)
			{
				warningMessage = getWarningMessages(inValue);
				validInput = string.IsNullOrEmpty(warningMessage);
			}

			var styleToUse = validInput ? (Math.Abs(inValue) < 0.0001f ? normalGuiStyle : highlightGuiStyle) : warningGuiStyle;

			string inOutText = inValue.ToString(CultureInfo.InvariantCulture);

			if (InputStrings.ContainsKey(inputId))
			{
				inOutText = InputStrings[inputId];
			}

			GUILayout.BeginHorizontal();
			DrawLabel(label);
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				inOutText = EditorGUILayout.TextField(inOutText, styleToUse, options);
			}
			else
			{
#endif
				inOutText = GUILayout.TextField(inOutText, styleToUse, options);
#if UNITY_EDITOR
			}
#endif

			if (!string.IsNullOrEmpty(postLabel))
			{
				DrawLabel(postLabel, "PropertyPostLabel");
			}
			if (additionalGuiBeside != null)
			{
				additionalGuiBeside();
			}
			GUILayout.EndHorizontal();

			DrawDescription(description);


			if (!validInput)
			{
				GUILayout.Label(warningMessage, ERROR_LABEL_STYLE_NAME);
			}

			bool enterWasPressed = Event.current.isKey && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
			bool tabWasPressed = Event.current.isKey && Event.current.keyCode == KeyCode.Tab;
			bool mouseWasClicked = Event.current.isMouse && Event.current.type == EventType.MouseDown;

			if (enterWasPressed || tabWasPressed || mouseWasClicked)
			{
				//Debug.LogFormat("raw input {0}", inOutText);
				inOutText = Regex.Match(inOutText, @"-?[0-9]*(\.[0-9]+)?").Value;

				//Debug.LogFormat("parsing input {0}", inOutText);
				float.TryParse(inOutText, NumberStyles.Number, CultureInfo.InvariantCulture, out inValue);

				if (inValue < minValueAllowed)
				{
					inValue = minValueAllowed;
				}

				if (inValue > maxValueAllowed)
				{
					inValue = maxValueAllowed;
				}

				if (roundNumber)
				{
					inValue = (float)Math.Round(inValue, decimalPlaces);
				}

				inOutText = inValue.ToString(CultureInfo.InvariantCulture);
				//Debug.LogFormat("got {0}", inValue);

				if (onValueChangeCallback != null)
				{
					onValueChangeCallback(inValue);
				}
			}

			InputStrings[inputId] = inOutText;

			return inValue;
		}

		// ================================================================================================

		const int DEFAULT_DECIMAL_PLACES = 5;

		public static float DrawNumberField(float inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawNumberField(default(Rect), string.Empty, string.Empty, inputId, null, inValue, null, float.MinValue, float.MaxValue, true, false, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, options);
		}

		public static float DrawZeroOrPositiveNumberField(float inValue, string inputId, params GUILayoutOption[] options)
		{
			return DrawNumberField(default(Rect), string.Empty, string.Empty, inputId, null, inValue, null, 0, float.MaxValue,  true, false, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, options);
		}

		public static float DrawNumberField(Rect rect, float inValue, string inputId)
		{
			return DrawNumberField(rect, string.Empty, string.Empty, inputId, null, inValue, null, float.MinValue, float.MaxValue, true, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, null);
		}

		public static float DrawNumberField(Rect rect, float inValue, float minAllowed, float maxAllowed, string inputId)
		{
			return DrawNumberField(rect, string.Empty, string.Empty, inputId, null, inValue, null, minAllowed, maxAllowed, true, true, true, DEFAULT_DECIMAL_PLACES,
				DEFAULT_TEXT_FIELD_STYLE_NAME, DEFAULT_TEXT_FIELD_STYLE_NAME, WARNING_TEXT_FIELD_STYLE_NAME, null);
		}

		public static float DrawNumberField(Rect rect, string label, string description, string inputId, Action<float> onValueChangeCallback, float inValue, Func<float, string> getWarningMessages, float minValueAllowed, float maxValueAllowed, bool allowZero, bool useRect, bool roundNumber, int decimalPlaces, GUIStyle normalGuiStyle, GUIStyle highlightGuiStyle, GUIStyle warningGuiStyle, params GUILayoutOption[] options)
		{
			string warningMessage = null;
			bool validInput = true;
			if (getWarningMessages != null)
			{
				warningMessage = getWarningMessages(inValue);
				validInput = string.IsNullOrEmpty(warningMessage);
			}

			var styleToUse = validInput ? (Math.Abs(inValue) < 0.0001f ? normalGuiStyle : highlightGuiStyle) : warningGuiStyle;

			string inOutText = inValue.ToString(CultureInfo.InvariantCulture);

			if (InputStrings.ContainsKey(inputId))
			{
				inOutText = InputStrings[inputId];
			}

			if (useRect)
			{
#if UNITY_EDITOR
				if (!EditorApplication.isPlaying)
				{
					inOutText = EditorGUI.TextField(rect, inOutText, normalGuiStyle);
				}
				else
				{
#endif
					inOutText = GUI.TextField(rect, inOutText, normalGuiStyle);
#if UNITY_EDITOR
				}
#endif
			}
			else
			{
				GUILayout.BeginHorizontal();
				DrawLabel(label);
#if UNITY_EDITOR
				if (!EditorApplication.isPlaying)
				{
					inOutText = EditorGUILayout.TextField(inOutText, styleToUse, options);
				}
				else
				{
#endif
					inOutText = GUILayout.TextField(inOutText, styleToUse, options);
#if UNITY_EDITOR
				}
#endif
				GUILayout.EndHorizontal();

				DrawDescription(description);

				if (!validInput)
				{
					GUILayout.Label(warningMessage, ERROR_LABEL_STYLE_NAME);
				}
			}

			bool enterWasPressed = Event.current.isKey && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
			bool tabWasPressed = Event.current.isKey && Event.current.keyCode == KeyCode.Tab;
			bool mouseWasClicked = Event.current.isMouse && Event.current.type == EventType.MouseDown;

			if (enterWasPressed || tabWasPressed || mouseWasClicked)
			{
				//Debug.LogFormat("raw input {0}", inOutText);
				inOutText = Regex.Match(inOutText, @"-?[0-9]*(\.[0-9]+)?").Value;

				//Debug.LogFormat("parsing input {0}", inOutText);
				float.TryParse(inOutText, NumberStyles.Number, CultureInfo.InvariantCulture, out inValue);

				if (inValue < minValueAllowed)
				{
					inValue = minValueAllowed;
				}

				if (inValue > maxValueAllowed)
				{
					inValue = maxValueAllowed;
				}

				if (roundNumber)
				{
					inValue = (float)Math.Round(inValue, decimalPlaces);
				}

				inOutText = inValue.ToString(CultureInfo.InvariantCulture);
				//Debug.LogFormat("got {0}", newValue);

				if (onValueChangeCallback != null)
				{
					onValueChangeCallback(inValue);
				}
			}

			InputStrings[inputId] = inOutText;

			return inValue;
		}

		// ================================================================================================

		static Texture2D GetIcon(string iconName)
		{
			return GUI.skin.GetStyle(iconName).normal.background;
		}

		public static void DrawDebugBox(Rect rect, Color color)
		{
			DrawIcon(rect, Texture2D.whiteTexture, color);
		}

		public static void DrawDebugPosition(Vector2 topLeftPos)
		{
			DrawIcon(new Rect(topLeftPos, Vector2.one * 50), Texture2D.whiteTexture, new Color(1,1,1,0.5f));
		}

		public static void DrawDebugBox(Vector2 topLeftPos, Color color)
		{
			DrawIcon(new Rect(topLeftPos, Vector2.one * 50), Texture2D.whiteTexture, color);
		}

		public static void DrawIcon(float x, float y, int size, Texture icon, Color color)
		{
			var prevColor = GUI.contentColor;
			GUI.contentColor = color;
			GUI.Label(new Rect(x, y, size, size), icon, "IconHolder");
			GUI.contentColor = prevColor;
		}

		public static void DrawIcon(Rect rect, Texture icon, Color color, ScaleMode scaleMode = ScaleMode.StretchToFill)
		{
			GUI.DrawTexture(rect, icon, scaleMode,
				true, 0, color,
				Vector4.zero, Vector4.zero);
		}

		public static void DrawIcon(Texture icon, Color color)
		{
			var prevColor = GUI.contentColor;
			GUI.contentColor = color;
			GUILayout.Label(icon, "IconHolder");
			GUI.contentColor = prevColor;
		}

		public static void DrawIcon(Texture icon, Color color, int width, int height, ScaleMode scaleMode = ScaleMode.ScaleToFit)
		{
			var rect = GUILayoutUtility.GetRect(width, width, height, height);
			GUI.DrawTexture(rect, icon, scaleMode,
				true, 0, color,
				Vector4.zero, Vector4.zero);
		}

		public static void DrawIcon(Texture icon, Color color, int minWidth, int maxWidth, int minHeight, int maxHeight, ScaleMode scaleMode = ScaleMode.ScaleToFit)
		{
			var rect = GUILayoutUtility.GetRect(minWidth, maxWidth, minHeight, maxHeight);
			GUI.DrawTexture(rect, icon, scaleMode,
				true, 0, color,
				Vector4.zero, Vector4.zero);
		}

		public static void DrawIcon(Texture icon, Color color, int upperSpacing)
		{
			var prevColor = GUI.contentColor;
			GUI.contentColor = color;
			GUILayout.BeginVertical();
			GUILayout.Space(upperSpacing);
			GUILayout.Label(icon, "IconHolder");
			GUILayout.EndVertical();
			GUI.contentColor = prevColor;
		}

		public static int IntAttributeGUI(string label, int val, bool willUseRepeatButton = false, Action<int> intCallback = null, Func<int, string> validationCallback = null)
		{
			int temp;
			string warningMessage = "";
			bool validInput = true;
			// Prepare Validation
			if (validationCallback != null)
			{
				warningMessage = validationCallback(val);
				validInput = string.IsNullOrEmpty(warningMessage);

			}

			GUILayout.BeginHorizontal();
			GUILayout.Label(label + ": ");
			string s;
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				s = EditorGUILayout.TextField(string.Empty, val.ToString(), (validInput) ? DEFAULT_TEXT_FIELD_STYLE_NAME : WARNING_TEXT_FIELD_STYLE_NAME);
			}
			else
			{
#endif
				s = GUILayout.TextField(val.ToString(), (willUseRepeatButton) ? 7 : 3, (validInput) ? DEFAULT_TEXT_FIELD_STYLE_NAME : WARNING_TEXT_FIELD_STYLE_NAME);
#if UNITY_EDITOR
			}
#endif

			Int32.TryParse(s, out temp);

			if (((willUseRepeatButton) ? GUILayout.RepeatButton("+") : GUILayout.Button("+"))) temp += 1;
			if (((willUseRepeatButton) ? GUILayout.RepeatButton("-") : GUILayout.Button("-"))) temp -= 1;

			GUILayout.EndHorizontal();

			if (!validInput) GUILayout.Label(warningMessage, ERROR_LABEL_STYLE_NAME);

			// Value has changed
			if (val != temp)
			{
				if (intCallback != null) intCallback(temp);
			}


			return temp;
		}


		// ================================================================================================

		public static void DrawDropDownBoxWithDescription<T>(string label, string description, DropDownBox<T> dropDownBox, GUILayoutOption[] options)
		{
			GUILayout.BeginVertical(options);

			GUILayout.BeginHorizontal();
			DrawLabel(label, DROP_DOWN_BOX_LABEL_STYLE_NAME);
			dropDownBox.DrawButton();
			GUILayout.EndHorizontal();

			DrawDescription(description);

			GUILayout.EndVertical();
		}

		public static void DrawDropDownBoxWithDescription<T>(string label, string description,
			DropDownBox<T> dropDownBox)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			DrawLabel(label, DROP_DOWN_BOX_LABEL_STYLE_NAME);
			dropDownBox.DrawButton();
			GUILayout.EndHorizontal();

			DrawDescription(description);

			GUILayout.EndVertical();
		}

		public static void DrawDropDownBox<T>(string label,
			DropDownBox<T> dropDownBox)
		{
			GUILayout.BeginHorizontal();
			DrawLabel(label, DROP_DOWN_BOX_LABEL_STYLE_NAME);
			dropDownBox.DrawButton();
			GUILayout.EndHorizontal();
		}

		public static void DrawDropDownBox<T>(GUIContent label,
			DropDownBox<T> dropDownBox)
		{
			GUILayout.BeginHorizontal();
			DrawLabel(label, DROP_DOWN_BOX_LABEL_STYLE_NAME);
			dropDownBox.DrawButton();
			GUILayout.EndHorizontal();
		}

		public static void DrawDropDownBoxWithDescription<T>(GUIContent label, string description,
			DropDownBox<T> dropDownBox)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			GUILayout.Label(label, DROP_DOWN_BOX_LABEL_STYLE_NAME);
			dropDownBox.DrawButton();
			GUILayout.EndHorizontal();

			DrawDescription(description);

			GUILayout.EndVertical();
		}

		public static void DrawDropDownBoxVerticalWithDescription<T>(string label, string description, DropDownBox<T> dropDownBox, GUILayoutOption[] options = null)
		{
			GUILayout.BeginVertical(options);

			DrawLabel(label, DROP_DOWN_BOX_LABEL_STYLE_NAME);
			dropDownBox.DrawButton();

			DrawDescription(description);

			GUILayout.EndVertical();
		}

		public static void DrawDropDownBoxLabel(string label)
		{
			DrawLabel(label, DROP_DOWN_BOX_LABEL_STYLE_NAME);
		}

		public static void DrawDropDownBoxLabel(GUIContent label)
		{
			DrawLabel(label, DROP_DOWN_BOX_LABEL_STYLE_NAME);
		}

		/// <summary>
		/// Draw a button that aligns neatly beside a dropdown box.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static bool DrawDropDownBoxButton(string label, params GUILayoutOption[] options)
		{
			return GUILayout.Button(label, DROP_DOWN_BOX_BUTTON_STYLE_NAME, options);
		}

		/// <summary>
		/// Draw a button that aligns neatly beside a dropdown box.
		/// </summary>
		/// <param name="guiContent"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static bool DrawDropDownBoxButton(GUIContent guiContent, params GUILayoutOption[] options)
		{
			return GUILayout.Button(guiContent, DROP_DOWN_BOX_BUTTON_STYLE_NAME, options);
		}

		/// <summary>
		/// Draw a button that aligns neatly beside a dropdown box.
		/// </summary>
		/// <param name="guiContent"></param>
		/// <returns></returns>
		public static bool DrawDropDownBoxButton(GUIContent guiContent)
		{
			return GUILayout.Button(guiContent, DROP_DOWN_BOX_BUTTON_STYLE_NAME);
		}

		// ================================================================================================

		public static T DrawStringSearchPickerWithDescription<T>(string label, string description, StringSearchPicker<T> stringSearchPicker, T val)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			DrawLabel(label, DROP_DOWN_BOX_LABEL_STYLE_NAME);
			T returnValue = stringSearchPicker.Draw(val);
			GUILayout.EndHorizontal();

			DrawDescription(description);

			GUILayout.EndVertical();

			return returnValue;
		}

		public static T DrawStringSearchPicker<T>(StringSearchPicker<T> stringSearchPicker, T val)
		{
			return stringSearchPicker.Draw(val);
		}

		public static T DrawStringSearchPickerWithRect<T>(Rect textFieldRect, StringSearchPicker<T> stringSearchPicker, T val)
		{
			return stringSearchPicker.Draw(textFieldRect, val);
		}

		// ================================================================================================

		static void GetHeightScaledDimensions(int actualWidth, int actualHeight, float maxWidth, float maxHeight,
			out float scaledWidth, out float scaledHeight)
		{
			scaledWidth = Mathf.Min(actualWidth, maxWidth);
			scaledHeight = Mathf.Min(actualHeight, maxHeight);

			if (actualWidth == actualHeight)
			{
				scaledWidth = scaledHeight;
			}
			else
			{
				if (actualHeight > maxHeight)
				{
					scaledHeight = maxHeight;
					scaledWidth = (scaledHeight/actualHeight) * actualWidth;
				}
				else if (actualWidth > maxWidth)
				{
					scaledWidth = maxWidth;
					scaledHeight = (scaledWidth/actualWidth) * actualHeight;
				}
			}
		}

		public static void GetHeightScaledDimensions(Texture image, float maxWidth, float maxHeight,
			out float scaledWidth, out float scaledHeight)
		{
			if (image == null)
			{
				scaledWidth = 0;
				scaledHeight = 0;
				return;
			}

			GetHeightScaledDimensions(image.width, image.height, maxWidth, maxHeight, out scaledWidth, out scaledHeight);
		}

		public static void DrawHeightScaledImage(Texture image, Color tint, float maxWidth, float maxHeight, GUIStyle style)
		{
			if (image == null)
			{
				return;
			}

			GetHeightScaledDimensions(image, maxWidth, maxHeight, out var usedWidth, out var usedHeight);

			var prevColor = GUI.contentColor;
			GUI.contentColor = tint;
			GUILayout.Box(image, style, GUILayout.Width(usedWidth), GUILayout.Height(usedHeight));
			GUI.contentColor = prevColor;
		}



		static void GetWidthScaledDimensions(int actualWidth, int actualHeight, float maxWidth, float maxHeight,
			out float scaledWidth, out float scaledHeight)
		{
			scaledWidth = Mathf.Min(actualWidth, maxWidth);
			scaledHeight = Mathf.Min(actualHeight, maxHeight);

			if (actualWidth == actualHeight)
			{
				scaledWidth = scaledHeight;
			}
			else
			{
				if (actualWidth > maxWidth)
				{
					scaledWidth = maxWidth;
					scaledHeight = (scaledWidth/actualWidth) * actualHeight;
				}
				else if (actualHeight > maxHeight)
				{
					scaledHeight = maxHeight;
					scaledWidth = (scaledHeight/actualHeight) * actualWidth;
				}
			}
		}

		public static void GetWidthScaledDimensions(Texture2D image, float maxWidth, float maxHeight,
			out float scaledWidth, out float scaledHeight)
		{
			if (image == null)
			{
				scaledWidth = 0;
				scaledHeight = 0;
				return;
			}

			GetWidthScaledDimensions(image.width, image.height, maxWidth, maxHeight, out scaledWidth, out scaledHeight);
		}

		public static void DrawWidthScaledImage(Texture2D image, float maxWidth, float maxHeight, GUIStyle style)
		{
			if (image == null)
			{
				return;
			}

			float usedWidth;
			float usedHeight;
			GetWidthScaledDimensions(image, maxWidth, maxHeight, out usedWidth, out usedHeight);

			GUILayout.Box(image, style, GUILayout.Width(usedWidth), GUILayout.Height(usedHeight));
		}




		static void GetScaledDimensions(int actualWidth, int actualHeight, float maxWidth, float maxHeight,
			out float scaledWidth, out float scaledHeight)
		{
			if (actualWidth <= maxWidth && actualHeight <= maxHeight)
			{
				scaledWidth = actualWidth;
				scaledHeight = actualHeight;
				return;
			}

			scaledWidth = Mathf.Min(actualWidth, maxWidth);
			scaledHeight = Mathf.Min(actualHeight, maxHeight);

			if (actualWidth == actualHeight)
			{
				scaledWidth = scaledHeight;
			}
			else
			{
				//if (actualWidth > maxWidth && actualHeight > maxHeight)
				{
					var horizScaledW = maxWidth;
					var horizScaledH = (maxWidth/actualWidth) * actualHeight;

					var vertScaledW = (maxHeight/actualHeight) * actualWidth;
					var vertScaledH = maxHeight;

					if (horizScaledH <= maxHeight)
					{
						scaledWidth = horizScaledW;
						scaledHeight = horizScaledH;
					}
					if (vertScaledW <= maxWidth)
					{
						scaledWidth = vertScaledW;
						scaledHeight = vertScaledH;
					}
				}
			}
		}

		public static void GetScaledDimensions(Texture2D image, float maxWidth, float maxHeight,
			out float scaledWidth, out float scaledHeight)
		{
			if (image == null)
			{
				scaledWidth = 0;
				scaledHeight = 0;
				return;
			}

			GetScaledDimensions(image.width, image.height, maxWidth, maxHeight, out scaledWidth, out scaledHeight);
		}

		public static void FitRectWithinScreen(TooltipLocationPreference locationPreference, ref Rect adjustedRect)
		{
			FitRectWithinScreen(0, 0, Vector2.zero, locationPreference, ref adjustedRect);
		}

		public static void FitRectWithinScreen(float rectOriginalXPos, float rectOriginalYPos, TooltipLocationPreference locationPreference, ref Rect adjustedRect)
		{
			FitRectWithinScreen(rectOriginalXPos, rectOriginalYPos, Vector2.zero, locationPreference, ref adjustedRect);
		}

		public static void FitRectWithinScreen(float rectOriginalXPos, float rectOriginalYPos, Vector2 screenOffset, TooltipLocationPreference locationPreference, ref Rect adjustedRect)
		{
			// ------------------------------------------------
			// move tooltip to bottom if rect is way below screen

			int availableHeight = Screen.height - 40;
			if (adjustedRect.yMax > availableHeight)
			{
				adjustedRect.y = availableHeight - adjustedRect.height + rectOriginalYPos;
			}

			// ------------------------------------------------

			if (locationPreference == TooltipLocationPreference.Right)
			{
				// move tooltip to left if not enough space at right
				if (adjustedRect.xMax > Screen.width)
				{
					adjustedRect.x = rectOriginalXPos - adjustedRect.width;
				}
				// if rect ended up being too far the other side because of this,
				// then just force the x position to the given value
				if (adjustedRect.xMin < 0)
				{
					// if not enough space at right also,
					// then just put the tooltip directly on top of the entry
					adjustedRect.x = rectOriginalXPos;
				}
			}
			else if (locationPreference == TooltipLocationPreference.Bottom)
			{
				// move tooltip to left if not enough space at right
				if (adjustedRect.xMax - screenOffset.x > Screen.width)
				{
					adjustedRect.x = Screen.width - adjustedRect.width + screenOffset.x;
				}
				// if rect ended up being too far the other side because of this,
				// then just force the x position to the given value
				if (adjustedRect.xMin < 0)
				{
					// if not enough space at right also,
					// then just put the tooltip directly on top of the entry
					adjustedRect.x = rectOriginalXPos;
				}
			}
			else
			{
				// every other tooltip location preference

				// move tooltip to right if not enough space at left
				if (adjustedRect.xMin < 0)
				{
					adjustedRect.x = rectOriginalXPos + adjustedRect.width;
				}
				// if rect ended up being too far the other side because of this,
				// then just force the x position to the given value
				if (adjustedRect.xMax > Screen.width)
				{
					// if not enough space at left also,
					// then just put the tooltip directly on top of the entry
					adjustedRect.x = rectOriginalXPos;
				}
			}
		}

		public static (float, float) GetMaxNumberingSize(int count, GUIContent content, GUIStyle style)
		{
			float maxNumberWidth = 0;
			float maxNumberHeight = 0;

			for (int n = 0; n < count; ++n)
			{
				content.text = $"{(n+1).ToString()}.";
				var numberSize = style.CalcSize(content);
				maxNumberWidth = Mathf.Max(maxNumberWidth, numberSize.x);
				maxNumberHeight = Mathf.Max(maxNumberHeight, numberSize.y);
			}

			return (maxNumberWidth, maxNumberHeight);
		}
	}

}