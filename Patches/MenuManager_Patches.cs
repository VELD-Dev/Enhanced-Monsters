using Vector2 = UnityEngine.Vector2;

namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(MenuManager))]
public class MenuManager_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MenuManager), "Start")]  // Putted string directly cuz method is private and too lazy to publicize it lmao
    private static void Start(MenuManager __instance)
    {
        if(__instance is null)
        {
            Plugin.logger.LogError("MenuManager instance is null. Something might have booted too early.");
            return;
        }
        __instance.versionNumberText.text += $"\n<size=75%>{PluginInfo.DisplayName} v{PluginInfo.Version}</size>";
        __instance.versionNumberText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 50);
    }
}
