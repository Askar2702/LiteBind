using UnityEngine;

namespace LiteBindDI.Services.Localization
{
    [CreateAssetMenu(menuName = nameof(LiteBindDI) + "/" + nameof(LocalizationSettings), fileName = nameof(LocalizationSettings))]
    public class LocalizationSettings : ScriptableObject
    {
        [Header("Logging")]
        public bool Debug = false;
        
        [Header("Save Load")]
        public string PrefsKey = "LocalizationSettings";
        
        [Header("Settings")]
        public SystemLanguage DefaultSystemLanguage = SystemLanguage.English;
        public TextAsset DefaultTextAsset;

        [Header("Translations")]
        public TranslationsData[] TranslationsData;
        
#if UNITY_EDITOR
        [Header("Remote")]
        public string SheetId = "1roFfP6VEhsV6AAtk9CK1m-cHeNa_DEMtu7sxtb-uX7Q";
        [HideInInspector] public string SheetsExportUrl = "https://docs.google.com/spreadsheets/d/{0}/export?format=tsv";
        [HideInInspector] public string SheetsUrl = "https://docs.google.com/spreadsheets/d/{0}";
#endif
    }
}