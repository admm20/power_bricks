using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using power_bricks.Global;

namespace power_bricks.Arkanoid
{
    class GameMode : ProgramState
    {
        private const int WINDOW_WIDTH = 1600;
        private const int WINDOW_HEIGHT = 1200;

        PowerBricks game;

        Paddle paddle = new Paddle(new Rectangle(500, 1100, 240, 40));

        Wall frame_left = new Wall(new Rectangle(-175, 0, 200, WINDOW_HEIGHT));
        Wall frame_up = new Wall(new Rectangle(-200, -200, WINDOW_WIDTH + 400, 200));
        Wall frame_right = new Wall(new Rectangle(WINDOW_WIDTH - 25, 0, 200, WINDOW_HEIGHT));
        Wall frame_down = new Wall(new Rectangle(-200, WINDOW_HEIGHT, WINDOW_WIDTH + 400, 200));

        Ball ball = new Ball(new Rectangle(1000, 1000, 30, 30));

        Texture2D brick_texture;
        List<Brick> listOfBricks = new List<Brick>();

        GameObject background = new GameObject(new Rectangle(0, 0, 1600, 1200));

        private void LoadMap(String filePath)
        {
            Console.WriteLine("LOADING LEVEL");
            string[] lines = System.IO.File.ReadAllLines(filePath);
            for (int row = 0; row < 19; row++)
            {
                for (int col = 0; col < 19; col++)
                {
                    if (lines[row][col].Equals('1'))
                    {
                        Brick brick = new Brick(new Rectangle(30 + col * 81, 60 + row * 40, 81, 40));
                        brick.texture = brick_texture; // ????
                        listOfBricks.Add(brick);
                    }

                }
            }
        }

        private void UpdateBallPosition(int delta)
        {
            int old_x = ball.position.X;
            int old_y = ball.position.Y;

            ball.position.X += (int)(ball.speed.x_direction * delta);
            ball.position.Y += (int)(ball.speed.y_direction * delta);


            foreach (Collision gameObject in Collision.ListOfObjectsWithCollision)
            {

                if (ball.Collided(gameObject) && !ball.isInPaddle)
                {

                    ball.position.X = old_x;
                    ball.position.Y = old_y;

                    // calculate distance between object and ball
                    double dist_x = Math.Abs(ball.position.Center.X - gameObject.position.Center.X);
                    double dist_y = Math.Abs(ball.position.Center.Y - gameObject.position.Center.Y);

                    double ratio = dist_x / dist_y;

                    if (ratio < gameObject.side_ratio && !ball.isInPaddle)
                    {
                        ball.speed.y_direction *= -1;

                        // odbijanie pilki od paletki ma byc podobne do tego, jak jest w arkanoidach:
                        if (gameObject.type.Equals(GameObjectType.PADDLE))
                        {
                            double x1 = 0.3, y1 = -0.3;
                            if (dist_x > 40)
                            {
                                x1 = 0.384513829233677;
                                y1 = -0.179301743237636;

                            }

                            if (ball.position.Center.X < gameObject.position.Center.X - 10) // ball is on left side
                            {
                                ball.speed.x_direction = -x1 * 2;
                                ball.speed.y_direction = y1 * 2;
                            }
                            else if (ball.position.Center.X > gameObject.position.Center.X + 10)
                            {
                                ball.speed.x_direction = x1 * 2;
                                ball.speed.y_direction = y1 * 2;
                            }
                            else // around middle of paddle
                            {
                                ball.speed.y_direction = y1 * 2;
                                if (ball.speed.x_direction < 0)
                                    ball.speed.x_direction = -0.3 * 2;
                                else
                                    ball.speed.x_direction = 0.3 * 2;


                            }

                        }
                        
                    }
                    else
                    {
                        // specyficzny przypadek, gdzie paletka uderza piłkę bokiem 
                        if (gameObject.type.Equals(GameObjectType.PADDLE))
                        {
                            Paddle paddle = (Paddle)gameObject;
                            if (ball.speed.x_direction > 0)
                            {
                                if (paddle.lastMove.Equals(LastMove.LEFT))
                                    ball.speed.x_direction *= -1;
                            }
                            else
                                if (paddle.lastMove.Equals(LastMove.RIGHT))
                                ball.speed.x_direction *= -1;
                        }
                        else
                            ball.speed.x_direction *= -1;
                    }

                    if (gameObject.type.Equals(GameObjectType.PADDLE))
                    {
                        ball.isInPaddle = true;
                    }

                    if (gameObject.type.Equals(GameObjectType.BRICK))
                    {
                        Collision.ListOfObjectsWithCollision.Remove(gameObject);
                        listOfBricks.Remove((Brick)gameObject);
                    }

                    //if (gameObject.type.Equals(GameObjectType.LOSE_AREA))
                    //{
                    //    // PRZEGRALES
                    //    position.X = 600;
                    //    position.Y = 400;
                    //    speed.x_direction = 0.3;
                    //    speed.y_direction = -0.3;

                    //    isFail = true;
                    //}
                    

                    break;
                }
                else if (gameObject.type.Equals(GameObjectType.PADDLE) && !ball.Collided(gameObject))
                {
                    ball.isInPaddle = false;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background.texture, background.position, Color.White);
            spriteBatch.Draw(paddle.texture, paddle.position, Color.White);
            spriteBatch.Draw(ball.texture, ball.position, Color.White);
            foreach(Brick b in listOfBricks)
            {
                spriteBatch.Draw(brick_texture, b.position, Color.White);
            }
        }

        public override void LoadTextures(ContentManager content)
        {
            background.texture = content.Load<Texture2D>("Texture/Game/background");
            paddle.texture = content.Load<Texture2D>("Texture/Game/podest5");
            ball.texture = content.Load<Texture2D>("Texture/Game/pilka2");
            brick_texture = content.Load<Texture2D>("Texture/Game/k3");
        }

        public override void OnEnter()
        {
            game.FadeInEffect();
            game.FadeFinished += FadeFinished;
            game.MouseClicked += MouseClicked;

            // load map
            //for(int col = 0; col < 19; col++)
            //{
            //    for(int row = 0; row < 19; row++)
            //    {
            //        Brick brick = new Brick(new Rectangle(30 + col * 81, 60 + row * 40, 81, 40));
            //        brick.texture = brick_texture; // ????
            //        listOfBricks.Add(brick);

            //    }
            //}
            LoadMap("map1.txt");
        }

        public override void Update(int deltaTime, PowerBricks game)
        {
            paddle.Move_to_x(game.mouse_x);

            UpdateBallPosition(deltaTime);
        }

        private void FadeFinished(object o, FadeEffectEventArgs e)
        {

        }

        private void MouseClicked(object o, MouseClickEventArgs e)
        {
            
        }

        public GameMode(PowerBricks game)
        {
            this.game = game;
        }
    }
}
