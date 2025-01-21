using Vector2 = UnityEngine.Vector2;

namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(MenuManager))]
public class MenuManager_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MenuManager), "Start")]  // Putted string directly cuz method is private and too lazy to publicize it lmao
    private static void Start(MenuManager __instance)
    {
        if(!__instance)
        {
            Plugin.logger.LogError("MenuManager instance is null. Something might have booted too early.");
            return;
        }
        if(__instance.versionNumberText is null)
        {
            Plugin.logger.LogWarning("It seems like the Version Number Text of main menu is missing, it's probably the first main menu iteration. Skipping this one.");
            return;
        }
        __instance.versionNumberText.text += $"\n<size=75%>{PluginInfo.DisplayName} v{PluginInfo.Version}</size>";
        __instance.versionNumberText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 50);


        // PATCH ENEMIES


        var enemies = Resources.FindObjectsOfTypeAll<EnemyAI>();
        Plugin.logger.LogInfo($"{enemies.Length} enemies to patch.");
        foreach(var enemy in enemies)
        {
            if (SyncedConfig.Instance.EnemiesData.ContainsKey(enemy.enemyType.enemyName))
                continue;

            EnemiesDataManager.RegisterEnemy(enemy.enemyType.enemyName, new(true, Metadata: new(new(0,0,0), new(0,0,0), true)));
            Plugin.logger.LogInfo($"Mob was not registered. Registered it with name '{enemy.enemyType.enemyName}'");
        }
        EnemiesDataManager.SaveEnemiesData();

        EnemiesDataManager.EnsureEnemy2PropPrefabs();

        Plugin.logger.LogInfo("All enemies have been patched correctly.");
    }
}
