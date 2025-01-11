using System;
using System.Collections.Generic;
using System.Text;

namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
internal class GameNetworkManager_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch("StartDisconnect")]
    public static void StartDisconnect()
    {
        SyncedConfig.RevertSync();
    }
}
