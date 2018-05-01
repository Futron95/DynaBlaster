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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace DynaBlaster
{
    public class Character : Entity
    {
        public static Point[] sprites;
        public static int[] left, right, up, down, dying;
        public Boolean idle;
        public int bombPower, lives;

       
        private int bombsAvailable;
        
        

        public int column
        {
            get { return (int)((x + 3) / 16 + 1); }
        }

        public int row
        {
            get { return (int)((y - 11) / 16); }
        }

        public List<Bomb> bombs;
        
        public void restart()
        {
            x = 21;
            y = 35;
            visible = true;
            dead = false;
            dir = direction.DOWN;
            bombs = new List<Bomb>();
        }

        public Character()
        {
            width = 24;
            height = 24;
            sourceRectangle = new Rectangle(0, 0, 24, 24);
            speed = 0.8;    
            bombsAvailable = 4;
            bombPower = 3;
            lives = 2;
            deathTime = 0;
        }

        public void Update(Controls controls)
        {
            if (dead && visible)
            {
                if (Game1.gameMiliseconds - deathTime < dyingAnimation.length)
                    sourceRectangle.Location = sprites[dying[dyingAnimation.getCurrentFrame()]];
                else
                    visible = false;
                return;
            }
            idle = true;
            foreach (TouchLocation tl in Game1.touchCollection)
            {
                int ctrlNr = controls.getTouchedDirection(tl);

                switch (ctrlNr)
                {
                    case 0:
                        {
                            if (idle == true)
                            {
                                if (dir != direction.UP)
                                {
                                    walkingUp.reset();
                                    dir = direction.UP;
                                }
                                idle = false;
                                goVertically();
                                sourceRectangle.Location = sprites[up[walkingUp.getCurrentFrame()]];
                            }
                            break;
                        }
                    case 1:
                        {
                            if (idle == true)
                            {
                                if (dir != direction.DOWN)
                                {
                                    walkingDown.reset();
                                    dir = direction.DOWN;
                                }
                                idle = false;
                                goVertically();
                                sourceRectangle.Location = sprites[down[walkingDown.getCurrentFrame()]];
                            }
                            break;
                        }
                    case 2:
                        {
                            if (idle == true)
                            {
                                if (dir != direction.LEFT)
                                {
                                    walkingLeft.reset();
                                    dir = direction.LEFT;
                                }
                                idle = false;
                                goHorizontally();
                                sourceRectangle.Location = sprites[left[walkingLeft.getCurrentFrame()]];
                            }
                            break;
                        }
                    case 3:
                        {
                            if (idle == true)
                            {
                                if (dir != direction.RIGHT)
                                {
                                    walkingRight.reset();
                                    dir = direction.RIGHT;
                                }
                                idle = false;
                                goHorizontally();
                                sourceRectangle.Location = sprites[right[walkingRight.getCurrentFrame()]];
                            }
                            break;
                        }
                    case 4:
                        {
                            if (bombsAvailable>0)
                            {
                                int bombColumn = column;
                                int bombRow = row;
                                Boolean occupied = false;
                                foreach (Bomb bomb in bombs)
                                    if (bomb.column == bombColumn && bomb.row == bombRow)
                                    {
                                        occupied = true;
                                        break;
                                    }
                                if (!occupied)
                                {
                                    bombs.Add(new Bomb(bombColumn, bombRow));
                                    bombsAvailable--;
                                }
                            }
                            break;
                        }
                }
            }
            if (idle == false)
                return;
            int idleX = 0;
            switch (dir)
            {
                case direction.RIGHT: idleX = 72; break;
                case direction.LEFT: idleX = 144; break;
                case direction.UP: idleX = 216; break;
            }
            sourceRectangle = new Rectangle(idleX, 0, 24, 24);
        }

        public void die()
        {
            dead = true;
            lives--;
            dyingAnimation.reset();
            deathTime = Game1.gameMiliseconds;
        }

        public void draw(SpriteBatch sb)
        {
            Bomb bomb;
            for (int i = bombs.Count - 1; i >= 0; i--)
            {
                bomb = bombs.ElementAt(i);
                if (bomb.exploded && Game1.gameMiliseconds-bomb.explosionTime>bomb.explosionAnimation.length)
                {
                    bombs.Remove(bomb);
                    bombsAvailable++;
                }
                else
                {
                    if (!bomb.exploded && Game1.gameMiliseconds - bomb.plantTime > 2500)
                        bomb.explode(this);
                    bomb.draw(sb, bombPower);
                }
            }
            if (visible)
                sb.Draw(Game1.spriteAtlas, locationRectangle, sourceRectangle, Color.White);           
        }

        private void goHorizontally()
        {
            double dist;
            if (dir == direction.LEFT)
            {
                if (locationRectangle.X <= 21)
                    return;
                else
                    dist = -speed;
            }
            else
            {
                int levelWidth = (Game1.levels[Game1.currentLevelNr].columns - 1) * 16;
                if (locationRectangle.X >= levelWidth - 43)
                    return;
                else
                    dist = speed;
            }
            int type = (locationRectangle.Y - 35) % 32;
            int targetRow = (locationRectangle.Y - 35 - type) / 32 * 2 + 1;
            if (type > 16)
                targetRow += 2;
            int targetColumn = (locationRectangle.X - 21) / 16+2;
            if (dir == direction.LEFT && (locationRectangle.X - 21) % 16 == 0)
                targetColumn--;
            else
            if (dir == direction.RIGHT)
                targetColumn++;
            //Game1.debug = new Rectangle(targetColumn * 16 - 4, targetRow * 16 + 28, 8, 8);
            if (Game1.levels[Game1.currentLevelNr].tiles[targetRow, targetColumn].hasObstacle)
                return;
            if (type == 0)
                x += dist;
            else
            if (type > 0 && type < 7)
            {
                x += dist;
                y -= speed;
            }
            else
            if (type > 6 && type < 16)
                y -= speed;
            else
            if (type > 16 && type < 24)
                y += speed;
            else
            if (type > 23)
            {
                x += dist;
                y += speed;
            }
        }

        private void goVertically()
        {
            double dist;
            if (dir == direction.UP)
            {
                if (locationRectangle.Y <= 35)
                    return;
                else
                    dist = -speed;
            }
            else
            {
                int levelHeight = Game1.levels[Game1.currentLevelNr].rows * 16 + 24;
                if (locationRectangle.Y >= levelHeight - 37)
                    return;
                else
                    dist = speed;
            }
            int type = (locationRectangle.X - 21) % 32;
            int targetColumn = (locationRectangle.X - 21 - type) / 32 * 2 + 2;
            if (type >= 16)
                targetColumn += 2;
            int targetRow = (locationRectangle.Y - 35) / 16+1;
            if (dir == direction.UP && (locationRectangle.Y - 35) % 16 == 0)
                targetRow--;
            else
            if (dir == direction.DOWN)
                targetRow++;
            //Game1.debug = new Rectangle(targetColumn * 16 - 4, targetRow * 16 + 28, 8, 8);
            if (Game1.levels[Game1.currentLevelNr].tiles[targetRow, targetColumn].hasObstacle)
                return;
            if (type == 0)
                y += dist;
            else
            if (type > 0 && type < 7)
            {
                y += dist;
                x -= speed;
            }
            else
            if (type > 6 && type < 16)
                x -= speed;
            else
            if (type > 16 && type < 24)
                x += speed;
            else
            if (type > 23)
            {
                y += dist;
                x += speed;
            }
        }
    }
}