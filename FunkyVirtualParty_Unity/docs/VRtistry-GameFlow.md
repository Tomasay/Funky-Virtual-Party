# VRtistry Game Flow & Logic

VRtistry is a Pictionary-style game: clients type answers to a prompt, the VR player draws the chosen answer on a poseable mannequin, then everyone guesses.

## Active Scenes

| Scene | Purpose |
|---|---|
| `VRtistryStandalone.unity` | VR player host — **active** |
| `VRtistryStandaloneClient.unity` | WebGL clients — **active** |

> `VRtistry.unity` / `VRtistryClient.unity` are deprecated (used in the old Party Crashers suite). Do not touch those.

## Key Scripts

All active gameplay logic lives in `Assets/Scripts/3DPaint/` and `Assets/Scripts/3DPaint/Standalone/`.

| Script | Role |
|---|---|
| [VRtistryGameManager.cs](../Assets/Scripts/3DPaint/Standalone/VRtistryGameManager.cs) | VR-side state machine + host logic |
| [VRtistryGameManagerWeb.cs](../Assets/Scripts/3DPaint/Standalone/VRtistryGameManagerWeb.cs) | Client-side state reactions + UI |
| [VRtistryMainMenuManager.cs](../Assets/Scripts/3DPaint/Standalone/VRtistryMainMenuManager.cs) | VR lobby: shows party code, tracks client readiness, starts game |
| [VRtistryMainMenuManagerWeb.cs](../Assets/Scripts/3DPaint/Standalone/VRtistryMainMenuManagerWeb.cs) | Client lobby: join flow + face-drawing step |
| [VRtistrySyncer.cs](../Assets/Scripts/3DPaint/VRtistrySyncer.cs) | Singleton Normcore `RealtimeComponent` — single source of truth for all game state |
| [VRtistrySyncModel.cs](../Assets/Scripts/3DPaint/VRtistrySyncModel.cs) | Normcore model — all synced fields with their formats |
| [DrawingsSyncer.cs](../Assets/Scripts/3DPaint/DrawingsSyncer.cs) | Syncs stroke/paint/pose data per round |
| [PaintBrush.cs](../Assets/Scripts/3DPaint/PaintBrush.cs) | VR drawing tool; also drives the client-side stroke-reveal animation |
| [MannequinSolver.cs](../Assets/Scripts/3DPaint/MannequinSolver.cs) | VR-side IK poser for the mannequin |
| [MannequinSolverClient.cs](../Assets/Scripts/3DPaint/MannequinSolverClient.cs) | Client-side IK pose reconstruction |
| [GeminiFakeAnswersGenerator.cs](../Assets/Scripts/3DPaint/GeminiFakeAnswersGenerator.cs) | Calls Google Gemini API to generate AI decoy answers (currently commented out in standalone) |
| [ThreeDPaintGlobalVariables.cs](../Assets/Scripts/3DPaint/ThreeDPaintGlobalVariables.cs) | All tuning constants and point values |
| [VRtistryClientPlayer.cs](../Assets/Scripts/3DPaint/VRtistryClientPlayer.cs) | Per-client behaviour in this game |
| [VRtistryVRPlayerController.cs](../Assets/Scripts/3DPaint/VRtistryVRPlayerController.cs) | VR player input handling |
| [AnswerOptionButton.cs](../Assets/Scripts/3DPaint/AnswerOptionButton.cs) | UI component for a single answer/guess choice |

> The deprecated files `ThreeDPaintGameManager.cs` and `ThreeDPaintGameManagerWeb.cs` are the party-suite equivalents of `VRtistryGameManager.cs` / `VRtistryGameManagerWeb.cs`. They share the same `VRtistrySyncer` and most shared scripts, but are not in active use.

## State Machine

`VRtistrySyncer.State` (a synced string) drives both sides. `VRtistryGameManager` (VR host) **sets** state; both managers react to `OnStateChangeEvent`.

```
[lobby]  VRtistryMainMenuManager / VRtistryMainMenuManagerWeb
    ↓  VR player presses Play (requires min players + all faces drawn)
clients answering      ← clients type answer to prompt (60s timer)
    ↓  all clients answered (or timer expires)
vr picking prompt      ← VR player selects which client's answer to draw
    ↓  VR picks one
vr posing              ← VR poses mannequin (no timer; any controller button locks pose)
    ↓  VR presses any controller button
vr painting            ← VR draws on mannequin (120s timer)
    ↓  timer ends or VR taps "Done"
clients typing guess   ← clients FREE-TEXT type what they think the drawing is
    ↓  all clients submitted typed guess
clients guessing       ← clients pick from typed guesses + correct answer (multiple choice)
    ↓  all clients guessed
vr guessing            ← VR player guesses which client wrote the chosen prompt answer
    ↓
results                ← reveal animations play
    ↓
leaderboard            ← shown for ~6s with animated timer slider
    ↓  if rounds remain, loop back to "clients answering"
gallery                ← all drawings on pedestals, final leaderboard
    ↓
game over
```

**Round count:** `ThreeDPaintGlobalVariables.NUMBER_OF_ROUNDS = 3`

## Lobby Flow (before the game starts)

This is unique to the standalone version — there is no equivalent in the deprecated party scenes.

**VR side (`VRtistryMainMenuManager`):**
- Shows party code and client indicator slots
- Play button is locked until `ClientPlayer.clients.Count >= MINIMUM_NUMBER_OF_PLAYERS` AND every client has submitted a face drawing (`ClientSync.FaceDrawing.Length > 0`)
- On Play: calls `VRtistryGameManager.SetupGame()` then `StartGame()`

**Client side (`VRtistryMainMenuManagerWeb`):**
- On connect to room: camera zooms in, `faceDrawCanvas` slides in
- Client draws their avatar's face using `P3dPaintSphere` on a proxy texture
- On submit: `ClientSync.FaceDrawing` is set to the PNG bytes of the drawn texture → triggers `OnFaceDrawingChangedEvent` on VR side
- Camera zooms back out; client waits in lobby

## Per-Round Sequence (detailed)

### 1. `clients answering`
- VR host selects a prompt from `promptList` (or `clientThemedPromptList` in round 2) and sets `VRtistrySyncer.CurrentPrompt`
- `OnPromptChangedEvent` fires on clients → `VRtistryGameManagerWeb.SetNewPrompt()` resets all UI, clears previous drawings
- Clients type an answer into `answerInputField` and press Submit
- Each submission appends to `VRtistrySyncer.Answers`: `"OWNER_ID:ANSWER"` (newline-separated)
- **Round 2 special:** `ChosenClientToRoast` is set; the prompt is about that specific client, so they see "This prompt is about you!" and cannot submit an answer
- `OnPlayerAnswered` fires on VR side; VR host waits until all eligible clients have answered or timer expires

### 2. `vr picking prompt`
- VR player's UI shows all submitted answers as selectable buttons (hidden from stream with a `DONT_SAY` warning banner)
- VR physically points at and clicks the answer they want to draw
- On selection: `VRtistrySyncer.ChosenAnswerOwner` is set to that client's ID
  - That client immediately earns `POINTS_CLIENT_SELECTED_PROMPT = 125` pts (via `OnChosenAnswerOwnerChanged`)
  - If the chosen client IS the subject of a client-themed round, their answer is skipped when building the button list
- Practice painting tools are active during this state so VR can warm up

### 3. `vr posing`
- VR player physically poses the mannequin using `MannequinSolver` (IK)
- VR UI shows the chosen prompt with a `DONT_SAY_WARNING` reminder not to say the answer aloud
- Clients see blurred view: "Waiting for VR player to set a pose..."
- **Locking pose:** pressing any controller button starts a 3-second countdown, then calls `solver.SetPose()` → transitions to `vr painting`

### 4. `vr painting`
- A new `DrawingModel` is added to `DrawingsSyncer.Drawings` (one per round, added when entering `clients answering`)
- VR draws with `PaintBrush` (line strokes) and spray paint (texture hits)
- Strokes synced in real time via `PenStrokeModel → LinePointModel` chain
- Paint texture synced via `PaintHitLineModel` entries and a final `paintTexture` byte[] snapshot
- Client side: `MannequinSolverClient.SetPoseColliders()` reconstructs the mannequin pose; `blurWipUI` animates in
- 120s countdown displayed (`VRtistrySyncer.DrawingTimer`); VR can tap "Done" early

### 5. `clients typing guess`
- Triggered when painting timer hits 0 or VR taps "Done" — also bakes the final paint texture and pose data into `DrawingsSyncer`
- **Clients see the drawing revealed** — `PaintBrush.AnimatePaintingReveal()` replays all strokes; drawing model auto-rotates
- Clients type a **free-text guess** of what the drawing is into `typedGuessInputField`
  - If you are `ChosenAnswerOwner` (your own prompt was chosen), you see "This is your prompt!" and cannot type a guess
- Each submission appended to `VRtistrySyncer.TypedGuesses`: `"CLIENT_ID:TYPED_GUESS"` (newline-separated)
- VR host transitions to `clients guessing` once all eligible clients have submitted

### 6. `clients guessing`
- **Important:** the buttons shown here are the **typed guesses** (`VRtistrySyncer.TypedGuesses`), NOT the original prompt answers
- The correct answer (the original chosen client answer from `Answers`) is also added as one of the buttons, mixed in
- If you are `ChosenAnswerOwner`, you see "Waiting for other players" — no buttons
- Guess submitted → appended to `VRtistrySyncer.ArtGuesses`: `"CLIENT_ID:CHOSEN_OWNER_ID"` (owner of the typed guess they picked, or the correct answer owner)
  - Selecting the correct original answer → `POINTS_CLIENT_CORRECT_GUESS = 100` pts (first correct also gets `POINTS_CLIENT_FIRST_CORRECT_GUESS = 25`)

### 7. `vr guessing`
- VR sees 3 randomly selected client names (plus correct author) as buttons and must pick who wrote the chosen prompt answer
- Clients simultaneously guess which player wrote the chosen original prompt answer:
  - If you ARE the chosen answer author → you see a different player's answer to identify
  - Otherwise → you guess who wrote the chosen answer
- Client guess submitted → `VRtistrySyncer.PlayerGuesses`: `"CLIENT_ID:GUESSED_OWNER_ID"`
  - Correct → `POINTS_CLIENT_CORRECT_PLAYER = 50` pts + `correctPlayerGuessAnimation` plays
- VR selects the author → stored in `VRtistrySyncer.VRPlayerGuess`
  - Correct → `POINTS_VR_CORRECT_PLAYER = 50` pts

### 8. `results`
- `AnswerOptionButton` objects animate in sequence: answers with no guesses hidden, wrong answers first, correct answer last
- VR points calculated via `ThreeDPaintGlobalVariables.calculatePointsVrCorrectGuesses(correctGuesses)` (scales with number of clients who guessed correctly, snapped to nearest 25, min 25)

### 9. `leaderboard`
- Animated `leaderboardTimerSlider` shows time remaining
- VR player card inserted at correct rank relative to client scores
- After display time, loops back to `clients answering` for next round (or → `gallery` after final round)

### 10. `gallery` (after all rounds)
- `DrawingsSyncer.SetDrawingGalleryPositions()` repositions line-drawing data to gallery pedestal positions
- All round drawings displayed on pedestals with titles; final leaderboard shown

## Synced Data Formats

All multi-value strings in `VRtistrySyncModel` — be careful when parsing/building:

| Field | Format | Example |
|---|---|---|
| `answers` | `"ID:answer\nID:answer"` | `"3:Minion\n7:Batman"` |
| `typedGuesses` | `"ID:guess\nID:guess"` (newline-sep) | `"3:Ballerina\n7:Scarecrow"` |
| `artGuesses` | `"ID:chosenOwnerID"` (newline-sep) | `"3:7\n5:3"` |
| `playerGuesses` | `"ID:guessedID"` (newline-sep) | `"3:7\n5:3"` |
| `decoyAnswers` | `"answer,answer,answer"` (comma-sep, unused in standalone) | `"Athlete,Baller,Dunk King"` |

## Drawing Sync Architecture

```
DrawingsSyncer (RealtimeComponent<DrawingsModel>)
└── Drawings: RealtimeDictionary<DrawingModel>      ← one entry per round
    ├── penStrokes: RealtimeArray<PenStrokeModel>   ← one per brush stroke
    │   └── linePoints: RealtimeArray<LinePointModel>  ← one per point
    ├── practicePenStrokes                          ← strokes drawn during posing phase
    ├── paintHitLines: RealtimeArray<PaintHitLineModel> ← spray paint hits (replayed on clients)
    ├── poseData: RealtimeArray<JointModel>         ← mannequin joint transforms
    └── paintTexture: byte[]                        ← final baked texture snapshot
```

On the client side, `PaintBrush` reads from `DrawingsSyncer.drawingLines` to replay strokes during the reveal animation.

## Point Values

| Event | Points |
|---|---|
| Your answer is chosen as the prompt | 125 |
| Client correctly identifies the drawing | 100 |
| First client to correctly identify (bonus) | +25 |
| Client correctly guesses who wrote an answer | 50 |
| VR player correctly guesses who wrote chosen answer | 50 |
| VR player points (based on how many clients guessed correctly) | min 25, scales to 100 |

Constants live in [ThreeDPaintGlobalVariables.cs](../Assets/Scripts/3DPaint/ThreeDPaintGlobalVariables.cs).

## Platform Differences (`#if` Guards)

- `GeminiFakeAnswersGenerator` is VR/Standalone only — `#if !UNITY_WEBGL` (and currently commented out)
- FMOD audio is VR/Standalone only — `#if !UNITY_WEBGL`
- `DrawingsSyncer` only clears old drawings on start for Android/Standalone — `#if UNITY_ANDROID || UNITY_STANDALONE_WIN`
- WebGL clients use JS interop (`DllImport("__Internal")`) for native keyboard, haptics, and keyboard height detection
- `MINIMUM_NUMBER_OF_PLAYERS`: 2 in Editor, 3 in builds

## Ownership Quirk

`VRtistryGameManager` calls `VRtistrySyncer.instance.realtimeView.ClearOwnership()` every second via `InvokeRepeating`. This is an intentional workaround — without it, VR player ownership can block clients from writing to `Answers` / `ArtGuesses` / `PlayerGuesses`.
