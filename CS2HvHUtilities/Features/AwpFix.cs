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
using static CounterStrikeSharp.API.Core.Listeners;

namespace cs2hvh_utilities.Features;

public class AwpFix
{
    private readonly Plugin _plugin;
    public static readonly FakeConVar<int> hvh_awp_bullets = new("hvh_awp_bullets", "Awp default clip bullets amount", 5, ConVarFlags.FCVAR_REPLICATED, new RangeValidator<int>(5, 30));

    public AwpFix(Plugin plugin)
    {
        _plugin = plugin;
        _plugin.RegisterFakeConVars(this);
        hvh_awp_bullets.Value = (int)_plugin.Config.AwpBullets;
    }

    public void OnEntityCreated(CEntityInstance entity)
    {
        if (hvh_awp_bullets.Value <= 5)
            return;

        if (hvh_awp_bullets.Value > 30)
            hvh_awp_bullets.Value = 30;

        var value = hvh_awp_bullets.Value;

        if (entity?.Entity == null || !entity.IsValid ||
                    !string.Equals(entity.DesignerName, "weapon_awp", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Server.NextFrame(() =>
        {
            var weapon = new CBasePlayerWeapon(entity.Handle);

            if (!weapon.IsValid) return;

            var csWeapon = weapon.As<CCSWeaponBase>();
            if (csWeapon == null) return;

            if (csWeapon.VData != null)
            {
                csWeapon.VData.MaxClip1 = value;
                csWeapon.VData.DefaultClip1 = value;
            }

            csWeapon.Clip1 = value;

            Utilities.SetStateChanged(csWeapon, "CBasePlayerWeapon", "m_iClip1");
        });
    }
}