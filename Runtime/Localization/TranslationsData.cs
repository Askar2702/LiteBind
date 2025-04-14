using System;
using UnityEngine;

namespace LiteBindDI.Services.Localization
{
    [Serializable]
    public struct TranslationsData
    {
        public SystemLanguage SystemLanguage;
        public TextAsset TextAsset;
    }
}