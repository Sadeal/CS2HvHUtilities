using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using cs2hvh_utilities.Enums;

namespace cs2hvh_utilities.Features;

public class ChargeInfo
{
    public int CurrentCharge { get; set; }
    public int LastCharge { get; set; }
    public int CurrentChargeSecondary { get; set; }
    public string LastWeapon { get; set; } = string.Empty;
    public int LastChargeTick { get; set; } = 0;
    public int LastTickBullets {  get; set; } = 0;
    public bool NotToShow = false;
}


public class RapidFireFeature
{
    private readonly Plugin _plugin;

    private Dictionary<int, ChargeInfo> playerCharges = new Dictionary<int, ChargeInfo>();

    public RapidFireFeature(Plugin plugin)
    {
        _plugin = plugin;
        return;
    }

    public void OnTick()
    {
        if (RapidFire.hvh_restrict_rapidfire.Value != (int)FixMethod.RapidFire)
            return;

        var players = Utilities.GetPlayers();

        for (var i = 0; i < players.Count; i++)
        {
            if (players[i] == null || !players[i].IsValid|| players[i].IsBot || !players[i].PawnIsAlive)
                continue;

            var player = players[i];
            var playerSlot = player.Slot;
            var weapon = player.Pawn.Get()?.WeaponServices?.ActiveWeapon.Get();
            var weaponData = weapon?.GetVData<CCSWeaponBaseVData>();

            if (weapon == null || weapon.DesignerName == "weapon_knife" || weapon.DesignerName == "weapon_bayonet" || weaponData == null)
            {
                if (!playerCharges[playerSlot].NotToShow)
                {
                    player.PrintToCenterHtml("<b><font color='red'>Невозможно зарядить</font></b>");
                    playerCharges[playerSlot].NotToShow = true;
                }
                if (playerCharges[playerSlot] != null)
                {
                    playerCharges[playerSlot].CurrentCharge = 0;
                    playerCharges[playerSlot].LastCharge = 0;
                    playerCharges[playerSlot].LastChargeTick = Server.TickCount;
                }

                continue;
            }

            if (!playerCharges.ContainsKey(playerSlot))
            {
                playerCharges[playerSlot] = new ChargeInfo
                {
                    CurrentCharge = 0,
                    LastWeapon = weapon.DesignerName
                };
            }

            var chargeInfo = playerCharges[playerSlot];
            var currentTick = Server.TickCount;
            int ticksPerCharge = (int)Math.Round(weaponData!.CycleTime.Values[0] * 64);
            int ticksPerChargeSecondary = (int)Math.Round(weaponData!.CycleTime.Values[1] * 64);

            if (chargeInfo.LastWeapon != weapon.DesignerName || chargeInfo.LastTickBullets < weapon.Clip1)
            {
                chargeInfo.CurrentCharge = 0;
                chargeInfo.LastCharge = 0;
                chargeInfo.CurrentChargeSecondary = 0;
                chargeInfo.LastWeapon = weapon.DesignerName;
                chargeInfo.LastChargeTick = currentTick;
                chargeInfo.LastTickBullets = weapon.Clip1;
                continue;
            }

            int maxCharge = weapon.Clip1;
            if(maxCharge < 1 || weaponData.MaxClip1 <= 1)
            {
                if (!playerCharges[playerSlot].NotToShow)
                {
                    player.PrintToCenterHtml("<b><font color='red'>Невозможно зарядить</font></b>");
                    playerCharges[playerSlot].NotToShow = true;
                }
                if (playerCharges[playerSlot] != null)
                {
                    playerCharges[playerSlot].CurrentCharge = 0;
                    playerCharges[playerSlot].LastChargeTick = currentTick;
                    playerCharges[playerSlot].LastCharge = 0;
                }
                continue;
            }

            playerCharges[playerSlot].NotToShow = false;

            if (weapon.DesignerName == "weapon_awp" || weapon.DesignerName == "weapon_ssg08")
            {
                ticksPerCharge = (int)(ticksPerCharge / 2);
            }
            if (chargeInfo.CurrentCharge < maxCharge && chargeInfo.LastChargeTick + ticksPerCharge <= currentTick)
            {
                chargeInfo.LastCharge = chargeInfo.CurrentCharge;
                chargeInfo.CurrentCharge++;
                chargeInfo.LastChargeTick = currentTick;
            }

            if (chargeInfo.CurrentCharge <= 0)
                player.PrintToCenterHtml($"<font color='orange'>{chargeInfo.CurrentCharge}</font>"); 
            else 
                if (chargeInfo.CurrentCharge < maxCharge)
                    player.PrintToCenterHtml($"<font color='yellow'>{chargeInfo.CurrentCharge}</font>");
                else
                    player.PrintToCenterHtml($"<font color='green'>{chargeInfo.CurrentCharge}</font>");

            if (currentTick < weapon.NextPrimaryAttackTick && weapon.DesignerName != "weapon_revolver")
            {
                if (chargeInfo.CurrentCharge > 0 && chargeInfo.LastCharge != 0)
                {
                    weapon.NextPrimaryAttackTick = currentTick + 1;
                    weapon.NextPrimaryAttackTickRatio = 1 / 64;
                }
                else
                {
                    weapon.NextPrimaryAttackTick = chargeInfo.LastChargeTick + ticksPerCharge;
                    weapon.NextPrimaryAttackTickRatio = 1 / 64;
                }
            }

            if (currentTick < weapon.NextSecondaryAttackTick)
            {
                if (chargeInfo.CurrentChargeSecondary > 0 && weapon.DesignerName == "weapon_revolver" && chargeInfo.LastCharge != 0)
                {
                    weapon.NextSecondaryAttackTick = currentTick + 1;
                    weapon.NextSecondaryAttackTickRatio = 1 / 64;
                }
            }

            Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_nNextPrimaryAttackTick");
            Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_flNextPrimaryAttackTickRatio");
            Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_nNextSecondaryAttackTick");
            Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_flNextSecondaryAttackTickRatio");
        }
    }

    public HookResult OnWeaponFire(EventWeaponFire evt, GameEventInfo info)
    {
        if (RapidFire.hvh_restrict_rapidfire.Value != (int)FixMethod.RapidFire)
            return HookResult.Continue;

        if (evt.Userid?.Pawn?.Value?.WeaponServices?.ActiveWeapon?.Value == null)
            return HookResult.Continue;

        var player = evt.Userid;
        var playerSlot = player.Slot;
        if (player == null || player.IsBot || !player.PawnIsAlive)
            return HookResult.Continue;

        CBasePlayerWeapon firedWeapon = evt.Userid.Pawn.Value.WeaponServices.ActiveWeapon.Value!;

        var chargeInfo = playerCharges[playerSlot];
        if(chargeInfo != null)
        {
            chargeInfo.LastCharge = Math.Max(chargeInfo.CurrentCharge, 0);
            chargeInfo.CurrentCharge = Math.Max(chargeInfo.CurrentCharge - 1, 0);
            chargeInfo.LastTickBullets = firedWeapon.Clip1;
        }

        return HookResult.Continue;
    }
}