using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcadeGame
{
    class Background
    {
        private static List<double[]> dots_;
        private static Random rand_;

        public static void Initialize(int screenwidth, int screenheight)
        {
            if (!Drawer.Hd_)
                return;
            rand_ = new Random();
            dots_ = new List<double[]>();
            int n = screenwidth * screenheight / 1000;
            for (int i = 0; i < n; i++)
            {
                int depth = rand_.Next(16, 192);
                int x = rand_.Next(screenwidth);
                int y = rand_.Next(screenheight);
                double[] content = { depth, x, y };
                dots_.Add(content);
            }
        }

        public static void Update(int screenwidth, int screenheight)
        {
            if (!Drawer.Hd_)
                return;
            int count = dots_.Count();
            for (int i = 0; i < count; i++)
            {
                dots_[i][2] -= Character.Gravity_sign_ * dots_[i][0] / 128;
                if (Character.Gravity_sign_ > 0)
                {
                    if (dots_[i][2] < 0)
                    {
                        int depth = rand_.Next(16, 192);
                        int x = rand_.Next(0, screenwidth);
                        double[] new_content = { depth, x, screenheight };
                        dots_[i] = new_content;
                    }
                }
                else
                {
                    if (dots_[i][2] >= screenheight)
                    {
                        int depth = rand_.Next(16, 192);
                        int x = rand_.Next(0, screenwidth);
                        double[] new_content = { depth, x, 0 };
                        dots_[i] = new_content;
                    }
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (!Drawer.Hd_)
                return;
            foreach (double[] n in dots_)
                spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)n[1], (int)n[2], 3, 3), new Color(0, 0, 0, (int)n[0]));
        }
    }
}
