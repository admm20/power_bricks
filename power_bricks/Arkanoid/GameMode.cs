using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using power_bricks.Global;

namespace power_bricks.Arkanoid
{

    class GameMode : ProgramState
    {
        public enum GameEventEnum
        {
            BRICK_DESTROYED,
            UNBREAKABLE_BRICK_HIT,
            LIFE_LOST,
            GAME_OVER,
            BALL_HIT_PADDLE,
            BALL_HIT_WALL,
            BONUS_PICKED,
            GAME_WIN,
            LEVEL_COMPLETED,
            SHOOT
        }

        private const int WINDOW_WIDTH = 1600;
        private const int WINDOW_HEIGHT = 1200;

        PowerBricks game;

        Paddle paddle = new Paddle(new Rectangle(500, 1100, 240, 40));

        Wall frame_left = new Wall(new Rectangle(-175, 0, 200, WINDOW_HEIGHT));
        Wall frame_up = new Wall(new Rectangle(-200, -200, WINDOW_WIDTH + 400, 200));
        Wall frame_right = new Wall(new Rectangle(WINDOW_WIDTH - 25, 0, 200, WINDOW_HEIGHT));
        Wall frame_down = new Wall(new Rectangle(-200, WINDOW_HEIGHT + 30, WINDOW_WIDTH + 400, 200));

        List<Ball> balls = new List<Ball>();
        Texture2D ballTexture;

        Texture2D brick_normal_texture;
        Texture2D brick_1_hp_texture;
        Texture2D brick_2_hp_texture;
        Texture2D brick_unbreakable_texture;
        List<Brick> listOfBricks = new List<Brick>();

        List<Laser> listOfLasers = new List<Laser>();

        List<Bonus> listOfBonus = new List<Bonus>();
        Texture2D bonus_expand;
        Texture2D bonus_life;
        Texture2D bonus_speed;
        Texture2D bonus_shoot;
        Texture2D bonus_shrink;
        Texture2D bonus_slow;
        Texture2D bonus_double_ball;


        Player player = new Player();

        GameObject background = new GameObject(new Rectangle(0, 0, 1600, 1200));

        private string[] levels = { "Levels/map1.txt", "Levels/map2.txt", "Levels/map3.txt" };

        private SpriteFont scoreFont;
        private bool gamePaused = false;
        private bool changeLevel = false;
        private bool ballDropped = false;
        private bool showScoreboard = false;

        private int lastDestroyedBrickX = 0, lastDestroyedBrickY = 0;

        private void GameEvent(GameEventEnum ev)
        {
            switch (ev)
            {
                case GameEventEnum.BRICK_DESTROYED:
                    player.points += 10;
                    int brickCounter = 0;
                    foreach (Brick b in listOfBricks)
                    {
                        if (!b.brickType.Equals(BrickTypeEnum.UNBREAKABLE))
                        {
                            brickCounter++;
                        }
                    }
                    if (brickCounter == 0)
                    {
                        GameEvent(GameEventEnum.LEVEL_COMPLETED);
                    }
                    GenerateBonus();
                    break;
                case GameEventEnum.UNBREAKABLE_BRICK_HIT:
                    break;
                case GameEventEnum.LIFE_LOST:
                    listOfBonus.Clear();
                    player.holdingBall = false;
                    player.lifes--;
                    player.size = 2;
                    paddle.position.Width = 240;
                    player.canShoot = false;
                    if (player.lifes < 1)
                    {
                        GameEvent(GameEventEnum.GAME_OVER);
                    }
                    else
                    {
                        ballDropped = true;
                        game.FadeOutEffect();
                    }
                    break;
                case GameEventEnum.GAME_OVER:
                    Console.WriteLine("PRZEGRALES");
                    showScoreboard = true;
                    break;
                case GameEventEnum.BALL_HIT_PADDLE:
                    break;
                case GameEventEnum.BALL_HIT_WALL:
                    break;
                case GameEventEnum.BONUS_PICKED:
                    break;
                case GameEventEnum.GAME_WIN:
                    Console.WriteLine("WYGRALES");
                    showScoreboard = true;
                    break;
                case GameEventEnum.LEVEL_COMPLETED:
                    Console.WriteLine("wygrales poziom");
                    player.level++;
                    if (player.level > 2)
                    {
                        GameEvent(GameEventEnum.GAME_WIN);
                        return;
                    }
                    changeLevel = true;
                    game.FadeOutEffect();
                    break;
                case GameEventEnum.SHOOT:
                    break;
            }
        }

        private void GenerateBonus()
        {
            double chance = 20; // 0-100 %

            Random rnum = new Random();
            int random = rnum.Next(0, 100);

            if(random < chance)
            {
                random = rnum.Next(0, 6);
                Rectangle temp = new Rectangle(lastDestroyedBrickX, lastDestroyedBrickY, 60, 60);
                switch (random)
                {
                    case 0:
                        listOfBonus.Add(new Bonus(temp, BonusTypeEnum.DOUBLE_BALL));
                        break;
                    case 1:
                        listOfBonus.Add(new Bonus(temp, BonusTypeEnum.EXPAND));
                        break;
                    case 2:
                        listOfBonus.Add(new Bonus(temp, BonusTypeEnum.FASTER));
                        break;
                    case 3:
                        listOfBonus.Add(new Bonus(temp, BonusTypeEnum.LIFE));
                        break;
                    case 4:
                        listOfBonus.Add(new Bonus(temp, BonusTypeEnum.SHOOTING));
                        break;
                    case 5:
                        listOfBonus.Add(new Bonus(temp, BonusTypeEnum.SHRINK));
                        break;
                    case 6:
                        listOfBonus.Add(new Bonus(temp, BonusTypeEnum.SLOWER));
                        break;
                }
            }

        }

        private void UpdateBonusPosition(int delta)
        {
            List<Bonus> tempList = new List<Bonus>(listOfBonus);
            foreach(Bonus b in tempList)
            {
                b.Y += 0.4 * delta;

                if (b.position.Intersects(paddle.position))
                {
                    GameEvent(GameEventEnum.BONUS_PICKED);
                    switch (b.type)
                    {
                        case BonusTypeEnum.EXPAND:
                            if (player.size < 3)
                            {
                                player.size++;
                                paddle.position.Width += 100;
                            }
                            break;
                        case BonusTypeEnum.SHRINK:
                            if (player.size > 1)
                            {
                                player.size--;
                                paddle.position.Width -= 100;
                            }
                            break;
                        case BonusTypeEnum.LIFE:
                            player.lifes++;
                            break;
                        case BonusTypeEnum.FASTER:
                            foreach(Ball ball in balls)
                            {
                                ball.speed.x_direction *= 1.1f;
                                ball.speed.y_direction *= 1.1f;
                            }
                            break;
                        case BonusTypeEnum.SLOWER:
                            foreach (Ball ball in balls)
                            {
                                ball.speed.x_direction *= 0.9f;
                                ball.speed.y_direction *= 0.9f;
                            }
                            break;
                        case BonusTypeEnum.SHOOTING:
                            player.canShoot = true;
                            break;
                        case BonusTypeEnum.DOUBLE_BALL:
                            balls.Add(new Ball(new Rectangle(paddle.position.X + 100, paddle.position.Y - 30, 30, 30)));
                            break;
                    }

                    listOfBonus.Remove(b);
                }
            }
        }

        private void UpdateLasers(int delta)
        {
            double move = 0.5 * delta;
            if (move < 1)
                move = 1;
            
            for(int i = listOfLasers.Count - 1; i >= 0; i--)
            {
                listOfLasers[i].position.Y -= (int)move;
                List<GameObject> tempList = new List<GameObject>(Collision.ListOfObjectsWithCollision);
                foreach (Collision gameObject in tempList)
                {

                    if (listOfLasers[i].position.Intersects(gameObject.position) &&
                        gameObject.type.Equals(GameObjectType.WALL))
                    {
                        listOfLasers.RemoveAt(i);
                        break;
                    }

                        if (listOfLasers[i].position.Intersects(gameObject.position) && 
                        gameObject.type.Equals(GameObjectType.BRICK))
                    {
                        Brick temp = (Brick)gameObject;
                        if (!temp.brickType.Equals(BrickTypeEnum.UNBREAKABLE))
                        {
                            if (temp.brickType.Equals(BrickTypeEnum.HP_2))
                            {
                                temp.brickType = BrickTypeEnum.HP_1;
                            }
                            else
                            {
                                lastDestroyedBrickX = temp.position.X;
                                lastDestroyedBrickY = temp.position.Y;
                                Collision.ListOfObjectsWithCollision.Remove(gameObject);
                                listOfBricks.Remove((Brick)gameObject);
                                GameEvent(GameEventEnum.BRICK_DESTROYED);
                            }

                        }
                        else
                            GameEvent(GameEventEnum.UNBREAKABLE_BRICK_HIT);
                        
                        listOfLasers.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void LoadMap(String filePath)
        {

            listOfBricks.Clear();
            List<GameObject> tempList = new List<GameObject>(Collision.ListOfObjectsWithCollision);
            foreach (Collision gameObject in tempList)
            {
                if (gameObject.type.Equals(GameObjectType.BRICK))
                {
                    Collision.ListOfObjectsWithCollision.Remove(gameObject);
                }
            }

                Console.WriteLine("LOADING LEVEL");
            string[] lines = System.IO.File.ReadAllLines(filePath);
            for (int row = 0; row < 19; row++)
            {
                for (int col = 0; col < 19; col++)
                {
                    if (lines[row][col].Equals('1'))
                    {
                        Brick brick = new Brick(new Rectangle(30 + col * 81, 60 + row * 40, 81, 40));
                        brick.texture = brick_normal_texture;
                        brick.brickType = BrickTypeEnum.NORMAL;
                        listOfBricks.Add(brick);
                    }
                    else if (lines[row][col].Equals('2'))
                    {
                        Brick brick = new Brick(new Rectangle(30 + col * 81, 60 + row * 40, 81, 40));
                        brick.texture = brick_1_hp_texture;
                        brick.brickType = BrickTypeEnum.HP_1;
                        listOfBricks.Add(brick);
                    }
                    else if (lines[row][col].Equals('3'))
                    {
                        Brick brick = new Brick(new Rectangle(30 + col * 81, 60 + row * 40, 81, 40));
                        brick.texture = brick_2_hp_texture;
                        brick.brickType = BrickTypeEnum.HP_2;
                        listOfBricks.Add(brick);
                    }
                    else if (lines[row][col].Equals('4'))
                    {
                        Brick brick = new Brick(new Rectangle(30 + col * 81, 60 + row * 40, 81, 40));
                        brick.texture = brick_unbreakable_texture;
                        brick.brickType = BrickTypeEnum.UNBREAKABLE;
                        listOfBricks.Add(brick);
                    }

                }
            }

            balls.Clear();
            listOfBonus.Clear();
            listOfLasers.Clear();
            balls.Add(new Ball(new Rectangle(1000, 1000, 30, 30)));
            player.canShoot = false;
            player.holdingBall = true;
            player.holdingBallBonusPicked = false;
            player.size = 2;
            paddle.position.Width = 240;
        }


        // obliczanie nowej pozycji pilek
        private void UpdateBallPosition(int delta)
        {
            List<Ball> tempBalls = new List<Ball>(balls);
            foreach(Ball ball in tempBalls)
            {
                Vector2 distance = new Vector2(ball.speed.x_direction * delta, ball.speed.y_direction * delta);

                // drogę do pokonania dzielę na 10 części, żeby zminimalizować ryzyko tunelowania
                // im większa wartość, tym większa dokładnośc
                int divisions = 10;

                for (int d = 0; d < divisions; d++)
                {
                    float old_x = ball.X;
                    float old_y = ball.Y;

                    ball.X += (ball.speed.x_direction * delta) / (float)divisions;
                    ball.Y += (ball.speed.y_direction * delta) / (float)divisions;
                    ball.position.X = (int)(ball.X);
                    ball.position.Y = (int)(ball.Y);

                    foreach (Collision gameObject in Collision.ListOfObjectsWithCollision)
                    {

                        if (ball.Collided(gameObject) && !ball.isInPaddle)
                        {
                            // powroc do pozycji sprzed uderzenia
                            ball.X = old_x;
                            ball.Y = old_y;

                            // obliczanie odleglosci pomiedzy pilka a obiektem, w ktory uderzyla
                            double dist_x = Math.Abs(ball.X + 15 - gameObject.position.Center.X); // ball.X + 15 to jest srodek pilki
                            double dist_y = Math.Abs(ball.Y + 15 - gameObject.position.Center.Y);

                            double ratio = dist_x / dist_y;

                            if (ratio < gameObject.side_ratio && !ball.isInPaddle)
                            {
                                ball.speed.y_direction *= -1;

                                if (gameObject.type.Equals(GameObjectType.PADDLE))
                                {
                                    if (ball.position.Center.X - 15 < gameObject.position.Center.X) // ball is on left side
                                    {
                                        if (ball.speed.x_direction > 0)
                                            ball.speed.x_direction *= -1;
                                    }
                                    else if (ball.position.Center.X > gameObject.position.Center.X + 10)
                                    {
                                        if (ball.speed.x_direction < 0)
                                            ball.speed.x_direction *= -1;
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
                                Brick temp = (Brick)gameObject;
                                if (!temp.brickType.Equals(BrickTypeEnum.UNBREAKABLE))
                                {
                                    if (temp.brickType.Equals(BrickTypeEnum.HP_2))
                                    {
                                        temp.brickType = BrickTypeEnum.HP_1;
                                    }
                                    else
                                    {
                                        lastDestroyedBrickX = temp.position.X;
                                        lastDestroyedBrickY = temp.position.Y;
                                        Collision.ListOfObjectsWithCollision.Remove(gameObject);
                                        listOfBricks.Remove((Brick)gameObject);
                                        GameEvent(GameEventEnum.BRICK_DESTROYED);
                                    }

                                }
                                else
                                    GameEvent(GameEventEnum.UNBREAKABLE_BRICK_HIT);

                                
                            }

                            if (gameObject.type.Equals(GameObjectType.LOSE_AREA))
                            {
                                balls.Remove(ball);
                                if(balls.Count == 0)
                                {
                                    GameEvent(GameEventEnum.LIFE_LOST);
                                }
                            }

                            break;
                        }
                        else if (gameObject.type.Equals(GameObjectType.PADDLE) && !ball.Collided(gameObject))
                        {
                            ball.isInPaddle = false;
                        }
                    }
                }
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (gamePaused)
            {
                spriteBatch.DrawString(scoreFont, "PAUSED", new Vector2(1600 / 2 - 80, 1200 / 2 - 100), Color.White);
                return;
            }

            spriteBatch.Draw(background.texture, background.position, Color.White);
            spriteBatch.Draw(paddle.texture, paddle.position, Color.White);

            foreach(Ball b in balls)
            {
                spriteBatch.Draw(ballTexture, b.position, Color.White);
            }
            foreach(Brick b in listOfBricks)
            {
                if(b.brickType.Equals(BrickTypeEnum.NORMAL))
                    spriteBatch.Draw(brick_normal_texture, b.position, Color.White);
                else if (b.brickType.Equals(BrickTypeEnum.HP_1))
                    spriteBatch.Draw(brick_1_hp_texture, b.position, Color.White);
                else if (b.brickType.Equals(BrickTypeEnum.HP_2))
                    spriteBatch.Draw(brick_2_hp_texture, b.position, Color.White);
                else if (b.brickType.Equals(BrickTypeEnum.UNBREAKABLE))
                    spriteBatch.Draw(brick_unbreakable_texture, b.position, Color.White);
            }

            // narysuj wynik
            spriteBatch.DrawString(scoreFont, player.points.ToString(), new Vector2(40, 20), Color.White);

            // narysuj życia
            for(int i = 0; i < player.lifes; i++)
            {
                spriteBatch.Draw(paddle.texture, new Rectangle(1510 - (i * 55), 28, 50, 10), Color.White); //240 40
            }

            // narysuj lasery
            foreach(Laser l in listOfLasers)
            {
                spriteBatch.Draw(brick_normal_texture, l.position, Color.White);
            }

            // narysuj bonusy
            foreach(Bonus b in listOfBonus)
            {
                Texture2D temp = null;
                switch (b.type)
                {
                    case BonusTypeEnum.EXPAND:
                        temp = bonus_expand;
                        break;
                    case BonusTypeEnum.SHRINK:
                        temp = bonus_shrink;
                        break;
                    case BonusTypeEnum.LIFE:
                        temp = bonus_life;
                        break;
                    case BonusTypeEnum.FASTER:
                        temp = bonus_speed;
                        break;
                    case BonusTypeEnum.SLOWER:
                        temp = bonus_slow;
                        break;
                    case BonusTypeEnum.SHOOTING:
                        temp = bonus_shoot;
                        break;
                    case BonusTypeEnum.DOUBLE_BALL:
                        temp = bonus_double_ball;
                        break;
                }
                b.position.Y = (int)b.Y;
                spriteBatch.Draw(temp, b.position, Color.White);
            }

            if (showScoreboard)
            {
                // W TYM MIEJSCU RYSUJESZ SCOREBOARDA
            }
        }

        public override void LoadTextures(ContentManager content)
        {
            background.texture = content.Load<Texture2D>("Texture/Game/background");
            paddle.texture = content.Load<Texture2D>("Texture/Game/podest5");
            ballTexture = content.Load<Texture2D>("Texture/Game/pilka2");

            brick_normal_texture = content.Load<Texture2D>("Texture/Game/k3");
            brick_1_hp_texture = content.Load<Texture2D>("Texture/Game/1hp");
            brick_2_hp_texture = content.Load<Texture2D>("Texture/Game/2hp");
            brick_unbreakable_texture = content.Load<Texture2D>("Texture/Game/unbreakable");

            bonus_expand = content.Load<Texture2D>("Texture/Game/expand_paddle");
            bonus_life = content.Load<Texture2D>("Texture/Game/extra_life");
            bonus_speed = content.Load<Texture2D>("Texture/Game/fast_ball");
            bonus_shoot = content.Load<Texture2D>("Texture/Game/shooting_paddle");
            bonus_shrink = content.Load<Texture2D>("Texture/Game/shrink_paddle");
            bonus_slow = content.Load<Texture2D>("Texture/Game/slow_ball");
            bonus_double_ball = content.Load<Texture2D>("Texture/Game/split_ball");

            scoreFont = content.Load<SpriteFont>("Fonts/ScoreFont");
        }

        public override void OnEnter()
        {
            game.FadeInEffect();
            game.HideCursor();
            game.FadeFinished += FadeFinished;
            game.MouseClicked += MouseClicked;
            game.KeyboardClicked += KeyboardClicked;

            gamePaused = false;
            changeLevel = false;
            ballDropped = false;
            showScoreboard = false;

            player = new Player();
            
            LoadMap(levels[0]);
        }

        public override void Update(int deltaTime, PowerBricks game)
        {
            if (!gamePaused && !showScoreboard)
            {
                paddle.Move_to_x(game.mouse_x);

                if (player.holdingBall)
                {
                    balls[0].X = paddle.position.X + 110;
                    balls[0].Y = 1100 - 30;
                    balls[0].speed.Set_velocity(0.5f, -0.5f);
                }

                UpdateBallPosition(deltaTime);
                UpdateBonusPosition(deltaTime);
                UpdateLasers(deltaTime);
            }

            if (showScoreboard)
            {
               
            }
        }

        private void FadeFinished(object o, FadeEffectEventArgs e)
        {
            Console.WriteLine("WCHODZE");
            if (ballDropped)
            {
                ballDropped = false;
                balls.Add(new Ball(new Rectangle(1000, 1000, 30, 30)));
                player.holdingBall = true;
                game.FadeInEffect();
            }

            if (changeLevel)
            {
                game.FadeInEffect();
                changeLevel = false;
                LoadMap(levels[player.level]);
            }
        }

        private void MouseClicked(object o, MouseClickEventArgs e)
        {
            player.holdingBall = false;
            if (player.canShoot)
            {
                GameEvent(GameEventEnum.SHOOT);
                listOfLasers.Add(new Laser(new Rectangle(paddle.position.Center.X, paddle.position.Y - 20, 4, 15)));
            }
        }
        
        private void KeyboardClicked(object o, KeyboardClickEventArgs e)
        {
            if (e.ks.IsKeyDown(Keys.P) & !game.PreviousKbState.IsKeyDown(Keys.P))
            {
                // klik p
                gamePaused = !gamePaused;
            }
        }

        public GameMode(PowerBricks game)
        {
            this.game = game;
            

            frame_down.type = GameObjectType.LOSE_AREA;
        }
    }
}
