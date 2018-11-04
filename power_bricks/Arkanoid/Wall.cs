using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace power_bricks.Arkanoid
{
    class Wall : Collision
    {
        public Wall(Rectangle position)
        {
            base.position = position;
            type = Global.GameObjectType.WALL;
            ListOfObjectsWithCollision.Add(this);
            side_ratio = (double)position.Width / (double)position.Height;
        }
    }
}
