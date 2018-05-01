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
    public class Bomb
    {
        public static Point[] burning, topEnd, rightEnd, bottomEnd, leftEnd, vertical, horizontal, center;

        public int column, row;
        public long plantTime, explosionTime;
        int[] explosionRange;
        public Boolean exploded;
        Point size;
        public Animation burningAnimation, explosionAnimation;
        

        public Bomb(int column, int row)
        {
            this.column = column;
            this.row = row;
            size = new Point(16, 16);
            burningAnimation = new Animation(4, 250);
            plantTime = Game1.gameMiliseconds;
            exploded = false;
        }

        public void draw(SpriteBatch sb, int bombPower)
        {
            int x = column * 16 - 8 + Game1.dynOrigin.X;
            int y = row * 16 + 24 + Game1.dynOrigin.Y;
            if (exploded)
            {
                Point[] currentAnimation;
                int targetY = 0, targetX = 0, currentFrame = explosionAnimation.getCurrentFrame();
                for (int dir = 0; dir < 4; dir++)
                {
                    for (int i = 1; i <= explosionRange[dir]; i++)
                    {
                        switch (dir)
                        {
                            case 0:
                                {
                                    targetY = y - i * 16;
                                    targetX = x;
                                    if (i == bombPower)
                                        currentAnimation = topEnd;
                                    else
                                        currentAnimation = vertical;
                                    break;
                                }
                            case 1:
                                {
                                    targetY = y;
                                    targetX = x + i * 16;
                                    if (i == bombPower)
                                        currentAnimation = rightEnd;
                                    else
                                        currentAnimation = horizontal;
                                    break;
                                }
                            case 2:
                                {
                                    targetY = y + i * 16;
                                    targetX = x;
                                    if (i == bombPower)
                                        currentAnimation = bottomEnd;
                                    else
                                        currentAnimation = vertical;
                                    break;
                                }
                            case 3:
                                {
                                    targetY = y;
                                    targetX = x - i * 16;
                                    if (i == bombPower)
                                        currentAnimation = leftEnd;
                                    else
                                        currentAnimation = horizontal;
                                    break;
                                }
                            default: currentAnimation = null; break;
                        }
                        sb.Draw(Game1.spriteAtlas, new Rectangle(targetX, targetY, 16, 16), new Rectangle(currentAnimation[currentFrame], new Point(16, 16)), Color.White);
                    }
                }
                sb.Draw(Game1.spriteAtlas, new Rectangle(x, y, 16, 16), new Rectangle(center[currentFrame], new Point(16, 16)), Color.White);
            }
            else
                sb.Draw(Game1.spriteAtlas, new Rectangle(x, y, size.X, size.Y), new Rectangle(burning[burningAnimation.getCurrentFrame()], size), Color.White);
        }

        public void explode(Character character)
        {
            explosionAnimation = new Animation(7, 67);
            exploded = true;
            explosionTime = Game1.gameMiliseconds;
            explosionRange = new int[4];
            int checkedRow=0, checkedColumn=0, i;
            for (int dir = 0; dir < 4; dir++)
            {
                for (i = 1; i <= character.bombPower; i++)
                {                    
                    switch (dir)
                    {
                        case 0: checkedRow = row - i; checkedColumn = column; break;
                        case 1: checkedRow = row; checkedColumn = column + i; break;
                        case 2: checkedRow = row + i; checkedColumn = column; break;
                        case 3: checkedRow = row; checkedColumn = column - i; break;
                    }
                    int levelRows = Game1.levels[Game1.currentLevelNr].rows;
                    int levelColumns = Game1.levels[Game1.currentLevelNr].columns;
                    if (checkedRow>0 && checkedColumn >1 && checkedRow<levelRows-1 && checkedColumn<levelColumns-2)
                    {
                        Tile checkedTile = Game1.levels[Game1.currentLevelNr].tiles[checkedRow, checkedColumn];
                        if (!checkedTile.hasObstacle)
                        {
                            if (checkedTile.walkable)
                            {
                                if (!character.dead && checkedRow == character.row && checkedColumn == character.column)
                                    character.die();
                                Boolean trigger = false;
                                foreach (Bomb bomb in character.bombs)                          //sprawdzanie czy wybuch sięgnie inną bombę
                                    if (!bomb.exploded && bomb.column == checkedColumn && bomb.row == checkedRow)
                                    {
                                        trigger = true;
                                        bomb.explode(character);                                //TODO: sprawdzić czy wybuch zabija postać, potwora lub bonus
                                        break;
                                    }
                                if (!trigger)
                                    continue;
                            }
                        }
                        else
                            checkedTile.destroy();        //TODO: wywolanie animacji wybuchania obstacla
                    }                   
                    break;
                }
                explosionRange[dir] = i - 1;
            }
        }
    }
}