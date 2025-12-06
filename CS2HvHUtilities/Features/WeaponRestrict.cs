using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using cs2hvh_utilities.Extensions;
using System.Numerics;

namespace cs2hvh_utilities.Features;

public class WeaponRestrict
{
    private readonly Plugin _plugin;
    private readonly Dictionary<string, Tuple<ItemDefinition, int>> _weaponPrices = new()
    {
        { "weapon_ssg08", new Tuple<ItemDefinition, int>(ItemDefinition.SSG_08, 1700) },
        { "weapon_awp", new Tuple<ItemDefinition, int>(ItemDefinition.AWP, 4750) },
        { "weapon_scar20", new Tuple<ItemDefinition, int>(ItemDefinition.SCAR_20, 5000) },
        { "weapon_g3sg1", new Tuple<ItemDefinition, int>(ItemDefinition.G3SG1, 5000) },
    };
    private readonly Dictionary<uint, float> _lastWeaponRestrictPrint = new();
    public static readonly FakeConVar<int> hvh_restrict_awp = new("hvh_restrict_awp", "Restrict awp to X per team", -1, ConVarFlags.FCVAR_REPLICATED, new RangeValidator<int>(-1, int.MaxValue));
    public static readonly FakeConVar<int> hvh_restrict_scout = new("hvh_restrict_scout", "Restrict scout to X per team", -1, ConVarFlags.FCVAR_REPLICATED, new RangeValidator<int>(-1, int.MaxValue));
    public static readonly FakeConVar<int> hvh_restrict_auto = new("hvh_restrict_auto", "Restrict autosniper to X per team", -1, ConVarFlags.FCVAR_REPLICATED, new RangeValidator<int>(-1, int.MaxValue));
    public static readonly FakeConVar<string> hvh_allow_only_weapons = new("hvh_allow_only_weapons", "Allows only selected weapons", "", ConVarFlags.FCVAR_REPLICATED);

    private bool checkWarmup = false;
    public bool InWarmup = false;
    private string AllowedFlag = "@css/restrict";

    public WeaponRestrict(Plugin plugin)
    {
        _plugin = plugin;
        _plugin.RegisterFakeConVars(this);
        hvh_restrict_awp.Value = _plugin.Config.AllowedAwpCount;
        hvh_restrict_scout.Value = _plugin.Config.AllowedScoutCount;
        hvh_restrict_auto.Value = _plugin.Config.AllowedAutoSniperCount;
        checkWarmup = _plugin.Config.AllowAllWeaponsOnWarmup;

        hvh_allow_only_weapons.Value = _plugin.Config.AllowedOnlyWeapons.Length > 0 
            ? _plugin.Config.AllowedOnlyWeapons 
            : "";

        AllowedFlag = _plugin.Config.AllowWeaponsForFlag is string && _plugin.Config.AllowWeaponsForFlag.Length > 0 
            ? _plugin.Config.AllowWeaponsForFlag 
            : "@css/restrict";
    }

    public HookResult OnWeaponCanAcquire(DynamicHook hook)
    {
        if (checkWarmup && InWarmup)
            return HookResult.Continue;

        CCSWeaponBaseVData vdata = VirtualFunctions.GetCSWeaponDataFromKeyFunc.Invoke(-1, hook.GetParam<CEconItemView>(1).ItemDefinitionIndex.ToString()) ?? throw new Exception("Failed to get CCSWeaponBaseVData");

        CCSPlayerController client = hook.GetParam<CCSPlayer_ItemServices>(0).Pawn.Value!.Controller.Value!.As<CCSPlayerController>();

        if (client == null || !client.IsValid || !client.PawnIsAlive)
            return HookResult.Continue;

        if (hvh_allow_only_weapons.Value.Length > 0)
        {
            List<string> deniedWeapons = new List<string>
            {
                "weapon_mp9",
                "weapon_mac10",
                "weapon_bizon",
                "weapon_mp7",
                "weapon_ump45",
                "weapon_p90",
                "weapon_mp5sd",
                "weapon_famas",
                "weapon_galilar",
                "weapon_m4a1",
                "weapon_m4a4",
                "weapon_m4a1_silencer",
                "weapon_ak47",
                "weapon_aug",
                "weapon_sg553",
                "weapon_ssg08",
                "weapon_awp",
                "weapon_scar20",
                "weapon_g3sg1",
                "weapon_nova",
                "weapon_xm1014",
                "weapon_mag7",
                "weapon_sawedoff",
                "weapon_m249",
                "weapon_negev"
            };

            var weapons = hvh_allow_only_weapons.Value.Split(',');
            List<string> resultWeapons = new List<string>();

            foreach (var weapon in weapons)
            {
                var wepName = weapon.Trim();
                if (!wepName.StartsWith("weapon_"))
                {
                    wepName = "weapon_" + wepName;
                }
                deniedWeapons.Remove(wepName);
            }
            if (deniedWeapons.Contains(vdata.Name))
            {
                if (hook.GetParam<AcquireMethod>(2) != AcquireMethod.PickUp)
                {
                    hook.SetReturn(AcquireResult.AlreadyOwned);

                    var fieldValues = new Dictionary<string, object>
                    {
                        { "WeaponName", vdata.Name.Replace("weapon_", "") },
                        { "RestrictAmount", 0 }
                    };

                    var message = _plugin.FormatString(_plugin.Config.CustomPhrasesSettings.WeaponRestrictPhrase, fieldValues);

                    Server.NextFrame(() => client.PrintToChat($"{Colors.FormatMessage(_plugin.Config.ChatPrefix)} {message}"));
                }
                else
                {
                    hook.SetReturn(AcquireResult.InvalidItem);
                }
                return HookResult.Stop;
            }
        }

        if (AdminManager.PlayerHasPermissions(client, "@css/root") || AdminManager.PlayerHasPermissions(client, AllowedFlag))
            return HookResult.Continue;

        int limit = GetAllowedWeaponCount(vdata.Name);
        bool disabled = limit <= -1;

        if (!disabled)
        {
            int count = GetWeaponCountInTeam(vdata.Name, client.Team);
            if (count < limit)
                return HookResult.Continue;
        }
        else
        {
            return HookResult.Continue;
        }

        if (hook.GetParam<AcquireMethod>(2) != AcquireMethod.PickUp)
        {
            hook.SetReturn(AcquireResult.AlreadyOwned);

            var fieldValues = new Dictionary<string, object>
            {
                { "WeaponName", vdata.Name },
                { "RestrictAmount", limit }
            };

            var message = _plugin.FormatString(_plugin.Config.CustomPhrasesSettings.WeaponRestrictPhrase, fieldValues);

            Server.NextFrame(() => client.PrintToChat($"{Colors.FormatMessage(_plugin.Config.ChatPrefix)} {message}"));
        }
        else
        {
            hook.SetReturn(AcquireResult.InvalidItem);
        }

        return HookResult.Stop;
    }
    
    private int GetAllowedWeaponCount(string item)
    {
        return item switch
        {
            "weapon_awp" => hvh_restrict_awp.Value,
            "weapon_ssg08" => hvh_restrict_scout.Value,
            "weapon_scar20" or "weapon_g3sg1" => hvh_restrict_auto.Value,
            _ => -1
        };
    }

    private int GetWeaponCountInTeam(string item, CsTeam team)
    {
        var activePlayers = Utilities.GetPlayers()
            .Where(pl => pl is { IsValid: true, PlayerPawn.IsValid: true, PlayerPawn.Value.IsValid: true } &&
                         pl.TeamNum == (byte)team).ToList();

        var weaponCount = activePlayers
            .Select(player => player.PlayerPawn.Value!.WeaponServices!.MyWeapons)
            .Select(playerWeapons => playerWeapons
                .Where(weapon => weapon.IsValid && weapon.Value!.IsValid)
                .Count(weapon => weapon.Value!.AttributeManager.Item.ItemDefinitionIndex ==
                                 (ushort)_weaponPrices[item].Item1))
            .Sum();

        return weaponCount;
    }
}