using System;
using System.Collections.Generic;
using System.Text;

namespace LootableMonsters.Patches;

[HarmonyPatch(typeof(EnemyAI))]
internal class EnemyAI_KillEnemy_Postfix
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.KillEnemy))]
    private static void KillEnemy(EnemyAI __instance, bool destroy)
    {
        Plugin.logger.LogInfo($"Mob {__instance.enemyType.enemyName} died. Trying to make it grabbable.");
        if (__instance == null) return;  // Should never happen

        if (destroy) return;

        Plugin.logger.LogInfo("Mob was not destroyed. Now making it grabbable.");

        if (!__instance.isEnemyDead)
        {
            __instance.isEnemyDead = true;
        }

        if(!EnemiesValueManager.EnemiesData.ContainsKey(__instance.enemyType.enemyName))
        {
            EnemiesValueManager.RegisterEnemy(__instance.enemyType.enemyName, new());
            Plugin.logger.LogInfo($"Mob was not registered. Registered it with name '{__instance.enemyType.enemyName}'");
        }

        var enemyData = EnemiesValueManager.EnemiesData[__instance.enemyType.enemyName];

        var grabbableGO = __instance.gameObject.AddComponent<GrabbableObject>();
        Plugin.logger.LogInfo("Added GrabbableObject component to mob. Now setting it up.");
        grabbableGO.grabbable = true;
        grabbableGO.customGrabTooltip = __instance.enemyType.enemyName;
        grabbableGO.parentObject = grabbableGO.gameObject.transform;
        grabbableGO.mainObjectRenderer = grabbableGO.gameObject.GetComponent<MeshRenderer>();
        grabbableGO.enabled = true;
        grabbableGO.itemProperties = new()
        {
            allowDroppingAheadOfPlayer = true,
            creditsWorth = 0,
            isScrap = true,
            itemSpawnsOnGround = false,
            twoHanded = true,
            toolTips = ["A mob.", "It can be sold"],
            spawnPrefab = __instance.enemyType.enemyPrefab,
            minValue = enemyData.MinValue,
            maxValue = enemyData.MaxValue,
            weight = enemyData.Mass,
        };
        Plugin.logger.LogInfo("Mob should now be grabbable.");
    }
}
