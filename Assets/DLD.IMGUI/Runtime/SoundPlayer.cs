// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;
using System.Globalization;
using System.IO;
using DLD.Utility;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using FileUtil = DLD.Utility.FileUtil;

namespace DLD.IMGUI
{
	/// <summary>
	/// GUI widget that displays a sound file with timeline and play, pause, stop buttons.
	/// </summary>
	public class SoundPlayer
	{
		/// <summary>
		/// File (with absolute path) of a sound file that user wants to be played.
		/// If this has a value, this means user wants to have the sound played but
		/// is currently unable to (most likely because it hasn't finished loading yet).
		/// </summary>
		string _currentAudioFileWantingPreview;

		/// <summary>
		/// File (with absolute path) of a sound file that is being played right now.
		/// </summary>
		string _currentAudioFileBeingPreviewed;

		/// <summary>
		/// True if there is currently a sound being played.
		/// This will make the GUI show a stop button for
		/// the file instead of a play button.
		/// </summary>
		bool _isPreviewingSound;

		/// <summary>
		/// Time in seconds when the currently played sound was started.
		/// This is used to compute the current playback position.
		/// </summary>
		double _timeLastSoundPreviewStart;

		/// <summary>
		/// If there is a sound being played, this is the current time
		/// the sound is in. Measured in seconds.
		/// </summary>
		float _currentPreviewedSoundTimeValue;

		/// <summary>
		/// Length of current sound being played. Measured in seconds.
		/// This is shown in the GUI. Also, it's used to compute the
		/// current playback position.
		/// </summary>
		float _currentPreviewedSoundLength;

		// ===========================================================

		const int SOUND_FILE_MIN_HEIGHT = 50;
		const int PLAY_SOUND_BUTTON_WIDTH = 50;
		const int PLAY_SOUND_BUTTON_HEIGHT = 50;

		const string PREVIEW_SOUND_PLAY_BUTTON_STYLE = "Icon.PlayPreview";
		const string PREVIEW_SOUND_STOP_BUTTON_STYLE = "Icon.StopPreview";

		const string SOUND_FILE_ENTRY_STYLE = "FileDlg.ComplexEntry";
		const string SOUND_FILE_ENTRY_SELECTED_STYLE = "FileDlg.ComplexEntry.Selected";

		// ===========================================================

		public void Clear()
		{
			_currentAudioFileWantingPreview = null;
			_currentAudioFileBeingPreviewed = null;
			_isPreviewingSound = false;
			_currentPreviewedSoundTimeValue = 0;
			_currentPreviewedSoundLength = 0;
		}

		void PlaySound(string soundAbsolutePath, bool waitForSoundToLoad = true)
		{
			var audioLibrary = IoC.Get<AudioLibrary>();
			var success = audioLibrary.PlayClipSolo(soundAbsolutePath, waitForSoundToLoad);

			if (success)
			{
				//Debug.LogFormat("Will play: {0}", soundAbsolutePath);

				_currentAudioFileBeingPreviewed = soundAbsolutePath;
				_currentAudioFileWantingPreview = null;
				_isPreviewingSound = true;

#if UNITY_EDITOR
				if (!EditorApplication.isPlayingOrWillChangePlaymode)
				{
					_timeLastSoundPreviewStart = EditorApplication.timeSinceStartup;
				}
				else
#endif
				{
					_timeLastSoundPreviewStart = Time.unscaledTime;
				}
			}
			else
			{
				//Debug.LogFormat("Could not load yet: {0}", soundAbsolutePath);

				// could not play file right now
				// save the filename so we can continue to try
				// to play the file
				// this is done in TryToPlaySoundWantingPreview() which is executed per frame

				_currentAudioFileWantingPreview = soundAbsolutePath;

				// make sure nothing is considered playing
				_currentAudioFileBeingPreviewed = null;
				_isPreviewingSound = false;
			}
		}

		public void StopAllSounds()
		{
			var audioLibrary = IoC.Get<AudioLibrary>();
			audioLibrary.StopAllClips();

			_currentAudioFileBeingPreviewed = null;
			_currentAudioFileWantingPreview = null;
			_isPreviewingSound = false;
		}

		// ===========================================================

		public void DrawUnremovableSoundEntry(string soundAbsolutePath, string soundFilename, string label,
			string description,
			GUIContent assignNewValueLabel, GUIContent changeValueLabel, GUIContent removeLabel,
			Action onClickChooseCallback)
		{
			DrawChangeableSoundEntry(soundAbsolutePath, soundFilename, label, description,
				assignNewValueLabel, changeValueLabel, removeLabel, false, onClickChooseCallback, null);
		}

		static readonly GUILayoutOption[] ExpandHeightLayout = { GUILayout.ExpandHeight(true) };
		static readonly GUILayoutOption[] SoundFileEntryLayout = { GUILayout.MinHeight(SOUND_FILE_MIN_HEIGHT) };
		static readonly GUILayoutOption[] SoundButtonLayout = { GUILayout.Height(SOUND_FILE_MIN_HEIGHT) };

		static readonly GUILayoutOption[] PlayButtonLayout =
			{ GUILayout.Width(PLAY_SOUND_BUTTON_WIDTH), GUILayout.Height(PLAY_SOUND_BUTTON_HEIGHT) };

#if UNITY_STANDALONE_OSX
		const string SHOW_IN_FILE_BROWSER_LABEL = "Show in\nFinder";
#elif UNITY_STANDALONE_WIN
		const string SHOW_IN_FILE_BROWSER_LABEL = "Show in\nExplorer";
#else
		const string SHOW_IN_FILE_BROWSER_LABEL = "Show in\nBrowser";
#endif

		public void DrawChangeableSoundEntry(string soundAbsolutePath, string soundFilename, string label,
			string description,
			GUIContent assignNewValueLabel, GUIContent changeValueLabel, GUIContent removeLabel, bool allowRemove,
			Action onClickChooseCallback, Action onClickRemoveCallback)
		{
			if (!string.IsNullOrEmpty(label) || !string.IsNullOrEmpty(description))
			{
				Widget.DrawLabelWithDescription(label, description);
			}

			GUILayout.Space(3);
			if (!string.IsNullOrEmpty(soundFilename))
			{
				var thisSoundIsPreviewed = false;
				AudioClip thisSoundClip = null;

				var soundFileExists = File.Exists(soundAbsolutePath);
				if (soundFileExists)
				{
					var audioLibrary = IoC.Get<AudioLibrary>();
					if (audioLibrary.IsAudioClipLoaded(soundAbsolutePath))
					{
						thisSoundClip = audioLibrary.GetAudioClip(soundAbsolutePath);
					}

					thisSoundIsPreviewed = _isPreviewingSound &&
					                       _currentAudioFileBeingPreviewed == soundAbsolutePath &&
					                       thisSoundClip != null &&
					                       thisSoundClip.loadState == AudioDataLoadState.Loaded;
				}

				// -------------------

				var previewButtonToUse = PREVIEW_SOUND_PLAY_BUTTON_STYLE;
				if (thisSoundIsPreviewed)
				{
					previewButtonToUse = PREVIEW_SOUND_STOP_BUTTON_STYLE;
				}

				// -------------------

				GUILayout.BeginVertical(string.Empty, SOUND_FILE_ENTRY_STYLE, SoundFileEntryLayout);
				GUILayout.BeginHorizontal();

				// -------------------
				// Preview Button

				if (soundFileExists && GUILayout.Button(string.Empty, previewButtonToUse, PlayButtonLayout))
				{
					if (thisSoundIsPreviewed)
					{
						StopAllSounds();
					}
					else
					{
						PlaySound(soundAbsolutePath);
					}
				}

				// -------------------
				// Sound File Details (filename, sound length)

				GUILayout.BeginVertical(ExpandHeightLayout);
				GUILayout.Label(soundFilename);

				if (thisSoundClip != null)
				{
					if (thisSoundClip.loadState != AudioDataLoadState.Loaded)
					{
						GUILayout.Label("Loading...");
					}
					else
					{
						if (thisSoundIsPreviewed)
						{
							_currentPreviewedSoundLength = thisSoundClip.length;
						}

						GUILayout.Label($"Length: {thisSoundClip.length.ToString(CultureInfo.InvariantCulture)} secs.");

						GUILayout.HorizontalSlider(thisSoundIsPreviewed ? _currentPreviewedSoundTimeValue : 0, 0,
							thisSoundClip.length);
					}
				}

				GUILayout.BeginHorizontal();
				if (GUILayout.Button(changeValueLabel))
				{
					onClickChooseCallback();
				}

				if (allowRemove && GUILayout.Button(removeLabel))
				{
					onClickRemoveCallback();
				}

				GUILayout.EndHorizontal();

				GUILayout.EndVertical();

				GUILayout.EndHorizontal();

				if (!soundFileExists)
				{
					if (!string.IsNullOrWhiteSpace(soundAbsolutePath))
					{
						GUILayout.BeginHorizontal(Widget.ERROR_LABEL_STYLE_NAME);
						GUILayout.Label($"Sound file \"<b>{soundAbsolutePath}</b>\" not found", "TinyLabel-WarnText");

						var accessible = FileUtil.FixPath(soundAbsolutePath, out var fixedPath);
						var prevEnabled = GUI.enabled;
						GUI.enabled = prevEnabled && accessible;
						if (GUILayout.Button(SHOW_IN_FILE_BROWSER_LABEL, "MiniButton.WithText"))
						{
							ExplorerUtil.OpenInFileBrowser(fixedPath);
						}

						GUI.enabled = prevEnabled;
						GUILayout.EndHorizontal();
					}
					else
					{
						Widget.DrawErrorMessage($"Sound file \"<b>{soundFilename}</b>\" not found");
					}
				}

				GUILayout.EndVertical();
			}
			else
			{
				if (GUILayout.Button(assignNewValueLabel, SoundButtonLayout))
				{
					onClickChooseCallback();
				}
			}
		}

		public void DrawReadOnlySoundEntry(string soundAbsolutePath, string soundFilename, string label,
			string description)
		{
			if (!string.IsNullOrEmpty(label) || !string.IsNullOrEmpty(description))
			{
				Widget.DrawLabelWithDescription(label, description);
			}


			GUILayout.Space(3);
			if (!string.IsNullOrEmpty(soundFilename))
			{
				var audioLibrary = IoC.Get<AudioLibrary>();

				var thisSoundIsPreviewed = false;
				AudioClip thisSoundClip = null;
				if (audioLibrary.IsAudioClipLoaded(soundAbsolutePath))
				{
					thisSoundClip = audioLibrary.GetAudioClip(soundAbsolutePath);

					thisSoundIsPreviewed = _isPreviewingSound &&
					                       _currentAudioFileBeingPreviewed == soundAbsolutePath &&
					                       thisSoundClip != null &&
					                       thisSoundClip.loadState == AudioDataLoadState.Loaded;
				}

				// -------------------

				var previewButtonToUse = PREVIEW_SOUND_PLAY_BUTTON_STYLE;
				if (thisSoundIsPreviewed)
				{
					previewButtonToUse = PREVIEW_SOUND_STOP_BUTTON_STYLE;
				}

				// -------------------

				GUILayout.BeginVertical(string.Empty, SOUND_FILE_ENTRY_STYLE, SoundFileEntryLayout);
				GUILayout.BeginHorizontal();

				// -------------------
				// Preview Button

				if (GUILayout.Button(string.Empty, previewButtonToUse, PlayButtonLayout))
				{
					if (thisSoundIsPreviewed)
					{
						StopAllSounds();
					}
					else
					{
						PlaySound(soundAbsolutePath);
					}
				}

				// -------------------
				// Sound File Details (filename, sound length)

				GUILayout.BeginVertical(ExpandHeightLayout);
				GUILayout.Label(soundFilename);

				if (thisSoundClip != null)
				{
					if (thisSoundClip.loadState != AudioDataLoadState.Loaded)
					{
						GUILayout.Label("Loading...");
					}
					else
					{
						if (thisSoundIsPreviewed)
						{
							_currentPreviewedSoundLength = thisSoundClip.length;
						}

						GUILayout.Label($"Length: {thisSoundClip.length.ToString(CultureInfo.InvariantCulture)} secs.");

						GUILayout.HorizontalSlider(thisSoundIsPreviewed ? _currentPreviewedSoundTimeValue : 0, 0,
							thisSoundClip.length);
					}
				}

				GUILayout.EndVertical();

				GUILayout.EndHorizontal();

				if (!File.Exists(soundAbsolutePath))
				{
					Widget.DrawErrorMessage($"Sound file \"<b>{soundAbsolutePath}</b>\" not found");
				}

				GUILayout.EndVertical();
			}
		}

		public void DrawSelectableSoundEntry(string soundAbsolutePath, string soundFilename, bool isSelected,
			ref Rect soundEntryRect)
		{
			var audioLibrary = IoC.Get<AudioLibrary>();

			var thisSoundIsPreviewed = false;
			AudioClip thisSoundClip = null;
			if (audioLibrary.IsAudioClipLoaded(soundAbsolutePath))
			{
				thisSoundClip = audioLibrary.GetAudioClip(soundAbsolutePath);

				thisSoundIsPreviewed = _isPreviewingSound &&
				                       _currentAudioFileBeingPreviewed == soundAbsolutePath &&
				                       thisSoundClip != null &&
				                       thisSoundClip.loadState == AudioDataLoadState.Loaded;
			}

			// --------------------------------------------

			var entryStyleToUse = SOUND_FILE_ENTRY_STYLE;
			if (isSelected)
			{
				entryStyleToUse = SOUND_FILE_ENTRY_SELECTED_STYLE;
			}

			var previewButtonToUse = PREVIEW_SOUND_PLAY_BUTTON_STYLE;
			if (thisSoundIsPreviewed)
			{
				previewButtonToUse = PREVIEW_SOUND_STOP_BUTTON_STYLE;
			}

			// --------------------------------------------

			GUILayout.BeginHorizontal(string.Empty, entryStyleToUse, SoundButtonLayout);


			// -------------------
			// Preview Button

			if (GUILayout.Button(string.Empty, previewButtonToUse, PlayButtonLayout))
			{
				if (thisSoundIsPreviewed)
				{
					StopAllSounds();
				}
				else
				{
					PlaySound(soundAbsolutePath);
					//_selectedFileIdx = n;
				}
			}

			// -------------------
			// Sound File Details (filename, sound length)

			GUILayout.BeginVertical(ExpandHeightLayout);
			GUILayout.Label(soundFilename);

			if (thisSoundClip != null)
			{
				if (thisSoundClip.loadState != AudioDataLoadState.Loaded)
				{
					GUILayout.Label("Loading...");
				}
				else
				{
					if (thisSoundIsPreviewed)
					{
						_currentPreviewedSoundLength = thisSoundClip.length;
					}

					GUILayout.Label($"Length: {thisSoundClip.length.ToString("n2", CultureInfo.InvariantCulture)} secs.");

					GUILayout.HorizontalSlider(thisSoundIsPreviewed ? _currentPreviewedSoundTimeValue : 0, 0,
						thisSoundClip.length);
				}
			}

			GUILayout.EndVertical();
			if (Event.current.type == EventType.Repaint)
			{
				soundEntryRect = GUILayoutUtility.GetLastRect();
			}
		}

		// ===========================================================

		public void UpdateSoundPreview(
#if UNITY_EDITOR
			ref bool requestRepaint
#endif
		)
		{
#if UNITY_EDITOR
			if (_isPreviewingSound)
			{
				requestRepaint = true;
			}
#endif

			UpdatePlaybackPosition();

			// keep trying to play requested file if there is one
			TryToPlaySoundWantingPreview();
		}

		void UpdatePlaybackPosition()
		{
			if (_isPreviewingSound)
			{
#if UNITY_EDITOR
				_currentPreviewedSoundTimeValue = (float)(EditorApplication.timeSinceStartup - _timeLastSoundPreviewStart);
#else
				_currentPreviewedSoundTimeValue = (float)(Time.unscaledTime - _timeLastSoundPreviewStart);
#endif
				if (_currentPreviewedSoundTimeValue >= _currentPreviewedSoundLength)
				{
					_isPreviewingSound = false;
				}
			}
		}

		void TryToPlaySoundWantingPreview()
		{
			if (!string.IsNullOrEmpty(_currentAudioFileWantingPreview))
			{
				PlaySound(_currentAudioFileWantingPreview);
			}
		}

		// ===========================================================
	}
}