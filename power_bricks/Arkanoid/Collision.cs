using Microsoft.Xna.Framework;
using power_bricks.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace power_bricks.Arkanoid
{
    class Collision : GameObject
    {
        public static List<GameObject> ListOfObjectsWithCollision = new List<GameObject>();

        public double side_ratio = 1.0;

        public bool Collided(Collision collider)
        {
            if (position.Intersects(collider.position))
                return true;
            return false;
        }

        public bool Collided(GameObject gameObject)
        {
            if (position.Intersects(gameObject.position))
                return true;
            return false;
        }

        public bool Collided(Rectangle rectangle)
        {
            if (this.position.Intersects(rectangle))
                return true;
            return false;
        }
    }
}
