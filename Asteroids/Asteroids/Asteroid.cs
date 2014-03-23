using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    class Asteroid
    {
        public Asteroid(Microsoft.Xna.Framework.Vector2 pos, Microsoft.Xna.Framework.Vector2 orig, float rot,
            Microsoft.Xna.Framework.Graphics.Texture2D tex)
        {
            Position = pos;
            Origin = orig;
            Rotation = rot;
            Texture = tex;
        }

        public Microsoft.Xna.Framework.Vector2 Position;
        public Microsoft.Xna.Framework.Vector2 Origin;
        public float Rotation;
        public const float SPEED = 1f;
        public Microsoft.Xna.Framework.Graphics.Texture2D Texture;
    }
}
