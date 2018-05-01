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
    public static class NonGameplay
    {
        static Rectangle stageNrRect;
        static Rectangle[] numbers;
        public static long stageNrTime;

        static NonGameplay()
        {
            stageNrRect = new Rectangle(213, 64, 107, 30);
            numbers = new Rectangle[10];
            for (int i = 0; i < 10; i++)
                numbers[i] = new Rectangle(200+i*8, 95, 8, 13);
        }

        public static void drawStageNr(SpriteBatch sb)
        {
            int x = (256 - stageNrRect.Width) / 2;
            int y = (232 - stageNrRect.Height) / 2;
            sb.Draw(Game1.spriteAtlas, new Rectangle(new Point(x, y), stageNrRect.Size), stageNrRect, Color.White);
            int worldNr = Game1.currentLevelNr / 8 + 1;
            int stageNr = Game1.currentLevelNr % 8 + 1;
            sb.Draw(Game1.spriteAtlas, new Rectangle(new Point(x + 76, y), numbers[worldNr].Size), numbers[worldNr], Color.White);
            sb.Draw(Game1.spriteAtlas, new Rectangle(new Point(x + 90, y), numbers[stageNr].Size), numbers[stageNr], Color.White);
        }
    }
}