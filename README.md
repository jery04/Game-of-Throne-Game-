# 🎮⚙️ Gwent Project - Game of Thrones Inspired Card Game (Unity) 🐺🐉

This repository contains a complete Unity card game inspired by Gwent, with a Game of Thrones theme and one major custom feature: a built-in DSL compiler that lets players create their own playable cards with scripted effects. ✨

If you want to understand this project from the root folder down to every gameplay and compiler module, this document is the full step-by-step guide. 📜

## 🗺️ Table of Contents

1. [📝 Project Summary](#1-project-summary)  
2. [🎯 What This Project Is About](#2-what-this-project-is-about)  
3. [🧭 Repository Walkthrough from Root (Step by Step)](#3-repository-walkthrough-from-root-step-by-step)  
4. [📁 Source vs Generated Folders](#4-source-vs-generated-folders)  
5. [🗂️ Assets Folder Deep Dive](#5-assets-folder-deep-dive)  
6. [🌊 Scene Flow and Player Journey](#6-scene-flow-and-player-journey)  
7. [🏗️ Gameplay Systems Architecture](#7-gameplay-systems-architecture)  
8. [🃏 Card Model and Rules](#8-card-model-and-rules)  
9. [✨ Effects Engine](#9-effects-engine)  
10. [🤖 AI Behavior](#10-ai-behavior)  
11. [🛠️ Custom Card Compiler (DSL) Pipeline](#11-custom-card-compiler-dsl-pipeline)  
12. [📜 Full Script Map (File-by-File)](#12-full-script-map-file-by-file)  
13. [🔧 Technologies and Dependencies](#13-technologies-and-dependencies)  
14. [🚀 How to Open and Run the Project](#14-how-to-open-and-run-the-project)  
15. [📦 How to Build the Game](#15-how-to-build-the-game)  
16. [⚠️ Troubleshooting and Maintenance Notes](#16-troubleshooting-and-maintenance-notes)  
17. [🌟 Suggested Next Improvements](#17-suggested-next-improvements)

---

## 1) 📝 Project Summary

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

## 2) 🎯 What This Project Is About

At gameplay level, this is a tactical card battle game where each player manages:
- 🃏 A deck
- ✋ A hand
- 🛡️ Three combat rows (Melee, Range, Siege)
- ⭐ Boost slots
- 🌩️ Weather slot
- ⚰️ Cemetery

Players take turns placing cards and activating effects. At the end of each round, row power is calculated and the match concludes when a player wins enough rounds. 🏆

At engineering level, the project combines:
- 🎮 Traditional Unity game systems (scene flow, UI, drag and drop, audio, turn manager)
- 📊 Data-driven card creation with ScriptableObject-like structure
- 🧠 A custom lexer-parser-semantic-runtime pipeline for user-generated card logic

---

## 3) 🧭 Repository Walkthrough from Root (Step by Step)

The following section explains the repository from the root level, one item at a time.

### Step 1 - Version control and repository metadata
- 📦 `.git/`
- 📋 `.gitattributes`
- 🚫 `.gitignore`

### Step 2 - IDE and local tool state
- 💻 `.vs/`
- ⚙️ `.vsconfig`

### Step 3 - Unity/C# solution and project files
- 📄 `Assembly-CSharp.csproj`
- 📂 `Gwent Pro 2D.sln`
- 📂 `Gwent-Project.sln`

### Step 4 - Main source folder
- 🎯 `Assets/` → The most important source folder in Unity. Contains scenes, scripts, images, resources, fonts, and TMP assets.

### Step 5 - Build output artifacts
- 📦 `Builds/` → Already generated executable build output.

### Step 6 - Supporting documents
- 📄 `Decks.pdf`
- 📄 `READ_ME.pdf`

### Step 7 - Unclear or temporary root files
- `git commit -mVersion2.1`
- `s`

### Step 8 - Unity generated project cache
- 🗄️ `Library/`, `Logs/`, `obj/`

### Step 9 - Unity package and project configuration
- 📦 `Packages/`
- ⚙️ `ProjectSettings/`
- 👤 `UserSettings/`

### Step 10 - Repository documentation
- 📝 `README.md` → This file.

---

## 4) 📁 Source vs Generated Folders

**Primary source-of-truth folders:**
- ✅ `Assets/`
- ✅ `Packages/`
- ✅ `ProjectSettings/`

**Usually generated or local-only:**
- ❌ `Library/`
- ❌ `Logs/`
- ❌ `obj/`
- ❌ `.vs/`
- ❌ `UserSettings/`
- ❌ `Builds/`

**Practical rule:** Focus on `Assets/`, `Packages/`, and `ProjectSettings/` when modifying gameplay or content.

---

## 5) 🗂️ Assets Folder Deep Dive

### 🎬 `Assets/Scenes/`
- 🏠 `Main.unity` - Main menu scene
- 🛡️ `ChosingDeck.unity` - Deck and player setup scene
- ⚔️ `Game.unity` - Main battle scene
- 🧪 `CreateCard.unity` - Custom card compiler scene

**Build order:**
1. `Main.unity`
2. `ChosingDeck.unity`
3. `Game.unity`
4. `CreateCard.unity`

### 💻 `Assets/Script/`
Main C# codebase grouped by domains (core flow, cards, drag & drop, effects, menus, compiler).

### 🖼️ Other folders
- `Image/` → Card art and UI icons
- `Resources/` → Audio and runtime assets
- `game-of-thrones/` → Game of Thrones style font
- `TextMesh Pro/` → TMP shaders and fonts

---

## 6) 🌊 Scene Flow and Player Journey

**Main journey:**
1. 🏠 Main menu
2. 🛡️ Deck selection scene
3. ⚔️ Game scene (full match)

**Optional compiler journey:**
1. Open card compiler from main menu
2. Write DSL code
3. Compile successfully
4. Use the compiler deck in game

---

## 7) 🏗️ Gameplay Systems Architecture

- 👑 `GameManager` → Global match coordinator
- 🧍 `Player` → Runtime player state and operations
- 📊 `Panels` → Row and panel management

---

## 8) 🃏 Card Model and Rules

**Main card fields:**
- Name, Faction, Power, Description, Artwork, Card type, Position constraints, Effect

**Card positions:** `M`, `R`, `S`, combinations (`MR`, `MS`, `RS`, `MRS`), and special zones (`I`, `C`, `L`)

**Card kinds:** Golden, Silver, Climate, Clear, Bait, Increase, Leader

**Predefined decks:** Stark 🐺, Targaryen 🐉, Dead ❄️, and `deckCompiler`

---

## 9) ✨ Effects Engine

Effects implemented in the static `Effects` class:
- Row buffs, weather damage, remove max/min cards, draw cards, power multipliers, row clear, bait return, and leader-specific effects (Jon Snow, Daenerys, Night King).

---

## 10) 🤖 AI Behavior

AI class `IA` controls automated turns using heuristics:
- Prioritizes strong hero cards
- Uses increase and climate cards strategically
- Falls back to standard units

---

## 11) 🛠️ Custom Card Compiler (DSL) Pipeline

The most advanced subsystem:
- Lexer → Parser → Semantic Analysis → Execution
- Supports effects, cards, loops, conditionals, selectors, and board context
- Full UI in `CreateCard.unity` scene

---

## 12) 📜 Full Script Map (File-by-File)

**Core gameplay:**
- `GameManager.cs`
- `IA.cs`

**Card & Player:**
- `Card.cs`
- `Player.cs`
- `CardDisplay.cs`
- `DeckManager.cs`

**Drag & Drop:**
- `Drag.cs`
- `Drop.cs`
- `DropCard.cs`

**Effects:**
- `Effects.cs`
- `EventClick.cs`

**Menus & Panels:**
- `MainMenu.cs`
- `Chose.cs`
- `Panels.cs`
- `KeepMusic.cs`

**Compiler:**
- `ProgramCompiler.cs`
- `Button_Run.cs`
- `Lexer/`, `Parser/`, `Card/`, `Effect/`, `Expression/`, `Scope/`, `Utils/`

---

## 13) 🔧 Technologies and Dependencies

- 🟦 Unity 2022.3.15f1
- 💻 C#
- TextMesh Pro, UGUI, and other Unity packages

---

## 14) 🚀 How to Open and Run the Project

**Prerequisites:**
- Unity Hub
- Unity Editor 2022.3.15f1

**Steps:**
1. Open the project in Unity Hub
2. Open `Assets/Scenes/Main.unity`
3. Press Play 🎮

**Recommended test:**
- Start game → Choose decks → Play with drag & drop

---

## 15) 📦 How to Build the Game

1. Open **Build Settings**
2. Verify the 4 scenes are added and enabled
3. Select **Standalone Windows 64-bit**
4. Build

---

## 16) ⚠️ Troubleshooting and Maintenance Notes

- Scene references broken → Reimport assets
- Text not rendering → Import TextMesh Pro essentials
- Compiler errors → Check field names and effect parameters
- Clean temporary files: `git commit -mVersion2.1` and `s`

---

## 17) 🌟 Suggested Next Improvements

**Short-term:**
- Replace hardcoded effect lists with metadata
- Add unit tests for compiler
- Move card definitions to JSON/ScriptableObjects

**Mid-term:**
- Deck save/load system
- Localization support
- Replace `Resources.Load` with Addressables

---

## ✨ Closing Notes

This codebase is an ambitious educational project that combines real-time Unity gameplay, tactical card mechanics, and a complete mini-compiler embedded into the game.

It is especially useful for studying game architecture, scripting language implementation, and integrating custom runtime logic into a playable experience. 🔥

Enjoy building and creating your own legendary cards! 🃏