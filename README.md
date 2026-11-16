# VR Chess

[Version française](README.fr.md)

A chess game in virtual reality. You grab the pieces with the controllers, set them down on the board, and play against a friend who has their own headset. The club builds it for its new members. The game matters, but the real goal is to learn by building it.

## Why this project

Everyone knows the rules of chess, so we can focus on the code. Along the way you run into object-oriented programming (inheritance for the pieces, for example: a base class, then the pawn, the rook, the knight...), the single responsibility principle, how C# works inside Unity, testing, and the basics of networking.

We don't try to optimize everything. A faster engine that is harder to explain doesn't help us here. Between clever code and code you can understand by reading it, we pick the second.

## What the game does

All the rules are there: castling, en passant, promotion, checkmate, stalemate. In VR, you grab a piece, the squares it can reach light up, and you drop it where you want. If the move isn't valid, the piece goes back where it was.

To play together, one player creates a room and the other joins it. At first, everything runs on the same local network. Each player has their own headset, so nobody has to hand theirs over.

## How it's built

The code is split into layers that don't mix. The basic idea fits in one sentence: VR proposes a move, the game controller decides, the view shows the result.

The Engine holds the rules of chess, in plain C#. It knows nothing about Unity, so you can test it without a headset. The Game layer holds the game controller, which checks a move, applies it and tells everyone. It doesn't know Unity either. On the Unity side, the View handles what you see (board, pieces, highlights, messages), VRInput turns VR gestures into a move to play, and Multiplayer takes care of the room and sends the moves around.

On the network, only the moves travel. Each player keeps their own pieces on their own machine, and the host stays the referee: it validates every move before the other player sees it.

(It will be reviewed, not the final structure but the idea of it)
```
Assets/
└── _Project/
    └── Scripts/
        ├── Engine/        # chess rules, plain C#
        ├── Game/          # game controller, players, results, plain C#
        ├── View/          # board, pieces, highlights, messages
        ├── VRInput/       # VR gestures turned into moves
        └── Multiplayer/   # room and network code
```

Each folder has its own namespace (`Chess.Engine`, `Chess.Game`, and so on). The rule to remember: nothing in `Engine` or `Game` may reference `UnityEngine`.

## Tech

Unity (2022 LTS or newer) with the URP pipeline. OpenXR and the XR Interaction Toolkit for VR, Netcode for GameObjects for multiplayer, C# everywhere, and the Unity Test Framework for tests.

## Getting started

You need Unity Hub, Git, and the Unity version listed in `ProjectSettings/ProjectVersion.txt`. A headset that supports OpenXR is handy but not required: an XR simulator lets you test without one. To try multiplayer, two devices on the same local network are enough, or two Unity instances on the same machine.

```bash
git clone <REPOSITORY-URL>
```

Open the project with Unity Hub and let Unity import the packages. Then open the `GameTable` scene and press Play. Setup notes for your own headset still need to be added here.

## Ideas for later

None of this is planned from the start. We get to it if there's time and the rest holds up. Since the logic is separate from the display, most of these ideas only touch the View.

Life-size chess, with giant pieces: you walk around a board at human scale. Scale, movement and the way you grab a piece all have to be rethought.

XR Hands: playing with your hands instead of the controllers, pinching your fingers to pick up a piece. It depends on the headset.

Mixed reality: you place a board on a real table and the headset simulates the pieces on top of it. Again, everything depends on what the headset can do.

Seeing your opponent play live: their hand grabs a piece and it comes off its square. This goes through a networked avatar (head and hands), not through the pieces themselves. If they let go on an invalid move, the piece goes back to its place.

An AI opponent, to play alone. Later still, online play outside the local network, with a matchmaking service such as Unity Relay and Lobby. If the room sits behind a clean interface, only the way you create and find it changes.

And the smaller things: clocks, captured pieces, move history, a few different environments, rematch, resign, controller sounds and vibrations.

## Contributing

Anyone can contribute, whatever their level. A few habits to keep: respect the layers, one class for one responsibility, comments that explain why rather than what, and a test for every chess rule you add to the engine. Work on a branch, and have another member review your pull request before it's merged.
