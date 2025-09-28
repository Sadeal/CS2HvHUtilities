using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;

namespace cs2hvh_utilities.Features;

public class MetaCommandsBlocker
{
    private readonly Plugin _plugin;
    public static readonly FakeConVar<bool> hvh_restrict_meta_commands = new("hvh_restrict_meta_commands", "Restricts players from getting result for meta/cssharp commands", true, ConVarFlags.FCVAR_REPLICATED);

    public MetaCommandsBlocker(Plugin plugin)
    {
        _plugin = plugin;
        _plugin.RegisterFakeConVars(this);
        hvh_restrict_meta_commands.Value = _plugin.Config.RestrictMetaCommands;
    }

    public HookResult CommandListener_BlockOutput(CCSPlayerController? player, CommandInfo info)
    {
        if (!hvh_restrict_meta_commands.Value)
            return HookResult.Continue;

        if (player == null)
        {
            return HookResult.Continue;
        }

        if (!player.IsValid)
        {
            return HookResult.Continue;
        }

        if (!player.PlayerPawn.IsValid)
        {
            return HookResult.Continue;
        }

        if (AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            return HookResult.Continue;
        }

        return HookResult.Stop;
    }
}
