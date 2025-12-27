using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using CSSharpUtils.Extensions;
using cs2hvh_utilities.Enums;
using cs2hvh_utilities.Extensions;
using CounterStrikeSharp.API.Modules.Entities;

namespace cs2hvh_utilities.Features;

public class RapidFire
{
    private readonly Dictionary<uint, int> _lastPlayerShotTick = new();
    private readonly HashSet<uint> _rapidFireBlockUserIds = new();
    private readonly Dictionary<uint, float> _rapidFireBlockWarnings = new();

    private readonly Plugin _plugin;
    public static readonly FakeConVar<int> hvh_restrict_rapidfire = new("hvh_restrict_rapidfire", "Restrict rapid fire", 0, ConVarFlags.FCVAR_REPLICATED, new RangeValidator<int>(0, 4));
    public static readonly FakeConVar<bool> hvh_rapidfire_print_message = new("hvh_rapidfire_print_message", "Print message to all chat about rapid fire", true, ConVarFlags.FCVAR_REPLICATED, new RangeValidator<bool>(false, true));

    public RapidFire(Plugin plugin)
    {
        _plugin = plugin;
        _plugin.RegisterFakeConVars(this);
        hvh_restrict_rapidfire.Value = (int) _plugin.Config.RapidFireFixMethod;
        hvh_rapidfire_print_message.Value = _plugin.Config.RapidFirePrintMessage;
    }

    public HookResult OnBulletImpact(EventBulletImpact evt, GameEventInfo info)
    {
        if (hvh_restrict_rapidfire.Value != (int)FixMethod.Ignore)
            return HookResult.Continue;

        if (evt.Userid?.Pawn?.Value?.WeaponServices?.ActiveWeapon?.Value == null)
            return HookResult.Continue;

        CBasePlayerWeapon firedWeapon = evt.Userid.Pawn.Value.WeaponServices.ActiveWeapon.Value!;

        CCSWeaponBaseVData? weaponData = firedWeapon.GetVData<CCSWeaponBaseVData>();

        if (weaponData == null)
            return HookResult.Continue;

        if (firedWeapon.DesignerName == "weapon_revolver")
            return HookResult.Continue;

        int tickBase = (int)evt.Userid.TickBase;

        int fixedPrimaryTick = (int)Math.Round(weaponData.CycleTime.Values[0] * 64) - 3;
        firedWeapon.NextPrimaryAttackTick = Math.Max(firedWeapon.NextPrimaryAttackTick, tickBase + fixedPrimaryTick);
        Utilities.SetStateChanged(firedWeapon, "CBasePlayerWeapon", "m_nNextPrimaryAttackTick");

        if (firedWeapon.DesignerName == "weapon_revolver")
        {
            int fixedSecondaryTick = (int)Math.Round(weaponData.CycleTime.Values[1] * 64) - 3;
            firedWeapon.NextSecondaryAttackTick = Math.Max(firedWeapon.NextSecondaryAttackTick, tickBase + fixedSecondaryTick);
            Utilities.SetStateChanged(firedWeapon, "CBasePlayerWeapon", "m_flNextPrimaryAttackTickRatio");
        }

        return HookResult.Continue;
    }
}