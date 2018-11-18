using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace power_bricks.Arkanoid
{
    public enum BrickTypeEnum{
        NORMAL,
        HP_1,
        HP_2,
        UNBREAKABLE
    }

    class Brick : Collision
    {
        public BrickTypeEnum brickType;

        public Brick(Rectangle position)
        {
            base.position = position;
            type = Global.GameObjectType.BRICK;
            ListOfObjectsWithCollision.Add(this);
            side_ratio = (double)position.Width / (double)position.Height;
        }
    }
}
