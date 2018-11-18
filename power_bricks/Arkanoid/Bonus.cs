using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace power_bricks.Arkanoid
{
    public enum BonusTypeEnum
    {
        EXPAND,
        SHRINK,
        LIFE,
        FASTER,
        SLOWER,
        SHOOTING,
        DOUBLE_BALL
    }
    class Bonus
    {
        public Rectangle position;
        public BonusTypeEnum type;
        public double Y;

        public Bonus(Rectangle rect, BonusTypeEnum bonus)
        {
            this.Y = rect.Y;
            this.position = rect;
            this.type = bonus;
        }
    }
}
