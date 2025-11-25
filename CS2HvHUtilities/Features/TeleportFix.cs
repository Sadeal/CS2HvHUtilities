using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using CSSharpUtils.Extensions;
using cs2hvh_utilities.Extensions;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;

namespace cs2hvh_utilities.Features;

public class TeleportFix
{
    private readonly Plugin _plugin;
    private readonly Dictionary<uint, float> _teleportBlockWarnings = new();
    private readonly Dictionary<uint, float> _switchTeam = new();
    public static readonly FakeConVar<bool> hvh_restrict_teleport = new("hvh_restrict_teleport", "Restricts players from teleporting/airstucking and crashing the server", true, ConVarFlags.FCVAR_REPLICATED);
    public static readonly FakeConVar<bool> hvh_teleport_print_message = new("hvh_teleport_print_message", "Print in chat message about using teleporting/airstucking", true, ConVarFlags.FCVAR_REPLICATED);

    public TeleportFix(Plugin plugin)
    {
        _plugin = plugin;
        _plugin.RegisterFakeConVars(this);
        hvh_restrict_teleport.Value = _plugin.Config.RestrictTeleport;
        hvh_teleport_print_message.Value = _plugin.Config.TeleportPrintMessage;
    }

    public HookResult RunCommand(DynamicHook h)
    {
        if (!hvh_restrict_teleport.Value)
            return HookResult.Continue;
        
        // check if the player is a valid player
        var player = h.GetParam<CCSPlayer_MovementServices>(0).Pawn.Value.Controller.Value?.As<CCSPlayerController>();
        if (!player.IsPlayer())
            return HookResult.Continue;
        
        // get the user command and view angles
        var userCmd = new CUserCmd(h.GetParam<IntPtr>(1));
        var viewAngles = userCmd.GetViewAngles();
        
        // no valid view angles or not infinite
        if (viewAngles is null || viewAngles.IsValid()) 
            return HookResult.Continue;
        
        // fix the view angles (prevents the player from using teleport or airstuck)
        viewAngles.Fix();

        // if allow to print
        if (hvh_teleport_print_message.Value)
        {
            // not warned yet or last warning was more than 3 seconds ago
            if (_teleportBlockWarnings.TryGetValue(player!.Index, out var lastWarningTime) &&
                !(lastWarningTime + 3 <= Server.CurrentTime))
                return HookResult.Changed;

            // print a warning to all players
            var fieldValues = new Dictionary<string, object>
            {
                { "PlayerName", player.PlayerName }
            };

            var message = _plugin.FormatString(Language.GetMessage("TeleportPhrase"), fieldValues);

            Server.PrintToChatAll($"{Colors.FormatMessage(_plugin.Config.ChatPrefix)} {message}");

            _teleportBlockWarnings[player.Index] = Server.CurrentTime;
        }

        return HookResult.Changed;
    }

    public HookResult ListenerJoinTeam(CCSPlayerController? player, CommandInfo info)
    {
        if(!_plugin.Config.RestrictGhost)
            return HookResult.Continue;

        if (player?.IsValid == true && player.PlayerPawn?.IsValid == true)
        {
            if(AdminManager.PlayerHasPermissions(player, "@css/general"))
            {
                return HookResult.Continue;
            }

            var index = player.Index;
            CCSGameRules gamerules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!;
            if (!gamerules.FreezePeriod)
            {
                _switchTeam[index] = Server.CurrentTime;
                return HookResult.Continue;
            }
            if (player.Team != CsTeam.None)
            {
                if (info.ArgByIndex(1) == "1" || info.ArgByIndex(1) == "2" || info.ArgByIndex(1) == "3")
                {
                    var mp_freezetime = ConVar.Find("mp_freezetime");
                    var freezetime = 10;
                    if (mp_freezetime != null)
                    {
                        freezetime = mp_freezetime.GetPrimitiveValue<int>();
                    }
                    if (_switchTeam.TryGetValue(index, out var lastChangeTime) && lastChangeTime + freezetime + 1 > Server.CurrentTime)
                    {
                        return HookResult.Stop;
                    }
                }

                _switchTeam[index] = Server.CurrentTime;
            }
        }

        return HookResult.Continue;
    }
}