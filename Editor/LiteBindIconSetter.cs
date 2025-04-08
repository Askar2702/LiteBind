#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class LiteBindIconSetter
{
    static Texture2D _icon;
    private const string targetNamespace = "LiteBindDI";
    private static readonly string[] typeKeywords = { "interface", "struct", "enum" };

    static LiteBindIconSetter()
    {
        string iconPath = GetIconPath();
        if (string.IsNullOrEmpty(iconPath)) return;

        _icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        if (_icon == null) return;

        EditorApplication.delayCall += AssignIcons;
    }

    private static string GetIconPath()
    {
        // Находим сам скрипт этого класса
        string[] guids = AssetDatabase.FindAssets("LiteBindIconSetter t:Script");
        if (guids.Length == 0) return null;

        string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        if (string.IsNullOrEmpty(scriptPath)) return null;

        string basePath = Path.GetDirectoryName(scriptPath); // путь до папки Editor
        string iconPath = Path.Combine(basePath, "LiteBindDI_Icon.png");
        return iconPath.Replace("\\", "/"); // на всякий случай
    }

    private static void AssignIcons()
    {
        string[] guids = AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets", "Packages" });

        foreach (string guid in guids)
        {
            string scriptPath = AssetDatabase.GUIDToAssetPath(guid);
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
            if (script == null)
                continue;

            bool matchesNamespace = false;
            var scriptClass = script.GetClass();

            if (scriptClass != null)
            {
                matchesNamespace = scriptClass.Namespace == targetNamespace;
            }
            else
            {
                string fileText = File.ReadAllText(scriptPath);
                if (fileText.Contains("namespace " + targetNamespace))
                {
                    matchesNamespace = typeKeywords.Any(fileText.Contains);
                }
            }

            if (matchesNamespace)
            {
                EditorGUIUtility.SetIconForObject(script, _icon);
            }
        }
    }
}
#endif