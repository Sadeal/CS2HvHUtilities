#pragma once

#include <array>
#include <cmath>
#include <cstdint>
#include <optional>
#include <regex>
#include <string>
#include <string_view>
#include <unordered_map>
#include <unordered_set>
#include <vector>

#include <ISmmPlugin.h>
#include <igameevents.h>
#include <sh_vector.h>

namespace hvh {

enum class FixMethod : int {
    Allow = 0,
    Ignore = 1,
    Reflect = 2,
    ReflectSafe = 3,
    RapidFire = 4,
};

struct CustomPhrases {
    std::string advertiseBlockerMessage{"Advertisement is prohibited!"};
    std::string advertiseNameChangeTo{"[Player1],[Player2],[Player3]"};
    std::string resetScorePhrase{"Player {PlayerName} has reset their stats!"};
    std::string resetScoreAlreadyPhrase{"Your stats are already 0."};
    std::string resetDeathPhrase{"Player {PlayerName} has reset their deaths!"};
    std::string resetDeathAlreadyPhrase{"Your deaths are already 0."};
    std::string rapidFirePhrase{"Player {PlayerName} tried using double tap!"};
    std::string teleportPhrase{"Player {PlayerName} tried using Teleport!"};
    std::string weaponRestrictPhrase{"{WeaponName} is restricted to {RestrictAmount} per team!"};
    std::string serverRules{"Server rules:"};
    std::string grenadesFF{"Grenades only friendly fire:"};
    std::string teleportFP{"Teleport/FakePitch:"};
    std::string rapidDT{"RapidFire/DoubleTap:"};
    std::string weaponRestriction{"Weapon restriction:"};
    std::string weaponRestrictionPerTeam{"per team."};
    std::string helpMessage{"Type !rules to see these settings again"};
};

struct PluginConfig {
    FixMethod rapidFireFixMethod{FixMethod::Ignore};
    bool rapidFirePrintMessage{false};
    float rapidFireReflectScale{1.0f};
    bool fixMoneyOnJoin{true};
    int allowedAwpCount{-1};
    int allowedScoutCount{-1};
    int allowedAutoSniperCount{-1};
    bool allowAllWeaponsOnWarmup{true};
    std::string allowWeaponsForFlag{"@css/restrict"};
    std::string allowedOnlyWeapons;
    int awpBullets{10};
    bool instantDefuse{true};
    bool utilitiesFriendlyFire{true};
    bool restrictTeleport{true};
    bool teleportPrintMessage{false};
    bool restrictGhost{true};
    bool allowAdPrint{false};
    bool advertiseBlockerName{true};
    bool advertiseBlockerChat{true};
    bool allowSettingsPrint{true};
    bool allowResetScore{true};
    bool showResetScorePrint{true};
    int allowResetDeath{1};
    std::string resetDeathFlag{"@css/general"};
    bool showResetDeathPrint{false};
    bool restrictMetaCommands{true};
    std::string chatPrefix{"[Utils]"};
    CustomPhrases phrases{};
    int configVersion{11};
};

struct SignatureDef {
    std::string library;
    std::string windows;
    std::string linux;
};

struct QAngle {
    float x{0.0f};
    float y{0.0f};
    float z{0.0f};

    [[nodiscard]] bool IsFinite() const {
        return std::isfinite(x) && std::isfinite(y) && std::isfinite(z);
    }

    [[nodiscard]] bool IsReasonable() const {
        return x >= -89.0f && x <= 89.0f && y >= -180.0f && y <= 180.0f && z == 0.0f;
    }

    [[nodiscard]] bool IsValid() const {
        return IsFinite() && IsReasonable();
    }

    void Normalize();
    void Clamp();
    void Fix();
};

struct PlayerState {
    bool connected{false};
    bool alive{false};
    bool isBot{false};
    bool isRoot{false};
    bool isGeneralAdmin{false};
    int team{0};
    int money{0};
    int score{0};
    int mvps{0};
    int kills{0};
    int headshots{0};
    int deaths{0};
    int assists{0};
    int utilityDamage{0};
    int damage{0};
    int objective{0};
    std::string name;
    float lastRulesPrintTime{0.0f};
    float lastTeleportWarn{0.0f};
    float lastRapidWarn{0.0f};
    float lastTeamChange{0.0f};
};

class CS2HvHUtilitiesMM final : public ISmmPlugin, public IGameEventListener2 {
public:
    bool Load(PluginId id, ISmmAPI* ismm, char* error, size_t maxlen, bool late) override;
    bool Unload(char* error, size_t maxlen) override;
    bool Pause(char* error, size_t maxlen) override;
    bool Unpause(char* error, size_t maxlen) override;
    void AllPluginsLoaded() override;

    const char* GetAuthor() override { return "Sadeal / port by Codex"; }
    const char* GetName() override { return "CS2HvHUtilitiesMM"; }
    const char* GetDescription() override { return "MetaMod rewrite of CS2 HvH utilities"; }
    const char* GetURL() override { return "https://github.com/HvH-gg"; }
    const char* GetLicense() override { return "GPLv3"; }
    const char* GetVersion() override { return "2.0.0-metamod"; }
    const char* GetDate() override { return __DATE__; }
    const char* GetLogTag() override { return "CS2HVHMM"; }

    void FireGameEvent(IGameEvent* event) override;
    int GetEventDebugID() override { return EVENT_DEBUG_ID_INIT; }

    void OnClientSay(int slot, const std::string& rawText);
    void OnClientCommand(int slot, const std::string& cmd, const std::vector<std::string>& args);
    bool OnRunCommandPre(int slot, QAngle& viewAngles);

private:
    bool LoadConfig();
    bool LoadGameData();
    bool RegisterHooks();
    void UnregisterHooks();
    void RegisterEvents();
    void UnregisterEvents();

    void HandleRoundStart();
    void HandleRoundPreStart();
    void HandleRoundEnd();
    void HandlePlayerSpawn(int slot);
    void HandlePlayerConnectFull(int slot);
    void HandleBombPlanted();
    void HandleBombBeginDefuse(int slot);
    void HandleWeaponFire(int slot);
    void HandleBulletImpact(int slot);

    void TryInstantDefuse(int slot);
    bool TeamHasAlivePlayers(int team) const;
    int GetAllowedWeaponCount(std::string_view weapon) const;
    bool IsAdvertiseText(const std::string& text) const;
    std::string GetSafeNameReplacement();
    std::string Format(const std::string& pattern, const std::unordered_map<std::string, std::string>& vars = {}) const;

private:
    PluginConfig m_config{};
    std::unordered_map<std::string, SignatureDef> m_signatures{};
    std::unordered_map<int, PlayerState> m_players{};
    std::unordered_map<int, int> m_rapidCharge{};
    std::unordered_set<int> m_activeInfernos{};

    float m_now{0.0f};
    float m_bombPlantedAt{NAN};
    bool m_bombTicking{false};
    bool m_inWarmup{false};
    int m_molotovThreat{0};
    int m_heThreat{0};
    int m_nameRotateIndex{0};
};

} // namespace hvh

extern hvh::CS2HvHUtilitiesMM g_CS2HvHUtilitiesMM;
PLUGIN_GLOBALVARS();
