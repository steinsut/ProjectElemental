using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace GsKit.Extensions
{
    public static class UnityObjectExtensions
    {
        public static string Color(this string myStr, string color)
        {
            return $"<color={color}>{myStr}</color>";
        }

        private static void DoLog(Action<string, UnityObject> LogFunction, string prefix, UnityObject myObj,
            params object[] msg)
        {
#if UNITY_EDITOR
            var name = (myObj ? myObj.name : "NullObject").Color("lightblue");
            LogFunction($"{prefix}[{name}]: {string.Join("; ", msg)}\n ", myObj);
#endif
        }

        public static void LogInfo(this UnityObject myObj, params object[] msg)
        {
            DoLog(Debug.Log, "[INFO]", myObj, msg);
        }

        public static void LogError(this UnityObject myObj, params object[] msg)
        {
            DoLog(Debug.LogError, "[ERROR]".Color("red"), myObj, msg);
        }

        public static void LogWarning(this UnityObject myObj, params object[] msg)
        {
            DoLog(Debug.LogWarning, "[WARNING]".Color("yellow"), myObj, msg);
        }

        public static void LogSuccess(this UnityObject myObj, params object[] msg)
        {
            DoLog(Debug.Log, "[SUCCESS]".Color("green"), myObj, msg);
        }
    }
}