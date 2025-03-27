![Copyright Sadeal](https://img.shields.io/badge/Developer-Sadeal-blue)

# [Sadeal](https://sadeal.ru) CS2HvHUtilities (1.0.0)
### If you use this plugin, you do NOT need [RapidFireFix](https://github.com/HvH-gg/RapidFireFix), [TeleportFix](https://github.com/HvH-gg/TeleportFix) and WeaponResctrict anymore.

This CS2HvHUtilities plugin is the only plugin you need to run a successful HvH server. It includes basic features like **money fix**, **reset score**, **reset death** and **rage quit** as well as optional restrictions or allowence for **weapons**, **friendly fire**, **rapid fire** **meta/sm/cssharp commands** and other exploit/crash fixes.

# Features
- Custom Vote support (requires [CS2-CustomVotes](https://github.com/imi-tat0r/CS2-CustomVotes))
- Reset score `!rs`
- Reset death `!rd`
- Rage quit `!rq`
- Restrict weapons (awp, scout, autosniper)
- Money fix for newly joined player
- Restrict friendly fire (default, only utility damage)
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
To reload your config after editing, use `hvh_cfg_reload` in the server console.
```json
{
   "RapidFireFixMethod": 1, // 0 allows rapid fire, 1 blocks rapid fire, 2 reflects damage scaled, 3 reflects damage but keeps the player alive, 4 enables servers rapid-fire (any user can use it, even midnight)
   "RapidFirePrintMessage": true, // Allow to print message to all players about someone using rapid fire (shows only when reflect type selected)
   "RapidFireReflectScale": 1, // damage reflect percentage (0.0 - 1.0)
   "FixMoneyOnJoin": true, // if true, player that join in any round (except pistol, both halfs) will be given {mp_maxmoney}
   "AllowedAwpCount": -1, // how many awps are allowed per team (0 for none, -1 for unlimited)
   "AllowedScoutCount": -1, // how many scouts are allowed per team (0 for none, -1 for unlimited)
   "AllowedAutoSniperCount": -1, // how many auto snipers are allowed per team (0 for none, -1 for unlimited)
   "AllowAllWeaponsOnWarmup": true, // when warmup will allow everyone use any count of any weapon (issue: first freezetime is counter as warmup)
   "AllowWeaponsForFlag": "@css/restrict", // flag for allow to buy any amount of any weapon
   "AllowedOnlyWeapons": "", // write weapons that shoud be ONLY accesable from rifles and snipers (pistols, utilities will be always accessable). Restriction of weapons will also apply e.g.: "weapon_ssg08,weapon_scar20,weapon_g3sg1"
   "UnmatchedFriendlyFire": true, // if true, only utility damage will be dealt to teammates (like on unmatched.gg)
   "RestrictTeleport": true, // if true, the teleport and airstuck exploit will be restricted. This will also prevent the server crash exploit
   "TeleportPrintMessage": true, // Allow to print message to all players about someone using teleport/airstuck/fakepitch exploits
   "AllowAdPrint": true, // if true, players will see a "powered by HvH.gg" ad in the chat with the settings print
   "AllowSettingsPrint": true, // if true, players will see an overview of the server settings with `!settings` and on spawn
   "AllowResetScore": true, // if true, players will be able to reset their score with `!rs`
   "ShowResetScorePrint": true, // print in chat that player reset score
   "AllowResetDeath": 1, // allow to admins(@css/generic) or all players to reset their deaths (0 - off, 1 - admins, 2 - all)
   "ResetDeathFlag": "@css/general", // which flag will have access to !rd if setted to 1
   "ShowResetDeathPrint": true, // print in chat that player reset deaths
   "RestrictMetaCommands": true, // if true, players will NOT be able to get meta/sm/cssharp commands execute
   "AllowRageQuit": true, // if true, players will be able to rage quit with `!rq`
   "ChatPrefix": "[{Blue}Utils{Default}]", // chat prefix for plugin messages
   "CustomVoteSettings": { // settings for custom votes (requires CS2-CustomVotes)
      "FriendlyFireVote": false, // if true, players will be able to vote for friendly fire settings
      "TeleportFixVote": false, // if true, players will be able to vote for the teleport fix settings
      "RapidFireVote": "full", // You can set the style (`off`, `simple`, `full`) for the rapid fire vote
      "Style":"center" // You can set the style (`center` or `chat`) for the vote menu (might be overridden by CS2-CustomVotes settings)
   },
   "ConfigVersion": 8 // do not change, config version
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
- `hvh_ragequit 0/1` if true, players will be able to rage quit with `!rq`
- `hvh_restrict_meta_commands` - if true then nobody unless @css/root will access meta/css_plugins/sm commands

# ChatPrefix Colors
You can use all available colors from CounterStrikeSharp in the chat prefix.

# Custom Votes
If you have [CS2-CustomVotes](https://github.com/imi-tat0r/CS2-CustomVotes) installed, you can use the following custom votes:
- `!rapidfire` You can set the style (`off`, `simple`, `full`) in the config via `CustomVoteSettings.RapidFireVote`
- `!friendlyfire` You can enable this in the config via `CustomVoteSettings.FriendlyFireVote`
- `!teleport` You can enable this in the config via `CustomVoteSettings.TeleportVote`
> **Warning:** Teleport restriction is needed to prevent the server crash exploit. If you enable this vote, the server will be vulnerable to the crash exploit.

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
- [FusionHVH](https://fusionhvh.ru) - servers for testing
- [Metamod:Source](https://www.sourcemm.net/) 
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- [MagicBulletFix](https://github.com/CS2Plugins/MagicBulletFix)
- [RapidFireFix](https://github.com/CS2Plugins/RapidFireFix)
- [MetaCommandsBlocker](https://github.com/ManifestManah/PluginsCommandsBlocker)

# Contact me if you have any questions
Discord: Sadeal_Dev
