using System;
using System.Reflection;
using ContentWarning.GUI;
using HarmonyLib;
using Steamworks;
using UnityCheats_ContentWarning;
using UnityEngine;
using Zorro.Core;

namespace Util
{
    public class HarmonyPatcher : MonoBehaviour
    {
        private void Awake()
        {
            Initialize();
        }

        public static void Initialize()
        {
            var harmony = new Harmony("com.FatherBone.ContentWarning");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(Player), "Die")]
        public static class DiePatch
        {
            [HarmonyPrefix]
            public static bool DiePrefix(Player __instance)
            {
                if (Class1.GodMode && __instance.IsLocal)
                {
                    return false;
                }
                return true;
            }
        }
     
        [HarmonyPatch(typeof(GameHandler), "OnKickNotifactionReceived")] //Only kick thing i can find
        public static class KickPatch
        {
            [HarmonyPrefix]
            public static bool OnKickNotifactionReceived(GameHandler __instance, KickPlayerNotificationPackage obj)
            {
                if (Class1.AntiKick)
                {
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(Player), "RPCA_PlayerDie")] //Wont work in real code so had to make it a patcher, Maybe it will work as a class this time?
        public static class DiePatcher
        {
            [HarmonyPrefix]
            public static void RPCA_PlayerDieRevivee(Player __instance)
            {
                if (Class1.GodMode && __instance.IsLocal && __instance.data.dead)
                {
                    __instance.CallRevive();
                }
            }
        }
        [HarmonyPatch(typeof(Player), "RPCA_PlayerDie")] //Wont work in real code so had to make it a patcher, UPDATE: does not work gotta make it a class maybe?
        public static void RPCA_PlayerDieRevive(Player __instance)
        {
            if (Class1.GodMode && __instance.IsLocal && __instance.data.dead)
            {
                __instance.CallRevive();
            }
        }
            [HarmonyPatch(typeof(Player), "CallDie")]
        public static class CallDiePatch
        {
            [HarmonyPrefix]
            public static bool CallDiePrefix(Player __instance)
            {
                if (Class1.GodMode && __instance.IsLocal)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
