using System;
using System.Collections.Generic;
using System.Text;

namespace EnhancedMonsters.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRound_Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void StartPatch()
        {
            var enemies = Resources.FindObjectsOfTypeAll<EnemyAI>();

            foreach (var enemy in enemies)
            {
                if (enemy is null)
                {
                    Plugin.logger.LogWarning("An enemy is null!");
                    continue;
                }

                if (enemy.enemyType is null)
                {
                    Plugin.logger.LogWarning($"{enemy.name} has a null enemyType (tf?)");
                    continue;
                }

                if (!SyncedConfig.Instance.EnemiesData.ContainsKey(enemy.enemyType.enemyName))
                {
                    EnemiesDataManager.RegisterEnemy(enemy.enemyType.enemyName, new());
                    Plugin.logger.LogInfo($"Mob was not registered. Registered it with name '{enemy.enemyType.enemyName}'");
                }
            }

            EnemiesDataManager.SaveEnemiesData();
            EnemiesDataManager.EnsureEnemy2PropPrefabs();
        }
    }
}