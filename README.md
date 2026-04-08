# 🎮⚙️ Gwent Project - Game of Thrones Inspired Card Game (Unity) 🐺🐉

This repository contains a complete Unity card game inspired by Gwent, with a Game of Thrones theme and one major custom feature: a built-in DSL compiler that lets players create their own playable cards with scripted effects. ✨

If you want to understand this project from the root folder down to every gameplay and compiler module, this document is the full step-by-step guide. 📜

## 🗺️ Table of Contents

1. 📝 Project Summary
2. 🎯 What This Project Is About
3. 🧭 Repository Walkthrough from Root (Step by Step)
4. 📁 Source vs Generated Folders
5. 🗂️ Assets Folder Deep Dive
6. 🌊 Scene Flow and Player Journey
7. 🏗️ Gameplay Systems Architecture
8. 🃏 Card Model and Rules
9. ✨ Effects Engine
10. 🤖 AI Behavior
11. 🛠️ Custom Card Compiler (DSL) Pipeline
12. 📜 Full Script Map (File-by-File)
13. 🔧 Technologies and Dependencies
14. 🚀 How to Open and Run the Project
15. 📦 How to Build the Game
16. ⚠️ Troubleshooting and Maintenance Notes
17. 🌟 Suggested Next Improvements

---

## 📝 1) Project Summary

**Project Type:**
- 🎮 Unity 2D Card Strategy Game

**Core Idea:**
- ⚔️ Two players (human vs human or human vs AI) battle with faction decks in rounds.
- 🃏 Cards have types, positions, power values, and effects.
- 🌨️ Includes weather, boosts, bait mechanics, leader effects, and cemetery interactions.

**Unique Feature:**
- ✨ A custom compiler lets players define new cards through a mini language (DSL).
- 🃏 Compiled cards are added to a special custom deck that can be selected in deck selection.

**Engine and Language:**
- 🟦 Unity 2022.3.15f1
- 💻 C# scripts

---

## 🎯 2) What This Project Is About

At gameplay level, this is a tactical card battle game where each player manages:
- 🃏 A deck
- ✋ A hand
- 🛡️ Three combat rows (Melee, Range, Siege)
- ⭐ Boost slots
- 🌩️ Weather slot
- ⚰️ Cemetery

Players take turns placing cards and activating effects. At the end of each round, row power is calculated and the match ends when a player wins enough rounds. 🏆

At engineering level, the project combines:
- 🎮 Traditional Unity systems (scene flow, UI, drag & drop, audio, turn manager)
- 📊 Data-driven card creation
- 🧠 A complete lexer-parser-semantic-runtime pipeline for user-generated card logic

---

## 🧭 3) Repository Walkthrough from Root (Step by Step)

### Step 1 - Version Control and Repository Metadata
- 📦 `.git/`
- 📋 `.gitattributes`
- 🚫 `.gitignore`

### Step 2 - IDE and Local Tool State
- 💻 `.vs/`
- ⚙️ `.vsconfig`

### Step 3 - Unity/C# Solution Files
- 📄 `Assembly-CSharp.csproj`
- 📂 `Gwent Pro 2D.sln`
- 📂 `Gwent-Project.sln`

### Step 4 - Main Source Folder
- 🎯 `Assets/` → The most important folder (scenes, scripts, images, resources, fonts)

### Step 5 - Build Output
- 📦 `Builds/`

### Step 6 - Supporting Documents
- 📄 `Decks.pdf`
- 📄 `READ_ME.pdf`

### Other Folders
- 🗄️ `Library/`, `Logs/`, `obj/`, `Packages/`, `ProjectSettings/`, `UserSettings/`

---

## 📁 4) Source vs Generated Folders

**Primary Source Folders:**
- ✅ `Assets/`
- ✅ `Packages/`
- ✅ `ProjectSettings/`

**Generated / Local Folders:**
- ❌ `Library/`
- ❌ `Logs/`
- ❌ `obj/`
- ❌ `.vs/`
- ❌ `UserSettings/`
- ❌ `Builds/`

---

## 🗂️ 5) Assets Folder Deep Dive

### 🎬 Scenes
- 🏠 `Main.unity` - Main menu
- 🛡️ `ChosingDeck.unity` - Deck selection
- ⚔️ `Game.unity` - Main battle scene
- 🧪 `CreateCard.unity` - Card compiler scene

### 💻 Scripts
Organized by domain: core gameplay, cards & players, drag & drop, effects, menus, and compiler.

### 🖼️ Other Assets
- `Image/` → Card art and UI
- `Resources/` → Runtime loaded assets
- `game-of-thrones/` → GoT style font
- `TextMesh Pro/` → TMP assets

---

## 🌊 6) Scene Flow and Player Journey

**Main Journey:**
1. 🏠 Main menu
2. 🛡️ Deck selection
3. ⚔️ Battle scene

**Compiler Journey:**
1. Open CreateCard scene from menu
2. Write DSL code
3. Compile and create new cards
4. Use the custom compiler deck in game

---

## 🏗️ 7) Gameplay Systems Architecture

- 👑 `GameManager` → Global match coordinator
- 🧍 `Player` → Player state and logic
- 📊 `Panels` → Row and panel management

---

## 🃏 8) Card Model and Rules

**Card Fields:** Name, Faction, Power, Description, Type, Position, Effect

**Positions:** M, R, S and combinations  
**Types:** Golden, Silver, Climate, Clear, Bait, Increase, Leader

**Decks:** Stark 🐺, Targaryen 🐉, Dead ❄️ + Compiler deck

---

## ✨ 9) Effects Engine

Includes row buffs, weather effects, bait mechanics, card draw, power multipliers, and powerful leader abilities.

---

## 🤖 10) AI Behavior

AI chooses cards strategically using heuristics:
- Prioritizes strong cards
- Uses boosts and weather at the right moment
- Falls back to standard units

---

## 🛠️ 11) Custom Card Compiler (DSL) Pipeline

The most advanced part of the project:
- Lexer → Parser → Semantic Analysis → Execution
- Supports loops, conditionals, selectors, and board context
- Full UI in the CreateCard scene

---

## 📜 12) Full Script Map (File-by-File)

**Core:**
- `GameManager.cs` 👑
- `IA.cs` 🤖

**Cards & Players:**
- `Card.cs` 🃏
- `Player.cs` 🧍
- `CardDisplay.cs`

**Drag & Drop:**
- `Drag.cs` 🖱️
- `Drop.cs`

**Effects:**
- `Effects.cs` ✨

**Compiler:**
- `ProgramCompiler.cs`
- `Lexer/`, `Parser/`, `Card/`, `Effect/` folders

---

## 🔧 13) Technologies and Dependencies

- 🟦 Unity 2022.3.15f1
- 💻 C#
- TextMesh Pro, UGUI, and other Unity packages

---

## 🚀 14) How to Open and Run the Project

1. Install Unity 2022.3.15f1 via Unity Hub
2. Open the project
3. Load `Assets/Scenes/Main.unity`
4. Press Play 🎮

---

## 📦 15) How to Build the Game

1. Open Build Settings
2. Ensure all 4 scenes are added
3. Build for Windows 64-bit

---

## ⚠️ 16) Troubleshooting and Maintenance Notes

- Assets broken → Reimport all
- Text issues → Check TextMesh Pro
- Compiler errors → Verify names and parameters

---

## 🌟 17) Suggested Next Improvements

**Short term:**
- Add unit tests for the compiler
- Improve separation between UI and logic
- Move card data to JSON

**Mid term:**
- Deck save/load
- Localization
- Use Addressables instead of Resources.Load

---

## ✨ Closing Notes

This codebase is an ambitious educational project that combines real-time Unity gameplay, tactical card mechanics, and a complete mini-compiler embedded in the game.

Perfect for learning game architecture, language implementation, and custom runtime logic integration. 🔥

Enjoy exploring and creating your own cards!