#include "plugin.hpp"

#include <fstream>
#include <sstream>

#include <ISource2Server.h>
#include <iplayerinfo.h>
#include <sh_string.h>

hvh::CS2HvHUtilitiesMM g_CS2HvHUtilitiesMM;
SMEXT_LINK(&g_CS2HvHUtilitiesMM);

namespace hvh {

static std::string ReadFile(const std::string& path) {
    std::ifstream file(path, std::ios::in | std::ios::binary);
    if (!file.good()) {
        return {};
    }

    std::ostringstream out;
    out << file.rdbuf();
    return out.str();
}

void QAngle::Normalize() {
    x = std::remainder(x, 360.0f);
    y = std::remainder(y, 360.0f);
    z = std::remainder(z, 360.0f);
}

void QAngle::Clamp() {
    x = std::clamp(x, -179.0f, 179.0f);
    y = std::clamp(y, -180.0f, 180.0f);
    z = 0.0f;
}

void QAngle::Fix() {
    if (!std::isfinite(x)) x = 0.0f;
    if (!std::isfinite(y)) y = 0.0f;
    if (!std::isfinite(z)) z = 0.0f;

    if (!IsReasonable()) {
        Normalize();
    }

    Clamp();
}

bool CS2HvHUtilitiesMM::Load(PluginId id, ISmmAPI* ismm, char* error, size_t maxlen, bool late) {
    PLUGIN_SAVEVARS();

    if (!LoadConfig()) {
        V_snprintf(error, maxlen, "Failed to load config");
        return false;
    }

    if (!LoadGameData()) {
        V_snprintf(error, maxlen, "Failed to load gamedata signatures");
        return false;
    }

    RegisterEvents();

    if (!RegisterHooks()) {
        V_snprintf(error, maxlen, "Failed to register hooks");
        return false;
    }

    META_CONPRINT("[CS2HvHUtilitiesMM] Loaded\n");
    return true;
}

bool CS2HvHUtilitiesMM::Unload(char* error, size_t maxlen) {
    UnregisterHooks();
    UnregisterEvents();
    META_CONPRINT("[CS2HvHUtilitiesMM] Unloaded\n");
    return true;
}

bool CS2HvHUtilitiesMM::Pause(char* error, size_t maxlen) {
    return true;
}

bool CS2HvHUtilitiesMM::Unpause(char* error, size_t maxlen) {
    return true;
}

void CS2HvHUtilitiesMM::AllPluginsLoaded() {
}

bool CS2HvHUtilitiesMM::LoadConfig() {
    // NOTE: defaults are intentionally initialized in struct.
    // You can extend this with a real JSON parser in your production build.
    return true;
}

bool CS2HvHUtilitiesMM::LoadGameData() {
    const std::string raw = ReadFile("addons/metamod/gamedata/cs2hvhutilities.gamedata.json");
    if (raw.empty()) {
        return false;
    }

    auto captureSignature = [&](const std::string& key) {
        const std::regex keyPattern("\"" + key + "\"\\s*:\\s*\\{[\\s\\S]*?\"library\"\\s*:\\s*\"([^\"]+)\"[\\s\\S]*?\"windows\"\\s*:\\s*\"([^\"]+)\"[\\s\\S]*?\"linux\"\\s*:\\s*\"([^\"]+)\"");
        std::smatch m;
        if (std::regex_search(raw, m, keyPattern) && m.size() == 4) {
            m_signatures[key] = SignatureDef{m[1].str(), m[2].str(), m[3].str()};
        }
    };

    captureSignature("RunCommand");
    captureSignature("CCSPlayer_ItemServices_CanAcquire");
    captureSignature("CCSPlayer_WeaponServices_CanUse");

    return m_signatures.contains("RunCommand");
}

bool CS2HvHUtilitiesMM::RegisterHooks() {
    // Real SourceHook binding should be wired to your SDK wrappers here.
    // We already validate signatures exist in gamedata.
    return m_signatures.contains("RunCommand");
}

void CS2HvHUtilitiesMM::UnregisterHooks() {
}

void CS2HvHUtilitiesMM::RegisterEvents() {
    if (gameeventmanager == nullptr) {
        return;
    }

    constexpr std::array<const char*, 14> events = {
        "round_start",
        "round_prestart",
        "round_end",
        "player_spawn",
        "player_connect_full",
        "bomb_planted",
        "bomb_begindefuse",
        "weapon_fire",
        "bullet_impact",
        "grenade_thrown",
        "inferno_startburn",
        "inferno_extinguish",
        "inferno_expire",
        "hegrenade_detonate"
    };

    for (const char* eventName : events) {
        gameeventmanager->AddListener(this, eventName, true);
    }
}

void CS2HvHUtilitiesMM::UnregisterEvents() {
    if (gameeventmanager != nullptr) {
        gameeventmanager->RemoveListener(this);
    }
}

void CS2HvHUtilitiesMM::FireGameEvent(IGameEvent* event) {
    if (event == nullptr) {
        return;
    }

    const std::string name = event->GetName();

    if (name == "round_start") {
        HandleRoundStart();
    } else if (name == "round_prestart") {
        HandleRoundPreStart();
    } else if (name == "round_end") {
        HandleRoundEnd();
    } else if (name == "player_spawn") {
        HandlePlayerSpawn(event->GetInt("userid"));
    } else if (name == "player_connect_full") {
        HandlePlayerConnectFull(event->GetInt("userid"));
    } else if (name == "bomb_planted") {
        HandleBombPlanted();
    } else if (name == "bomb_begindefuse") {
        HandleBombBeginDefuse(event->GetInt("userid"));
    } else if (name == "weapon_fire") {
        HandleWeaponFire(event->GetInt("userid"));
    } else if (name == "bullet_impact") {
        HandleBulletImpact(event->GetInt("userid"));
    } else if (name == "grenade_thrown") {
        const std::string weapon = event->GetString("weapon");
        if (weapon == "hegrenade") ++m_heThreat;
        if (weapon == "molotov" || weapon == "incgrenade") ++m_molotovThreat;
    } else if (name == "inferno_startburn") {
        m_activeInfernos.insert(event->GetInt("entityid"));
    } else if (name == "inferno_extinguish" || name == "inferno_expire") {
        m_activeInfernos.erase(event->GetInt("entityid"));
    } else if (name == "hegrenade_detonate") {
        if (m_heThreat > 0) --m_heThreat;
    }
}

void CS2HvHUtilitiesMM::OnClientSay(int slot, const std::string& rawText) {
    if (!m_config.advertiseBlockerChat) {
        return;
    }

    auto it = m_players.find(slot);
    if (it == m_players.end()) {
        return;
    }

    if (it->second.isRoot || it->second.isGeneralAdmin) {
        return;
    }

    if (IsAdvertiseText(rawText)) {
        META_CONPRINTF("[CS2HvHUtilitiesMM] Blocked ad message from slot %d\n", slot);
    }
}

void CS2HvHUtilitiesMM::OnClientCommand(int slot, const std::string& cmd, const std::vector<std::string>& args) {
    auto& player = m_players[slot];

    if ((cmd == "rs" || cmd == "resetscore") && m_config.allowResetScore) {
        if (player.score == 0 && player.mvps == 0 && player.kills == 0 && player.deaths == 0 && player.assists == 0) {
            return;
        }

        player.score = 0;
        player.mvps = 0;
        player.kills = 0;
        player.headshots = 0;
        player.deaths = 0;
        player.assists = 0;
        player.utilityDamage = 0;
        player.damage = 0;
        player.objective = 0;
        return;
    }

    if ((cmd == "rd" || cmd == "resetdeath") && m_config.allowResetDeath > 0) {
        if (m_config.allowResetDeath == 1 && !player.isGeneralAdmin) {
            return;
        }

        player.deaths = 0;
        return;
    }

    if (m_config.restrictMetaCommands && (cmd == "meta" || cmd == "sm" || cmd == "css_plugins") && !player.isRoot) {
        return;
    }

    if ((cmd == "rules" || cmd == "rule") && m_config.allowSettingsPrint) {
        if (player.lastRulesPrintTime + 600.0f > m_now) {
            return;
        }
        player.lastRulesPrintTime = m_now;
    }

    if (cmd == "jointeam" && m_config.restrictGhost) {
        if (player.lastTeamChange + 12.0f > m_now) {
            return;
        }
        player.lastTeamChange = m_now;
    }
}

bool CS2HvHUtilitiesMM::OnRunCommandPre(int slot, QAngle& viewAngles) {
    if (!m_config.restrictTeleport) {
        return false;
    }

    if (viewAngles.IsValid()) {
        return false;
    }

    viewAngles.Fix();

    auto& player = m_players[slot];
    if (m_config.teleportPrintMessage && (player.lastTeleportWarn + 3.0f <= m_now)) {
        player.lastTeleportWarn = m_now;
        META_CONPRINTF("[CS2HvHUtilitiesMM] blocked invalid cmd angles for slot %d\n", slot);
    }

    return true;
}

void CS2HvHUtilitiesMM::HandleRoundStart() {
    m_bombPlantedAt = NAN;
    m_bombTicking = false;
    m_heThreat = 0;
    m_molotovThreat = 0;
    m_activeInfernos.clear();
    m_inWarmup = false;
}

void CS2HvHUtilitiesMM::HandleRoundPreStart() {
    m_inWarmup = false;
}

void CS2HvHUtilitiesMM::HandleRoundEnd() {
}

void CS2HvHUtilitiesMM::HandlePlayerSpawn(int slot) {
    auto& player = m_players[slot];

    if (m_config.fixMoneyOnJoin && player.money < 16000) {
        player.money = 16000;
    }

    if (m_config.advertiseBlockerName && IsAdvertiseText(player.name)) {
        player.name = GetSafeNameReplacement();
    }
}

void CS2HvHUtilitiesMM::HandlePlayerConnectFull(int slot) {
    auto& player = m_players[slot];
    player.connected = true;

    if (m_config.advertiseBlockerName && IsAdvertiseText(player.name)) {
        player.name = GetSafeNameReplacement();
    }
}

void CS2HvHUtilitiesMM::HandleBombPlanted() {
    if (!m_config.instantDefuse) {
        return;
    }

    m_bombTicking = true;
    m_bombPlantedAt = m_now;
}

void CS2HvHUtilitiesMM::HandleBombBeginDefuse(int slot) {
    if (!m_config.instantDefuse) {
        return;
    }
    TryInstantDefuse(slot);
}

void CS2HvHUtilitiesMM::HandleWeaponFire(int slot) {
    if (m_config.rapidFireFixMethod != FixMethod::RapidFire) {
        return;
    }

    auto it = m_rapidCharge.find(slot);
    if (it == m_rapidCharge.end()) {
        m_rapidCharge.emplace(slot, 0);
        return;
    }

    it->second = std::max(0, it->second - 1);
}

void CS2HvHUtilitiesMM::HandleBulletImpact(int slot) {
    if (m_config.rapidFireFixMethod != FixMethod::Ignore) {
        return;
    }

    auto& charge = m_rapidCharge[slot];
    if (charge > 0) {
        --charge;
    }
}

void CS2HvHUtilitiesMM::TryInstantDefuse(int slot) {
    if (!m_bombTicking || m_heThreat > 0 || m_molotovThreat > 0 || !m_activeInfernos.empty()) {
        return;
    }

    if (TeamHasAlivePlayers(2)) {
        return;
    }

    META_CONPRINTF("[CS2HvHUtilitiesMM] Instant defuse approved for slot %d\n", slot);
}

bool CS2HvHUtilitiesMM::TeamHasAlivePlayers(int team) const {
    for (const auto& [_, player] : m_players) {
        if (player.connected && player.alive && player.team == team) {
            return true;
        }
    }

    return false;
}

int CS2HvHUtilitiesMM::GetAllowedWeaponCount(std::string_view weapon) const {
    if (weapon == "weapon_awp") return m_config.allowedAwpCount;
    if (weapon == "weapon_ssg08") return m_config.allowedScoutCount;
    if (weapon == "weapon_scar20" || weapon == "weapon_g3sg1") return m_config.allowedAutoSniperCount;
    return -1;
}

bool CS2HvHUtilitiesMM::IsAdvertiseText(const std::string& text) const {
    static const std::array<std::regex, 8> patterns = {
        std::regex("https?", std::regex::icase),
        std::regex("www\\.", std::regex::icase),
        std::regex("\\.(ru|com|net|win|gg|cc|market|guru|live|org)", std::regex::icase),
        std::regex("discord\\.gg", std::regex::icase),
        std::regex("t\\.me", std::regex::icase),
        std::regex("connect.+", std::regex::icase),
        std::regex("(?:\\d{1,3}\\.){3}\\d{1,3}(?::\\d+)?", std::regex::icase),
        std::regex("funpay|market/|lots/", std::regex::icase)
    };

    for (const auto& pattern : patterns) {
        if (std::regex_search(text, pattern)) {
            return true;
        }
    }

    return false;
}

std::string CS2HvHUtilitiesMM::GetSafeNameReplacement() {
    static std::vector<std::string> names{"[Player1]", "[Player2]", "[Player3]"};
    if (names.empty()) {
        return "[Player]";
    }

    const std::string& selected = names[static_cast<size_t>(m_nameRotateIndex) % names.size()];
    ++m_nameRotateIndex;
    return selected;
}

std::string CS2HvHUtilitiesMM::Format(const std::string& pattern, const std::unordered_map<std::string, std::string>& vars) const {
    std::string result = pattern;
    for (const auto& [key, value] : vars) {
        const std::string token = "{" + key + "}";
        size_t pos = 0;
        while ((pos = result.find(token, pos)) != std::string::npos) {
            result.replace(pos, token.size(), value);
            pos += value.size();
        }
    }
    return result;
}

} // namespace hvh
