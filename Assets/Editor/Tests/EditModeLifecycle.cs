using System.Reflection;
using UnityEngine;

namespace Tests
{
    static class EditModeLifecycle
    {
        public static void InvokeAwake(MonoBehaviour behaviour)
        {
            if (!behaviour)
            {
                return;
            }

            MethodInfo awake = behaviour.GetType().GetMethod(
                "Awake",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            awake?.Invoke(behaviour, null);
        }

        public static void InvokeOnEnable(MonoBehaviour behaviour)
        {
            if (!behaviour)
            {
                return;
            }

            MethodInfo onEnable = behaviour.GetType().GetMethod(
                "OnEnable",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            onEnable?.Invoke(behaviour, null);
        }

        public static void InvokeMethod(MonoBehaviour behaviour, string methodName)
        {
            if (!behaviour)
            {
                return;
            }

            MethodInfo method = behaviour.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            method?.Invoke(behaviour, null);
        }
    }
}
