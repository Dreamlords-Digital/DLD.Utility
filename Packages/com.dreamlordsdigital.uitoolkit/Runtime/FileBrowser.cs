// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DLD.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

[UxmlElement]
public partial class FileBrowser : VisualElement, IContextMenuListener
{
	const string TemplateResourcesPath = "DLD UIToolkit/FileBrowser";

	const string OpenIconStyleClass = "dld-icon--title--open";
	const string SaveIconStyleClass = "dld-icon--title--save";

	const string FileSystemNameGroupStyleClass = "dld-file-entry__name-group";
	const string FileSystemIconStyleClass = "dld-file-entry__icon";
	const string FileSystemNameStyleClass = "dld-file-entry__filename";
	const string ToolbarButtonPressedStyleClass = "dld-toolbar-button--pressed";
	const string ListViewEntryStyleClass = "dld-list-view-entry-group";

	const string HereIndicatorStyleClass = "dld-icon--here";

	const string FileSystemEntriesViewNameColumn = "Name";
	const string FileSystemEntriesViewSizeColumn = "Size";
	const string FileSystemEntriesViewIcon = "Icon";
	const string FileSystemEntriesViewName = "Name";

	const float ButtonMenuDefaultHoldDownDuration = 0.4f;

	const int UserFavoritesIDStart = 100;
	const int RecentEntriesIDStart = 200;

	const string ContextMenuRemoveFromFavorites = "RemoveFromFavorites";
	const string ContextMenuAddFavorite = "AddFavorite";
	const string ContextMenuOpenFileExplorer = "OpenFileExplorer";
	const string ContextMenuOpenUsingAssociated = "OpenUsingAssociated";

	// -----------------------------------------

	public enum OperationMode : byte
	{
		/// <summary>
		///    User is allowed to type in a non-existent filename (if they want to save to a new file).
		///    Also, the confirm button is renamed to "Save".
		/// </summary>
		Save,

		/// <summary>
		///    User is only allowed to choose existing files.
		///    Also, the confirm button is renamed to "Open".
		/// </summary>
		Open,
	}

	OperationMode _currentOperationMode = OperationMode.Save;

	// -----------------------------------------

	Action _cancel;
	Action<string> _fileChosen;

	// -----------------------------------------

	public enum FilterType : byte
	{
		/// <summary>
		///    No filter, show all files.
		/// </summary>
		None,

		/// <summary>
		///    Show only image files.
		/// </summary>
		Image,

		/// <summary>
		///    Show only video files.
		/// </summary>
		Video,

		/// <summary>
		///    Show only sound files.
		/// </summary>
		Sound,

		/// <summary>
		///    Use a custom filter (denoted by a string) for what file types to show.
		/// </summary>
		Custom,
	}

	FilterType _currentFilterType = FilterType.None;
	readonly List<string> _customFileFilters = new();

	/// <summary>
	///    When in <see cref="OperationMode.Save"/>, this is what the chosen filename's extension will be.
	/// </summary>
	string _saveFileExtension;

	Func<string, (string, string)> _customFileTypeGetter;

	// -----------------------------------------

	string _currentPath;
	bool _currentPathIsRoot;

	readonly VisualElement _titleIcon;
	readonly Label _title;
	readonly VisualElement _popUpMenuOutside;
	readonly VisualElement _pathHistory;
	readonly ListView _pathHistoryListView;
	readonly VisualElement _jumpMenu;
	readonly TreeView _jumpMenuTreeView;
	protected readonly MultiColumnListView _fileSystemEntriesView;
	readonly Button _backButton;
	readonly Button _forwardButton;
	readonly Toggle _jumpButton;
	readonly Button _exploreButton;
	readonly TextField _currentPathTextField;
	protected readonly FilenameTextField _filenameTextField;
	readonly Button _confirmButton;

	IContextMenu _contextMenu;

	// -----------------------------------------

	int _pathHistoryIdx = -1;
	readonly List<string> _pathHistoryEntries = new();

	bool _pathHistoryListShown;
	float _backButtonHoldTimeStart = -1;
	float _forwardButtonHoldTimeStart = -1;

	// -----------------------------------------

	enum SpecialFolderType : byte
	{
		/// <summary>
		///    A regular folder that can have files inside it.
		/// </summary>
		None,

		/// <summary>
		///    Not an actual folder on the hard drive.
		///    This folder is just a representation for the entire computer.
		/// </summary>
		System,

		/// <summary>
		///    Special folder whose files are displayed on the user's desktop.
		/// </summary>
		Desktop,

		/// <summary>
		///    User's My Documents (home folder).
		/// </summary>
		Personal,

		/// <summary>
		///    Folder of the current Unity project.
		/// </summary>
		UnityProject,

		/// <summary>
		///    Not an actual folder on the hard drive.
		///    Serves as a container for paths that the user has marked as favorite.
		/// </summary>
		Favorites,

		/// <summary>
		///    Not an actual folder on the hard drive.
		///    Serves as a container for paths that the user recently used, in case they want to use them again.
		/// </summary>
		Recent,
	}

	struct JumpMenuEntry
	{
		public string Label;
		public string Path;
		public SpecialFolderType SpecialFolderType;

		public override string ToString()
		{
			if (string.IsNullOrEmpty(Path))
			{
				return Label;
			}

			return Path;
		}
	}

	readonly List<TreeViewItemData<JumpMenuEntry>> _jumpMenuEntries = new();
	readonly List<TreeViewItemData<JumpMenuEntry>> _userFavoritesEntries = new();
	readonly List<TreeViewItemData<JumpMenuEntry>> _recentEntries = new();

	int _nextFreeFavoriteEntryId = UserFavoritesIDStart + 1;

	static string UserFavoritesSavePath
	{
		get
		{
			// the hash is for in case the user has multiple projects with the same name
			string projectPathHash = StringUtil.ComputeMD5Hash(Application.dataPath);

			return $"{FileUtil.UserFolderPath}/DLD/{FileUtil.ProjectFolderName}-{projectPathHash}/FileBrowserFavorites.txt";
		}
	}

	// -----------------------------------------

	enum FileSystemEntryType : byte
	{
		File,
		Folder,
		ToParentFolder
	}

	struct FileSystemEntry
	{
		/// <summary>
		///    Name of folder/file as it appears in the GUI.
		///    For files, this is the full filename, including the file type extension.
		/// </summary>
		public string Name;

		/// <summary>
		///    Whether this is a file or folder.
		/// </summary>
		public FileSystemEntryType EntryType;

		/// <summary>
		///    File size as displayed in the GUI.
		/// </summary>
		public string ReadableSize;

		/// <summary>
		///    Actual value used when sorting by file size.
		/// </summary>
		public long SizeBytes;
	}

	readonly List<FileSystemEntry> _fileSystemEntries = new();

	protected string CurrentPath => _currentPath;

	// -----------------------------------------

	public FileBrowser()
	{
		var asset = Resources.Load<VisualTreeAsset>(TemplateResourcesPath);
		asset.CloneTree(this);

		var titleBar = this.Q<VisualElement>("TitleBar");
		_titleIcon = titleBar.Q<VisualElement>("Icon");
		_title = titleBar.Q<Label>("Title");

		_fileSystemEntriesView = this.Q<MultiColumnListView>();
		_fileSystemEntriesView.itemsSource = _fileSystemEntries;

		_currentPathTextField = this.Q<TextField>("PathTextField");
		_currentPathTextField.RegisterCallback((KeyDownEvent e, FileBrowser f) => f.OnPathTextFieldKeyDown(e), this);
		_currentPathTextField.RegisterCallback((FocusOutEvent e, FileBrowser f) => f.OnPathTextFieldFocusOut(e), this);

		_filenameTextField = this.Q<FilenameTextField>();
		_filenameTextField.SetEditCallback(OnEditFileTextField);
		_filenameTextField.SetConfirmCallback(ProcessConfirmedFile);

		_confirmButton = this.Q<Button>("Confirm");
		_confirmButton.RegisterCallback((ClickEvent e, FileBrowser f) => f.OnPressConfirm(e), this);

		var cancelButton = this.Q<Button>("Cancel");
		cancelButton.RegisterCallback((ClickEvent e, FileBrowser f) => f.OnPressCancel(e), this);

		// ---------------------------------------------------

		_backButton = this.Q<Button>("Back");
		_backButton.RegisterCallback((MouseDownEvent e, FileBrowser f) => f.OnMouseDownBackHistory(e), this, TrickleDown.TrickleDown);
		_backButton.RegisterCallback((MouseUpEvent e, FileBrowser f) => f.OnMouseUpBackHistory(e), this);
		_backButton.RegisterCallback((MouseLeaveEvent e, FileBrowser f) => f.OnMouseLeaveBackHistory(e), this);
		_backButton.SetEnabled(false);

		_forwardButton = this.Q<Button>("Forward");
		_forwardButton.RegisterCallback((MouseDownEvent e, FileBrowser f) => f.OnMouseDownForwardHistory(e), this, TrickleDown.TrickleDown);
		_forwardButton.RegisterCallback((MouseUpEvent e, FileBrowser f) => f.OnMouseUpForwardHistory(e), this);
		_forwardButton.RegisterCallback((MouseLeaveEvent e, FileBrowser f) => f.OnMouseLeaveForwardHistory(e), this);
		_forwardButton.SetEnabled(false);

		_jumpButton = this.Q<Toggle>("Jump");
		_jumpButton?.RegisterCallback((ChangeEvent<bool> e, FileBrowser f) => f.OnPressJumpButton(e), this);

		_exploreButton = this.Q<Button>("Explore");
		_exploreButton.RegisterCallback((ClickEvent e, FileBrowser f) => f.OnPressExploreButton(e), this);

		// ---------------------------------------------------

		_popUpMenuOutside = this.Q<VisualElement>("PopUpMenuOutside");
		_popUpMenuOutside.style.display = DisplayStyle.None;
		_popUpMenuOutside.RegisterCallback((ClickEvent e, FileBrowser f) => f.OnPressOutsidePopUpMenu(e), this);

		// ---------------------------------------------------

		_pathHistory = _popUpMenuOutside.Q<VisualElement>("PathHistory");
		_pathHistory.style.display = DisplayStyle.None;
		_pathHistory.RegisterCallback((ClickEvent e, FileBrowser f) => f.OnPressInsidePopUpMenu(e), this);

		_pathHistoryListView = _pathHistory.Q<ListView>();

		// ---------------------------------------------------

		_jumpMenu = _popUpMenuOutside.Q<VisualElement>("JumpMenu");
		_jumpMenu.style.display = DisplayStyle.None;
		_jumpMenu.RegisterCallback((ClickEvent e, FileBrowser f) => f.OnPressInsidePopUpMenu(e), this);

		_jumpMenuTreeView = _jumpMenu.Q<TreeView>();

		// ---------------------------------------------------

		_pathHistoryListView.itemsSource = _pathHistoryEntries;
		_pathHistoryListView.Clear();

		const string HereIndicatorName = "Here";

		_pathHistoryListView.makeItem = () =>
		{
			// create row with icon and label
			var group = new VisualElement();
			var hereIndicator = new VisualElement();
			var label = new Label();

			hereIndicator.name = HereIndicatorName;

			group.AddToClassList(ListViewEntryStyleClass);
			hereIndicator.AddToClassList(FileSystemIconStyleClass);
			label.AddToClassList(FileSystemNameStyleClass);

			group.Add(hereIndicator);
			group.Add(label);
			return group;
		};

		_pathHistoryListView.bindItem = (visualElement, idx) =>
		{
			var label = visualElement.Q<Label>();
			label.text = _pathHistoryEntries[idx].GetShortFolderName();

			var hereIndicator = visualElement.Q<VisualElement>(HereIndicatorName);
			if (idx == _pathHistoryIdx)
			{
				hereIndicator.AddToClassList(HereIndicatorStyleClass);
			}
			else
			{
				hereIndicator.RemoveFromClassList(HereIndicatorStyleClass);
			}
		};

		_pathHistoryListView.selectionType = SelectionType.Single;
		_pathHistoryListView.selectedIndicesChanged += OnChosePathHistoryEntry;

		// ---------------------------------------------------

		RebuildJumpMenuEntries();
		_jumpMenuTreeView.SetRootItems(_jumpMenuEntries);

		// ---------------------------------------------------

		_jumpMenuTreeView.selectionType = SelectionType.Single;
		_jumpMenuTreeView.selectedIndicesChanged += OnChoseJumpMenuEntry;

		_jumpMenuTreeView.makeItem = () =>
		{
			var group = new VisualElement();
			var icon = new VisualElement();
			var label = new Label();

			icon.name = FileSystemEntriesViewIcon;

			group.Add(icon);
			group.Add(label);

			group.AddToClassList(ListViewEntryStyleClass);
			icon.AddToClassList(FileSystemIconStyleClass);
			label.AddToClassList(FileSystemNameStyleClass);

			label.displayTooltipWhenElided = true;

			return group;
		};

		_jumpMenuTreeView.bindItem = (visualElement, idx) =>
		{
			var jumpMenuEntry = _jumpMenuTreeView.GetItemDataForIndex<JumpMenuEntry>(idx);
			int jumpMenuEntryId = _jumpMenuTreeView.GetIdForIndex(idx);
			var label = visualElement.Q<Label>();
			label.text = jumpMenuEntry.Label;

			string properTooltip = jumpMenuEntry.SpecialFolderType switch
			{
				SpecialFolderType.Desktop => Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ConvertBackToForwardSlash(),
				SpecialFolderType.Personal => Environment.GetFolderPath(Environment.SpecialFolder.Personal).ConvertBackToForwardSlash(),
				SpecialFolderType.UnityProject => FileUtil.ProjectPath,
				_ => jumpMenuEntry.Path
			};

			label.tooltip = properTooltip;

			var icon = visualElement.Q<VisualElement>(FileSystemEntriesViewIcon);
			if (icon != null)
			{
				string iconStyleClass = jumpMenuEntry.SpecialFolderType switch
				{
					SpecialFolderType.System => "dld-icon--file-entry--folder-system",
					SpecialFolderType.Desktop => "dld-icon--file-entry--folder-desktop",
					SpecialFolderType.Personal => "dld-icon--file-entry--folder-home",
					SpecialFolderType.UnityProject => "dld-icon--file-entry--folder-unity-project",
					SpecialFolderType.Favorites => "dld-icon--file-entry--folder-favorites",
					SpecialFolderType.Recent => "dld-icon--file-entry--folder-recent",
					_ => "dld-icon--file-entry--folder-generic"
				};
				icon.AddToClassList(iconStyleClass);
				icon.tooltip = properTooltip;
			}

			if (jumpMenuEntryId > UserFavoritesIDStart && jumpMenuEntryId < RecentEntriesIDStart)
			{
				// add Context Menu to allow user to remove this Favorite Entry
				visualElement.RegisterCallback((ContextClickEvent e, IContextMenu c) =>
				{
					c.ClearMenu();
					c.AddMenu("Remove from Favorites",
						listener: this, userArg1: ContextMenuRemoveFromFavorites, userArg2: jumpMenuEntry.Path);
					c.Show(e);
				}, _contextMenu, TrickleDown.TrickleDown);
			}
		};

		_jumpMenuTreeView.unbindItem = (visualElement, idx) =>
		{
			var icon = visualElement.Q<VisualElement>(FileSystemEntriesViewIcon);
			if (icon != null)
			{
				icon.ClearClassList();
				icon.AddToClassList(FileSystemIconStyleClass);
			}
		};

		// ---------------------------------------------------

		_fileSystemEntriesView.selectedIndicesChanged += OnSelectedFileSystemEntry;
		_fileSystemEntriesView.itemsChosen += OnChoseFileSystemEntry;

		_fileSystemEntriesView.RegisterCallback((KeyDownEvent e, FileBrowser f) => f.OnPressKey(e), this);
		_fileSystemEntriesView.RegisterCallback((ContextClickEvent e, FileBrowser f) => f.OnContextClick(e), this);

		_fileSystemEntriesView.sortingMode = ColumnSortingMode.Custom;
		_fileSystemEntriesView.columnSortingChanged += OnSortingChanged;
		_fileSystemEntriesView.columns.reorderable = false;

		// ---------------------------------------------------

		var nameColumn = _fileSystemEntriesView.columns[FileSystemEntriesViewNameColumn];
		nameColumn.makeCell = () =>
		{
			var group = new VisualElement();
			var icon = new VisualElement();
			var fileNameLabel = new Label();

			icon.name = FileSystemEntriesViewIcon;

			group.Add(icon);
			group.Add(fileNameLabel);

			group.AddToClassList(FileSystemNameGroupStyleClass);
			icon.AddToClassList(FileSystemIconStyleClass);
			fileNameLabel.AddToClassList(FileSystemNameStyleClass);

			fileNameLabel.displayTooltipWhenElided = true;

			// e.currentTarget = the entry's container (the `group` variable)
			// e.currentTarget.userData = the entry's idx (assigned in bindCell)
			group.RegisterCallback((ContextClickEvent e, FileBrowser f) =>
				f.OnContextClickEntry(e, (int)((VisualElement)e.currentTarget).userData), this);

			return group;
		};
		nameColumn.bindCell = (visualElement, idx) =>
		{
			visualElement.userData = idx;
			var fileSysEntry = _fileSystemEntries[idx];

			var filename = visualElement.Q<Label>();
			if (filename != null)
			{
				filename.text = fileSysEntry.Name;
			}

			var icon = visualElement.Q<VisualElement>(FileSystemEntriesViewIcon);
			if (icon != null)
			{
				string iconStyleClass = GetIconStyleClass(fileSysEntry.Name, fileSysEntry.EntryType);
				icon.AddToClassList(iconStyleClass);
			}
		};
		nameColumn.unbindCell = (visualElement, idx) =>
		{
			var icon = visualElement.Q<VisualElement>(FileSystemEntriesViewIcon);
			if (icon != null)
			{
				icon.ClearClassList();
				icon.AddToClassList(FileSystemIconStyleClass);
			}
		};
		nameColumn.optional = false;
		nameColumn.sortable = true;
		nameColumn.width = Length.Percent(100);
		nameColumn.stretchable = true;

		// ---------------------------------------------------

		var sizeColumn = _fileSystemEntriesView.columns[FileSystemEntriesViewSizeColumn];
		sizeColumn.makeCell = () =>
		{
			var sizeLabel = new Label();
			sizeLabel.AddToClassList("dld-file-entry__size");

			// e.currentTarget = the sizeLabel
			// e.currentTarget.userData = the entry's idx (assigned in bindCell)
			sizeLabel.RegisterCallback((ContextClickEvent e, FileBrowser f) =>
				f.OnContextClickEntry(e, (int)((VisualElement)e.currentTarget).userData), this);

			return sizeLabel;
		};
		sizeColumn.bindCell = (visualElement, idx) =>
		{
			visualElement.userData = idx;
			var fileSysEntry = _fileSystemEntries[idx];
			var sizeLabel = visualElement.Q<Label>();
			sizeLabel.text = fileSysEntry.ReadableSize;
		};
		sizeColumn.sortable = true;
		sizeColumn.width = 65;
	}

	public void SetCustomFileTypeGetter(Func<string, (string, string)> newCallback)
	{
		_customFileTypeGetter = newCallback;
	}

	public void Show()
	{
		style.display = DisplayStyle.Flex;
		_fileSystemEntriesView.ClearSelection();
		_filenameTextField.Focus();

		if (_currentOperationMode == OperationMode.Open)
		{
			_filenameTextField.SetFilename(null, null, null);
		}

		ReloadCurrentPath();
	}

	public void SetTitle(string newTitle)
	{
		_title.text = newTitle;
	}

	public void SetContextMenu(IContextMenu contextMenu)
	{
		_contextMenu = contextMenu;
	}

	public void SetOperationMode(OperationMode newMode)
	{
		_currentOperationMode = newMode;

		_titleIcon.ClearClassList();
		_titleIcon.AddToClassList(BaseIcons.IconStyleClass);

		switch (_currentOperationMode)
		{
			case OperationMode.Open:
				_confirmButton.text = "Open";
				_titleIcon.AddToClassList(OpenIconStyleClass);
				break;
			case OperationMode.Save:
				_confirmButton.text = "Save";
				_titleIcon.AddToClassList(SaveIconStyleClass);
				break;
			default:
				_confirmButton.text = "Ok";
				break;
		}
	}

	public void SetFilterType(FilterType newFilter)
	{
		_currentFilterType = newFilter;

		if (!string.IsNullOrEmpty(_currentPath))
		{
			ReloadCurrentPath();
		}
	}

	public void SetFilterType(IEnumerable<string> customFileFilters)
	{
		_currentFilterType = FilterType.Custom;
		_customFileFilters.Clear();
		_customFileFilters.AddRange(customFileFilters);

		if (!string.IsNullOrEmpty(_currentPath))
		{
			ReloadCurrentPath();
		}
	}

	public void SetFilterType(string customFileFilter)
	{
		_currentFilterType = FilterType.Custom;
		_customFileFilters.Clear();
		_customFileFilters.Add(customFileFilter);

		if (!string.IsNullOrEmpty(_currentPath))
		{
			ReloadCurrentPath();
		}
	}

	public void SetSaveFilename(string newFilename)
	{
		_filenameTextField.value = newFilename;
	}

	public void SetSaveFileExtension(string newFileExtension)
	{
		_saveFileExtension = newFileExtension;
		_filenameTextField.Extension = newFileExtension;
	}

	public bool SetInitialPath(string newPath)
	{
		_pathHistoryEntries.Clear();
		return SetPath(newPath);
	}

	public bool SetPath(string newPath)
	{
		if (string.IsNullOrEmpty(newPath))
		{
			return false;
		}

		if (!newPath.IsPathValid())
		{
			return false;
		}

		(bool success, string normalizedPath) = newPath.FixPath();
		if (!success)
		{
			return false;
		}

		if (string.Equals(normalizedPath, _currentPath, StringComparison.Ordinal))
		{
			return false;
		}

		_currentPath = normalizedPath;
		_currentPathTextField.value = _currentPath;

		if (_pathHistoryIdx != -1 && _pathHistoryIdx != _pathHistoryEntries.Count - 1)
		{
			_pathHistoryEntries.RemoveRange(_pathHistoryIdx + 1, _pathHistoryEntries.Count - 1 - _pathHistoryIdx);
		}

		_pathHistoryEntries.Add(_currentPath);
		_pathHistoryIdx = _pathHistoryEntries.Count - 1;

		_backButton.SetEnabled(_pathHistoryIdx > 0);
		_forwardButton.SetEnabled(false);

		ReloadCurrentPath();
		return true;
	}

	public void RegisterFileChosenCallback(Action<string> callback)
	{
		_fileChosen = callback;
	}

	public void RegisterCancelCallback(Action callback)
	{
		_cancel = callback;
	}

	public void Update(float deltaTimeSeconds)
	{
		// show path history list if mouse is pressed down on back/forward button long enough
		if (!_pathHistoryListShown)
		{
			if (_backButtonHoldTimeStart > 0 &&
			    Time.realtimeSinceStartup - _backButtonHoldTimeStart >= ButtonMenuDefaultHoldDownDuration)
			{
				ShowPathHistoryListPopUp(_backButton);
			}
			else if (_forwardButtonHoldTimeStart > 0 &&
			         Time.realtimeSinceStartup - _forwardButtonHoldTimeStart >= ButtonMenuDefaultHoldDownDuration)
			{
				ShowPathHistoryListPopUp(_forwardButton);
			}
		}
	}

	// =====================================================================

	void OnMouseDownBackHistory(MouseDownEvent e)
	{
		if (e.button != 0)
		{
			return;
		}

		_pathHistoryListShown = false;
		_backButtonHoldTimeStart = Time.realtimeSinceStartup;
	}

	void OnMouseLeaveBackHistory(MouseLeaveEvent e)
	{
		if (_backButtonHoldTimeStart <= 0)
		{
			// user did not mouse down on the button, should not respond to the event
			return;
		}

		var button = (VisualElement)e.target;
		if (!button.enabledInHierarchy)
		{
			// button is disabled, should not respond to the event
			return;
		}

		if (e.localMousePosition.y > button.contentRect.height)
		{
			ShowPathHistoryListPopUp(_backButton);
		}
	}

	void OnMouseUpBackHistory(MouseUpEvent e)
	{
		if (e.button != 0)
		{
			return;
		}

		if (_pathHistoryListShown)
		{
			// history list is shown
			// button shouldn't do anything, we have to wait for the history list to be closed
			return;
		}

		_backButtonHoldTimeStart = -1;

		var button = (VisualElement)e.target;
		bool mouseIsInButton = button.contentRect.Contains(e.localMousePosition);
		if (!mouseIsInButton)
		{
			// we only proceed if the mouse up is performed inside the same button as the mouse down
			return;
		}

		if (_pathHistoryIdx <= 0)
		{
			// already at starting path in history
			return;
		}

		_backButton.SetEnabled(_pathHistoryIdx - 1 > 0);
		_forwardButton.SetEnabled(true);

		_pathHistoryIdx -= 1;
		_currentPath = _pathHistoryEntries[_pathHistoryIdx];
		_currentPathTextField.value = _currentPath;
		ReloadCurrentPath();
	}

	// -----------------------------------------

	void OnMouseDownForwardHistory(MouseDownEvent e)
	{
		if (e.button != 0)
		{
			return;
		}

		_pathHistoryListShown = false;
		_forwardButtonHoldTimeStart = Time.realtimeSinceStartup;
	}

	void OnMouseLeaveForwardHistory(MouseLeaveEvent e)
	{
		if (_forwardButtonHoldTimeStart <= 0)
		{
			// user did not mouse down on the button, should not respond to the event
			return;
		}

		var button = (VisualElement)e.target;
		if (!button.enabledInHierarchy)
		{
			// button is disabled, should not respond to the event
			return;
		}

		if (e.localMousePosition.y > button.contentRect.height)
		{
			ShowPathHistoryListPopUp(_forwardButton);
		}
	}

	void OnMouseUpForwardHistory(MouseUpEvent e)
	{
		if (e.button != 0)
		{
			return;
		}

		if (_pathHistoryListShown)
		{
			// history list is shown
			// button shouldn't do anything, we have to wait for the history list to be closed
			return;
		}

		_forwardButtonHoldTimeStart = -1;

		var button = (VisualElement)e.target;
		bool mouseIsInButton = button.contentRect.Contains(e.localMousePosition);
		if (!mouseIsInButton)
		{
			// we only proceed if the mouse up is performed inside the same button as the mouse down
			return;
		}

		if (_pathHistoryIdx >= _pathHistoryEntries.Count - 1)
		{
			// already at last path in history
			return;
		}

		_backButton.SetEnabled(true);
		_forwardButton.SetEnabled(_pathHistoryIdx + 1 < _pathHistoryEntries.Count - 1);

		_pathHistoryIdx += 1;
		_currentPath = _pathHistoryEntries[_pathHistoryIdx];
		_currentPathTextField.value = _currentPath;
		ReloadCurrentPath();
	}

	// -----------------------------------------

	void OnPressOutsidePopUpMenu(ClickEvent e)
	{
		ClosePopUpMenu();
		e.StopPropagation();
	}

	void OnPressInsidePopUpMenu(ClickEvent e)
	{
		e.StopPropagation();
	}

	void OnPressJumpButton(ChangeEvent<bool> e)
	{
		if (e.newValue)
		{
			_jumpMenuTreeView.ClearSelection();

			var buttonRect = _jumpButton.ChangeCoordinatesTo(this, _jumpButton.contentRect);
			_jumpMenu.style.left = buttonRect.x;
			_jumpMenu.style.top = buttonRect.yMax;
		}

		DisplayStyle display = e.newValue ? DisplayStyle.Flex : DisplayStyle.None;
		_popUpMenuOutside.style.display = display;
		_jumpMenu.style.display = display;
	}

	void OnPressExploreButton(ClickEvent e)
	{
		ExplorerUtil.OpenInFileBrowser(_currentPath);
	}

	void OnChosePathHistoryEntry(IEnumerable<int> selectedIdx)
	{
		int idx = _pathHistoryListView.selectedIndex;
		if (idx == -1)
		{
			return;
		}

		ClosePopUpMenu();

		_backButton.SetEnabled(idx > 0);
		_forwardButton.SetEnabled(idx < _pathHistoryEntries.Count - 1);

		_pathHistoryIdx = idx;
		_currentPath = _pathHistoryEntries[_pathHistoryIdx];
		_currentPathTextField.value = _currentPath;
		ReloadCurrentPath();
	}

	void OnChoseJumpMenuEntry(IEnumerable<int> chosen)
	{
		int idx = _jumpMenuTreeView.selectedIndex;
		var jumpMenuEntry = _jumpMenuTreeView.GetItemDataForIndex<JumpMenuEntry>(idx);

		bool success;
		switch (jumpMenuEntry.SpecialFolderType)
		{
			case SpecialFolderType.Desktop:
				success = SetPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
				break;
			case SpecialFolderType.Personal:
				success = SetPath(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
				break;
			case SpecialFolderType.UnityProject:
				success = SetPath(FileUtil.ProjectPath);
				break;
			case SpecialFolderType.None:
				success = SetPath(jumpMenuEntry.Path);
				break;
			default:
				_jumpMenuTreeView.ClearSelection();
				return;
		}

		if (success)
		{
			_jumpButton.value = false;
		}
	}

	// -----------------------------------------

	void OnPathTextFieldKeyDown(KeyDownEvent e)
	{
		if (e.keyCode == KeyCode.Return ||
		    e.keyCode == KeyCode.KeypadEnter)
		{
			bool success = SetPath(_currentPathTextField.value);
			if (!success)
			{
				_currentPathTextField.SetValueWithoutNotify(_currentPath);
			}
		}
	}

	void OnPathTextFieldFocusOut(FocusOutEvent e)
	{
		bool success = SetPath(_currentPathTextField.value);
		if (!success)
		{
			_currentPathTextField.SetValueWithoutNotify(_currentPath);
		}
	}

	// -----------------------------------------

	void OnSortingChanged()
	{
		bool needsRebuild = RefreshSorting();
		if (needsRebuild)
		{
			_fileSystemEntriesView.Rebuild();
		}
	}

	void OnPressKey(KeyDownEvent e)
	{
		// Note: Pressing enter to "choose" the currently selected file/folder is already implemented.
		if (e.keyCode == KeyCode.Backspace)
		{
			MoveToParentFolder();
			e.StopPropagation();
		}
		else
		{
			char keyPressed = char.ToLowerInvariant(e.character);
			if (!keyPressed.IsInvalidFileNameChar())
			{
				// Go to first file/folder whose first letter matches the key that was pressed.
				// If the selection is already there, move to the next matching file/folder.

				int startIdx = _fileSystemEntriesView.selectedIndex + 1;
				bool foundMatch = DoSearch(startIdx);

				bool DoSearch(int idx)
				{
					for (int i = idx; i < _fileSystemEntries.Count; ++i)
					{
						var fileSysEntry = _fileSystemEntries[i];
						if (char.ToLowerInvariant(fileSysEntry.Name[0]) == e.character)
						{
							// found match
							_fileSystemEntriesView.SetSelection(i);
							_fileSystemEntriesView.ScrollToItem(i);
							return true;
						}
					}

					return false;
				}

				// Index 0 is the "To Parent Folder" entry, so the search really starts at index 1.
				// If the search didn't start there, then we may be able to find a match
				// if we wrap around and start again at the beginning.
				// todo: if _fileSystemEntriesView is displaying list of drives, search should start at index 0 instead
				if (!foundMatch && startIdx != 1)
				{
					foundMatch = DoSearch(1);
				}

				if (foundMatch)
				{
					e.StopPropagation();
				}
			}
		}
	}

	void OnContextClick(ContextClickEvent e)
	{
		var fileSystemEntriesContent = _fileSystemEntriesView.Q<VisualElement>("unity-content-container");

		var contentMousePos = ((VisualElement)e.target).ChangeCoordinatesTo(fileSystemEntriesContent, e.localMousePosition);
		var entryClicked = fileSystemEntriesContent.localBound.Contains(contentMousePos);

		if (!entryClicked)
		{
			_fileSystemEntriesView.ClearSelection();

			if (_currentOperationMode == OperationMode.Open)
			{
				_filenameTextField.SetFilename(null, null, null);
			}

			// Context Menu for the current path
			_contextMenu.DoAltBgStyling(false);
			_contextMenu.SetAlwaysLeaveSpaceForSelectedIndicator(false);
			_contextMenu.ClearMenu();

			if (IsPathInFavorites(_currentPath))
			{
				_contextMenu.AddMenu("Remove Current Path from Favorites", iconClassStyle: BaseIcons.RemoveFromFavorites,
					listener: this, userArg1: ContextMenuRemoveFromFavorites, userArg2: _currentPath);
			}
			else
			{
				_contextMenu.AddMenu("Add Current Path to Favorites", iconClassStyle: BaseIcons.AddToFavorites,
					listener: this, userArg1: ContextMenuAddFavorite, userArg2: _currentPath);
			}

			_contextMenu.AddMenu("Explore Here", iconClassStyle: BaseIcons.OpenFileExplorer,
				listener: this, userArg1: ContextMenuOpenFileExplorer, userArg2: _currentPath);

			_contextMenu.Show(e);
		}
	}

	void OnContextClickEntry(ContextClickEvent e, int idx)
	{
		_fileSystemEntriesView.SetSelection(idx);

		_contextMenu.DoAltBgStyling(false);
		_contextMenu.SetAlwaysLeaveSpaceForSelectedIndicator(false);
		_contextMenu.ClearMenu();

		if (idx != 0)
		{
			// todo: add menu entries for file operations (cut, copy, paste, delete, rename)
		}

		if (IsPathInFavorites(_currentPath))
		{
			_contextMenu.AddMenu("Remove Current Path from Favorites", iconClassStyle: BaseIcons.RemoveFromFavorites,
				listener: this, userArg1: ContextMenuRemoveFromFavorites, userArg2: _currentPath);
		}
		else
		{
			_contextMenu.AddMenu("Add Current Path to Favorites", iconClassStyle: BaseIcons.AddToFavorites,
				listener: this, userArg1: ContextMenuAddFavorite, userArg2: _currentPath);
		}

		string clickedFileFullPath = FileUtil.CombinePath(_currentPath, _fileSystemEntries[idx].Name);

		_contextMenu.AddMenu("Explore Here", iconClassStyle: BaseIcons.OpenFileExplorer,
			listener: this, userArg1: ContextMenuOpenFileExplorer, userArg2: clickedFileFullPath);

		if (_fileSystemEntries[idx].EntryType == FileSystemEntryType.File)
		{
			_contextMenu.AddMenu("Open with Associated Program", iconClassStyle: BaseIcons.OpenUsingAssociated,
				listener: this, userArg1: ContextMenuOpenUsingAssociated, userArg2: clickedFileFullPath);
		}

		_contextMenu.Show(e);
		e.StopPropagation();
	}

	public void OnContextMenuChosen(ContextMenuParams parameters)
	{
		switch (parameters.UserArg1 as string)
		{
			case ContextMenuRemoveFromFavorites:
				RemoveFromFavorites(parameters.UserArg2 as string);
				break;
			case ContextMenuAddFavorite:
				AddToFavorites(parameters.UserArg2 as string);
				break;
			case ContextMenuOpenFileExplorer:
				ExplorerUtil.OpenInFileBrowser(parameters.UserArg2 as string);
				break;
			case ContextMenuOpenUsingAssociated:
				ExplorerUtil.OpenWithDefaultProgram(parameters.UserArg2 as string);
				break;
		}
	}

	public void OnContextMenuClosed(bool userCancelled)
	{
	}

	/// <summary>
	///    Called when user clicks to select a file/folder in the List View.
	/// </summary>
	void OnSelectedFileSystemEntry(IEnumerable<int> selectedIndices)
	{
		int firstSelectedIdx = _fileSystemEntriesView.selectedIndex;
		if (firstSelectedIdx == -1)
		{
			// nothing selected
			return;
		}

		var selectedFileEntry = _fileSystemEntries[firstSelectedIdx];

		if (selectedFileEntry.EntryType == FileSystemEntryType.File)
		{
			// Since user selected a file, assign that to the File TextField.
			(string filenameNoExtension, string extension) = GetFilenameAndExtension(selectedFileEntry.Name);

			_filenameTextField.SetFilename(selectedFileEntry.Name, filenameNoExtension);
			if (_currentOperationMode == OperationMode.Open)
			{
				_filenameTextField.Extension = extension;
			}
		}
		else
		{
			// User selected a folder, remove the value in the File TextField then.
			if (_currentOperationMode == OperationMode.Open)
			{
				_filenameTextField.SetFilename(null, null, null);
			}
		}
	}

	(string, string) GetFilenameAndExtension(string filename)
	{
		string filenameNoExtension = null;
		string extension = null;

		if (_currentFilterType == FilterType.Custom)
		{
			if (_customFileTypeGetter != null)
			{
				(filenameNoExtension, extension) = _customFileTypeGetter(filename);
			}

			if (string.IsNullOrEmpty(filenameNoExtension) || string.IsNullOrEmpty(extension))
			{
				for (int i = 0; i < _customFileFilters.Count; i++)
				{
					if (filename.EndsWith(_customFileFilters[i], StringComparison.OrdinalIgnoreCase))
					{
						filenameNoExtension = filename[..^_customFileFilters[i].Length];
						extension = filename[^_customFileFilters[i].Length..];

						break;
					}
				}
			}
		}
		else
		{
			filenameNoExtension = Path.GetFileNameWithoutExtension(filename);
			extension = Path.GetExtension(filename);
		}

		return (filenameNoExtension, extension);
	}

	/// <summary>
	///    Called when user double-clicks on a file/folder in the List View.
	/// </summary>
	void OnChoseFileSystemEntry(IEnumerable<object> objects)
	{
		ProcessChosenFileEntry();
	}

	void OnEditFileTextField(string newFilename)
	{
		int firstSelectedIdx = _fileSystemEntriesView.selectedIndex;
		if (firstSelectedIdx == -1)
		{
			// nothing selected, no need to do anything
			return;
		}

		var selectedFileEntry = _fileSystemEntries[firstSelectedIdx];
		if (selectedFileEntry.EntryType == FileSystemEntryType.File && selectedFileEntry.Name != _filenameTextField.FullValue)
		{
			// the new typed filename doesn't match the file that's selected
			// so unselect it
			_fileSystemEntriesView.ClearSelection();
		}
	}

	void OnPressConfirm(ClickEvent e)
	{
		ProcessConfirmedFile();
	}

	void OnPressCancel(ClickEvent e)
	{
		_cancel?.Invoke();
	}

	// =====================================================================

	void ShowPathHistoryListPopUp(VisualElement button)
	{
		button.AddToClassList(ToolbarButtonPressedStyleClass);

		var buttonRect = button.ChangeCoordinatesTo(this, button.contentRect);
		_pathHistory.style.left = buttonRect.x;
		_pathHistory.style.top = buttonRect.yMax;

		_pathHistoryListShown = true;
		_pathHistoryListView.ClearSelection();
		_popUpMenuOutside.style.display = DisplayStyle.Flex;
		_pathHistory.style.display = DisplayStyle.Flex;
	}

	void ClosePopUpMenu()
	{
		_popUpMenuOutside.style.display = DisplayStyle.None;
		_pathHistory.style.display = DisplayStyle.None;
		_jumpMenu.style.display = DisplayStyle.None;
		_pathHistoryListShown = false;
		_backButtonHoldTimeStart = -1;
		_forwardButtonHoldTimeStart = -1;

		_jumpButton.value = false;
		_backButton.RemoveFromClassList(ToolbarButtonPressedStyleClass);
		_forwardButton.RemoveFromClassList(ToolbarButtonPressedStyleClass);
	}

	void ReloadCurrentPath()
	{
		_currentPathIsRoot = _currentPath.IsPathRoot();

		_fileSystemEntries.Clear();

		if (!_currentPathIsRoot)
		{
			_fileSystemEntries.Add(new FileSystemEntry()
			{
				Name = ".. Up to parent folder",
				EntryType = FileSystemEntryType.ToParentFolder,
				ReadableSize = string.Empty,
				SizeBytes = 0
			});
		}

		if (Directory.Exists(_currentPath))
		{
			foreach (string subFolder in Directory.EnumerateDirectories(_currentPath))
			{
				string subFolderName = Path.GetFileName(subFolder);

				// The "System Volume Information" is something we can't access anyway.
				if (_currentPathIsRoot && subFolderName == "System Volume Information")
				{
					continue;
				}

				try
				{
					Directory.EnumerateDirectories(subFolder);
				}
				catch (UnauthorizedAccessException)
				{
					// If we get an UnauthorizedAccessException exception, that means
					// we don't have permission to access the folder.
					// Just silently skip it.
					continue;
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					continue;
				}

				_fileSystemEntries.Add(new FileSystemEntry()
				{
					Name = subFolderName,
					EntryType = FileSystemEntryType.Folder,
					ReadableSize = string.Empty,
					SizeBytes = 0
				});
			}

			foreach (string file in Directory.EnumerateFiles(_currentPath))
			{
				bool matchedFileFilter;
				switch (_currentFilterType)
				{
					case FilterType.Custom:
					{
						matchedFileFilter = false;
						for (int i = 0; i < _customFileFilters.Count; i++)
						{
							if (file.EndsWith(_customFileFilters[i], StringComparison.OrdinalIgnoreCase))
							{
								matchedFileFilter = true;
								break;
							}
						}

						break;
					}
					case FilterType.Video:
						matchedFileFilter = file.IsVideoFile();
						break;
					case FilterType.Image:
						matchedFileFilter = file.IsImageFile();
						break;
					case FilterType.Sound:
						matchedFileFilter = file.IsSoundFile();
						break;
					default: // FilterType.None
						matchedFileFilter = true;
						break;
				}

				if (!matchedFileFilter)
				{
					continue;
				}

				long sizeBytes = FileUtil.GetFileSizeInBytes(file);
				_fileSystemEntries.Add(new FileSystemEntry()
				{
					Name = Path.GetFileName(file),
					EntryType = FileSystemEntryType.File,
					ReadableSize = FileUtil.GetBytesReadable(sizeBytes),
					SizeBytes = sizeBytes
				});
			}

			bool sorted = RefreshSorting();
			if (!sorted)
			{
				// there's no way to set the initial sorting so we'll just have to manually sort it
				_fileSystemEntries.Sort(SortByNameAsc);
			}

			_fileSystemEntriesView.horizontalScrollingEnabled = true;
			var listScrollView = _fileSystemEntriesView.Q<ScrollView>(className: BaseVerticalCollectionView.listScrollViewUssClassName);
			listScrollView.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
			listScrollView.horizontalScrollerVisibility = ScrollerVisibility.Auto;
		}
		else
		{
			_fileSystemEntriesView.horizontalScrollingEnabled = false;
			var listScrollView = _fileSystemEntriesView.Q<ScrollView>(className: BaseVerticalCollectionView.listScrollViewUssClassName);
			listScrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
			listScrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
		}

		_fileSystemEntriesView.Rebuild();
		_fileSystemEntriesView.SetSelection(0);
		_fileSystemEntriesView.ScrollToItem(0);
	}

	protected virtual void MoveToParentFolder()
	{
		string parentFolder = Path.GetDirectoryName(_currentPath).ConvertBackToForwardSlash();
		if (!string.IsNullOrEmpty(parentFolder))
		{
			SetPath(parentFolder);
		}
	}

	bool RefreshSorting()
	{
		foreach (var sortedColumn in _fileSystemEntriesView.sortedColumns)
		{
			switch (sortedColumn.columnName)
			{
				case FileSystemEntriesViewNameColumn:
					if (sortedColumn.direction == SortDirection.Ascending)
					{
						_fileSystemEntries.Sort(SortByNameAsc);
					}
					else
					{
						_fileSystemEntries.Sort(SortByNameDesc);
					}

					return true;
				case FileSystemEntriesViewSizeColumn:
					if (sortedColumn.direction == SortDirection.Ascending)
					{
						_fileSystemEntries.Sort(SortBySizeAsc);
					}
					else
					{
						_fileSystemEntries.Sort(SortBySizeDesc);
					}

					return true;
			}

			break;
		}

		return false;
	}

	protected void AddFileToPath(string file)
	{
		string newPath = FileUtil.CombinePath(_currentPath, file);

		_currentPath = newPath;
		_currentPathTextField.value = _currentPath;

		if (_pathHistoryIdx != -1 && _pathHistoryIdx != _pathHistoryEntries.Count - 1)
		{
			_pathHistoryEntries.RemoveRange(_pathHistoryIdx + 1, _pathHistoryEntries.Count - 1 - _pathHistoryIdx);
		}

		_pathHistoryEntries.Add(_currentPath);
		_pathHistoryIdx = _pathHistoryEntries.Count - 1;

		_backButton.SetEnabled(_pathHistoryIdx > 0);
		_forwardButton.SetEnabled(false);

		ReloadCurrentPath();
	}

	/// <summary>
	///    Called when:<br/>
	///    1. User presses enter while in the File TextField.<br/>
	///    2. User presses the Confirm Button.<br/>
	/// </summary>
	protected virtual void ProcessConfirmedFile()
	{
		if (string.IsNullOrWhiteSpace(_filenameTextField.FullValue))
		{
			// Nothing typed in the File TextField.
			return;
		}

		string fileFullPath = FileUtil.CombinePath(_currentPath, _filenameTextField.FullValue);

		// Save operation doesn't care if the filename specified doesn't exist (user likely wants to save to a new file).
		// Otherwise, we assume we are in Open operation. In that case, the file needs to exist.
		if (_currentOperationMode == OperationMode.Save || File.Exists(fileFullPath))
		{
			ProcessChosenFile(fileFullPath);
		}
	}

	/// <summary>
	///    Called when:<br/>
	///    1. User double-clicks on a file/folder in the List View.<br/>
	/// </summary>
	void ProcessChosenFileEntry()
	{
		int selectedIdx = _fileSystemEntriesView.selectedIndex;
		if (selectedIdx < 0 || selectedIdx >= _fileSystemEntries.Count)
		{
			// Nothing selected in the List View.
			return;
		}

		var chosenFileSysEntry = _fileSystemEntries[selectedIdx];

		switch (chosenFileSysEntry.EntryType)
		{
			case FileSystemEntryType.ToParentFolder:
			{
				MoveToParentFolder();
				break;
			}
			case FileSystemEntryType.Folder:
			{
				string newPath = FileUtil.CombinePath(_currentPath, chosenFileSysEntry.Name);
				SetPath(newPath);
				break;
			}
			case FileSystemEntryType.File:
			{
				string fileFullPath = FileUtil.CombinePath(_currentPath, chosenFileSysEntry.Name);
				ProcessChosenFile(fileFullPath);
				break;
			}
		}
	}

	protected virtual void ProcessChosenFile(string file)
	{
		if (_currentOperationMode == OperationMode.Save && !file.EndsWith(_saveFileExtension, StringComparison.OrdinalIgnoreCase))
		{
			// ensure the file has proper file type extension
			file = $"{file}{_saveFileExtension}";
		}

		var newRecentEntry = new TreeViewItemData<JumpMenuEntry>(RecentEntriesIDStart + 1 + _recentEntries.Count,
			new JumpMenuEntry()
			{
				Label = _currentPath.GetShortFolderName(),
				Path = _currentPath,
				SpecialFolderType = SpecialFolderType.None
			});

		// AddItem() causes _recentEntries to be modified
		_jumpMenuTreeView.AddItem(newRecentEntry, RecentEntriesIDStart);

		_fileChosen?.Invoke(file);
	}

	void RebuildJumpMenuEntries()
	{
		_jumpMenuEntries.Clear();
		var quickAccess = new JumpMenuEntry()
		{
			Label = "System",
			SpecialFolderType = SpecialFolderType.System
		};

		int id = 0;
		var quickAccessEntries = new List<TreeViewItemData<JumpMenuEntry>>();
		quickAccessEntries.Add(new TreeViewItemData<JumpMenuEntry>(++id, new JumpMenuEntry()
		{
			Label = "Desktop",
			SpecialFolderType = SpecialFolderType.Desktop
		}));
		quickAccessEntries.Add(new TreeViewItemData<JumpMenuEntry>(++id, new JumpMenuEntry()
		{
			Label = "User",
			SpecialFolderType = SpecialFolderType.Personal
		}));
#if UNITY_EDITOR
		quickAccessEntries.Add(new TreeViewItemData<JumpMenuEntry>(++id, new JumpMenuEntry()
		{
			Label = FileUtil.ProjectFolderName,
			SpecialFolderType = SpecialFolderType.UnityProject
		}));
#endif
		_jumpMenuEntries.Add(new TreeViewItemData<JumpMenuEntry>(0, quickAccess, quickAccessEntries));

		// ---------------------------------------------------

		var favorites = new JumpMenuEntry()
		{
			Label = "Favorites",
			SpecialFolderType = SpecialFolderType.Favorites
		};

		// load user favorite paths
		{
			_userFavoritesEntries.Clear();
			string userFavoritesPath = UserFavoritesSavePath;
			if (File.Exists(userFavoritesPath))
			{
				foreach (string line in File.ReadLines(userFavoritesPath, Encoding.UTF8))
				{
					if (string.IsNullOrEmpty(line))
					{
						continue;
					}

					if (!line.IsPathValid())
					{
						continue;
					}

					(bool success, string normalizedPath) = line.FixPath();
					if (!success)
					{
						continue;
					}

					_userFavoritesEntries.Add(new TreeViewItemData<JumpMenuEntry>(_nextFreeFavoriteEntryId++, new JumpMenuEntry()
					{
						Label = normalizedPath.GetShortFolderName(),
						Path = normalizedPath,
						SpecialFolderType = SpecialFolderType.None
					}));
				}
			}
		}
		_jumpMenuEntries.Add(new TreeViewItemData<JumpMenuEntry>(UserFavoritesIDStart, favorites, _userFavoritesEntries));

		// ---------------------------------------------------

		var recent = new JumpMenuEntry()
		{
			Label = "Recent",
			SpecialFolderType = SpecialFolderType.Recent
		};
		_jumpMenuEntries.Add(new TreeViewItemData<JumpMenuEntry>(RecentEntriesIDStart, recent, _recentEntries));
	}

	bool IsPathInFavorites(string pathToCheck)
	{
		foreach (var userFavorite in _userFavoritesEntries)
		{
			if (pathToCheck == userFavorite.data.Path)
			{
				return true;
			}
		}

		return false;
	}

	void RemoveFromFavorites(string pathToRemove)
	{
		int idOfExistingFavoriteEntry = -1;
		foreach (var userFavorite in _userFavoritesEntries)
		{
			if (pathToRemove == userFavorite.data.Path)
			{
				idOfExistingFavoriteEntry = userFavorite.id;
				break;
			}
		}

		if (_jumpMenuTreeView.TryRemoveItem(idOfExistingFavoriteEntry))
		{
			string userFavoritesPath = UserFavoritesSavePath;
			if (!File.Exists(userFavoritesPath))
			{
				return;
			}

			// recreate the contents but without the entry that has been removed
			var newContents = new List<string>();
			foreach (string line in File.ReadLines(userFavoritesPath, Encoding.UTF8))
			{
				if (string.IsNullOrEmpty(line))
				{
					continue;
				}

				if (line == pathToRemove)
				{
					continue;
				}

				newContents.Add(line);
			}

			File.WriteAllLines(userFavoritesPath, newContents, Encoding.UTF8);
		}
	}

	void AddToFavorites(string pathToAdd)
	{
		var newFavorite = new TreeViewItemData<JumpMenuEntry>(_nextFreeFavoriteEntryId++, new JumpMenuEntry()
		{
			Label = pathToAdd.GetShortFolderName(),
			Path = pathToAdd,
			SpecialFolderType = SpecialFolderType.None
		});

		// AddItem() causes _userFavoritesEntries to be modified
		_jumpMenuTreeView.AddItem(newFavorite, UserFavoritesIDStart);

		// Save the new favorite entry
		string userFavoritesPath = UserFavoritesSavePath;
		string userFavoritesFolder = Path.GetDirectoryName(userFavoritesPath);
		if (!string.IsNullOrEmpty(userFavoritesFolder))
		{
			Directory.CreateDirectory(userFavoritesFolder);
		}

		File.AppendAllText(userFavoritesPath, pathToAdd + Environment.NewLine, Encoding.UTF8);
	}

	// =====================================================================

	Func<string, bool, string> _customFileIconRule;

	public void SetCustomFileIconRule(Func<string, bool, string> newCallback)
	{
		_customFileIconRule = newCallback;
	}

	string GetIconStyleClass(string filename, FileSystemEntryType type)
	{
		string customIconStyleClass = _customFileIconRule?.Invoke(filename, type == FileSystemEntryType.File);
		if (!string.IsNullOrEmpty(customIconStyleClass))
		{
			return customIconStyleClass;
		}

		switch (type)
		{
			case FileSystemEntryType.ToParentFolder:
				return "dld-icon--file-entry--folder-parent";
			case FileSystemEntryType.Folder:
				return "dld-icon--file-entry--folder-generic";
			case FileSystemEntryType.File:
				// todo: change icon based on mime type?
				if (filename.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
				{
					return "dld-icon--file-entry--file-unity";
				}
				else if (filename.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
				{
					return "dld-icon--file-entry--file-prefab";
				}
				else
				{
					return "dld-icon--file-entry--file-generic";
				}
			default:
				return "dld-icon--file-entry--file-generic";
		}
	}

	// =====================================================================

	const int AFirstThenB = -1;
	const int BFirstThenA = +1;

	static readonly Comparison<FileSystemEntry> SortByNameAsc = _SortByNameAsc;

	static int _SortByNameAsc(FileSystemEntry a, FileSystemEntry b)
	{
		// the special "To Parent Folder" always comes first
		if (a.EntryType == FileSystemEntryType.ToParentFolder && b.EntryType != FileSystemEntryType.ToParentFolder)
		{
			return AFirstThenB;
		}

		if (b.EntryType == FileSystemEntryType.ToParentFolder && a.EntryType != FileSystemEntryType.ToParentFolder)
		{
			return BFirstThenA;
		}

		// folders come first before files
		if (a.EntryType == FileSystemEntryType.Folder && b.EntryType == FileSystemEntryType.File)
		{
			return AFirstThenB;
		}

		if (b.EntryType == FileSystemEntryType.Folder && a.EntryType == FileSystemEntryType.File)
		{
			return BFirstThenA;
		}

		return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
	}

	static readonly Comparison<FileSystemEntry> SortByNameDesc = _SortByNameDesc;

	static int _SortByNameDesc(FileSystemEntry a, FileSystemEntry b)
	{
		// the special "To Parent Folder" always comes first
		if (a.EntryType == FileSystemEntryType.ToParentFolder && b.EntryType != FileSystemEntryType.ToParentFolder)
		{
			return AFirstThenB;
		}

		if (b.EntryType == FileSystemEntryType.ToParentFolder && a.EntryType != FileSystemEntryType.ToParentFolder)
		{
			return BFirstThenA;
		}

		// folders come first before files
		if (a.EntryType == FileSystemEntryType.Folder && b.EntryType == FileSystemEntryType.File)
		{
			return AFirstThenB;
		}

		if (b.EntryType == FileSystemEntryType.Folder && a.EntryType == FileSystemEntryType.File)
		{
			return BFirstThenA;
		}

		return string.Compare(b.Name, a.Name, StringComparison.Ordinal);
	}

	static readonly Comparison<FileSystemEntry> SortBySizeAsc = _SortBySizeAsc;

	static int _SortBySizeAsc(FileSystemEntry a, FileSystemEntry b)
	{
		// the special "To Parent Folder" always comes first
		if (a.EntryType == FileSystemEntryType.ToParentFolder && b.EntryType != FileSystemEntryType.ToParentFolder)
		{
			return AFirstThenB;
		}

		if (b.EntryType == FileSystemEntryType.ToParentFolder && a.EntryType != FileSystemEntryType.ToParentFolder)
		{
			return BFirstThenA;
		}

		// folders come first before files
		if (a.EntryType == FileSystemEntryType.Folder && b.EntryType == FileSystemEntryType.File)
		{
			return AFirstThenB;
		}

		if (b.EntryType == FileSystemEntryType.Folder && a.EntryType == FileSystemEntryType.File)
		{
			return BFirstThenA;
		}

		if (a.EntryType == FileSystemEntryType.Folder && b.EntryType == FileSystemEntryType.Folder)
		{
			// both folders
			// since folders don't have a size, sort these by name instead
			return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
		}

		return a.SizeBytes.CompareTo(b.SizeBytes);
	}

	static readonly Comparison<FileSystemEntry> SortBySizeDesc = _SortBySizeDesc;

	static int _SortBySizeDesc(FileSystemEntry a, FileSystemEntry b)
	{
		// the special "To Parent Folder" always comes first
		if (a.EntryType == FileSystemEntryType.ToParentFolder && b.EntryType != FileSystemEntryType.ToParentFolder)
		{
			return AFirstThenB;
		}

		if (b.EntryType == FileSystemEntryType.ToParentFolder && a.EntryType != FileSystemEntryType.ToParentFolder)
		{
			return BFirstThenA;
		}

		// folders come first before files
		if (a.EntryType == FileSystemEntryType.Folder && b.EntryType == FileSystemEntryType.File)
		{
			return AFirstThenB;
		}

		if (b.EntryType == FileSystemEntryType.Folder && a.EntryType == FileSystemEntryType.File)
		{
			return BFirstThenA;
		}

		if (a.EntryType == FileSystemEntryType.Folder && b.EntryType == FileSystemEntryType.Folder)
		{
			// both folders
			// since folders don't have a size, sort these by name instead
			return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
		}

		return b.SizeBytes.CompareTo(a.SizeBytes);
	}
}

}
