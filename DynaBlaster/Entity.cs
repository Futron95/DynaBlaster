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

namespace DynaBlaster
{
    public abstract class Entity
    {
        protected enum direction { LEFT, UP, RIGHT, DOWN };
        protected direction dir;

        public Animation walk, dyingAnimation;
        public Rectangle sourceRectangle;
        protected Rectangle locationRectangle
        {
            get { return new Rectangle((int)x+ Game1.dynOrigin.X, (int)y+ Game1.dynOrigin.Y, width, height); }
        }
        public Boolean dead, visible;
        protected int width, height;
        protected double speed;
        protected Random r;

        public double x;
        public double y;  

        public long deathTime;      
    }
}