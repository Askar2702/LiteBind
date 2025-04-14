#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LiteBindDI.Editor
{
    public static class FileSystemEditor
    {
        private static readonly string _path = "Assets/Resources/Translations";

        public static void CreateAssetFile<TData>() where TData : ScriptableObject
        {
            EnsureFoldersExist(_path);

            string assetName = typeof(TData).Name;
            string assetPath = $"{_path}/{assetName}.asset";

            if (!File.Exists(assetPath))
            {
                var scriptableObject = ScriptableObject.CreateInstance<TData>();
                AssetDatabase.CreateAsset(scriptableObject, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = scriptableObject;

                LoggerEditor.Success($"'{assetPath}' file created");
            }
        }

        public static void CreateTranslationFile(string language, string jsonData)
        {
            EnsureFoldersExist(_path);

            string filePath = $"{_path}/{language}.json";
            bool fileExists = File.Exists(filePath);

            File.WriteAllText(filePath, jsonData);
            AssetDatabase.Refresh();

            LoggerEditor.Success(fileExists
                ? $"'{filePath}' file updated"
                : $"'{filePath}' file created");
        }

        private static void EnsureFoldersExist(string fullPath)
        {
            string[] folders = fullPath.Split('/');
            string currentPath = folders[0]; // "Assets"

            for (int i = 1; i < folders.Length; i++)
            {
                string nextFolder = folders[i];
                string combinedPath = $"{currentPath}/{nextFolder}";

                if (!AssetDatabase.IsValidFolder(combinedPath))
                {
                    AssetDatabase.CreateFolder(currentPath, nextFolder);
                    LoggerEditor.Debug($"Created folder: '{combinedPath}'");
                }

                currentPath = combinedPath;
            }
        }
    }
}
#endif
