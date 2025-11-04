using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace MonogameDoors
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;

    // Primitive texture
    private Texture2D _whiteTex = null!;

        // Game systems
        public Player Player { get; private set; } = null!;
        public RoomGenerator RoomGen { get; private set; } = null!;
        public List<Entity> Entities { get; private set; } = new List<Entity>();
    public System.Collections.Generic.List<Item> Items { get; private set; } = new System.Collections.Generic.List<Item>();
    public Inventory Inventory { get; private set; } = null!;

        // UI
        private SpriteFont? _font;

        // Game state
    // room index is tracked by RoomGenerator

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = ""; // We'll load textures directly from files
            IsMouseVisible = true;
            Window.Title = "Doors - MonoGame Prototype (2.5D)";
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _whiteTex = new Texture2D(GraphicsDevice, 1, 1);
            _whiteTex.SetData(new[] { Color.White });

            // Load a default SpriteFont from system fallback if available.
            // The user can place a SpriteFont file in the folder and update the Load if needed.
            try
            {
                // Try to load a default font stream if present
                _font = Content.Load<SpriteFont>("default");
            }
            catch
            {
                _font = null; // font optional
            }

            // Initialize game systems
            RoomGen = new RoomGenerator();
            RoomGen.InitializeRooms();

            Player = new Player(new Vector2(200, 200));

            // Initialize audio manager
            AudioManager.Initialize(Content);
            // Optionally load sounds placed under Content (if you add .xnb/.wav to the project)
            AudioManager.LoadSound("rush_kill", "rush_kill");

            // Generate small runtime SFX so the prototype has audible feedback without external files
            AudioManager.GenerateBeep("rush_beep", 700f, 0.12f, 0.6f);
            AudioManager.GenerateBeep("screech_beep", 1400f, 0.18f, 0.7f);
            AudioManager.GenerateBeep("ambush_beep", 420f, 0.10f, 0.55f);
            AudioManager.GenerateBeep("figure_scream", 900f, 0.35f, 0.9f);
            AudioManager.GenerateBeep("eyes_stare", 600f, 0.09f, 0.6f);
            AudioManager.GenerateBeep("eyes_glare", 1200f, 0.14f, 0.85f);
            AudioManager.GenerateBeep("timothy_hum", 220f, 0.24f, 0.45f);
            AudioManager.GenerateBeep("timothy_whisper", 440f, 0.18f, 0.5f);
            AudioManager.GenerateBeep("glitch_pop", 900f, 0.08f, 0.6f);
            AudioManager.GenerateBeep("glitch_bite", 700f, 0.12f, 0.75f);
            AudioManager.GenerateBeep("glitch_buzz", 200f, 0.35f, 0.65f);
            AudioManager.GenerateBeep("jack_knock", 500f, 0.12f, 0.9f);
            AudioManager.GenerateBeep("jack_bite", 800f, 0.2f, 1f);

            // Initialize texture manager and attempt to load sprites from Assets/Sprites
            TextureManager.Initialize(GraphicsDevice);
            TextureManager.Load("player", "player.png");
            TextureManager.Load("rush", "rush.png");
            TextureManager.Load("hide", "hide.png");
            TextureManager.Load("screech", "screech.png");
            TextureManager.Load("ambush", "ambush.png");
            TextureManager.Load("key", "key.png");
            TextureManager.Load("lockpick", "lockpick.png");


            // Spawn an example Rush and Hide in room 0
            Entities.Add(new RushEntity(new Vector2(400, 220)));
            Entities.Add(new HideEntity(new Vector2(300, 300)));
            Entities.Add(new ScreechEntity(new Vector2(500, 260)));
            Entities.Add(new AmbushEntity(new Vector2(600, 260)));
            Entities.Add(new FigureEntity(new Vector2(700, 260)));
            Entities.Add(new EyesEntity(new Vector2(720, 220)));
            Entities.Add(new TimothyEntity(new Vector2(360, 260)));
            Entities.Add(new GlitchEntity(new Vector2(520, 220)));
            Entities.Add(new JackEntity(new Vector2(450, 220)));

            // Example items
            Items = new System.Collections.Generic.List<Item>();
            Items.Add(new Item("Key", new Vector2(360, 240)));
            Items.Add(new Item("Lockpick", new Vector2(420, 240)));

            Inventory = new Inventory();

            // If save exists, try to load
            var save = SaveSystem.Load();
            if (save != null)
            {
                Player.Position = save.PlayerPosition;
                RoomGen.CurrentRoomIndex = save.CurrentRoom;
                Player.Sanity = save.PlayerSanity;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var k = Keyboard.GetState();
            if (k.IsKeyDown(Keys.Escape)) Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Save/load shortcuts
            if (k.IsKeyDown(Keys.F5))
            {
                SaveSystem.Save(new SaveData { CurrentRoom = RoomGen.CurrentRoomIndex, PlayerPosition = Player.Position, PlayerSanity = Player.Sanity });
            }
            if (k.IsKeyDown(Keys.F9))
            {
                var s = SaveSystem.Load();
                if (s != null)
                {
                    Player.Position = s.PlayerPosition;
                    RoomGen.CurrentRoomIndex = s.CurrentRoom;
                    Player.Sanity = s.PlayerSanity;
                }
            }

            // Update player (handles input)
            Player.Update(dt, RoomGen, Entities);

            // Pickup handling: press F to pick up nearby items
            var ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F))
            {
                for (int i = Items.Count - 1; i >= 0; i--)
                {
                    var it = Items[i];
                    if (Vector2.Distance(it.Position, Player.Position) < 24)
                    {
                        Inventory.Add(it);
                        Items.RemoveAt(i);
                    }
                }
            }

            // Update entities
            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                Entities[i].Update(dt, Player);
                if (Entities[i].IsDead)
                    Entities.RemoveAt(i);
            }

            // Update VFX manager (screen shake)
            VFXManager.Update(dt);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            // apply camera offset from VFXManager
            var camOffset = VFXManager.CameraOffset;

            // Draw current room (simple rectangle)
            var roomRect = RoomGen.GetRoomRect(RoomGen.CurrentRoomIndex);
            _spriteBatch.Draw(_whiteTex, new Rectangle((int)(roomRect.X + camOffset.X), (int)(roomRect.Y + camOffset.Y), (int)roomRect.Width, (int)roomRect.Height), Color.DarkSlateGray);

            // Draw door at right edge
            Rectangle door = RoomGen.GetDoorRect(RoomGen.CurrentRoomIndex);
            _spriteBatch.Draw(_whiteTex, door, Color.SaddleBrown);

            // Draw hiding spots
            foreach (var spot in RoomGen.GetHidingSpots(RoomGen.CurrentRoomIndex))
            {
                _spriteBatch.Draw(_whiteTex, spot, Color.DimGray);
            }

            // Draw entities (use textures when available)
            foreach (var e in Entities)
            {
                Texture2D? tex = null;
                if (e is RushEntity) tex = TextureManager.Get("rush");
                else if (e is HideEntity) tex = TextureManager.Get("hide");
                else if (e is ScreechEntity) tex = TextureManager.Get("screech");
                else if (e is AmbushEntity) tex = TextureManager.Get("ambush");

                if (tex != null)
                {
                    var dest = new Rectangle((int)(e.Position.X + camOffset.X) - tex.Width/2, (int)(e.Position.Y + camOffset.Y) - tex.Height/2, tex.Width, tex.Height);
                    _spriteBatch.Draw(tex, dest, Color.White);
                }
                else
                {
                    Color col = Color.Red;
                    if (e is HideEntity) col = Color.Purple;
                    _spriteBatch.Draw(_whiteTex, new Rectangle((int)(e.Position.X + camOffset.X) - 10, (int)(e.Position.Y + camOffset.Y) - 10, 20, 20), col);
                }
            }

            // Draw items (use textures if available)
            foreach (var it in Items)
            {
                Texture2D? itTex = null;
                if (it.Name.ToLower().Contains("key")) itTex = TextureManager.Get("key");
                if (it.Name.ToLower().Contains("lockpick")) itTex = TextureManager.Get("lockpick");

                if (itTex != null)
                {
                    var dest = new Rectangle((int)(it.Position.X + camOffset.X) - itTex.Width/2, (int)(it.Position.Y + camOffset.Y) - itTex.Height/2, itTex.Width, itTex.Height);
                    _spriteBatch.Draw(itTex, dest, Color.White);
                }
                else
                {
                    _spriteBatch.Draw(_whiteTex, new Rectangle((int)(it.Position.X + camOffset.X) - 6, (int)(it.Position.Y + camOffset.Y) - 6, 12, 12), Color.Gold);
                }

                if (_font != null)
                    _spriteBatch.DrawString(_font, it.Name, new Vector2(it.Position.X + 10 + camOffset.X, it.Position.Y + camOffset.Y), Color.White);
            }

            // Draw player (semi-3D: circle + shadow)
            // Draw player (use texture if available)
            var playerTex = TextureManager.Get("player");
            if (playerTex != null)
            {
                // shadow
                _spriteBatch.Draw(_whiteTex, new Rectangle((int)(Player.Position.X + camOffset.X) - 12, (int)(Player.Position.Y + camOffset.Y) - 8, 24, 8), Color.Black * 0.4f);
                var dest = new Rectangle((int)(Player.Position.X + camOffset.X) - playerTex.Width/2, (int)(Player.Position.Y + camOffset.Y) - playerTex.Height/2, playerTex.Width, playerTex.Height);
                _spriteBatch.Draw(playerTex, dest, Color.White);
            }
            else
            {
                _spriteBatch.Draw(_whiteTex, new Rectangle((int)(Player.Position.X + camOffset.X) - 12, (int)(Player.Position.Y + camOffset.Y) - 8, 24, 8), Color.Black * 0.4f); // shadow
                _spriteBatch.Draw(_whiteTex, new Rectangle((int)(Player.Position.X + camOffset.X) - 10, (int)(Player.Position.Y + camOffset.Y) - 20, 20, 20), Color.CornflowerBlue);
            }

            // UI: sanity bar
            int barW = 200; int barH = 18;
            var barRect = new Rectangle(20, 20, barW, barH);
            _spriteBatch.Draw(_whiteTex, barRect, Color.Gray * 0.6f);
            var fillRect = new Rectangle(20, 20, (int)(barW * (Player.Sanity / 100f)), barH);
            _spriteBatch.Draw(_whiteTex, fillRect, Color.LimeGreen);
            if (_font != null)
                _spriteBatch.DrawString(_font, $"Sanity: {(int)Player.Sanity}", new Vector2(20, 44), Color.White);

            if (_font != null)
                _spriteBatch.DrawString(_font, $"Room: {RoomGen.CurrentRoomIndex}", new Vector2(20, 70), Color.White);

            if (_font != null)
            {
                _spriteBatch.DrawString(_font, "Controls: WASD move, E hide/unhide, F5 save, F9 load, Enter to open door", new Vector2(20, 100), Color.LightGray);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
