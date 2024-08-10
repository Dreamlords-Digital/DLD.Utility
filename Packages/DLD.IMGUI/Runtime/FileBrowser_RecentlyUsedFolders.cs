// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using DLD.Serializer;
using DLD.Utility;
using UnityEngine;

namespace DLD.IMGUI
{
	public class RecentlyUsedFolders : ITextData
	{
		[Serialized("Folders")]
		List<string> _folders;


		public void PostLoad(string fullPath, string filename)
		{
		}

		public void PrepareSave()
		{
		}

		public bool Contains(string folder)
		{
			if (_folders == null)
			{
				_folders = new List<string>();
			}

			return _folders.Contains(folder);
		}

		public void Add(string folder)
		{
			if (_folders == null)
			{
				_folders = new List<string>();
			}

			if (!_folders.Contains(folder))
			{
				_folders.Add(folder);
			}
		}

		public void SetValuesFrom(List<GUIContent> folders)
		{
			if (_folders == null)
			{
				_folders = new List<string>();
			}

			_folders.Clear();
			for (int n = 0; n < folders.Count; ++n)
			{
				_folders.Add(folders[n].tooltip);
			}
		}

		public void AssignValuesTo(List<GUIContent> folders)
		{
			folders.Clear();

			if (_folders == null || _folders.Count == 0)
			{
				return;
			}

			for (int n = 0; n < _folders.Count; ++n)
			{
				folders.Add(new GUIContent(FileUtil.GetLastFolder(_folders[n]), null, _folders[n]));
			}
		}
	}
}