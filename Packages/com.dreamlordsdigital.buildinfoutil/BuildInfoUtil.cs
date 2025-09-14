// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

#if !UNITY_EDITOR
using System.IO;
using UnityEngine;
#endif

namespace DLD.Utility
{

public static class BuildInfoUtil
{
	public const string BuildDateTimeFilename = "BuildDateTime.txt";
	public const string BuildCommitHashFilename = "CommitHash.txt";
	public const string DisplayDateTimeFormat = "yyyy MMM dd ddd hh:mm:ss tt UTCz";

	public static (string, string) GetBuildDateAndCommitHash()
	{
#if UNITY_EDITOR
		return (System.DateTime.UtcNow.ToLocalTime().ToString(DisplayDateTimeFormat),
			GitCommands.GetCurrentCommitHashWithUncommittedChanges());
#else
			string streamingAssetsPath = Application.streamingAssetsPath;

			if (string.IsNullOrEmpty(streamingAssetsPath))
			{
				Debug.LogWarning($"BuildInfoDisplay: Can't determine StreamingAssets path. Aborting. (platform: {Application.platform})");
				return (null, null);
			}

			if (!Directory.Exists(streamingAssetsPath))
			{
				Debug.LogWarning($"BuildInfoDisplay: StreamingAssets directory not found: {streamingAssetsPath}");
				return (null, null);
			}

			string buildDateTimeFilePath = Path.Combine(streamingAssetsPath, BuildDateTimeFilename);
			string buildDateTimeValue = File.Exists(buildDateTimeFilePath) ? File.ReadAllText(buildDateTimeFilePath) : null;

			string buildCommitHashFilePath = Path.Combine(streamingAssetsPath, BuildCommitHashFilename);
			string buildCommitHashValue = File.Exists(buildCommitHashFilePath) ? File.ReadAllText(buildCommitHashFilePath) : null;

			return (buildDateTimeValue, buildCommitHashValue);
#endif
	}
}

}
