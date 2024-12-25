// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DLD.Utility
{
	public class BuildInfoUpdater : IPreprocessBuildWithReport
	{
		public int callbackOrder => 0;


		public void OnPreprocessBuild(BuildReport report)
		{
			//Debug.Log($"Build complete. platform: {report.summary.platformGroup} target: {report.summary.platform} outputPath: {outputPath}");
			string streamingAssetsPath = GetOutputStreamingAssetsPath(report.summary.outputPath, report.summary.platform);
			//Debug.Log($"streamingAssetsPath in build: {streamingAssetsPath}");

			Directory.CreateDirectory(streamingAssetsPath);

			var dateTimeNow = DateTime.UtcNow;

			string buildDateAndTimeFilePath = Path.Combine(streamingAssetsPath, BuildInfoUtil.BUILD_DATE_TIME_FILENAME);
			string buildDateAndTimeValue = dateTimeNow.ToLocalTime().ToString(BuildInfoUtil.DISPLAY_DATE_TIME_FORMAT);

			string commitHashFilePath = Path.Combine(streamingAssetsPath, BuildInfoUtil.BUILD_COMMIT_HASH_FILENAME);
			string commitHashValue = GitCommands.GetCurrentCommitHashWithUncommittedChanges();

			Debug.Log($"BuildInfoUpdater: Build Date and Time:\n{buildDateAndTimeFilePath}\n{buildDateAndTimeValue}");
			Debug.Log($"BuildInfoUpdater: Commit Hash:\n{commitHashFilePath}\n{commitHashValue}");

			File.WriteAllText(buildDateAndTimeFilePath, buildDateAndTimeValue);
			File.WriteAllText(commitHashFilePath, commitHashValue);
		}

		/// <summary>
		/// Returns the correct StreamingAssets path of a build output
		/// </summary>
		/// <param name="outputPath">Value coming from BuildReport.summary.outputPath</param>
		/// <param name="outputBuildTarget">Value coming from BuildReport.summary.platform</param>
		/// <exception cref="UnityException">Thrown when outputPath has no parent folder in its path.</exception>
		static string GetOutputStreamingAssetsPath(string outputPath, BuildTarget outputBuildTarget)
		{
			if (outputBuildTarget == BuildTarget.StandaloneOSX)
			{
				// outputPath will be the app
				// Data will be in:
				// build.app/Contents/Resources/Data
				return Path.Combine(outputPath, "Contents/Resources/Data/StreamingAssets");
			}
			else // Windows/Linux
			{
				string correctedOutputPath;
				if (!outputPath.EndsWith("_Data"))
				{
					// outputPath is probably the filename of the build, get the parent folder path out of it
					correctedOutputPath = Path.GetDirectoryName(outputPath);
					if (string.IsNullOrEmpty(correctedOutputPath))
					{
						throw new UnityException($"Invalid BuildReport.summary.outputPath: \"{outputPath}\"");
					}
				}
				else
				{
					// outputPath is already a folder?
					correctedOutputPath = outputPath;
				}

				return Path.Combine(correctedOutputPath, $"{Application.productName}_Data", "StreamingAssets");
			}
		}
	}
}
