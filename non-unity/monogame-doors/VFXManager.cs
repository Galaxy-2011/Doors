using Microsoft.Xna.Framework;

namespace MonogameDoors
{
    public static class VFXManager
    {
        private static float trauma = 0f;
        private static float decay = 1.5f;
        public static Vector2 CameraOffset { get; private set; } = Vector2.Zero;

        public static void AddTrauma(float amount)
        {
            trauma = MathHelper.Clamp(trauma + amount, 0f, 1f);
        }

        public static void Update(float dt)
        {
            if (trauma <= 0f)
            {
                CameraOffset = Vector2.Zero;
                return;
            }

            trauma = MathHelper.Max(0f, trauma - decay * dt);
            float shake = trauma * trauma;
            float maxOffset = 8f;
            CameraOffset = new Vector2((float)(System.Math.Sin(System.Environment.TickCount * 0.01f) * maxOffset * shake), (float)(System.Math.Cos(System.Environment.TickCount * 0.008f) * maxOffset * shake));
        }
    }
}
