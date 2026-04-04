![Copyright Sadeal](https://img.shields.io/badge/Developer-Sadeal-blue)

# CS2HvHUtilities MetaMod Edition (C++)

Полный перенос проекта с CounterStrikeSharp (C#) на **MetaMod:Source + C++**.

## Что сделано

- Полностью удалена C# реализация.
- Добавлена MetaMod версия в директории `metamod/`.
- Добавлены:
  - CMake-проект сборки для `.so/.dll` MetaMod-плагина;
  - C++ реализация всех ключевых функций из исходного набора;
  - gamedata-файл с сигнатурой `RunCommand` и доп. сигнатурами для weapon hooks.

## Функции

- `!rs` / `!rd`
- Блок рекламы (чат и ник)
- Money fix
- Instant defuse
- Rapid fire logic
- Teleport/FakePitch fix через `RunCommand`
- Блокировка meta/css/sm команд для не-root
- Основа под weapon restriction hooks

## Структура

- `metamod/src/plugin.hpp` — интерфейсы, конфиг, состояния, сигнатуры.
- `metamod/src/plugin.cpp` — реализация фич и обработчиков событий.
- `metamod/gamedata/cs2hvhutilities.gamedata.json` — сигнатуры.
- `metamod/CMakeLists.txt` — сборка.

## Сборка

```bash
cmake -S metamod -B build-metamod \
  -DMETAMOD_SOURCE_INCLUDE_DIR=/path/to/metamod-core/core \
  -DSOURCE2SDK_INCLUDE_DIR=/path/to/source2sdk/public
cmake --build build-metamod -j
```

## Примечание

Для production-ветки рекомендуется:
- подключить полноценный JSON-парсер для config/gamedata;
- подставить в gamedata финальные проверенные сигнатуры под вашу версию сервера.
