using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcadeGame
{
    class MapChooser
    {
        private static Dictionary<int, Dictionary<int, WTuple<string, Delegate>>> menus_;
        private static Dictionary<int, string> titles_;
        private static bool active_;
        public static bool Active_
        {
            get { return MapChooser.active_; }
            set { MapChooser.active_ = value; }
        }
        private static int menu_ = 0;
        private static int state_ = 0;
        private static int offset_, rows_;
        private static int max_width_;
        private static int max_height_;
        private static float size_;
        private static string choosen_map_ = "";
        private static MapEditor map_;

        public static void Activate()
        {
            active_ = true;
        }

        public static void Initialize(int screenwidth, int screenheight)
        {
            choosen_map_ = Editor.Next_map_;
            menus_ = new Dictionary<int, Dictionary<int, WTuple<string, Delegate>>>();
            titles_ = new Dictionary<int, string>();
            Dictionary<int, WTuple<string, Delegate>> actions1 = new Dictionary<int, WTuple<string, Delegate>>();
            int i = 0;
            int rows = screenheight / 30 - 1;
            foreach (var entry in Maps.Maps_.OrderBy(entry => entry.Key))
            {
                if (entry.Key == choosen_map_)
                {
                    state_ = i;
                }
                actions1[i] = new WTuple<string, Delegate>(entry.Key, new Func<object>(ChooseMap));
                i++;
            }
            actions1[i] = new WTuple<string, Delegate>("Back", new Func<object>(ChooseBack));
            menus_[0] = actions1;
            titles_[0] = "CHOOSE THE NEXT MAP";
            max_height_ = screenheight - 20;
            max_width_ = screenwidth - (int)Textures.Font_.MeasureString(titles_[0]).X - 20;
            map_ = new MapEditor(actions1[state_].Item1_);
            ComputeSize();
            rows_ = screenheight / 30 - 1;
            SetState(state_);
        }

        private static object DoNothing()
        {
            return null;
        }

        private static object ChooseMap()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                choosen_map_ = menus_[menu_].Values.ElementAt(state_).Item1_;
                Editor.Next_map_ = choosen_map_;
                active_ = false;
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

        public static void ComputeSize()
        {
            int width = map_.Width_;
            int height = map_.Height_;
            if (max_width_ / width > max_height_ / height)
            {
                // Larger: height
                size_ = (float)max_height_ / (float)height;
            }
            else
            {
                // Larger: width
                size_ = (float)max_width_ / (float)width;
            }
            size_ = Math.Min(size_, 50);
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
                if (state_ != menus_[menu_].Count - 1)
                {
                    map_ = new MapEditor(menus_[menu_].Values.ToList()[state_].Item1_);
                    ComputeSize();
                }
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
                if (state_ != menus_[menu_].Count - 1)
                {
                    map_ = new MapEditor(menus_[menu_].Values.ToList()[state_].Item1_);
                    ComputeSize();
                }
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
                Vector2 text_size = Textures.Font_.MeasureString(item.Value.Item1_);
                spriteBatch.Draw(Textures.Pixel_, new Rectangle(state_ == item.Key ? 30 : 0, i * 30 + 30, (int)text_size.X, (int)text_size.Y), new Color(0, 0, 0, 0.75f));
                spriteBatch.DrawString(Textures.Font_, item.Value.Item1_, new Vector2(state_ == item.Key ? 30 : 0, i * 30 + 30), Color.White);
                i++;
            }
            if (map_ != null && state_ != menus_[menu_].Count - 1)
                map_.Draw(spriteBatch, new Vector2(Textures.Font_.MeasureString(titles_[0]).X + (max_width_ - map_.Width_ * size_) / 2, 10 + (max_height_ - map_.Height_ * size_) / 2), (int)size_);
        }
    }
}
