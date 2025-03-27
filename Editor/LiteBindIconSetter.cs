using System;
using System.Drawing;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class LiteBindIconSetter
{
    static Texture2D _icon;
    private const string targetNamespace = "LiteBindDI";
    // �������� ����� ��� ������ ��-��������� �����
    private static readonly string[] typeKeywords = { "interface", "struct", "enum" };

    static LiteBindIconSetter()
    {
        _icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LiteBind/Editor/LiteBindDI_Icon.png");
        if (_icon == null) return;

        EditorApplication.delayCall += AssignIcons;
    }

    private static void AssignIcons()
    {
        string[] guids = AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets/LiteBind" });
        foreach (string guid in guids)
        {
            string scriptPath = AssetDatabase.GUIDToAssetPath(guid);
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
            if (script == null)
                continue;

            bool matchesNamespace = false;
            // ������� �������� ����� �� �������
            var scriptClass = script.GetClass();
            if (scriptClass != null)
            {
                // ���� ����� ������, ��������� ��� ������������ ���
                matchesNamespace = scriptClass.Namespace == targetNamespace;
            }
            else
            {
                // ���� ����� �� ������, ��������, ��� ���������, ��������� ��� ������������.
                // ��������� ���������� ����� � ���� ������ �������� ����� � ������������ ���.
                string fileText = File.ReadAllText(scriptPath);
                if (fileText.Contains("namespace " + targetNamespace))
                {
                    foreach (var keyword in typeKeywords)
                    {
                        if (fileText.Contains(keyword))
                        {
                            matchesNamespace = true;
                            break;
                        }
                    }
                }
            }

            if (matchesNamespace)
            {
                EditorGUIUtility.SetIconForObject(script, _icon);
            }
        }
    }
}
