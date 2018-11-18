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
        public float x_direction;
        public float y_direction;

        public void Set_velocity(float x, float y)
        {
            x_direction = x;
            y_direction = y;
        }
    }

    class Ball : Collision
    {
        public Velocity speed;

        public bool isInPaddle = false;

        public float X, Y;

        public Ball(Rectangle position)
        {

            base.position = position;
            X = position.X;
            Y = position.Y;
            type = GameObjectType.BALL;

            speed.Set_velocity(0.5f, -0.5f);

        }
    }
}
