using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    class Projectile
    {
        public Projectile(Microsoft.Xna.Framework.Vector2 pos, Microsoft.Xna.Framework.Vector2 ori, float rot)
        {
            Position = pos;
            Origin = ori;
            Rotation = rot;
        }
        public Microsoft.Xna.Framework.Vector2 Position;
        public Microsoft.Xna.Framework.Vector2 Origin;
        public float Rotation;
        public const float SPEED = 5f;
    }
}
