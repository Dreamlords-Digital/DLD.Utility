// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using UnityEditor;

namespace DLD.Utility.Inspector.Editor
{
    public static class ReserializeUtil
    {
        [MenuItem("Assets/Reserialize selected")]
        public static void Reserialize()
        {
            if (Selection.count == 0)
            {
                return;
            }

            var assetPaths = new List<string>();
            foreach (var selected in Selection.objects)
            {
                var prefabAssetType = PrefabUtility.GetPrefabAssetType(selected);
                if (prefabAssetType == PrefabAssetType.Regular ||
                    prefabAssetType == PrefabAssetType.Variant)
                {
                    assetPaths.Add(AssetDatabase.GetAssetPath(selected));
                }
            }
            AssetDatabase.ForceReserializeAssets(assetPaths);
        }
    }
}
