using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using cs2hvh_utilities.Extensions;
using CounterStrikeSharp.API.Modules.Entities;
using System.Numerics;

namespace cs2hvh_utilities.Features;

public class AdBlocker
{
    private readonly Plugin _plugin;
    public bool useBlockerName;
    public string blockedNameChangeTo;
    public bool useBlockerChat;
    public string messageToChat;
    private int currentChangeName = 0;

    public AdBlocker(Plugin plugin)
    {
        _plugin = plugin;
        _plugin.RegisterFakeConVars(this);
        useBlockerName = _plugin.Config.AdvertiseBlockerName;
        blockedNameChangeTo = _plugin.Config.AdvertiseNameChangeTo;
        useBlockerChat = _plugin.Config.AdvertiseBlockerChat;
        messageToChat = Language.GetMessage("AdvertiseBlockerMessage");
        return;
    }

    public HookResult OnPlayerChat(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(!useBlockerChat) 
            return HookResult.Continue;

        var message = commandInfo.GetCommandString;
        message = message.Trim();

        if (player == null || !player.IsValid || player.IsBot)
            return HookResult.Continue;

        if (AdminManager.PlayerHasPermissions(player, "@css/root") || AdminManager.PlayerHasPermissions(player, "@css/general"))
            return HookResult.Continue;

        if(string.IsNullOrWhiteSpace(message)) 
            return HookResult.Continue;

        if (isAd(message))
        {
            player.PrintToChat($"{Colors.FormatMessage(messageToChat)}");
            return HookResult.Stop;
        }

        return HookResult.Continue;
    }

    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if (!useBlockerName)
            return HookResult.Continue;

        var players = Utilities.GetPlayers();

        for (var i = 0; i < players.Count; i++)
        {
            if (players[i] == null || players[i].IsBot || players[i].Connected != PlayerConnectedState.PlayerConnected)
                continue;

            if (AdminManager.PlayerHasPermissions(players[i], "@css/root"))
                return HookResult.Continue;

            if (isAd(players[i].PlayerName))
            {
                players[i].PrintToChat($"{Colors.FormatMessage(messageToChat)}");
                players[i].PlayerName = GetNewName(blockedNameChangeTo);
                currentChangeName += 1;
                Utilities.SetStateChanged(players[i], "CBasePlayerController", "m_iszPlayerName");
            }
            else
            {
                if (players[i].PlayerName.Length > 32)
                {
                    var newName = Colors.RemoveFromMessage(_plugin.Config.ChatPrefix) + " TooLongName";

                    if (newName.Length > 32)
                        newName = "TooLongName";

                    players[i].PlayerName = Colors.RemoveFromMessage(_plugin.Config.ChatPrefix) + " TooLongName";
                }
            }
        }

        return HookResult.Continue;
    }

    public HookResult OnRoundPreStart(EventRoundPrestart @event, GameEventInfo info)
    {
        if (!useBlockerName)
            return HookResult.Continue;

        var players = Utilities.GetPlayers();

        for (var i = 0; i < players.Count; i++)
        {
            if (players[i] == null || players[i].IsBot || players[i].Connected != PlayerConnectedState.PlayerConnected)
                continue;

            if (AdminManager.PlayerHasPermissions(players[i], "@css/root"))
                return HookResult.Continue;

            if (isAd(players[i].PlayerName))
            {
                players[i].PrintToChat($"{Colors.FormatMessage(messageToChat)}");
                players[i].PlayerName = GetNewName(blockedNameChangeTo);
                currentChangeName += 1;
                Utilities.SetStateChanged(players[i], "CBasePlayerController", "m_iszPlayerName");
            }
            else
            {
                if (players[i].PlayerName.Length > 32)
                {
                    var newName = Colors.RemoveFromMessage(_plugin.Config.ChatPrefix) + " TooLongName";

                    if (newName.Length > 32)
                        newName = "TooLongName";

                    players[i].PlayerName = Colors.RemoveFromMessage(_plugin.Config.ChatPrefix) + " TooLongName";
                }
            }
        }


        return HookResult.Continue;
    }

    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (!useBlockerName)
            return HookResult.Continue;

        var players = Utilities.GetPlayers();

        for (var i = 0; i < players.Count; i++)
        {
            if (players[i] == null || players[i].IsBot || players[i].Connected != PlayerConnectedState.PlayerConnected)
                continue;

            if (AdminManager.PlayerHasPermissions(players[i], "@css/root"))
                return HookResult.Continue;

            if (isAd(players[i].PlayerName))
            {
                players[i].PrintToChat($"{Colors.FormatMessage(messageToChat)}");
                players[i].PlayerName = GetNewName(blockedNameChangeTo);
                currentChangeName += 1;
                Utilities.SetStateChanged(players[i], "CBasePlayerController", "m_iszPlayerName");
            }
            else
            {
                if (players[i].PlayerName.Length > 32)
                {
                    var newName = Colors.RemoveFromMessage(_plugin.Config.ChatPrefix) + " TooLongName";

                    if (newName.Length > 32)
                        newName = "TooLongName";

                    players[i].PlayerName = Colors.RemoveFromMessage(_plugin.Config.ChatPrefix) + " TooLongName";
                }
            }
        }


        return HookResult.Continue;
    }

    public HookResult OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (!useBlockerName)
            return HookResult.Continue;

        var player = @event.Userid;

        if (player == null || player.IsBot || player.Connected != PlayerConnectedState.PlayerConnected)
            return HookResult.Continue;

        if (AdminManager.PlayerHasPermissions(player, "@css/root"))
            return HookResult.Continue;

        if (isAd(player.PlayerName))
        {
            player.PrintToChat($"{Colors.FormatMessage(messageToChat)}");
            player.PlayerName = GetNewName(blockedNameChangeTo);
            currentChangeName += 1;
            Utilities.SetStateChanged(player, "CBasePlayerController", "m_iszPlayerName");
        }
        else
        {
            if (player.PlayerName.Length > 32)
            {
                var newName = Colors.RemoveFromMessage(_plugin.Config.ChatPrefix) + " TooLongName";

                if (newName.Length > 32)
                    newName = "TooLongName";

                player.PlayerName = Colors.RemoveFromMessage(_plugin.Config.ChatPrefix) + " TooLongName";
            }
        }

        return HookResult.Continue;
    }

    private string GetNewName(string names)
    {
        var newName = "[Player]";
        var list = names.Split(',');
        var index = list.Length;
        var delimer = currentChangeName > 0 ? currentChangeName : 1;
        for (var i = 0; i < (int)(list.Length/ delimer); i++)
        {
            if (index > currentChangeName)
                break;
            currentChangeName -= index;
        }

        return list[currentChangeName];
    }

    public bool isAd(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        string[] patterns = {
            @"https?",
            @"www\.",
            @"\.(ru|com|net|win|gg|cc|market|guru|live|org)",
            @"funpay.",
            @"lots/",
            @"market/",
            @"item\?id",
            @"offer\?id",
            @"resources/",
            @"t\.me",
            @"@\S+",
            @"buy\s+cfg",
            @"connect.+",
            @"(?:\d{1,3}\.){3}\d{1,3}(?::\d+)?",
            @"discord\.gg",
            @"invite/",
            @"id\=([a-zA-Z0-9]{6})",
            @"id([a-zA-Z0-9]{6})"
        };

        foreach (string pattern in patterns)
        {
            if (Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase))
                return true;
        }

        return false;
    }
}
