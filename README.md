![Copyright Sadeal](https://img.shields.io/badge/Developer-Sadeal-blue)

# [Sadeal](https://sadeal.ru) CS2HvHUtilities (1.1.8) Discord: Sadeal_Dev
### If you use this plugin, you do NOT need [RapidFireFix](https://github.com/HvH-gg/RapidFireFix), [TeleportFix](https://github.com/HvH-gg/TeleportFix) and WeaponResctrict anymore.

This CS2HvHUtilities plugin is the only plugin you need to run a successful HvH server. It includes basic features like **money fix**, **reset score**, **reset death** as well as optional restrictions or allowence for **weapons**, **rapid fire** **meta/sm/cssharp commands** and other exploit/crash fixes.

# Features
- Reset score `!rs`
- Reset death `!rd`
- Restrict weapons (awp, scout, autosniper)
- Money fix for newly joined player
- Restrict ghost exploit
- Restrict rapid fire (0 allows rapid fire, 1 blocks rapid fire from registering shot, 2 reflects damage scaled, 3 reflects damage but keeps the player alive, 4 enables server RapidFire)
- Restrict players from access MetaMod, CSSharp and SM commands in console.
- **Linux only:** Restrict teleport/airstuck/fake-pitch exploits (enabling this will also prevent the server crash exploit)

# Requirements
- [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master)
- [CounterStrikeSharp(v284)](https://github.com/roflmuffin/CounterStrikeSharp/releases)

# Install
1. Install Metamod:Source and CounterStrikeSharp
2. Place the `addons` folder in your servers `game/csgo/` directory  
   ![extract](https://du.hurenso.hn/r/0NyFPY.png)
3. Edit the config file to your liking  
   3.1. Located at `addons/counterstrikesharp/configs/plugins/CS2HvHUtilities/CS2HvHUtilities.json`
4. Restart your server

# Config
```json
{
   "RapidFireFixMethod": 1, // 0 - allow doubletap, 1 - restrict doubletap, 2 - reflect doubletap (* RapidFireReflectScale) and allow to kill, 3 - reflect doubletap but cant kill with second bullet, 4 - rapidfire feature
   "RapidFirePrintMessage": false, // true - will print message to all that someone using doubletap, false - not
   "RapidFireReflectScale": 1, // 0-1 - scale for second bullet of doubletap (0 - 0%, 1 - 100%)
   "FixMoneyOnJoin": true, // if cvar mp_afterroundmoney is set to 0 and you have pistol rounds - this will fix that someone joining server and have 800 instead of 16000
   "AllowedAwpCount": -1, // -1 - allow all awp, 0 - awp not allowed, 1 - 1 to each team, 2....
   "AllowedScoutCount": -1, // -1 - allow all ssg - ssg not allowed, 1 - 1 to each team, 2....
   "AllowedAutoSniperCount": -1, // -1 - allow all autos, 0 - autos not allowed, 1 - 1 to each team, 2....
   "AllowAllWeaponsOnWarmup": true, // removes restriction on warmup period for everyone
   "AllowWeaponsForFlag": "@css/restrict", // flag that ignore weapon restrictions
   "AllowedOnlyWeapons": "", // list of weapons that ONLY allowed. "weapon_ssg08" - will allow only ssg08, not AK, not AWP and etc. (pistols, nades etc always accessable)
   "RestrictTeleport": true, // true - restrict teleport, fakepitch, crash
   "TeleportPrintMessage": false, // will print in chat if someone using exploit
   "RestrictGhost": true, // will restrict ghost exploit
   "AllowAdPrint": false, // my ad
   "AdvertiseBlockerName": true, // will block AD names (neverlose.cc, market/, funpay, ...)
   "AdvertiseBlockerChat": true, // will block AD in chat
   "AllowSettingsPrint": true, // will print current server settings (for this plugin) in chat
   "AllowResetScore": true, // allow !rs
   "ShowResetScorePrint": true, // will show in chat to all that someone !rs
   "AllowResetDeath": 0, // 0 - not allowed, 1 - allowed !rd for ResetDeathFlag only, 2 - allow !rd to all
   "ResetDeathFlag": "@css/general", // flag for !rd
   "ShowResetDeathPrint": false, // will print message to all that someone !rd
   "RestrictMetaCommands": true, // restrict anyone except @css/root to type "meta", "css_plugins" etc to game console and get result
   "ChatPrefix": "[{Red}Utils{Default}]", // chat prefix for !rs, resticts and etc for this plugin
     "CustomPhrasesSettings": {
       "AdvertiseBlockerMessage": "Advertisement is prohibited!", // message to user that trying to echo AD
       "AdvertiseNameChangeTo": "[Player1],[Player2],[Player3]", // will rename to one of this names if user have AD
       "ResetScorePhrase": "Player {Orange}{PlayerName}{Grey} has reset their stats!", // message for !rs
       "ResetScoreAlreadyPhrase": "Your stats are already 0.", // message for !rs but already 0
       "ResetDeathPhrase": "Player {Orange}{PlayerName}{Grey} has reset their deaths!", // message for !rd
       "ResetDeathAlreadyPhrase": "Your deaths are already 0.", // message for !rd but already 0
       "RapidFirePhrase": "Player {Orange}{PlayerName}{Grey} tried using {Orange}double tap{Grey}!", // message for RapidFireFix method 1
       "TeleportPhrase": "Player {Orange}{PlayerName}{Grey} tried using {Orange}Teleport{Grey}!", // message for TeleportFix 
       "WeaponRestrictPhrase": "{Orange}{WeaponName}{Grey} is restricted to {Orange}{RestrictAmount}{Grey} per team!", // Message for WeaponResctrict
   },
"ConfigVersion": 10 // not to change
}
```

# ConVars
Instead of editing the config file, you can also use the following ConVars to change certain settings **on the fly**. These changes will **NOT** be saved to th+e config file and will be reset after a server restart.
- `hvh_restrict_rapidfire` 0 allows rapid fire, 1 blocks rapid fire, 2 reflects damage scaled, 3 reflects damage but keeps the player alive
- `hvh_rapidfire_reflect_scale` damage reflect percentage, 0.0 - 1.0
- `hvh_rapidfire_print_message` if true, then all players will see in chat if someone using rapidfire (shows only when reflect type selected)
- `hvh_money_fix` if true, player that join in any round (except pistol, both halfs) will be given {mp_maxmoney}
- `hvh_restrict_teleport` if true, the teleport and airstuck exploit will be restricted. This will also prevent the server crash exploit
- `hvh_teleport_print_message` if true, then all players will see in chat if someone using teleport/airstuck/fakepitch exploits
- `hvh_restrict_awp` how many awps are allowed per team, 0 for none, -1 for unlimited
- `hvh_restrict_scout` how many scouts are allowed per team, 0 for none, -1 for unlimited
- `hvh_restrict_auto` how many auto snipers are allowed per team, 0 for none, -1 for unlimited
- `hvh_allow_only_weapons` which weapons will be only allowed (not includes pistols, utilities etc.). For example: "weapon_ssg08,weapon_scar20,weapon_g3sg1"
- `hvh_utilities_friendlyfire` if true, only utility damage will be dealt to teammates
- `hvh_resetscore 0/1` if true, players will be able to reset their score with `!rs`
- `hvh_resetdeath 0/1/2` 0 - off, 1 - admin only, 2 - everyone
- `hvh_restrict_meta_commands` - if true then nobody unless @css/root will access meta/css_plugins/sm commands

# ChatPrefix Colors
You can use all available colors from CounterStrikeSharp in the chat prefix.

# iksAdmin and flags
If you have iksAdmin installed you have to specialize `word` defenition to a custom flag that you typed in config
For example:
```json
...
  "p": {
    "@css/restrict"
  }
...
```

# Credits
- [HvH.gg](https://hvh.gg) - plugin code base
- [EXHVH](https://exhvh.ru) - servers for testing
- [Metamod:Source](https://www.sourcemm.net/) 
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- [MagicBulletFix](https://github.com/CS2Plugins/MagicBulletFix)
- [RapidFireFix](https://github.com/CS2Plugins/RapidFireFix)
- [MetaCommandsBlocker](https://github.com/ManifestManah/PluginsCommandsBlocker)

# Contact me if you have any questions
Discord: Sadeal_Dev
