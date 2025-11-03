using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace MonogameDoors
{
    public static class TextureManager
    {
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
    private static GraphicsDevice? _graphics;

        public static void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;
        }

        public static Texture2D? Load(string name, string filename)
        {
            if (_graphics == null) return null;
            if (textures.ContainsKey(name)) return textures[name];

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Sprites", filename);
            if (File.Exists(filePath))
            {
                try
                {
                    using (var fs = File.OpenRead(filePath))
                    {
                        var tex = Texture2D.FromStream(_graphics, fs);
                        textures[name] = tex;
                        return tex;
                    }
                }
                catch { }
            }

            // If file missing or failed to load, generate a small placeholder texture
            try
            {
                int size = 32;
                var tex = new Texture2D(_graphics, size, size);
                // deterministic color based on name
                int h = System.Math.Abs(name.GetHashCode());
                byte r = (byte)((h >> 16) & 0xFF);
                byte g = (byte)((h >> 8) & 0xFF);
                byte b = (byte)(h & 0xFF);
                Color[] data = new Color[size * size];
                for (int i = 0; i < data.Length; i++) data[i] = new Color(r, g, b);
                tex.SetData(data);
                textures[name] = tex;
                return tex;
            }
            catch
            {
                return null;
            }
        }

        public static Texture2D? Get(string name)
        {
            if (textures.TryGetValue(name, out var t)) return t;
            return null;
        }
    }
}
