using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;

namespace DynaBlaster
{
    public class Game1 : Game
    {
        public static TouchCollection touchCollection;
        public static long gameMiliseconds = 0;
        public static Point origin, dynOrigin;
        public static Level[] levels;
        public static int currentLevelNr;
        public static Texture2D spriteAtlas;
        public static Character character;
        public static int score = 0, hiScore;
        public static Sounds sounds;
        public static Hud hud;
        public Boolean levelLoaded, gameOver;
        int drawType;
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D _nativeRenderTarget;
        Rectangle screenRect;
        Controls controls;
       

        //public static Rectangle debug;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 256;
            graphics.PreferredBackBufferHeight = 232;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            drawType = 0;
            levelLoaded = false;
            sounds = new Sounds(Content);
            hud = new Hud();
            gameOver = false;
        }

        public static int getScreenWidth()
        {
            return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        }

        public static int getScreenHeight()
        {
            return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }

        protected override void Initialize()
        {
            setScale();           
            _nativeRenderTarget = new RenderTarget2D(GraphicsDevice, 256, 232);
            origin = new Point(0, 0);
            dynOrigin = new Point();
            base.Initialize();
        }

        private void setScale()
        {
            float scaleX = (float)(getScreenWidth() / 256.0);
            float scaleY = (float)(getScreenHeight() / 232.0);
            if (scaleX > scaleY)          
                screenRect = new Rectangle((int)(getScreenWidth() - 256 * scaleY) / 2, 0, (int)(256*scaleY), getScreenHeight());
            else
                screenRect = new Rectangle(0, (int)(getScreenHeight()-232*scaleX)/2, getScreenWidth(), (int)(232*scaleX));     
        }     


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            character = new Character(this);
            loadBombSprites();
            levels = new Level[64];
            currentLevelNr = 0;
            createCurrentLevel();
            restartLevel();
            spriteAtlas = Content.Load<Texture2D>("big_dyna");
            Texture2D dirButtonsTexture, bombButtonTexture;
            dirButtonsTexture = Content.Load<Texture2D>("directions");
            bombButtonTexture = Content.Load<Texture2D>("bomb");
            int screenHeight = getScreenHeight();
            Rectangle controlsRect = new Rectangle(0, screenHeight / 4, screenHeight / 3, screenHeight / 2);
            int bombSize = (int)(getScreenHeight() * (225 / 1080.0));
            Rectangle bombRect = new Rectangle(getScreenWidth() - bombSize, (getScreenHeight() - bombSize) / 2, bombSize, bombSize);
            controls = new Controls(dirButtonsTexture, controlsRect, bombButtonTexture, bombRect);
            GetHiScore();
            hud.fillHiScoreArray();
        }

        private Point[] getPointsForAnimation(int a)
        {
            return new Point[] { new Point(a + 24, 0), new Point(a, 0), new Point(a + 48, 0), new Point(a, 0) };
        }

        private void loadBombSprites()
        {
            Bomb.burning = new Point[4];
            Bomb.burning[0] = new Point(486, 0);
            Bomb.burning[1] = new Point(470, 0);
            Bomb.burning[2] = new Point(502, 0);
            Bomb.burning[3] = Bomb.burning[1];
            Bomb.topEnd = getExplosionSprites(326, 16);
            Bomb.rightEnd = getExplosionSprites(390, 16);
            Bomb.bottomEnd = getExplosionSprites(454, 16);
            Bomb.leftEnd = getExplosionSprites(518, 16);
            Bomb.vertical = getExplosionSprites(582, 16);
            Bomb.horizontal = getExplosionSprites(326, 32);
            Bomb.center = new Point[7];
            Bomb.center[0] = new Point(454, 32);
            Bomb.center[1] = new Point(422, 32);
            Bomb.center[2] = new Point(406, 32);
            Bomb.center[3] = new Point(390, 32);
            Bomb.center[4] = Bomb.center[2];
            Bomb.center[5] = Bomb.center[1];
            Bomb.center[6] = new Point(438, 32);
        }

        private Point[] getExplosionSprites(int x, int y)
        {
            Point[] sprites = new Point[7];
            sprites[0] = new Point(x+48, y);
            sprites[1] = new Point(x+32, y);
            sprites[2] = new Point(x+16, y);
            sprites[3] = new Point(x, y);
            sprites[4] = sprites[2];
            sprites[5] = sprites[1];
            sprites[6] = sprites[0];
            return sprites;
        }

        protected override void UnloadContent()
        {

        }

        private void restartLevel()
        {
            MediaPlayer.Play(sounds.stageStart);
            NonGameplay.stageNrTime = gameMiliseconds;
            if (currentLevelNr > 63)
                currentLevelNr = 0;
            if (levels[currentLevelNr] == null)
                createCurrentLevel();
            else
                levels[currentLevelNr].initialize();
            character.restart();
            levelLoaded = true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            base.Update(gameTime);
            gameMiliseconds += gameTime.ElapsedGameTime.Milliseconds;

            if (drawType == 1)
            {
                touchCollection = TouchPanel.GetState();
                character.Update(controls);
                int levelWidth = (levels[currentLevelNr].columns - 1) * 16;
                int levelHeight = levels[currentLevelNr].rows * 16;

                dynOrigin.X = -((int)character.x - 116);
                if (character.x <= 116)
                    dynOrigin.X = 0;
                else if (dynOrigin.X < -levelWidth + 256)
                    dynOrigin.X = -levelWidth + 256;

                dynOrigin.Y = -((int)character.y - 128);
                if (character.y <= 128)
                    dynOrigin.Y = 0;
                else if (dynOrigin.Y < -levelHeight + 208)
                    dynOrigin.Y = -levelHeight + 208;

                levels[currentLevelNr].updateMonsters(character);
                hud.update();

                if (!character.visible)
                {
                    if (character.dead && gameMiliseconds - character.deathTime > 3000)
                    {
                        drawType = 0;
                        if (gameOver)
                        {
                            restart();
                            return;
                        }
                        restartLevel();
                    }
                    if (character.cuts == 24 && gameMiliseconds - character.cutTime > 1000)
                    {
                        drawType = 0;                      
                        int bonusNr = currentLevelNr % 4;
                        switch (bonusNr)
                        {
                            case 0: character.bombPower++; break;
                            case 1:
                                {
                                    if (character.lives < 9)
                                        character.lives++;
                                    break;
                                }
                            case 2: character.bombsAvailable++; break;                                                      
                            case 3: if (character.speed<2.0) character.speed += 0.1; break;
                        }
                        currentLevelNr++;
                        restartLevel();
                    }
                }
            }
            else if (gameMiliseconds - NonGameplay.stageNrTime > 3000 && levelLoaded)
            {
                drawType = 1;
                Level.startTime = gameMiliseconds;
                sounds.music[currentLevelNr%3].Play();
            }
        }     

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_nativeRenderTarget);
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
            spriteBatch.Begin();
            if (drawType == 1)
            {
                levels[currentLevelNr].draw(spriteBatch);                                                 //rysowanie poziomu           
                character.draw(spriteBatch);                                                              //rysowanie postaci      
              //DrawRectangle(debug, Color.White);                                                        //rysowanie prostokąta do debugowania
                hud.draw(spriteBatch);                                                                    //rysowanie hud'a
            }
            else
                NonGameplay.drawStageNr(spriteBatch);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(_nativeRenderTarget, screenRect, Color.White);
            if (drawType==1)
                controls.draw(spriteBatch);
            spriteBatch.End();
        }

        public void restart()
        {
            if (score >= hiScore)
                saveHiScore();
            score = 0;
            gameMiliseconds = 0;
            drawType = 0;
            levelLoaded = false;
            character = new Character(this);
            currentLevelNr = 0;
            createCurrentLevel();
            restartLevel();
        }

        void DrawRectangle(Rectangle coords, Color color)
        {
            Texture2D rect = new Texture2D(GraphicsDevice, 1, 1);
            rect.SetData(new[] { Color.White });

            spriteBatch.Draw(rect, coords, color);
        }

        private void createCurrentLevel()
        {
            switch(currentLevelNr)
            {
                case 0: levels[0] = new Level(13, 17, 0, new int[] { 0, 1, 2 }); break;
                case 1: levels[1] = new Level(19, 17, 1, new int[] { 3, 4, 5}); break;
                case 2: levels[2] = new Level(13, 23, 2, new int[] { 6, 7, 8 }); break;
                default: levels[currentLevelNr] = Level.getRandomLevel(); break;
            }
            

        }

        public static Boolean isWalkable(Point p)
        {
            int column = p.X;
            int row = p.Y;
            if (!levels[currentLevelNr].tiles[row, column].walkable)
                return false;
            if (levels[currentLevelNr].tiles[row, column].hasObstacle)
                return false;
            foreach (Bomb bomb in character.bombs)
                if (bomb.row == row && bomb.column == column)
                    return false;
            return true;
        }

        private void GetHiScore()
        {
            var store = IsolatedStorageFile.GetUserStoreForApplication();

            if (store.FileExists("score.txt"))
            {
                var fs = store.OpenFile("score.txt", FileMode.Open);
                using (StreamReader sr = new StreamReader(fs))
                {
                    hiScore = Convert.ToInt16(sr.ReadLine());
                }
            }
            else
            {
                var fs = store.CreateFile("score.txt");
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write("0");
                }
                hiScore = 0;
            }
        }

        private void saveHiScore()
        {
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            var fs = store.OpenFile("score.txt", FileMode.OpenOrCreate);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(score);
            }
            hiScore = score;
            hud.fillHiScoreArray();
        }
    }

}
