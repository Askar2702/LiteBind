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
    // Ключевые слова для поиска не-классовых типов
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
            // Попытка получить класс из скрипта
            var scriptClass = script.GetClass();
            if (scriptClass != null)
            {
                // Если класс найден, проверяем его пространство имён
                matchesNamespace = scriptClass.Namespace == targetNamespace;
            }
            else
            {
                // Если класс не найден, возможно, это интерфейс, структура или перечисление.
                // Считываем содержимое файла и ищем нужные ключевые слова и пространство имён.
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
