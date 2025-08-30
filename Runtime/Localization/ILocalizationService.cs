using System;
using UnityEngine;

namespace LiteBindDI.Services.Localization
{
    public interface ILocalizationService 
    {
        event Action OnLocalizationUpdate;
        SystemLanguage GetLanguage();
        void SetLanguage(SystemLanguage language);
        string GetTranslation(string key);
        string GetTranslation(string key, params object[] args);
        FontLocalizationData GetFontLocalizationData();
    }
}