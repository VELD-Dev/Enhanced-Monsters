namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(EnemyAI))]
public class EnemyAI_Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Start))]
    private static void Start(EnemyAI __instance)
    {
        Plugin.logger.LogDebug($"Mob {__instance.enemyType.enemyName} spawned, assigning rank.");
        string creatureRank = EnemiesDataManager.EnemiesData[__instance.enemyType.enemyName].Rank ?? "?";
        var scanData = __instance.gameObject.transform.Find("ScanNode").gameObject.EnsureComponent<ScanNodeProperties>();
        scanData.subText = $"Rank {creatureRank}";
        Plugin.logger.LogDebug($"Mob rank assigned. Rank: {creatureRank}");
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.KillEnemy))]
    private static void KillEnemy(EnemyAI __instance, bool destroy)
    {
        if (__instance == null) return;  // Should never happen

        Plugin.logger.LogInfo($"Mob {__instance.enemyType.enemyName} died.");

        if (destroy) return;

        if (!__instance.IsHost || !__instance.IsServer) return;

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
        int mobValue = RoundManager.Instance.AnomalyRandom.Next(enemyData.MinValue, enemyData.MaxValue);

        __instance.meshRenderers[0].GetComponent<Collider>().enabled = true;
        var physPropComponent = __instance.gameObject.EnsureComponent<PhysicsProp>();
        var scanNode = __instance.gameObject.transform.Find("ScanNode").gameObject.EnsureComponent<ScanNodeProperties>();

        Plugin.logger.LogInfo("Added GrabbableObject component to mob. Now setting it up.");
        physPropComponent.grabbable = true;
        physPropComponent.customGrabTooltip = __instance.enemyType.enemyName;
        physPropComponent.scrapValue = mobValue;
        physPropComponent.propColliders =
        [
            scanNode.gameObject.GetComponent<Collider>(),
            physPropComponent.gameObject.GetComponentInChildren<MeshRenderer>().gameObject.GetComponent<Collider>()
        ];
        physPropComponent.mainObjectRenderer = __instance.meshRenderers[0];
        physPropComponent.enabled = true;
        physPropComponent.itemProperties = ScriptableObject.CreateInstance<Item>();
        physPropComponent.itemProperties.itemId = 0;
        physPropComponent.itemProperties.itemName = __instance.enemyType.enemyName;
        physPropComponent.itemProperties.allowDroppingAheadOfPlayer = true;
        physPropComponent.itemProperties.creditsWorth = 0;
        physPropComponent.itemProperties.grabAnim = "HoldLung";
        physPropComponent.itemProperties.isScrap = true;
        physPropComponent.itemProperties.itemSpawnsOnGround = false;
        physPropComponent.itemProperties.twoHanded = true;
        physPropComponent.itemProperties.twoHandedAnimation = true;
        physPropComponent.itemProperties.spawnPrefab = __instance.enemyType.enemyPrefab;
        physPropComponent.itemProperties.minValue = enemyData.MinValue;
        physPropComponent.itemProperties.maxValue = enemyData.MaxValue;
        physPropComponent.itemProperties.weight = enemyData.Mass / 100f;

        __instance.gameObject.transform.Find("ScanNode").gameObject.GetComponent<Collider>().enabled = true;
        scanNode.scrapValue = physPropComponent.scrapValue;
        scanNode.subText = $"Value: ${scanNode.scrapValue}";
        scanNode.maxRange = 10;
        scanNode.minRange = 1;
        //scanNode.nodeType = (int)NodeType.Normal;
        scanNode.requiresLineOfSight = true;
        Plugin.logger.LogInfo("Mob should now be grabbable.");
    }
}
