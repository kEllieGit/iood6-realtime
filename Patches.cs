using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RealTime
{
    [HarmonyPatch(typeof(ClockSystem), "AdvanceTimer")]
    public static class Patch_ClockSystem_AdvanceTimer
    {
        public static bool Prefix(ref IEnumerator __result, ClockSystem __instance)
        {
            __result = AdvanceTimer(__instance);
            return false;
        }

        public static IEnumerator AdvanceTimer(ClockSystem __instance)
        {
            FieldInfo hoursField = AccessTools.Field(typeof(ClockSystem), "_hours");
            FieldInfo minutesField = AccessTools.Field(typeof(ClockSystem), "_minutes");

            int hours = (int)hoursField.GetValue(__instance);
            int minutes = (int)minutesField.GetValue(__instance);
            
            yield return new WaitForSeconds(60f);

            minutes++;
            if (minutes >= 60)
            {
                minutes = 0;
                hours++;
            }

            hoursField.SetValue(__instance, hours);
            minutesField.SetValue(__instance, minutes);

            typeof(ClockSystem).GetMethod("UpdateTimeText", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, null);
            typeof(ClockSystem).GetMethod("TriggerEvents", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, null);
        }
    }

    // 
    // Bug Fixes
    //

    [HarmonyPatch(typeof(AnomalyCameraSystem), "SelectAvailableAnomalyFromList")]
    public static class Patch_AnomalyCameraSystem_SelectAvailableAnomalyFromList
    {
        public static bool Prefix(ref AnomalyActivator __result, AnomalyCameraSystem __instance)
        {
            __result = SelectRandomAnomaly(__instance);
            return false;
        }

        public static AnomalyActivator SelectRandomAnomaly(AnomalyCameraSystem __instance)
        {
            MethodInfo availableAnomalies = typeof(AnomalyCameraSystem).GetMethod("GetAvailableAnomalies", BindingFlags.NonPublic | BindingFlags.Instance);

            if (availableAnomalies.Invoke(__instance, null) is not List<AnomalyActivator> anomalies)
            {
                Plugin.LogSource.LogError("GetAvailableAnomalies returned null!");
                return null;
            }

            if (anomalies.Any())
            {
                return anomalies[Random.Range(0, anomalies.Count - 1)];
            }

            Plugin.LogSource.LogMessage("No available anomalies left, refreshing list...");

            foreach (var item in __instance.Anomalies)
            {
                __instance.AvailableAnomalies.Add(item);
            }

            return __instance.AvailableAnomalies[Random.Range(0, __instance.AvailableAnomalies.Count - 1)];
        }
    }
}
