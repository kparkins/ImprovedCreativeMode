﻿using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace ImprovedCreativeMode
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInProcess("valheim.exe")]
    public class ImprovedCreativeMode : BaseUnityPlugin
    {

        private const string PluginGUID = "io.kparkins.ImprovedCreativeMode";
        private const string PluginName = "ImprovedCreativeMode";
        private const string PluginVersion = "2.0.0";

        private readonly Harmony harmony = new Harmony("io.kparkins.ImprovedCreativeMode");

        private void Awake()
        {
            Debug.Log("ImprovedCreativeMode has loaded.");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(Player), nameof(Player.HaveStamina))]
        class HaveStaminaPatch
        {
            static bool Prefix(ref bool __result, ref bool ___m_debugMode, ref float ___m_stamina, ref float ___m_maxStamina)
            {
                Debug.Log("Calling prefix HaveStamina method.");
                if (___m_debugMode && Console.instance.IsCheatsEnabled())
                {
                    __result = true;
                    ___m_stamina = ___m_maxStamina;
                    return false;
                }
                return true;
            }
        }
    }
}