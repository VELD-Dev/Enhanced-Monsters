using System;
using System.Collections.Generic;
using System.Text;

namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(RoundManager))]
public static class RoundManager_Patches
{
    [HarmonyPatch()]
    public static void RoundStart()
    {
        var enemies = Resources.FindObjectsOfTypeAll<EnemyAI>();
        foreach(var enemy in enemies)
        {
            EnemiesDataManager.FixupEnemyNetworkBehaviours(enemy);
        }
        EnemiesDataManager.SaveEnemiesData();
    }
}
