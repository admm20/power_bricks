using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace power_bricks.Arkanoid
{
    enum LastMove
    {
        LEFT,
        RIGHT
    }

    class Paddle : Collision
    {

        public LastMove lastMove = LastMove.RIGHT;

        public Paddle(Rectangle position)
        {
            base.position = position;
            type = Global.GameObjectType.PADDLE;
            ListOfObjectsWithCollision.Add(this);
            side_ratio = (double)position.Width / (double)position.Height;
        }

        public void Move_to_x(int x)
        {
            if (x > 1600 - position.Width)
                position.X = 1600 - position.Width;
            else if (x < 0)
                position.X = 0;
            else
            {
                if (x < position.X)
                    lastMove = LastMove.LEFT;
                else
                    lastMove = LastMove.RIGHT;
                position.X = x;

            }
        }

    }
}
