using EnhancedMonsters.Utils;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(EnemyAI))]
public class EnemyAI_Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Start))]
    private static void Start(EnemyAI __instance)
    {
        if (__instance.enemyType is null)
            return;

        string creatureRank;
        if(LocalConfig.Singleton.synchronizeRanks.Value)
            creatureRank = SyncedConfig.Instance.EnemiesData[__instance.enemyType.enemyName].Rank ?? "?";
        else
            creatureRank = SyncedConfig.Default.EnemiesData[__instance.enemyType.enemyName].Rank ?? "?";

        var scanNode = __instance.gameObject.GetComponentInChildren<ScanNodeProperties>();
        if(scanNode)
        {
            scanNode.subText = $"Rank {creatureRank}";
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.KillEnemy))]
    private static void KillEnemy(EnemyAI __instance, bool destroy)
    {
        if (__instance == null) return;  // Should never happen

        if (__instance.enemyType == null) return; // This should never happen but some mods have this issue.

        Plugin.logger.LogDebug($"Mob {__instance.enemyType.enemyName} died.");

        if (destroy) return;

        if (!__instance.isEnemyDead)
        {
            __instance.isEnemyDead = true;
        }

        if (!SyncedConfig.Instance.EnemiesData.ContainsKey(__instance.enemyType.enemyName))
        {
            EnemiesDataManager.RegisterEnemy(__instance.enemyType.enemyName, new(true));
            Plugin.logger.LogInfo($"Mob was not registered. Registered it with name '{__instance.enemyType.enemyName}'");
        }

        if (NetworkManager.Singleton.IsServer)
        {
            Plugin.logger.LogDebug($"Spawning EnemyScrap for enemy {__instance.enemyType.enemyName}.");
            if (!EnemiesDataManager.Enemies2Props.TryGetValue(__instance.enemyType.enemyName, out var enemy2prop))
            {
                Plugin.logger.LogWarning($"Mob {__instance.enemyType.enemyName} has no enemy2prop prefab.");
                return;
            }

            var enemyToPropInstance = NetworkManager.Instantiate(enemy2prop);
            enemyToPropInstance.hideFlags = HideFlags.None;
            enemyToPropInstance.transform.position = __instance.transform.position;
            enemyToPropInstance.GetComponent<NetworkObject>().Spawn();

            if (__instance.GetComponent<NutcrackerEnemyAI>() != null && __instance.GetComponent<NutcrackerEnemyAI>().gun != null) 
            {
                int shotgunPrice = __instance.GetComponent<NutcrackerEnemyAI>().gun.scrapValue;
                __instance.GetComponent<NutcrackerEnemyAI>().gun.GetComponent<NetworkObject>().Despawn();
                Item shotgunItem = StartOfRound.Instance.allItemsList.itemsList.First(i => i.name == "Shotgun");
                GameObject newShotgun = Object.Instantiate(shotgunItem.spawnPrefab, __instance.transform.position, __instance.transform.rotation);
                newShotgun.GetComponent<ShotgunItem>().scrapValue = shotgunPrice;
                newShotgun.GetComponent<NetworkObject>().Spawn();
                StartCoroutine(SendDetailsWithDelay(shotgunPrice, new NetworkObjectReference(newShotgun.GetComponent<NetworkObject>())));
            }
        }
        
        //move the original body away for all players
        __instance.transform.position = new(-10000, -10000, -10000);
        __instance.SyncPositionToClients();
        Plugin.logger.LogDebug("Mob should now be grabbable for all users.");
    }

    private IEnumerator SendDetailsWithDelay(int price, NetworkObjectReference netRef)
    {
        yield return new WaitForSeconds(0.25f);
        SyncDetailsClientRpc(price, netRef);
    }
    
    [ClientRpc]
    void SyncDetailsClientRpc(int price, NetworkObjectReference netRef)
    {
        NetworkObject networkObject;
        netRef.TryGet(out networkObject, null);
        ShotgunItem shotgun = networkObject.GetComponent<ShotgunItem>();
        if (shotgun != null)
        {
            shotgun.scrapValue = price;
            shotgun.itemProperties.creditsWorth = price;
            shotgun.GetComponentInChildren<ScanNodeProperties>().subText = $"Value: ${price}";
            Debug.Log("Successfully synced shotgun values");
        }
        else Debug.LogError("Failed to resolve network reference!");
    }
}
