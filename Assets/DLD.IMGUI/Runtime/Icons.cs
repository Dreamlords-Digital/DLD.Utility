// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;
using UnityEditor;

namespace DLD.IMGUI
{
	public static class Icons
	{
		// ================================================================================================

		static Texture2D GetIcon(GUISkin guiSkin, string iconName)
		{
#if UNITY_EDITOR
			if (guiSkin.FindStyle(iconName) == null)
			{
				Debug.LogWarning($"{iconName} not found in GUIStyle", guiSkin);
				return null;
			}
#endif

			var style = guiSkin.GetStyle(iconName);
			if (style == null)
			{
				Debug.LogWarning($"{iconName} not found in GUIStyle", guiSkin);
				return null;
			}
			return style.normal.background;
		}

		public enum StyleState
		{
			Normal,
			Hover,
			Active,
			Focused,
			OnNormal,
			OnHover,
			OnActive,
			OnFocused,
		}

		static Texture2D GetIcon(GUISkin guiSkin, StyleState styleState, string iconName)
		{
#if UNITY_EDITOR
			if (guiSkin.FindStyle(iconName) == null)
			{
				Debug.LogWarning($"{iconName} not found in GUIStyle", guiSkin);
				return null;
			}
#endif

			var style = guiSkin.GetStyle(iconName);
			if (style == null)
			{
				Debug.LogWarning($"{iconName} not found in GUIStyle", guiSkin);
				return null;
			}

			switch (styleState)
			{
				case StyleState.Normal:
					return style.normal.background;
				case StyleState.Hover:
					return style.hover.background;
				case StyleState.Active:
					return style.active.background;
				case StyleState.Focused:
					return style.focused.background;
				case StyleState.OnNormal:
					return style.onNormal.background;
				case StyleState.OnHover:
					return style.onHover.background;
				case StyleState.OnActive:
					return style.onActive.background;
				case StyleState.OnFocused:
					return style.onFocused.background;
				default:
					return null;
			}
		}

		// ================================================================================================

		static readonly GUIContent NewAsset = new GUIContent();
		static readonly GUIContent AssignCurrent = new GUIContent();
		static readonly GUIContent ChangeAsset = new GUIContent();
		static readonly GUIContent EditAsset = new GUIContent();
		static readonly GUIContent ClearAsset = new GUIContent();

		static readonly GUIContent CharacterEntityLabel = new GUIContent();
		static readonly GUIContent ScenarioLabel = new GUIContent();
		static readonly GUIContent ChangeCharacterLabel = new GUIContent("Change");
		static readonly GUIContent ClearCharacterLabel = new GUIContent("Clear");
		static readonly GUIContent WarningIconLabel = new GUIContent();
		static readonly GUIContent ErrorIconLabel = new GUIContent();

		public static Texture2D UnityIcon;
		public static Texture2D ExplorerIcon;
		public static Texture2D NewFileIcon;
		public static Texture2D OpenFileIcon;
		public static Texture2D RecentlyOpenedIcon;
		public static Texture2D EditFileIcon;
		public static Texture2D SaveFileIcon;
		public static Texture2D SaveFileAsIcon;
		public static Texture2D UndoIcon;
		public static Texture2D ReadMeIcon;
		public static Texture2D ExportIcon;
		public static Texture2D OpenEditorIcon;
		public static Texture2D FileHistoryIcon;
		public static Texture2D ModalIcon;
		public static Texture2D ShowDebugIcon;
		public static Texture2D CancelIcon;
		public static Texture2D CutIcon;
		public static Texture2D CopyIcon;
		public static Texture2D PasteIcon;
		public static Texture2D ClearIcon;
		public static Texture2D DeleteIcon;
		public static Texture2D RemoveIcon;
		public static Texture2D DiceIcon;
		public static Texture2D WaitTimeIcon;
		public static Texture2D InfoIcon;
		public static Texture2D WarningIcon;
		public static Texture2D ErrorIcon;
		public static Texture2D AsteriskIcon;
		public static Texture2D RefreshIcon;
		public static Texture2D PlusIcon;
		public static Texture2D MinusIcon;
		public static Texture2D MoveUpIcon;
		public static Texture2D MoveDownIcon;
		public static Texture2D FilterIcon;
		public static Texture2D ConnectIcon;
		public static Texture2D ResetPanIcon;
		public static Texture2D OverridesIcon;
		public static Texture2D BringToFrontIcon;
		public static Texture2D SendToBackIcon;
		public static Texture2D LockIcon;
		public static Texture2D UnlockIcon;
		public static Texture2D VisibleIcon;
		public static Texture2D NotVisibleIcon;
		public static Texture2D GlobalIcon;
		public static Texture2D NotGlobalIcon;
		public static Texture2D BrowserListEntryAdd;
		public static Texture2D BrowserListEntryRemove;
		public static Texture2D WeaponLoadAmmo;
		public static Texture2D WeaponUnloadAmmo;
		public static Texture2D WeaponFixAmmoCapacity;

		public static Texture2D ModPackageIcon;
		public static Texture2D CharacterIcon;
		public static Texture2D SkillIcon;
		public static Texture2D AbilityIcon;
		public static Texture2D AttacksIcon;
		public static Texture2D StatusEffectIcon;
		public static Texture2D ItemIcon;
		public static Texture2D ItemAddIcon;
		public static Texture2D ItemRemoveIcon;
		public static Texture2D EquipmentIcon;
		public static Texture2D ShopIcon;
		public static Texture2D ShopAddIcon;
		public static Texture2D ShopRemoveIcon;
		public static Texture2D RaceIcon;
		public static Texture2D PathIcon;
		public static Texture2D FactionIcon;
		public static Texture2D ConversationIcon;
		public static Texture2D QuestIcon;
		public static Texture2D ScenarioIcon;
		public static Texture2D EncounterIcon;
		public static Texture2D RandomEncounterIcon;
		public static Texture2D BehaviourTreeIcon;
		public static Texture2D CharacterInteractionIcon;
		public static Texture2D CharacterInteractionAfterDefeatIcon;
		public static Texture2D CharacterInteractionNoneIcon;
		public static Texture2D TeamsIcon;
		public static Texture2D RandomItemsIcon;
		public static Texture2D ReloadCostIcon;
		public static Texture2D AnimationClipIcon;
		public static Texture2D SoundsIcon;
		public static Texture2D SpritesIcon;
		public static Texture2D BulletTrailIcon;
		public static Texture2D MoneyIcon;
		public static Texture2D AwardIcon;
		public static Texture2D VictoryIcon;
		public static Texture2D SurrenderIcon;
		public static Texture2D InitiativeIcon;
		public static Texture2D PlayerDeadIcon;
		public static Texture2D EnemyDeadIcon;
		public static Texture2D CharacterDeadIcon;
		public static Texture2D PlayerAliveIcon;
		public static Texture2D EnemyAliveIcon;
		public static Texture2D CharacterAliveIcon;
		public static Texture2D SuccessIcon;
		public static Texture2D RepeatIcon;
		public static Texture2D RepeatSuccessIcon;
		public static Texture2D RepeatFailureIcon;
		public static Texture2D SequenceIcon;
		public static Texture2D OptionalSequenceIcon;
		public static Texture2D ParallelIcon;
		public static Texture2D SelectorIcon;
		public static Texture2D InvertIcon;
		public static Texture2D PlaceholderIcon;
		public static Texture2D ComparisonIcon;

		public static Texture2D GameStartIcon;
		public static Texture2D TagIcon;
		public static Texture2D ExpTableIcon;
		public static Texture2D CharacterRulesIcon;
		public static Texture2D MissionModeIcon;
		public static Texture2D GameLabelsIcon;
		public static Texture2D CharacterCreationIcon;

		public static Texture2D FaceNorthIcon;
		public static Texture2D FaceNorthEastIcon;
		public static Texture2D FaceEastIcon;
		public static Texture2D FaceSouthEastIcon;
		public static Texture2D FaceSouthIcon;
		public static Texture2D FaceSouthWestIcon;
		public static Texture2D FaceWestIcon;
		public static Texture2D FaceNorthWestIcon;

		public static Texture2D FormulaIcon;
		public static Texture2D MaleIcon;
		public static Texture2D FemaleIcon;
		public static Texture2D HealthIcon;
		public static Texture2D GameDataCheckIcon;
		public static Texture2D GameDataChangeIcon;
		public static Texture2D MapMarkerIcon;
		public static Texture2D ClearMapMarkerIcon;

		public static Texture2D MapIcon;
		public static Texture2D GridIcon;
		public static Texture2D ResizeGridIcon;
		public static Texture2D PlainTileIcon;
		public static Texture2D UnwalkableTileIcon;
		public static Texture2D VictoryConditionsIcon;
		public static Texture2D SpawnIcon;
		public static Texture2D InteractiveAreaIcon;
		public static Texture2D ItemContainerIcon;
		public static Texture2D ExitPointIcon;
		public static Texture2D StartingPositionIcon;

		public static Texture2D OnEnterIcon;
		public static Texture2D OnExitIcon;
		public static Texture2D WhileInsideIcon;

		public static Texture2D CombatIcon;
		public static Texture2D CombatAiIcon;
		public static Texture2D NonCombatAiIcon;
		public static Texture2D CharacterIdleIcon;
		public static Texture2D StopMovementIcon;
		public static Texture2D LineOfSightIcon;
		public static Texture2D EnergyIcon;
		public static Texture2D LevelIcon;
		public static Texture2D ProximityIcon;
		public static Texture2D AttributeIcon;

		public static Texture2D AlliesIcon;
		public static Texture2D MakeAlliesIcon;
		public static Texture2D EnemiesIcon;
		public static Texture2D MakeEnemiesIcon;
		public static Texture2D NeutralIcon;
		public static Texture2D MakeNeutralIcon;

		public static Texture2D QuestObjectiveIcon;
		public static Texture2D QuestOngoingIcon;
		public static Texture2D QuestSuccessIcon;
		public static Texture2D QuestFailedIcon;
		public static Texture2D QuestBotchedIcon;

		public static Texture2D QuestLogIcon;
		public static Texture2D QuestRewardIcon;

		public static Texture2D QuestObjectiveNewIcon;
		public static Texture2D QuestObjectiveOngoingIcon;
		public static Texture2D QuestObjectiveSuccessRequiredIcon;
		public static Texture2D QuestObjectiveFailRequiredIcon;
		public static Texture2D QuestObjectiveAlreadySuccessIcon;
		public static Texture2D QuestObjectiveAlreadyFailedIcon;

		public static Texture2D QuestTransitionAllPassIcon;
		public static Texture2D QuestTransitionNoneIcon;
		public static Texture2D QuestTransitionSpecificPassIcon;
		public static Texture2D QuestTransitionSpecificFailIcon;
		public static Texture2D QuestTransitionSpecificPassFailIcon;

		public static Texture2D ConversationToCombatIcon;
		public static Texture2D SetConversationEntryIcon;
		public static Texture2D ConversationRequirementIcon;
		public static Texture2D ConversationEffectIcon;
		public static Texture2D ConversationSpriteIcon;
		public static Texture2D ConversationImageIcon;

		public static Texture2D NodeEntryIcon;
		public static Texture2D NodeEndIcon;
		public static Texture2D NodePropertiesIcon;
		public static Texture2D NodeConnectionsIcon;
		public static Texture2D NodeTraversalIcon;
		public static Texture2D NodeCommentIcon;
		public static Texture2D NodeArrowImage;
		public static Texture2D NodeArrowOutlineImage;
		public static Texture2D NodeDoubleArrowImage;
		public static Texture2D NodeDoubleArrowOutlineImage;
		public static Texture2D NodeCrossImage;
		public static Texture2D NodeCrossOutlineImage;

		[InitializeOnLoadMethod]
		static void OnProjectLoadedInEditor()
		{
			Initialize(IMGUI.Utility.GetDefaultGUISkin());
		}

		public static void Initialize(GUISkin guiSkin)
		{
			UnityIcon = GetIcon(guiSkin, "Icon.Unity");
			ExplorerIcon = GetIcon(guiSkin, "Icon.Explorer");
			NewFileIcon = GetIcon(guiSkin, "Icon.NewFile");
			OpenFileIcon = GetIcon(guiSkin, "Icon.OpenFile");
			RecentlyOpenedIcon = GetIcon(guiSkin, "Icon.RecentlyOpened");
			EditFileIcon = GetIcon(guiSkin, "Icon.EditFile");
			SaveFileIcon = GetIcon(guiSkin, "Icon.SaveFile");
			SaveFileAsIcon = GetIcon(guiSkin, "Icon.SaveFileAs");
			UndoIcon = GetIcon(guiSkin, "Icon.Undo");
			ReadMeIcon = GetIcon(guiSkin, "Icon.Readme");
			ExportIcon = GetIcon(guiSkin, "Icon.Export");
			OpenEditorIcon = GetIcon(guiSkin, "Icon.OpenEditor");
			FileHistoryIcon = GetIcon(guiSkin, "Icon.History");
			ModalIcon = GetIcon(guiSkin, "Icon.Modal");
			ShowDebugIcon = GetIcon(guiSkin, "Icon.ShowDebug");
			CancelIcon = GetIcon(guiSkin, "Icon.Cancel");
			CutIcon = GetIcon(guiSkin, "Icon.Cut");
			CopyIcon = GetIcon(guiSkin, "Icon.Copy");
			PasteIcon = GetIcon(guiSkin, "Icon.Paste");
			ClearIcon = GetIcon(guiSkin, "Icon.Clear");
			DeleteIcon = GetIcon(guiSkin, "Icon.Delete");
			RemoveIcon = GetIcon(guiSkin, "Icon.Remove");
			DiceIcon = GetIcon(guiSkin, "Icon.Dice");
			WaitTimeIcon = GetIcon(guiSkin, "Icon.WaitTime");
			InfoIcon = GetIcon(guiSkin, "Icon.Info");
			WarningIcon = GetIcon(guiSkin, "Icon.Warning");
			ErrorIcon = GetIcon(guiSkin, "Icon.Error");
			AsteriskIcon = GetIcon(guiSkin, "Icon.Asterisk");
			RefreshIcon = GetIcon(guiSkin, "Icon.Refresh");
			PlusIcon = GetIcon(guiSkin, "Icon.Plus");
			MinusIcon = GetIcon(guiSkin, "Icon.Minus");
			MoveUpIcon = GetIcon(guiSkin, "Icon.MoveUp");
			MoveDownIcon = GetIcon(guiSkin, "Icon.MoveDown");
			FilterIcon = GetIcon(guiSkin, "Icon.Filter");
			ConnectIcon = GetIcon(guiSkin, "Icon.Connect");
			ResetPanIcon = GetIcon(guiSkin, "Icon.ResetPan");
			OverridesIcon = GetIcon(guiSkin, "Icon.Overrides");
			BringToFrontIcon = GetIcon(guiSkin, "Icon.BringToFront");
			SendToBackIcon = GetIcon(guiSkin, "Icon.SendToBack");
			LockIcon = GetIcon(guiSkin, "Icon.Lock");
			UnlockIcon = GetIcon(guiSkin, "Icon.Unlock");
			VisibleIcon = GetIcon(guiSkin, "Icon.Visible");
			NotVisibleIcon = GetIcon(guiSkin, "Icon.NotVisible");
			GlobalIcon = GetIcon(guiSkin, "Icon.Globe");
			NotGlobalIcon = GetIcon(guiSkin, "Icon.Globe.Disable");
			BrowserListEntryAdd = GetIcon(guiSkin, "Icon.AbilityList.Add");
			BrowserListEntryRemove = GetIcon(guiSkin, "Icon.AbilityList.Remove");
			WeaponLoadAmmo = GetIcon(guiSkin, "Icon.Weapon.LoadAmmo");
			WeaponUnloadAmmo = GetIcon(guiSkin, "Icon.Weapon.UnloadAmmo");
			WeaponFixAmmoCapacity = GetIcon(guiSkin, "Icon.Weapon.FixAmmoCapacity");

			ModPackageIcon = GetIcon(guiSkin, "Icon.ModPackageWindow");
			CharacterIcon = GetIcon(guiSkin, "Icon.CharacterWindow");
			SkillIcon = GetIcon(guiSkin, "Icon.SkillWindow");
			AbilityIcon = GetIcon(guiSkin, "Icon.AbilityWindow");
			AttacksIcon = GetIcon(guiSkin, "Icon.Attacks");
			StatusEffectIcon = GetIcon(guiSkin, "Icon.ConditionWindow");
			ItemIcon = GetIcon(guiSkin, "Icon.ItemWindow");
			ItemAddIcon = GetIcon(guiSkin, StyleState.Hover, "Icon.ItemWindow");
			ItemRemoveIcon = GetIcon(guiSkin, StyleState.Active, "Icon.ItemWindow");
			EquipmentIcon = GetIcon(guiSkin, "Icon.Equipment");
			ShopIcon = GetIcon(guiSkin, "Icon.ShopWindow");
			ShopAddIcon = GetIcon(guiSkin, StyleState.Hover, "Icon.ShopWindow");
			ShopRemoveIcon = GetIcon(guiSkin, StyleState.Active, "Icon.ShopWindow");
			RaceIcon = GetIcon(guiSkin, "Icon.RaceWindow");
			PathIcon = GetIcon(guiSkin, "Icon.PathWindow");
			FactionIcon = GetIcon(guiSkin, "Icon.FactionWindow");
			ConversationIcon = GetIcon(guiSkin, "Icon.ConversationWindow");
			QuestIcon = GetIcon(guiSkin, "Icon.QuestWindow");
			ScenarioIcon = GetIcon(guiSkin, "Icon.ScenarioWindow");
			EncounterIcon = GetIcon(guiSkin, "Icon.EncounterWindow");
			RandomEncounterIcon = GetIcon(guiSkin, "Icon.RandomEncounterWindow");
			BehaviourTreeIcon = GetIcon(guiSkin, "Icon.BehaviourTree");
			CharacterInteractionIcon = GetIcon(guiSkin, "Icon.CharacterInteraction");
			CharacterInteractionAfterDefeatIcon = GetIcon(guiSkin, "Icon.CharacterInteractionAfterDefeat");
			CharacterInteractionNoneIcon = GetIcon(guiSkin, "Icon.CharacterInteractionNone");
			TeamsIcon = GetIcon(guiSkin, "Icon.Teams");
			RandomItemsIcon = GetIcon(guiSkin, "Icon.RandomItems");
			ReloadCostIcon = GetIcon(guiSkin, "Icon.ReloadCost");
			AnimationClipIcon = GetIcon(guiSkin, "Icon.AnimationClip");
			SoundsIcon = GetIcon(guiSkin, "Icon.Sounds");
			SpritesIcon = GetIcon(guiSkin, "Icon.Sprites");
			BulletTrailIcon = GetIcon(guiSkin, "Icon.BulletTrail");
			MoneyIcon = GetIcon(guiSkin, "Icon.Money");
			AwardIcon = GetIcon(guiSkin, "Icon.Award");
			VictoryIcon = GetIcon(guiSkin, "Icon.Victory");
			SurrenderIcon = GetIcon(guiSkin, "Icon.Surrender");
			InitiativeIcon = GetIcon(guiSkin, "Icon.Initiative");
			PlayerDeadIcon = GetIcon(guiSkin, "Icon.PlayerDead");
			EnemyDeadIcon = GetIcon(guiSkin, "Icon.EnemyDead");
			CharacterDeadIcon = GetIcon(guiSkin, "Icon.CharacterDead");
			PlayerAliveIcon = GetIcon(guiSkin, "Icon.PlayerAlive");
			EnemyAliveIcon = GetIcon(guiSkin, "Icon.EnemyAlive");
			CharacterAliveIcon = GetIcon(guiSkin, "Icon.CharacterAlive");
			SuccessIcon = GetIcon(guiSkin, "Icon.Success");
			RepeatIcon = GetIcon(guiSkin, "Icon.Repeat");
			RepeatSuccessIcon = GetIcon(guiSkin, "Icon.RepeatSuccess");
			RepeatFailureIcon = GetIcon(guiSkin, "Icon.RepeatFailure");
			SequenceIcon = GetIcon(guiSkin, "Icon.Sequence");
			OptionalSequenceIcon = GetIcon(guiSkin, "Icon.OptionalSequence");
			ParallelIcon = GetIcon(guiSkin, "Icon.Parallel");
			SelectorIcon = GetIcon(guiSkin, "Icon.Selector");
			InvertIcon = GetIcon(guiSkin, "Icon.Invert");
			PlaceholderIcon = GetIcon(guiSkin, "Icon.Placeholder");
			ComparisonIcon = GetIcon(guiSkin, "Icon.Comparison");

			GameStartIcon = GetIcon(guiSkin, "Icon.GameStartWindow");
			TagIcon = GetIcon(guiSkin, "Icon.Tag");
			ExpTableIcon = GetIcon(guiSkin, "Icon.ExperienceTableWindow");
			CharacterRulesIcon = GetIcon(guiSkin, "Icon.CharacterRulesWindow");
			MissionModeIcon = GetIcon(guiSkin, "Icon.MissionModeSettingsWindow");
			GameLabelsIcon = GetIcon(guiSkin, "Icon.GameLabelsWindow");
			CharacterCreationIcon = GetIcon(guiSkin, "Icon.CharacterCreationSettingsWindow");

			FaceNorthIcon = GetIcon(guiSkin, "Icon.FaceNorth");
			FaceNorthEastIcon = GetIcon(guiSkin, "Icon.FaceNortheast");
			FaceEastIcon = GetIcon(guiSkin, "Icon.FaceEast");
			FaceSouthEastIcon = GetIcon(guiSkin, "Icon.FaceSoutheast");
			FaceSouthIcon = GetIcon(guiSkin, "Icon.FaceSouth");
			FaceSouthWestIcon = GetIcon(guiSkin, "Icon.FaceSouthwest");
			FaceWestIcon = GetIcon(guiSkin, "Icon.FaceWest");
			FaceNorthWestIcon = GetIcon(guiSkin, "Icon.FaceNorthwest");

			FormulaIcon = GetIcon(guiSkin, "Icon.Formula");
			MaleIcon = GetIcon(guiSkin, "Icon.Male");
			FemaleIcon = GetIcon(guiSkin, "Icon.Female");
			HealthIcon = GetIcon(guiSkin, "Icon.Health");
			GameDataCheckIcon = GetIcon(guiSkin, "Icon.GameDataCheck");
			GameDataChangeIcon = GetIcon(guiSkin, "Icon.GameDataChange");
			MapMarkerIcon = GetIcon(guiSkin, "Icon.MapMarker");
			ClearMapMarkerIcon = GetIcon(guiSkin, "Icon.ClearMapMarker");

			MapIcon = GetIcon(guiSkin, "Icon.MapWindow");
			GridIcon = GetIcon(guiSkin, "Icon.Grid");
			ResizeGridIcon = GetIcon(guiSkin, "Icon.ResizeGrid");
			PlainTileIcon = GetIcon(guiSkin, "Icon.Tile");
			UnwalkableTileIcon = GetIcon(guiSkin, "Icon.Tile.Unwalkable");
			VictoryConditionsIcon = GetIcon(guiSkin, "Icon.Objectives");
			SpawnIcon = GetIcon(guiSkin, "Icon.Spawn");
			InteractiveAreaIcon = GetIcon(guiSkin, "Icon.InteractiveArea");
			ItemContainerIcon = GetIcon(guiSkin, "Icon.ItemContainer");
			ExitPointIcon = GetIcon(guiSkin, "Icon.MapTransitions");
			StartingPositionIcon = GetIcon(guiSkin, "Icon.StartingPosition");

			OnEnterIcon = GetIcon(guiSkin, "Icon.OnEnter");
			OnExitIcon = GetIcon(guiSkin, "Icon.OnExit");
			WhileInsideIcon = GetIcon(guiSkin, "Icon.WhileInside");

			CombatIcon = GetIcon(guiSkin, "Icon.Combat");
			CombatAiIcon = GetIcon(guiSkin, "Icon.AI.Combat");
			NonCombatAiIcon = GetIcon(guiSkin, "Icon.AI.NonCombat");
			CharacterIdleIcon = GetIcon(guiSkin, "Icon.IdleCharacter");
			StopMovementIcon = GetIcon(guiSkin, "Icon.StopMove");
			LineOfSightIcon = GetIcon(guiSkin, "Icon.LineOfSight");
			EnergyIcon = GetIcon(guiSkin, "Icon.Energy");
			LevelIcon = GetIcon(guiSkin, "Icon.Level");
			ProximityIcon = GetIcon(guiSkin, "Icon.Proximity");
			AttributeIcon = GetIcon(guiSkin, "Icon.Attribute");

			AlliesIcon = GetIcon(guiSkin, "Icon.Allies");
			MakeAlliesIcon = GetIcon(guiSkin, "Icon.MakeAllies");
			EnemiesIcon = GetIcon(guiSkin, "Icon.Enemies");
			MakeEnemiesIcon = GetIcon(guiSkin, "Icon.MakeEnemies");
			NeutralIcon = GetIcon(guiSkin, "Icon.Neutral");
			MakeNeutralIcon = GetIcon(guiSkin, "Icon.MakeNeutral");

			QuestObjectiveIcon = GetIcon(guiSkin, "Icon.QuestObjective");
			QuestOngoingIcon = GetIcon(guiSkin, "Icon.QuestOngoing");
			QuestSuccessIcon = GetIcon(guiSkin, "Icon.QuestSuccess");
			QuestFailedIcon = GetIcon(guiSkin, "Icon.QuestFailed");
			QuestBotchedIcon = GetIcon(guiSkin, "Icon.QuestBotched");

			QuestLogIcon = GetIcon(guiSkin, "Icon.Quest.Logs");
			QuestRewardIcon = GetIcon(guiSkin, "Icon.Quest.Rewards");

			QuestObjectiveNewIcon = GetIcon(guiSkin, "Icon.Quest.Objective.New");
			QuestObjectiveOngoingIcon = GetIcon(guiSkin, "Icon.Quest.Objective.Ongoing");
			QuestObjectiveSuccessRequiredIcon = GetIcon(guiSkin, "Icon.Quest.Objective.Success.Required");
			QuestObjectiveFailRequiredIcon = GetIcon(guiSkin, "Icon.Quest.Objective.Fail.Required");
			QuestObjectiveAlreadySuccessIcon = GetIcon(guiSkin, "Icon.Quest.Objective.Success.Already");
			QuestObjectiveAlreadyFailedIcon = GetIcon(guiSkin, "Icon.Quest.Objective.Failed.Already");

			QuestTransitionAllPassIcon = GetIcon(guiSkin, "Icon.Quest.Transition.AllPass");
			QuestTransitionNoneIcon = GetIcon(guiSkin, "Icon.Quest.Transition.None");
			QuestTransitionSpecificPassIcon = GetIcon(guiSkin, "Icon.Quest.Transition.SpecificPass");
			QuestTransitionSpecificFailIcon = GetIcon(guiSkin, "Icon.Quest.Transition.SpecificFail");
			QuestTransitionSpecificPassFailIcon = GetIcon(guiSkin, "Icon.Quest.Transition.SpecificPassFail");

			ConversationToCombatIcon = GetIcon(guiSkin, "Icon.ConversationToCombat");
			SetConversationEntryIcon = GetIcon(guiSkin, "Icon.SetConversationEntryPoint");
			ConversationRequirementIcon = GameDataCheckIcon;
			ConversationEffectIcon = GameDataChangeIcon;
			ConversationSpriteIcon = GetIcon(guiSkin, "Icon.ConversationSprite");
			ConversationImageIcon = GetIcon(guiSkin, "Icon.ConversationImage");

			NodeEntryIcon = GetIcon(guiSkin, "Icon.NodeEntryPoint");
			NodeEndIcon = GetIcon(guiSkin, "Icon.NodeEndPoint");
			NodePropertiesIcon = GetIcon(guiSkin, "Icon.NodeProperties");
			NodeConnectionsIcon = GetIcon(guiSkin, "Icon.NodeConnections");
			NodeTraversalIcon = GetIcon(guiSkin, "Icon.NodeTraversal");
			NodeCommentIcon = GetIcon(guiSkin, "Icon.Comment");

			NodeArrowImage = GetIcon(guiSkin, "Icon.NodeArrow");
			NodeArrowOutlineImage = GetIcon(guiSkin, "Icon.NodeArrowOutline");
			NodeDoubleArrowImage = GetIcon(guiSkin, "Icon.NodeDoubleArrow");
			NodeDoubleArrowOutlineImage = GetIcon(guiSkin, "Icon.NodeDoubleArrowOutline");
			NodeCrossImage = GetIcon(guiSkin, "Icon.NodeX");
			NodeCrossOutlineImage = GetIcon(guiSkin, "Icon.NodeXOutline");

			// --------------------------------

			CharacterEntityLabel.image = CharacterIcon;
			ScenarioLabel.image = ScenarioIcon;
			ChangeCharacterLabel.image = CharacterIcon;
			ClearCharacterLabel.image = ClearIcon;

			NewAsset.image = NewFileIcon;
			AssignCurrent.image = ResetPanIcon;
			ChangeAsset.image = OpenFileIcon;
			EditAsset.image = OpenEditorIcon;
			ClearAsset.image = ClearIcon;

			WarningIconLabel.image = WarningIcon;
			ErrorIconLabel.image = ErrorIcon;
		}
	}
}