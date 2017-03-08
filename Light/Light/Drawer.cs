using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ArcadeGame
{
    class Drawer
    {
        private static Random rand_;
        private static bool hd_ = true;
        public static bool Hd_
        {
            get { return Drawer.hd_; }
            set { Drawer.hd_ = value; }
        }
        private static bool full_screen_;
        public static bool Full_screen_
        {
            get { return Drawer.full_screen_; }
            set { Drawer.full_screen_ = value; }
        }
        private static float shade_;
        private static GraphicsDeviceManager graphics_;
        public static GraphicsDeviceManager Graphics_
        {
            get { return Drawer.graphics_; }
        }
        private static bool fading_;

        public static void Initialize(GraphicsDeviceManager graphics)
        {
            rand_ = new Random();
            fading_ = false;
            graphics_ = graphics;
        }

        public static void EnableFullScreen()
        {
            full_screen_ = true;
            graphics_.IsFullScreen = true;
            graphics_.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics_.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics_.ApplyChanges();
            Background.Initialize(graphics_.PreferredBackBufferWidth, graphics_.PreferredBackBufferHeight);
        }

        public static void DisableFullScreen()
        {
            full_screen_ = false;
            graphics_.IsFullScreen = false;
            graphics_.PreferredBackBufferWidth = 1280;//GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2;
            graphics_.PreferredBackBufferHeight = 720;//GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;
            graphics_.ApplyChanges();
            Background.Initialize(graphics_.PreferredBackBufferWidth, graphics_.PreferredBackBufferHeight);
        }

        public static void Fade(int ms)
        {
            if (fading_)
                return;
            fading_ = true;
            Thread thread = new Thread(() =>
            {
                shade_ = 0f;
                DateTime start = DateTime.Now;
                while (DateTime.Now - start < new TimeSpan(0, 0, 0, 0, ms))
                {
                    shade_ = (float)(DateTime.Now - start).TotalMilliseconds / (float)ms;
                }
                start = DateTime.Now;
                while (DateTime.Now - start < new TimeSpan(0, 0, 0, 0, ms))
                {
                    shade_ = 1f - ((float)(DateTime.Now - start).TotalMilliseconds / (float)ms);
                }
                shade_ = 0f;
                fading_ = false;
            });
            thread.Start();
        }

        public static void Draw(SpriteBatch spriteBatch, Map map, Character character, Character dash_character, TimeSpan timer, int screenwidth, int screenheight)
        {
            Background.Draw(spriteBatch);
            for (int j = map.Shift_y_ / Map.Size_; j <= map.Shift_y_ / Map.Size_ + screenheight / Map.Size_ + 1 && j < map.Height_; j++)
            {
                for (int i = map.Shift_x_ / Map.Size_; i <= map.Shift_x_ / Map.Size_ + screenwidth / Map.Size_ + 1 && i < map.Width_; i++)
                {
                    map.Map_[i, j].Draw(spriteBatch, i * Map.Size_ - (map.Shift_x_ + map.F_shift_x_), j * Map.Size_ - (map.Shift_y_ + map.F_shift_y_), 0, 0);
                }
            }
            for (int j = 0; j < map.Height_; j++)
            {
                for (int i = 0; i < map.Width_; i++)
                {
                    if (map.Map_[i, j].Content_[0] == 16)
                    {
                        int x = i * Map.Size_ - (map.Shift_x_ + map.F_shift_x_);
                        int y = j * Map.Size_ - (map.Shift_y_ + map.F_shift_y_);
                        int length = (int)map.Map_[i, j].Var_[0, 5];
                        double angle = -map.Map_[i, j].Var_[0, 1] / 180f * Math.PI;
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x + Map.Size_ / 2, y + Map.Size_ / 2, length, 1),
                            null, new Color(1f, 0f, 0f, 0.7f), (float)angle, new Vector2(0, 0), SpriteEffects.None, 0f);
                        spriteBatch.Draw(Textures.Laser_, new Rectangle(x + Map.Size_ / 2, y + Map.Size_ / 2, Map.Size_, Map.Size_),
                            null, Color.White, (float)angle, new Vector2(Textures.Laser_.Width / 2, Textures.Laser_.Height / 2), SpriteEffects.None, 0f);
                    }
                }
            }
            Particles.Draw(spriteBatch, map);
            if (dash_character != null)
                dash_character.Draw(spriteBatch);
            character.Draw(spriteBatch);
            for (int j = map.Shift_y_ / Map.Size_; j <= map.Shift_y_ / Map.Size_ + screenheight / Map.Size_ + 1 && j < map.Height_; j++)
            {
                for (int i = map.Shift_x_ / Map.Size_; i <= map.Shift_x_ / Map.Size_ + screenwidth / Map.Size_ + 1 && i < map.Width_; i++)
                {
                    map.Map_[i, j].Draw(spriteBatch, i * Map.Size_ - (map.Shift_x_ + map.F_shift_x_), j * Map.Size_ - (map.Shift_y_ + map.F_shift_y_), 1);
                }
            }
            for (int j = Math.Max(0, map.Shift_y_ / Map.Size_ - (screenheight / 2) / Map.Size_); j <= map.Shift_y_ / Map.Size_ + screenheight / Map.Size_ + 1 + (screenheight / 2) / Map.Size_ && j < map.Height_; j++)
                //int j = map.Shift_y_ / Map.Size_; j <= map.Shift_y_ / Map.Size_ + screenheight / Map.Size_ + 1 && j < map.Height_; j++)
            {
                for (int i = Math.Max(0, map.Shift_x_ / Map.Size_ - (screenwidth / 2) / Map.Size_); i <= map.Shift_x_ / Map.Size_ + screenwidth / Map.Size_ + 1 + (screenwidth / 2) / Map.Size_ && i < map.Width_; i++)
                    //(int i = map.Shift_x_ / Map.Size_; i <= map.Shift_x_ / Map.Size_ + screenwidth / Map.Size_ + 1 && i < map.Width_; i++)
                {
                    float shadow = (float)map.Shadows_smooth_[i, j] == 0 || (float)map.Shadows_smooth_[i, j]  == 1 ? (float)map.Shadows_smooth_[i, j] : (float)map.Shadows_smooth_[i, j] + (rand_.Next(-1, 1) / 75f);
                    spriteBatch.Draw(Textures.Pixel_, new Rectangle(i * Map.Size_ - (map.Shift_x_ + map.F_shift_x_), j * Map.Size_ - (map.Shift_y_ + map.F_shift_y_), Map.Size_, Map.Size_), new Color(0, 0, 0, 1f - shadow));
                }
            }
            spriteBatch.Draw(Textures.Pixel_, new Rectangle(0, 0, -map.F_shift_x_, screenheight), Color.Black);
            spriteBatch.Draw(Textures.Pixel_, new Rectangle(0, 0, screenwidth, -map.F_shift_y_), Color.Black);
            spriteBatch.Draw(Textures.Pixel_, new Rectangle(-map.F_shift_x_ + Map.Size_ * map.Width_, 0, -map.F_shift_x_ + 1, screenheight), Color.Black);
            spriteBatch.Draw(Textures.Pixel_, new Rectangle(0, -map.F_shift_y_ + Map.Size_ * map.Height_, screenwidth, -map.F_shift_y_ + 1), Color.Black);
            spriteBatch.Draw(Textures.Pixel_, new Rectangle(0, 0, screenwidth, screenheight), new Color(0f, 0f, 0f, shade_));
            spriteBatch.DrawString(Textures.Font_, timer.ToString().Substring(3, Math.Min(8, timer.ToString().Length - 4)), new Vector2(5), Color.White);
            //string speed = Math.Round(character.Speed_length_).ToString();
            //Vector2 size = Textures.Font_.MeasureString(speed);
            //spriteBatch.DrawString(Textures.Font_, speed, new Vector2(graphics_.PreferredBackBufferWidth - 2, graphics_.PreferredBackBufferHeight - 2) - size, Color.White);
            if (Message.Display_)
                Message.Draw(spriteBatch);
        }
    }
}
