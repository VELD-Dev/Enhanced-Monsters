using Vector2 = UnityEngine.Vector2;

namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(MenuManager))]
public class MenuManager_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MenuManager), "Start")]
    private static void Start(MenuManager __instance)
    {
        if (!__instance)
        {
            Plugin.logger.LogError("MenuManager instance is null. Something might have booted too early.");
            return;
        }
        if (__instance.versionNumberText is null)
        {
            Plugin.logger.LogWarning("It seems like the Version Number Text of main menu is missing, it's probably the first main menu iteration. Skipping this one.");
            return;
        }
        __instance.versionNumberText.text += $"\n<size=75%>{PluginInfo.DisplayName} v{PluginInfo.Version}</size>";
        __instance.versionNumberText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 50);

        EnemiesDataManager.ScanAndRegisterUnknownEnemies();
        EnemiesDataManager.EnsureEnemy2PropPrefabs();
        Plugin.logger.LogInfo("All enemies have been patched correctly.");
    }
}
