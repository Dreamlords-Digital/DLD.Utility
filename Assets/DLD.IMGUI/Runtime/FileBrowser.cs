// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DLD.Serializer;
using DLD.Utility;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using FileUtil = DLD.Utility.FileUtil;
using Object = UnityEngine.Object;

namespace DLD.IMGUI
{
	// ===================================================================================

	public partial class FileBrowser
	{
		static bool ShowDebugInfo;

		public enum BrowserType
		{
			/// <summary>
			/// Standard file browser.
			/// </summary>
			File,

			/// <summary>
			/// Restrict the browser to viewing and selecting only folders.
			/// </summary>
			Folder,

			/// <summary>
			/// Restrict the browser to viewing images.
			/// Will show image thumbnails.
			/// </summary>
			Image,

			/// <summary>
			/// Restrict the browser to viewing sound files.
			/// Will show play buttons beside the sounds so the user can preview them.
			/// </summary>
			Sound,

			/// <summary>
			/// Restrict the browser to viewing animation files.
			/// This can be Unity .anim files, or fbx files.
			/// Fbx files can be browsed as if they were folders,
			/// to let the user choose the specific animation clip inside it.
			/// </summary>
			Animation,

			/// <summary>
			/// Restrict the browser to viewing prefabs.
			/// This can be the usual .prefab, fbx files,
			/// or prefab/fbx files inside AssetBundles.
			/// </summary>
			Prefab,
		}

		public enum OperationType
		{
			Open,
			Save
		}

		static readonly char[] FolderDelimiter = { '\\', '/' };

		// -----------------------

		// these are the only file types Unity can open in runtime
		const string SELECTION_PATTERN_IMAGE = "*.png;*.jpg;*.jpeg";

		// these are the only sound types Unity can open (for standalone)
		const string SELECTION_PATTERN_SOUND = "*.ogg;*.wav;*.xm;*.it;*.mod;*.s3m";

		const string SELECTION_PATTERN_ANIMATION = "*.anim;*.fbx;*.asset";

		const string SELECTION_PATTERN_PREFAB = "*.prefab;*.fbx;*.asset";

		const int DEFAULT_NUMBER_OF_IMAGES_TO_AUTO_LOAD = 20;
		const float DEFAULT_IMAGE_LOAD_DELAY_SECONDS = 0.25f;

#if UNITY_STANDALONE_OSX
		const string OPEN_IN_FILE_BROWSER_LABEL = "Finder";
#elif UNITY_STANDALONE_WIN
		const string OPEN_IN_FILE_BROWSER_LABEL = "Explorer";
#else
		const string OPEN_IN_FILE_BROWSER_LABEL = "File Browser";
#endif

		// -------------------------------------------

		readonly GUIContent _backButtonLabel = new GUIContent("Back");
		readonly GUIContent _forwardButtonLabel = new GUIContent("Forward");
		readonly GUIContent _jumpButtonLabel = new GUIContent("Jump...");
		readonly GUIContent _editPathButtonLabel = new GUIContent("Edit Path");
		readonly GUIContent _openInExplorerButtonLabel = new GUIContent(OPEN_IN_FILE_BROWSER_LABEL);

		Rect _jumpToButtonRect;
		Rect _jumpToListRect;
		bool _showJumpToList;

		readonly List<GUIContent> _recentlyUsed = new List<GUIContent>();

		RecentlyUsedFolders _recentlyUsedFoldersData;

		string RecentlyUsedFoldersFile
		{
			get
			{
				var result = $"{Path.GetTempPath()}{FileUtil.ProjectFolderName}.DLD.FileBrowser.Recent.txt";
				result = result.Replace("\\", "/");
				return result;
			}
		}

		void SaveRecentlyUsedFoldersData()
		{
			if (_recentlyUsedFoldersData == null)
			{
				_recentlyUsedFoldersData = new RecentlyUsedFolders();
				_recentlyUsedFoldersData.SetValuesFrom(_recentlyUsed);
			}

			var saveFilePath = RecentlyUsedFoldersFile;
			DefaultSerializer.Instance.SaveToLocal(saveFilePath, _recentlyUsedFoldersData);
			Debug.LogFormat("Saved recent folders to {0}", saveFilePath);
		}

		void LoadRecentlyUsedFoldersData()
		{
			var savedFilePath = RecentlyUsedFoldersFile;

			if (File.Exists(savedFilePath))
			{
				var loadResult = DefaultSerializer.Instance.TryLoadFromLocal(savedFilePath, ref _recentlyUsedFoldersData);
				if (loadResult.result == LoadResult.Success)
				{
					_recentlyUsedFoldersData.AssignValuesTo(_recentlyUsed);
				}
			}
		}

		/// <summary>
		/// Folder that we are currently displaying
		/// </summary>
		string _currentPath;

		/// <summary>
		/// File in the current path that should be highlighted
		/// after calling SwitchCurrentPath. This gets set to null
		/// after the path has been opened and displayed to the user.
		/// </summary>
		string _fileToSelect;

		/// <summary>
		/// The name of the file in the current path that we have "entered" into.
		/// A virtual folder is a file that contains files/assets inside, like archives,
		/// Asset Bundles, or fbx files that contain animations inside.
		/// </summary>
		string _currentVirtualFolder;

		/// <summary>
		/// The path inside the virtual folder that we are displaying.
		/// </summary>
		string _currentVirtualPath;

		struct PathHistoryEntry
		{
			/// <summary>
			/// The path to go to whenever we go back to this history entry.
			/// </summary>
			public string Path;

			/// <summary>
			/// When viewing the inside of a file (like an archive), this is the filename of that file.
			/// </summary>
			public string VirtualFolder;

			/// <summary>
			/// When viewing the inside of a file (like an archive), this is the path inside that file that we're in.
			/// </summary>
			public string VirtualPath;

			/// <summary>
			/// Position of the scrollbar when we left this path.
			/// </summary>
			public float ScrollbarPosition;

			/// <summary>
			/// Any selected file within that path. -1 if no file selected.
			/// </summary>
			public int SelectedFileIdx;

			/// <summary>
			/// Any selected folder within that path. -1 if no folder selected.
			/// </summary>
			public int SelectedFolderIdx;
		}

		readonly List<PathHistoryEntry> _pathHistory = new List<PathHistoryEntry>();
		int _pathHistoryIdx;

		PathHistoryEntry _pathUsed;


		/// <summary>Optional pattern for filtering selectable files/folders.</summary>
		///
		/// <para>example: "*.png;*.jpg;*.jpeg"</para>
		///
		/// <para>Split using ; or , or |</para>
		///
		/// <para>See references: http://msdn.microsoft.com/en-us/library/wz42302f(v=VS.90).aspx and
		/// http://msdn.microsoft.com/en-us/library/6ff71z1w(v=VS.90).aspx</para>
		string _selectionPattern;

		string[] _selectionPatterns;

		/// <summary>Optional pattern for filtering selectable files/folders.</summary>
		///
		/// <para>example: "*.png;*.jpg;*.jpeg"</para>
		///
		/// <para>Split using ; or , or |</para>
		///
		/// <para>See references: http://msdn.microsoft.com/en-us/library/wz42302f(v=VS.90).aspx and
		/// http://msdn.microsoft.com/en-us/library/6ff71z1w(v=VS.90).aspx</para>
		public void SetSelectionPattern(string newSelectionPattern)
		{
			_selectionPattern = newSelectionPattern;
			_selectionPatterns = _selectionPattern.Split(DefaultSeparators);
		}

		static readonly char[] DefaultSeparators = { ',', ';', '|' };

		public void SetSelectionPattern(BrowserType browserType)
		{
			switch (browserType)
			{
				case BrowserType.Image:
					_selectionPattern = SELECTION_PATTERN_IMAGE;
					_selectionPatterns = _selectionPattern.Split(DefaultSeparators);
					break;
				case BrowserType.Sound:
					_selectionPattern = SELECTION_PATTERN_SOUND;
					_selectionPatterns = _selectionPattern.Split(DefaultSeparators);
					break;
				case BrowserType.Animation:
					_selectionPattern = SELECTION_PATTERN_ANIMATION;
					_selectionPatterns = _selectionPattern.Split(DefaultSeparators);
					break;
				case BrowserType.Prefab:
					_selectionPattern = SELECTION_PATTERN_PREFAB;
					_selectionPatterns = _selectionPattern.Split(DefaultSeparators);
					break;
			}
		}

		// Optional image for folder entries
		Texture2D _unityProjectFolderImage;
		Texture2D _folderImage;
		Texture2D _specialFolderImage;
		Texture2D _toParentFolderImage;
		Texture2D _driveImage;
		Texture2D _fileImage;
		Texture2D _matchedFileImage;

		Texture2D _animationClipImage;
		Texture2D _prefabImage;


		BrowserType _browserBrowserType;

		/// <summary>
		/// Whether we are saving to a file or opening a file.
		/// </summary>
		OperationType _operationType;

		/// <summary>
		/// Set whether we are saving to a file or opening a file.
		/// </summary>
		public void SetOperationType(OperationType newOpType)
		{
			_operationType = newOpType;
		}

		public bool IsCurrentPathSet => !string.IsNullOrWhiteSpace(_currentPath);

		public string CurrentPath => _currentPath;

		bool IsBrowserTypeForFiles
		{
			get
			{
				return _browserBrowserType != BrowserType.Folder;
			}
		}

		bool IsBrowserTypeForImages
		{
			get
			{
				return _browserBrowserType == BrowserType.Image;
			}
		}

		bool IsBrowserTypeForSounds
		{
			get
			{
				return _browserBrowserType == BrowserType.Sound;
			}
		}

		bool IsBrowserTypeForAnimations
		{
			get
			{
				return _browserBrowserType == BrowserType.Animation;
			}
		}

		bool IsBrowserTypeForPrefabs
		{
			get
			{
				return _browserBrowserType == BrowserType.Prefab;
			}
		}

		bool IsBrowserTypeForFolders
		{
			get
			{
				return _browserBrowserType == BrowserType.Folder;
			}
		}


		/// <summary>
		/// Folders in the path
		/// </summary>
		string[] _currentPathParts;

		/// <summary>
		/// Folders in the path as GUIContent, ready for display to the GUI.
		/// </summary>
		GUIContent[] _currentPathPartsDisplayed;

		readonly GUIContent _rootLocationLabel = new GUIContent("This PC");
		readonly GUIContent _desktopLocationLabel = new GUIContent("Desktop");
		readonly GUIContent _userHomeLocationLabel = new GUIContent("Home");
		readonly GUIContent _myDocsLocationLabel = new GUIContent("My Documents");
		readonly GUIContent _gameCommonDataLabel = new GUIContent("Game Common Data");
		readonly GUIContent _thisProjectAssetsLocationLabel = new GUIContent("Project Assets");
		readonly GUIContent _thisProjectStreamingAssetsLabel = new GUIContent("Streaming Assets");

		/// <summary>
		/// Which element in <see cref="_files"/> is selected.
		/// When this is given a value, _selectedFolderIdx will be set to -1.
		/// Only one of them should have a value at any given time.
		/// </summary>
		int _selectedFileIdx = -1;

		/// <summary>
		/// Which element in <see cref="_folders"/> is selected.
		/// When this is given a value, _selectedFileIdx will be set to -1.
		/// Only one of them should have a value at any given time.
		/// </summary>
		int _selectedFolderIdx = -1;

		int _hoveredFileIdx = -1;
		int _hoveredFolderIdx = -1;

		int _topVisibleFolderIdx = -1;
		int _topVisibleFileIdx = -1;

		int _bottomVisibleFileIdx = -1;

		int _lastLoadedImageIdx = -1;
		float _checkLoadImagesFrameCountdown;

		Rect _hoveredFileRect;

		/// <summary>
		/// Filenames on the current path
		/// </summary>
		string[] _files;

		/// <summary>
		/// Same as <see cref="_files"/> but as GUIContent with accompanying file icons.
		/// </summary>
		GUIContent[] _filesWithImages;

		/// <summary>
		/// Used when browsing images, holds the Textures per given filename.
		/// Key is file absolute path, value is the texture.
		/// </summary>
		readonly Dictionary<string, Texture2D> _fileThumbnails = new Dictionary<string, Texture2D>();

		/// <summary>
		/// Filenames on the current path that can't be selected (non-matching files).
		/// </summary>
		string[] _nonMatchingFiles;

		/// <summary>
		/// Same as <see cref="_nonMatchingFiles"/> but as GUIContent with accompanying file icons.
		/// </summary>
		GUIContent[] _nonMatchingFilesWithImages;

		/// <summary>
		/// Subfolder names on the current path.
		/// </summary>
		string[] _folders;

		/// <summary>
		/// Same as <see cref="_folders"/> but as GUIContent with accompanying folder icons.
		/// </summary>
		GUIContent[] _foldersWithImages;


		Vector2 _scrollPosition;

		Action<string> _confirmCallback;
		Action<string> _cancelCallback;

		public void SetConfirmCallback(Action<string> newCallback)
		{
			_confirmCallback = newCallback;
		}

		public void SetCancelCallback(Action<string> newCallback)
		{
			_cancelCallback = newCallback;
		}

		void CallConfirmCallback(string path)
		{
			AddToRecentlyUsedFolders(_currentPath);

			if (_confirmCallback != null)
			{
				_confirmCallback(path);
			}
		}

		void CallCancelCallback(string path)
		{
			if (_cancelCallback != null)
			{
				_cancelCallback(path);
			}
		}

		void AddToRecentlyUsedFolders(string path)
		{
			if (!string.IsNullOrEmpty(path) && !_recentlyUsed.Exists(content => content.tooltip == path))
			{
				var folderName = FileUtil.GetLastFolder(path);
				_recentlyUsed.Add(new GUIContent(folderName, _specialFolderImage, path));
				Debug.LogFormat("Added {0} to recently used path. Folder: {1}", path, folderName);

				SaveRecentlyUsedFoldersData();
			}
		}

		// -------------------------------------------------------------------------

		public FileBrowser(Action<string> confirmCallback = null)
		{
			_browserBrowserType = BrowserType.File;
			_operationType = OperationType.Open;
			_confirmCallback = confirmCallback;
			SwitchCurrentPath(Directory.GetCurrentDirectory());

			LoadRecentlyUsedFoldersData();
		}

		public FileBrowser(string initialPath,
			Action<string> confirmCallback = null)
		{
			_browserBrowserType = BrowserType.File;
			_operationType = OperationType.Open;
			_confirmCallback = confirmCallback;
			SwitchCurrentPath(initialPath);

			LoadRecentlyUsedFoldersData();
		}

		public FileBrowser(string initialPath, string filePattern,
			OperationType opType,
			Action<string> confirmCallback = null)
		{
			_browserBrowserType = BrowserType.File;
			_operationType = opType;
			_confirmCallback = confirmCallback;
			_selectionPattern = filePattern;
			_selectionPatterns = _selectionPattern.Split(',', ';', '|');
			SwitchCurrentPath(initialPath);

			LoadRecentlyUsedFoldersData();
		}

		public FileBrowser(string initialPath, string filePattern,
			OperationType opType, BrowserType browserBrowserType,
			Action<string> confirmCallback = null)
		{
			_browserBrowserType = browserBrowserType;
			_operationType = opType;
			_confirmCallback = confirmCallback;
			_selectionPattern = filePattern;
			_selectionPatterns = _selectionPattern.Split(',', ';', '|');
			SwitchCurrentPath(initialPath);

			LoadRecentlyUsedFoldersData();
		}

		public FileBrowser(OperationType opType, BrowserType browserBrowserType,
			Action<string> confirmCallback = null)
		{
			_browserBrowserType = browserBrowserType;
			_operationType = opType;
			_confirmCallback = confirmCallback;

			SetSelectionPattern(browserBrowserType);

			MoveToRootPath();

			LoadRecentlyUsedFoldersData();
		}

		// -------------------------------------------------------------------------

		public void SetBrowserType(BrowserType newBrowserType)
		{
			_browserBrowserType = newBrowserType;
			SetSelectionPattern(newBrowserType);
		}

		// -------------------------------------------------------------------------

		bool _showPathAsButtons = true;

		void ShowPathAsTextField()
		{
			_showPathAsButtons = false;
			_editingCurrentPath = _currentPath;
		}

		void ShowPathAsButtons()
		{
			_showPathAsButtons = true;
		}

		bool IsPathShownAsTextField
		{
			get
			{
				return !_showPathAsButtons;
			}
		}

		// -------------------------------------------------------------------------

		bool _SwitchCurrentPath(string pathToSwitchTo, string virtualFolderSpecified, string virtualPathSpecified)
		{
			if (string.IsNullOrEmpty(pathToSwitchTo) ||
			    (_currentPath == pathToSwitchTo &&
			     _currentVirtualFolder == virtualFolderSpecified &&
			     _currentVirtualPath == virtualPathSpecified &&
			     ((_fileToSelect == null && _selectedFileIdx == -1) ||
			      (_selectedFileIdx > -1 && _fileToSelect == _files[_selectedFileIdx]))))
			{
				// no path specified, or already at the path specified
				// no file should be selected and no file is already selected, or
				// a file that should be selected is already selected
				return false;
			}

			// ---------------------
			// check first if folders in path exist

			bool accessible;

			if (IsPathRoot(pathToSwitchTo))
			{
				// root is always accessible
				accessible = true;
			}
			else
			{
				accessible = FileUtil.FixPath(pathToSwitchTo, out pathToSwitchTo);
			}

			if (!accessible)
			{
				Debug.LogWarningFormat("Can't access path: {0}", pathToSwitchTo);
				return false;
			}

			// ---------------------

			//Debug.LogFormat("going to {0}", pathToSwitchTo);

			// note: old path is only recorded for debugging purposes
			string oldPath = _currentPath;

			_currentPath = pathToSwitchTo;
			_currentVirtualFolder = virtualFolderSpecified;
			_currentVirtualPath = virtualPathSpecified;

			// reset scrollbar
			_scrollPosition.x = 0;
			_scrollPosition.y = 0;

			_soundPlayer.StopAllSounds();
			ReadDirectoryContents(_currentPath, virtualFolderSpecified, virtualPathSpecified, oldPath);

			return true;
		}

		void _SwitchCurrentPath(PathHistoryEntry pathToSwitchTo)
		{
			var success =
				_SwitchCurrentPath(pathToSwitchTo.Path, pathToSwitchTo.VirtualFolder, pathToSwitchTo.VirtualPath);

			if (success)
			{
				_pathUsed = pathToSwitchTo;
			}
		}

		void SwitchToParentFolderOfCurrentPath(int numberOfTimesToGoUp = 1)
		{
			Assert.IsTrue(numberOfTimesToGoUp > 0);

			if (AlreadyAtLockPathIfApplicable)
			{
				Debug.LogFormat("already at lock path: {0}", _lockPath);
				return;
			}

			string parentFolder = _currentPath;

			if (!string.IsNullOrEmpty(_currentVirtualFolder))
			{
				--numberOfTimesToGoUp;

				// going up from the virtual folder
				// will get us to the _currentPath
				parentFolder = _currentPath;
			}

			for (int n = 0; n < numberOfTimesToGoUp; ++n)
			{
				parentFolder = Path.GetDirectoryName(parentFolder);
			}

			for (int n = 0; n < _pathHistory.Count; ++n)
			{
				if (_pathHistory[n].Path == parentFolder)
				{
					_pathUsed = _pathHistory[n];
					break;
				}
			}

			SwitchCurrentPath(parentFolder);
		}

		bool IsFilenameAnAssetBundle(string filename)
		{
			return filename != null &&
			       filename.EndsWith(FileUtil.ASSET_BUNDLE_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase);
		}

		int GetAnimationClipCount(string animationFullPath)
		{
			if (animationFullPath.Contains(FileUtil.RESOURCES_FOLDER, StringComparison.OrdinalIgnoreCase))
			{
				// fbx to "open" is in a Resources folder

				// if the fbx only has 1 animation inside, then don't bother going inside it,
				// open the folder containing the fbx file instead

				var resourcesPathOfFbxFile = FileUtil.GetResourcesPathOfFile(animationFullPath);

				var anims = Resources.LoadAll<AnimationClip>(resourcesPathOfFbxFile);

				return anims != null ? anims.Length : -1;
			}
			else
			{
				// fbx to "open" is not in a Resources folder

				// if the fbx only has 1 animation inside, then don't bother going inside it,
				// open the folder containing the fbx file instead

#if UNITY_EDITOR
				Object[] subAssets =
					AssetDatabase.LoadAllAssetRepresentationsAtPath(
						FileUtil.GetPathRelativeToAssets(animationFullPath));
#else
				UnityEngine.Object[] subAssets = null;
#endif

				if (subAssets == null || subAssets.Length == 0)
				{
					return 0;
				}

				var animCount = 0;
				for (int subIdx = 0; subIdx < subAssets.Length; ++subIdx)
				{
					if (subAssets[subIdx] is AnimationClip)
					{
						++animCount;
					}
				}

				return animCount;
			}
		}

		/// <summary>
		/// Change which folder the browser is viewing. Needs to be given a full path.
		/// If the path doesn't exist, it will try to move to the specified parent folder and try that one.
		/// If the parent path doesn't exist too, it will keep trying by moving to the next parent until the resulting path exists.
		/// </summary>
		/// <param name="fullPath"></param>
		public void SwitchCurrentPath(string fullPath)
		{
			if (string.IsNullOrEmpty(fullPath))
			{
				return;
			}

			// -------------------

			//Debug.LogFormat("initial path to go to: {0}", pathToSwitchTo);

			// filename (without the path) in the specified pathToSwitchTo, if any.
			// this indicates the filename to select inside the path
			string fileSpecified = null;

			// filename (without the path) in the specified fullPath of the virtual folder to go to, if any.
			// virtual folder is a file that contains files/assets inside,
			// like archives, Asset Bundles, or fbx files containing animations inside
			string virtualFolderSpecified = null;

			// filename (with complete absolute path) in the specified pathToSwitchTo of the virtual folder to go to, if any.
			string virtualFolderSpecifiedAbsPath = null;

			// filename (without the path) of what to select inside the virtual folder
			string virtualFileSpecified = null;

			// path where specified virtual file resides, if any. Relative to the root of that virtual folder.
			// only applies to virtual folders that allow "subfolders", like archives or Asset Bundles
			//todo implement this
			string virtualPathSpecified = null;

			// -------------------

			fullPath = fullPath.Replace('\\', '/');


			// check for indications that the path specifies a virtual folder
			var enteredFbxIdx = fullPath.IndexOf(FileUtil.ENTERED_FBX_FILE, StringComparison.OrdinalIgnoreCase);
			var enteredAssetBundleIdx =
				fullPath.IndexOf(FileUtil.ENTERED_ASSET_BUNDLE_FILE, StringComparison.OrdinalIgnoreCase);
			if (enteredFbxIdx > 0 || enteredAssetBundleIdx > 0)
			{
				if (enteredAssetBundleIdx > 0)
				{
					// specified a folder/file inside the asset bundle
					Debug.LogError("Going into an Asset Bundle, and selecting a file inside it. Not yet implemented");

					if (enteredFbxIdx > 0)
					{
						if (IsBrowserTypeForAnimations)
						{
							// specified an animation inside an fbx file, that's inside an asset bundle
						}
						else
						{
							Debug.LogError(
								$"Shouldn't be entering inside fbx files if not browsing animations. Path: \"{fullPath}\"");
						}
					}
					else
					{
						// specified regular asset inside an asset bundle
					}
				}
				else if (enteredFbxIdx > 0)
				{
					// fullPath specifies to "enter" the fbx file and select an animation clip inside it

					if (IsBrowserTypeForAnimations)
					{
						// there aren't any "subfolders" inside fbx files, so whatever is after the fbx file
						// is automatically assumed to be the selected animation

						var lastFolderIdx = fullPath.LastIndexOf('/');

						// the fbx filename (without any path)
						virtualFolderSpecified = fullPath.Substring(lastFolderIdx + 1,
							(enteredFbxIdx + FileUtil.FBX_FILE_EXTENSION_LEN) - lastFolderIdx - 1);

						// anything after the colon. this is the animation clip name
						virtualFileSpecified = fullPath.Substring(enteredFbxIdx + FileUtil.ENTERED_FBX_FILE_LEN);

						// from the real (absolute) path up to the fbx file, without the colon
						virtualFolderSpecifiedAbsPath =
							fullPath.Substring(0, enteredFbxIdx + FileUtil.FBX_FILE_EXTENSION_LEN);

						// the real (absolute) path but only folders, not including the fbx filename or anything after that
						fullPath = fullPath.Substring(0, lastFolderIdx);

						Debug.LogFormat("FileBrowser.SwitchCurrentPath(): going inside fbx file.\n" +
						                "pathToSwitchTo: {0}\n" +
						                "virtualFolderSpecified: {1}\n" +
						                "virtualFileSpecified: {2}\n" +
						                "virtualFolderSpecifiedAbsPath: {3}",
							fullPath, virtualFolderSpecified, virtualFileSpecified, virtualFolderSpecifiedAbsPath);

						if (IsAnimationVirtualFolderAnFbxFile(fullPath, virtualFolderSpecified))
						{
							if (!Application.isEditor)
							{
								Debug.LogError(
									$"FileBrowser.SwitchCurrentPath(): Can't open during runtime: \"{virtualFolderSpecifiedAbsPath}\"");
								return;
							}

							if (GetAnimationClipCount(virtualFolderSpecifiedAbsPath) == 1)
							{
								// only 1 animation in this fbx file
								// instead of bothering to go inside the fbx file,
								// open the folder containing the fbx file instead

								fileSpecified = virtualFolderSpecified;
								virtualFolderSpecified = null;
								virtualFileSpecified = null;
								virtualFolderSpecifiedAbsPath = null;

								Debug.LogFormat(
									"FileBrowser.SwitchCurrentPath(): not going inside fbx file anymore since only 1 animation inside it.\npathToSwitchTo: {0}\nfileSpecified: {1}",
									fullPath, fileSpecified);
							}
						}
						else
						{
							Debug.LogError(
								$"FileBrowser.SwitchCurrentPath(): Can't open: \"{virtualFolderSpecifiedAbsPath}\"");
							return;
						}
					}
					else
					{
						Debug.LogError(
							$"Shouldn't be entering inside fbx files if not browsing animations. Path: \"{fullPath}\"");
					}
				}
				else
				{
					// unknown virtual folder

					Debug.LogError(
						$"FileBrowser.SwitchCurrentPath(): Unknown virtual folder in specified path: \"{fullPath}\"");

					// just remove everything after the colon

					var colonIdx = fullPath.IndexOf(':');
					fullPath = fullPath.Substring(0, colonIdx);
				}
			}
			else if (File.Exists(fullPath))
			{
				// fullPath should only be pointing to a folder
				// since it's pointing to a file, we go to the containing folder
				// and have that file highlighted

				if (IsFilenameAnAssetBundle(fullPath))
				{
					// going inside asset bundle
					Debug.LogError("Going into an Asset Bundle, Not yet implemented");

					virtualFolderSpecified = Path.GetFileName(fullPath);
					fullPath = Path.GetDirectoryName(fullPath);
				}
				else if (IsBrowserTypeForAnimations)
				{
					if (fullPath.EndsWith(FileUtil.FBX_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
					{
						// going inside fbx file to view animation clips inside

						// if only 1 animation in this fbx file
						// instead of bothering to go inside the fbx file,
						// open the folder containing the fbx file instead
						if (GetAnimationClipCount(fullPath) == 1)
						{
							fileSpecified = Path.GetFileName(fullPath);
							fullPath = Path.GetDirectoryName(fullPath);
							virtualFolderSpecified = null;
							virtualFileSpecified = null;
							virtualFolderSpecifiedAbsPath = null;

							Debug.LogFormat(
								"FileBrowser.SwitchCurrentPath(): not going inside fbx file anymore since only 1 animation inside it.\nfullPath: {0}\nfileSpecified: {1}",
								fullPath, fileSpecified);
						}
						else
						{
							// the absolute path including the fbx filename
							virtualFolderSpecifiedAbsPath = fullPath;

							// the fbx filename only (without the path)
							virtualFolderSpecified = Path.GetFileName(fullPath);

							// the absolute path of only the folders, no filename
							fullPath = Path.GetDirectoryName(fullPath);

							Debug.LogFormat(
								"FileBrowser.SwitchCurrentPath(): going inside fbx file.\npathToSwitchTo: {0}\nvirtualFolderSpecified: {1}\nvirtualFolderSpecifiedAbsPath: {2}",
								fullPath, virtualFolderSpecified, virtualFolderSpecifiedAbsPath);
						}
					}
					else
					{
						// the path isn't ".fbx" so we're not moving into an fbx file
						// treat the path like normal

						fileSpecified = Path.GetFileName(fullPath);
						fullPath = Path.GetDirectoryName(fullPath);
					}
				}
				else
				{
					// specified file is just a regular file, not a virtual folder

					fileSpecified = Path.GetFileName(fullPath);
					fullPath = Path.GetDirectoryName(fullPath);
				}
			}
			// else: we assume fullPath is already a folder-only path

			// -----------------------
			// abort if no change is needed

			if (_currentPath == fullPath &&
			    _currentVirtualFolder == virtualFolderSpecified &&
			    string.IsNullOrEmpty(fileSpecified) &&
			    string.IsNullOrEmpty(virtualFileSpecified))
			{
				// only going to a folder, and already at that folder
				Debug.Log($"already at same folder: {_currentPath}\nsame virtual folder: {_currentVirtualFolder}");
				return;
			}

			if (_currentPath == fullPath &&
			    _currentVirtualFolder == virtualFolderSpecified &&
			    !string.IsNullOrEmpty(fileSpecified) &&
			    _files.Length > 0 &&
			    _selectedFileIdx >= 0)
			{
				// specified a file, in a path that we're already in.
				// just make sure the specified file is selected.

				// if specified file is already selected, abort
				if (_files[_selectedFileIdx] == fileSpecified)
				{
					Debug.Log("aborting change since file specified is already selected");
					return;
				}

				// just make sure specified file is selected
				for (int n = 0; n < _files.Length; n++)
				{
					if (_files[n].Equals(fileSpecified, StringComparison.OrdinalIgnoreCase))
					{
						_selectedFileIdx = n;
						break;
					}
				}

				return;
			}

			// -----------------------
			// remove any folder in the path that is non-existent

			var accessible = FileUtil.FixPath(fullPath, out fullPath);

			if (!accessible)
			{
				Debug.LogWarning($"Can't access path: {fullPath}");
				return;
			}

			if (!string.IsNullOrEmpty(virtualFolderSpecified))
			{
				// ensure this virtual folder exists
				// the virtual folder can either be an asset bundle file, or fbx file
				if (!File.Exists(virtualFolderSpecifiedAbsPath))
				{
					Debug.LogWarning(
						$"Can't access file (perhaps it was renamed or deleted?): {virtualFolderSpecifiedAbsPath}");

					// in this case, just treat it as if we're only moving to the folder path
					// and not into a virtual folder
					virtualFolderSpecified = null;
					virtualFolderSpecifiedAbsPath = null;
					virtualFileSpecified = null;
					virtualPathSpecified = null;
				}
			}

			// -----------------------

			if (!string.IsNullOrEmpty(virtualFileSpecified))
			{
				_fileToSelect = virtualFileSpecified;
			}
			else if (!string.IsNullOrEmpty(fileSpecified))
			{
				_fileToSelect = fileSpecified;
			}

			if (_pathHistory.Count > 0)
			{
				// just before leaving the current path
				// make sure it remembers the scrollbar position and whatever file was selected
				var currentEntry = _pathHistory[_pathHistory.Count - 1];
				currentEntry.ScrollbarPosition = _scrollPosition.y;
				currentEntry.SelectedFileIdx = _selectedFileIdx;
				currentEntry.SelectedFolderIdx = _selectedFolderIdx;

				_pathHistory[_pathHistory.Count - 1] = currentEntry;
			}

			AddPathHistory(fullPath, virtualFolderSpecified, virtualPathSpecified);

			_SwitchCurrentPath(fullPath, virtualFolderSpecified, virtualPathSpecified);
		}

		void AddPathHistory(string path, string virtualFolder = null, string virtualPath = null)
		{
			// if user is viewing part of the history
			// remove path history entries beyond this point
			// since we are inserting a new entry to the history
			if (_pathHistory.Count > 1 && (_pathHistoryIdx != _pathHistory.Count - 1))
			{
				Debug.LogFormat("removing starting from {0}, count {1}. total count {2}.",
					(_pathHistoryIdx + 1).ToString(), (_pathHistory.Count - _pathHistoryIdx - 1).ToString(),
					_pathHistory.Count.ToString());

				_pathHistory.RemoveRange(_pathHistoryIdx + 1, _pathHistory.Count - _pathHistoryIdx - 1);
			}

			PathHistoryEntry newEntry;
			newEntry.Path = path;
			newEntry.VirtualFolder = virtualFolder;
			newEntry.VirtualPath = virtualPath;
			newEntry.ScrollbarPosition = 0;
			newEntry.SelectedFileIdx = -1;
			newEntry.SelectedFolderIdx = -1;

			_pathHistory.Add(newEntry);
			_pathHistoryIdx = _pathHistory.Count - 1;
		}

		bool CanMovePathHistoryBackward
		{
			get
			{
				return _pathHistory.Count > 1 && _pathHistoryIdx > 0;
			}
		}

		void MovePathHistoryBackward()
		{
			if (_pathHistoryIdx == 0 || _pathHistory.Count <= 1)
			{
				return;
			}

			--_pathHistoryIdx;
			_SwitchCurrentPath(_pathHistory[_pathHistoryIdx]);
		}

		bool CanMovePathHistoryForward
		{
			get
			{
				return _pathHistory.Count > 1 && _pathHistoryIdx < _pathHistory.Count - 1;
			}
		}

		void MovePathHistoryForward()
		{
			if (_pathHistoryIdx == _pathHistory.Count || _pathHistory.Count <= 1)
			{
				return;
			}

			++_pathHistoryIdx;
			_SwitchCurrentPath(_pathHistory[_pathHistoryIdx]);
		}

		// -------------------------------------------------------------------------------------------------


		// -------------------------------------------------------------------------------------------------

		/// <summary>
		/// Will give values to <see cref="_currentPathParts"/>, <see cref="_folders"/>,
		/// <see cref="_files"/>, and <see cref="_nonMatchingFiles"/>.
		/// </summary>
		/// <param name="newPathToRead"></param>
		/// <param name="virtualFolder"></param>
		/// <param name="virtualPath"></param>
		/// <param name="previousPath">only used for debugging</param>
		protected void ReadDirectoryContents(string newPathToRead, string virtualFolder, string virtualPath,
			string previousPath)
		{
			if (IsPathRoot(newPathToRead))
			{
				_currentPathParts = new[] { "" };
				_files = Array.Empty<string>();
				_nonMatchingFiles = Array.Empty<string>();

				// note: attempting to use System.IO.DriveInfo.GetDrives() results in a NotImplementedException (at least in Unity 5.5.5)
				_folders = Directory.GetLogicalDrives();

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
				for (int n = 0; n < _folders.Length; ++n)
				{
					_folders[n] = FileUtil.GetFormattedDriveName(_folders[n]);
				}
#endif

				// todo also insert favorites folders, plus desktop and my docs
			}
			else
			{
				bool isVirtualFolderAnFbxFile = IsAnimationVirtualFolderAnFbxFile(newPathToRead, virtualFolder);

				// -------------------------------------------------------------------
				// initialize _currentPathParts

				if (!string.IsNullOrEmpty(virtualFolder))
				{
					var realPathParts = newPathToRead.Split(FolderDelimiter);
					List<string> finalPathParts;

					if (isVirtualFolderAnFbxFile)
					{
						finalPathParts = new List<string>(realPathParts.Length + 1);
						finalPathParts.AddRange(realPathParts);

						finalPathParts.Add(virtualFolder);
					}
					else if (IsFilenameAnAssetBundle(virtualFolder))
					{
						finalPathParts = new List<string>(realPathParts.Length + 1);
						finalPathParts.AddRange(realPathParts);

						finalPathParts.Add(virtualFolder);
					}
					else
					{
						// unknown virtual folder type
						finalPathParts = new List<string>();
					}

					_currentPathParts = finalPathParts.ToArray();
				}
				else
				{
					// no virtual folder, normal behaviour
					if (HasLockPath)
					{
						_currentPathParts = CreateLockPathParts(newPathToRead);
					}
					else
					{
						_currentPathParts = newPathToRead.Split(FolderDelimiter);
					}
				}


				// -------------------------------------------------------------------
				// initialize _folders

				List<string> folders;

				if (!string.IsNullOrEmpty(virtualFolder))
				{
					if (isVirtualFolderAnFbxFile)
					{
						// fbx files do not have "folders" inside it, except the "exit" that we'll add
						folders = new List<string>();
						folders.Add(UP_FOLDER_LABEL);
					}
					else if (IsFilenameAnAssetBundle(virtualFolder))
					{
						// show all folders in the asset bundle
						Debug.LogError("FileBrowser.ReadDirectoryContents on Asset Bundle: not yet implemented");

						folders = new List<string>();
						folders.Add(UP_FOLDER_LABEL);

						if (!string.IsNullOrEmpty(virtualPath))
						{
						}
						else
						{
							// no specified path
						}
					}
					else
					{
						// unknown virtual folder type
						folders = new List<string>();
					}
				}
				else
				{
					// no virtual folder, normal behaviour
					folders = GetFolderEntries(newPathToRead);
					if (HasLockPath)
					{
						Debug.LogFormat("lockpath\n{0}\ncurrent path\n{1}", _lockPath, newPathToRead);
						if (!AlreadyAtLockPathIfApplicable)
						{
							folders.Insert(0, UP_FOLDER_LABEL);
						}
					}
					else
					{
						if (!FileUtil.IsRootPath(newPathToRead))
						{
							folders.Insert(0, UP_FOLDER_LABEL);
						}
					}
				}

				_folders = folders.ToArray();

				// -------------------------------------------------------------------
				// initialize _files and _nonMatchingFiles

				if (IsBrowserTypeForFolders || string.IsNullOrEmpty(_selectionPattern))
				{
					// either this browser is for folders, or there is no selection pattern
					// consider all files to be "matching"

					_files = Directory.GetFiles(newPathToRead);
					_nonMatchingFiles = Array.Empty<string>();
				}
				else
				{
					// -------------------------------------------
					// get matching files
					var matchingFiles = new List<string>();
					var removedFromMatchingFiles = new List<string>();

					if (!string.IsNullOrEmpty(virtualFolder))
					{
						if (isVirtualFolderAnFbxFile)
						{
							// get all animation clips inside the fbx file
							// note: this method only works in editor (not runtime)
							// for runtime, animations have to be contained inside an AssetBundle

							if (newPathToRead.Contains(FileUtil.RESOURCES_FOLDER, StringComparison.OrdinalIgnoreCase))
							{
								// fbx to "open" is in a Resources folder

								var resourcesPathOfFbxFile =
									FileUtil.GetResourcesPathOfFile(Path.Combine(newPathToRead, virtualFolder));

								var anims = Resources.LoadAll<AnimationClip>(resourcesPathOfFbxFile);

								foreach (var anim in anims)
								{
									matchingFiles.Add(anim.name);
								}
							}
							else
							{
								// fbx to "open" is not in a Resources folder

#if UNITY_EDITOR
								var fbxAbsPath = Path.Combine(newPathToRead, virtualFolder);
								var fbxAssetsPath = FileUtil.GetPathRelativeToAssets(fbxAbsPath);

								Object[] subAssets =
									AssetDatabase.LoadAllAssetRepresentationsAtPath(fbxAssetsPath);
#else
								UnityEngine.Object[] subAssets = null;
#endif

								if (subAssets == null || subAssets.Length == 0)
								{
									Debug.LogError(
										$"FileBrowser.ReadDirectoryContents(): no sub-assets inside of \"{newPathToRead}\"");
									return;
								}

								for (int subIdx = 0; subIdx < subAssets.Length; ++subIdx)
								{
									if (subAssets[subIdx] is AnimationClip)
									{
										matchingFiles.Add(subAssets[subIdx].name);
									}
								}

								if (matchingFiles.Count == 0)
								{
									Debug.LogWarning(
										$"FileBrowser.ReadDirectoryContents(): no animation clips inside of \"{newPathToRead}\"");
								}
							}
						}
						else if (IsFilenameAnAssetBundle(virtualFolder))
						{
							// get all matching assets inside the asset bundle, given the virtualPath
							// using:
							// if IsBrowserTypeForImages: AssetBundle.LoadAllAssets(typeof(Texture))
							// if IsBrowserTypeForAnimations: AssetBundle.LoadAllAssets(typeof(AnimationClip))
							// if IsBrowserTypeForSounds: AssetBundle.LoadAllAssets(typeof(AudioClip))
							//
							// it's also possible to use the Async version instead AssetBundle.LoadAllAssetsAsync()
							// so that the game won't lag while this is being processed

							Debug.LogError("FileBrowser.ReadDirectoryContents on Asset Bundle: not yet implemented");
						}
						else
						{
							// unknown/unsupported virtual folder type
							Debug.LogError(
								$"FileBrowser.ReadDirectoryContents unknown/unsupported virtual folder type:\nnewPathToRead: {newPathToRead}\nvirtualFolder: {virtualFolder}\nvirtualPath: {virtualPath}");
						}
					}
					else
					{
						// no virtual folder, normal behaviour
						if (HasVirtualFilesCallback)
						{
							var virtualFilesToAdd = _getVirtualFilesCallback(newPathToRead);
							if (virtualFilesToAdd != null)
							{
								matchingFiles.AddRange(virtualFilesToAdd);
							}
						}

						if (IsBrowserTypeForAnimations &&
						    _operationType == OperationType.Open &&
						    IsPathInResourcesOfProject(newPathToRead))
						{
							// for animation browsing,
							// add fbx file only if it has at least 1 animation inside

							foreach (var pattern in _selectionPatterns)
							{
								var matchingFilesFromPattern = Directory.GetFiles(newPathToRead, pattern);

								if (matchingFilesFromPattern.Length == 0)
								{
									// no files in path for this pattern
									continue;
								}

								if (matchingFilesFromPattern[0]
								    .EndsWith(FileUtil.FBX_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
								{
									// these are fbx files
									for (int n = 0, len = matchingFilesFromPattern.Length; n < len; ++n)
									{
										var resourcesPathOfFbxFile =
											FileUtil.GetResourcesPathOfFile(matchingFilesFromPattern[n]);

										var anims = Resources.LoadAll<AnimationClip>(resourcesPathOfFbxFile);

										if (anims.Length > 0)
										{
											matchingFiles.Add(matchingFilesFromPattern[n]);
										}
										else
										{
											removedFromMatchingFiles.Add(matchingFilesFromPattern[n]);
										}
									}
								}
								else
								{
									// not fbx files, just go ahead and add all of them then
									matchingFiles.AddRange(matchingFilesFromPattern);
								}
							}
						}
						else
						{
							// for all other types of browsers,
							// pattern matching is enough to ensure that they are allowed

							foreach (var pattern in _selectionPatterns)
							{
								matchingFiles.AddRange(Directory.GetFiles(newPathToRead, pattern));
							}
						}
					}

					_files = matchingFiles.ToArray();

					// ----------------------------------------
					// get non-matching files

					if (!string.IsNullOrEmpty(virtualFolder))
					{
						if (isVirtualFolderAnFbxFile)
						{
							// an fbx file has no non-matching files inside

							_nonMatchingFiles = Array.Empty<string>();
						}
						else if (IsFilenameAnAssetBundle(virtualFolder))
						{
							// get all non-matching assets inside the asset bundle, given the virtualPath

							Debug.Log("FileBrowser.ReadDirectoryContents on Asset Bundle: not yet implemented");
							_nonMatchingFiles = Array.Empty<string>();
						}
						else
						{
							// unknown virtual folder type
							_nonMatchingFiles = Array.Empty<string>();
						}
					}
					else
					{
						// no virtual folder, normal behaviour

						var nonMatchingFiles = new List<string>();

						// add all files that aren't found in the matching files
						foreach (var filePath in Directory.GetFiles(newPathToRead))
						{
							bool fileNotInMatchingFiles = true;
							for (int n = 0; n < _files.Length; n++)
							{
								if (_files[n].Equals(filePath, StringComparison.OrdinalIgnoreCase))
								{
									fileNotInMatchingFiles = false;
									break;
								}
							}

							if (fileNotInMatchingFiles && !FileUtil.IsUselessFile(filePath))
							{
								nonMatchingFiles.Add(filePath);
							}
						}

						nonMatchingFiles.AddRange(removedFromMatchingFiles);

						_nonMatchingFiles = nonMatchingFiles.ToArray();
					}

					FormatFilenameArray(ref _nonMatchingFiles);
				}

				FormatFilenameArray(ref _files);
			}


			// -------------------------------------------------------------------
			// auto-select specified file if present, or first entry in folder contents
			if (!string.IsNullOrEmpty(_fileToSelect))
			{
				int specifiedFileIdx = -1;
				for (int n = 0; n < _files.Length; n++)
				{
					if (_files[n].Equals(_fileToSelect, StringComparison.OrdinalIgnoreCase))
					{
						specifiedFileIdx = n;
						break;
					}
				}

				Debug.Log($"specified file idx: {specifiedFileIdx.ToString()}");

				if (specifiedFileIdx >= 0 && specifiedFileIdx < _files.Length)
				{
					_selectedFileIdx = specifiedFileIdx;
					_selectedFolderIdx = -1;
				}

				_fileToSelect = null;
			}
			else if (!string.IsNullOrEmpty(_pathUsed.Path))
			{
				// if this is a result of going back through history
				// then reselect what was recorded in that history entry
				_selectedFileIdx = _pathUsed.SelectedFileIdx;
				_selectedFolderIdx = _pathUsed.SelectedFolderIdx;
			}
			else
			{
				// just select the first file/folder

				if (_folders.Length > 0)
				{
					_selectedFolderIdx = 0;
					_selectedFileIdx = -1;
				}
				else if (_files.Length > 0)
				{
					_selectedFileIdx = 0;
					_selectedFolderIdx = -1;
				}
			}

			BuildContent();

			LoadImagesInCurrentPath();
		}


		void LoadImagesInCurrentPath(int startIdx = 0, int len = DEFAULT_NUMBER_OF_IMAGES_TO_AUTO_LOAD,
			bool forceReload = false)
		{
			if (startIdx >= _files.Length)
			{
				return;
			}

			if (!IsBrowserTypeForImages || _files.Length <= 0)
			{
				return;
			}

			// note: files listed in `_files` are all images at this point

			int imagesToAutoLoad = Mathf.Min(len, _files.Length);
			if (startIdx + imagesToAutoLoad >= _files.Length)
			{
				imagesToAutoLoad = _files.Length - startIdx;
			}

			Debug.Log(
				$"Loading images from {startIdx.ToString()}, length: {imagesToAutoLoad.ToString()} final idx: {(startIdx + imagesToAutoLoad - 1).ToString()}");

			for (int i = startIdx; i < startIdx + imagesToAutoLoad; ++i)
			{
				if (string.IsNullOrEmpty(_files[i]))
				{
					continue;
				}

				var fileAbsolutePath = FileUtil.NormalizedCombinePath(_currentPath, _files[i]);
				if (!File.Exists(fileAbsolutePath))
				{
					continue;
				}

				var fileUrl = $"file://{fileAbsolutePath}";

				if (!forceReload && _fileThumbnails.ContainsKey(fileUrl))
				{
					if (_filesWithImages[i] != null)
					{
						_filesWithImages[i].image = _fileThumbnails[fileUrl];
					}
					else
					{
						_filesWithImages[i] = new GUIContent(_files[i], _fileThumbnails[fileUrl]);
					}

					continue;
				}

#if UNITY_EDITOR
				Debug.Log($"Starting Coroutine for {i.ToString()}: {fileUrl}");
				EditorCoroutineUtility.StartCoroutine(
					DownloadImage(fileUrl, fileAbsolutePath, i), this);
#else
				// todo call coroutine during runtime
#endif
			}

			_lastLoadedImageIdx = startIdx + imagesToAutoLoad;
		}

		IEnumerator DownloadImage(string url, string absolutePath, int fileIdx)
		{
			using (var uwr = UnityWebRequestTexture.GetTexture(url))
			{
				yield return uwr.SendWebRequest();

				Texture2D texture;
				if (uwr.result == UnityWebRequest.Result.Success)
				{
					texture = DownloadHandlerTexture.GetContent(uwr);
				}
				else
				{
					Debug.LogError($"{url}\n{uwr.error}");
					texture = TextureUtil.SetTextureFromFile(absolutePath);
				}

				if (texture == null)
				{
					Debug.LogError($"Was not able to load image: {url}");
					yield break;
				}

				if (_fileThumbnails.ContainsKey(url))
				{
					_fileThumbnails[url] = texture;
				}
				else
				{
					_fileThumbnails.Add(url, texture);
				}

				//var filename = Path.GetFileName(url);
				//Debug.Log($"Got texture for {filename}\nmipmap count: {texture.mipmapCount.ToString()}", texture);

				if (_filesWithImages[fileIdx] != null)
				{
					_filesWithImages[fileIdx].image = texture;
				}
				else
				{
					_filesWithImages[fileIdx] = new GUIContent(_files[fileIdx], texture);
				}
				//Debug.Log($"Assigned image for {i.ToString()}: {filename}", texture);
			}
		}

		static List<string> GetFolderEntries(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			if (!Directory.Exists(path))
			{
				return null;
			}

			List<string> gotFolders = new List<string>(Directory.GetDirectories(path));
			List<string> allowedFolders = new List<string>();

			// gotFolders have values in absolute path
			// get only folder name out of the absolute path given
			for (int i = 0; i < gotFolders.Count; ++i)
			{
				int folderIdx = gotFolders[i].LastIndexOf('/');
				folderIdx = Mathf.Max(folderIdx, gotFolders[i].LastIndexOf('\\'));

				gotFolders[i] = gotFolders[i].Substring(folderIdx + 1);

				// don't include folders that we shouldn't
				// really go into, like the Recycle Bin
				if (FileUtil.IsSystemFolder(gotFolders[i]))
				{
					continue;
				}

				allowedFolders.Add(gotFolders[i]);
			}

			return allowedFolders;
		}

		/// <summary>
		/// Ensure that values inside are only filenames (no paths),
		/// and sort them alphabetically.
		/// </summary>
		/// <param name="filenameArray"></param>
		static void FormatFilenameArray(ref string[] filenameArray)
		{
			for (int i = 0; i < filenameArray.Length; ++i)
			{
				filenameArray[i] = Path.GetFileName(filenameArray[i]);
			}

			Array.Sort(filenameArray);
		}

		static bool IsDriveName(string name)
		{
			return name.Contains(":");
		}

		static bool IsUnityProjectPath(string absolutePath)
		{
			var assetsSubfolder = $"{absolutePath}/Assets";
			var librarySubfolder = $"{absolutePath}/Library";
			var projectSettingsSubfolder = $"{absolutePath}/ProjectSettings";

			var hasAssetsSubfolder = Directory.Exists(assetsSubfolder);
			var hasLibrarySubfolder = Directory.Exists(librarySubfolder);
			var hasProjectSettingsSubfolder = Directory.Exists(projectSettingsSubfolder);

			//Debug.LogFormat("checking if \"{0}\" is unity project path: {1}\nassets: {2}\nlibrary: {3}\nprojectSettings: {4}",
			//	absolutePath, (hasAssetsSubfolder && hasLibrarySubfolder && hasProjectSettingsSubfolder), assetsSubfolder,
			//	librarySubfolder, projectSettingsSubfolder);

			return (hasAssetsSubfolder && hasLibrarySubfolder && hasProjectSettingsSubfolder);
		}

		Texture2D GetIconForFolder(string folderName)
		{
			var icon = _folderImage; // default is folder icon

			if (folderName == UP_FOLDER_LABEL)
			{
				icon = _toParentFolderImage;
			}
			else if (IsDriveName(folderName))
			{
				icon = _driveImage;
			}
			else
			{
				string folderFullPath = $"{_currentPath}/{folderName}";
				if (IsUnityProjectPath(folderFullPath) || IsFilenameAnAssetBundle(folderFullPath))
				{
					icon = _unityProjectFolderImage;
				}
				else if (folderName != null &&
				         folderName.EndsWith(FileUtil.FBX_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
				{
					icon = _prefabImage;
				}
				else if (_recentlyUsed.Exists(content => content.tooltip == folderFullPath))
				{
					icon = _specialFolderImage;
				}
			}

			return icon;
		}

		/// <summary>
		/// Turns list of files and folders into GUIContent.
		/// </summary>
		protected void BuildContent()
		{
			for (int n = 0; n < _recentlyUsed.Count; ++n)
			{
				_recentlyUsed[n].image = _specialFolderImage;
			}

			_currentPathPartsDisplayed = new GUIContent[_currentPathParts.Length];
			for (int n = 0; n < _currentPathParts.Length; ++n)
			{
				_currentPathPartsDisplayed[n] = new GUIContent(_currentPathParts[n]);

				if (IsDriveName(_currentPathParts[n]))
				{
					_currentPathPartsDisplayed[n].image = _driveImage;
				}
				else
				{
					string fullPath = null;

					if (n == _currentPathParts.Length - 1)
					{
						fullPath = _currentPath;
					}
					else
					{
						for (int f = 0; f <= n; ++f)
						{
							if (f > 0)
							{
								fullPath += "/";
							}

							fullPath += _currentPathParts[f];
						}
					}

					//Debug.LogFormat("checking if {0} of path \"{1}\" is unity project", _currentPathParts[n], fullPath);
					if (IsUnityProjectPath(fullPath) || IsFilenameAnAssetBundle(fullPath))
					{
						_currentPathPartsDisplayed[n].image = _unityProjectFolderImage;
					}
					else if (_currentPathParts[n] != null &&
					         _currentPathParts[n].EndsWith(FileUtil.FBX_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
					{
						_currentPathPartsDisplayed[n].image = _prefabImage;
					}
				}
			}


			_foldersWithImages = new GUIContent[_folders.Length];
			for (int i = 0; i < _foldersWithImages.Length; ++i)
			{
				_foldersWithImages[i] = new GUIContent(_folders[i], GetIconForFolder(_folders[i]));
			}

			if (IsBrowserTypeForImages)
			{
				ShowThumbnailsForImageItems();
			}
			else if (IsBrowserTypeForSounds)
			{
				if (_filesWithImages == null || _filesWithImages.Length != _files.Length)
				{
					_filesWithImages = new GUIContent[_files.Length];
				}

				for (int i = 0; i < _filesWithImages.Length; ++i)
				{
					_filesWithImages[i] = new GUIContent(_files[i], _matchedFileImage);
				}
			}
			else if (IsBrowserTypeForAnimations)
			{
				if (_filesWithImages == null || _filesWithImages.Length != _files.Length)
				{
					_filesWithImages = new GUIContent[_files.Length];
				}

				for (int i = 0; i < _filesWithImages.Length; ++i)
				{
					Texture icon;
					if (!string.IsNullOrEmpty(_currentVirtualFolder))
					{
						// we're inside an .fbx file
						// all "files" inside the .fbx file
						// are animation clips
						icon = _animationClipImage;
					}
					else if (_files[i] != null)
					{
						if (_files[i].EndsWith(FileUtil.FBX_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
						{
							icon = _prefabImage;
						}
						else if (_files[i].EndsWith(FileUtil.ANIM_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
						{
							icon = _animationClipImage;
						}
						else
						{
							icon = _matchedFileImage;
						}
					}
					else
					{
						icon = _matchedFileImage;
					}

					_filesWithImages[i] = new GUIContent(_files[i], icon);
				}
			}
			else
			{
				// default file entry generation

				if (_filesWithImages == null || _filesWithImages.Length != _files.Length)
				{
					_filesWithImages = new GUIContent[_files.Length];
				}

				for (int i = 0; i < _filesWithImages.Length; ++i)
				{
					_filesWithImages[i] = new GUIContent(_files[i], _matchedFileImage);
				}
			}


			_nonMatchingFilesWithImages = new GUIContent[_nonMatchingFiles.Length];
			for (int i = 0; i < _nonMatchingFilesWithImages.Length; ++i)
			{
				_nonMatchingFilesWithImages[i] = new GUIContent(_nonMatchingFiles[i], _fileImage);
			}

			// move path bar to rightmost to show innermost subfolder
			_directoryScrollPosition.x = float.MaxValue;

			// ----------------------

			if (!string.IsNullOrEmpty(_pathUsed.Path))
			{
				Debug.Log(
					$"FileBrowser.BuildContent(): making use of _pathUsed.Path: {_pathUsed.Path}. SelectedFileIdx: {_pathUsed.SelectedFileIdx}");

				_scrollPosition.x = 0;
				_scrollPosition.y = _pathUsed.ScrollbarPosition;

				_selectedFileIdx = _pathUsed.SelectedFileIdx;
				_selectedFolderIdx = _pathUsed.SelectedFolderIdx;

				_pathUsed.Path = string.Empty;
			}
		}


		void ShowThumbnailsForImageItems()
		{
			if (_browserBrowserType != BrowserType.Image)
			{
				return;
			}

			if (_filesWithImages == null || _filesWithImages.Length != _files.Length)
			{
				_filesWithImages = new GUIContent[_files.Length];
			}

			for (int i = 0; i < _filesWithImages.Length; ++i)
			{
				Texture2D imageToUse = _matchedFileImage;

				if (_fileThumbnails.ContainsKey(_files[i]))
				{
					imageToUse = _fileThumbnails[_files[i]];
				}

				if (_filesWithImages[i] != null)
				{
					_filesWithImages[i].image = imageToUse;
				}
				else
				{
					_filesWithImages[i] = new GUIContent(_files[i], imageToUse);
				}
			}
		}

		Vector2 _directoryScrollPosition;

		// -----------------------------------------------------------------------------

		const string FOLDER_ROOT_DEFAULT_STYLE_NAME = "FolderRootButton";
		const string FOLDER_DEFAULT_STYLE_NAME = "FolderButton";
		const string FOLDER_ROOT_WITH_ICON_DEFAULT_STYLE_NAME = "FolderRootWithIconButton";
		const string FOLDER_WITH_ICON_DEFAULT_STYLE_NAME = "FolderWithIconButton";
		const string LIST_ITEM_DEFAULT_STYLE_NAME = "ListItem";
		const string IMAGE_ITEM_DEFAULT_STYLE_NAME = "FileDlg.ImageItem";

		string _folderRootButtonStyleName = FOLDER_ROOT_DEFAULT_STYLE_NAME;
		string _folderButtonStyleName = FOLDER_DEFAULT_STYLE_NAME;
		string _folderRootWithIconButtonStyleName = FOLDER_ROOT_WITH_ICON_DEFAULT_STYLE_NAME;
		string _folderWithIconButtonStyleName = FOLDER_WITH_ICON_DEFAULT_STYLE_NAME;

		string _listItemStyleName = LIST_ITEM_DEFAULT_STYLE_NAME;
		string _imageItemStyleName = IMAGE_ITEM_DEFAULT_STYLE_NAME;

		GUIStyle _folderRootButtonStyle;
		GUIStyle _folderButtonStyle;
		GUIStyle _folderRootWithIconButtonStyle;
		GUIStyle _folderWithIconButtonStyle;
		GUIStyle _listItemStyle;
		GUIStyle _imageItemStyle;

		public void SetStyles(string folderRootButtonStyle, string folderButtonStyle, string listItemStyle,
			string imageItemStyle)
		{
			_folderRootButtonStyleName = folderRootButtonStyle;
			_folderButtonStyleName = folderButtonStyle;
			_listItemStyleName = listItemStyle;
			_imageItemStyleName = imageItemStyle;
		}

		public void InitializeStyles(GUISkin guiSkin)
		{
			_folderRootButtonStyle = guiSkin.GetStyle(_folderRootButtonStyleName);
			_folderButtonStyle = guiSkin.GetStyle(_folderButtonStyleName);
			_folderRootWithIconButtonStyle = guiSkin.GetStyle(_folderRootWithIconButtonStyleName);
			_folderWithIconButtonStyle = guiSkin.GetStyle(_folderWithIconButtonStyleName);
			_listItemStyle = guiSkin.GetStyle(_listItemStyleName);
			_imageItemStyle = guiSkin.GetStyle(_imageItemStyleName);
		}

		void InitializeStylesIfNeeded()
		{
			if (_folderButtonStyle != null)
			{
				return;
			}

			InitializeStyles(GUI.skin);
		}

		// -----------------------------------------------------------------------------

		const string UNITY_FOLDER_ICON_DEFAULT_STYLE_NAME = "FileDlg.UnityProjectFolder";

		const string ROOT_ICON_DEFAULT_STYLE_NAME = "FileDlg.Root";
		const string DESKTOP_ICON_DEFAULT_STYLE_NAME = "FileDlg.Desktop";
		const string USER_HOME_ICON_DEFAULT_STYLE_NAME = "FileDlg.UserHome";
		const string MY_DOCS_ICON_DEFAULT_STYLE_NAME = "FileDlg.MyDocs";

		const string FOLDER_ICON_DEFAULT_STYLE_NAME = "FileDlg.Folder";
		const string SPECIAL_FOLDER_ICON_DEFAULT_STYLE_NAME = "FileDlg.SpecialFolder";

		const string UP_FOLDER_ICON_DEFAULT_STYLE_NAME = "FileDlg.ToParentFolder";
		const string DRIVE_ICON_DEFAULT_STYLE_NAME = "FileDlg.Drive";
		const string FILE_ICON_DEFAULT_STYLE_NAME = "FileDlg.File";
		const string MATCHED_FILE_ICON_DEFAULT_STYLE_NAME = "FileDlg.MatchedFile";

		const string ANIMATION_CLIP_ICON_DEFAULT_STYLE_NAME = "Icon.AnimationClip";
		const string PREFAB_ICON_DEFAULT_STYLE_NAME = "Icon.Prefab";

		const string BACK_ICON_DEFAULT_STYLE_NAME = "FileDlg.Back";
		const string FORWARD_ICON_DEFAULT_STYLE_NAME = "FileDlg.Forward";
		const string JUMP_ICON_DEFAULT_STYLE_NAME = "FileDlg.Jump";
		const string EDIT_PATH_ICON_DEFAULT_STYLE_NAME = "FileDlg.EditPath";


		protected static Texture2D GetIcon(GUISkin guiSkin, string styleName)
		{
			return guiSkin.GetStyle(styleName).normal.background;
		}

		void InitializeImages(GUISkin guiSkin)
		{
			_unityProjectFolderImage = GetIcon(guiSkin, UNITY_FOLDER_ICON_DEFAULT_STYLE_NAME);
			_folderImage = GetIcon(guiSkin, FOLDER_ICON_DEFAULT_STYLE_NAME);
			_specialFolderImage = GetIcon(guiSkin, SPECIAL_FOLDER_ICON_DEFAULT_STYLE_NAME);
			_toParentFolderImage = GetIcon(guiSkin, UP_FOLDER_ICON_DEFAULT_STYLE_NAME);
			_driveImage = GetIcon(guiSkin, DRIVE_ICON_DEFAULT_STYLE_NAME);
			_fileImage = GetIcon(guiSkin, FILE_ICON_DEFAULT_STYLE_NAME);
			_matchedFileImage = GetIcon(guiSkin, MATCHED_FILE_ICON_DEFAULT_STYLE_NAME);

			_animationClipImage = GetIcon(guiSkin, ANIMATION_CLIP_ICON_DEFAULT_STYLE_NAME);
			_prefabImage = GetIcon(guiSkin, PREFAB_ICON_DEFAULT_STYLE_NAME);

			_backButtonLabel.image = GetIcon(guiSkin, BACK_ICON_DEFAULT_STYLE_NAME);
			_forwardButtonLabel.image = GetIcon(guiSkin, FORWARD_ICON_DEFAULT_STYLE_NAME);
			_jumpButtonLabel.image = GetIcon(guiSkin, JUMP_ICON_DEFAULT_STYLE_NAME);
			_editPathButtonLabel.image = GetIcon(guiSkin, EDIT_PATH_ICON_DEFAULT_STYLE_NAME);
			_openInExplorerButtonLabel.image = GetIcon(guiSkin, ROOT_ICON_DEFAULT_STYLE_NAME);

			_rootLocationLabel.image = GetIcon(guiSkin, ROOT_ICON_DEFAULT_STYLE_NAME);
			_desktopLocationLabel.image = GetIcon(guiSkin, DESKTOP_ICON_DEFAULT_STYLE_NAME);
			_userHomeLocationLabel.image = GetIcon(guiSkin, USER_HOME_ICON_DEFAULT_STYLE_NAME);
			_myDocsLocationLabel.image = GetIcon(guiSkin, MY_DOCS_ICON_DEFAULT_STYLE_NAME);
			_thisProjectAssetsLocationLabel.image = GetIcon(guiSkin, UNITY_FOLDER_ICON_DEFAULT_STYLE_NAME);
			_thisProjectStreamingAssetsLabel.image = GetIcon(guiSkin, UNITY_FOLDER_ICON_DEFAULT_STYLE_NAME);
			_gameCommonDataLabel.image = GetIcon(guiSkin, SPECIAL_FOLDER_ICON_DEFAULT_STYLE_NAME);

			InitializeAdditionalImages(guiSkin);

			BuildContent();
		}

		void InitializeImagesIfNeeded()
		{
			if (_folderImage != null)
			{
				return;
			}

			InitializeImages(GUI.skin);
		}

		void CalculateTopVisibleItem(Vector2 newScrollPos)
		{
			var calculatedPosY = 0f;
			for (int n = 0, len = _foldersWithImages.Length; n < len; ++n)
			{
				var height = _listItemStyle.CalcHeight(_foldersWithImages[n], _scrollViewRect.width);
				calculatedPosY += height + _listItemStyle.margin.top;
				if (calculatedPosY > newScrollPos.y)
				{
					_topVisibleFolderIdx = n;
					_topVisibleFileIdx = -1;
					_bottomVisibleFileIdx = -1;
					return;
				}
			}

			var matchingFileStyle = (IsBrowserTypeForImages) ? _imageItemStyle : _listItemStyle;
			for (int n = 0, len = _filesWithImages.Length; n < len; ++n)
			{
				var height = matchingFileStyle.CalcHeight(_filesWithImages[n], _scrollViewRect.width);
				//Debug.Log($"got height for {n.ToString()}: {height.ToString(CultureInfo.InvariantCulture)}");

				calculatedPosY += height + matchingFileStyle.margin.top;
				if (calculatedPosY > newScrollPos.y)
				{
					_topVisibleFolderIdx = -1;
					_topVisibleFileIdx = n;
					break;
				}
			}

			_bottomVisibleFileIdx = -1;

			if (_topVisibleFileIdx > -1 && _topVisibleFileIdx < _filesWithImages.Length - 1)
			{
				var bottomPosY = newScrollPos.y + _scrollViewRect.height;
				for (int n = _topVisibleFileIdx + 1, len = _filesWithImages.Length; n < len; ++n)
				{
					var height = matchingFileStyle.CalcHeight(_filesWithImages[n], _scrollViewRect.width);
					//Debug.Log($"got height for {n.ToString()}: {height.ToString(CultureInfo.InvariantCulture)}");

					calculatedPosY += height + matchingFileStyle.margin.top;
					if (calculatedPosY >= bottomPosY)
					{
						_bottomVisibleFileIdx = n;
						break;
					}
				}

				if (_bottomVisibleFileIdx == -1)
				{
					// reached last file but last file's cumulative height did not pass through bottomPosY
					// that means the last file's pos Y was too low
					// just set _bottomVisibleFileIdx to the max it possibly can
					_bottomVisibleFileIdx = _filesWithImages.Length - 1;
				}
			}
		}

		// -----------------------------------------------------------------------------

		bool _recalculateContentScrollPosOnNextRepaint;

		void CheckLoadImages()
		{
			if (_topVisibleFileIdx == -1)
			{
				if (_filesWithImages.Length > 0)
				{
					LoadImagesInCurrentPath();
				}

				return;
			}

			for (int n = _topVisibleFileIdx; n <= _bottomVisibleFileIdx; ++n)
			{
				// find first file (among the visible ones) that has no image loaded
				// we'll loaded the images starting there
				if (DoesFileNotHaveImageThumbnail(n))
				{
					// make sure we load images for all that are visible
					var visibleImageCount = _bottomVisibleFileIdx - _topVisibleFileIdx + 1;
					var len = Mathf.Max(DEFAULT_NUMBER_OF_IMAGES_TO_AUTO_LOAD, visibleImageCount);

					LoadImagesInCurrentPath(n, len);
					break;
				}
			}
		}

		const int SCROLLBAR_THICKNESS = 15;
		const int SCROLLBAR_SPACE = SCROLLBAR_THICKNESS + 6;

		public void OnGUI()
		{
			InitializeStylesIfNeeded();
			InitializeImagesIfNeeded();

			DrawJumpToList();

			bool backspaceWasPressed = Event.current.isKey &&
			                           (Event.current.type == EventType.KeyUp) &&
			                           (Event.current.keyCode == KeyCode.Backspace);

			if (backspaceWasPressed && _showPathAsButtons)
			{
				SwitchToParentFolderOfCurrentPath();
				_editingCurrentPath = _currentPath;
				Utility.EatEvent();
			}


			GUILayout.BeginVertical();

			DrawToolbar();

			// -----------------------------------------------------------------------------
			// current path string (each folder in the path as a button)

			DrawCurrentPath();

			DrawOkCancelButtons();

			// -----------------------------------------------------------------------------
			// current path contents

			var newScrollPos = GUILayout.BeginScrollView(_scrollPosition,
				false,
				true,
				GUI.skin.horizontalScrollbar,
				GUI.skin.verticalScrollbar,
				GUI.skin.box);

			if (!Mathf.Approximately(_scrollPosition.y, newScrollPos.y))
			{
				_recalculateContentScrollPosOnNextRepaint = true;
			}

			_scrollPosition = newScrollPos;

			if (Event.current.type == EventType.Repaint && _recalculateContentScrollPosOnNextRepaint)
			{
				_recalculateContentScrollPosOnNextRepaint = false;
				CalculateTopVisibleItem(newScrollPos);
				_checkLoadImagesFrameCountdown = DEFAULT_IMAGE_LOAD_DELAY_SECONDS;
			}

			// first list: folders
			(_selectedFolderIdx, _hoveredFolderIdx, _) = SelectionList.Draw(_selectedFolderIdx,
				_hoveredFolderIdx,
				_foldersWithImages,
				_listItemStyle,
				_scrollViewRect.width - SCROLLBAR_SPACE,
				null,
				DirectoryDoubleClickCallback);

			// if a folder is selected, unselect file
			if (_selectedFolderIdx > -1)
			{
				_selectedFileIdx = -1;
			}

			if (_hoveredFolderIdx > -1)
			{
				_hoveredFileIdx = -1;
				_hoveredHasPrefabPreview = HasPreviewStatus.None;
			}

			var prevEnabled = GUI.enabled;

			// third list: files
			GUI.enabled = IsBrowserTypeForFiles;
			if (IsBrowserTypeForSounds)
			{
				DrawSoundFileEntries();
			}
			else
			{
				int newHoveredFileIdx;
				Rect hoveredRect;
				(_selectedFileIdx, newHoveredFileIdx, hoveredRect) = SelectionList.Draw(_selectedFileIdx,
					_hoveredFileIdx,
					_filesWithImages,
					(IsBrowserTypeForImages) ? _imageItemStyle : _listItemStyle,
					_scrollViewRect.width - SCROLLBAR_SPACE,
					FileSelectCallback,
					FileDoubleClickCallback);

				if (newHoveredFileIdx != _hoveredFileIdx)
				{
					_adjustedMouseY = Event.current.mousePosition.y - _scrollPosition.y;
					if (_adjustedMouseY >= 0 && _adjustedMouseY <= _scrollViewRect.height - SCROLLBAR_SPACE)
					{
						// new hovered
						//Debug.Log($"New hovered file: {newHoveredFileIdx.ToString()}: {(newHoveredFileIdx > -1 ? _files[newHoveredFileIdx] : "none")} at {Event.current.type}");
						_hoveredFileIdx = newHoveredFileIdx;
						_hoveredFileRect = hoveredRect;
						FindPreviewImageOfHoveredAsset();

						if (IsBrowserTypeForImages && DoesFileHaveImageThumbnail(_hoveredFileIdx))
						{
							int imageWidth = _filesWithImages[_hoveredFileIdx].image.width;
							int imageHeight = _filesWithImages[_hoveredFileIdx].image.height;

							var fileSize =
								FileUtil.GetBytesReadable(
									FileUtil.GetFileSizeInBytes($"{_currentPath}/{_files[_hoveredFileIdx]}"));
							_imageInfoLabel.text = $"{imageWidth.ToString()}✕{imageHeight.ToString()}, {fileSize}";
							_imageInfoSize = GUI.skin.label.CalcSize(_imageInfoLabel);
						}
					}
					else
					{
						_hoveredFileIdx = -1;
						_hoveredHasPrefabPreview = HasPreviewStatus.None;
					}
				}
			}

			// if a file is selected, unselect folder
			if (_selectedFileIdx > -1)
			{
				_selectedFolderIdx = -1;
			}

			if (_hoveredFileIdx > -1)
			{
				_hoveredFolderIdx = -1;
			}


			// fourth list: non-matching files
			GUI.enabled = false;
			SelectionList.Draw(-1, -1,
				_nonMatchingFilesWithImages,
				_listItemStyle,
				_scrollViewRect.width - SCROLLBAR_SPACE);

			GUI.enabled = prevEnabled;
			GUILayout.EndScrollView();
			if (Event.current.type == EventType.Repaint)
			{
				_scrollViewRect = GUILayoutUtility.GetLastRect();
			}


			if (_operationType == OperationType.Save)
			{
				_saveFilename = GUILayout.TextField(_saveFilename);
			}

			DrawOkCancelButtons();

			GUILayout.EndVertical();

			// -----------------------------------------
		}

		float _adjustedMouseY;

		void FindPreviewImageOfHoveredAsset()
		{
#if UNITY_EDITOR
			if (_hoveredFileIdx <= -1)
			{
				// no file hovered
				_hoveredHasPrefabPreview = HasPreviewStatus.None;
				return;
			}

			if (!_currentPath.Contains(Application.dataPath))
			{
				// not in project
				_hoveredHasPrefabPreview = HasPreviewStatus.None;
				return;
			}

			var hoveredFilePath = $"{_currentPath}/{_files[_hoveredFileIdx]}";
			if (_modelPrefabPreviewPath != hoveredFilePath)
			{
				var assetsIdx = hoveredFilePath.IndexOf("/Assets/", StringComparison.OrdinalIgnoreCase);
				if (assetsIdx > -1)
				{
					hoveredFilePath = hoveredFilePath.Substring(assetsIdx + 1);
#if UNITY_EDITOR
					var loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(hoveredFilePath);
					if (loadedPrefab != null)
					{
						_prefabPreview = AssetPreview.GetAssetPreview(loadedPrefab);

						if (AssetPreview.IsLoadingAssetPreview(loadedPrefab.GetInstanceID()))
						{
							_hoveredHasPrefabPreview = HasPreviewStatus.PreviewStillLoading;
						}
						else
						{
							_hoveredHasPrefabPreview =
								_prefabPreview != null ? HasPreviewStatus.HasPreview : HasPreviewStatus.None;
						}
					}
					else
#endif
					{
						_prefabPreview = null;
						_hoveredHasPrefabPreview = HasPreviewStatus.None;
					}
				}
				else
				{
					_prefabPreview = null;
					_hoveredHasPrefabPreview = HasPreviewStatus.None;
				}

				_modelPrefabPreviewPath = hoveredFilePath;
			}
#endif
		}

		public void OnOverlayGui(Vector2 offset)
		{
			OnOverlayGui(offset.x, offset.y);
		}

		public void OnOverlayGui(float offsetX, float offsetY)
		{
			if (_showJumpToList)
			{
				DrawJumpToList(offsetX);
			}
			else if (Event.current.type == EventType.Repaint)
			{
				if (_hoveredHasPrefabPreview == HasPreviewStatus.PreviewStillLoading)
				{
#if UNITY_EDITOR
					var loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_modelPrefabPreviewPath);
					if (loadedPrefab != null)
					{
						_prefabPreview = AssetPreview.GetAssetPreview(loadedPrefab);

						if (!AssetPreview.IsLoadingAssetPreview(loadedPrefab.GetInstanceID()))
						{
							_hoveredHasPrefabPreview =
								_prefabPreview != null ? HasPreviewStatus.HasPreview : HasPreviewStatus.None;
						}
					}
#endif
				}

				if (_hoveredHasPrefabPreview == HasPreviewStatus.HasPreview)
				{
					_tooltipRect = _hoveredFileRect;

					_tooltipRect.x = _scrollViewRect.xMax + offsetX;
					_tooltipRect.y += _scrollViewRect.y - _scrollPosition.y + offsetY;

					_tooltipRect.width = DEFAULT_IMAGE_TOOLTIP_WIDTH + DEFAULT_IMAGE_TOOLTIP_PADDING_H;
					_tooltipRect.height = DEFAULT_IMAGE_TOOLTIP_HEIGHT + DEFAULT_IMAGE_TOOLTIP_PADDING_V;

					FixTooltipRect(ref _tooltipRect, offsetX, offsetY);

					GUI.Box(_tooltipRect, GUIContent.none);

					// --------------------------------------------------------

					_tooltipRect.x += DEFAULT_IMAGE_TOOLTIP_PADDING_LEFT;
					_tooltipRect.y += DEFAULT_IMAGE_TOOLTIP_PADDING_TOP;
					_tooltipRect.width = DEFAULT_IMAGE_TOOLTIP_WIDTH;
					_tooltipRect.height = DEFAULT_IMAGE_TOOLTIP_HEIGHT;
					GUI.DrawTexture(_tooltipRect, _prefabPreview, ScaleMode.ScaleToFit);
				}
				else if (IsBrowserTypeForImages && DoesFileHaveImageThumbnail(_hoveredFileIdx))
				{
					_tooltipRect = _hoveredFileRect;

					_tooltipRect.x = _scrollViewRect.xMax + offsetX;
					_tooltipRect.y += _scrollViewRect.y - _scrollPosition.y + offsetY;

					int imageWidth = _filesWithImages[_hoveredFileIdx].image.width;
					int imageHeight = _filesWithImages[_hoveredFileIdx].image.height;

					_tooltipRect.width = Mathf.Max(imageWidth, _imageInfoSize.x) + DEFAULT_IMAGE_TOOLTIP_PADDING_H;
					_tooltipRect.height = imageHeight + _imageInfoSize.y + DEFAULT_IMAGE_TOOLTIP_PADDING_V;

					FixTooltipRect(ref _tooltipRect, offsetX, offsetY);
					var tooltipBottom = _tooltipRect.yMax;

					GUI.Box(_tooltipRect, GUIContent.none);

					// --------------------------------------------------------

					_tooltipRect.x += DEFAULT_IMAGE_TOOLTIP_PADDING_LEFT;
					_tooltipRect.y += DEFAULT_IMAGE_TOOLTIP_PADDING_TOP;
					_tooltipRect.width -= DEFAULT_IMAGE_TOOLTIP_PADDING_H;
					_tooltipRect.height -= DEFAULT_IMAGE_TOOLTIP_PADDING_V + _imageInfoSize.y;
					GUI.DrawTexture(_tooltipRect, _filesWithImages[_hoveredFileIdx].image, ScaleMode.ScaleToFit);

					_tooltipRect.y = tooltipBottom - _imageInfoSize.y - DEFAULT_IMAGE_TOOLTIP_PADDING_TOP;
					_tooltipRect.height = _imageInfoSize.y;
					GUI.Label(_tooltipRect, _imageInfoLabel);
				}
			}

			if (ShowDebugInfo)
			{
				var debugRect = _scrollViewRect;
				debugRect.x += _scrollViewRect.width + 25 + offsetX;
				debugRect.width = 450;

				if (debugRect.xMax > Screen.width)
				{
					debugRect.x = Screen.width - debugRect.width;
				}

				GUI.Label(debugRect,
					$@"_currentPath: {
						_currentPath
					}
_scrollPosition: {
	_scrollPosition.y.ToString(CultureInfo.InvariantCulture)
}
_scrollViewRect: {
	_scrollViewRect.ToString()
}
_scrollViewRect.yMax: {
	_scrollViewRect.yMax.ToString(CultureInfo.InvariantCulture)
}
_selectedFolderIdx: {
	_selectedFolderIdx.ToString()
}
_selectedFileIdx: {
	_selectedFileIdx.ToString()
}
_hoveredFolderIdx: {
	_hoveredFolderIdx.ToString()
}
_hoveredFileIdx: {
	_hoveredFileIdx.ToString()
}
_topVisibleFolderIdx: {
	_topVisibleFolderIdx.ToString()
}
_topVisibleFileIdx: {
	_topVisibleFileIdx.ToString()
}
_bottomVisibleFileIdx: {
	_bottomVisibleFileIdx.ToString()
}
_lastLoadedImageIdx: {
	_lastLoadedImageIdx.ToString()
}
_adjustedMouseY: {
	_adjustedMouseY.ToString(CultureInfo.InvariantCulture)
}
_tooltipRect: {
	_tooltipRect.ToString()
}
_tooltipRect.yMax: {
	_tooltipRect.yMax.ToString(CultureInfo.InvariantCulture)
}
Screen.width: {
	Screen.width.ToString()
}
Screen.height: {
	Screen.height.ToString()
}");
			}
		}

		readonly GUIContent _imageInfoLabel = new GUIContent();
		Vector2 _imageInfoSize;

		void FixTooltipRect(ref Rect tooltipRect, float offsetX, float offsetY)
		{
			// ---------------------------------------
			// fix y or height
			if (tooltipRect.height > Screen.height)
			{
				// shrink the height so it fits the available space
				tooltipRect.height = Screen.height;
#if UNITY_EDITOR
				tooltipRect.height -=
					Utility.UNITY_TOP_BAR_HEIGHT; // GameView/SceneView has a top bar so we need to subtract that
#endif
				tooltipRect.y = 0;
			}
			else
			{
				// tooltip height can fit inside the screen

				// if y position is too much above, move it down
				if (tooltipRect.y < _scrollViewRect.y)
				{
					tooltipRect.y = _scrollViewRect.y + offsetY;
				}
				// if y position is too much below, move it up
				else if (tooltipRect.yMax > _scrollViewRect.yMax)
				{
					tooltipRect.y = _scrollViewRect.yMax - tooltipRect.height + offsetY;
				}
			}

			// ---------------------------------------
			// fix x or width
			if (tooltipRect.xMax > Screen.width)
			{
				var spaceToTheLeft = _scrollViewRect.x + offsetX - DEFAULT_IMAGE_TOOLTIP_PADDING_H;
				var spaceToTheRight = Screen.width - (_scrollViewRect.xMax + offsetX) - DEFAULT_IMAGE_TOOLTIP_PADDING_H;

				// can the tooltip be placed to the left instead?
				if (spaceToTheLeft > spaceToTheRight && spaceToTheLeft > 0 && spaceToTheLeft >= tooltipRect.width)
				{
					// more space to the left, and it will fit the
					// tooltip without requiring to shrink the width,
					// so use that instead
					tooltipRect.x = _scrollViewRect.x + 1 - tooltipRect.width + offsetX;
				}
				else
				{
					// available space to the left is too small

					// can we just move the tooltip inside the scroll view?
					var labelSize = _imageItemStyle.CalcSize(_filesWithImages[_hoveredFileIdx]);
					if (_scrollViewRect.width - labelSize.x - SCROLLBAR_THICKNESS >= tooltipRect.width)
					{
						// there is space inside the scroll view
						// and the label will not get obscured
						tooltipRect.x = _scrollViewRect.xMax - SCROLLBAR_THICKNESS - tooltipRect.width + offsetX;
					}
					else
					{
						// tooltip is too large to fit inside the scroll view

						// our last resort is to shrink the width so it fits the available space
						if (spaceToTheLeft > spaceToTheRight)
						{
							// bigger space available to the left
							tooltipRect.width = spaceToTheLeft + DEFAULT_IMAGE_TOOLTIP_PADDING_V;

							tooltipRect.x = _scrollViewRect.x + 1 - tooltipRect.width + offsetX;
						}
						else
						{
							// bigger space available to the right
							tooltipRect.width = spaceToTheRight + DEFAULT_IMAGE_TOOLTIP_PADDING_V;
#if UNITY_EDITOR
							tooltipRect.width -= 3; // GameView/SceneView has a border so we need to subtract that
#endif
						}
					}
				}
			}
			// ---------------------------------------
		}

		const int DEFAULT_IMAGE_TOOLTIP_PADDING_LEFT = 1;
		const int DEFAULT_IMAGE_TOOLTIP_PADDING_TOP = 1;
		const int DEFAULT_IMAGE_TOOLTIP_PADDING_H = 2;
		const int DEFAULT_IMAGE_TOOLTIP_PADDING_V = 2;

		const int DEFAULT_IMAGE_TOOLTIP_WIDTH = 250;
		const int DEFAULT_IMAGE_TOOLTIP_HEIGHT = 250;

		bool DoesFileHaveImageThumbnail(int fileIdx)
		{
			return fileIdx > -1 &&
			       fileIdx < _filesWithImages.Length &&
			       _filesWithImages[fileIdx].image != _matchedFileImage;
		}

		bool DoesFileNotHaveImageThumbnail(int fileIdx)
		{
			return fileIdx > -1 &&
			       fileIdx < _filesWithImages.Length &&
			       _filesWithImages[fileIdx].image == _matchedFileImage;
		}

		/// <summary>
		/// Gotten rect for the entire scroll view of the files & folders list.
		/// </summary>
		Rect _scrollViewRect;

		/// <summary>
		/// If hovered file has a prefab preview image or not.
		/// </summary>
		HasPreviewStatus _hoveredHasPrefabPreview;

		enum HasPreviewStatus
		{
			None,
			PreviewStillLoading,
			HasPreview
		}

		/// <summary>
		/// Preview image of prefab that's currently shown.
		/// </summary>
		Texture _prefabPreview;

		/// <summary>
		/// Path to prefab whose preview is currently shown.
		/// </summary>
		string _modelPrefabPreviewPath;

		Rect _tooltipRect;

		// ==================================================================================

		// ===================================================

		public void Update(float deltaTime
#if UNITY_EDITOR
			, ref bool requestRepaint
#endif
		)
		{
			UpdateSoundPreview(
#if UNITY_EDITOR
				ref requestRepaint
#endif
			);

			if (_checkLoadImagesFrameCountdown > 0)
			{
				_checkLoadImagesFrameCountdown -= deltaTime;
				if (_checkLoadImagesFrameCountdown <= 0)
				{
					CheckLoadImages();
				}
			}
		}


		Vector2 GetJumpToListSize()
		{
			var resultingSize = Vector2.zero;

			var headerStyle = GUI.skin.GetStyle(JUMP_TO_LIST_HEADER_STYLE);
			var entryStyle = GUI.skin.GetStyle(JUMP_TO_LIST_ENTRY_STYLE);

			if (_recentlyUsed.Count > 0)
			{
				CollectSize(headerStyle, _jumpToListRecentlyUsedHeader, ref resultingSize);

				for (int n = 0; n < _recentlyUsed.Count; ++n)
				{
					CollectSize(entryStyle, _recentlyUsed[n], ref resultingSize);
					if (n > 0)
					{
						resultingSize.y += entryStyle.margin.bottom;
					}
				}

				resultingSize.y += 10; // spacing
			}

			CollectSize(entryStyle, _thisProjectStreamingAssetsLabel, ref resultingSize);
			CollectSize(entryStyle, _thisProjectAssetsLocationLabel, ref resultingSize);
			CollectSize(entryStyle, _gameCommonDataLabel, ref resultingSize);
			CollectSize(entryStyle, _rootLocationLabel, ref resultingSize);
			CollectSize(entryStyle, _desktopLocationLabel, ref resultingSize);
			CollectSize(entryStyle, _myDocsLocationLabel, ref resultingSize);
			CollectAdditionalPathSizes(entryStyle, ref resultingSize);

			// ----------------------------

			var bgStyle = GUI.skin.GetStyle(JUMP_TO_LIST_BG_STYLE);

			resultingSize.x += bgStyle.padding.horizontal;
			resultingSize.y += bgStyle.padding.vertical;

			return resultingSize;
		}

		protected static void CollectSize(GUIStyle entryStyle, GUIContent content, ref Vector2 resultingSize)
		{
			var entrySize = entryStyle.CalcSize(content);
			resultingSize.y += entrySize.y + entryStyle.margin.bottom;
			resultingSize.x = Mathf.Max(resultingSize.x, entrySize.x);
		}

		const string JUMP_TO_LIST_BG_STYLE = "FileDlg.JumpToList";
		const string JUMP_TO_LIST_HEADER_STYLE = "FileDlg.JumpToList.Header";
		protected const string JUMP_TO_LIST_ENTRY_STYLE = "ListItem";

		readonly GUIContent _jumpToListRecentlyUsedHeader = new GUIContent("Recent");

		const string DEFAULT_GAME_COMMON_DATA = "C:/ProgramData/Dreamlords Digital/GwP/";

		void DrawJumpToList(float offsetX = 0)
		{
			if (_showJumpToList)
			{
				//UnityEngine.GUI.Box(_jumpToListRect, "");

				var rect = _jumpToListRect;
				rect.x += offsetX;

				GUILayout.BeginArea(rect, string.Empty, JUMP_TO_LIST_BG_STYLE);
				GUILayout.BeginVertical();

				if (_recentlyUsed.Count > 0)
				{
					GUILayout.Label(_jumpToListRecentlyUsedHeader, JUMP_TO_LIST_HEADER_STYLE);
					for (int n = 0; n < _recentlyUsed.Count; ++n)
					{
						if (GUILayout.Button(_recentlyUsed[n], JUMP_TO_LIST_ENTRY_STYLE))
						{
							SwitchCurrentPath(_recentlyUsed[n].tooltip);
							CloseJumpToList();
						}
					}

					GUILayout.Space(10);
				}

				//GUILayout.Label("Special");
				if (GUILayout.Button(_thisProjectAssetsLocationLabel, JUMP_TO_LIST_ENTRY_STYLE))
				{
					SwitchCurrentPath(Application.dataPath);
					CloseJumpToList();
				}

				if (GUILayout.Button(_thisProjectStreamingAssetsLabel, JUMP_TO_LIST_ENTRY_STYLE))
				{
					SwitchCurrentPath(Application.streamingAssetsPath);
					CloseJumpToList();
				}

				DrawAdditionalProjectPaths();
				if (GUILayout.Button(_gameCommonDataLabel, JUMP_TO_LIST_ENTRY_STYLE))
				{
					SwitchCurrentPath(DEFAULT_GAME_COMMON_DATA);
					CloseJumpToList();
				}

				if (GUILayout.Button(_rootLocationLabel, JUMP_TO_LIST_ENTRY_STYLE))
				{
					MoveToRootPath();
					CloseJumpToList();
				}

				if (GUILayout.Button(_desktopLocationLabel, JUMP_TO_LIST_ENTRY_STYLE))
				{
					SwitchCurrentPath(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
					CloseJumpToList();
				}

				if (GUILayout.Button(_myDocsLocationLabel, JUMP_TO_LIST_ENTRY_STYLE))
				{
					SwitchCurrentPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
					CloseJumpToList();
				}

				GUILayout.EndVertical();
				GUILayout.EndArea();

				// --------------------------------------

				GUI.Button(rect, string.Empty, "PopupBoxHidden");

				if (!rect.Contains(Event.current.mousePosition) &&
				    (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp))
				{
					Utility.ForceGuiPassive();
					Utility.EatEvent();
					CloseJumpToList();
				}
				/*else if (!_jumpToListRect.Contains(Event.current.mousePosition) && Event.current.isMouse)
				{
					Utility.EatEvent();
				}*/
			}
		}

		protected void CloseJumpToList()
		{
			_showJumpToList = false;
		}

		void DrawToolbar()
		{
			GUILayout.BeginHorizontal(GUILayout.Height(63));

			const string TOOLBAR_BUTTON_STYLE = "ToolbarMiddle";

			GUI.enabled = CanMovePathHistoryBackward;
			if (GUILayout.Button(_backButtonLabel, TOOLBAR_BUTTON_STYLE))
			{
				MovePathHistoryBackward();
			}

			GUI.enabled = CanMovePathHistoryForward;
			if (GUILayout.Button(_forwardButtonLabel, TOOLBAR_BUTTON_STYLE))
			{
				MovePathHistoryForward();
			}

			GUI.enabled = true;


			var pressedJumpToButton = GUILayout.Button(_jumpButtonLabel, TOOLBAR_BUTTON_STYLE);
			if (Event.current.type == EventType.Repaint)
			{
				_jumpToButtonRect = GUILayoutUtility.GetLastRect();

				_jumpToListRect.x = _jumpToButtonRect.x;
				_jumpToListRect.y = _jumpToButtonRect.y + _jumpToButtonRect.height;

				var jumpToListSize = GetJumpToListSize();
				_jumpToListRect.width = jumpToListSize.x;
				_jumpToListRect.height = jumpToListSize.y;
			}

			if (pressedJumpToButton)
			{
				_showJumpToList = !_showJumpToList;
			}

			if (!HasLockPath && GUILayout.Button(_editPathButtonLabel, TOOLBAR_BUTTON_STYLE))
			{
				if (_showPathAsButtons)
				{
					ShowPathAsTextField();
				}
				else
				{
					// currently already in path edit mode
					ApplyEditedPath();
				}
			}

			if (GUILayout.Button(_openInExplorerButtonLabel, TOOLBAR_BUTTON_STYLE))
			{
				ExplorerUtil.OpenInFileBrowser(_currentPath);
			}

			GUILayout.EndHorizontal();
		}

		string _editingCurrentPath;

		void ApplyEditedPath()
		{
			SwitchCurrentPath(_editingCurrentPath);
			ShowPathAsButtons();
		}

		void CancelEditedPath()
		{
			_editingCurrentPath = _currentPath;
			ShowPathAsButtons();
		}

		void PingSelectedFile(int selectedFileIdx)
		{
#if UNITY_EDITOR
			if (selectedFileIdx < 0)
			{
				// no file selected
				return;
			}

			if (selectedFileIdx >= _files.Length)
			{
				return;
			}

			if (!_currentPath.Contains(Application.dataPath))
			{
				// current path is not in the project folder
				Debug.Log(
					$"Will not ping because current path is not in this project: {_currentPath} (project path: {Application.dataPath})");
				return;
			}

			var selectedFilePath = $"{_currentPath}/{_files[selectedFileIdx]}";
			var assetsIdx = selectedFilePath.IndexOf("/Assets/", StringComparison.OrdinalIgnoreCase);
			if (assetsIdx > -1)
			{
				selectedFilePath = selectedFilePath.Substring(assetsIdx + 1);
				Debug.Log($"Will ping: {selectedFilePath}");
				var loadedPrefab = AssetDatabase.LoadAssetAtPath<Object>(selectedFilePath);
				if (loadedPrefab != null)
				{
					EditorGUIUtility.PingObject(loadedPrefab);
				}
			}
#endif
		}

		void DrawCurrentPath()
		{
			if (IsPathShownAsTextField && !HasLockPath)
			{
				_editingCurrentPath = GUILayout.TextField(_editingCurrentPath);

				bool enterWasPressed = Event.current.isKey && Event.current.keyCode == KeyCode.Return;
				bool escapeWasPressed = Event.current.isKey && Event.current.keyCode == KeyCode.Escape;

				if (enterWasPressed)
				{
					ApplyEditedPath();
					Utility.EatEvent();
				}
				else if (escapeWasPressed)
				{
					CancelEditedPath();
					Utility.EatEvent();
				}
			}
			else
			{
				if (_pathRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ScrollWheel)
				{
					_directoryScrollPosition.x += Event.current.delta.y * 7;
				}


				float forcedHeight = GUI.skin.GetStyle("PathLayout").fixedHeight;
				float addedHeight = GUI.skin.GetStyle("PathLayout").contentOffset.y;

				_directoryScrollPosition.y = 0;
				_directoryScrollPosition = GUILayout.BeginScrollView(_directoryScrollPosition,
					true,
					false,
					"TinyScrollbar.Horizontal",
					"HiddenScrollbar",
					"HiddenScrollbar",
					GUILayout.MinHeight(forcedHeight + addedHeight),
					GUILayout.MaxHeight(forcedHeight + addedHeight),
					GUILayout.ExpandWidth(false),
					GUILayout.ExpandHeight(false));

				GUILayout.BeginHorizontal(GUILayout.MinHeight(forcedHeight),
					GUILayout.MaxHeight(forcedHeight),
					GUILayout.ExpandWidth(false),
					GUILayout.ExpandHeight(false));

				int startOfFolderIdx = HasLockPath ? 0 : -1;
				for (int folderIdx = startOfFolderIdx; folderIdx < _currentPathParts.Length; ++folderIdx)
				{
					if (!HasLockPath && folderIdx == -1)
					{
						var folderRootStyleToUse = _rootLocationLabel.image != null
							? _folderRootWithIconButtonStyle
							: _folderRootButtonStyle;
						if (GUILayout.Button(_rootLocationLabel, folderRootStyleToUse))
						{
							MoveToRootPath();
						}

						continue;
					}

					if (string.IsNullOrEmpty(_currentPathParts[folderIdx]))
					{
						continue;
					}

					if (HasLockPath && folderIdx == 0)
					{
						var folderRootStyleToUse = _currentPathPartsDisplayed[0].image != null
							? _folderRootWithIconButtonStyle
							: _folderRootButtonStyle;
						if (GUILayout.Button(_currentPathPartsDisplayed[0], folderRootStyleToUse))
						{
							var folderToMoveTo = FileUtil.MoveUpPath(folderIdx, _currentPath, _currentPathParts.Length);
							SwitchCurrentPath(folderToMoveTo);
						}

						continue;
					}

					var folderStyleToUse = _currentPathPartsDisplayed[folderIdx].image != null
						? _folderWithIconButtonStyle
						: _folderButtonStyle;

					if (folderIdx == _currentPathParts.Length - 1)
					{
						// final folder in path
						if (GUILayout.Button(_currentPathPartsDisplayed[folderIdx], folderStyleToUse))
						{
						}
					}
					else if (GUILayout.Button(_currentPathPartsDisplayed[folderIdx], folderStyleToUse))
					{
						var numOfFolders = string.IsNullOrEmpty(_currentVirtualFolder)
							? _currentPathParts.Length
							: _currentPathParts.Length -
							  1; // _currentPathParts includes the virtual folder, which shouldn't be counted here, so we -1
						var folderToMoveTo = FileUtil.MoveUpPath(folderIdx, _currentPath, numOfFolders);

						Debug.Log(
							$"Clicked path button, going to idx: {folderIdx} num: {_currentPathParts.Length}\n{folderToMoveTo}");

						SwitchCurrentPath(folderToMoveTo);
					}
				}

				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.EndScrollView();
				if (Event.current.type == EventType.Repaint)
				{
					_pathRect = GUILayoutUtility.GetLastRect();
				}
			}
		}

		Rect _pathRect;

		static readonly GUILayoutOption[] ButtonLayout = { GUILayout.Width(70), GUILayout.MinHeight(25) };

		void DrawOkCancelButtons()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

#if UNITY_EDITOR
			// Note: has to be a file in our project, but can't do if in a hidden folder
			GUI.enabled = _selectedFileIdx > -1 &&
			              _currentPath.Contains(Application.dataPath) &&
			              !_currentPath.Contains("/StreamingAssets/.GameData/");
			if (GUILayout.Button("Ping", ButtonLayout))
			{
				PingSelectedFile(_selectedFileIdx);
			}
#endif

			// determine whether user can press OK/Save
			if (IsBrowserTypeForFiles)
			{
				GUI.enabled = _selectedFileIdx > -1;
			}
			else
			{
				// folder type
				GUI.enabled = _selectedFolderIdx > -1;
			}

			if (_operationType == OperationType.Save)
			{
				GUI.enabled = !string.IsNullOrEmpty(_saveFilename);
			}

			// open/save button
			if (GUILayout.Button(OperationConfirmLabel, ButtonLayout))
			{
				string finalFileValue;
				if (_operationType == OperationType.Save)
				{
					// saving a file
					finalFileValue = CreateFinalFileValue(_currentPath, _currentVirtualFolder,
						_currentVirtualPath, _saveFilename);

					CallConfirmCallback(finalFileValue);
				}
				else
				{
					// opening a file
					if (IsBrowserTypeForFiles)
					{
						FileDoubleClickCallback(_selectedFileIdx, _files[_selectedFileIdx]);
					}
					else
					{
						// browsing folders

						if (_selectedFolderIdx > -1)
						{
							finalFileValue = CreateFinalFileValue(_currentPath, _currentVirtualFolder,
								_currentVirtualPath, _folders[_selectedFolderIdx]);
						}
						else
						{
							finalFileValue = _currentPath;
						}

						CallConfirmCallback(finalFileValue);
					}
				}
			}

			GUI.enabled = true;

			// cancel button
			if (GUILayout.Button("Cancel", ButtonLayout))
			{
				if (_showPathAsButtons)
				{
					CallCancelCallback(_currentPath);
				}
				else
				{
					CancelEditedPath();
				}
			}

			GUILayout.EndHorizontal();
		}


		/// <summary>
		/// In Windows, this will cause the File Browser to load a list of the computer's drives.
		/// In Mac OS or Linux, this will move the File Browser to the root.
		/// </summary>
		public void MoveToRootPath()
		{
			// note: old path is only recorded for debugging purposes
			string oldPath = _currentPath;

			_currentPath = "/";
			AddPathHistory("/");
			ReadDirectoryContents(_currentPath, null, null, oldPath);
		}

		static bool IsPathRoot(string path)
		{
			return path == "/";
		}

		public bool IsAtRootPath
		{
			get
			{
				return IsPathRoot(_currentPath);
			}
		}

		string OperationConfirmLabel
		{
			get
			{
				if (_operationType == OperationType.Save)
				{
					return "Save";
				}

				return "Open";
			}
		}

		string _saveFilename = "";

		// ------------------------------------------------------------------------------------------------

		/// <summary>
		/// If the browser is going to open an animation, is the virtual folder specified
		/// actually an fbx file that we can open, to view the animations inside?
		/// </summary>
		/// <param name="realPath"></param>
		/// <param name="virtualFolder"></param>
		/// <returns></returns>
		bool IsAnimationVirtualFolderAnFbxFile(string realPath, string virtualFolder)
		{
			return IsBrowserTypeForAnimations &&
			       _operationType == OperationType.Open &&
			       realPath.Contains(Application.dataPath, StringComparison.OrdinalIgnoreCase) &&
			       !string.IsNullOrEmpty(virtualFolder) &&
			       virtualFolder.EndsWith(FileUtil.FBX_FILE_EXTENSION,
				       StringComparison.OrdinalIgnoreCase);
		}

		bool IsPathInResourcesOfProject(string path)
		{
			// note: this only works in editor time because Application.dataPath doesn't have the project path in runtime
			return path.Contains(Application.dataPath, StringComparison.OrdinalIgnoreCase) &&
			       Application.isEditor &&
			       path.Contains(FileUtil.RESOURCES_FOLDER, StringComparison.OrdinalIgnoreCase);
		}

		protected void FileDoubleClickCallback(int idxOfSelectedItem, string label)
		{
			if (IsBrowserTypeForFiles)
			{
				if (IsAnimationVirtualFolderAnFbxFile(_currentPath, _files[idxOfSelectedItem]))
				{
					// double-clicked an fbx
					// treat fbx files as folders when browsing animations

					// if the fbx only has 1 animation inside, then don't bother going inside it,
					// just select that 1 animation already

					if (_currentPath.Contains(FileUtil.RESOURCES_FOLDER, StringComparison.OrdinalIgnoreCase))
					{
						// fbx to "open" is in a Resources folder

						string resourcesPathOfFbxFile =
							FileUtil.GetResourcesPathOfFile(Path.Combine(_currentPath, _files[idxOfSelectedItem]));

						var anims = Resources.LoadAll<AnimationClip>(resourcesPathOfFbxFile);

						if (anims.Length == 1)
						{
							// only 1 animation in this fbx file
							// instead of bothering to go inside the fbx file,
							// just go ahead and use that 1 animation

							string finalFileValue = CreateFinalFileValue(_currentPath, _files[idxOfSelectedItem],
								null, anims[0].name);

							Debug.Log($"Confirm Final Value: {finalFileValue}");
							CallConfirmCallback(finalFileValue);
						}
						else if (anims.Length > 1)
						{
							Debug.LogFormat("going inside fbx file: {0}", label);

							SwitchCurrentPath(FileUtil.NormalizedCombinePath(_currentPath, _files[idxOfSelectedItem]));
						}
						// else: anims.Length == 0. no animations in this fbx file. don't do anything then.
					}
					else
					{
						// fbx to "open" is not in a Resources folder

#if UNITY_EDITOR
						var fbxAbsPath = Path.Combine(_currentPath, _files[idxOfSelectedItem]);
						var fbxAssetsPath = FileUtil.GetPathRelativeToAssets(fbxAbsPath);

						Object[] subAssets =
							AssetDatabase.LoadAllAssetRepresentationsAtPath(fbxAssetsPath);
#else
						UnityEngine.Object[] subAssets = null;
#endif
						if (subAssets == null || subAssets.Length == 0)
						{
							Debug.LogWarning(
								$"FileBrowser.SwitchCurrentPath(): no sub-assets inside of \"{_files[idxOfSelectedItem]}\"");
							return;
						}

						var animCount = 0;
						string firstAnimName = null;
						for (int subIdx = 0; subIdx < subAssets.Length; ++subIdx)
						{
							if (subAssets[subIdx] is AnimationClip)
							{
								if (animCount == 0)
								{
									firstAnimName = subAssets[subIdx].name;
								}

								++animCount;
							}
						}

						if (animCount == 1)
						{
							// only 1 animation in this fbx file
							// instead of bothering to go inside the fbx file,
							// just go ahead and use that 1 animation

							var finalFileValue = CreateFinalFileValue(_currentPath, _files[idxOfSelectedItem],
								null, firstAnimName);

							Debug.Log($"Confirm Final Value: {finalFileValue}");
							CallConfirmCallback(finalFileValue);
						}
						else if (animCount > 1)
						{
							Debug.LogFormat("going inside fbx file: {0}", label);

							SwitchCurrentPath(FileUtil.NormalizedCombinePath(_currentPath, _files[idxOfSelectedItem]));
						}
						// else: animCount == 0. no animations in this fbx file. don't do anything then.
					}
				}
				else
				{
					var finalFileValue = CreateFinalFileValue(_currentPath, _currentVirtualFolder,
						_currentVirtualPath, _files[idxOfSelectedItem]);

					Debug.Log($"Confirm Final Value: {finalFileValue}");
					CallConfirmCallback(finalFileValue);
				}
			}
		}

		static string CreateFinalFileValue(string realPath, string virtualFolder, string virtualPath, string file)
		{
			string finalFileValue;
			if (!string.IsNullOrEmpty(virtualFolder))
			{
				finalFileValue = FileUtil.NormalizedCombinePath(realPath, virtualFolder);

				if (!string.IsNullOrEmpty(virtualPath))
				{
					finalFileValue = $"{finalFileValue}:{FileUtil.NormalizedCombinePath(virtualPath, file)}";
				}
				else
				{
					finalFileValue = $"{finalFileValue}:{file}";
				}
			}
			else
			{
				finalFileValue = FileUtil.NormalizedCombinePath(realPath, file);
			}

			return finalFileValue;
		}

		protected void FileSelectCallback(int idxOfSelectedItem, string label)
		{
			//Debug.LogFormat("selected {0}", label);

			if (_operationType == OperationType.Save)
			{
				_saveFilename = _files[idxOfSelectedItem];
			}

			//if (IsBrowserTypeForImages && _operationType == OperationType.Open &&
			//	!_fileThumbnails.ContainsKey(label))
			//{
			//	LoadAndRegisterImage(FileUtility.CombinePath(_currentPath, _files[idxOfSelectedItem]));
			//	ShowThumbnailsForImageItems();
			//}

			if (Event.current.button == 2)
			{
				PingSelectedFile(idxOfSelectedItem);
			}
		}

		protected void DirectoryDoubleClickCallback(int i, string label)
		{
			MoveToPathRelative(_folders[i]);
		}

		// ------------------------------------------------------------------------------------------------

		const string UP_FOLDER_LABEL = ".. <b>Up to parent folder</b>";

		void MoveToPathRelative(string subfolderToGoTo)
		{
			if (subfolderToGoTo == UP_FOLDER_LABEL)
			{
				SwitchToParentFolderOfCurrentPath();
			}
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
			else if (FileUtil.IsPathFormattedDriveName(subfolderToGoTo))
			{
				// this isn't really a sub-folder, but a drive letter
				// get drive letter only
				var driveLetterToGoTo = FileUtil.GetDriveLetterOutOfFormattedDriveName(subfolderToGoTo);
				SwitchCurrentPath(driveLetterToGoTo);
			}
#endif
			else
			{
				//Debug.LogFormat("folder to go to: {0}", subfolderToGoTo);
				SwitchCurrentPath(FileUtil.NormalizedCombinePath(_currentPath, subfolderToGoTo));
			}
		}

		string GetCurrentPathFolder()
		{
			return _currentPathParts[_currentPathParts.Length - 1];
		}

		// ---------------------------------------------------------

		/// <summary>
		/// If present, this delegate is used to get virtual files.
		/// </summary>
		Func<string, string[]> _getVirtualFilesCallback;

		bool HasVirtualFilesCallback
		{
			get
			{
				return _getVirtualFilesCallback != null;
			}
		}

		public void SetVirtualFilesCallback(Func<string, string[]> callback)
		{
			_getVirtualFilesCallback = callback;
		}

		// ---------------------------------------------------------

		/// <summary>
		/// If present, this delegate is used to edit a GUIContent with an appropriate label & icon for each matching file.
		/// </summary>
		Action<GUIContent, string> _createAssetItemCallback;

		bool HasCreateAssetItemCallback
		{
			get
			{
				return _createAssetItemCallback != null;
			}
		}

		public void SetCreateAssetItemCallback(Action<GUIContent, string> callback)
		{
			_createAssetItemCallback = callback;
		}

		// ---------------------------------------------------------

		/// <summary>
		/// An absolute path. If it is given a value, code will not allow user to go up beyond the lock path.
		/// </summary>
		string _lockPath;

		public void SetLockedPath(string newLockPath)
		{
			_lockPath = newLockPath;
		}

		bool AlreadyAtLockPathIfApplicable
		{
			get
			{
				return HasLockPath && FileUtil.ArePathsSame(_currentPath, _lockPath);
			}
		}

		/// <summary>
		/// If browser is set to disallow user from going up beyond a certain path.
		/// </summary>
		bool HasLockPath
		{
			get
			{
				return !string.IsNullOrEmpty(_lockPath);
			}
		}

		string[] CreateLockPathParts(string currentPath)
		{
			List<string> pathParts = new List<string>(currentPath.Split(FolderDelimiter));
			var lockPathParts = _lockPath.Split(FolderDelimiter);

			for (int n = 0, len = lockPathParts.Length - 2; n < len; ++n)
			{
				pathParts.RemoveAt(0);
			}

			return pathParts.ToArray();
		}

		// ----------------------------------------------

		protected virtual void InitializeAdditionalImages(GUISkin guiSkin)
		{
		}

		protected virtual void CollectAdditionalPathSizes(GUIStyle entryStyle, ref Vector2 resultingSize)
		{
		}

		protected virtual void DrawAdditionalProjectPaths()
		{
		}
	}

	// ===================================================================================
}