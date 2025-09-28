using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CSSharpUtils.Extensions;
using cs2hvh_utilities.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using CounterStrikeSharp.API.Modules.Utils;
using cs2hvh_utilities.Extensions;
using CounterStrikeSharp.API.Modules.Entities;

namespace cs2hvh_utilities;

public class Plugin : BasePlugin, IPluginConfig<Cs2HvhUtilitiesConfig>
{
    public override string ModuleName => "CS2 HVH Utilities";
    public override string ModuleVersion => "1.1.8";
    public override string ModuleAuthor => "Sadeal";
    public override string ModuleDescription => "Usefull utilities for CS2 HvH servers";
    public Cs2HvhUtilitiesConfig Config { get; set; } = new();

    private ServiceProvider? _serviceProvider;

    public required MemoryFunctionVoid<CCSPlayer_MovementServices, IntPtr> RunCommand;

    public void OnConfigParsed(Cs2HvhUtilitiesConfig config)
    {
        Config = config;
        
        RapidFire.hvh_restrict_rapidfire.Value = (int) Config.RapidFireFixMethod;
        RapidFire.hvh_rapidfire_print_message.Value = Config.RapidFirePrintMessage;
        RapidFire.hvh_rapidfire_reflect_scale.Value = Config.RapidFireReflectScale;
        TeleportFix.hvh_restrict_teleport.Value = Config.RestrictTeleport;
        TeleportFix.hvh_teleport_print_message.Value = Config.TeleportPrintMessage;
        WeaponRestrict.hvh_restrict_awp.Value = Config.AllowedAwpCount;
        WeaponRestrict.hvh_restrict_scout.Value = Config.AllowedScoutCount;
        WeaponRestrict.hvh_restrict_auto.Value = Config.AllowedAutoSniperCount;
        WeaponRestrict.hvh_allow_only_weapons.Value = Config.AllowedOnlyWeapons;
        ResetScore.hvh_resetscore.Value = Config.AllowResetScore;
        ResetScore.hvh_resetdeath.Value = Config.AllowResetDeath;
        MoneyFix.hvh_money_fix.Value = Config.FixMoneyOnJoin;
        MetaCommandsBlocker.hvh_restrict_meta_commands.Value = Config.RestrictMetaCommands;
    }

    public string FormatString(string input, Dictionary<string, object> fieldValues = null)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        StringBuilder result = new StringBuilder(input);

        foreach (var color in Colors.ColorMap)
        {
            string colorTag = $"{{{color.Key}}}";
            result.Replace(colorTag, color.Value.ToString());
        }

        if (fieldValues != null)
        {
            foreach (var field in fieldValues)
            {
                string fieldTag = $"{{{field.Key}}}";
                string fieldValue = field.Value?.ToString() ?? string.Empty;
                result.Replace(fieldTag, fieldValue);
            }
        }

        return result.ToString();
    }

    public override void Load(bool hotReload)
    {
        base.Load(hotReload);

        Console.WriteLine("[Utils] Start loading CS2HvHUtilities plugin");

        var services = new ServiceCollection();
        
        services.AddLogging(options =>
        {
            options.AddConsole();
        });

        services.AddSingleton(this);
        services.AddSingleton<ResetScore>();
        services.AddSingleton<RapidFire>();
        services.AddSingleton<RapidFireFeature>();
        services.AddSingleton<WeaponRestrict>();
        services.AddSingleton<TeleportFix>();
        services.AddSingleton<Misc>();
        services.AddSingleton<MetaCommandsBlocker>();
        services.AddSingleton<MoneyFix>();
        services.AddSingleton<AdBlocker>();
        services.AddSingleton<RapidFireFeature>();

        _serviceProvider = services.BuildServiceProvider();

        UseAdBlocker();
        UseMoneyFix();
        UseWeaponRestrict();
        UseRapidFireRestrict();
        UseResetScore();
        UseMisc();
        UseTeleport();
        UseMetaCommandsBlocker();
        UseRapidFireFeature();

        Console.WriteLine("[Utils] Finished loading CS2HvHUtilities plugin");
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        base.OnAllPluginsLoaded(hotReload);
    }

    private void UseAdBlocker()
    {
        Console.WriteLine("[Utils] Register valve mode");

        var adBlocker = _serviceProvider!.GetRequiredService<AdBlocker>();
        AddCommandListener("say", adBlocker.OnPlayerChat);
        AddCommandListener("say_team", adBlocker.OnPlayerChat);
        RegisterEventHandler<EventRoundPrestart>(adBlocker.OnRoundPreStart, HookMode.Pre);
        RegisterEventHandler<EventRoundStart>(adBlocker.OnRoundStart, HookMode.Pre);
        RegisterEventHandler<EventRoundEnd>(adBlocker.OnRoundEnd, HookMode.Pre);
        RegisterEventHandler<EventPlayerConnectFull>(adBlocker.OnPlayerConnect, HookMode.Pre);

        Console.WriteLine("[Utils] Finished registering valve mode");
    }

    private void UseTeleport()
    {
        var teleportFix = _serviceProvider!.GetRequiredService<TeleportFix>();
        
        Console.WriteLine("[Utils] Hooking run command");
        
        RunCommand = new MemoryFunctionVoid<CCSPlayer_MovementServices, IntPtr>(GameData.GetSignature("RunCommand"));
        RunCommand.Hook(teleportFix.RunCommand, HookMode.Pre);
        AddCommandListener("jointeam", teleportFix.ListenerJoinTeam);
    }

    private void UseMisc()
    {
        Console.WriteLine("[Utils] Register misc commands");
        
        var misc = _serviceProvider!.GetRequiredService<Misc>();
        RegisterConsoleCommandAttributeHandlers(misc);
        
        RegisterEventHandler<EventPlayerSpawn>((eventPlayerSpawn, _) =>
        {
            misc.AnnounceRules(eventPlayerSpawn.Userid);
            
            return HookResult.Continue;
        });
        
        Console.WriteLine("[Utils] Finished registering misc commands");
    }

    private void UseMoneyFix()
    {
        Console.WriteLine("[Utils] Register money fix");

        var moneyFix = _serviceProvider!.GetRequiredService<MoneyFix>();
        RegisterEventHandler<EventPlayerSpawn>(moneyFix.OnPlayerSpawn, HookMode.Post);
        
        Console.WriteLine("[Utils] Finished registering money fix");
    }

    private void UseMetaCommandsBlocker()
    {
        Console.WriteLine("[Utils] Register meta commands blocker");

        var metaCommandsBlocker = _serviceProvider!.GetRequiredService<MetaCommandsBlocker>();
        AddCommandListener("sm", metaCommandsBlocker.CommandListener_BlockOutput);
        AddCommandListener("meta", metaCommandsBlocker.CommandListener_BlockOutput);
        AddCommandListener("css_plugins", metaCommandsBlocker.CommandListener_BlockOutput);

        Console.WriteLine("[Utils] Finished registering meta commands blocker");
    }

    private void UseResetScore()
    {
        Console.WriteLine("[Utils] Register reset score command");
        
        var resetScore = _serviceProvider!.GetRequiredService<ResetScore>();
        RegisterConsoleCommandAttributeHandlers(resetScore);
        
        Console.WriteLine("[Utils] Finished registering reset score command");
    }
    private void UseRapidFireRestrict()
    {
        Console.WriteLine("[Utils] Register rapid fire listeners");
        
        var rapidFire = _serviceProvider!.GetRequiredService<RapidFire>();
        RegisterEventHandler<EventBulletImpact>(rapidFire.OnBulletImpact, HookMode.Pre);
        RegisterEventHandler<EventWeaponFire>(rapidFire.OnWeaponFire);
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(rapidFire.OnTakeDamage, HookMode.Pre);
        
        Console.WriteLine("[Utils] Finished registering rapid fire listeners");
    }
    private void UseRapidFireFeature()
    {
        Console.WriteLine("[Utils] Register rapid fire feature listeners");

        var rapidFire = _serviceProvider!.GetRequiredService<RapidFireFeature>();
        RegisterListener<Listeners.OnTick>(rapidFire.OnTick);
        RegisterEventHandler<EventWeaponFire>(rapidFire.OnWeaponFire, HookMode.Post);

        Console.WriteLine("[Utils] Finished registering rapid fire feature listeners");
    }
    private void UseWeaponRestrict()
    {
        Console.WriteLine("[Utils] Register weapon restriction listeners");
        
        var weaponRestrict = _serviceProvider!.GetRequiredService<WeaponRestrict>();
        VirtualFunctions.CCSPlayer_ItemServices_CanAcquireFunc.Hook(weaponRestrict.OnWeaponCanAcquire, HookMode.Pre);

        RegisterEventHandler<EventRoundAnnounceWarmup>((@event, info) =>
        {
            weaponRestrict.InWarmup = true;

            return HookResult.Continue;
        });

        RegisterEventHandler<EventBeginNewMatch>((@event, info) =>
        {
            weaponRestrict.InWarmup = false;

            return HookResult.Continue;
        });

        RegisterEventHandler<EventRoundPrestart>((@event, info) =>
        {
            weaponRestrict.InWarmup = false;

            return HookResult.Continue;
        });

        RegisterEventHandler<EventRoundStart>((@event, info) =>
        {
            weaponRestrict.InWarmup = false;

            return HookResult.Continue;
        });

        Console.WriteLine("[Utils] Finished registering weapon restriction listeners");
    }
    
    public override void Unload(bool hotReload)
    {
        if (_serviceProvider is null)
            return;
        
        var rapidFire = _serviceProvider.GetRequiredService<RapidFire>();
        
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Unhook(rapidFire.OnTakeDamage, HookMode.Pre);
        
        var weaponRestrict = _serviceProvider.GetRequiredService<WeaponRestrict>();
        VirtualFunctions.CCSPlayer_WeaponServices_CanUseFunc.Unhook(weaponRestrict.OnWeaponCanAcquire, HookMode.Pre);
        
        var teleportFix = _serviceProvider.GetRequiredService<TeleportFix>();
        RunCommand.Unhook(teleportFix.RunCommand, HookMode.Pre);
        
        base.Unload(hotReload);
    }
}