# Puffy Peaks

A cloud-themed 2D jumping game for Android, built with Unity and C#.

Jump between floating platforms, collect coins and keep climbing. Wind zones and moving platforms add variety as the difficulty increases. Unlock accessories in the Market and complete challenges to earn more coins.

## Gameplay

<img src="docs/images/gameplay.jpg" width="360" alt="Puffy Peaks gameplay: cloud platforms, coins and the player character">

<table>
<tr>
<td><img src="docs/images/gameplay-platforms.jpg" width="300" alt="Cloud and boost platforms"></td>
<td><img src="docs/images/gameplay-combo.jpg" width="300" alt="Landing combo counter"></td>
</tr>
<tr><td>Cloud, boost and moving platforms</td><td>Landing combos</td></tr>
<tr>
<td><img src="docs/images/gameplay-wind.jpg" width="300" alt="Wind zone arrows"></td>
<td><img src="docs/images/gameplay-magnet.jpg" width="300" alt="Magnet power-up and coins"></td>
</tr>
<tr><td>Wind zones</td><td>Magnet power-up</td></tr>
</table>

## Menus

<table>
<tr>
<td><img src="docs/images/main-menu.jpg" width="260" alt="Puffy Peaks main menu"></td>
<td><img src="docs/images/market-purchases.jpg" width="260" alt="Puffy Peaks Market"></td>
<td><img src="docs/images/game-over.png" width="260" alt="Puffy Peaks game-over screen"></td>
</tr>
<tr><td>Main Menu</td><td>Market</td><td>Game Over</td></tr>
</table>

## Features

- Vertical jumping, camera tracking and progressively harder platform layouts.
- Wind zones, moving platforms and precision landing combos.
- Collectible coins, magnet and shield power-ups, and eight cosmetic accessories with a character preview.
- Challenges with progress tracking, coin rewards and new goals after completion.
- Pause, restart, settings and local high-score storage.

## Built with

Unity and C#.

## Code

| System | Script |
| --- | --- |
| Run flow, scoring and difficulty | [GameManager.cs](Source/GameManager.cs) |
| Player movement | [OyuncuKontrol.cs](Source/OyuncuKontrol.cs) |
| Wind | [RuzgarBolgesi.cs](Source/RuzgarBolgesi.cs) |
| Landing combos | [KomboSistemi.cs](Source/KomboSistemi.cs) |
| Market | [MarketManager.cs](Source/MarketManager.cs) |
| Challenges | [ChallengesController.cs](Source/ChallengesController.cs) |
| Menus | [MenuManager.cs](Source/MenuManager.cs) |

Puffy Peaks is currently in closed testing on Android. Screenshots are captured in Unity Editor; feature previews use controlled capture scenarios to show each mechanic clearly.

This repository contains game scripts and screenshots. The complete Unity project and licensed art are maintained separately in a private backup. Service identifiers and signing credentials are not included here.