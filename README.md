# VR Chess Project Roadmap

This document outlines a complete, step-by-step roadmap for developing a feature-rich VR chess game in Unity. The goal is to build a high-quality experience with a powerful, from-scratch chess engine, intuitive VR interactions, a challenging AI opponent, and networked multiplayer capabilities using Unity's native tools.

## 1. Core Architectural Principles

-   **Logical Engine/View Separation:** While all scripts will live inside the Unity project, we will maintain a strict logical separation.
    1.  **Engine Layer:** Pure C# classes and logic for rules, move generation, and AI. These scripts will be organized in a dedicated folder and namespace (`Chess.Engine`) and will **not** reference `UnityEngine`. This makes the logic portable and easily testable.
    2.  **View Layer:** `MonoBehaviour` scripts that handle VR interaction, visual representation, and user interface. They will live in a separate folder (`Chess.View`) and will communicate with the Engine layer.
-   **VR-First Design:** Interactions will be designed from the ground up for VR controllers, focusing on intuitive grabbing, moving, and placing of pieces.
-   **Performance-Oriented Engine:** The Engine layer will use **Bitboards** for board representation and move generation, ensuring the performance required for a strong AI.

## 2. Technology Stack

-   **Game Engine:** Unity 2022.x or later
-   **VR SDK:** Unity's **OpenXR Plugin** (for broad compatibility with Oculus, SteamVR, etc.)
-   **Networking:** Unity's **Netcode for GameObjects** (the modern, officially supported solution)
-   **Language:** C#
-   **Testing:** Unity Test Framework

---

## 3. The Development Roadmap

### Milestone 0: Foundation & Project Setup (1-2 Days)

*Goal: Create a clean, well-structured Unity project ready for development.*

1.  **Unity Project Creation & Configuration:**
    -   [ ] Create a new 3D URP (Universal Render Pipeline) Unity Project. URP is optimized for VR performance.
    -   [ ] Switch the build platform to **PC, Mac & Linux Standalone**.
    -   [ ] From the Package Manager, install the following packages:
        -   **OpenXR Plugin**
        -   **Netcode for GameObjects**
2.  **Project Folder & Namespace Structure:**
    -   [ ] In the `Assets` folder, create a `_Project` folder to hold all your work.
    -   [ ] Inside `_Project`, create a `Scripts` folder.
    -   [ ] Inside `Scripts`, create the core logical folders:
        -   `Engine`: For all pure C# chess logic (rules, bitboards, AI).
        -   `View`: For all `MonoBehaviour` scripts that interact with the game world (VR input, piece movement).
        -   `Multiplayer`: For scripts specifically handling network logic.
3.  **Initial VR Scene:**
    -   [ ] Create a new scene named `GameTable`.
    -   [ ] Set up a basic VR rig (**XR Origin**) with camera and hand controllers.
    -   [ ] Configure OpenXR for your target devices (e.g., Oculus Touch, Index Controllers).
    -   [ ] Model or import a simple environment (a room, a table) and a chessboard model.
    -   [ ] Implement basic **Teleportation** movement to allow the player to move around the table.

### Milestone 1: The Core Chess Engine (5-7 Days)

*Goal: A fully functional, non-visual chess engine that understands all the rules of chess. **All work in this milestone is done within the `Scripts/Engine` folder and should use the `Chess.Engine` namespace.**.*

1.  **Data Structures:**
    -   [ ] Create `Enums` for `PieceType` (Pawn, Rook, etc.) and `PlayerColor` (White, Black).
    -   [ ] Define a `Move` struct to represent a move (containing `from` square, `to` square, and special flags like promotion type).
2.  **Bitboard Representation:**
    -   [ ] Create a `BoardState` class to hold an array of `ulong` (64-bit integers) representing the 12 piece bitboards (e.g., `WhitePawns`, `BlackRooks`).
    -   [ ] Implement a function to initialize the bitboards to the standard starting chess position using a FEN string.
3.  **Move Generation:**
    -   [ ] Create a `MoveGenerator` class.
    -   [ ] Implement pseudo-legal move generation for each piece type using bitwise operations, starting with King/Knight and moving to sliding pieces (**"Magic Bitboards"** are the goal for high performance).
    -   [ ] Add generation for special moves: castling, en-passant capture, and pawn promotions.
4.  **Game Logic & State Management:**
    -   [ ] Create a `GameState` class that holds a `BoardState` and metadata (current player, castling rights, en-passant square, etc.).
    -   [ ] Implement a `MakeMove(Move move)` function that takes a `GameState` and a `Move`, and returns a *new* `GameState` with the move applied. This immutability is key for AI.
    -   [ ] Implement `GenerateLegalMoves()`. This function will call the pseudo-legal generator, then filter out any moves that would leave the king in check.
5.  **Unit Testing:**
    -   [ ] Use the Unity Test Framework to create "Edit Mode" tests.
    -   [ ] Write tests to validate your move generator against known positions (**Perft testing**). This is critical for ensuring your engine is bug-free.

### Milestone 2: VR Visualization & Basic Interaction (3-4 Days)

*Goal: See the chessboard in VR and be able to pick up and drop pieces.*

1.  **The Engine-Unity Bridge (`GameManager.cs`):**
    -   [ ] Create a `GameManager.cs` script in `Scripts/View` and attach it to a `GameObject`.
    -   [ ] This script will hold the current game state: `private GameState currentGameState;`.
    -   [ ] Write a function `SyncViewFromState()` that iterates through the engine's bitboards and instantiates/moves piece prefabs to their correct positions.
2.  **Piece Prefabs & Scripting:**
    -   [ ] Create a prefab for each chess piece (e.g., `White_Pawn_Prefab`).
    -   [ ] Add an `XR Grab Interactable` component to each prefab to make them grabbable.
    -   [ ] Add a simple script, `ChessPieceView.cs`, that holds its piece type and color.
3.  **Board Interaction:**
    -   [ ] Add colliders to each square on the chessboard.
    -   [ ] Write a script that can identify which square a piece is hovered over or dropped on.
    -   [ ] When a piece is picked up, store its starting square. When it is dropped, determine its destination square.

### Milestone 3: Implementing the Game Loop (2-3 Days)

*Goal: A fully playable "hot seat" game for two players in VR.*

1.  **Turn Management:**
    -   [ ] The `GameManager` will now enforce turn order based on `currentGameState.CurrentPlayer`.
    -   [ ] Only allow pieces of the current player's color to be picked up.
2.  **Move Validation & Execution:**
    -   [ ] When a player drops a piece, construct a `Move` object.
    -   [ ] Ask the engine if this move is legal by checking against the list from `GenerateLegalMoves()`.
    -   [ ] If the move is legal:
        1.  Call the engine: `currentGameState = currentGameState.MakeMove(playerMove);`.
        2.  Call `SyncViewFromState()` to update the visual board.
    -   [ ] If the move is illegal, snap the piece back to its original square using a smooth animation.
3.  **Game End Conditions & Feedback:**
    -   [ ] After each move, check the engine for checkmate or stalemate conditions.
    -   [ ] Display a clear UI message in VR to announce the game's outcome.
    -   [ ] Add a visual highlight to legal destination squares when a piece is picked up.

### Milestone 4: The AI Opponent (4-5 Days)

*Goal: Create a challenging single-player experience.*

1.  **Evaluation Function (in `Scripts/Engine`):**
    -   [ ] Create an `Evaluator` class with a static `Evaluate(GameState state)` function.
    -   [ ] Start with a material count evaluation, then add **Piece-Square Tables** for positional awareness.
2.  **Search Algorithm (in `Scripts/Engine`):**
    -   [ ] Implement the **NegaMax with Alpha-Beta Pruning** search algorithm.
    -   [ ] Add **Transposition Tables** (using a dictionary) to cache evaluations and avoid re-calculating the same positions.
3.  **Asynchronous Integration (in `Scripts/View`):**
    -   [ ] In `GameManager`, when it's the AI's turn, call its `FindBestMove` function.
    -   [ ] **Crucially**, run this search on a background thread using `async/await Task.Run()`. This prevents the VR application from freezing.
    -   [ ] While the AI is "thinking," display a visual indicator to the player.
    -   [ ] When the background task completes, execute the returned move on the main thread and update the view.

### Milestone 5: Networked Multiplayer (5-7 Days)

*Goal: Allow two players to compete remotely using Unity Netcode.*

1.  **Network Setup:**
    -   [ ] Create a new `Menu` scene for matchmaking.
    -   [ ] Add the `NetworkManager` component to a `GameObject` in your `GameTable` scene. Configure its Network Prefabs list with all your chess piece prefabs.
    -   [ ] Create a simple UI in the `Menu` scene with "Host" and "Join" buttons.
2.  **Authoritative Game State:**
    -   [ ] The game will use a **Host-Client** model. The player who starts the game (the Host) will have the authoritative version of the `GameState`.
    -   [ ] The `GameManager` script will now need logic to differentiate between Host and Client.
3.  **Move Synchronization:**
    -   [ ] When a player makes a move, the process will be:
        1.  The client's `GameManager` validates the input locally (is it their turn? is the piece theirs?).
        2.  The client sends the intended `Move` data to the Host using a `[ServerRpc]`.
        3.  The Host receives the `ServerRpc`, runs the move through its **authoritative** engine instance for full validation (`GenerateLegalMoves`).
        4.  If the move is valid, the Host updates its `GameState` and then broadcasts the successful move to *all* clients (including itself) using a `[ClientRpc]`.
        5.  All clients receive the `ClientRpc`, apply the now-confirmed move to their local engine instance, and call `SyncViewFromState()` to update their board visually.
4.  **Networked Object Spawning:**
    -   [ ] The Host will be responsible for spawning the initial set of chess pieces as networked objects at the start of the game.

### Milestone 6: Polishing & Final Features (4-6 Days)

*Goal: Transform the functional prototype into a polished, complete game.*

1.  **UI/UX:**
    -   [ ] Create beautiful 3D menus that are easy to interact with in VR.
    -   [ ] Add a "Captured Pieces" area on the side of the board.
    -   [ ] Implement a move history display.
2.  **VR Comfort & Environment:**
    -   [ ] Offer different environments (a library, a park, a sci-fi room).
    -   [ ] Allow the player to resize the board or change their viewing angle.
    -   [ ] Add simple VR hand avatars to represent the player's hands. For multiplayer, synchronize the remote player's hand and head positions.
3.  **Advanced Game Features:**
    -   [ ] Implement game timers (Blitz, Rapid), synchronized over the network.
    -   [ ] Add an "undo move" feature for single-player games.
    -   [ ] Implement a "New Game" and "Resign" flow, both for single-player and multiplayer.
4.  **Sound Design & Haptics:**
    -   [ ] Add ambient background music and refined sound effects.
    -   [ ] Implement controller haptics for grabbing, dropping, and captures to make interactions feel physical.
5.  **Optimization:**
    -   [ ] Profile the application using the Unity Profiler to ensure it maintains a high and stable frame rate (90+ FPS is the goal for VR).