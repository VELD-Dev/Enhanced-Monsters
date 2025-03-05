# Changelog

All notable changes will be documented in the changelog.  

## [1.3.9](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.3.9) - 2025-03-05

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.3.8..1.3.9)
- Fixed original mob bodies not disappearing correctly on clients but correctly on host, by @Entity378 in pull request https://github.com/VELD-Dev/Enhanced-Monsters/pull/5

## [1.3.8](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.3.8) - 2025-03-03

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.3.7..1.3.8)
- Fixed mod not loading because the config file has bad keys.

## [1.3.7](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.3.7) - 2025-03-03

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.3.6..1.3.7)
- Fixed mobs body duplication occurring in multiplayer when the body is picked was picked too quickly.
- Removed two settings entries as they are not relevant anymore (feature unplanned).
- Also grayed-out two settings that are not relevant as the feature is not implemented yet.

## [1.3.6](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.3.6) - 2025-02-28

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.3.5..1.3.6)
- Fixed config file not being in the right folder.
- Fixed mobs not behaving correctly after they die.

## [1.3.5](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.3.5) - 2025-02-20

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.3.4..1.3.5)
- Fixed Bunker Spiders having both the arachnophobe and standard meshes. Now it will update just like when it's alive, and it WILL NOT REQUIRE A RESTART to change the spider model.
- Fixed "HandRotation" settings not working when in game. It's now possible, through the settings, to set a rotation to corpses when they are in your hands.

## [1.3.4](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.3.4) - 2025-02-20

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.3.3..1.3.4)
- Fixed a rare issue where the killable enemies scrap prefabs wouldn't be generated due to some mobs not having `EnemyType`.

## [1.3.3](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.3.3) - 2025-02-16

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.3.2..1.3.3)
- Fixed an error caused by some mods like ScarletDevilMansion where some mods that didn't have `EnemyType` were causing the mod to break configs and configs synchronization.

## [1.3.2](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.3.2) - 2025-02-08

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.3.1..1.3.2)
- Fixed custom rotations not setting correctly, and splitted *On Floor Rotation* and *In Hand Rotation* in the config.
- Temporarily suspended the custom sounds feature, it will only use the default sounds now.
- Added possibility to set `default` for default sound, or `none` for no sound at all in all the sound configs.
- Added a custom dead enemy item inventory icon, designed by **Dededenied**.
- Added a new setting to set the grab collision box size.

> Note: This update deprecates old configs. Make sure to port your changes from `OLD_EnemiesData.json`.

## [1.3.1](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.3.1) - 2025-02-02

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.3.0..1.3.1)
- Added a config backup system: Deprecated config versions are renamed to `OLD_EnemyData.json` in order to not screw all your modifications on each new version. All you have to do is port your changes to the newer versions of the config.
- Fixed the mod not loading correctly due to the custom sounds loading attempt even if there was none provided.

## [1.3.0](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.3.0) - 2025-02-02

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.2.1..1.3.0)
- Added new enemies metadata configs:
	- `TwoHanded`: Wether the body makes "hand fulls" or not
	- `GrabSFX`: Allows you to use a custom sound as the body grab SFX
	- `DropSFX`: Allows you to use a custom sound as the body drop SFX
	- `PocketSFX`: Allows you to use a custom sound as the body pocketed SFX (only for bodies that aren't TwoHanded)
- Added mob icon in inventory (instead of the white cube). This is meant to change, I'm looking for an artist to make a custom inventory icon.
- Added a body drop sound, made by **Moroxide** *(Lethal Resonance contributor)*
- Added a custom sound import system for mobs drops. More details on how to use in the `README.md`.
- Fixed some mobs not having their rank shown *when still alive*.

## [1.2.1](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.2.1) - 2025-01-30

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.2.0..1.2.1)
- Fixed mod not loading correctly when LethalConfig or LethalSettings was missing, episode 498.

## [1.2.0](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.2.0) - 2025-01-28

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.1.5..1.2.0)
- **Enemy scraps are now saved** along with other standard scraps !
- Fixed enemy bodies persisting between levels and saves.

## [1.1.5](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.1.5) - 2025-01-27

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.1.4..1.1.5)
- Fixed mob bodies not having scrap values after dying.
- Fixed some mobs having invalid animations or broken animations when they died.
- Fixed some mobs not being grabbable after dying.

Known issues:
- Mobs still don't save after leaving the game, they should but they don't and I don't know why they don't.
- Sometimes, dead bodies appear next to the ship after landing on a new moon, I still don't know why it happens.

## [1.1.4](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.1.4) - 2025-01-21

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.1.3..1.1.4)
- Thanks to **DropDaDeuce** *(LethalGargoyles)*, the config issues should now be totally fixed, at least it works on our machines.
- Fixed some modded mobs not registering correctly eventually.
 
## [1.1.3](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.1.3) - 2025-01-19

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.1.2..1.1.3)
- Fixed mod not loading correctly when Lethal Config and/or LethalSettings were installed, but for real now, hopefully
- Added a parameter on wether to play or not some mobs death animation. Turning this setting off on some monsters might fix some mobs to be invisible after their death animation.
- Added new metadata to the enemies config, including rotation when dead (leave it to zero for enemies with death anim, define it when disabling their death anim.)
- Added foundation of the next major update "Enemies Loots", will be implemented once the enemies scrap data is fully stable or at least in a satisfying state.

Known issues:
- Sometimes, dead bodies appear next to the ship after landing on a new moon, I don't know why it happens.

## [1.1.2](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.1.2) - 2025-01-18

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.1.1..1.1.2)
- Fixed the mod not loading correctly when Lethal Config and Lethal Settings weren't installed.
- Fixed error appearing when entering main menu. It will not change anything though, previously it was just there doing nothing.

## [1.1.1](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.1.1) - 2025-01-17

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.1.0..1.1.1)
- Fixed that some mobs wouldn't register their scrap self because I was not searching for ScanNode correctly inside of them, leading into some mobs could be killed but no corpse scrap was spawning
- Fixed external mod `EnemyDataRegister` function: It should now be callable from `BaseUnityPlugin.Awake()` and `.Start()`.

## [1.1.0](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.1.0) - 2025-01-16

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.0.5..1.1.0)
- Removed Xilo's StaticNetcode library for a proper networking.
- Made LethalConfig an optional mod, it is no longer required, but the config mod will still be correctly implemented by Enhanced Monsters
- Added support for LethalSettings, so users won't need to add yet another config mod if they didn't need one or another before.
- Added a button in both configs to easily copy the path to `EnemyData.json` configuration file. All you need is to paste it inside the File Explorer.
- Added a toggle to disable Ranks synchronization, so users can have their own ranking on their side. This setting is disabled by default.
- **Fixed desynchronization issues with the dead bodies that occurred when any player that is not the host picked up a dead monster**
- **Fixed desynchronization that were occasionally occurring when killing a monster.**
- **Reworked entirely the way enemy scraps were generated.** They are now more consistent thanks to a custom GrabbableObject component.
- There should be less errors in the console due to Enhanced Monsters, and there should be no error while in-game.
- Fixed settings names not showing correctly in LethalConfig.

## [1.0.5](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.0.5) - 2025-01-13  

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.0.4..1.0.5)
- Added a config versionning. Your current enemies data current configs will all be reset to the *new* default values.
- Removed mobs collisions when they're dead
- Removed mobs sounds when they die
- Fixed ScanNode not showing on some dead monsters.  

Known issues:
- The butler keeps moving after its death... wtf?
- The Forest Giant does not have any scan node for some reason, so I'll fix it next hotfix

## [1.0.4](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.0.4) - 2025-01-12

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.0.3..1.0.4)
- Fixed weight. The mass of object should now be exactly the same in the config file and in the inventory.
- Fixed prop-ized mobs animation: they should now look dead... All of them hopefully.

## [1.0.3](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.0.3) - 2025-01-12

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/compare/1.0.1...1.0.3)
- Fixed random number generator (used to generate mobs scrap value) which was not working on some modded moons.
- Working on a fix for weight being completely wonky

## 1.0.2 - 2025-01-12

- You will never know what this update changed

## [1.0.1](https://github.com/VELD-Dev/Enhanced-Monsters/releases/tag/1.0.1) - 2025-01-12

[View diff](https://github.com/VELD-Dev/Enhanced-Monsters/commits/1.0.1/)
- Fixed LethalLib dependency issue. The GUID from `LethalLib.PluginInfo.GUID` is not the one used by BepInEx.

## 1.0.0 - 2025-01-12

- Mod first release !