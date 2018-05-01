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
/*TODO:
*      2 wymiarowa tablica na rodzaje potworow i nr ich ruchow
*      wypelnienie tej tablicy podczas inicjalizacji
*      boolean mowiacy czy potwor ma osobne sprity dla kazdego kierunku
*       metoda zwracajaca source rectangle na podstawie nr sprite'a
*       tworzenie animacji chodzenia w konstruktorze
*       metoda update w której ustalany będzie source rectangle i pozycja potwora wygenerowana z algorytmu chodzenia
*       metoda draw rysująca potwora
*       dodac liste potworow do klasy level
*/
namespace DynaBlaster
{
    public class Monster : Entity
    {
        public static int[][] SpriteNrs;
        int type;
        Animation walk;

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
            SpriteNrs = new int[19][];
            SpriteNrs[0] = getNrs(7, 4);
            SpriteNrs[1] = new int[4] { 12, 13, 15, 14 };
            SpriteNrs[2] = getNrs(18, 4);
            SpriteNrs[3] = getNrs(25, 4);
            SpriteNrs[4] = getNrs(30, 16);
            SpriteNrs[5] = getNrs(46, 4);
            SpriteNrs[6] = getNrs(53, 4);
            SpriteNrs[7] = getNrs(68, 4);
            SpriteNrs[8] = getNrs(75,4);
            SpriteNrs[9] = getNrs(80,4);
            SpriteNrs[10] = getNrs(87,4);
            SpriteNrs[11] = getNrs(94,4);
            SpriteNrs[12] = getNrs(101,4);
            SpriteNrs[13] = getNrs(108,4);
            SpriteNrs[14] = getNrs(115,16);
            SpriteNrs[15] = new int[4] { 129, 130, 131, 132 };
            SpriteNrs[16] = getNrs(137,16);
            SpriteNrs[17] = getNrs(152,16);
            SpriteNrs[18] = getNrs(168,4);
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
            r = new Random();
            dir = (direction)r.Next(0,4);
            speed = 0.5;
            visible = true;
            sourceRectangle = getSourceRectangle(SpriteNrs[type][0]);
        }

        public void update()
        {
            direction current = dir;
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
            if (SpriteNrs[type].Length == 16)
                frameNr += (int)dir * 4;
            sourceRectangle = getSourceRectangle(SpriteNrs[type][frameNr]);
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
    }
}