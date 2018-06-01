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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace DynaBlaster
{
    public class Sounds
    {
        public SoundEffectInstance[] music;
        public Song stageStart, death, teleport;
        public SoundEffectInstance explosion;

        public Sounds(ContentManager content)
        {
            music = new SoundEffectInstance[3];
            music[0] = content.Load<SoundEffect>("music1").CreateInstance();
            music[1] = content.Load<SoundEffect>("music2").CreateInstance();
            music[2] = content.Load<SoundEffect>("music3").CreateInstance();
            music[0].IsLooped = true;
            music[1].IsLooped = true;
            music[2].IsLooped = true;
            explosion = content.Load<SoundEffect>("explosion").CreateInstance();
            stageStart = content.Load<Song>("stageStart");
            death = content.Load<Song>("death");
            teleport = content.Load<Song>("teleport");       
        }
    }    
}