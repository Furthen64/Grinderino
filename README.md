# Grinderino

A 2D mining grinder game built with **C#** and **MonoGame** (net9.0).

## Gameplay Loop

1. **Mine** — explore a randomly generated 2D world, digging downward through layers of dirt, stone, and ores. Deeper you go, the rarer the resources.
2. **Sell** — return to base camp, then visit the Market to sell your collected ores and artifacts for money.
3. **Upgrade** — spend money at the Upgrade Shed to sharpen and harden your current pickaxe, or buy a new one from a higher tier (Stone → Iron → Gold → Diamond).
4. **Repeat** — better tools let you mine faster and reach greater depths, unlocking rarer materials.

## Resources

| Block | Depth | Sell Value |
|---|---|---|
| Coal | Shallow | $5 |
| Iron Ore | Mid-shallow | $15 |
| Gold Ore | Mid / Deep | $50 |
| Diamond | Deep | $200 |
| Artifact (Treasure) | Very deep | $500 |

## Screens

- **Main Menu** — start a new run, continue a saved game, or view run history.
- **Base Camp (Lobby)** — hub that links to the Shed, Market, and the Mine.
- **Mining** — side-scrolling 2D level with physics (gravity, jumping, collision). Hold the mine button on a block to break it. Collected valuables go into your bag.
- **Upgrade Shed** — sharpen blades (faster mining), harden steel (higher power to mine harder blocks), or purchase a new pickaxe tier.
- **Market** — sell mined resources individually or all at once. Also buy a Metal Detector ($250) to reveal hidden artifacts underground.
- **Run History (Graphs)** — table and bar chart tracking blocks mined, ores found, artifacts, money earned, and max depth across all runs.

## Controls (Mining)

| Input | Action |
|---|---|
| A / D or ← / → | Move left / right |
| W / ↑ / Space | Jump |
| Z / E / LCtrl | Mine block in front (hold) |
| S / ↓ + Mine | Dig downward |

## Build & Run

```bash
./build.sh    # or dotnet build Grinderino/Grinderino.csproj
./launch.sh   # or dotnet run --project Grinderino/Grinderino
```

Windows: `.\winbuild.ps1` then `.\winlaunch.ps1`
