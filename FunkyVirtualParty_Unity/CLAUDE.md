# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**VRtistry** — a standalone multiplayer VR + WebGL Pictionary-style game by SynthLabs. One VR player draws on a poseable 3D mannequin while web clients (browsers/mobile) guess what was drawn.

The codebase also contains deprecated Party Crashers mini-games (Chase, Kaiju, Shootout, MazeGame) and their scenes. **Do not work on those** — the active focus is the VRtistry standalone experience (`VRtistryStandalone.unity` / `VRtistryStandaloneClient.unity`). The current active branch is `Normcore`.

## Build Commands

There are no CLI build scripts; builds are done from the Unity Editor. Key Editor menu tools:

- **Tools > Platform Switcher** — Switch between WebGL (client), Android/VR, and Standalone targets via `Assets/Scripts/Editor/PlatformSwitcher.cs`
- **SuperUnityBuild** — Automated multi-platform builds configured in `Assets/SuperUnityBuild/`
- **BuildCommand.cs** (`Assets/Scripts/Editor/BuildCommand.cs`) — WebGL build automation callable from CI

**Platform targets:**
- **WebGL** — web clients; Color Space: Gamma
- **Android/Oculus** — VR player; Color Space: Linear
- **Standalone** — VRtistry-only standalone mode

**Multi-editor testing:** ParrelSync (`com.veriorpies.parrelsync`) allows opening cloned editor instances to test VR + client simultaneously.

## Architecture

### Dual-Platform Pattern

Every scene and game manager has a VR version and a WebGL client version:
- `MainMenu.unity` / `MainMenuClient.unity`
- `ChaseGame.unity` / `ChaseGameClient.unity`
- `ChaseGameManager.cs` / `ChaseGameManagerWeb.cs`

VR scripts use full XR input; Web scripts use `KeyboardController.cs` and `ClientJoystick.cs`.

### Networking — Normcore

Package: `com.normalvr.normcore@2.17.0` from `normcore-registry.normcore.io`.

All networked state follows the **RealtimeComponent pattern**:
- `[Thing]SyncModel.cs` — extends `RealtimeModel`, contains `[RealtimeProperty]`-annotated fields
- `[Thing]Syncer.cs` — extends `RealtimeComponent<[Thing]SyncModel>`, subscribes to property change callbacks

**Connection flow:**
1. VR player in `MainMenu` creates a Normcore room; a 4-letter party code is shown
2. Web clients enter the code in `MainMenuClient`; `RealtimeSingletonWeb` joins that room
3. A `ClientPlayer` prefab is spawned (via `Realtime.Instantiate`) for each web client
4. `SceneChangerSyncer` broadcasts scene transitions to all connected players

**Core singletons:**
- `RealtimeSingleton.cs` — VR; manages room connection, avatar spawning
- `RealtimeSingletonWeb.cs` — WebGL; manages room join, keyboard input

### Player System

`ClientPlayer.cs` is the central web-client avatar, holding movement, customization, score, and drawing data. Its data is synced via `ClientSync.cs` / `ClientSyncModel.cs` (name, color, height, score, face drawing bytes).

`RealtimeAvatarManager.cs` manages VR avatar creation/destruction and fires events that game managers listen to.

### Game Manager Structure

Each mini-game has a consistent state machine driven by string states: `"countdown"`, `"game loop"`, `"time ended"`, etc. Game-specific controllers:

```
[Game]GameManager.cs          — VR host logic, state machine
[Game]GameManagerWeb.cs       — Client-side counterpart
[Game]VRPlayerController.cs   — VR input & actions
[Game]ClientPlayer.cs         — Per-client behaviour in that game
```

### VRtistry — Active Game

> See [docs/VRtistry-GameFlow.md](docs/VRtistry-GameFlow.md) for the full state machine, per-round sequence, data formats, and scoring details.

Active scripts live in `Assets/Scripts/3DPaint/` and `Assets/Scripts/3DPaint/Standalone/`. The standalone managers (`VRtistryGameManager.cs` / `VRtistryGameManagerWeb.cs`) supersede the deprecated party-suite versions (`ThreeDPaintGameManager.cs` / `ThreeDPaintGameManagerWeb.cs`).

- `VRtistryMainMenuManager.cs` / `VRtistryMainMenuManagerWeb.cs` — standalone lobby (includes client face-drawing step)
- `MannequinSolver.cs` / `MannequinSolverClient.cs` — IK pose synchronization
- `DrawingsSyncer.cs` / `DrawingModel.cs` — networked stroke data per round
- `GeminiFakeAnswersGenerator.cs` — Google Gemini API decoy answers (currently commented out in standalone)

### Key Directories

```
Assets/Scripts/
├── 3DPaint/          VRtistry drawing game (~30 files)
├── ChaseGame/        Chase mini-game
├── Kaiju/            Kaiju boss battle
├── ShootoutGame/     Fireball shootout
├── MazeGame/         Maze with coins
├── Main Menu/        Menu logic (VR + Web)
├── UI/               Shared UI components
├── Util/             Utilities (haptics, bandwidth, ping, etc.)
├── CustomAvatars/    RealtimeAvatar system
└── Editor/           Build tools, platform switcher

Assets/Scenes/        16 scenes (VR + Client pairs per game)
Assets/Prefabs/       Networked prefabs (Client Players, XR Avatars, Effects)
Assets/Normal/        Normcore resources
```

## External APIs & SDKs

- **Normcore** — multiplayer networking; docs in `Assets/Normal/Readme.pdf`
- **FMOD** — audio (Firelight Technologies); referenced in `Assets/Plugins/FMOD/`
- **AutoHand** — VR hand tracking & interaction (third-party asset)
- **AmplifyShaderEditor** — shader authoring
- **Google Gemini API** — used by `GeminiFakeAnswersGenerator.cs` for AI-generated decoy answers
- **UniTask** (`com.cysharp.unitask`) — async/await in Unity
