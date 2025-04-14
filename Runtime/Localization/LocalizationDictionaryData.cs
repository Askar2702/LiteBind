using System;
using System.Collections.Generic;

namespace LiteBindDI.Services.Localization
{
    [Serializable]
    public struct LocalizationDictionaryData
    {
        public List<LocalizationData> Localization;
    }
}