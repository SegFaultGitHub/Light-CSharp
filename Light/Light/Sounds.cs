using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcadeGame
{
    static class Sounds
    {
        private static SoundEffect splash_;
        private static SoundEffect gravity_pad_;
        private static SoundEffect jump_pad_;

        public static void LoadContent(ContentManager Content)
        {
            splash_ = Content.Load<SoundEffect>("splash");
            gravity_pad_ = Content.Load<SoundEffect>("gravity_pad");
            jump_pad_ = Content.Load<SoundEffect>("jump_pad");
        }

        public static void PlaySplash()
        {
            if (GameHandler.Sounds_)
                splash_.Play(0.1f, 0, 0);
        }

        public static void PlayGravityPad()
        {
            if (GameHandler.Sounds_)
                gravity_pad_.Play(0.02f, 0, 0);
        }

        public static void PlayJumpPad()
        {
            if (GameHandler.Sounds_)
                jump_pad_.Play(0.02f, 0, 0);
        }
    }
}
