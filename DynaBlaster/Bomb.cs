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
        
        static Bomb()
        {
            burning = new Point[4];
            burning[0] = new Point(486, 0);
            burning[1] = new Point(470, 0);
            burning[2] = new Point(502, 0);
            burning[3] = Bomb.burning[1];
            topEnd = getExplosionSprites(326, 16);
            rightEnd = getExplosionSprites(390, 16);
            bottomEnd = getExplosionSprites(454, 16);
            leftEnd = getExplosionSprites(518, 16);
            vertical = getExplosionSprites(582, 16);
            horizontal = getExplosionSprites(326, 32);
            center = new Point[7];
            center[0] = new Point(454, 32);
            center[1] = new Point(422, 32);
            center[2] = new Point(406, 32);
            center[3] = new Point(390, 32);
            center[4] = Bomb.center[2];
            center[5] = Bomb.center[1];
            center[6] = new Point(438, 32);
        }

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

        public void destroy(Character character)    //niszczy postacie potwory i inne bomby znajdujące się w zasięgu wybuchu
        {
            if (character.row == row && character.column == column)
                character.die();
            int checkedRow = 0, checkedColumn = 0, i;
            for (int dir = 0; dir < 4; dir++)
            {
                for (i = 1; i <= explosionRange[dir]; i++)
                {
                    switch (dir)        //określanie w którym rzędzie i kolumnie znajduje się następny sprawdzany tile na podstawie kierunku(dir) i odległości od bomby(i)
                    {
                        case 0: checkedRow = row - i; checkedColumn = column; break;
                        case 1: checkedRow = row; checkedColumn = column + i; break;
                        case 2: checkedRow = row + i; checkedColumn = column; break;
                        case 3: checkedRow = row; checkedColumn = column - i; break;
                    }
                    if (!character.dead && checkedRow == character.row && checkedColumn == character.column && !character.teleporting)
                        character.die();
                    foreach (Monster m in Game1.levels[Game1.currentLevelNr].monsters)      //sprawdzanie czy wybuch sięgnie potwora
                        if (!m.dead && checkedRow == m.row && checkedColumn == m.column)
                            m.die();
                    foreach (Bomb bomb in character.bombs)                                  //sprawdzanie czy wybuch sięgnie inną bombę
                        if (!bomb.exploded && bomb.column == checkedColumn && bomb.row == checkedRow)
                        {
                            bomb.explode(character);                  
                            break;
                        }
                }
            }
        }

        public void explode(Character character)
        {
            explosionAnimation = new Animation(7, 67);
            exploded = true;
            character.bombsAvailable++;
            explosionTime = Game1.gameMiliseconds;
            Game1.sounds.explosion.Stop();
            Game1.sounds.explosion.Play();
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
                                continue;
                        }
                        else
                            checkedTile.destroy();
                    }                   
                    break;
                }
                explosionRange[dir] = i - 1;
            }
            destroy(character);
        }

        private static Point[] getExplosionSprites(int x, int y)
        {
            Point[] sprites = new Point[7];
            sprites[0] = new Point(x + 48, y);
            sprites[1] = new Point(x + 32, y);
            sprites[2] = new Point(x + 16, y);
            sprites[3] = new Point(x, y);
            sprites[4] = sprites[2];
            sprites[5] = sprites[1];
            sprites[6] = sprites[0];
            return sprites;
        }
    }
}