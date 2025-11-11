# Chess Engine: Bitboards & Performance Optimizations

This document explains the **Bitboard** approach for chessboard representation, a fundamental technique for building a fast chess engine, which is essential for a performant AI.

## 1. Why Not Use an 8x8 Array?

The most intuitive approach is to use a `Piece[8,8]` array. This is simple to understand but extremely slow for an AI. Every question, such as "Is this square attacked?", requires `for` loops to scan ranks, files, or diagonals.

Bitboards replace these loops with bitwise logical operations, which are thousands of times faster at the CPU level.

## 2. What is a Bitboard?

A bitboard is a simple 64-bit unsigned integer (`ulong` in C#). Each bit in this integer corresponds to one of the 64 squares on the chessboard.

-   If a bit is `1`, a piece is present on that square.
-   If a bit is `0`, the square is empty.

The mapping is typically from square A1 (bit 0) to square H8 (bit 63).

```
Bit 63  ...  Bit 56 (H8..A8)
...            ...
Bit 7   ...  Bit 0  (H1..A1)
```

An engine doesn't use a single bitboard but a **set** of them to describe the complete game state:

-   **12 Piece Bitboards:** `WhitePawns`, `WhiteRooks`, ..., `BlackKings`.
-   **3 Utility Bitboards:** `WhitePieces`, `BlackPieces`, and `AllPieces` (obtained via `OR` operations).

### The Power of Bitwise Operations

The speed comes from using bitwise operations to ask complex questions in a single CPU instruction.

-   **All pieces on the board:** `AllPieces = WhitePieces | BlackPieces;`
-   **Check if A1 is occupied:** `if ((AllPieces & 1UL) != 0) { ... }` (where `1UL` is a mask for the A1 square).
-   **Move a pawn from E2 to E4:** `WhitePawns ^= (maskE2 | maskE4);` (`XOR` is used to toggle bits on and off).

| Operator | Name         | Use in Chess                                       |
| :------- | :----------- | :------------------------------------------------- |
| `&`      | AND          | Isolate pieces, check for square occupancy.        |
| `|`      | OR           | Combine piece sets, merge attack maps.             |
| `^`      | XOR          | Flip bits (making/unmaking a move).                |
| `~`      | NOT          | Invert all bits (e.g., to get all empty squares).  |
| `<<`, `>>` | Bit Shift    | Move pieces on the board (e.g., a pawn push).      |

---

## 3. Move Generation & Key Optimizations

### A. Knight and King Move Generation

The moves for these pieces are "fixed." A knight on E4 can always attack the same 8 squares. We can therefore **pre-calculate** these attack masks into an array.

`AttackMasks[Square.E4]` would return a bitboard with 8 bits set to `1` for all possible destinations. Move generation becomes a simple lookup.

### B. Pawn Move Generation

Pawn moves are handled with `Bit Shifts`.
-   **Single white pawn push:** `singlePushes = (WhitePawns << 8) & EmptySquares;`
-   **Diagonal captures:** `captures = ((WhitePawns << 7) | (WhitePawns << 9)) & BlackPieces;`

Additional masks are needed to prevent pieces from "wrapping" from one side of the board to the other (e.g., a pawn on the H-file cannot capture on the A-file).

### C. Magic Bitboards (For Sliding Pieces)

This is the most important optimization. For a Rook or Bishop, the possible moves depend on which pieces are blocking the path.

The goal of "Magic Bitboards" is to transform this complex problem into a **simple array lookup**, just like for the knight. It is a pre-computation technique.

**The Concept:**
1.  For a Rook on E4, we first identify all squares that could possibly block it on its rank and file (a "relevant blocker mask").
2.  We use a perfect hashing function (the "magic") that takes the bitboard of *actual* blocking pieces and transforms it into a unique index.
3.  This index is used to look up the correct move bitboard in a massive pre-calculated table.

**Result:** Generating all moves for a Rook or Bishop, regardless of the position's complexity, is done in just a few instructions with no loops. It's a complex technique to implement but offers spectacular performance gains.

---

## 4. Other Essential Optimizations for AI

A fast move generator is the foundation, but the AI needs other tools to be intelligent.

### A. Zobrist Hashing

This is a technique for creating a **quasi-unique 64-bit identifier (hash)** for any given board position.

**How it works:**
-   We pre-calculate a random 64-bit number for every possible piece on every square (e.g., `RandomTable[PieceType.Pawn][Color.White][Square.E4]`).
-   The hash for a position is simply the `XOR` of all the numbers corresponding to the pieces on the board.

**Its Power:** When a piece moves, you don't need to re-calculate everything. You update the hash with just a few `XOR` operations, which is nearly instantaneous.
`newHash = oldHash ^ pieceAtFromSquare ^ pieceAtToSquare;`

### B. Transposition Tables

A transposition table is a huge hash table (a `Dictionary` in C#) that stores the results of the AI's analysis.

-   **Key:** The Zobrist Hash of the position.
-   **Value:** The calculated information (the evaluation score, the search depth, the best move found, etc.).

When the AI explores the game tree, before analyzing a position, it calculates its Zobrist Hash and looks in the table. If the position has already been analyzed to a sufficient depth, it uses the stored result directly, **pruning entire branches of the search tree**. This is the single most impactful optimization for AI strength.

### C. Piece-Square Tables (PST)

This is an optimization for the **evaluation function**. The idea is that a piece's value depends on its position. A knight in the center is stronger than a knight in a corner.

A PST is simply an array that assigns a bonus or penalty for each piece on each square. The total evaluation of a position is no longer just the sum of material, but the sum of material **plus** the sum of the positional bonuses/penalties for every piece. This gives the AI a basic positional understanding.