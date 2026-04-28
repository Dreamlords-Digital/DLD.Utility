// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public static class ExplorerUtil
{
	public static void OpenInMacFileBrowser(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return;
		}

		bool openInsidesOfFolder = false;

		// try mac
		string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

		if (Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
		{
			openInsidesOfFolder = true;
		}

		//Debug.Log("macPath: " + macPath);
		//Debug.Log("openInsidesOfFolder: " + openInsidesOfFolder);

		if (!macPath.StartsWith("\""))
		{
			macPath = "\"" + macPath;
		}

		if (!macPath.EndsWith("\""))
		{
			macPath = macPath + "\"";
		}

		string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

		//Debug.Log("arguments: " + arguments);
		try
		{
			Process.Start("open", arguments);
		}
		catch (System.ComponentModel.Win32Exception e)
		{
			// tried to open mac finder in windows
			// just silently skip error
			// we currently have no platform define for the current OS we are in, so we resort to this
			e.HelpLink = ""; // do anything with this variable to silence warning about not using it
		}
	}

	public static void OpenInWinFileBrowser(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return;
		}

		bool openInsidesOfFolder = false;

		// try windows
		string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

		if (Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
		{
			openInsidesOfFolder = true;
		}

		try
		{
			Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
		}
		catch (System.ComponentModel.Win32Exception e)
		{
			// tried to open win explorer in mac
			// just silently skip error
			// we currently have no platform define for the current OS we are in, so we resort to this
			e.HelpLink = ""; // do anything with this variable to silence warning about not using it
		}
	}

	public static void OpenInLinuxFileBrowser(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return;
		}

		string linuxPath = path.Replace("\\", "/"); // linux doesn't like backward slashes

		if (File.Exists(linuxPath))
		{
			// from https://stackoverflow.com/a/73409251
			using Process dbusShowItemsProcess = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "dbus-send",
					Arguments = "--print-reply --dest=org.freedesktop.FileManager1 /org/freedesktop/FileManager1 org.freedesktop.FileManager1.ShowItems array:string:\"file://" +
					            linuxPath +
					            "\" string:\"\"",
					UseShellExecute = true
				}
			};
			dbusShowItemsProcess.Start();
			dbusShowItemsProcess.WaitForExit();

			if (dbusShowItemsProcess.ExitCode == 0)
			{
				// dbus ran successfully
				return;
			}
		}

		// The dbus invocation can fail for a variety of reasons:
		// - dbus is not available
		// - no programs implement the service,
		// - ...

		// Attempt xdg-open instead

		if (File.Exists(linuxPath))
		{
			// xdg-open will open the file, so get the directory path instead
			linuxPath = Path.GetDirectoryName(linuxPath);
		}

		//Debug.Log("macPath: " + macPath);
		//Debug.Log("openInsidesOfFolder: " + openInsidesOfFolder);

		if (!linuxPath.StartsWith("\""))
		{
			linuxPath = "\"" + linuxPath;
		}

		if (!linuxPath.EndsWith("\""))
		{
			linuxPath = linuxPath + "\"";
		}

		string arguments = linuxPath;

		//Debug.Log("arguments: " + arguments);
		try
		{
			Process.Start("xdg-open", arguments);
		}
		catch (System.ComponentModel.Win32Exception e)
		{
			e.HelpLink = ""; // do anything with this variable to silence warning about not using it
		}
	}

	public static void OpenWithDefaultProgram(string path)
	{
		Process.Start(path);
	}

	public static void OpenInFileBrowser(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return;
		}

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			OpenInWinFileBrowser(path);
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			OpenInMacFileBrowser(path);
		}
		else // assume Linux
		{
			OpenInLinuxFileBrowser(path);
		}
	}

	public static string GetRevealInFileBrowser()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return "Reveal in Explorer";
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return "Reveal in Finder";
		}
		else // assume Linux
		{
			return "Reveal in File Browser";
		}
	}
}
