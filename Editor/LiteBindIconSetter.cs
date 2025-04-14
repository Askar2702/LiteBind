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
        if (string.IsNullOrEmpty(iconPath))
            return;

        _icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        if (_icon == null)
            return;

        EditorApplication.delayCall += AssignIcons;
    }

    private static string GetIconPath()
    {
        // ������� ��� ������ ����� ������
        string[] guids = AssetDatabase.FindAssets("LiteBindIconSetter t:Script");
        if (guids.Length == 0)
            return null;

        string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        if (string.IsNullOrEmpty(scriptPath))
            return null;

        string basePath = Path.GetDirectoryName(scriptPath); // ���� �� ����� Editor
        string iconPath = Path.Combine(basePath, "LiteBindDI_Icon.png");
        return iconPath.Replace("\\", "/"); // �� ������ ������
    }

    private static void AssignIcons()
    {
        // ���� ��� MonoScript � ������ Assets � Packages
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
                // ��������� ������������ ����:
                // - ��� ��� ����� targetNamespace
                // - ��� ���������� � "targetNamespace." (��������, LiteBindDI.Services)
                matchesNamespace = scriptClass.Namespace != null &&
                                   (scriptClass.Namespace == targetNamespace ||
                                    scriptClass.Namespace.StartsWith(targetNamespace + "."));
            }
            else
            {
                // ���� ����� �� ������, ������ ����� ����� � ���� ���������
                string fileText = File.ReadAllText(scriptPath);
                bool foundNamespace = fileText.Contains("namespace " + targetNamespace + ".") ||
                                      fileText.Contains("namespace " + targetNamespace + " ");
                if (foundNamespace)
                {
                    // ������������� ��������� ������� �������� ����, ����������� � ����� (interface, struct, enum)
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
