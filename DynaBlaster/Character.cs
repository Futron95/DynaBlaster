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
        public Boolean idle, teleporting;
        public int bombPower, lives;
        Animation teleportingAnimation, starsAnimation;
        Rectangle starsSourceRectangle;
        public long cutTime;
        public int bombsAvailable, cuts;       

        public int column
        {
            get { return (int)((x + 3) / 16 + 1); }
        }

        public int row
        {
            get { return (int)((y - 11) / 16); }
        }

        public List<Bomb> bombs;

        static Character()
        {
            Character.sprites = new Point[20];
            for (int i = 0; i < 13; i++)
                Character.sprites[i] = new Point(i * 24, 0);
            for (int i = 13; i < 20; i++)
                Character.sprites[i] = new Point((i - 13) * 24, 24);
            Character.left = new int[4] { 6, 7, 6, 8 };
            Character.right = new int[4] { 3, 4, 3, 5 };
            Character.up = new int[4] { 9, 10, 9, 11 };
            Character.down = new int[4] { 0, 1, 0, 2 };
            Character.dying = new int[12] { 12, 13, 12, 13, 12, 13, 14, 15, 16, 17, 18, 19 };
        }   

        public Character()
        {
            width = 24;
            height = 24;
            sourceRectangle = new Rectangle(0, 0, 24, 24);
            speed = 1.0;    
            bombsAvailable = 4;
            bombPower = 3;
            lives = 2;
            deathTime = 0;
            walk = new Animation(4, 125);
            dyingAnimation = new Animation(12, 167);
            teleportingAnimation = new Animation(4, 125);
            starsAnimation = new Animation(4, 66);
        }

        public void restart()
        {
            x = 21;
            y = 35;
            height = 24;
            visible = true;
            dead = false;
            teleporting = false;
            dir = direction.DOWN;
            bombs = new List<Bomb>();
            cuts = 0;
        }

        private void setTeleportingSourceRectangle()
        {
            if (Game1.gameMiliseconds - cutTime > 166)
            {
                cuts++;
                y++;
                height--;
                cutTime = Game1.gameMiliseconds;

                if (cuts == 24)
                {
                    visible = false;
                    return;
                }
            }
            int frameNr = teleportingAnimation.getCurrentFrame();
            Point point;
            switch (frameNr)
            {
                case 0: point = sprites[left[0]]; break;
                case 1: point = sprites[up[0]]; break;
                case 2: point = sprites[right[0]]; break;
                case 3: point = sprites[down[0]]; break;
                default: point = new Point(0, 0); break;
            }
            point.Y += cuts;
            sourceRectangle = new Rectangle(point, new Point(width, height));
        }

        public void Update(Controls controls)
        {
            if (!visible)
                return;
            if (!teleporting)
            {
                if (Game1.levels[Game1.currentLevelNr].monstersNumber == 0 && Game1.levels[Game1.currentLevelNr].tiles[row, column].hasTeleport)
                {
                    if (Math.Abs((int)x+3 - (column*16-8))<3 && Math.Abs((int)y+5 - (row*16+24))<3)
                    {
                        x = column * 16 - 11;
                        y = row * 16 + 19;
                        teleporting = true;
                        cutTime = Game1.gameMiliseconds;
                    }
                }
            }
            if (teleporting)
            {
                setTeleportingSourceRectangle();
                setStarsSourceRectangle(starsAnimation.getCurrentFrame());
                return;
            }
            if (dead)
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
                                    walk.reset();
                                    dir = direction.UP;
                                }
                                idle = false;
                                goVertically();
                                sourceRectangle.Location = sprites[up[walk.getCurrentFrame()]];
                            }
                            break;
                        }
                    case 1:
                        {
                            if (idle == true)
                            {
                                if (dir != direction.DOWN)
                                {
                                    walk.reset();
                                    dir = direction.DOWN;
                                }
                                idle = false;
                                goVertically();
                                sourceRectangle.Location = sprites[down[walk.getCurrentFrame()]];
                            }
                            break;
                        }
                    case 2:
                        {
                            if (idle == true)
                            {
                                if (dir != direction.LEFT)
                                {
                                    walk.reset();
                                    dir = direction.LEFT;
                                }
                                idle = false;
                                goHorizontally();
                                sourceRectangle.Location = sprites[left[walk.getCurrentFrame()]];
                            }
                            break;
                        }
                    case 3:
                        {
                            if (idle == true)
                            {
                                if (dir != direction.RIGHT)
                                {
                                    walk.reset();
                                    dir = direction.RIGHT;
                                }
                                idle = false;
                                goHorizontally();
                                sourceRectangle.Location = sprites[right[walk.getCurrentFrame()]];
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
            {
                sb.Draw(Game1.spriteAtlas, locationRectangle, sourceRectangle, Color.White);
                if (teleporting)
                    sb.Draw(Game1.spriteAtlas, new Rectangle((int)x, (int)y-8, 28, 16), starsSourceRectangle, Color.White);
            }
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
            Game1.debug = new Rectangle(targetColumn * 16 - 4, targetRow * 16 + 28, 8, 8);
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
            Game1.debug = new Rectangle(targetColumn * 16 - 4, targetRow * 16 + 28, 8, 8);
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

        private void setStarsSourceRectangle(int frame)
        {
            starsSourceRectangle = new Rectangle(170 + frame * 28, 24, 28, 16);
        }
    }
}