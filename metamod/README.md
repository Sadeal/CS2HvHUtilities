# CS2HvHUtilities MetaMod (C++)

Порт плагина на MetaMod:Source + Source2 SDK.

## Что включено

- Каркас плагина `ISmmPlugin` + listener игровых ивентов.
- Реализация логики всех основных фич из C# версии:
  - `!rs` / `!rd`
  - anti-ad (чат + ник)
  - money fix
  - instant defuse
  - rapid fire state-machine
  - teleport/fakepitch protection через pre-hook `RunCommand`
  - блокировка `meta` / `sm` / `css_plugins`
- Загрузка сигнатур из `addons/metamod/gamedata/cs2hvhutilities.gamedata.json`.

## Сборка

```bash
cmake -S metamod -B build-metamod \
  -DMETAMOD_SOURCE_INCLUDE_DIR=/path/to/metamod-core/core \
  -DSOURCE2SDK_INCLUDE_DIR=/path/to/source2sdk/public
cmake --build build-metamod -j
```

## Важно

- Для production рекомендуется заменить regex-парсинг gamedata/config на полноценный JSON-парсер (например, nlohmann/json).
- Если у вас уже есть готовые сигнатуры из CounterStrikeSharp gamedata — подставьте их в файл gamedata.
