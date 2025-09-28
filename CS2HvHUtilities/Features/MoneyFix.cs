using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;

namespace cs2hvh_utilities.Features;

public class MoneyFix
{
    private readonly Plugin _plugin;
    public static readonly FakeConVar<bool> hvh_money_fix = new("hvh_money_fix", "Give {mp_maxmoney} to player on spawn if not a pistol round", true, ConVarFlags.FCVAR_REPLICATED);

    public MoneyFix(Plugin plugin)
    {
        _plugin = plugin;
        _plugin.RegisterFakeConVars(this);
        hvh_money_fix.Value = _plugin.Config.FixMoneyOnJoin;
    }

    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        if (!hvh_money_fix.Value)
            return HookResult.Continue;

        var player = @event.Userid;

        if(player == null || !player.IsValid)
            return HookResult.Continue;

        var maxRounds = 24;
        var maxRoundsVar = ConVar.Find("mp_maxrounds");
        if(maxRoundsVar != null)
        {
            maxRounds = maxRoundsVar.GetPrimitiveValue<int>();
        }

        var secondHalfFirstRound = 0;
        var halfTimeEnabledVar = ConVar.Find("mp_halftime");
        if (halfTimeEnabledVar != null)
        {
            if(halfTimeEnabledVar.GetPrimitiveValue<bool>() == true)
            {
                secondHalfFirstRound = (int)maxRounds / 2;
            }
        }

        CCSGameRules gamerules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!;

        if (gamerules.ITotalRoundsPlayed != 0 && gamerules.ITotalRoundsPlayed != secondHalfFirstRound)
        {
            var moneyServices = player.InGameMoneyServices!;
            var maxMoneyVar = ConVar.Find("mp_maxmoney");
            var maxMoney = 16000;
            if (maxMoneyVar != null)
            {
                maxMoney = maxMoneyVar.GetPrimitiveValue<int>();
            }

            moneyServices.Account += maxMoney - moneyServices.Account;

            Console.WriteLine($"[Utils] Giving spawned player money");
        }

        return HookResult.Continue;
    }
}
