using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonogameDoors
{
    public class Player
    {
        public Vector2 Position;
        public float Speed = 160f; // pixels per second
        public float Sanity = 100f;
        public bool IsHidden { get; private set; } = false;

        public Player(Vector2 start)
        {
            Position = start;
        }

        public void Update(float dt, RoomGenerator rooms, List<Entity> entities)
        {
            if (IsHidden)
            {
                // Unhide by pressing E
                var ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E))
                {
                    IsHidden = false;
                }
                return;
            }

            var k = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            Vector2 dir = Vector2.Zero;
            if (k.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W)) dir.Y -= 1;
            if (k.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S)) dir.Y += 1;
            if (k.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A)) dir.X -= 1;
            if (k.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D)) dir.X += 1;

            if (dir.LengthSquared() > 0)
            {
                dir.Normalize();
                Position += dir * Speed * dt;
            }

            // Clamp to current room
            var room = rooms.GetRoomRect(rooms.CurrentRoomIndex);
            Position.X = Microsoft.Xna.Framework.MathHelper.Clamp(Position.X, room.X + 10, room.X + room.Width - 10);
            Position.Y = Microsoft.Xna.Framework.MathHelper.Clamp(Position.Y, room.Y + 10, room.Y + room.Height - 10);

            // Interact with hide spots
            if (k.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E))
            {
                foreach (var spot in rooms.GetHidingSpots(rooms.CurrentRoomIndex))
                {
                    var r = spot;
                    if (r.Contains(Position))
                    {
                        IsHidden = true;
                        // Snap into center of spot
                        Position = new Vector2(r.Center.X, r.Center.Y + 10);
                        break;
                    }
                }
            }

            // Open door (enter next room)
            if (k.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                var door = rooms.GetDoorRect(rooms.CurrentRoomIndex);
                if (door.Contains(Position))
                {
                    // move to next room
                    rooms.MoveToNextRoom();
                }
            }

            // simple sanity drain if in entity proximity
            foreach (var e in entities)
            {
                float d = Vector2.Distance(e.Position, Position);
                if (d < 30)
                {
                    Sanity -= 10f * dt;
                }
            }

            Sanity = MathHelper.Clamp(Sanity, 0, 100);
        }
    }
}
