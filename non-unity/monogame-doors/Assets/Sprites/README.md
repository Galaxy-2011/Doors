This folder is where sprite assets for the MonoGame prototype go.

Recommended filenames (used by the prototype):
- player.png
- rush.png
- hide.png
- screech.png
- ambush.png
- key.png
- lockpick.png

You can place simple PNG files here. The TextureManager will try to load them at runtime from Assets/Sprites/<filename> using Texture2D.FromStream.

If a texture isn't present, the game falls back to simple colored rectangles.

Tips:
- Use small images (like 32x32 or 64x64) for quick testing.
- If you don't have image editing tools, you can create small PNGs with any image editor or use online placeholder generators.
