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
        public Rectangle sourceRectangle;
        public Boolean walkable, hasObstacle, destroyed;
        public Animation destroyAnimation;
        long destroyTime;
        public Tile(Rectangle rect, Boolean walkable=false)
        {
            sourceRectangle = rect;
            this.walkable = walkable;
            destroyed = false;
        }

        public void destroy()
        {
            destroyAnimation = new Animation(7, 67);
            destroyed = true;
            destroyTime = Game1.gameMiliseconds;
        }

        public void draw(SpriteBatch sb, int x, int y)
        {
            if (destroyed && Game1.gameMiliseconds - destroyTime > destroyAnimation.length)
                hasObstacle = false;
            if (!hasObstacle)
                sb.Draw(Game1.spriteAtlas, new Rectangle(x, y, 16, 16), sourceRectangle, Color.White);
            else
            if (!destroyed)
                sb.Draw(Game1.spriteAtlas, new Rectangle(x, y, 16, 16), Game1.levels[Game1.currentLevelNr].obstacleTile.sourceRectangle, Color.White);
            else
            {
                sb.Draw(Game1.spriteAtlas, new Rectangle(x, y, 16, 16), sourceRectangle, Color.White);
                sb.Draw(Game1.spriteAtlas, new Rectangle(x, y, 16, 16), new Rectangle(Game1.levels[Game1.currentLevelNr].obstacleDestroy[destroyAnimation.getCurrentFrame()], new Point(16, 16)), Color.White);
            }
        }
    }
}