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
    public class Hud
    {
        private int[] scoreArray, hiScoreArray;
        long timeLeft;
        int min, s1, s2;

        public Hud()
        {
            scoreArray = new int[9];
            hiScoreArray = new int[9];
        }

        public void update()
        {
            String scoreStr = Game1.score.ToString();
            int scoreLength = scoreStr.Length;
            for (int i=0; i<9;i++)
            {
                if (9 - i > scoreLength)
                    scoreArray[i] = -1;
                else
                    scoreArray[i] = (int)Char.GetNumericValue(scoreStr[i - (9 - scoreLength)]);
            }
            if (!Game1.character.teleporting)
                timeLeft = Level.timeLimit - (Game1.gameMiliseconds - Level.startTime) / 1000;
            if (timeLeft < 0)
            {
                Game1.character.die();
                timeLeft = 0;
            }
            min = (int)timeLeft / 60;
            s1 = (int)timeLeft % 60 / 10;
            s2 = (int)timeLeft % 10;
        }
   
        public void draw(SpriteBatch sb)
        {
            sb.Draw(Game1.spriteAtlas, new Rectangle(Game1.origin, new Point(256, 24)), new Rectangle(0, 148, 256, 24), Color.White);
            for (int i=0;i<9;i++)
            {
                if (scoreArray[i] != -1)
                    sb.Draw(Game1.spriteAtlas, new Rectangle(16 + 8 * i, 8,8,8), getDigitRect(scoreArray[i]), Color.White);
                if (hiScoreArray[i] != -1)
                    sb.Draw(Game1.spriteAtlas, new Rectangle(175 + 8 * i, 8, 8, 8), getDigitRect(hiScoreArray[i]), Color.White);
            }
            sb.Draw(Game1.spriteAtlas, new Rectangle(105, 8, 8, 8), getDigitRect(min), Color.White);
            sb.Draw(Game1.spriteAtlas, new Rectangle(120, 8, 8, 8), getDigitRect(s1), Color.White);
            sb.Draw(Game1.spriteAtlas, new Rectangle(128, 8, 8, 8), getDigitRect(s2), Color.White);
            sb.Draw(Game1.spriteAtlas, new Rectangle(152, 8, 8, 8), getDigitRect(Game1.character.lives), Color.White);
        }

        private Rectangle getDigitRect(int nr)
        {
            return new Rectangle(803 + 8 * nr, 912,8, 8);
        }

        public void fillHiScoreArray()
        {
            String hiScoreStr = Game1.hiScore.ToString();
            int scoreLength = hiScoreStr.Length;
            for (int i = 0; i < 9; i++)
            {
                if (9 - i > scoreLength)
                    hiScoreArray[i] = -1;
                else
                    hiScoreArray[i] = (int)Char.GetNumericValue(hiScoreStr[i - (9 - scoreLength)]);
            }
        }
    }
}