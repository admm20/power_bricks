using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace power_bricks.Arkanoid
{
    class Player
    {
        public int lifes = 3;
        public int points = 0;

        public bool canShoot = false;
        public bool holdingBall = true;
        public bool holdingBallBonusPicked = false;

        public int size = 2; // 1 - najmniejszy, 2 - normalny, 3 - duzy

        public int level = 0;
        
    }
}
