# :trident: Enhanced Monsters
![Source downloads](https://img.shields.io/github/downloads/veld-dev/LootableMonsters/total) ![Latest release](https://img.shields.io/github/v/release/veld-dev/LootableMonsters)   
Lethal Company mod allowing to pick enemies as loots, and some enemies to loot some stuff.

### Features
- Allows you to sell dead mobs, even modded ones !
- All "dangerous" mobs display their rank (dangerousness) when you scan them.
- Configurable mobs minimum and maximum values, mass, wether it can be sold or not, and mob's rank.
- `[WIP]` - Masked mimics drop their mask when they die.
- `[WIP]` - See and hear what's happening inside the facility from the door's windows

I must still think about more features in fact.

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