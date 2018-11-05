using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using power_bricks.Arkanoid;
using power_bricks.Global;
using power_bricks.Menu;
using System;
using System.Runtime.InteropServices;

namespace power_bricks
{

    public class FadeEffectEventArgs : EventArgs
    {
        public bool fadeIn;
        public FadeEffectEventArgs(bool fadeIn)
        {
            this.fadeIn = fadeIn;
        }
    }

    public class MouseClickEventArgs : EventArgs
    {
        public int x, y;
        public MouseClickEventArgs(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class PowerBricks : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        RenderTarget2D renderer;

        Texture2D black_tile;
        
        ProgramState currentState;

        MainMenu menu;
        GameMode gameMode;
        
        public int mouse_x = 0, mouse_y = 0;

        public int GAME_WIDTH = 1600;
        public int GAME_HEIGHT = 1200;

        public event EventHandler<FadeEffectEventArgs> FadeFinished;
        public event EventHandler<MouseClickEventArgs> MouseClicked;

        private float transition_opacity = 0.0f;
        private int transition_timer = 0;
        private bool transition_fade_in = false; // true - fade in // false - fade out
        private bool transition_timer_working = false;

        bool mouseClick = false;

        // used to keep cursor inside window
        [DllImport("user32.dll")]
        static extern void ClipCursor(ref Rectangle rect);

        bool clipCursorActive = true;

        public void HideCursor()
        {
            clipCursorActive = true;
            IsMouseVisible = false;
        }

        public void ShowCursor()
        {
            clipCursorActive = false;
            IsMouseVisible = true;

        }

        public void ShowMainMenu()
        {
            currentState = menu;
            ShowCursor();
            menu.OnEnter();
        }

        public void ShowGameMode()
        {
            currentState = gameMode;
            ShowCursor();
            gameMode.OnEnter();
        }

        // przejscie z ciemnego ekranu w jasny
        public void FadeInEffect()
        {
            transition_opacity = 1.0f;
            transition_timer = 0;
            transition_timer_working = true;
            transition_fade_in = true;
        }

        // przejscie z jasnego ekranu w ciemny
        public void FadeOutEffect()
        {
            transition_opacity = 0.0f;
            transition_timer = 0;
            transition_timer_working = true;
            transition_fade_in = false;
        }

        public void OnFadeFinished(FadeEffectEventArgs e)
        {
            EventHandler<FadeEffectEventArgs> handler = FadeFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnMouseClick(MouseClickEventArgs e)
        {
            EventHandler<MouseClickEventArgs> handler = MouseClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        

        public PowerBricks()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.AllowUserResizing = true;
            renderer = new RenderTarget2D(GraphicsDevice, GAME_WIDTH, GAME_HEIGHT);

            graphics.PreferredBackBufferWidth = 800;  // width of game window
            graphics.PreferredBackBufferHeight = 600;   // height
            graphics.ApplyChanges();
            menu = new MainMenu(this);
            gameMode = new GameMode(this);

            // change update frequency
            base.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 240.0);

            //ShowMainMenu();
            ShowGameMode();
            

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            black_tile = Content.Load<Texture2D>("Texture/Menu/black_tile");
            menu.LoadTextures(Content);
            gameMode.LoadTextures(Content);
        }
        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (IsActive && clipCursorActive)
            {
                Rectangle rect = Window.ClientBounds;
                rect.Width += rect.X;
                rect.Height += rect.Y;

                ClipCursor(ref rect);
            }

            if (transition_timer_working)
            {
                transition_timer += gameTime.ElapsedGameTime.Milliseconds;

                if (transition_fade_in)
                    transition_opacity = 1 - (transition_timer / 1000.0f);
                else
                    transition_opacity = (transition_timer / 1000.0f);

                if (transition_opacity > 1.0f)
                    transition_opacity = 1.0f;
                if (transition_opacity < 0.0f)
                    transition_opacity = 0.0f;

                if (transition_timer > 1000)
                {
                    transition_timer_working = false;
                    OnFadeFinished(new FadeEffectEventArgs(transition_fade_in));
                }
            }

            MouseState mouseState = Mouse.GetState();
            mouse_x = (int)(mouseState.Position.X * ((float)GAME_WIDTH / GraphicsDevice.PresentationParameters.BackBufferWidth));
            mouse_y = (int)(mouseState.Position.Y * ((float)GAME_HEIGHT / GraphicsDevice.PresentationParameters.BackBufferHeight));
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                mouseClick = true;
            }
            else if (mouseClick == true)
            {
                mouseClick = false;
                OnMouseClick(new MouseClickEventArgs(mouse_x, mouse_y));
            }

            currentState.Update(gameTime.ElapsedGameTime.Milliseconds, this);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            GraphicsDevice.SetRenderTarget(renderer);
            GraphicsDevice.Clear(Color.Violet);

            spriteBatch.Begin();
            currentState.Draw(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            Rectangle windowSize = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight);

            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(renderer, windowSize, Color.White);
            spriteBatch.Draw(black_tile, windowSize, Color.White * transition_opacity);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
