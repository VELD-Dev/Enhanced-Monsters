﻿using System.Security.Cryptography;
using System.Xml.Linq;
using UnityEngine;

namespace EnhancedMonsters.Utils;

public static class EnemiesDataManager
{
    [JsonObject]
    [method: JsonConstructor]
    public class EnemyDataFile(int version, Dictionary<string, EnemyData> enemiesData)
    {
        public int Version { get; set; } = version;
        public Dictionary<string, EnemyData> EnemiesData { get; set; } = enemiesData ?? [];
    }

    public static readonly Dictionary<string, EnemyData> EnemiesData = [];

#pragma warning disable IDE0055  // This was really annoying
    public static readonly Dictionary<string, EnemyData> DefaultEnemiesData = new()
    {
        // Lootable
        ["Manticoil"]           = new EnemyData(true, 20, 30, 10, "F", new(new(0, 0, 0), new(0, 0, 0), true)),
        ["Tulip Snake"]         = new EnemyData(true, 10, 20, 11, "F", new(new(0, 0, 0), new(0, 0, 0), true)),
        ["Hoarding bug"]        = new EnemyData(true, 30, 60, 24, "E", new(new(0, 1, 0), new(0, 0, 0), true)),
        ["Puffer"]              = new EnemyData(true, 30, 60, 69, "E", new(new(0, 0, 0), new(0, 0, 0), true)),
        ["Centipede"]           = new EnemyData(true, 25, 40, 23, "D", new(new(0, 0, 0), new(0, 0, 0), true)),
        ["Baboon hawk"]         = new EnemyData(true, 40, 70, 31, "D", new(new(0, 0, 0), new(0, 0, 0), true)),
        ["Bunker Spider"]       = new EnemyData(true, 90, 120, 57, "C", new(new(0, 0, 0), new(0, 0, 0), true)),
        ["MouthDog"]            = new EnemyData(true, 110, 140, 88, "C", new(new(0, 0, 0), new(0, 0, 0),  true)),
        ["Crawler"]             = new EnemyData(true, 150, 180, 66, "B", new(new(0, 0, 0), new(0, 0, 0), true)),
        ["Flowerman"]           = new EnemyData(true, 162, 190, 40, "B", new(new(0, 0, 0), new(0, 90, 0), false)),
        ["Butler"]              = new EnemyData(true, 170, 199, 77, "B", new(new(0, 0, 0), new(0, 90, 0), false)),
        ["Nutcracker"]          = new EnemyData(true, 190, 220, 43, "A", new(new(0, 0, 0), new(0, 90, 0), false)),
        ["Maneater"]            = new EnemyData(true, 250, 290, 42, "S", new(new(0, 0, 0), new(0, 0, 0), true)),


        // Invincible
        ["Docile Locust Bees"]  = new EnemyData(false, 0, 0, 0, "F", new()),
        ["Red pill"]            = new EnemyData(false, 0, 0, 0, "F", new()),
        ["Blob"]                = new EnemyData(false, 0, 0, 0, "D", new()),
        ["Red Locust Bees"]     = new EnemyData(false, 0, 0, 0, "C", new()),
        ["Butler Bees"]         = new EnemyData(false, 0, 0, 0, "C", new()),
        ["Earth Leviathan"]     = new EnemyData(false, 0, 0, 0, "B", new()),
        ["Masked"]              = new EnemyData(false, 0, 0, 0, "B", new()),
        ["Clay Surgeon"]        = new EnemyData(false, 0, 0, 0, "B", new()),
        ["Spring"]              = new EnemyData(false, 0, 0, 0, "A", new()), // Coilhead
        ["Jester"]              = new EnemyData(false, 0, 0, 0, "S+", new()),
        ["RadMech"]             = new EnemyData(false, 0, 0, 0, "S+", new()),
        ["Girl"]                = new EnemyData(false, 0, 0, 0, "?", new()),
        ["Lasso"]               = new EnemyData(false, 0, 0, 0, "dont exist haha", new()),

        // Unsellable
        ["ForestGiant"]         = new EnemyData(false, 0, 0, 0, "S", new(new(0, 0, 0), new(0, 90, 0), false)), // This one is just too big lmao

        // MODDED
        ["PinkGiant"]           = new EnemyData(false, 0, 0, 0, "F", new(new(0, 0, 0), new(0, 90, 0), false)), // Too big too to be sold
        ["Football"]            = new EnemyData(false, 0, 0, 0, "B", new()),
        ["Locker"]              = new EnemyData(false, 0, 0, 0, "A", new()),
        ["Bush Wolf"]           = new EnemyData(true, 250, 320, 51, "A", new(new(0, 0, 0), new(0, 0, 0), true)),
        ["PjonkGoose"]          = new EnemyData(true, 279, 340, 64, "A", new(new(0, 0, 0), new(0, 0, 0), true)),

    };
#pragma warning restore IDE0055

    public static string EnemiesDataFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "config", "EnhancedMonsters", "EnemiesData.json");
    public static readonly Dictionary<string, GameObject> Enemies2Props = [];

    public static void LoadEnemiesData()
    {
        if (!File.Exists(EnemiesDataFile))
        {
            Plugin.logger.LogWarning("Enemy Data File did not exist!");
            EnemiesData.ProperConcat(DefaultEnemiesData);
            SaveEnemiesData();
            return;
        }

        var filetext = File.ReadAllText(EnemiesDataFile);
        Plugin.logger.LogDebug(filetext);
        var parsed = JsonConvert.DeserializeObject<EnemyDataFile>(filetext);
        if (parsed is null)
        {
            Plugin.logger.LogWarning("Enemy Data File seems to be empty or invalid.");
            EnemiesData.ProperConcat(DefaultEnemiesData);
            SaveEnemiesData();
            return;
        }

        if((PluginInfo.ConfigVersion - parsed.Version) > 1)
        {
            Plugin.logger.LogWarning("Enemy Data File seems to be outdated. A new config file will be created.");
            EnemiesData.ProperConcat(DefaultEnemiesData);
            SaveEnemiesData();
            return;
        }

        EnemiesData.ProperConcat(parsed.EnemiesData);
        Plugin.logger.LogDebug($"{parsed} => {EnemiesData.Count}");
        EnemiesData.ProperConcat(DefaultEnemiesData);
        Plugin.logger.LogDebug($"{DefaultEnemiesData.Count} => {EnemiesData.Count}");

        SaveEnemiesData();
    }

    /// <summary>
    /// Allows external mods to add their own enemy and its stats through a simple interface which they can soft reference easily.
    /// </summary>
    /// <param name="enemyName">Enemy Name found in <see cref="EnemyType.enemyName"/></param>
    /// <param name="sellable">Wether the enemy is sellable or not. Giant enemies and unkillable enemies should be set to false.</param>
    /// <param name="minPrice">Minimum price of the entity</param>
    /// <param name="maxPrice">Maximum price of the entity</param>
    /// <param name="mass">Mass (in lb) of the entity</param>
    /// <param name="rank">Rank of the entity shown in the holo-display when scanned. Do not line-break, make it as short as possible.</param>
    /// <param name="metadata">Advanced metadata of the entity.</param>
    /// <param name="overrideRegister">Wether to override any existing entry or only register it if wasn't already. You better not override unless you're testing your mod.</param>
    public static void RegisterEnemy(string enemyName, bool sellable, int minPrice, int maxPrice, float mass, string rank, EnemyData.EnemyMetadata metadata, bool overrideRegister = false)
    {
        if (!EnemiesData.ContainsKey(enemyName))
        {
            EnemiesData.Add(enemyName, new(sellable, minPrice, maxPrice, mass, rank, metadata));
        }
        else
        {
            if(overrideRegister)
            {
                EnemiesData[enemyName] = new(sellable, minPrice, maxPrice, mass, rank, metadata);
            }
            else
            {
                var modName = Assembly.GetCallingAssembly().GetName().Name;
                Plugin.logger.LogWarning($"Cannot register modded from mod {modName} enemy {enemyName}: It is already registered (either from file or from .");
                return;
            }
        }

        if (SyncedConfig.Instance != null)
            if (!SyncedConfig.IsHost && NetworkManager.Singleton.IsListening)
                return;

        SyncedConfig.BroadcastSync();
    }

    internal static void RegisterEnemy(string enemyName, EnemyData enemyData)
    {
        if (!SyncedConfig.IsHost && NetworkManager.Singleton.IsListening) return;

        if(!ReferenceEquals(EnemiesData, SyncedConfig.Instance.EnemiesData))
        {
            Plugin.logger.LogError("Cannot update the mob configs. Somehow, the EnemiesData from the EnemiesDataManager and the one from the SyncedConfigs are not the same, even though they both are yours.");
            return;
        }

        if (!EnemiesData.TryAdd(enemyName, enemyData))
        {
            Plugin.logger.LogDebug($"EnemyData '{enemyName}' already exists!");
            return;
        }

        // Will tell everyone to synchronize with user's sync if he's the host, since there's a new mob that was registered.
        SyncedConfig.BroadcastSync();
    }

    public static void SaveEnemiesData()
    {
        if (!SyncedConfig.IsHost && (NetworkManager.Singleton?.IsListening ?? false)) return;

        var newFile = new EnemyDataFile(PluginInfo.ConfigVersion, EnemiesData);

        var output = JsonConvert.SerializeObject(newFile, Newtonsoft.Json.Formatting.Indented);
        //Plugin.logger.LogDebug(output);
        //Plugin.logger.LogDebug(EnemiesDataFile);
        if (!Directory.Exists(Path.GetDirectoryName(EnemiesDataFile)))
            Directory.CreateDirectory(Path.GetDirectoryName(EnemiesDataFile));
        File.WriteAllText(EnemiesDataFile, output);
        Plugin.logger.LogInfo("Saved enemies data.");
    }

    public static void EnsureEnemy2PropPrefabs()
    {
        var enemies = Resources.FindObjectsOfTypeAll<EnemyAI>();
        foreach(var enemy in enemies)
        {
            Plugin.logger.LogInfo($"Registering NetworkPrefab '{enemy.enemyType.enemyName}'");
            ref var enemyName = ref enemy.enemyType.enemyName;
            if (Enemies2Props.TryGetValue(enemy.enemyType.enemyName, out var e2p))
            {
                if (!SyncedConfig.Instance.EnemiesData[enemyName].Pickupable)
                {
                    Enemies2Props.Remove(enemyName);
                    continue;
                }

                var pp = e2p.GetComponent<EnemyScrap>();
                pp.enemyType = enemy.enemyType;
                pp.itemProperties.minValue = SyncedConfig.Instance.EnemiesData[enemyName].MinValue;
                pp.itemProperties.maxValue = SyncedConfig.Instance.EnemiesData[enemyName].MaxValue;
                pp.itemProperties.weight = SyncedConfig.Instance.EnemiesData[enemyName].LCMass;

                continue;
            }

            if (!SyncedConfig.Instance.EnemiesData.TryGetValue(enemy.enemyType.enemyName, out var enemyData) || enemyData.Pickupable == false) continue;

            var copy = new GameObject(enemy.name + " neutralized");
            foreach(Transform c in enemy.transform)
            {
                if (c.name.StartsWith("MapDot")) continue;
                if (c.name.StartsWith("Collider")) continue;
                if (c.name.StartsWith("VoiceSFX")) continue;
                if (c.name.StartsWith("CreatureSFX")) continue;
                if (c.name.StartsWith("SeepingSFX")) continue;
                if (c.name.StartsWith("CreatureVoice")) continue;
                if (c.name.StartsWith("Ambience")) continue;

                var goCopy = GameObject.Instantiate(c);
                goCopy.name = c.name;
                goCopy.transform.parent = copy.transform;
            }
            // Clearing components that should not be
            copy.RemoveComponentsInChildren<Collider>();
            copy.RemoveComponentsInChildren<OccludeAudio>();
            copy.RemoveComponentsInChildren<EnemyAICollisionDetect>();
            copy.RemoveComponentsInChildren<AudioSource>();
            copy.RemoveComponentsInChildren<AudioLowPassFilter>();
            copy.RemoveComponentsInChildren<AudioReverbFilter>();
            copy.RemoveComponentsInChildren<ParticleSystem>();
            copy.RemoveComponentsInChildren<ParticleSystemRenderer>();

            copy.transform.localScale = enemy.transform.localScale;
            var e2prop = LethalLib.Modules.NetworkPrefabs.CloneNetworkPrefab(Plugin.EnemyToPropPrefab, enemy.name + " propized");
            copy.transform.parent = e2prop.transform;
            Plugin.logger.LogInfo($"Attached {copy.name} to {copy.transform.parent.name}");
            var enemyScrap = e2prop.GetComponent<EnemyScrap>();
            enemyScrap.grabbable = true;
            enemyScrap.grabbableToEnemies = false;
            enemyScrap.enemyType = enemy.enemyType;
            enemyScrap.EnemyGameObject = copy;

            // It should always exist on a pickupable mob, otherwise it means that the enemy is client-side and is not networked, so it cant be sold.
            var scanNodeProperties = copy.GetComponentInChildren<ScanNodeProperties>();
            if (!scanNodeProperties)
                continue;

            var scanNode = scanNodeProperties.gameObject.transform;
            scanNode.transform.parent = e2prop.transform;
            scanNode.gameObject.AddComponent<BoxCollider>();
            scanNode.localPosition = new(0, 0, 0);
            scanNodeProperties.maxRange = 13;
            scanNodeProperties.minRange = 1;
            scanNodeProperties.nodeType = 2;
            scanNodeProperties.requiresLineOfSight = true;
            scanNodeProperties.headerText = "Dead " + scanNodeProperties.headerText;

            var enemyItem = ScriptableObject.CreateInstance<Item>();
            enemyScrap.itemProperties = enemyItem;

            enemyItem.name = enemyName + " scrap";
            enemyItem.itemName = scanNodeProperties.headerText;
            enemyItem.minValue = SyncedConfig.Instance.EnemiesData[enemyName].MinValue;
            enemyItem.maxValue = SyncedConfig.Instance.EnemiesData[enemyName].MaxValue;
            enemyItem.allowDroppingAheadOfPlayer = true;
            enemyItem.canBeGrabbedBeforeGameStart = true;
            enemyItem.isScrap = SyncedConfig.Instance.EnemiesData[enemyName].Pickupable;
            enemyItem.itemSpawnsOnGround = false;
            enemyItem.twoHanded = true;
            enemyItem.requiresBattery = false;
            enemyItem.twoHandedAnimation = true;
            enemyItem.weight = SyncedConfig.Instance.EnemiesData[enemyName].LCMass;
            enemyItem.spawnPrefab = e2prop;
            enemyItem.restingRotation = SyncedConfig.Instance.EnemiesData[enemyName].Metadata.MeshRotation.ToUnityVec() * ((float)Math.PI / 180f);
            enemyItem.positionOffset = SyncedConfig.Instance.EnemiesData[enemyName].Metadata.MeshOffset.ToUnityVec();

            LethalLib.Modules.Items.RegisterItem(enemyItem);
            Items.RegisterScrap(enemyItem, 0, Levels.LevelTypes.None);
            Enemies2Props.Add(enemyName, e2prop);
            Plugin.logger.LogInfo($"Registered NetworkPrefab '{e2prop.name}'/'{copy.name}' ({enemyItem.itemName})");
        }
    }
}
