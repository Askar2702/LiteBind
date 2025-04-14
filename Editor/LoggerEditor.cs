#if UNITY_EDITOR
namespace LiteBindDI.Editor
{
    public static class LoggerEditor
    {
        public static void Success(string message)
        {
            UnityEngine.Debug.Log($"<color=#00C853><b>✔ {message}</b></color>"); 
        }

        public static void Warning(string message)
        {
            UnityEngine.Debug.LogWarning($"<color=#FFD600><b>⚠ {message}</b></color>"); 
        }

        public static void Error(string message)
        {
            UnityEngine.Debug.LogError($"<color=#D50000><b>✖ {message}</b></color>"); 
        }

        public static void Debug(string message)
        {
            UnityEngine.Debug.Log($"<color=#64B5F6><b>🐞 {message}</b></color>");
        }
    }
}
#endif