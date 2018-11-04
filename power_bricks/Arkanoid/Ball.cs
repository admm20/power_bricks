using Microsoft.Xna.Framework;
using power_bricks.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace power_bricks.Arkanoid
{
    struct Velocity
    {
        public double x_direction;
        public double y_direction;

        public void Set_velocity(double x, double y)
        {
            x_direction = x;
            y_direction = y;
        }
    }

    class Ball : Collision
    {
        public Velocity speed;

        public bool isInPaddle = false;

        public Ball(Rectangle position)
        {

            base.position = position;
            type = GameObjectType.BALL;

            speed.Set_velocity(0.7, -0.7);

        }
    }
}
