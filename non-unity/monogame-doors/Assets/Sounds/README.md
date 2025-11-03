Place sound files here (WAV or OGG are recommended). The AudioManager will try to load from the Content pipeline first, and if that fails it will attempt to load raw files from Assets/Sounds/<filename> using SoundEffect.FromStream.

Example filenames used by the prototype (you can add more):
- rush_kill.wav
- ambient_room.ogg

Notes:
- Use short sounds for entity SFX (under 3 seconds).
- If you add files, call AudioManager.LoadSound("name", "filename_without_extension") from Game1.LoadContent, or update the code to use exact filenames.
