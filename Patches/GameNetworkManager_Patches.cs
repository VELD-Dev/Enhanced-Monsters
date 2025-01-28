using System;
using System.Collections.Generic;
using System.Text;

namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
internal class GameNetworkManager_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    public static void Start(GameNetworkManager __instance)
    {
        var enemies = Resources.FindObjectsOfTypeAll<EnemyAI>();
        foreach (var enemy in enemies)
        {
            if (!SyncedConfig.Instance.EnemiesData.ContainsKey(enemy.enemyType.enemyName))
            {
                EnemiesDataManager.RegisterEnemy(enemy.enemyType.enemyName, new());
                Plugin.logger.LogInfo($"Mob was not registered. Registered it with name '{enemy.enemyType.enemyName}'");
            }
        }
        EnemiesDataManager.SaveEnemiesData();

        EnemiesDataManager.EnsureEnemy2PropPrefabs();

        /*
        foreach(var corpse in EnemiesDataManager.Enemies2Props.Values)
        {
            var eScrap = corpse.GetComponent<EnemyScrap>();
            if (StartOfRound.Instance.allItemsList.itemsList.Contains(eScrap.itemProperties))
                continue;

            Plugin.logger.LogInfo("Registering ItemData to AllItemList because because LethalLib was too slow.");
            // Not doing this may provoke a soft race error, item data would be missing and items would be skipped from the save loading.

            StartOfRound.Instance.allItemsList.itemsList.Add(eScrap.itemProperties);
            Items.LethalLibItemList.Add(eScrap.itemProperties);
        }
        */
    }

    [HarmonyPostfix]
    [HarmonyPatch("StartDisconnect")]
    public static void StartDisconnect()
    {
        SyncedConfig.RevertSync();
    }

    [HarmonyPostfix]
    [HarmonyPatch("SaveItemsInShip")]
    public static void SaveItemsInShip(GameNetworkManager __instance)
    {
        var objectsInShip = UnityEngine.Object.FindObjectsByType<GrabbableObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Physics.OverlapBox(StartOfRound.Instance.shipBounds.bounds.center, StartOfRound.Instance.shipBounds.bounds.size / 2f, StartOfRound.Instance.shipBounds.transform.rotation, 6, QueryTriggerInteraction.Collide);
        for(int i = 0; i < objectsInShip.Length && i <= StartOfRound.Instance.maxShipItemCapacity; i++)
        {
            var obj = objectsInShip[i];
            Plugin.logger.LogInfo($"SAVE CHECKER: Checking object {obj.itemProperties.itemName}...");
            if(StartOfRound.Instance.allItemsList.itemsList.Contains(obj.itemProperties) && !obj.deactivated)
            {
                if(obj.itemProperties.spawnPrefab == null)
                {
                    Plugin.logger.LogError($"SAVE CHECKER: Object {obj.itemProperties.itemName} didn't have a spawn prefab.");
                    continue;
                }
                if(obj.itemUsedUp)
                {
                    Plugin.logger.LogError($"SAVE CHECKER: Object {obj.itemProperties.itemName} didn't save because it was used up.");
                    continue;
                }

                if(obj.itemProperties.isScrap)
                {
                    Plugin.logger.LogInfo($"SAVE CHECKER: Saving object {obj.itemProperties.itemName} scrap value: ${obj.scrapValue}");
                }

                Plugin.logger.LogInfo($"SAVE CHECKER: Saving object {obj.itemProperties.itemName}");
            }
            else
            {
                Plugin.logger.LogInfo($"SAVE CHECKER: Not saving {obj.itemProperties.itemName}. Deactivated: {obj.deactivated} - Registered correctly: {StartOfRound.Instance.allItemsList.itemsList.Contains(obj.itemProperties)}");
            }
        }
    }
}
