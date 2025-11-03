using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonogameDoors
{
    public class RoomGenerator
    {
        private List<Rectangle> _rooms = new List<Rectangle>();
        private List<List<Rectangle>> _hidingSpots = new List<List<Rectangle>>();
    public int CurrentRoomIndex { get; set; } = 0;

        public void InitializeRooms()
        {
            _rooms.Clear();
            _hidingSpots.Clear();

            // Create 6 simple rooms in a row
            int w = 800; int h = 500;
            for (int i = 0; i < 6; i++)
            {
                var rect = new Rectangle(50 + i * (w + 50), 100, w, h);
                _rooms.Add(rect);

                var spots = new List<Rectangle>();
                // place a closet spot in each room
                spots.Add(new Rectangle(rect.X + 80, rect.Y + rect.Height - 80, 60, 50));
                spots.Add(new Rectangle(rect.X + rect.Width - 140, rect.Y + rect.Height - 80, 60, 50));
                _hidingSpots.Add(spots);
            }
        }

        public Rectangle GetRoomRect(int index)
        {
            if (index < 0 || index >= _rooms.Count) return _rooms[0];
            return _rooms[index];
        }

        public Rectangle GetDoorRect(int index)
        {
            var r = GetRoomRect(index);
            // door on the right edge
            return new Rectangle(r.X + r.Width - 24, r.Y + r.Height / 2 - 40, 20, 80);
        }

        public List<Rectangle> GetHidingSpots(int index)
        {
            if (index < 0 || index >= _hidingSpots.Count) return new List<Rectangle>();
            return _hidingSpots[index];
        }

        public void MoveToNextRoom()
        {
            if (CurrentRoomIndex < _rooms.Count - 1)
                CurrentRoomIndex++;
        }

        // Convenience: an exposed property used by Player clamp logic
        public int CurrentRoom => CurrentRoomIndex;
    }
}
