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
	public const string ResourcesFolder = "/Resources/";
	public const int EnteredFbxFileLen = 5;
	public const int FbxFileExtensionLen = 4;

	public const string FbxFileExtension = ".fbx";
	public const string EnteredFbxFile = ".fbx:";
	public const string AnimFileExtension = ".anim";
	public const string PrefabFileExtension = ".prefab";
	public const string AssetBundleFileExtension = ".asset";
	public const string EnteredAssetBundleFile = ".asset:";

	public static long GetFileSizeInBytes(string filename)
	{
		if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
		{
			return 0;
		}

		FileInfo fi = new FileInfo(filename);
		return fi.Length;
	}

	public static string GetBytesReadable(long bytes)
	{
		return MyFileSizeReadable(bytes);
	}

	const double OneTerabyte = 1099511627776.0;
	const double OneGigabyte = 1073741824.0;
	const double OneMegabyte = 1048576.0;
	const double OneKilobyte = 1024.0;

	static string MyFileSizeReadable(long bytes)
	{
		if (bytes < 0)
		{
			return "N/A";
		}

		double converted = bytes;
		string units = "B";

		if (bytes >= OneTerabyte)
		{
			converted = bytes / OneTerabyte;
			units = "TB";
		}
		else if (bytes >= OneGigabyte)
		{
			converted = bytes / OneGigabyte;
			units = "GB";
		}
		else if (bytes >= OneMegabyte)
		{
			converted = bytes / OneMegabyte;
			units = "MB";
		}
		else if (bytes >= OneKilobyte)
		{
			converted = bytes / OneKilobyte;
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

	public static bool IsInvalidFileNameChar(this char c)
	{
		var invalidChars = Path.GetInvalidFileNameChars();
		for (int i = 0; i < invalidChars.Length; i++)
		{
			if (c == invalidChars[i])
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///    Does string end in an image file type that Unity supports?
	///    (psd, jpg, gif, png, tif, tga, bmp, dds, exr, iff, pict)
	/// </summary>
	/// <param name="file"></param>
	/// <returns></returns>
	public static bool IsImageFile(this string file)
	{
		return file.IsFileOfType(".psd") ||
		       file.IsFileOfType(".jpeg") ||
		       file.IsFileOfType(".jpg") ||
		       file.IsFileOfType(".gif") ||
		       file.IsFileOfType(".png") ||
		       file.IsFileOfType(".tiff") ||
		       file.IsFileOfType(".tif") ||
		       file.IsFileOfType(".tga") ||
		       file.IsFileOfType(".bmp") ||
		       file.IsFileOfType(".dds") ||
		       file.IsFileOfType(".exr") ||
		       file.IsFileOfType(".iff") ||
		       file.IsFileOfType(".pict");
	}

	/// <summary>
	///    Does string end in a sound file type that Unity supports?
	///    (wav, mp3, ogg, aif, xm, mod, it, s3m)
	/// </summary>
	/// <param name="file"></param>
	/// <returns></returns>
	public static bool IsSoundFile(this string file)
	{
		return file.IsFileOfType(".wav") ||
		       file.IsFileOfType(".mp3") ||
		       file.IsFileOfType(".ogg") ||
		       file.IsFileOfType(".aif") ||
		       file.IsFileOfType(".xm") ||
		       file.IsFileOfType(".mod") ||
		       file.IsFileOfType(".it") ||
		       file.IsFileOfType(".s3m");
	}

	/// <summary>
	///    Does string end in a video file type that Unity supports?
	/// </summary>
	public static bool IsVideoFile(this string file)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return file.IsFileOfType(".ogv") ||
			       file.IsFileOfType(".vp8") ||
			       file.IsFileOfType(".webm");
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return file.IsFileOfType(".ogv") || // linux supported
			       file.IsFileOfType(".vp8") ||
			       file.IsFileOfType(".webm") ||
			       file.IsFileOfType(".dv") || // mac supported
			       file.IsFileOfType(".m4v") ||
			       file.IsFileOfType(".mov") ||
			       file.IsFileOfType(".mp4") ||
			       file.IsFileOfType(".mpg") ||
			       file.IsFileOfType(".mpeg");
		}
		else // assume Windows
		{
			return file.IsFileOfType(".ogv") || // linux supported
			       file.IsFileOfType(".vp8") ||
			       file.IsFileOfType(".webm") ||
			       file.IsFileOfType(".dv") || // mac supported
			       file.IsFileOfType(".m4v") ||
			       file.IsFileOfType(".mov") ||
			       file.IsFileOfType(".mp4") ||
			       file.IsFileOfType(".mpg") ||
			       file.IsFileOfType(".mpeg") ||
			       file.IsFileOfType(".asf") || // windows supported
			       file.IsFileOfType(".avi") ||
			       file.IsFileOfType(".wmv");
		}
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
	///    This gets the name of the deepest subfolder in the specified path.
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
			lastSlashIdx = inFolder.LastIndexOf('/', inFolder.Length - 2, inFolder.Length - 1);
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
	///    If passed string is path to a file, this will remove the file part of the string.
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
	///    Removes any non-existent folders from the path. Requires an absolute path.
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
	///    Compare two paths if they are the same.
	///    Backslash and forward slashes are considered equivalent.
	///    Trailing slashes on either path are ignored.
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
	///    Combines the strings like <see cref="Path.Combine(string,string)"/>, but always uses forward slash.
	/// </summary>
	/// <param name="path1"></param>
	/// <param name="path2"></param>
	/// <returns></returns>
	public static string CombinePath(string path1, string path2)
	{
		path1 = path1.ConvertBackToForwardSlash();
		path2 = path2.ConvertBackToForwardSlash();

		if (string.IsNullOrEmpty(path2))
		{
			return path1;
		}

		if (string.IsNullOrEmpty(path1))
		{
			return path2;
		}

		char path1End = path1[^1];
		if (path1End != '/')
		{
			return $"{path1}/{path2}";
		}

		return $"{path1}{path2}";
	}

	public static string CombinePath(string path1, string path2, string path3)
	{
		return CombinePath(CombinePath(path1, path2), path3);
	}

	public static string CombinePath(string path1, string path2, string path3, string path4)
	{
		return CombinePath(CombinePath(path1, path2), CombinePath(path3, path4));
	}

	public static string CombinePath(string path1, string path2, string path3, string path4, string path5)
	{
		return CombinePath(CombinePath(path1, path2), CombinePath(path3, path4), path5);
	}

	public static string CombinePath(string path1, string path2, string path3, string path4, string path5, string path6)
	{
		return CombinePath(CombinePath(path1, path2), CombinePath(path3, path4), CombinePath(path5, path6));
	}

	public static string CombinePath(string path1, string path2, string path3, string path4, string path5, string path6, string path7)
	{
		return CombinePath(CombinePath(path1, path2), CombinePath(path3, path4), CombinePath(path5, path6), path7);
	}

	public static string CombinePath(string path1, string path2, string path3, string path4, string path5, string path6, string path7, string path8)
	{
		return CombinePath(CombinePath(path1, path2), CombinePath(path3, path4), CombinePath(path5, path6), CombinePath(path7, path8));
	}

	/// <summary>
	///    <para>Changes a string so that it can be used in Resources.Load().</para>
	///    For example, it will change
	///    "Assets/Resources/SomePackage/My.Path/Something.prefab" to:
	///    "SomePackage/My.Path/Something"
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

		const string ResourcesFolderName = "/Resources/";
		const int ResourcesFolderNameLength = 11;

		var lastResources = path.LastIndexOf(ResourcesFolderName, StringComparison.OrdinalIgnoreCase);

		string result;
		if (lastResources >= 0)
		{
			result = path.Substring(lastResources + ResourcesFolderNameLength);
		}
		else
		{
			result = path;
		}

		// -----------------------------------------

		var hasEnteredFbxFile = result.Contains(EnteredFbxFile, StringComparison.OrdinalIgnoreCase);
		var hasEnteredAssetBundleFile = result.Contains(EnteredAssetBundleFile, StringComparison.OrdinalIgnoreCase);

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
	///    Given a full path, return only part of the path that starts with "Assets/",
	///    as in the project's top-level Assets folder.
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

	public static string GetShortFolderName(this string path)
	{
		if (path.IsPathRoot())
		{
			// "C:/" or "/"
			return path;
		}
		else
		{
			return Path.GetFileName(path);
		}
	}

	/// <summary>
	///    Whether the specified path is the root of the file system ("C:/" or "/")
	/// </summary>
	public static bool IsPathRoot(this string path)
	{
		// Path.GetDirectoryName returns null if path does not have a parent directory.
		// That only happens if the specified path is the root already.
		return string.IsNullOrEmpty(Path.GetDirectoryName(path));
	}

	public static bool IsPathValid(this string path)
	{
		if (path.Contains(':') && (path.Length < 2 || path[0] == ':' || path[1] != ':' || path.Count(':') > 1))
		{
			// if there is a colon, it must be only one, and as the 2nd char in the string
			return false;
		}

		try
		{
			// if path is invalid somehow, this will throw an exception
			string unused = Path.GetFullPath(path);
		}
		catch (Exception e)
		{
			Debug.LogError($"In path: \"{path}\" {e}");
			return false;
		}

		return true;
	}

	public static (bool success, string modifiedPath) FixPath(this string path)
	{
		// ensure drive letter is capitalized
		if (path.Length >= 2 && char.IsLower(path[0]) && path[1] == ':')
		{
			path = char.ToUpper(path[0]) + path[1..];
		}

		path = path.ConvertBackToForwardSlash();

		// Remove trailing slash if present,
		// this is to make the path values consistent.
		// Only time we don't do this is for root "C:/" or "/"
		if (!path.IsPathRoot() && path[^1] == '/')
		{
			path = path[..^1];
		}
		else if (path.Length == 2 && char.IsLetter(path[0]) && path[1] == ':')
		{
			// change "C:" to "C:/"
			path += "/";
		}

		// if folder doesn't exist, keep trying the parent folder until we find one that exists
		if (!Directory.Exists(path))
		{
			string tryPath = path;
			do
			{
				tryPath = Path.GetDirectoryName(tryPath);
			} while (!string.IsNullOrEmpty(tryPath) && !Directory.Exists(tryPath));

			if (string.IsNullOrEmpty(tryPath) || !Directory.Exists(tryPath))
			{
				// no folder in the path exists
				return (false, null);
			}

			path = tryPath;
		}

		return (true, path);
	}

	public static string NormalizePath(this string path)
	{
		// ensure drive letter is capitalized
		if (path.Length >= 2 && char.IsLower(path[0]) && path[1] == ':')
		{
			path = char.ToUpper(path[0]) + path[1..];
		}

		path = path.ConvertBackToForwardSlash();

		// Remove trailing slash if present,
		// this is to make the path values consistent.
		// Only time we don't do this is for root "C:/" or "/"
		if (!path.IsPathRoot() && path[^1] == '/')
		{
			path = path[..^1];
		}
		else if (path.Length == 2 && char.IsLetter(path[0]) && path[1] == ':')
		{
			// change "C:" to "C:/"
			path += "/";
		}

		return path;
	}

	public static void DeleteAllFilesInFolder(string folderPath)
	{
		var directory = new DirectoryInfo(folderPath);
		foreach (FileInfo file in directory.EnumerateFiles())
		{
			file.Delete();
		}
	}

	/// <summary>
	///    Absolute path to Project's folder (without the "/Assets" at the end).
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

	/// <summary>
	///    Absolute path to Project's folder (but with the subfolder "UserSettings" instead of "Assets").
	/// </summary>
	public static string ProjectUserSettingsPath
	{
		get
		{
#if UNITY_EDITOR
			return $"{Application.dataPath[..^6]}UserSettings"; // remove the "Assets" and add "UserSettings"
#else
			return Application.dataPath;
#endif
		}
	}

	public static string ProjectPathWithTrailingSlash
	{
		get
		{
#if UNITY_EDITOR
			string result = Application.dataPath;

			return result[..^6]; // minus 7 to remove the "Assets"
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
			result = result.Substring(lastSlash + 1);

			return result;
		}
	}

	static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
	static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();

	// Based on https://stackoverflow.com/a/37347881/1377948
	public static string RemoveWhitespaceAndInvalid(string str, char replacementChar = '\0',
		bool removeWhitespace = true, bool removeInvalidFileChars = true)
	{
		int len = str.Length;
		char[] src = str.ToCharArray();
		int dstIdx = 0;

		for (int i = 0; i < len; i++)
		{
			char ch = src[i];

			if (removeInvalidFileChars)
			{
				if (InvalidFileNameChars.IndexOf(ch) != -1 || InvalidPathChars.IndexOf(ch) != -1)
				{
					if (replacementChar != '\0' && src[dstIdx - 1] != replacementChar)
					{
						src[dstIdx++] = replacementChar;
					}

					continue;
				}
			}

			if (removeWhitespace)
			{
				switch (ch)
				{
					case '\u0008': // Backspace
					case '\u0009': // Horizontal Tab
					case '\u000A': // Line Feed
					case '\u000B': // Vertical Tab
					case '\u000C': // Form Feed
					case '\u000D': // Carriage Return
					case '\u0085': // Next Line
					case '\u0020': // Space
					case '\u00A0': // No-break Space
					case '\u1680': // Ogham Space Mark
					case '\u2000': // En Quad
					case '\u2001': // Em Quad
					case '\u2002': // En Space
					case '\u2003': // Em Space
					case '\u2004': // Three-Per-Em Space
					case '\u2005': // Four-Per-Em Space
					case '\u2006': // Six-Per-Em Space
					case '\u2007': // Figure Space
					case '\u2008': // Punctuation Space
					case '\u2009': // Thin Space
					case '\u200A': // Hair Space
					case '\u202F': // Narrow No-Break Space
					case '\u205F': // Medium Mathematical Space
					case '\u3000': // Ideographic Space
					case '\u2028': // Line Separator
					case '\u2029': // Paragraph Separator
						if (replacementChar != '\0' && src[dstIdx - 1] != replacementChar)
						{
							src[dstIdx++] = replacementChar;
						}
						continue;
				}
			}

			src[dstIdx++] = ch;
		}

		return new string(src, 0, dstIdx);
	}

	public static string ToValidFilename(this string input, char replacementChar = '\0')
	{
		input = input.Trim();
		input = RemoveWhitespaceAndInvalid(input, replacementChar);

		return input;
	}

	public static string ProductNameValidatedForPath => Application.productName.ToValidFilename('_');

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)

	// -------------------------------------------------------------------------------------------------
	// code from http://www.codeproject.com/Articles/22328/Getting-Drive-s-Volume-Information-using-C
	// obviously works only in windows
	//

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	static extern bool GetVolumeInformation(
		string volume, StringBuilder volumeName,
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

	#region Signatures imported from http: //pinvoke.net

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
		///    Do not display a dialog box if the link cannot be resolved. When SLR_NO_UI is set,
		///    the high-order word of fFlags can be set to a time-out value that specifies the
		///    maximum amount of time to be spent resolving the link. The function returns if the
		///    link cannot be resolved within the time-out duration. If the high-order word is set
		///    to zero, the time-out duration will be set to the default value of 3,000 milliseconds
		///    (3 seconds). To specify a value, set the high word of fFlags to the desired time-out
		///    duration, in milliseconds.
		/// </summary>
		SLR_NO_UI = 0x1,

		/// <summary>Obsolete and no longer used</summary>
		SLR_ANY_MATCH = 0x2,

		/// <summary>
		///    If the link object has changed, update its path and list of identifiers.
		///    If SLR_UPDATE is set, you do not need to call IPersistFile::IsDirty to determine
		///    whether or not the link object has changed.
		/// </summary>
		SLR_UPDATE = 0x4,

		/// <summary>Do not update the link information</summary>
		SLR_NOUPDATE = 0x8,

		/// <summary>Do not execute the search heuristics</summary>
		SLR_NOSEARCH = 0x10,

		/// <summary>Do not use distributed link tracking</summary>
		SLR_NOTRACK = 0x20,

		/// <summary>
		///    Disable distributed link tracking. By default, distributed link tracking tracks
		///    removable media across multiple devices based on the volume name. It also uses the
		///    Universal Naming Convention (UNC) path to track remote file systems whose drive letter
		///    has changed. Setting SLR_NOLINKINFO disables both types of tracking.
		/// </summary>
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
		void GetIconLocation(
			[Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath,
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
		void Load([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);

		[PreserveSig]
		void Save(
			[In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
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
	///    This is in the game's StreamingAssets folder.
	///    This is for "core" mod packages that are expected
	///    to be bundled alongside the game itself when installed.
	///    Note that this does not include a trailing slash.
	/// </summary>
	/// <remarks>
	///    In Editor, this is: <c>C:/path/to/unity/project/Assets/StreamingAssets</c><br/>
	///    In runtime, this is: <c>C:/path/to/standalone/build/buildname_Data/StreamingAssets</c>
	/// </remarks>
	public static string GameInstallFolderPath => Application.streamingAssetsPath;

	/// <summary>
	///    This is the CommonApplicationData folder path,
	///    accessible regardless of which user is logged in.
	///    Note that this does not include a trailing slash.
	/// </summary>
	/// <remarks>
	///    <para>
	///       Ideal location for 3rd-party mod packages.
	///    </para>
	///    <para>
	///       In Windows, this is <c>C:/ProgramData</c>
	///    </para>
	///    <para>
	///       In Mac and Linux, this technically should be <c>/usr/share</c>,
	///       but instead we resort to using <see cref="UserFolderPath"/>,
	///       since <c>/usr/share</c> is normally meant for system-wide apps, not user apps like games.
	///    </para>
	///    <para>
	///       That means in Linux, we use <c>/home/<i>username</i>/.local/share</c><br/>
	///       In Mac, this is <c>/Users/<i>username</i>/.local/share</c>
	///    </para>
	/// </remarks>
	public static string CommonDataFolder
	{
		get
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				return UserFolderPath;
			}

			return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).ConvertBackToForwardSlash();
		}
	}

	/// <summary>
	///    This is the ideal location for saved game files, and installed user mods.
	///    Note that this does not include a trailing slash.
	/// </summary>
	/// <remarks>
	///    In Windows, this is <c>C:/Users/<i>username</i>/AppData/Local</c><br/>
	///    <br/>
	///    In Mac, this is <c>/Users/<i>username</i>/.local/share</c><br/>
	///    <br/>
	///    In Linux, this is <c>/home/<i>username</i>/.local/share</c>
	/// </remarks>
	public static string UserFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ConvertBackToForwardSlash();

	/// <summary>
	///    This is the ideal location for user settings/preferences.
	///    Note that this does not include a trailing slash.
	/// </summary>
	/// <remarks>
	///    In Windows, this is <c>C:/Users/<i>username</i>/AppData/Roaming</c><br/>
	///    <br/>
	///    In Mac, this is <c>/Users/<i>username</i>/.config</c><br/>
	///    <br/>
	///    In Linux, this is <c>/home/<i>username</i>/.config</c>
	/// </remarks>
	public static string UserSettingsFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ConvertBackToForwardSlash();

	/// <summary>
	///    Will return absolute path to the save location specified.
	///    Note that the return value includes a trailing slash.
	///    <list type="number">
	///       <item>
	///          <term>GameInstallFolder: </term>
	///          <description>The game's StreamingAssets folder</description>
	///       </item>
	///       <item>
	///          <term>CommonDataFolder: </term>
	///          <description>
	///             <c>C:/ProgramData</c> or <c>/Users/<i>username</i>/.local/share</c> or
	///             <c>/home/<i>username</i>/.local/share</c>
	///          </description>
	///       </item>
	///       <item>
	///          <term>UserFolder: </term>
	///          <description>
	///             <c>C:/Users/<i>username</i>/AppData/Local</c> or <c>/Users/<i>username</i>/.local/share</c> or
	///             <c>/home/<i>username</i>/.local/share</c>
	///          </description>
	///       </item>
	///    </list>
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
			case DataSaveLocation.UserSettingsFolder:
				return UserSettingsFolderPath;
			default:
				return string.Empty;
		}
	}
}

/// <summary>
///    Possible areas where the game saves/loads Mod Packages, Asset Files, save game files, and other data files.
/// </summary>
public enum DataSaveLocation : byte
{
	None,

	/// <summary>
	///    <para>
	///       This is in the game's StreamingAssets folder.
	///       This is for "core" mod packages that are expected
	///       to be bundled alongside the game itself when installed.
	///    </para>
	///    <para>
	///       In Editor, this is: <c>C:/path/to/unity/project/Assets/StreamingAssets/</c><br/>
	///       In runtime, this is: <c>C:/path/to/standalone/build/buildname_Data/StreamingAssets/</c>
	///    </para>
	/// </summary>
	GameInstallFolder,

	/// <summary>
	///    <para>
	///       In Windows, this is <c>C:/ProgramData/</c>
	///    </para>
	///    <para>
	///       This is the CommonApplicationData folder path,
	///       ideal location for 3rd-party mod packages.
	///       Regardless of the OS user logged-in, the files here
	///       will always be available.
	///    </para>
	///    <para>
	///       In Mac and Linux, this technically should be <c>/usr/share</c>,
	///       but instead we resort to using <see cref="UserFolder"/>,
	///       since <c>/usr/share</c> is normally meant for system-wide apps, not user apps like games.
	///    </para>
	///    <para>
	///       That means in Linux, we use <c>/home/<i>username</i>/.local/share</c><br/>
	///       In Mac, this is <c>/Users/<i>username</i>/.local/share</c>
	///    </para>
	/// </summary>
	CommonDataFolder,

	/// <summary>
	///    <para>
	///       In Windows, this is <c>C:/Users/<i>username</i>/AppData/Local/</c><br/>
	///       In Mac, this is <c>/Users/<i>username</i>/.local/share/</c><br/>
	///       In Linux, this is <c>/home/<i>username</i>/.local/share/</c>
	///    </para>
	///    <para>
	///       This is the ideal location for saved game files,
	///       user settings/preferences, and game-wide user data (Steam achievement progress).
	///    </para>
	///    <para>
	///       This folder is unique to each OS user, but when running the game in Steam,
	///       this merely assumes a 1:1 correspondence between OS user and Steam user account.
	///       For example, if the Steam user for Bob logs in to Steam in a PC whose Windows is
	///       logged-in to the Windows account of Ted (who also has his own Steam account),
	///       this will end up overwriting the saved games and user preferences of Ted.
	///       The proper thing to do would have been to create a new Windows (local) user account
	///       for Bob in that PC, so that he has his own separate MyDocs folder in that PC.
	///    </para>
	/// </summary>
	UserFolder,

	/// <summary>
	///    <para>
	///       In Windows, this is <c>C:/Users/<i>username</i>/AppData/Roaming</c><br/>
	///       In Mac, this is <c>/Users/<i>username</i>/.config</c><br/>
	///       In Linux, this is <c>/home/<i>username</i>/.config</c>
	///    </para>
	///    <para>
	///       This is the ideal location for user settings/preferences.
	///       Note that this does not include a trailing slash.
	///    </para>
	/// </summary>
	UserSettingsFolder,

	/// <summary>
	///    In the root path of a drive,
	///    ideal location for 3rd-party mod packages
	///    located in removable drives/flash disks.
	/// </summary>
	DriveDataFolder,
}

}
