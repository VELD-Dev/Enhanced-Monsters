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
        if (__instance == null) return;

        if (destroy) return;

        if (!__instance.isEnemyDead)
        {
            __instance.isEnemyDead = true;
        }

        if(!EnemiesValueManager.EnemiesData.ContainsKey(__instance.enemyType.enemyName))
        {
            EnemiesValueManager.RegisterEnemy();
        }

        var grabbableGO = __instance.gameObject.AddComponent<GrabbableObject>();
        grabbableGO.grabbable = true;
        grabbableGO.customGrabTooltip = __instance.enemyType.enemyName;
        grabbableGO.parentObject = grabbableGO.gameObject.transform;
        grabbableGO.mainObjectRenderer = grabbableGO.gameObject.GetComponent<MeshRenderer>();
        grabbableGO.enabled = true;
        grabbableGO.itemProperties = new()
        {
            allowDroppingAheadOfPlayer = true,
            creditsWorth = 100,
        };
    }
}
