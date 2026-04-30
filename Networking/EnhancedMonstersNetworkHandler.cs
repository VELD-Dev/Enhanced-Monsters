using EnhancedMonsters.Config;

namespace EnhancedMonsters.Networking;

/// <summary>
/// Singleton NetworkBehaviour that hosts every server-to-client and client-to-server RPC
/// owned by Enhanced Monsters. Spawned server-side by <see cref="Patches.StartOfRound_Patches"/>
/// and replicated to every client via NGO. Late-joining clients receive the existing instance
/// through normal NGO replication.
/// </summary>
public class EnhancedMonstersNetworkHandler : NetworkBehaviour
{
    public static EnhancedMonstersNetworkHandler Instance { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Instance = this;
        if (IsServer)
            ConfigSyncService.Synced = true;
        Plugin.logger.LogInfo($"EnhancedMonstersNetworkHandler spawned ({(IsServer ? "server" : "client")})");
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (Instance == this)
            Instance = null;
        Plugin.logger.LogInfo("EnhancedMonstersNetworkHandler despawned");
    }

    // ----- Config sync RPCs -----

    [ServerRpc(RequireOwnership = false)]
    public void RequestConfigSyncServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer) return;

        ulong senderId = serverRpcParams.Receive.SenderClientId;
        Plugin.logger.LogDebug($"ConfigSync request received from client {senderId}");

        byte[] payload = ConfigSyncService.SerializeCurrent();
        if (payload.Length == 0)
        {
            Plugin.logger.LogError("Aborting sync response: empty payload");
            return;
        }

        var rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { senderId } }
        };
        ReceiveConfigSyncClientRpc(payload, rpcParams);
    }

    [ClientRpc]
    public void ReceiveConfigSyncClientRpc(byte[] data, ClientRpcParams clientRpcParams = default)
    {
        if (IsServer) return;
        Plugin.logger.LogInfo($"Received config sync via ClientRpc, {data?.Length ?? 0} bytes");
        ConfigSyncService.ApplyReceivedSync(data);
    }

    [ClientRpc]
    public void BroadcastConfigSyncClientRpc(byte[] data)
    {
        if (IsServer) return;
        Plugin.logger.LogInfo($"Received config broadcast via ClientRpc, {data?.Length ?? 0} bytes");
        ConfigSyncService.ApplyReceivedSync(data);
    }

    // ----- Enemy body RPCs -----

    /// <summary>
    /// Server-broadcast: the original LC enemy body was teleported off-map after the corpse
    /// scrap was spawned. Clients teleport their local copy to match so the dead enemy is
    /// not visible alongside the new pickupable corpse. The pickupable corpse itself is
    /// already replicated to clients via NGO's <see cref="NetworkObject.Spawn"/> pipeline,
    /// so no RPC is needed for that side.
    /// </summary>
    [ClientRpc]
    public void HideOriginalEnemyBodyClientRpc(NetworkObjectReference originalEnemyRef, Vector3 hidePosition)
    {
        if (IsServer) return;
        if (!originalEnemyRef.TryGet(out var enemyNo))
        {
            Plugin.logger.LogWarning("HideOriginalEnemyBodyClientRpc: could not resolve enemy NetworkObject; skipping client-side teleport");
            return;
        }
        enemyNo.transform.position = hidePosition;
        Plugin.logger.LogDebug($"Hid original enemy body '{enemyNo.gameObject.name}' at {hidePosition}");
    }
}
