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

namespace DynaBlaster
{
    public class Animation
    {
        public int length;
        int frameTime, totalFrames, currentFrame;
        long lastFrameChangeTime;

        public Animation(int totalFrames, int frameTime=125)
        {
            this.totalFrames = totalFrames;
            currentFrame = 0;
            this.frameTime = frameTime;
            lastFrameChangeTime = 0;
            length = totalFrames * frameTime;
        }

        public int getCurrentFrame()
        {
            long timeDelta = Game1.gameMiliseconds-lastFrameChangeTime;
            if (timeDelta >= frameTime)
            {
                currentFrame = currentFrame + 1;
                if (currentFrame >= totalFrames)
                    currentFrame = 0;
                lastFrameChangeTime = Game1.gameMiliseconds;
            }
            return currentFrame;
        }

        public void reset()
        {
            lastFrameChangeTime = Game1.gameMiliseconds;
            currentFrame = 0;
        }
    }
}