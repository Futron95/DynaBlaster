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
using Microsoft.Xna.Framework.Input.Touch;

namespace DynaBlaster
{
    public class Controls
    {
        private Texture2D arrowsTexture, bombTexture;
        private Rectangle DirectionsRect, bombRect;
        public Rectangle directionsRect
        {
            get { return DirectionsRect; }
            set
            {
                DirectionsRect = value;
                setDirectionRectangles();
            }
        }
        private Rectangle[] controlRectangles;


        public Controls(Texture2D arrowsTexture, Rectangle directionsRect, Texture2D bombTexture, Rectangle bombRect)
        {
            this.arrowsTexture = arrowsTexture;
            this.bombTexture = bombTexture;
            controlRectangles = new Rectangle[5];
            this.directionsRect = directionsRect;
            this.bombRect = bombRect;            
            controlRectangles[4] = bombRect;
        }

        private void setDirectionRectangles()
        {
            controlRectangles[0] = new Rectangle(DirectionsRect.X + DirectionsRect.Width / 4, DirectionsRect.Y, DirectionsRect.Width / 2, DirectionsRect.Height / 3);
            controlRectangles[1] = new Rectangle(DirectionsRect.X + DirectionsRect.Width / 4, DirectionsRect.Y+DirectionsRect.Height*2/3, DirectionsRect.Width / 2, DirectionsRect.Height / 3);
            controlRectangles[2] = new Rectangle(DirectionsRect.X, DirectionsRect.Y + DirectionsRect.Height/3, DirectionsRect.Width / 2, DirectionsRect.Height / 3);
            controlRectangles[3] = new Rectangle(DirectionsRect.X + DirectionsRect.Width/2, DirectionsRect.Y + DirectionsRect.Height / 3, DirectionsRect.Width / 2, DirectionsRect.Height / 3);
        }

        public int getTouchedDirection(TouchLocation tl)
        {
            for (int i=0;i<controlRectangles.Length;i++)
                if (controlRectangles[i].Contains(tl.Position)) return i;
            return -1;
        }

        public void draw(SpriteBatch sb)
        {
            sb.Draw(arrowsTexture, DirectionsRect, Color.White);
            sb.Draw(bombTexture, bombRect, Color.White);
        }
    }
}