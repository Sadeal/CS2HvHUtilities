using System.Reflection;
using System.Text.RegularExpressions;

namespace cs2hvh_utilities.Extensions;
class Language
{
    private static readonly Plugin _plugin;

    private static Dictionary<string, string> en { get; } = new Dictionary<string, string>
    {
        { "AdvertiseBlockerMessage", "Advertisement is prohibited!" },
        { "ResetScorePhrase", "Player {Orange}{PlayerName}{Grey} has reset their stats!" },
        { "ResetScoreAlreadyPhrase", "Your stats are already 0." },
        { "ResetDeathPhrase", "Player {Orange}{PlayerName}{Grey} has reset their deaths!" },
        { "ResetDeathAlreadyPhrase", "Your deaths are already 0." },
        { "RapidFirePhrase", "Player {Orange}{PlayerName}{Grey} tried using {Orange}DoubleTap{Grey}!" },
        { "TeleportPhrase", "Player {Orange}{PlayerName}{Grey} tried using {Orange}Teleport{Grey}!" },
        { "WeaponRestrictPhrase", "{Orange}{WeaponName}{Grey} is restricted to {Orange}{RestrictAmount}{Grey} per team!" },
        { "ServerRules", "Server rules:" },
        { "GrenadesFF", "Grenades only friendly fire:" },
        { "TeleportFP", "Teleport/FakePitch:" },
        { "RapidDT", "RapidFire/DoubleTap:" },
        { "Method", "Method:" },
        { "MethodIgnore", "Method: {Orange}blocking shot registration" },
        { "MethodRapid", "Method: {Orange}enabling server-side RapidFire" },
        { "WeaponResctriction", "Weapon restriction:" },
        { "WeaponResctrictionPerTeam", "per team." },
        { "HelpMessage", "Type {Orange}!rules{Grey} to see these settings again" }
    };

    private static Dictionary<string, string> ru { get; } = new Dictionary<string, string>
    {
        { "AdvertiseBlockerMessage", "Реклама запрещена!" },
        { "ResetScorePhrase", "Игрок {Orange}{PlayerName}{Grey} сбросил свою статистику!" },
        { "ResetScoreAlreadyPhrase", "Твоя статистика уже равна 0." },
        { "ResetDeathPhrase", "Игрок {Orange}{PlayerName}{Grey} сбросил свои смерти!" },
        { "ResetDeathAlreadyPhrase", "Твои смерти уже равны 0." },
        { "RapidFirePhrase", "Игрок {Orange}{PlayerName}{Grey} попытался использовать {Orange}DoubleTap{Grey}!" },
        { "TeleportPhrase", "Игрок {Orange}{PlayerName}{Grey} попытался использовать {Orange}Телепорт{Grey}!" },
        { "WeaponRestrictPhrase", "{Orange}{WeaponName}{Grey} ограничено {Orange}{RestrictAmount}{Grey} на команду!" },
        { "ServerRules", "Правила сервера:" },
        { "GrenadesFF", "Гранаты наносят урон команде:" },
        { "TeleportFP", "Телепорт/ФейкПитч:" },
        { "RapidDT", "RapidFire/DoubleTap:" },
        { "Method", "Метод:" },
        { "MethodIgnore", "Метод: {Orange}блокировка регистрации выстрелов" },
        { "MethodRapid", "Метод: {Orange}включение серверного Rapidfire" },
        { "WeaponResctriction", "Ограничения оружий:" },
        { "WeaponResctrictionPerTeam", "на команду." },
        { "HelpMessage", "Введи {Orange}!rules{Grey}, чтобы снова увидеть параметры сервера" }
    };

    private static Dictionary<string, string> pl { get; } = new Dictionary<string, string>
    {
        { "AdvertiseBlockerMessage", "Reklama jest zabroniona!" },
        { "ResetScorePhrase", "Gracz {Orange}{PlayerName}{Grey} zresetował swoje statystyki!" },
        { "ResetScoreAlreadyPhrase", "Twoje statystyki już wynoszą 0." },
        { "ResetDeathPhrase", "Gracz {Orange}{PlayerName}{Grey} zresetował swoje zgony!" },
        { "ResetDeathAlreadyPhrase", "Twoje zgony już wynoszą 0." },
        { "RapidFirePhrase", "Gracz {Orange}{PlayerName}{Grey} próbował użyć {Orange}DoubleTap{Grey}!" },
        { "TeleportPhrase", "Gracz {Orange}{PlayerName}{Grey} próbował użyć {Orange}Teleportu{Grey}!" },
        { "WeaponRestrictPhrase", "{Orange}{WeaponName}{Grey} ograniczona do {Orange}{RestrictAmount}{Grey} na drużynę!" },
        { "ServerRules", "Zasady serwera:" },
        { "GrenadesFF", "Granaty – tylko ogień sojuszniczy:" },
        { "TeleportFP", "Teleport/FakePitch:" },
        { "RapidDT", "Szybki ogień/Podwójny strzał:" },
        { "Method", "Metoda:" },
        { "MethodIgnore", "Metoda: {Orange}blokowanie rejestracji strzałów" },
        { "MethodRapid", "Metoda: {Orange}włączenie RapidFire po stronie serwera" },
        { "WeaponResctriction", "Ograniczenie broni:" },
        { "WeaponResctrictionPerTeam", "na drużynę." },
        { "HelpMessage", "Wpisz {Orange}!rules{Grey}, aby ponownie zobaczyć te ustawienia" }
    };

    private static Dictionary<string, string> de { get; } = new Dictionary<string, string>
    {
        { "AdvertiseBlockerMessage", "Werbung ist verboten!" },
        { "ResetScorePhrase", "Spieler {Orange}{PlayerName}{Grey} hat seine Statistiken zurückgesetzt!" },
        { "ResetScoreAlreadyPhrase", "Deine Statistiken sind bereits 0." },
        { "ResetDeathPhrase", "Spieler {Orange}{PlayerName}{Grey} hat seine Tode zurückgesetzt!" },
        { "ResetDeathAlreadyPhrase", "Deine Tode sind bereits 0." },
        { "RapidFirePhrase", "Spieler {Orange}{PlayerName}{Grey} hat versucht, {Orange}DoubleTap{Grey} zu benutzen!" },
        { "TeleportPhrase", "Spieler {Orange}{PlayerName}{Grey} hat versucht, {Orange}Teleport{Grey} zu benutzen!" },
        { "WeaponRestrictPhrase", "{Orange}{WeaponName}{Grey} ist auf {Orange}{RestrictAmount}{Grey} pro Team beschränkt!" },
        { "ServerRules", "Serverregeln:" },
        { "GrenadesFF", "Granaten – nur Eigenbeschuss:" },
        { "TeleportFP", "Teleport/FakePitch:" },
        { "RapidDT", "Schnellfeuer/Doppelschuss:" },
        { "Method", "Methode:" },
        { "MethodIgnore", "Methode: {Orange}Blockieren der Schusserkennung" },
        { "MethodRapid", "Methode: {Orange}Aktivierung von serverseitigem Schnellfeuer" },
        { "WeaponResctriction", "Waffenbeschränkung:" },
        { "WeaponResctrictionPerTeam", "pro Team." },
        { "HelpMessage", "Gib {Orange}!rules{Grey} ein, um diese Einstellungen erneut zu sehen" }
    };

    private static Dictionary<string, string> cn { get; } = new Dictionary<string, string>
    {
        { "AdvertiseBlockerMessage", "禁止广告！" },
        { "ResetScorePhrase", "玩家 {Orange}{PlayerName}{Grey} 已重置统计数据！" },
        { "ResetScoreAlreadyPhrase", "你的统计数据已经是 0。" },
        { "ResetDeathPhrase", "玩家 {Orange}{PlayerName}{Grey} 已重置死亡次数！" },
        { "ResetDeathAlreadyPhrase", "你的死亡次数已经是 0。" },
        { "RapidFirePhrase", "玩家 {Orange}{PlayerName}{Grey} 尝试使用 {Orange}DoubleTap{Grey}！" },
        { "TeleportPhrase", "玩家 {Orange}{PlayerName}{Grey} 尝试使用 {Orange}传送{Grey}！" },
        { "WeaponRestrictPhrase", "{Orange}{WeaponName}{Grey} 每队限制 {Orange}{RestrictAmount}{Grey}！" },
        { "ServerRules", "服务器规则：" },
        { "GrenadesFF", "手榴弹仅友军伤害：" },
        { "TeleportFP", "传送/假俯仰：" },
        { "RapidDT", "快速射击/双击：" },
        { "Method", "方法：" },
        { "MethodIgnore", "方法: {Orange}阻止射击注册" },
        { "MethodRapid", "方法: {Orange}启用服务器端快速射击" },
        { "WeaponResctriction", "武器限制：" },
        { "WeaponResctrictionPerTeam", "每队。" },
        { "HelpMessage", "输入 {Orange}!rules{Grey} 以再次查看这些设置" }
    };

    private static Dictionary<string, string> kr { get; } = new Dictionary<string, string>
    {
        { "AdvertiseBlockerMessage", "광고는 금지되어 있습니다!" },
        { "ResetScorePhrase", "플레이어 {Orange}{PlayerName}{Grey}가 자신의 통계를 초기화했습니다!" },
        { "ResetScoreAlreadyPhrase", "당신의 통계는 이미 0입니다." },
        { "ResetDeathPhrase", "플레이어 {Orange}{PlayerName}{Grey}가 자신의 사망 횟수를 초기화했습니다!" },
        { "ResetDeathAlreadyPhrase", "당신의 사망 횟수는 이미 0입니다." },
        { "RapidFirePhrase", "플레이어 {Orange}{PlayerName}{Grey}가 {Orange}DoubleTap{Grey}을 사용하려고 했습니다!" },
        { "TeleportPhrase", "플레이어 {Orange}{PlayerName}{Grey}가 {Orange}텔레포트{Grey}를 사용하려고 했습니다!" },
        { "WeaponRestrictPhrase", "{Orange}{WeaponName}{Grey}는 팀당 {Orange}{RestrictAmount}{Grey}개로 제한됩니다!" },
        { "ServerRules", "서버 규칙:" },
        { "GrenadesFF", "수류탄 아군 피해만:" },
        { "TeleportFP", "텔레포트/페이크피치:" },
        { "RapidDT", "래피드파이어/더블탭:" },
        { "Method", "방법:" },
        { "MethodIgnore", "방법: {Orange}총알 등록 차단" },
        { "MethodRapid", "방법: {Orange}서버측 래피드파이어 활성화" },
        { "WeaponResctriction", "무기 제한:" },
        { "WeaponResctrictionPerTeam", "팀당." },
        { "HelpMessage", "{Orange}!rules{Grey}를 입력하면 설정을 다시 볼 수 있습니다" }
    };

    private static Dictionary<string, string> jp { get; } = new Dictionary<string, string>
    {
        { "AdvertiseBlockerMessage", "広告は禁止です！" },
        { "ResetScorePhrase", "プレイヤー {Orange}{PlayerName}{Grey} は統計をリセットしました！" },
        { "ResetScoreAlreadyPhrase", "あなたの統計はすでに0です。" },
        { "ResetDeathPhrase", "プレイヤー {Orange}{PlayerName}{Grey} は死亡数をリセットしました！" },
        { "ResetDeathAlreadyPhrase", "あなたの死亡数はすでに0です。" },
        { "RapidFirePhrase", "プレイヤー {Orange}{PlayerName}{Grey} は {Orange}DoubleTap{Grey}を使おうとしました！" },
        { "TeleportPhrase", "プレイヤー {Orange}{PlayerName}{Grey} は {Orange}テレポート{Grey}を使おうとしました！" },
        { "WeaponRestrictPhrase", "{Orange}{WeaponName}{Grey} はチームごとに {Orange}{RestrictAmount}{Grey} に制限されています！" },
        { "ServerRules", "サーバールール：" },
        { "GrenadesFF", "グレネードは味方へのダメージのみ：" },
        { "TeleportFP", "テレポート/フェイクピッチ：" },
        { "RapidDT", "ラピッドファイア/ダブルタップ：" },
        { "Method", "方法：" },
        { "MethodIgnore", "方法: {Orange}射撃登録をブロック" },
        { "MethodRapid", "方法: {Orange}サーバー側ラピッドファイアの有効化" },
        { "WeaponResctriction", "武器制限：" },
        { "WeaponResctrictionPerTeam", "チームごと。" },
        { "HelpMessage", "{Orange}!rules{Grey} を入力して再度設定を確認できます" }
    };

    public static string GetMessage(string message)
    {
        Dictionary<string, string> langDict;
        var language = _plugin.Config.Language;
        switch (language.ToLower())
        {
            case "ru":
                langDict = ru;
                break;
            case "en":
                langDict = en;
                break;
            case "pl":
                langDict = pl;
                break;
            case "de":
                langDict = de;
                break;
            case "zh":
                langDict = cn;
                break;
            case "ko":
                langDict = kr;
                break;
            case "ja":
                langDict = jp;
                break;
            default:
                langDict = en;
                break;
        }

        if (langDict.TryGetValue(message, out var phrase))
        {
            return phrase;
        }
        
        return "";
    }
}
