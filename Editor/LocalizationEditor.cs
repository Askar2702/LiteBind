#if UNITY_EDITOR
using LiteBindDI.Services.Localization;
using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace LiteBindDI.Editor
{
    [CustomEditor(typeof(LocalizationSettings))]
    public class LocalizationEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Space(20);
            var settings = (LocalizationSettings)target;

            if (GUILayout.Button("Download Remote Localizations", GUILayout.Height(30)))
            {
                if (string.IsNullOrEmpty(settings.SheetId))
                {
                    var source = $"{nameof(LiteBindDI)}/{nameof(LocalizationSettings)}";
                    LoggerEditor.Error($"'SheetId' cannot be null or empty. Correct the settings file '{source}'");
                    return;
                }
                
                DownloadSheets(string.Format(settings.SheetsExportUrl, settings.SheetId), OnSheetLoaded);
            }  
            
            if (GUILayout.Button("Open Remote Localizations", GUILayout.Height(30)))
            {
                if (string.IsNullOrEmpty(settings.SheetId))
                {
                    var source = $"{nameof(LiteBindDI)}/{nameof(LocalizationSettings)}";
                    LoggerEditor.Error($"'SheetId' cannot be null or empty. Correct the settings file '{source}'");
                    return;
                }
                Application.OpenURL(string.Format(settings.SheetsUrl, settings.SheetId));

            }
        }
        
        private void OnSheetLoaded(string sheet)
        {
            LoggerEditor.Success("Download successful");

            foreach (SystemLanguage language in Enum.GetValues(typeof(SystemLanguage)))
            {
                ParseLanguage(sheet, language.ToString());
            }    
        }

        private void ParseLanguage(string sheet, string language)
        {
            var lines = sheet.Split('\n').Select(line => line.Trim()).ToArray();
            var headers = lines[0].Split('\t').Select(header => header.Trim()).ToArray();
            foreach (var h in headers)
            {
                LoggerEditor.Debug($"Header: [{h}]");
            }
            var keyIndex = Array.IndexOf(headers, headers[0]);
            var langIndex = Array.IndexOf(headers, language);

            if (keyIndex == -1)
            {
                LoggerEditor.Error($"Key Index {keyIndex}");
                return;
            }

            LoggerEditor.Debug(keyIndex.ToString());
            LoggerEditor.Debug(langIndex.ToString());
            if (langIndex == -1)
                return;

            var dictionaryData = new LocalizationDictionaryData
            {
                Localization = new()
            };

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split('\t');
                
                if (parts.Length > keyIndex)
                {
                    var key = parts[keyIndex];
                    
                    if (parts.Length > langIndex)
                    {
                        dictionaryData.Localization.Add(new()
                        {
                            Key = key,
                            Value = parts[langIndex]
                        });
                    }
                }
            }
            var data = JsonConvert.SerializeObject(dictionaryData, Formatting.Indented);
            FileSystemEditor.CreateTranslationFile(language, data);
        }

        private void DownloadSheets(string url, Action<string> callback)
        {
            LoggerEditor.Debug($"Start downloading sheets with '{url}'");
            var request = UnityWebRequest.Get(url);
            
            request.SendWebRequest().completed += (asyncOperation) =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                    callback?.Invoke(request.downloadHandler.text);
                else
                    LoggerEditor.Error($"Download error: {request.error}");
            };
        }
    }
}
#endif