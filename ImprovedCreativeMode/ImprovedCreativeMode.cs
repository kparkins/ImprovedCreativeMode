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
        private const string PluginVersion = "1.2.1";
        private static bool m_noPlacementCost = false;

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

        [HarmonyPatch(typeof(Player))]
        [HarmonyPatch("UpdatePlacement")]
        class UpdatePlacementPatch
        {
            private const float INVALID_DURABILITY = -1;
            private static float m_rightItemDurability = INVALID_DURABILITY;
            static bool Prefix(Player __instance, ref bool ___m_debugMode, bool takeInput, float dt)
            {
                if(___m_debugMode && Console.instance.IsCheatsEnabled() && __instance.InPlaceMode())
                {
                    var method = AccessTools.Method(typeof(Player), "GetRightItem");
                    var item = (ItemDrop.ItemData) method.Invoke(__instance, null);
                    if (item != null && item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool)
                    {
                        m_rightItemDurability = item.m_durability;
                    }
                }
                return true;
            }
            static void Postfix(Player __instance, ref bool ___m_debugMode, bool takeInput, float dt)
            {
                if (___m_debugMode && Console.instance.IsCheatsEnabled() && __instance.InPlaceMode())
                {
                    var method = AccessTools.Method(typeof(Player), "GetRightItem");
                    var item = (ItemDrop.ItemData)method.Invoke(__instance, null);
                    if (m_rightItemDurability != INVALID_DURABILITY && item != null && item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool)
                    {
                        item.m_durability = m_rightItemDurability;
                        m_rightItemDurability = INVALID_DURABILITY;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.ToggleNoPlacementCost))]
        class ToggleNoPlacementCost
        {
            static void Postfix(ref bool __result)
            {
                m_noPlacementCost = __result;
            }
        }

        [HarmonyPatch(typeof(Piece), nameof(Piece.DropResources))]
        class DropResourcesPatch
        {
            static bool Prefix()
            {
                if(m_noPlacementCost && Player.m_debugMode && Console.instance.IsCheatsEnabled())
                {
                    return false;
                }
                return true;
            }
        }
    }
}
