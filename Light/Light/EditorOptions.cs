using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcadeGame
{
    class EditorOptions
    {
        private static Dictionary<int, Dictionary<int, WTuple<string, Delegate>>> menus_;
        private static Dictionary<int, string> titles_;
        private static bool active_;
        public static bool Active_
        {
            get { return EditorOptions.active_; }
            set { EditorOptions.active_ = value; }
        }
        private static int menu_ = 0;
        private static int state_ = 0;
        private static int offset_, rows_;
        private static int max_width_;
        private static int max_height_;
        private static float size_;

        public static void Activate()
        {
            active_ = true;
            menu_ = 0;
            state_ = 0;
            offset_ = 0;
        }

        public static void Initialize(int screenwidth, int screenheight)
        {
            menus_ = new Dictionary<int, Dictionary<int, WTuple<string, Delegate>>>();
            titles_ = new Dictionary<int, string>();
            Dictionary<int, WTuple<string, Delegate>> actions1 = new Dictionary<int, WTuple<string, Delegate>>();
            int rows = screenheight / 30 - 1;
            actions1[0] = new WTuple<string, Delegate>("Tutorial map: " + (Editor.Tutorial_ ? "Yes" : "No"), new Func<object>(ChooseTutorial));
            actions1[1] = new WTuple<string, Delegate>("Custom map: " + (Editor.Custom_ ? "Yes" : "No"), new Func<object>(ChooseCustom));
            actions1[2] = new WTuple<string, Delegate>("Back", new Func<object>(ChooseBack));
            menus_[0] = actions1;
            titles_[0] = "OPTIONS";
            max_height_ = screenheight - 20;
            max_width_ = screenwidth - (int)Textures.Font_.MeasureString(titles_[0]).X - 20;
            rows_ = screenheight / 30 - 1;
            SetState(state_);
        }

        private static object DoNothing()
        {
            return null;
        }

        private static object ChooseTutorial()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                Editor.Tutorial_ = !Editor.Tutorial_;
                menus_[menu_].Values.ElementAt(state_).Item1_ = "Tutorial map: " + (Editor.Tutorial_ ? "Yes" : "No");
            }
            return null;
        }

        private static object ChooseCustom()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                Editor.Custom_ = !Editor.Custom_;
                menus_[menu_].Values.ElementAt(state_).Item1_ = "Custom map: " + (Editor.Custom_ ? "Yes" : "No");
            }
            return null;
        }

        private static object ChooseBack()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                active_ = false;
            }
            return null;
        }

        public static void SetState(int new_state)
        {
            state_ = new_state;
            offset_ = state_ - rows_ / 2;
            offset_ = Math.Min(Math.Max(0, offset_), menus_[menu_].Count - rows_);
        }

        public static void Update(int screenwidth, int screenheight)
        {
            max_height_ = screenheight - 20;
            max_width_ = screenwidth - (int)Textures.Font_.MeasureString(titles_[0]).X - 20;
            rows_ = screenheight / 30 - 1;
            if (Input.IsPressedOnce(Keys.Down))
            {
                state_++;
                if (state_ >= offset_ + (rows_ - 2))
                    offset_++;
                if (state_ < 0)
                {
                    state_ = menus_[menu_].Count - 1;
                    offset_ = menus_[menu_].Count - rows_;
                }
                else if (state_ >= menus_[menu_].Count)
                {
                    state_ = 0;
                    offset_ = 0;
                }
                offset_ = Math.Min(Math.Max(0, offset_), menus_[menu_].Count - rows_);
            }
            else if (Input.IsPressedOnce(Keys.Up))
            {
                state_--;
                if (state_ <= offset_ + 1)
                    offset_--;
                if (state_ < 0)
                {
                    state_ = menus_[menu_].Count - 1;
                    offset_ = menus_[menu_].Count - rows_;
                }
                else if (state_ >= menus_[menu_].Count)
                {
                    state_ = 0;
                    offset_ = 0;
                }
                offset_ = Math.Min(Math.Max(0, offset_), menus_[menu_].Count - rows_);
            }
            else if (Input.IsPressedOnce(Keys.Escape))
            {
                active_ = false;
            }
            menus_[menu_].Values.ToList()[state_].Item2_.DynamicInvoke();
        }

        public static void Draw(SpriteBatch spriteBatch, int screenwidth, int screenheight)
        {
            spriteBatch.Draw(Textures.Pixel_, new Rectangle(0, 0, screenwidth, screenheight), new Color(0, 0, 0, 0.75f));
            spriteBatch.DrawString(Textures.Font_, titles_[menu_], Vector2.Zero + new Vector2(0, 0), Color.Gray);
            int i = 0;
            foreach (var item in menus_[menu_].Skip(offset_).Take(rows_))
            {
                spriteBatch.DrawString(Textures.Font_, item.Value.Item1_, new Vector2(state_ == item.Key ? 30 : 0, i * 30 + 30), Color.White);
                i++;
            }
        }
    }
}
