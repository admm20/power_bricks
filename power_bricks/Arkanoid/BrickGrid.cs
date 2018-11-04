using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace power_bricks.Arkanoid
{
    class BrickGrid
    {
        public int columns = 15;
        public int rows = 15;

        public List<List<Brick>> lines;


        public BrickGrid()
        {
            lines = new List<List<Brick>>();
            for(int row = 0; row < rows; row++)
            {
                List<Brick> line = new List<Brick>();
                for(int col = 0; col < columns; col++)
                {
                    //line.Add(new Brick(new Rectangle(20, 20, )))
                }
                lines.Add(new List<Brick>());
            }
        }
    }
}
