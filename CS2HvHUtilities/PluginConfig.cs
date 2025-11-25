using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
using cs2hvh_utilities.Enums;

namespace cs2hvh_utilities;

public partial class Cs2HvhUtilitiesConfig : BasePluginConfig
{
    [JsonPropertyName("RapidFireFixMethod")] public FixMethod RapidFireFixMethod { get; set; } = FixMethod.Ignore;
    [JsonPropertyName("RapidFirePrintMessage")] public bool RapidFirePrintMessage { get; set; } = true;
    [JsonPropertyName("RapidFireReflectScale")] public float RapidFireReflectScale { get; set; } = 1f;
    [JsonPropertyName("FixMoneyOnJoin")] public bool FixMoneyOnJoin { get; set; } = true;
    [JsonPropertyName("AllowedAwpCount")] public int AllowedAwpCount { get; set; } = -1;
    [JsonPropertyName("AllowedScoutCount")] public int AllowedScoutCount { get; set; } = -1;
    [JsonPropertyName("AllowedAutoSniperCount")] public int AllowedAutoSniperCount { get; set; } = -1;
    [JsonPropertyName("AllowAllWeaponsOnWarmup")] public bool AllowAllWeaponsOnWarmup { get; set; } = true;
    [JsonPropertyName("AllowWeaponsForFlag")] public string AllowWeaponsForFlag { get; set; } = "@css/restrict";
    [JsonPropertyName("AllowedOnlyWeapons")] public string AllowedOnlyWeapons { get; set; } = "";
    [JsonPropertyName("UtilitiesFriendlyFire")] public bool UtilitiesFriendlyFire { get; set; } = true;
    [JsonPropertyName("RestrictTeleport")] public bool RestrictTeleport { get; set; } = true;
    [JsonPropertyName("TeleportPrintMessage")] public bool TeleportPrintMessage { get; set; } = true;
    [JsonPropertyName("RestrictGhost")] public bool RestrictGhost { get; set; } = true;
    [JsonPropertyName("AllowAdPrint")] public bool AllowAdPrint { get; set; } = true;
    [JsonPropertyName("AdvertiseBlockerName")] public bool AdvertiseBlockerName { get; set; } = true;
    [JsonPropertyName("AdvertiseNameChangeTo")] public string AdvertiseNameChangeTo { get; set; } = "[Player1],[Player2],[Player3]";
    [JsonPropertyName("AdvertiseBlockerChat")] public bool AdvertiseBlockerChat { get; set; } = true;
    [JsonPropertyName("AllowSettingsPrint")] public bool AllowSettingsPrint { get; set; } = true;
    [JsonPropertyName("AllowResetScore")] public bool AllowResetScore { get; set; } = true;
    [JsonPropertyName("ShowResetScorePrint")] public bool ShowResetScorePrint { get; set; } = true;
    [JsonPropertyName("AllowResetDeath")] public int AllowResetDeath { get; set; } = 1;
    [JsonPropertyName("ResetDeathFlag")] public string ResetDeathFlag { get; set; } = "@css/general";
    [JsonPropertyName("ShowResetDeathPrint")] public bool ShowResetDeathPrint { get; set; } = true;
    [JsonPropertyName("RestrictMetaCommands")] public bool RestrictMetaCommands { get; set; } = true;
    [JsonPropertyName("AllowRageQuit")] public bool AllowRageQuit { get; set; } = true;
    [JsonPropertyName("ChatPrefix")] public string ChatPrefix { get; set; } = "[{Blue}Utils{Default}]";
    [JsonPropertyName("Language")] public string Language { get; set; } = "en";
    [JsonPropertyName("ConfigVersion")] public override int Version { get; set; } = 10;
}