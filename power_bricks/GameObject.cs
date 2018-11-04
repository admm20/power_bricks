using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace power_bricks.Global
{
    public enum GameObjectType
    {
        BALL,
        BRICK,
        WALL,
        PADDLE,
        LOSE_AREA
    }

    class GameObject
    {
        public Rectangle position;

        // texture, that will be drawn on screen
        public Texture2D texture;
        
        public List<Texture2D> additional_textures;

        public string id;

        public GameObjectType type;

        public GameObject()
        {

        }

        public GameObject(Rectangle position)
        {
            this.position = position;
        }
    }
}
