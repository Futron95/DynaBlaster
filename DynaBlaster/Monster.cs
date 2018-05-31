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
    public class Monster : Entity
    {
        public static int[][] spriteNrs, dyingSpriteNrs;
        public static int[] scores;
        static int comboMultiplier;
        static long lastDeathTime;
        int type, actualScore;
        public Boolean scoreDisplay;
        static Dictionary<int, int> scoreDictionary;

        public int column
        {
            get { return (int)(x / 16 + 1); }
        }

        public int row
        {
            get { return (int)((y - 14) / 16); }
        }

        static Monster()
        {
            spriteNrs = new int[19][];
            spriteNrs[0] = getNrs(7, 4);
            spriteNrs[1] = new int[4] { 12, 13, 15, 14 };
            spriteNrs[2] = getNrs(18, 4);
            spriteNrs[3] = getNrs(25, 4);
            spriteNrs[4] = getNrs(30, 16);
            spriteNrs[5] = getNrs(46, 4);
            spriteNrs[6] = getNrs(53, 4);
            spriteNrs[7] = getNrs(68, 4);
            spriteNrs[8] = getNrs(75,4);
            spriteNrs[9] = getNrs(80,4);
            spriteNrs[10] = getNrs(87,4);
            spriteNrs[11] = getNrs(94,4);
            spriteNrs[12] = getNrs(101,4);
            spriteNrs[13] = getNrs(108,4);
            spriteNrs[14] = getNrs(115,16);
            spriteNrs[15] = new int[4] { 129, 130, 131, 132 };
            spriteNrs[16] = new int[16] { 142, 141, 142, 141, 139, 140, 145, 140, 143, 144, 143, 144, 137, 138, 147, 138 };
            spriteNrs[17] = getNrs(152,16);
            spriteNrs[18] = getNrs(168,4);
            dyingSpriteNrs = new int[19][];
            dyingSpriteNrs[0] = new int[10] { 10,10,10,10,10,11,0,1,2,3};
            dyingSpriteNrs[1] = new int[10] { 16,16,16,16,17,11,0,1,2,3};
            dyingSpriteNrs[2] = new int[10] { 21,21,21,21,22,23,24,4,5,6};
            dyingSpriteNrs[3] = new int[10] { 28,28,28,28,29,11,0,1,2,3};
            dyingSpriteNrs[4] = new int[10] { 42,42,42,42,43,44,45,4,5,6};
            dyingSpriteNrs[5] = new int[10] { 49,49,49,49,50,51,52,4,5,6};
            dyingSpriteNrs[6] = new int[10] { 56,56,56,56,57,58,59,4,5,6};
            dyingSpriteNrs[7] = new int[10] { 71,71,71,71,72,73,74,4,5,6};
            dyingSpriteNrs[8] = new int[10] { 78,78,78,78,79,11,0,1,2,3};
            dyingSpriteNrs[9] = new int[10] { 83,83,83,83,84,85,86,4,5,6};
            dyingSpriteNrs[10] = new int[10] { 90,90,90,90,91,92,93,4,5,6};
            dyingSpriteNrs[11] = new int[10] { 97,97,97,97,98,99,100,4,5,6};
            dyingSpriteNrs[12] = new int[10] { 104,104,104,104,105,106,107,4,5,6};
            dyingSpriteNrs[13] = new int[10] { 111,111,111,111,112,113,114,4,5,6};
            dyingSpriteNrs[14] = new int[10] { 127,127,127,127,128,175,176,4,5,6};
            dyingSpriteNrs[15] = new int[10] { 133,133,133,133,134,135,136,4,5,6};
            dyingSpriteNrs[16] = new int[10] { 148,148,148,148,149,150,151,4,5,6};
            dyingSpriteNrs[17] = new int[10] { 164,164,164,164,165,166,167,4,5,6};
            dyingSpriteNrs[18] = new int[10] { 171,171,171,171,172,173,174,4,5,6};
            scores = new int[19] { 100, 200, 1000, 2000, 400, 1000, 400, 100, 400, 100, 400, 100, 400, 100, 400, 400, 100, 100, 400 };
            scoreDictionary = new Dictionary<int, int>();
            scoreDictionary.Add(100, 188);
            scoreDictionary.Add(200, 189);
            scoreDictionary.Add(400, 190);
            scoreDictionary.Add(800, 191);
            scoreDictionary.Add(1600, 192);
            scoreDictionary.Add(3200, 193);
            scoreDictionary.Add(6400, 194);
            scoreDictionary.Add(1000, 195);
            scoreDictionary.Add(2000, 196);
            scoreDictionary.Add(4000, 197);
            scoreDictionary.Add(8000, 198);
            comboMultiplier = 1;
            lastDeathTime = -3000;
        }

        static int[] getNrs(int i, int l)
        {
            int[] array;
            if (l == 4)
                array = new int[4] { i, i + 1, i + 2, i + 1 };
            else
                array = new int[16] { i, i + 1, i + 2, i + 1, i + 3, i + 4, i + 5, i + 4, i + 6, i + 7, i + 8, i + 7, i + 9, i + 10, i + 11, i + 10 };
            return array;
        }

        private Rectangle getSourceRectangle(int nr)
        {
            int x = 314 + (nr % 20) * 16;
            int y = 215 + (nr / 20) * 18;
            return new Rectangle(x,y,16,18);
        }

        public Monster(int x, int y, int type)
        {
            this.x = x;
            this.y = y;
            this.width = 16;
            this.height = 18;
            this.type = type;
            walk = new Animation(4, 167);
            dyingAnimation = new Animation(10, 200);
            dir = (direction)r.Next(0,4);
            speed = 0.5;
            visible = true;
            scoreDisplay = false;
        }

        public void update()
        {
            if(dead)
            {
                if (scoreDisplay || Game1.gameMiliseconds-deathTime>dyingAnimation.length)
                {
                    if (actualScore != 0)
                        return;
                    actualScore = scores[type]*comboMultiplier;
                    if (actualScore > 8000)
                        if (scores[type] > 800)
                            actualScore = 8000;
                        else
                            actualScore = 6400;

                    sourceRectangle = getSourceRectangle(scoreDictionary[actualScore]);
                    scoreDisplay = true;
                    Game1.score += actualScore;
                }
                else
                    sourceRectangle = getSourceRectangle(dyingSpriteNrs[type][dyingAnimation.getCurrentFrame()]);
                return;
            }
            Boolean go = true;
            if ((x - 8) % 16 == 0 && (y - 22) % 16 == 0)
                if (!setDirection())
                    go = false;
            if (go)
                switch ((int)dir)
                {
                    case 0: x -= speed; break;
                    case 1: y -= speed; break;
                    case 2: x += speed; break;
                    case 3: y += speed; break;
                }
            int frameNr = walk.getCurrentFrame();
            if (spriteNrs[type].Length == 16)
                frameNr += (int)dir * 4;
            sourceRectangle = getSourceRectangle(spriteNrs[type][frameNr]);
        }

        public void draw(SpriteBatch sb)
        {
            sb.Draw(Game1.spriteAtlas, locationRectangle, sourceRectangle, Color.White);
        }

        private Boolean setDirection()
        {
            Boolean canContinue = Game1.isWalkable(getTarget((int)dir));
            if (r.NextDouble() > 0.1 && canContinue)
                return true;

            int i = r.Next(0, 4), j, limit = i+4;
            for(;i<limit;i++)
            {
                j = i % 4;
                if (j == (int)dir)
                    continue;
                if (Game1.isWalkable(getTarget(j)))
                {
                    dir = (direction)j;
                    return true;
                }
            }
            return canContinue;

        }

        private Point getTarget(int d)
        {
            int targetRow = 0, targetColumn = 0;

            switch (d)
            {
                case 0: targetRow = row; targetColumn = column - 1; break;
                case 1: targetRow = row - 1; targetColumn = column; break;
                case 2: targetRow = row; targetColumn = column + 1; break;
                case 3: targetRow = row + 1; targetColumn = column; break;
            }

            return new Point(targetColumn, targetRow);
        }

        public void die()
        {
            dead = true;
            dyingAnimation.reset();
            deathTime = Game1.gameMiliseconds;
            if (deathTime - lastDeathTime < 3000)
                comboMultiplier *= 2;
            else
                comboMultiplier = 1;
            lastDeathTime = deathTime;
            Game1.levels[Game1.currentLevelNr].monstersNumber--;
        }
    }
}