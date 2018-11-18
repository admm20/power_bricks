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
        
        GameObject buttonStart = new GameObject();
        GameObject buttonExit = new GameObject();

        GameObject logo = new GameObject();

        private const float logo_show_time = 1000.0f;
        private float logo_opacity = 0.0f;
        private int logo_timer = 0;
        private bool logo_fading_away = false;

        private bool show_menu = false;

        private int hover = 0;

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background.texture, background.position, Color.White);
            spriteBatch.Draw(logo.texture, logo.position, Color.White * logo_opacity);

            if (show_menu)
            {
                spriteBatch.Draw(buttonStart.texture, buttonStart.position, Color.White);
                spriteBatch.Draw(buttonExit.texture, buttonExit.position, Color.White);
            }
        }

        public override void LoadTextures(ContentManager content)
        {
            background.texture = content.Load<Texture2D>("Texture/Menu/background_clear");

            logo.texture = content.Load<Texture2D>("Texture/Menu/logo");

            buttonStart.additional_textures.Add(content.Load<Texture2D>("Texture/Menu/start-game"));
            buttonStart.additional_textures.Add(content.Load<Texture2D>("Texture/Menu/start-game-selected"));
            buttonStart.texture = buttonStart.additional_textures[0];

            buttonExit.additional_textures.Add(content.Load<Texture2D>("Texture/Menu/exit"));
            buttonExit.additional_textures.Add(content.Load<Texture2D>("Texture/Menu/exit-selected"));
            buttonExit.texture = buttonExit.additional_textures[0];
        }

        public override void Update(int deltaTime, PowerBricks game)
        {
            UpdateLogoTimer(deltaTime);

            if(show_menu)
            {
                if (buttonStart.position.Contains(game.mouse_x, game.mouse_y))
                {
                    hover = 1;
                    buttonStart.texture = buttonStart.additional_textures[1];
                    buttonExit.texture = buttonExit.additional_textures[0];
                }
                else if (buttonExit.position.Contains(game.mouse_x, game.mouse_y))
                {
                    hover = 2;
                    buttonStart.texture = buttonStart.additional_textures[0];
                    buttonExit.texture = buttonExit.additional_textures[1];
                }
                else
                    hover = 0;
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
                if (hover == 1)
                    game.FadeOutEffect();
                else if (hover == 2)
                    game.Exit();
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
            buttonStart.position = new Rectangle(800 - 234, 400, 468, 45);
            buttonExit.position = new Rectangle(800 - 234, 500, 468, 45);

            logo.position = new Rectangle((game.GAME_WIDTH - 567) / 2,
                (game.GAME_HEIGHT - 283) / 2, 567, 283);

            this.game = game;
        }
    }
}
