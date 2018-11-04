using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using power_bricks.Global;

namespace power_bricks.Menu
{
    class MainMenu : ProgramState
    {
        PowerBricks game = null;

        GameObject background = new GameObject(new Rectangle(0, 0, 1600, 1200));

        Rectangle option_position = new Rectangle(570, 370, 500, 310);
        GameObject option_1 = new GameObject();
        GameObject option_2 = new GameObject();
        GameObject option_3 = new GameObject();

        GameObject logo = new GameObject();

        private float logo_opacity = 0.0f;
        private int logo_timer = 0;
        private bool logo_fading_away = false;

        private bool show_menu = false;

        private int hover = 1;

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background.texture, background.position, Color.White);
            spriteBatch.Draw(logo.texture, logo.position, Color.White * logo_opacity);
            if (show_menu)
            {
                switch (hover)
                {
                    case 1:
                        spriteBatch.Draw(option_1.texture, option_1.position, Color.White);
                        break;
                    case 2:
                        spriteBatch.Draw(option_2.texture, option_2.position, Color.White);
                        break;
                    case 3:
                        spriteBatch.Draw(option_3.texture, option_3.position, Color.White);
                        break;
                    default:
                        break;
                }
            }
        }

        public override void LoadTextures(ContentManager content)
        {
            background.texture = content.Load<Texture2D>("Texture/Menu/background_clear");
            option_1.texture = content.Load<Texture2D>("Texture/Menu/option_1");
            option_2.texture = content.Load<Texture2D>("Texture/Menu/option_2");
            option_3.texture = content.Load<Texture2D>("Texture/Menu/option_3");
            logo.texture = content.Load<Texture2D>("Texture/Menu/logo");

        }

        public override void Update(int deltaTime, PowerBricks game)
        {
            UpdateLogoTimer(deltaTime);

            if(show_menu)
            {

            }
        }

        private void FadeFinished(object o, FadeEffectEventArgs e)
        {
            if (!e.fadeIn)
            {
                game.FadeFinished -= FadeFinished;
                game.MouseClicked -= MouseClicked;
                game.ShowGameMode();
            }
        }

        private void MouseClicked(object o, MouseClickEventArgs e)
        {
            if (show_menu)
            {
                game.FadeOutEffect();
            }
        }

        public override void OnEnter()
        {
            
            game.FadeFinished += FadeFinished;
            game.MouseClicked += MouseClicked;

            logo_timer = 0;
            logo_opacity = 0.0f;
            logo_fading_away = false;
            show_menu = false;
        }

        private const float logo_show_time = 10.0f;
        private void UpdateLogoTimer(int deltaTime)
        {
            logo_timer += deltaTime;
            if (!logo_fading_away)
            {
                logo_opacity = (logo_timer / logo_show_time);
                if (logo_timer > logo_show_time)
                {
                    logo_timer = 0;
                    logo_fading_away = true;
                }

            }
            else
            {
                logo_opacity = 1 - (logo_timer / logo_show_time);
                if (logo_timer > logo_show_time + 100)
                {
                    show_menu = true;
                }
            }

            if (logo_opacity > 1.0f)
                logo_opacity = 1.0f;
            if (logo_opacity < 0.0f)
                logo_opacity = 0.0f;
        }

        public MainMenu(PowerBricks game)
        {
            option_1.position = option_position;
            option_2.position = option_position;
            option_3.position = option_position;

            logo.position = new Rectangle((game.GAME_WIDTH - 567) / 2,
                (game.GAME_HEIGHT - 283) / 2, 567, 283);

            this.game = game;
        }
    }
}
