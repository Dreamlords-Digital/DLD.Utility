// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.IO;
using UnityEngine;

namespace DLD.Utility
{
	public static class FileUtil
	{
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