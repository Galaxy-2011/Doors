using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System;

namespace MonogameDoors
{
    public static class AudioManager
    {
        private static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
    private static ContentManager? _content;

        public static void Initialize(ContentManager content)
        {
            _content = content;
        }

        public static void LoadSound(string name, string path)
        {
            // Try Content pipeline first
            if (_content != null)
            {
                try
                {
                    var s = _content.Load<SoundEffect>(path);
                    sounds[name] = s;
                    return;
                }
                catch { }
            }

            // Fallback: try loading raw WAV/OGG from Assets/Sounds/<path>
            string filePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Assets", "Sounds", path);
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    using (var fs = System.IO.File.OpenRead(filePath))
                    {
                        var s = SoundEffect.FromStream(fs);
                        sounds[name] = s;
                        return;
                    }
                }
                catch { }
            }
        }

        public static void GenerateBeep(string name, float freqHz, float durationSeconds, float volume = 0.5f, int sampleRate = 44100)
        {
            try
            {
                var wav = CreateWavPcm16(freqHz, sampleRate, durationSeconds, volume);
                using (var ms = new MemoryStream(wav))
                {
                    var s = SoundEffect.FromStream(ms);
                    sounds[name] = s;
                }
            }
            catch { }
        }

        private static byte[] CreateWavPcm16(float freqHz, int sampleRate, float durationSeconds, float volume)
        {
            int samples = (int)(sampleRate * durationSeconds);
            int channels = 1;
            short[] data = new short[samples * channels];
            for (int i = 0; i < samples; i++)
            {
                double t = (double)i / sampleRate;
                double v = Math.Sin(2.0 * Math.PI * freqHz * t);
                data[i] = (short)(v * short.MaxValue * Math.Clamp(volume, 0.0f, 1.0f));
            }

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                // RIFF header
                bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                bw.Write((int)(36 + data.Length * 2)); // file size - 8
                bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

                // fmt subchunk
                bw.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                bw.Write((int)16); // subchunk1 size
                bw.Write((short)1); // PCM
                bw.Write((short)channels);
                bw.Write((int)sampleRate);
                bw.Write((int)(sampleRate * channels * 2)); // byte rate
                bw.Write((short)(channels * 2)); // block align
                bw.Write((short)16); // bits per sample

                // data subchunk
                bw.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                bw.Write((int)(data.Length * 2));
                foreach (var s in data)
                {
                    bw.Write(s);
                }

                bw.Flush();
                return ms.ToArray();
            }
        }

        public static void Play(string name, float volume = 1f)
        {
            if (sounds.TryGetValue(name, out var s))
            {
                try { s.Play(volume, 0f, 0f); } catch { }
            }
        }
    }
}
