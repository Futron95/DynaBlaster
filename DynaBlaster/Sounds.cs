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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace DynaBlaster
{
    public class Sounds
    {
        public Song[] music;
        public Song stageStart, death, teleport;
        public SoundEffect explosion;

        public Sounds()
        {
            music = new Song[3];
        }
    }    
}