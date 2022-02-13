using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace ImprovedCreativeMode
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInProcess("valheim.exe")]
    public class ImprovedCreativeMode : BaseUnityPlugin
    {

        private const string PluginGUID = "com.github.kparkins.ImprovedCreativeMode";
        private const string PluginName = "ImprovedCreativeMode";
        private const string PluginVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony("com.github.kparkins.ImprovedCreativeMode");

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
