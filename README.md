
Monogame Doors - 2.5D Prototype (MonoGame DesktopGL)

Overview
--------
This is a small 2.5D prototype port of the core "Doors" gameplay (procedural room progression, hiding spots, and a couple of entities) to MonoGame so you can run it without Unity.

What is included
- Minimal MonoGame project (DesktopGL) using .NET 7
- Player movement, hiding in closets (press E)
- Room generator with simple rooms and door transitions
- Two entities: Rush and Hide
- Sanity bar and simple UI text
- Save/load (F5 to save, F9 to load) to a JSON file in your home folder

Target platform
---------------
- Designed to run on Linux (works on ChromeOS Linux/Termux/Crostini when dotnet SDK and MonoGame runtime are available).

Requirements
------------
- dotnet SDK (6.0 or 7.0) installed on your ChromeOS Linux container
- On Linux, you'll need OpenGL and the necessary MonoGame native dependencies. In many distros the NuGet package `MonoGame.Framework.DesktopGL` pulls these in.

Quick start (ChromeOS Linux container)
-------------------------------------
1. Install dotnet SDK (follow Microsoft's instructions for Linux / Debian). Example for Debian-based systems:

```bash
# Install dotnet (example for Debian/Ubuntu; adapt for your distro)
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
bash dotnet-install.sh --channel 7.0
export PATH="$HOME/.dotnet:$PATH"
```

2. Change to the project directory and restore packages:

```bash
cd /workspaces/Doors/non-unity/monogame-doors
dotnet restore
```

3. Run the game:

```bash
dotnet run --project MonogameDoors.csproj
```

Controls
--------
- WASD: move
- E: hide / unhide when inside a hiding spot
- Enter: open door when standing on it (move to next room)
- F5: save
- F9: load
- Esc: quit

Notes
-----
- This is a prototype. Graphics are drawn with a single white texture and colored rectangles.
- Audio is not included in this prototype. We can add sound using MonoGame `SoundEffect` / `SoundEffectInstance` later.
 - Audio support has been added via `AudioManager` (place .wav files in the Content pipeline with matching names to load them).
 - Screen shake (VFX) implemented via `VFXManager.AddTrauma()`; entities call this on attacks.
 - Item pickups: press F to pick up nearby items (Key, Lockpick examples are placed in the scene).
- If you want a packaged executable, you can publish a self-contained build for Linux via `dotnet publish -r linux-x64` (requires additional steps and testing on your target device).

Publishing a self-contained Linux build (example)
-----------------------------------------------
To produce a self-contained build that doesn't require an installed dotnet runtime on the target machine, run:

```bash
dotnet publish MonogameDoors.csproj -c Release -r linux-x64 --self-contained true -o ./publish/linux-x64
```

This creates a folder `publish/linux-x64` with a runnable executable (test it inside your Linux container). The resulting binary may require additional native dependencies for SDL/OpenGL depending on your system.
