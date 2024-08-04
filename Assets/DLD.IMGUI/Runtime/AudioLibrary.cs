// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

#define USE_UnityWebRequestMultimedia

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
#if USE_UnityWebRequestMultimedia
using UnityEngine.Networking;
#endif
#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif

namespace DLD.IMGUI
{
	/// <summary>
	/// Holds reference to many AudioClips so they can be retrieved on-demand.
	/// This class handles loading of the AudioClip, and can work even in the Editor (thanks to UnityEngine.WWW).
	/// </summary>
	public class AudioLibrary
	{
		/// <summary>
		/// Holds all the loaded AudioClips. Key is filename (absolute path).
		/// This is a Dictionary for easy getting of the AudioClip, as long as you have the absolute path.
		/// </summary>
		readonly Dictionary<string, AudioClip> _audioClipsPerFilename = new Dictionary<string, AudioClip>();

		public struct Entry
		{
			public string Filename;
			public string AbsolutePath;
		}

		/// <summary>
		/// Holds all the loaded AudioClips, but in a list for easy iterating.
		/// </summary>
		readonly List<Entry> _audioClipsLoopableList = new List<Entry>();

		public AudioClip GetAudioClip(string absolutePath, bool loadAsStream = false)
		{
			if (string.IsNullOrEmpty(absolutePath))
			{
				return null;
			}

			if (!IsAudioClipLoaded(absolutePath))
			{
				var audioClip = LoadSound(absolutePath, loadAsStream);
				if (audioClip != null)
				{
					_audioClipsPerFilename.Add(absolutePath, audioClip);

					Entry newEntry;
					newEntry.AbsolutePath = absolutePath;
					newEntry.Filename = Path.GetFileName(absolutePath);

					audioClip.name = newEntry.Filename;

					if (!_audioClipsLoopableList.Contains(newEntry))
					{
						_audioClipsLoopableList.Add(newEntry);
					}
				}
			}

			if (_audioClipsPerFilename.ContainsKey(absolutePath))
			{
				return _audioClipsPerFilename[absolutePath];
			}

			return null;
		}

		public bool IsAudioClipLoaded(string absolutePath)
		{
			return _audioClipsPerFilename.ContainsKey(absolutePath);
		}

		// =============================================

		AudioClip LoadSound(string audioAbsolutePath, bool loadAsStream = false)
		{
			if (string.IsNullOrEmpty(audioAbsolutePath))
			{
				return null;
			}

			if (!File.Exists(audioAbsolutePath))
			{
				return null;
			}

			var audioUrl = audioAbsolutePath;
			if (!audioUrl.StartsWith("file://"))
			{
				audioUrl = $"file://{audioUrl}";
			}

			audioUrl = audioUrl.Replace("+", "%2B");

#if UNITY_2018_4_OR_NEWER && USE_UnityWebRequestMultimedia

#if UNITY_EDITOR
			EditorCoroutineUtility.StartCoroutineOwnerless(GetAudioClip(audioAbsolutePath, audioUrl));
#endif

			UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.UNKNOWN);
			www.SendWebRequest();
			if (www.result == UnityWebRequest.Result.Success)
			{
				return DownloadHandlerAudioClip.GetContent(www);
			}
			else
			{
				Debug.LogError(www.error);
				return null;
			}
#else
			WWW www = new WWW(audioUrl);

			if (loadAsStream)
			{
				return www.GetAudioClip(false, true);
			}

#if UNITY_2018_1_OR_NEWER
			return www.GetAudioClip();
#else
			return www.audioClip;
#endif
#endif
		}

#if UNITY_2018_4_OR_NEWER && USE_UnityWebRequestMultimedia
		IEnumerator GetAudioClip(string absolutePath, string audioUrl)
		{
			using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.UNKNOWN))
			{
				yield return www.SendWebRequest();

				if (www.result == UnityWebRequest.Result.Success)
				{
					AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);

					if (audioClip != null)
					{
						_audioClipsPerFilename.Add(absolutePath, audioClip);

						Entry newEntry;
						newEntry.AbsolutePath = absolutePath;
						newEntry.Filename = Path.GetFileName(absolutePath);

						audioClip.name = newEntry.Filename;

						if (!_audioClipsLoopableList.Contains(newEntry))
						{
							_audioClipsLoopableList.Add(newEntry);
						}
					}
				}
				else
				{
					Debug.LogError(www.error);
				}
			}
		}
#endif

		// =============================================

		public bool PlayClipSolo(string soundFileAbsolutePath, bool waitForSoundToLoad = true)
		{
			var clip = GetAudioClip(soundFileAbsolutePath);
			if (clip == null)
			{
				return false;
			}

			if (waitForSoundToLoad)
			{
				while (clip.loadState == AudioDataLoadState.Loading)
				{
				}
			}

			if (clip.loadState == AudioDataLoadState.Loaded)
			{
				PlayClipSolo(clip);
				return true;
			}

			// clip.loadState is probably AudioDataLoadState.Failed
			return false;
		}

		public void PlayClipSolo(AudioClip clip)
		{
			StopAllClips();
			PlayClip(clip);
		}

		// ----------------------------------------------

#if UNITY_EDITOR
		const string UNITY_EDITOR_AUDIO_UTIL_CLASS = "UnityEditor.AudioUtil";
		const string UNITY_EDITOR_AUDIO_UTIL_PLAY_CLIP_METHOD = "PlayClip";
		const string UNITY_EDITOR_AUDIO_UTIL_STOP_ALL_CLIPS_METHOD = "StopAllClips";

		static MethodInfo _playClipMethod;
		static MethodInfo _stopAllClipsMethod;
#else
		GameObject _audioSourceObject;
		AudioSource _audioSource;
#endif

		public void PlayClip(AudioClip clip)
		{
			if (clip == null || clip.loadState != AudioDataLoadState.Loaded)
			{
				return;
			}

#if UNITY_EDITOR
			if (_playClipMethod == null)
			{
				var unityEditorAssembly = typeof(AudioImporter).Assembly;
				var audioUtilClass = unityEditorAssembly.GetType(UNITY_EDITOR_AUDIO_UTIL_CLASS);

				if (audioUtilClass == null)
				{
					Debug.LogErrorFormat("AudioLibrary.PlayClip: Could not find class {0}", UNITY_EDITOR_AUDIO_UTIL_CLASS);
					return;
				}

				_playClipMethod = audioUtilClass.GetMethod(UNITY_EDITOR_AUDIO_UTIL_PLAY_CLIP_METHOD,
					BindingFlags.Static | BindingFlags.Public,
					null,
					new[] { typeof(AudioClip) },
					null);
			}

			if (_playClipMethod == null)
			{
				Debug.LogErrorFormat("AudioLibrary.PlayClip: Could not find {0}.{1}", UNITY_EDITOR_AUDIO_UTIL_CLASS,
					UNITY_EDITOR_AUDIO_UTIL_PLAY_CLIP_METHOD);
				return;
			}

			_playClipMethod.Invoke(null,
				new object[] { clip });
#else
			if (_audioSourceObject == null)
			{
				_audioSourceObject = new GameObject("AudioSourcePlayer");
				UnityEngine.Object.DontDestroyOnLoad(_audioSourceObject);
				_audioSource = _audioSourceObject.AddComponent<AudioSource>();
			}

			_audioSource.clip = clip;
			_audioSource.Play();
#endif
		}

		public void StopAllClips()
		{
#if UNITY_EDITOR
			if (_stopAllClipsMethod == null)
			{
				var unityEditorAssembly = typeof(AudioImporter).Assembly;
				var audioUtilClass = unityEditorAssembly.GetType(UNITY_EDITOR_AUDIO_UTIL_CLASS);

				if (audioUtilClass == null)
				{
					Debug.LogErrorFormat("AudioLibrary.PlayClip: Could not find class {0}", UNITY_EDITOR_AUDIO_UTIL_CLASS);
					return;
				}

				_stopAllClipsMethod = audioUtilClass.GetMethod(UNITY_EDITOR_AUDIO_UTIL_STOP_ALL_CLIPS_METHOD,
					BindingFlags.Static | BindingFlags.Public,
					null,
					new Type[] { },
					null);
			}

			if (_stopAllClipsMethod == null)
			{
				Debug.LogErrorFormat("AudioLibrary.PlayClip: Could not find {0}.{1}", UNITY_EDITOR_AUDIO_UTIL_CLASS,
					UNITY_EDITOR_AUDIO_UTIL_PLAY_CLIP_METHOD);
				return;
			}

			_stopAllClipsMethod.Invoke(null,
				new object[] { });
#else
			if (_audioSource != null)
			{
				_audioSource.Stop();
			}
#endif
		}

		// =============================================
	}
}