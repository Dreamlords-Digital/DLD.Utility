// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DLD.Utility
{
	public static class GitCommands
	{
		static string GetCurrentCommitHash(bool shortVersion = false)
		{
			var processStart = new ProcessStartInfo("git", shortVersion ? "rev-parse --short HEAD" : "rev-parse HEAD");
			processStart.UseShellExecute = false;
			processStart.CreateNoWindow = true;
			processStart.RedirectStandardOutput = true;

			var process = Process.Start(processStart);
			if (process == null)
			{
				return null;
			}

			var sb = new StringBuilder();

			while (!process.StandardOutput.EndOfStream)
			{
				sb.Append(process.StandardOutput.ReadLine());
			}

			process.WaitForExit();

			return sb.ToString();
		}

		static bool AreThereUncommittedChanges()
		{
			var processStart = new ProcessStartInfo("git", "diff-index --quiet HEAD");
			processStart.UseShellExecute = false;
			processStart.CreateNoWindow = true;

			var process = Process.Start(processStart);
			if (process == null)
			{
				return false;
			}

			process.WaitForExit();

			return process.ExitCode > 0;
		}

		public static string GetCurrentCommitHashWithUncommittedChanges(bool shortVersion = false)
		{
			if (AreThereUncommittedChanges())
			{
				return $"{GetCurrentCommitHash(shortVersion)}*";
			}

			return GetCurrentCommitHash(shortVersion);
		}

		static bool RevertToNonNullCommit(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				return false;
			}

			if (!FileUtil.IsFileAllNull(filePath))
			{
				return true;
			}

			var indexRevertSuccess = RevertFile(filePath, null); // try reverting to index first
			if (!indexRevertSuccess || FileUtil.IsFileAllNull(filePath))
			{
				// reverting to index failed

				var commitHashes = new List<string>();
				GetFileCommitHistory(filePath, "\"%H\"", commitHashes); // get file history

				for (int n = 0, len = commitHashes.Count; n < len; ++n)
				{
					var success = RevertFile(filePath, commitHashes[n]); // revert to this commit
					if (!success)
					{
						continue;
					}

					if (!FileUtil.IsFileAllNull(filePath))
					{
						return true;
					}
				}

				return false;
			}
			else
			{
				return true;
			}
		}

		public static void GetLastCommitInfo(string filePath, out string authorName, out string date, out string dateRelative)
		{
			authorName = null;
			date = null;
			dateRelative = null;

			if (string.IsNullOrWhiteSpace(filePath))
			{
				return;
			}

			var processStart = new ProcessStartInfo("git", $"log -n 1 --pretty=format:%an%n%ad%n%ar -- {filePath}");
			processStart.UseShellExecute = false;
			processStart.CreateNoWindow = true;
			processStart.RedirectStandardOutput = true;

			var process = Process.Start(processStart);
			if (process == null)
			{
				return;
			}

			while (!process.StandardOutput.EndOfStream)
			{
				var got = process.StandardOutput.ReadLine();
				if (string.IsNullOrEmpty(authorName))
				{
					authorName = got;
				}
				else if (string.IsNullOrEmpty(date))
				{
					date = got;
				}
				else if (string.IsNullOrEmpty(dateRelative))
				{
					dateRelative = got;
				}
			}

			process.WaitForExit();
		}

		public static void GetFileCommitHistory(string filePath, string prettyFormat, IList<string> commitHashes)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				return;
			}

			var processStart = new ProcessStartInfo("git", $"log --patience --pretty=format:{prettyFormat} -- {filePath}");
			processStart.UseShellExecute = false;
			processStart.CreateNoWindow = true;
			processStart.RedirectStandardOutput = true;

			var process = Process.Start(processStart);
			if (process == null)
			{
				return;
			}

			commitHashes.Clear();
			while (!process.StandardOutput.EndOfStream)
			{
				var got = process.StandardOutput.ReadLine();
				if (!string.IsNullOrEmpty(got))
				{
					commitHashes.Add(got);
				}
			}

			process.WaitForExit();
		}

		static bool RevertFile(string filePath, string commitHash)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				return false;
			}

			// git checkout <commit_hash> -- <filePath>
			string args;
			if (string.IsNullOrEmpty(commitHash))
			{
				args = $"checkout -- {filePath}";
			}
			else
			{
				args = $"checkout {commitHash} -- {filePath}";
			}

			var processStart = new ProcessStartInfo("git", args);
			processStart.UseShellExecute = false;
			processStart.CreateNoWindow = true;
			processStart.RedirectStandardOutput = false;
			var process = Process.Start(processStart);
			if (process == null)
			{
				return false;
			}

			process.WaitForExit();

			return true;
		}

		public static string GetDiff(string filePath, string commitHashBefore, string commitHashAfter, Func<string, string> changeLine)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				return null;
			}

			var processStart = new ProcessStartInfo("git",
				$"diff --ignore-cr-at-eol --exit-code --diff-algorithm=patience --no-indent-heuristic {commitHashBefore}..{commitHashAfter} -- {filePath}");
			processStart.UseShellExecute = false;
			processStart.CreateNoWindow = true;
			processStart.RedirectStandardOutput = true;

			var process = Process.Start(processStart);
			if (process == null)
			{
				return null;
			}

			var sb = new StringBuilder();
			var foundFirstDiffLine = false;

			while (!process.StandardOutput.EndOfStream)
			{
				var gotLine = process.StandardOutput.ReadLine();

				if (!foundFirstDiffLine && gotLine != null && gotLine.StartsWith("@@ "))
				{
					foundFirstDiffLine = true;
				}

				if (!foundFirstDiffLine)
				{
					continue;
				}

				if (changeLine != null)
				{
					gotLine = changeLine(gotLine);
				}

				sb.Append(gotLine);
				sb.Append("\n");
			}

			process.WaitForExit();

			return sb.ToString();
		}
	}
}