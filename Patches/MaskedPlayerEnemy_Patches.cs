namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(MaskedPlayerEnemy))]
public class MaskedPlayerEnemy_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.KillEnemyServerRpc))]
    private static void KillEnemyServerRpc(MaskedPlayerEnemy __instance, bool destroy)
    {
        Plugin.logger.LogInfo("Hey. Spawn a mimic mask. now.");
    }
}
