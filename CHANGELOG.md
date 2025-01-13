# Changelog

All notable changes will be documented in the changelog.  
  
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