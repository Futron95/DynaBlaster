using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynaBlaster
{
    public class Tile
    {
        private static Rectangle[] teleportRectangle;
        private static Tile[,] tiles;
        private static Animation teleportAnimation;
        public Rectangle sourceRectangle;
        public Boolean walkable, hasObstacle, destroyed, hasTeleport;
        public Animation destroyAnimation;
        
        long destroyTime;

        static Tile()
        {
            tiles = new Tile[8, 35];
            int x = 367;
            int y = 110;
            for (int i = 0; i < 8; i++)
            {
                loadTiles(i, x, y);
                y += 33;
                if (i == 2)
                {
                    x = 689;
                    y = 44;
                }
            }
            teleportRectangle = new Rectangle[2]{ new Rectangle(240,48,16,16), new Rectangle(256,48,16,16)};
            teleportAnimation = new Animation(2, 250);
        }

        private static void loadTiles(int tileNr, int x, int y)
        {
            for (int column = 0; column < 20; column++)
                tiles[tileNr, column] = new Tile(new Rectangle(x + column * 16, y, 16, 16));
            for (int column = 0; column < 15; column++)
                tiles[tileNr, column + 20] = new Tile(new Rectangle(x + column * 16, y + 16, 16, 16));
            tiles[tileNr, 0].walkable = true;
            tiles[tileNr, 1].walkable = true;
        }

        public static Tile getTile(int type, int nr)
        {
            if (!tiles[type, nr].walkable)
                return tiles[type, nr];
            else
                return new Tile(tiles[type, nr].sourceRectangle, tiles[type, nr].walkable);
        }

        public Tile(Rectangle rect, Boolean walkable=false)
        {
            sourceRectangle = rect;
            this.walkable = walkable;
            destroyed = false;
            hasTeleport = false;
        }

        public void destroy()
        {
            destroyed = true;
            if (hasTeleport)
            {
                hasObstacle = false;
            }
            destroyAnimation = new Animation(7, 67);            
            destroyTime = Game1.gameMiliseconds;
        }

        public void draw(SpriteBatch sb, int x, int y, Level level)
        {
            if (destroyed && Game1.gameMiliseconds - destroyTime > destroyAnimation.length)     //jezeli animacja niszczenia sie skończyłaa to ustaw że tile nie ma już przeszkody
                hasObstacle = false;
            if (!hasObstacle) //jeżeli nie ma przeszkody to wyświetl tile taki jaki jest, chyba że ma teleport
            {
                if (hasTeleport)
                {
                    if (level.monstersNumber > 0)
                        sb.Draw(Game1.spriteAtlas, new Rectangle(x, y, 16, 16), teleportRectangle[0], Color.White);
                    else
                        sb.Draw(Game1.spriteAtlas, new Rectangle(x, y, 16, 16), teleportRectangle[teleportAnimation.getCurrentFrame()], Color.White);
                }   
                else
                    sb.Draw(Game1.spriteAtlas, new Rectangle(x, y, 16, 16), sourceRectangle, Color.White);
            }
            else
            if (!destroyed)                                                                     //jeżeli nie ma przeszkody i nie został zniszczony to wyświetl obstacleTile obecnego poziomu
                sb.Draw(Game1.spriteAtlas, new Rectangle(x, y, 16, 16), level.obstacleTile.sourceRectangle, Color.White);
            else
            {                                                                                   //jeżeli obiekt jest w trakcie niszczenia to wyświetl jego normalny wygląd a na tym odpowiednią klatkę z animacji niszczenia
                sb.Draw(Game1.spriteAtlas, new Rectangle(x, y, 16, 16), sourceRectangle, Color.White);
                sb.Draw(Game1.spriteAtlas, new Rectangle(x, y, 16, 16), new Rectangle(level.obstacleDestroy[destroyAnimation.getCurrentFrame()], new Point(16, 16)), Color.White);
            }
        }
    }
}