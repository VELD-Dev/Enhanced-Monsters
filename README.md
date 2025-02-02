# :trident: Enhanced Monsters
![Latest release](https://img.shields.io/github/v/release/veld-dev/LootableMonsters) ![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/VELD/Enhanced_Monsters?logo=thunderstore&color=%2323FFB0) ![Source downloads](https://img.shields.io/github/downloads/veld-dev/LootableMonsters/total?logo=github)  
A mod aiming at enhancing gameplay with monsters, such as allowing to sell dead monsters, make masked/mimics return their mask once dead,
Nutcrackers to drop their shotgun, and also adds a ranking mechanic to mobs.

## Features
- Allows you to sell dead mobs, even modded ones! If it can die, it can be sold. Technically.
- All mobs will display a rank based on their dangerousness when you scan them. Their rank is synchronized between players from host's configuration.
- Configurable mobs minimum and maximum values, mass, wether it can be sold or not, and mob's rank.
- `[WIP]` - Nutcrackers have a chance to drop their shotgun when they die.
- `[WIP]` - Masked mimics drop their mask when they die.
- `[WIP]` - See and hear what's happening inside the facility from the door's windows.

I must still think about more features in fact.

## Verified Compatibilities
Here are a few mods I have personally verified the compatibility with.
- `LethalConfig`: I made sure that LethalConfig works fine to configure the mod's local settings. It's an optional mod.
- `LethalSettings`: I made sure that LethalSettings works fine too, it's just so that you can use whatever mods you've installed instead of downloading yet another config mod.
- `Football` (Kittenji): Football has a rank, I haven't verified if she has a ScanNode though, so maybe you won't even be able to scan her but yeah, should work anyway.
- `Giant Species` (XuXiaolan): I'm trying to make a proper support for Giant Species mod, with ranks and eventually, for the smallest of them, prices and values.

## How to edit mobs settings ?
No matter which way you do it, **do never modify the `Version` field, it is made to verify enemies data compatibility between versions of the mod**.
### With R2ModMan
- :warning: **You need to launch the game at least once, in order to generate the config list.**
- Into r2modman, head into `Settings`, then `Locations` and click `Browse profile folder`.
- Reach `/BepInEx/plugins/EnhancedMonsters/`
- Double-click `EnemiesData.json`
- For more efficient search, you can press `[CTRL]`+`[F]` in order to open the word search. Type the name of the mob you want to edit.
- Edit the values. be careful, you need to respect the types given:
  - `MinValue`, `MaxValue` must be integers (integral numbers, without decimals).
  - `Mass` must be a floating point number (number with decimals) and it is in pounds (lb).
  - `Pickupable` must remain a boolean, it can only have two values: `true` or `false`.
  - `Rank` is a string, it has no length restriction but it's better to keep it short.
- Don't forget to save! **You need to relaunch the game for these settings to be applied**.

### For Warriors (manual installers yarrrgh)
- :warning: **You need to launch the game at least once, in order to generate the config list.**
- If you install mods, you technically know where you install them. Go to `/BepInEx/plugins/EnhancedMonsters/`
- Open `EnemiesData.json`
- Search a mob by its name with `[CTRL]`+`[F]`
- Modify its values by respecting the information given in the R2ModMan notice
- **You need to relaunch the game in order to apply the settings**.

## Q&A
#### Q: Is it possible to make the mod client-side only so I can use only the ranking with my vanilla friends ?
> A: *I will probably do it, but it's far from being a priority.*
---
#### Q: Is this mod compatible with **SellBodies** or **SellBodiesFixed** ?
> A: *No.*
---
#### Q: Is this mod compatible with **TakeThatMaskOff** ?
> A: *For now, it is, but when the feature will be added, it won't.*
---
#### Q: If I have a mod allowing to kill all enemies, will all of them be sellable
> A: *Yes but there are prerequisites for this. You need to go into the `EnemiesData.json` settings file and set every mob's `Pickupable` boolean value to `true`. Don't forget to add them values.*
---
#### Q: Why do some of the modded monsters of my modpack aren't grabbable after killing them ?
> A: *By default, any mob that doesn't have default data is not "pickupable". You need to access the EnemyData config file and set the modded mob's values and wether it's pickupable or not. After that you need to restart the game. Note that some mods do register default data for their mobs so it is not required with every modded mob.*
---
#### Q: I have encountered a bug, how do I report it ?
> A: *You can report it in the [**GitHub issues**](https://github.com/VELD-Dev/Enhanced-Monsters/issues) page, or join the [**Lethal Company Modding**](https://discord.gg/4AuKMbYVys) Discord Server and go to the [**Enhanced Monsters Topic**](https://discord.com/channels/1168655651455639582/1212448321017483275) inside the **`#mod-releases`** forum*

## For modders
### Make Enhanced Monsters a Soft-Dependency

Here's a quick tutorial on how to use EnhancedMonsters without making it mandatory for users of your mods!
- You still need to add a reference to the mod into your `.csproj`.

```cs
// Plugin.cs

[BepInDependency("com.velddev.enhancedmonsters", BepInDependency.DependencyFlags.SoftDependency)]
public class MyPlugin : BaseUnityPlugin
{
    private void Start()
    {
        if(EnhancedMonstersCompatibilityLayer.Enabled)
        {
            EnhancedMonstersCompatibilityLayer.RegisterCustomMonsterEnemyData();
        }
    }
}
```
```cs
// You can do the following part inside your main Plugin class, it just needs a container class.
public static class EnhancedMonstersCompatibilityLayer
{
    private static bool? _enabled;
    public static bool Enabled
    {
        get
        {
            if (_enabled == null)
                _enabled = BepInex.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.velddev.enhancedmonsters");
            
            return (bool)_enabled;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void RegisterCustomMonsterEnemyData()
    {
        // The EnemyName must ABSOLUTELY be the same than inside your EnemyType scriptable object!
        EnhancedMonster.Utils.EnemiesDataManager.RegisterEnemy(EnemyType.enemyName, /*is enemy sellable ?*/ true, /*min value:*/ 150, /*max value:*/ 200, /*mass:*/ 14, /*rank:*/ "S+");
        // ...
    }
}
```

---

### Attributions
- **IAmBatby** (LethalExpansion, LethalToolbox) for helping me with the networking and with my prefab generation system (for the mobs scraps)
- **Xilophor** (LethalNetworkAPI, StaticNetcode) for helping me with the networking
- **Zagster** (OpenBodyCams) for helping me with the networking and various hints
- **FROG** for helping me with the networking
- **Xu Xiaolan** (Giant Species) for various help
- **Moroxide** (Lethal Resonance collaborator) for creating a enemy dead body drop sound, and helping me testing the mod
- **DropDaDeuce** (LethalGargoyles) for making a fix for soft dependencies.