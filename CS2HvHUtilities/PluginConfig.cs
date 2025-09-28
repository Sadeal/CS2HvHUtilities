using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
using cs2hvh_utilities.Enums;

namespace cs2hvh_utilities;

public class Cs2CustomPhrasesSettings
{
    [JsonPropertyName("AdvertiseBlockerMessage")] public string AdvertiseBlockerMessage { get; set; } = "Advertisement is prohibited!";
    [JsonPropertyName("AdvertiseNameChangeTo")] public string AdvertiseNameChangeTo { get; set; } = "[Player1],[Player2],[Player3]";
    [JsonPropertyName("ResetScorePhrase")] public string ResetScorePhrase { get; set; } = "Player {Orange}{PlayerName}{Grey} has reset their stats!";
    [JsonPropertyName("ResetScoreAlreadyPhrase")] public string ResetScoreAlreadyPhrase { get; set; } = "Your stats are already 0.";
    [JsonPropertyName("ResetDeathPhrase")] public string ResetDeathPhrase { get; set; } = "Player {Orange}{PlayerName}{Grey} has reset their deaths!";
    [JsonPropertyName("ResetDeathAlreadyPhrase")] public string ResetDeathAlreadyPhrase { get; set; } = "Your deaths are already 0.";
    [JsonPropertyName("RapidFirePhrase")] public string RapidFirePhrase { get; set; } = "Player {Orange}{PlayerName}{Grey} tried using {Orange}double tap{Grey}!";
    [JsonPropertyName("TeleportPhrase")] public string TeleportPhrase { get; set; } = "Player {Orange}{PlayerName}{Grey} tried using {Orange}Teleport{Grey}!";
    [JsonPropertyName("WeaponRestrictPhrase")] public string WeaponRestrictPhrase { get; set; } = "{Orange}{WeaponName}{Grey} is restricted to {Orange}{RestrictAmount}{Grey} per team!";
    [JsonPropertyName("RageQuitPhrase")] public string RageQuitPhrase { get; set; } = "Player {Orange}{PlayerName}{Grey} has rage quit!";
}

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
    [JsonPropertyName("CustomPhrasesSettings")] public Cs2CustomPhrasesSettings CustomPhrasesSettings { get; set; } = new();
    [JsonPropertyName("ConfigVersion")] public override int Version { get; set; } = 10;
}