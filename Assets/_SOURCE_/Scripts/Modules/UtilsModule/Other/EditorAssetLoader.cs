namespace _GameResources.Scripts.Tools
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public static class EditorAssetLoader
    {
        public static T FindFirstAsset<T>(this T _)
            where T : Object
        {
            return FindFirstAssetOfType<T>();
        }

        public static T[] FindAllAssets<T>(this T _)
            where T : Object
        {
            return FindAllAssetsOfType<T>();
        }

        public static T FindFirstAssetOfType<T>()
            where T : Object
        {
#if UNITY_EDITOR

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    return asset;
                }
            }

#endif

            return null;
        }

        public static T[] FindAllAssetsOfType<T>()
            where T : Object
        {
#if UNITY_EDITOR

            List<T> result = new List<T>();

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    result.Add(asset);
                }
            }

            return result.ToArray();

#endif

            return null;
        }
    }
}