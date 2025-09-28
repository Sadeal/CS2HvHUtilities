using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using CSSharpUtils.Extensions;
using cs2hvh_utilities.Enums;
using cs2hvh_utilities.Extensions;

namespace cs2hvh_utilities.Features;

public class RapidFire
{
    private readonly Dictionary<uint, int> _lastPlayerShotTick = new();
    private readonly HashSet<uint> _rapidFireBlockUserIds = new();
    private readonly Dictionary<uint, float> _rapidFireBlockWarnings = new();

    private readonly Plugin _plugin;
    public static readonly FakeConVar<int> hvh_restrict_rapidfire = new("hvh_restrict_rapidfire", "Restrict rapid fire", 0, ConVarFlags.FCVAR_REPLICATED, new RangeValidator<int>(0, 4));
    public static readonly FakeConVar<float> hvh_rapidfire_reflect_scale = new("hvh_rapidfire_reflect_scale", "Reflect scale", 1, ConVarFlags.FCVAR_REPLICATED, new RangeValidator<float>(0, 1));
    public static readonly FakeConVar<bool> hvh_rapidfire_print_message = new("hvh_rapidfire_print_message", "Print message to all chat about rapid fire", true, ConVarFlags.FCVAR_REPLICATED, new RangeValidator<bool>(false, true));

    public RapidFire(Plugin plugin)
    {
        _plugin = plugin;
        _plugin.RegisterFakeConVars(this);
        hvh_restrict_rapidfire.Value = (int) _plugin.Config.RapidFireFixMethod;
        hvh_rapidfire_reflect_scale.Value = _plugin.Config.RapidFireReflectScale;
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

    public HookResult OnWeaponFire(EventWeaponFire eventWeaponFire, GameEventInfo info)
    {
        if (hvh_restrict_rapidfire.Value != (int)FixMethod.Reflect && hvh_restrict_rapidfire.Value != (int)FixMethod.ReflectSafe)
            if(
                hvh_restrict_rapidfire.Value == (int)FixMethod.Ignore 
                && eventWeaponFire.Userid!.Pawn.Value?.WeaponServices?.ActiveWeapon?.Value!.DesignerName != "weapon_knife"
                && eventWeaponFire.Userid!.Pawn.Value?.WeaponServices?.ActiveWeapon?.Value!.DesignerName != "weapon_bayonet"
            )
                return HookResult.Continue;

        if (!eventWeaponFire.Userid.IsPlayer())
            return HookResult.Continue;
        
        var firedWeapon = eventWeaponFire.Userid!.Pawn.Value?.WeaponServices?.ActiveWeapon.Value;
        var weaponData = firedWeapon?.GetVData<CCSWeaponBaseVData>();
        
        var index = eventWeaponFire.Userid.Pawn.Index;
            
        if (!_lastPlayerShotTick.TryGetValue(index, out var lastShotTick))
        {
            _lastPlayerShotTick[index] = Server.TickCount;
            return HookResult.Continue;
        }
            
        _lastPlayerShotTick[index] = Server.TickCount;
        
        var shotTickDiff = Server.TickCount - lastShotTick;
        var possibleAttackDiff = (weaponData?.CycleTime.Values[0] * 64 ?? 0) - 1;

        if (shotTickDiff > possibleAttackDiff ||
            firedWeapon?.DesignerName == "weapon_revolver")
            return HookResult.Continue;
            
        Console.WriteLine($"[Utils] Detected double tap from {eventWeaponFire.Userid.PlayerName}");
            
        if (_rapidFireBlockUserIds.Count == 0)
            Server.NextFrame(_rapidFireBlockUserIds.Clear);
            
        _rapidFireBlockUserIds.Add(index);

        if (hvh_rapidfire_print_message.Value)
        {
            if (_rapidFireBlockWarnings.TryGetValue(index, out var lastWarningTime) &&
                lastWarningTime + 3 > Server.CurrentTime)
                return HookResult.Continue;

            Server.PrintToChatAll($"{Colors.FormatMessage(_plugin.Config.ChatPrefix)} Player {ChatColors.Orange}{eventWeaponFire.Userid.PlayerName}{ChatColors.Grey} tried using {ChatColors.Orange}double tap{ChatColors.Grey}!");
            _rapidFireBlockWarnings[index] = Server.CurrentTime;
        }

        return HookResult.Continue;
    }

    public HookResult OnTakeDamage(DynamicHook h)
    {
        var damageInfo = h.GetParam<CTakeDamageInfo>(1);

        if (damageInfo.Attacker.Value == null)
            return HookResult.Continue;

        if (!_rapidFireBlockUserIds.Contains(damageInfo.Attacker.Index))
            return HookResult.Continue;
            
        switch (hvh_restrict_rapidfire.Value)
        {
            case (int)FixMethod.Allow:
            case (int)FixMethod.RapidFire:
                break;
            case (int)FixMethod.Ignore:
                damageInfo.Damage = 0;
                break;
            case (int)FixMethod.Reflect:
            case (int)FixMethod.ReflectSafe:
                damageInfo.Damage *= hvh_rapidfire_reflect_scale.Value;
                h.SetParam<CEntityInstance>(0, damageInfo.Attacker.Value);
                if (hvh_restrict_rapidfire.Value == (int)FixMethod.ReflectSafe)
                    damageInfo.DamageFlags |= TakeDamageFlags_t.DFLAG_PREVENT_DEATH; //https://docs.cssharp.dev/api/CounterStrikeSharp.API.Core.TakeDamageFlags_t.html
                break;
            default:
                break;
        }

        return HookResult.Changed;
    }
}