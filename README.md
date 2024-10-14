
# Dungeon

Dungeon is a C# game developed as part of the *Computer Lab 3* course. Inspired by [Rogue Fable III](https://justin-wang123.itch.io/rogue-fable-iii), this game challenges players to explore procedurally generated dungeons, defeat enemies, gather treasure, and progress through increasingly difficult levels.

## Game Objective
- Collect as many coins and points as possible.
- Advance to higher levels by lighting all torches in each room.
- Survive enemy attacks with a starting life count of 3.

## Gameplay
- **Player Movement**: Use `W`, `A`, `S`, `D` keys to move.
- **Attacking Enemies / Opening Chests**: Left-click on the enemy or chest.
- **Lighting Torches**: Approach the torch stand and left-click to light it.
- **Buying Upgrades**: Purchase upgrades from the main menu using coins collected during gameplay.

## Features
- **Random Dungeon Generation**: Each level is generated using the [Basic BSP Dungeon Generation algorithm](https://www.roguebasin.com/index.php/Basic_BSP_Dungeon_generation), ensuring a unique experience every time.
- **Upgrades and Equipment**: Players can buy health, speed, and damage boosts from the shop menu. Available equipment includes various swords and armor pieces.
- **Permadeath**: Once the player dies, the game resets and all purchased items are lost.
- **Challenging Enemies**: Face off against ghosts, reapers, skeletons, and more in your quest to survive and thrive in the dungeon.

## Starting Settings
- Difficulty: Easy
- Lives: 3
- Damage: 1
- Speed: 1
- Weapon: Wooden Sword

## Shop System
Players can use collected coins to upgrade:
- **Number of Lives**: Buy chest plates or helmets.
- **Player Speed**: Buy boots.
- **Damage Output**: Buy stronger swords (e.g., stone, diamond).

**Note**: The player can only carry the strongest weapon they have purchased.

## Menu Navigation
- The menu is navigated using the mouse. Click to change settings or purchase items.
- Upon death, the game resets, but scores and coins are saved.

## Technical Details
- **Language**: C#
- **Framework**: .NET
- **Dungeon Generation**: Basic BSP algorithm for procedural level creation.
- **Graphics Library**: [SFML](https://www.sfml-dev.org/) (Simple and Fast Multimedia Library) for handling graphics, window management, and input.
- **Collision Detection**: Implemented using AABB (Axis-Aligned Bounding Box) collision detection.
- **Main Classes**:
  - `DungeonGenerator.cs`: Handles random dungeon generation.
  - `Player.cs`, `Ghost.cs`, `Reaper.cs`: Handle player and enemy entities.
  - `Sword.cs`, `Shield.cs`: Define various swords and armor.
  - `Menu.cs`: Manages the game's shop and settings menu.

## How to Run
1. Clone the repository:
   ```
   git clone https://github.com/RobertoGrabovac/Dungeon.git
   ```
2. Open the solution in Visual Studio or your preferred IDE.
3. Build and run the project.

## Contributing
Contributions are welcome! Feel free to fork the repository, create new features, fix bugs, or optimize the code.
