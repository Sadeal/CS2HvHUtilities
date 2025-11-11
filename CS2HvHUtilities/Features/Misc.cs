using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CSSharpUtils.Extensions;
using cs2hvh_utilities.Enums;
using cs2hvh_utilities.Extensions;

namespace cs2hvh_utilities.Features;

public class Misc
{
    private readonly Plugin _plugin;
    private readonly Dictionary<uint, float> _lastRulePrint = new();

    public Misc(Plugin plugin)
    {
        _plugin = plugin;
    }

    [ConsoleCommand("rules", "Print rules")]
    [ConsoleCommand("кгдуы", "Print rules")]
    [ConsoleCommand("rule", "Print rules")]
    [ConsoleCommand("кгду", "Print rules")]
    public void OnSettings(CCSPlayerController? player, CommandInfo inf)
    {
        AnnounceRules(player, true);
    }
    
    [ConsoleCommand("hvh_cfg_reload", "Reload the config in the current session without restarting the server")]
    [RequiresPermissions("@css/generic")]
    [CommandHelper(minArgs: 0, whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void OnReloadConfigCommand(CCSPlayerController? player, CommandInfo info)
    {
        _plugin.OnConfigParsed(new Cs2HvhUtilitiesConfig().Reload());
    }
    
    public void AnnounceRules(CCSPlayerController? player, bool force = false)
    {
        if (!player.IsPlayer())
            return;

        if (_lastRulePrint.TryGetValue(player!.Pawn.Index, out var lastPrintTime) &&
            lastPrintTime + 600 > Server.CurrentTime && !force)
            return;

        _lastRulePrint[player.Pawn.Index] = Server.CurrentTime;

        if (_plugin.Config.AllowSettingsPrint)
        {
            var fieldValues = new Dictionary<string, object> {};

            var message = _plugin.FormatString(Language.GetMessage("ServerRules"), fieldValues);
            player.PrintToChat($"{message}");

            message = _plugin.FormatString(Language.GetMessage("GrenadesFF"), fieldValues);
            player.PrintToChat(
                $"{message} {(_plugin.Config.UtilitiesFriendlyFire ? $"{ChatColors.Lime}ON" : $"{ChatColors.Orange}OFF")}");

            message = _plugin.FormatString(Language.GetMessage("TeleportFP"), fieldValues);
            player.PrintToChat(
                $"{message} {(!_plugin.Config.RestrictTeleport ? $"{ChatColors.Lime}ON" : $"{ChatColors.Orange}OFF")}");

            message = _plugin.FormatString(Language.GetMessage("RapidDT"), fieldValues);
            player.PrintToChat(
                $"{message} {(_plugin.Config.RapidFireFixMethod != FixMethod.Ignore ? $"{ChatColors.Lime}ON" : $"{ChatColors.Orange}OFF")}");

            switch (_plugin.Config.RapidFireFixMethod)
            {
                case FixMethod.Allow:
                    break;
                case FixMethod.Ignore:
                    message = _plugin.FormatString(Language.GetMessage("MethodIgnore"), fieldValues);
                    player.PrintToChat($"{message}");
                    break;
                case FixMethod.Reflect:
                    message = _plugin.FormatString(Language.GetMessage("Method"), fieldValues);
                    player.PrintToChat(
                        $"{message} {ChatColors.Orange}reflect damage{ChatColors.Grey} at {ChatColors.Orange}{_plugin.Config.RapidFireReflectScale}x{ChatColors.Grey}");
                    break;
                case FixMethod.ReflectSafe:
                    message = _plugin.FormatString(Language.GetMessage("Method"), fieldValues);
                    player.PrintToChat(
                        $"{message} {ChatColors.Orange}reflect damage{ChatColors.Grey} at {ChatColors.Orange}{_plugin.Config.RapidFireReflectScale}x{ChatColors.Grey} and prevent death (1 hp)");
                    break;
                case FixMethod.RapidFire:
                    message = _plugin.FormatString(Language.GetMessage("MethodRapid"), fieldValues);
                    player.PrintToChat(
                        $"{message}");
                    break;
                default:
                    break;
            }

            player.PrintToChat(" ");
            if (_plugin.Config.AllowedAwpCount > -1 || _plugin.Config.AllowedScoutCount > -1 || _plugin.Config.AllowedAutoSniperCount > -1)
            {
                message = _plugin.FormatString(Language.GetMessage("WeaponResctriction"), fieldValues);
                player.PrintToChat($"{message}");
                message = _plugin.FormatString(Language.GetMessage("WeaponResctrictionPerTeam"), fieldValues);
                if (_plugin.Config.AllowedAwpCount != -1)
                    player.PrintToChat(
                        $"AWP: {(_plugin.Config.AllowedAwpCount == 0 ? ChatColors.Red : ChatColors.Orange)}{_plugin.Config.AllowedAwpCount} {message}");
                if (_plugin.Config.AllowedScoutCount != -1)
                    player.PrintToChat(
                        $"Scout: {(_plugin.Config.AllowedScoutCount == 0 ? ChatColors.Red : ChatColors.Orange)}{_plugin.Config.AllowedScoutCount} {message}");
                if (_plugin.Config.AllowedAutoSniperCount != -1)
                    player.PrintToChat(
                        $"Auto: {(_plugin.Config.AllowedAutoSniperCount == 0 ? ChatColors.Red : ChatColors.Orange)}{_plugin.Config.AllowedAutoSniperCount} {message}");
            }

            player.PrintToChat(" ");

            message = _plugin.FormatString(Language.GetMessage("HelpMessage"), fieldValues);
            player.PrintToChat(
                $"{Colors.FormatMessage(_plugin.Config.ChatPrefix)} {message}");
        }

        if (_plugin.Config.AllowAdPrint)
        {
            player.PrintToChat(Colors.FormatMessage($"Created and supported by {ChatColors.Lime}Sadeal"));
        }
    }
}