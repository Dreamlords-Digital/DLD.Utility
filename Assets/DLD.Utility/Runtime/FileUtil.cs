// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace DLD.Utility
{
	public static class FileUtil
	{
		public const string RESOURCES_FOLDER = "/Resources/";
		public const int ENTERED_FBX_FILE_LEN = 5;
		public const int FBX_FILE_EXTENSION_LEN = 4;

		public const string FBX_FILE_EXTENSION = ".fbx";
		public const string ENTERED_FBX_FILE = ".fbx:";
		public const string ANIM_FILE_EXTENSION = ".anim";
		public const string PREFAB_FILE_EXTENSION = ".prefab";
		public const string ASSET_BUNDLE_FILE_EXTENSION = ".asset";
		public const string ENTERED_ASSET_BUNDLE_FILE = ".asset:";

		public static long GetFileSizeInBytes(string filename)
		{
			if (string.IsNullOrEmpty(filename) || !System.IO.File.Exists(filename))
			{
				return 0;
			}

			System.IO.FileInfo fi = new System.IO.FileInfo(filename);
			return fi.Length;
		}

		public static string GetBytesReadable(long bytes)
		{
			//return UnityEditor.EditorUtility.FormatBytes(bytes);
			return MyFileSizeReadable(bytes);
		}

		const double ONE_TERABYTE = 1099511627776.0;
		const double ONE_GIGABYTE = 1073741824.0;
		const double ONE_MEGABYTE = 1048576.0;
		const double ONE_KILOBYTE = 1024.0;

		static string MyFileSizeReadable(long bytes)
		{
			if (bytes < 0)
			{
				return "N/A";
			}

			double converted = bytes;
			string units = "B";

			if (bytes >= ONE_TERABYTE)
			{
				converted = bytes / ONE_TERABYTE;
				units = "TB";
			}
			else if (bytes >= ONE_GIGABYTE)
			{
				converted = bytes / ONE_GIGABYTE;
				units = "GB";
			}
			else if (bytes >= ONE_MEGABYTE)
			{
				converted = bytes / ONE_MEGABYTE;
				units = "MB";
			}
			else if (bytes >= ONE_KILOBYTE)
			{
				converted = bytes / ONE_KILOBYTE;
				units = "KB";
			}

			return $"{converted.ToString("0.##", CultureInfo.InvariantCulture)} {units}";
		}

		static readonly byte[] FileAllNullBuffer = new byte[1];
		public static bool IsFileAllNull(string filePath)
		{
			bool fileIsAllNull = true;

			using Stream source = File.OpenRead(filePath);
			while (source.Read(FileAllNullBuffer, 0, 1) > 0)
			{
				if (FileAllNullBuffer[0] != 0)
				{
					fileIsAllNull = false;
					break;
				}
			}

			return fileIsAllNull;
		}

		public static bool IsFileOfType(this string filepath, string typeExtenstion)
		{
			if (string.IsNullOrEmpty(filepath))
			{
				return false;
			}

			return filepath.EndsWith(typeExtenstion, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// This gets the name of the deepest subfolder in the specified path.
		/// </summary>
		/// <param name="inFolder"></param>
		/// <returns></returns>
		public static string GetLastFolder(string inFolder)
		{
			inFolder = inFolder.Replace('\\', '/');

			//Debug.Log("folder: " + inFolder);
			//string folderName = Path.GetDirectoryName(folderEntries[n]);

			int lastSlashIdx = -1;
			var lastCharIsSlash = inFolder[inFolder.Length - 1] == '/';
			// if the final character in the path is a slash, skip that one
			if (lastCharIsSlash)
			{
				lastSlashIdx = inFolder.LastIndexOf('/', inFolder.Length-2, inFolder.Length-1);
			}
			else
			{
				lastSlashIdx = inFolder.LastIndexOf('/');
			}

			if (lastSlashIdx == -1)
			{
				return "";
			}

			if (lastCharIsSlash)
			{
				// do not include that last slash
				return inFolder.Substring(lastSlashIdx + 1, inFolder.Length - 1 - lastSlashIdx - 1);
			}

			return inFolder.Substring(lastSlashIdx + 1, inFolder.Length - lastSlashIdx - 1);
		}

		/// <summary>
		/// If passed string is path to a file, this will remove the file part of the string.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetFolderPath(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				if (File.Exists(path))
				{
					return Path.GetDirectoryName(path);
				}
				else // already a path to a folder
				{
					return path;
				}
			}

			return path;
		}

		/// <summary>
		/// Removes any non-existent folders from the path. Requires an absolute path.
		/// </summary>
		/// <returns>True if the path was fixed (now points to a folder that exists). False if not.</returns>
		public static bool FixPath(string path, out string resultingPath)
		{
			resultingPath = path.Trim();
			resultingPath = resultingPath.ConvertBackToForwardSlash();

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			// remove leading slash in windows
			if (resultingPath.StartsWith("/"))
			{
				resultingPath = resultingPath.Substring(1, resultingPath.Length - 1);
			}
#endif

			//Debug.LogFormat("<b>going to:</b> {0}", pathToSwitchTo);

			int endlessLoopGuard = 0;

			if (resultingPath.Contains('/'))
			{
				while (!Directory.Exists(resultingPath) && !string.IsNullOrEmpty(resultingPath))
				{
					++endlessLoopGuard;
					if (endlessLoopGuard >= 100)
					{
						//Debug.LogErrorFormat("endless loop on getting path {0}", pathToSwitchTo);
						break;
					}

					// if path doesn't exist anymore, try its parent folder
					resultingPath = Path.GetDirectoryName(resultingPath);
				}
			}
			//Debug.LogFormat("Path.GetDirectoryName {0}", Path.GetDirectoryName(pathToSwitchTo));

			return IsPathAccessible(resultingPath);
		}

		public static bool IsPathAccessible(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}

			try
			{
				Directory.GetDirectories(path);
			}
			catch (IOException)
			{
				return false;
			}

			return true;
		}

		public static bool IsRootPath(string path)
		{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			return (path.Length == 3) &&
			       (path[1] == ':') &&
			       (path[2] == '/' || path[2] == '\\');

#elif UNITY_STANDALONE
		// todo need to test IsRootPath() on mac and linux
		return path == "/";
#endif
		}

		/// <summary>
		/// Compare two paths if they are the same.
		/// Backslash and forward slashes are considered equivalent.
		/// Trailing slashes on either path are ignored.
		/// </summary>
		/// <param name="pathA"></param>
		/// <param name="pathB"></param>
		/// <param name="comparison"></param>
		/// <returns></returns>
		public static bool ArePathsSame(string pathA, string pathB, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		{
			pathA = pathA.ConvertBackToForwardSlash();
			pathB = pathB.ConvertBackToForwardSlash();

			if (pathA.EndsWith('/'))
			{
				pathA = pathA[..^1];
			}
			if (pathB.EndsWith('/'))
			{
				pathB = pathB[..^1];
			}

			return string.Equals(pathA, pathB, comparison);
		}

		public static bool IsUselessFile(string filepath, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
		{
			return filepath.EndsWith(".meta", comparisonType) ||
			       filepath.EndsWith("Thumbs.db", comparisonType) ||
			       filepath.EndsWith(".DS_Store", comparisonType);
		}

		public static bool IsSystemFolder(string folderName)
		{
			return folderName == "System Volume Information" ||
			       folderName == "$RECYCLE.BIN" ||
			       folderName == "$Recycle.Bin" ||
			       folderName == "TheVolumeSettingsFolder";
		}

		/// <summary>
		/// Like <see cref="Path.Combine(string, string)"/>, except this always uses forward slashes.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="pathToAppend"></param>
		/// <returns></returns>
		public static string NormalizedCombinePath(string path, string pathToAppend)
		{
			path = path.ConvertBackToForwardSlash();

			if (path.EndsWith('/'))
			{
				return $"{path}{pathToAppend.ConvertBackToForwardSlash()}";
			}

			return $"{path}/{pathToAppend.ConvertBackToForwardSlash()}";
		}

		/// <summary>
		/// <para>Changes a string so that it can be used in Resources.Load().</para>
		/// For example, it will change
		/// "Assets/Resources/SomePackage/My.Path/Something.prefab" to:
		/// "SomePackage/My.Path/Something"
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetResourcesPathOfFile(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}

			// -----------------------------------------

			const string RESOURCES_FOLDER_NAME = "/Resources/";
			const int RESOURCES_FOLDER_NAME_LENGTH = 11;

			var lastResources = path.LastIndexOf(RESOURCES_FOLDER_NAME, StringComparison.OrdinalIgnoreCase);

			string result;
			if (lastResources >= 0)
			{
				result = path.Substring(lastResources + RESOURCES_FOLDER_NAME_LENGTH);
			}
			else
			{
				result = path;
			}

			// -----------------------------------------

			var hasEnteredFbxFile = result.Contains(ENTERED_FBX_FILE, StringComparison.OrdinalIgnoreCase);
			var hasEnteredAssetBundleFile = result.Contains(ENTERED_ASSET_BUNDLE_FILE, StringComparison.OrdinalIgnoreCase);

			if (!hasEnteredFbxFile && !hasEnteredAssetBundleFile)
			{
				// remove the dot and the file type extension. this is needed because Resources.Load() expects that.
				var lastDot = result.LastIndexOf('.');

				// prevent removing dot from folders: ex: "SomePackage/My.Path/Asset"
				var lastSlash = result.LastIndexOf('/');

				if (lastDot >= 0 && lastDot > lastSlash)
				{
					result = result.Substring(0, lastDot);
				}
			}

			return result;
		}

		/// <summary>
		/// Given a full path, return only part of the path that starts with "Assets/",
		/// as in the project's top-level Assets folder.
		/// </summary>
		/// <param name="absoluteFilePath"></param>
		/// <returns></returns>
		public static string GetPathRelativeToAssets(string absoluteFilePath)
		{
			if (Application.isEditor)
			{
				var assetsIdx = Application.dataPath.Length - 6; // minus 6 so that we don't remove the "Assets"

				return absoluteFilePath.Substring(assetsIdx);
			}

			return null;
		}

		public static string MoveUpPath(int folderIdxToGoTo, string initialPath, int initialPathNumOfFolders)
		{
			string folderToMoveTo = initialPath;

			// keep going up in the path (removing last folder in path) until we reach desired folder
			for (int i = initialPathNumOfFolders - 1; i > folderIdxToGoTo; --i)
			{
				folderToMoveTo = Path.GetDirectoryName(folderToMoveTo);
			}
			return folderToMoveTo;
		}

		/// <summary>
		/// Absolute path to Project's folder (without the "/Assets" at the end).
		/// </summary>
		public static string ProjectPath
		{
			get
			{
#if UNITY_EDITOR
				string result = Application.dataPath;

				return result[..^7]; // minus 7 to remove the "/Assets"
#else
				return Application.dataPath;
#endif
			}
		}

		public static string ProjectFolderName
		{
			get
			{
				string result = Application.dataPath;
				result = result.Substring(0, result.Length - 7); // minus 6 to remove the "/Assets"
				int lastSlash = result.LastIndexOf("/", StringComparison.Ordinal);
				result = result.Substring(lastSlash+1);

				return result;
			}
		}

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
		// -------------------------------------------------------------------------------------------------
		// code from http://www.codeproject.com/Articles/22328/Getting-Drive-s-Volume-Information-using-C
		// obviously works only in windows
		//

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		static extern bool GetVolumeInformation(string volume, StringBuilder volumeName,
			uint volumeNameSize, out uint serialNumber, out uint serialNumberLength,
			out uint flags, StringBuilder fs, uint fsSize);

		public static string GetVolumeLabel(string driveLetter)
		{
			uint serialNum, serialNumLength, flags;
			var volumename = new StringBuilder(256);
			var fstype = new StringBuilder(256);

			var ok = GetVolumeInformation(driveLetter, volumename,
				(uint)volumename.Capacity - 1, out serialNum, out serialNumLength,
				out flags, fstype, (uint)fstype.Capacity - 1);

			// "FileType is" + fstype.ToString()

			return ok ? volumename.ToString() : string.Empty;
		}

		public static string GetFormattedDriveName(string driveLetter)
		{
			var volumeLabel = GetVolumeLabel(driveLetter);

			if (string.IsNullOrEmpty(volumeLabel))
			{
				return driveLetter;
			}

			return $"{driveLetter} <b>{volumeLabel}</b>";
		}

		public static bool IsPathFormattedDriveName(string path)
		{
			return path.Contains(":\\ <b>");
		}

		public static string GetDriveLetterOutOfFormattedDriveName(string label)
		{
			// drive letter is on first 3 chars, example:
			// "C:\ <b>Windows</b>"
			return label.Substring(0, 3);
		}

		// ------------------------------------------------------------------------------------------------
		// from "How to resolve a .lnk in c#" http://stackoverflow.com/a/220870
		//

		#region Signatures imported from http://pinvoke.net

		[DllImport("shfolder.dll", CharSet = CharSet.Auto)]
		internal static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);

		[Flags()]
		enum SLGP_FLAGS
		{
			/// <summary>Retrieves the standard short (8.3 format) file name</summary>
			SLGP_SHORTPATH = 0x1,
			/// <summary>Retrieves the Universal Naming Convention (UNC) path name of the file</summary>
			SLGP_UNCPRIORITY = 0x2,
			/// <summary>Retrieves the raw path name. A raw path is something that might not exist and may include environment variables that need to be expanded</summary>
			SLGP_RAWPATH = 0x4
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		struct WIN32_FIND_DATAW
		{
			public uint dwFileAttributes;
			public long ftCreationTime;
			public long ftLastAccessTime;
			public long ftLastWriteTime;
			public uint nFileSizeHigh;
			public uint nFileSizeLow;
			public uint dwReserved0;
			public uint dwReserved1;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string cFileName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
			public string cAlternateFileName;
		}

		[Flags()]
		enum SLR_FLAGS
		{
			/// <summary>
			/// Do not display a dialog box if the link cannot be resolved. When SLR_NO_UI is set,
			/// the high-order word of fFlags can be set to a time-out value that specifies the
			/// maximum amount of time to be spent resolving the link. The function returns if the
			/// link cannot be resolved within the time-out duration. If the high-order word is set
			/// to zero, the time-out duration will be set to the default value of 3,000 milliseconds
			/// (3 seconds). To specify a value, set the high word of fFlags to the desired time-out
			/// duration, in milliseconds.
			/// </summary>
			SLR_NO_UI = 0x1,
			/// <summary>Obsolete and no longer used</summary>
			SLR_ANY_MATCH = 0x2,
			/// <summary>If the link object has changed, update its path and list of identifiers.
			/// If SLR_UPDATE is set, you do not need to call IPersistFile::IsDirty to determine
			/// whether or not the link object has changed.</summary>
			SLR_UPDATE = 0x4,
			/// <summary>Do not update the link information</summary>
			SLR_NOUPDATE = 0x8,
			/// <summary>Do not execute the search heuristics</summary>
			SLR_NOSEARCH = 0x10,
			/// <summary>Do not use distributed link tracking</summary>
			SLR_NOTRACK = 0x20,
			/// <summary>Disable distributed link tracking. By default, distributed link tracking tracks
			/// removable media across multiple devices based on the volume name. It also uses the
			/// Universal Naming Convention (UNC) path to track remote file systems whose drive letter
			/// has changed. Setting SLR_NOLINKINFO disables both types of tracking.</summary>
			SLR_NOLINKINFO = 0x40,
			/// <summary>Call the Microsoft Windows Installer</summary>
			SLR_INVOKE_MSI = 0x80
		}


		/// <summary>The IShellLink interface allows Shell links to be created, modified, and resolved</summary>
		[ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214F9-0000-0000-C000-000000000046")]
		interface IShellLinkW
		{
			/// <summary>Retrieves the path and file name of a Shell link object</summary>
			void GetPath([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out WIN32_FIND_DATAW pfd, SLGP_FLAGS fFlags);
			/// <summary>Retrieves the list of item identifiers for a Shell link object</summary>
			void GetIDList(out IntPtr ppidl);
			/// <summary>Sets the pointer to an item identifier list (PIDL) for a Shell link object.</summary>
			void SetIDList(IntPtr pidl);
			/// <summary>Retrieves the description string for a Shell link object</summary>
			void GetDescription([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
			/// <summary>Sets the description for a Shell link object. The description can be any application-defined string</summary>
			void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
			/// <summary>Retrieves the name of the working directory for a Shell link object</summary>
			void GetWorkingDirectory([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
			/// <summary>Sets the name of the working directory for a Shell link object</summary>
			void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
			/// <summary>Retrieves the command-line arguments associated with a Shell link object</summary>
			void GetArguments([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
			/// <summary>Sets the command-line arguments for a Shell link object</summary>
			void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
			/// <summary>Retrieves the hot key for a Shell link object</summary>
			void GetHotkey(out short pwHotkey);
			/// <summary>Sets a hot key for a Shell link object</summary>
			void SetHotkey(short wHotkey);
			/// <summary>Retrieves the show command for a Shell link object</summary>
			void GetShowCmd(out int piShowCmd);
			/// <summary>Sets the show command for a Shell link object. The show command sets the initial show state of the window.</summary>
			void SetShowCmd(int iShowCmd);
			/// <summary>Retrieves the location (path and index) of the icon for a Shell link object</summary>
			void GetIconLocation([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath,
				int cchIconPath, out int piIcon);
			/// <summary>Sets the location (path and index) of the icon for a Shell link object</summary>
			void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
			/// <summary>Sets the relative path to the Shell link object</summary>
			void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
			/// <summary>Attempts to find the target of a Shell link, even if it has been moved or renamed</summary>
			void Resolve(IntPtr hwnd, SLR_FLAGS fFlags);
			/// <summary>Sets the path and file name of a Shell link object</summary>
			void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);

		}

		[ComImport, Guid("0000010c-0000-0000-c000-000000000046"),
		InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IPersist
		{
			[PreserveSig]
			void GetClassID(out Guid pClassID);
		}


		[ComImport, Guid("0000010b-0000-0000-C000-000000000046"),
		InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IPersistFile : IPersist
		{
			new void GetClassID(out Guid pClassID);
			[PreserveSig]
			int IsDirty();

			[PreserveSig]
			void Load([In, MarshalAs(UnmanagedType.LPWStr)]
		string pszFileName, uint dwMode);

			[PreserveSig]
			void Save([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
				[In, MarshalAs(UnmanagedType.Bool)] bool fRemember);

			[PreserveSig]
			void SaveCompleted([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

			[PreserveSig]
			void GetCurFile([In, MarshalAs(UnmanagedType.LPWStr)] string ppszFileName);
		}

		const uint STGM_READ = 0;
		const int MAX_PATH = 260;

		// CLSID_ShellLink from ShlGuid.h
		[
			ComImport(),
			Guid("00021401-0000-0000-C000-000000000046")
		]
		public class ShellLink
		{
		}

		#endregion


		public static string ResolveLinkFilePath(string filename)
		{
			ShellLink link = new ShellLink();
			((IPersistFile)link).Load(filename, STGM_READ);
			// TODO: if I can get hold of the hwnd call resolve first. This handles moved and renamed files.
			// ((IShellLinkW)link).Resolve(hwnd, 0)
			StringBuilder sb = new StringBuilder(MAX_PATH);
			WIN32_FIND_DATAW data = new WIN32_FIND_DATAW();
			((IShellLinkW)link).GetPath(sb, sb.Capacity, out data, 0);
			return sb.ToString();
		}
#endif

		public static string ToNamedString(this DataSaveLocation me)
		{
			switch (me)
			{
				case DataSaveLocation.None:
					return "None";
				case DataSaveLocation.GameInstallFolder:
					return "GameInstallFolder";
				case DataSaveLocation.CommonDataFolder:
					return "CommonDataFolder";
				case DataSaveLocation.UserFolder:
					return "UserFolder";
				case DataSaveLocation.DriveDataFolder:
					return "Drive";
				default:
					return $"Unrecognized DataSaveLocation: {me}";
			}
		}

		public static DataSaveLocation ToDataSaveLocation(this int dataSaveLocationInt)
		{
			switch (dataSaveLocationInt)
			{
				case 0:
					return DataSaveLocation.None;
				case 1:
					return DataSaveLocation.GameInstallFolder;
				case 2:
					return DataSaveLocation.CommonDataFolder;
				case 3:
					return DataSaveLocation.UserFolder;
				case 4:
					return DataSaveLocation.DriveDataFolder;
			}

			return DataSaveLocation.None;
		}

		public static int ToInt(this DataSaveLocation dataSaveLocation)
		{
			switch (dataSaveLocation)
			{
				case DataSaveLocation.None:
					return 0;
				case DataSaveLocation.GameInstallFolder:
					return 1;
				case DataSaveLocation.CommonDataFolder:
					return 2;
				case DataSaveLocation.UserFolder:
					return 3;
				case DataSaveLocation.DriveDataFolder:
					return 4;
			}

			return 0;
		}

		/// <summary>
		/// This is in the game's StreamingAssets folder.
		/// This is for "core" mod packages that are expected
		/// to be bundled alongside the game itself when installed.
		/// Note that this does not include a trailing slash.
		/// </summary>
		///
		/// <remarks>
		/// In Editor, this is: C:/path/to/unity/project/Assets/StreamingAssets<br/>
		/// In runtime, this is: C:/path/to/standalone/build/buildname_Data/StreamingAssets<br/>
		/// </remarks>
		public static string GameInstallFolderPath => Application.streamingAssetsPath;

		/// <summary>
		/// This is the CommonApplicationData folder path,
		/// accessible regardless of which user is logged in.
		/// Note that this does not include a trailing slash.
		/// </summary>
		///
		/// <remarks>
		/// <para>Ideal location for 3rd-party mod packages.</para>
		///
		/// In Windows, this is C:/ProgramData<br/>
		/// In Mac and Linux, this is /usr/share<br/>
		/// </remarks>
		public static string CommonDataFolder => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).ConvertBackToForwardSlash();

		/// <summary>
		/// This is the ideal location for saved game files
		/// and user settings/preferences.
		/// Note that this does not include a trailing slash.
		/// </summary>
		///
		/// <remarks>
		/// In Windows, this is C:/Users/<i>username</i>/AppData/Local<br/>
		/// <br/>
		/// In Mac, this is /Users/<i>username</i>/.local/share<br/>
		/// <br/>
		/// In Linux, this is /home/<i>username</i>/.local/share<br/>
		/// </remarks>
		public static string UserFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ConvertBackToForwardSlash();

		/// <summary>
		/// Will return absolute path to the save location specified.
		/// Note that the return value includes a trailing slash.
		///
		/// <list type="number">
		/// <item>
		/// <term>GameInstallFolder: </term>
		/// <description>The game's StreamingAssets folder</description>
		/// </item>
		/// <item>
		/// <term>CommonDataFolder: </term>
		/// <description>C:/ProgramData or /usr/share</description>
		/// </item>
		/// <item>
		/// <term>UserFolder: </term>
		/// <description>C:/Users/<i>username</i>/AppData/Local or /Users/<i>username</i>/.local/share or /home/<i>username</i>/.local/share</description>
		/// </item>
		/// </list>
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public static string GetPath(DataSaveLocation location)
		{
			switch (location)
			{
				case DataSaveLocation.GameInstallFolder:
					return GameInstallFolderPath;
				case DataSaveLocation.CommonDataFolder:
					return CommonDataFolder;
				case DataSaveLocation.UserFolder:
					return UserFolderPath;
				default:
					return string.Empty;
			}
		}
	}

	/// <summary>
	/// Possible areas where the game saves/loads Mod Packages, Asset Files, save game files, and other data files.
	/// </summary>
	public enum DataSaveLocation
	{
		None,

		/// <summary>
		/// This is in the game's StreamingAssets folder.
		/// This is for "core" mod packages that are expected
		/// to be bundled alongside the game itself when installed.<br/>
		///
		/// In Editor, this is: C:/path/to/unity/project/Assets/StreamingAssets/<br/>
		/// In runtime, this is: C:/path/to/standalone/build/buildname_Data/StreamingAssets/
		/// </summary>
		GameInstallFolder,

		/// <summary>
		/// In Windows, this is C:/ProgramData/<br/>
		/// In Mac and Linux, this is /usr/share/<br/>
		/// This is the CommonApplicationData folder path,
		/// ideal location for 3rd-party mod packages.
		/// Regardless of the OS user logged-in, the files here
		/// will always be available.
		/// </summary>
		CommonDataFolder,

		/// <summary>
		/// In Windows, this is C:/Users/<i>username</i>/AppData/Local/<br/>
		/// In Mac, this is /Users/<i>username</i>/.local/share/<br/>
		/// In Linux, this is /home/<i>username</i>/.local/share/<br/>
		/// This is the ideal location for saved game files,
		/// user settings/preferences, and game-wide user data (Steam achievement progress).<br/>
		///
		/// This folder is unique to each OS user, but when running the game in Steam,
		/// this merely assumes a 1:1 correspondence between OS user and Steam user account.
		/// For example, if the Steam user for Bob logs in to Steam in a PC whose Windows is
		/// logged-in to the Windows account of Ted (who also has his own Steam account),
		/// this will end up overwriting the saved games and user preferences of Ted.
		/// The proper thing to do would have been to create a new Windows (local) user account
		/// for Bob in that PC, so that he has his own separate MyDocs folder in that PC.
		/// </summary>
		UserFolder,

		/// <summary>
		/// In the root path of a drive,
		/// ideal location for 3rd-party mod packages
		/// located in removable drives/flash disks.
		/// </summary>
		DriveDataFolder,
	}
}