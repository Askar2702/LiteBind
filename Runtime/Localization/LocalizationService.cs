using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiteBindDI.Services.Localization
{
    public class LocalizationService :ILocalizationService 
    {
        private readonly Dictionary<string, string> _dictionary = new();
        private readonly string _prefsKey = "Localization.Language";
        private readonly LocalizationSettings _settings;

        public event Action OnLocalizationUpdate;

        public LocalizationService()
        {
            _settings = Resources.Load<LocalizationSettings>("Translations/LocalizationSettings");
            var lang = GetLanguage();
            var asset = GetTextAsset(lang);
            LoadTextAsset(asset);
        }
      

        public SystemLanguage GetLanguage()
        {
            var saved = PlayerPrefs.GetString(_prefsKey, _settings.DefaultSystemLanguage.ToString());
            Enum.TryParse(saved, out SystemLanguage language);
            return language;
        }

        public void SetLanguage(SystemLanguage language)
        {
            PlayerPrefs.SetString(_prefsKey, language.ToString());
            LoadTextAsset(GetTextAsset(language));
        }

        public string GetTranslation(string key) =>
            _dictionary.TryGetValue(key, out var value) ? value : $"'{key}'";

        public string GetTranslation(string key, params object[] args) =>
            _dictionary.TryGetValue(key, out var value) ? string.Format(value, args) : $"'{key}'";

        private TextAsset GetTextAsset(SystemLanguage language)
        {
            foreach (var entry in _settings.TranslationsData)
            {
                if (entry.SystemLanguage == language && entry.TextAsset != null)
                    return entry.TextAsset;
            }
            return _settings.DefaultTextAsset;
        }

        private void LoadTextAsset(TextAsset asset)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<LocalizationDictionaryData>(asset.text);
                _dictionary.Clear();
                foreach (var entry in data.Localization)
                {
                    _dictionary[entry.Key] = entry.Value;
                }

                OnLocalizationUpdate?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"Localization load failed: {e.Message}");
            }
        }


    }
}
