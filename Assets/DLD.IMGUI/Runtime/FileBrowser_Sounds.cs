// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using DLD.Utility;
using UnityEngine;

namespace DLD.IMGUI
{
	// FileBrowser_Sounds
	// -------
	// All the code that deals with drawing sound
	// files and being able to preview the sound.
	//

	public partial class FileBrowser
	{
		// ===========================================================

		readonly SoundPlayer _soundPlayer = IoC.Get<SoundPlayer>();
		readonly Dictionary<int, Rect> _rectsPerFile = new Dictionary<int, Rect>();

		// ===========================================================

		void DrawSoundFileEntries()
		{
			if (_filesWithImages == null || _filesWithImages.Length == 0)
			{
				return;
			}

			for (int n = 0, len = _filesWithImages.Length; n < len; ++n)
			{
				var soundAbsolutePath = _currentPath + "/" + _filesWithImages[n].text;

				Rect thisSoundEntryRect;
				if (_rectsPerFile.ContainsKey(n))
				{
					thisSoundEntryRect = _rectsPerFile[n];
				}
				else
				{
					thisSoundEntryRect = new Rect();
				}

				_soundPlayer.DrawSelectableSoundEntry(soundAbsolutePath, _filesWithImages[n].text, _selectedFileIdx == n, ref thisSoundEntryRect);

				_rectsPerFile[n] = thisSoundEntryRect;

				// --------------------------------------------

				// -------------------
				// Clicking sound file

				if (Event.current.type == EventType.MouseDown)
				{
					if (_rectsPerFile[n].Contains(Event.current.mousePosition))
					{
						_selectedFileIdx = n;

						if (Event.current.clickCount == 1)
						{
							FileSelectCallback(n, _filesWithImages[n].text);
						}
						else if (Event.current.clickCount == 2)
						{
							FileDoubleClickCallback(n, _filesWithImages[n].text);
						}
					}
				}

				GUILayout.EndHorizontal();

			} // end of for loop for sound files

			// -------------------
		}

		// ===========================================================

		void UpdateSoundPreview(
#if UNITY_EDITOR
			ref bool requestRepaint
#endif
		)
		{
			_soundPlayer.UpdateSoundPreview(
#if UNITY_EDITOR
				ref requestRepaint
#endif
			);
		}

		// ===========================================================
	}
}
