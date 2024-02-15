using System;
using System.Collections.Generic;
using System.Text;

namespace LootableMonsters.Patches;

[HarmonyPatch(typeof(EnemyAI))]
internal class EnemyAI_Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Start))]
    private static void Start(EnemyAI __instance)
    {
        Plugin.logger.LogDebug($"Mob {__instance.enemyType.enemyName} spawned, assigning rank.");
        string creatureRank = EnemiesDataManager.CreaturesRank[__instance.enemyType.enemyName] ?? "?";
        if(__instance.gameObject.TryGetComponent<ScanNodeProperties>(out var scanData))
        {
            scanData.subText = $"Rank {creatureRank}";
        }
        Plugin.logger.LogDebug($"Mob rank assigned. Rank: {creatureRank}");
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.KillEnemy))]
    private static void KillEnemy(EnemyAI __instance, bool destroy)
    {
        Plugin.logger.LogInfo($"Mob {__instance.enemyType.enemyName} died.");
        if (__instance == null) return;  // Should never happen

        if (destroy) return;

        Plugin.logger.LogInfo("Mob was not destroyed. Now making it grabbable.");

        if (!__instance.isEnemyDead)
        {
            __instance.isEnemyDead = true;
        }

        if(!EnemiesDataManager.EnemiesData.ContainsKey(__instance.enemyType.enemyName))
        {
            EnemiesDataManager.RegisterEnemy(__instance.enemyType.enemyName, new(true));
            Plugin.logger.LogInfo($"Mob was not registered. Registered it with name '{__instance.enemyType.enemyName}'");
        }

        var enemyData = EnemiesDataManager.EnemiesData[__instance.enemyType.enemyName];

        var physPropComponent = __instance.gameObject.AddComponent<PhysicsProp>();
        Plugin.logger.LogInfo("Added GrabbableObject component to mob. Now setting it up.");
        physPropComponent.grabbable = true;
        physPropComponent.customGrabTooltip = __instance.enemyType.enemyName;
        physPropComponent.parentObject = physPropComponent.gameObject.transform;
        physPropComponent.mainObjectRenderer = physPropComponent.gameObject.GetComponent<MeshRenderer>();
        physPropComponent.enabled = true;
        physPropComponent.itemProperties = ScriptableObject.CreateInstance<Item>();
        physPropComponent.itemProperties.allowDroppingAheadOfPlayer = true;
        physPropComponent.itemProperties.creditsWorth = 0;
        physPropComponent.itemProperties.isScrap = true;
        physPropComponent.itemProperties.itemSpawnsOnGround = false;
        physPropComponent.itemProperties.twoHanded = true;
        physPropComponent.itemProperties.toolTips = ["A mob.", "It can be sold"];
        physPropComponent.itemProperties.spawnPrefab = __instance.enemyType.enemyPrefab;
        physPropComponent.itemProperties.minValue = enemyData.MinValue;
        physPropComponent.itemProperties.maxValue = enemyData.MaxValue;
        physPropComponent.itemProperties.weight = enemyData.Mass;
        Plugin.logger.LogInfo("Mob should now be grabbable.");
    }
}
