using LiteBindDI;
using System;

namespace LiteBindDI.Services.Localization
{
    public static class LocalizationExtensions
    {
        public static string ToLocalization(this string key) =>
            GetTranslation(key);

        public static string ToLocalization(this Enum key) =>
            GetTranslation(key.ToString());

        public static string ToLocalization(this string key, params object[] args) =>
            GetTranslation(key, args);

        public static string ToLocalization(this Enum key, params object[] args) =>
            GetTranslation(key.ToString(), args);

        private static string GetTranslation(string key) =>
            GetLocalizationService().GetTranslation(key);

        private static string GetTranslation(string key, object[] args) =>
            GetLocalizationService().GetTranslation(key, args);

        private static ILocalizationService GetLocalizationService() =>
            LiteSceneContext.Container.Resolve<ILocalizationService>();
    }
}