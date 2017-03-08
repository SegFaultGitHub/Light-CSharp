using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ArcadeGame
{
    public enum GamePlay
    { Editor, Play, Quit, None }

    class TitleScreen
    {
        private static bool active_;
        public static bool Active_
        {
            get { return TitleScreen.active_; }
        }
        private static Dictionary<int, Dictionary<int, WTuple<string, Delegate>>> menus_;
        private static Dictionary<int, string> titles_;
        private static int menu_;
        private static int state_;
        private static int menu_save_;
        private static int state_save_;
        private static int offset_, rows_;
        private static GamePlay gameplay_;
        public static GamePlay Gameplay_
        {
            get { return TitleScreen.gameplay_; }
            set { TitleScreen.gameplay_ = value; }
        }
        private static int width_ = 20;
        private static int height_ = 15;
        private static string choosen_map_ = "";
        private static MapEditor map_;
        private static int max_width_;
        private static int max_height_;
        private static float size_;

        public static void Activate()
        {
            menu_ = 0;
            state_ = 0;
            gameplay_ = GamePlay.None;
            active_ = true;
        }

        public static void SaveState()
        {
            menu_save_ = menu_;
            state_save_ = state_;
        }

        public static void RestoreState()
        {
            menu_ = menu_save_;
            state_ = state_save_;
        }

        public static void InitializeMaps()
        {
            Dictionary<int, WTuple<String, Delegate>> actions4 = new Dictionary<int, WTuple<string, Delegate>>();
            int i = 0;
            if (Maps.Maps_.Where(map => map.Value.Tutorial_).Count() != 0)
                actions4[i++] = new WTuple<string, Delegate>("Tutorial", new Func<object>(ChooseTutorial));
            if (Maps.Maps_.Where(map => map.Value.Official_ && !map.Value.Tutorial_).Count() != 0)
                actions4[i++] = new WTuple<string, Delegate>("Official maps", new Func<object>(ChooseOfficialMaps));
            if (Maps.Maps_.Where(map => !map.Value.Official_).Count() != 0)
                actions4[i++] = new WTuple<string, Delegate>("Custom maps", new Func<object>(ChooseCustomMaps));
            actions4[i] = new WTuple<string, Delegate>("Back", new Func<object>(ChooseBack));
            menus_[3] = actions4;
            titles_[3] = "PLAY";
        }

        public static void Initialize()
        {
            int screenwidth = Drawer.Graphics_.PreferredBackBufferWidth;
            int screenheight = Drawer.Graphics_.PreferredBackBufferHeight;
            //gameplay_ = GamePlay.None;
            menus_ = new Dictionary<int, Dictionary<int, WTuple<string, Delegate>>>();
            titles_ = new Dictionary<int, string>();
            Dictionary<int, WTuple<string, Delegate>> actions1 = new Dictionary<int, WTuple<string, Delegate>>();
            actions1[0] = new WTuple<string, Delegate>("Editor", new Func<object>(ChooseEditor));
            actions1[1] = new WTuple<string, Delegate>("Play", new Func<object>(ChoosePlay));
            actions1[2] = new WTuple<string, Delegate>("Options", new Func<object>(ChooseOptions));
            actions1[3] = new WTuple<string, Delegate>("Quit", new Func<object>(ChooseQuit));
            menus_[0] = actions1;
            titles_[0] = "TITLE SCREEN";

            Dictionary<int, WTuple<String, Delegate>> actions2 = new Dictionary<int, WTuple<string, Delegate>>();
            actions2[0] = new WTuple<string, Delegate>("Quality: " + (Drawer.Hd_ ? "HIGH" : "LOW"), new Func<object>(ChooseQuality));
            actions2[1] = new WTuple<string, Delegate>("Full screen: OPTION DISABLED", new Func<object>(DoNothing));
            //actions2[1] = new WTuple<string, Delegate>("Full screen: " + (Drawer.Full_screen_ ? "ON" : "OFF"), new Func<object>(ChooseFullScreen));
            actions2[2] = new WTuple<string, Delegate>("Sounds: " + (GameHandler.Sounds_ ? "ON" : "OFF"), new Func<object>(ChooseSounds));
            actions2[3] = new WTuple<string, Delegate>("Delete best scores", new Func<object>(ChooseDeleteScores));
            //actions2[3] = new WTuple<string, Delegate>("Delete best scores (OPTION DISABLED)", new Func<object>(DoNothing));
            actions2[4] = new WTuple<string, Delegate>("Back", new Func<object>(ChooseBack));
            menus_[1] = actions2;
            titles_[1] = "OPTIONS";
            
            Dictionary<int, WTuple<String, Delegate>> actions3 = new Dictionary<int, WTuple<string, Delegate>>();
            actions3[0] = new WTuple<string, Delegate>("Width: " + width_, new Func<object>(ChooseWidth));
            actions3[1] = new WTuple<string, Delegate>("Height: " + height_, new Func<object>(ChooseHeight));
            actions3[2] = new WTuple<string, Delegate>("Done", new Func<object>(ChooseDone));
            actions3[3] = new WTuple<string, Delegate>("Back", new Func<object>(ChooseBack));
            menus_[2] = actions3;
            titles_[2] = "CREATE A NEW MAP";

            Dictionary<int, WTuple<String, Delegate>> actions4 = new Dictionary<int, WTuple<string, Delegate>>();
            int i = 0;
            if (Maps.Maps_.Where(map => map.Value.Tutorial_).Count() != 0)
                actions4[i++] = new WTuple<string, Delegate>("Tutorial", new Func<object>(ChooseTutorial));
            if (Maps.Maps_.Where(map => map.Value.Official_ && !map.Value.Tutorial_).Count() != 0)
                actions4[i++] = new WTuple<string, Delegate>("Official maps", new Func<object>(ChooseOfficialMaps));
            if (Maps.Maps_.Where(map => !map.Value.Official_).Count() != 0)
                actions4[i++] = new WTuple<string, Delegate>("Custom maps", new Func<object>(ChooseCustomMaps));
            actions4[i] = new WTuple<string, Delegate>("Back", new Func<object>(ChooseBack));
            menus_[3] = actions4;
            titles_[3] = "PLAY";

            Dictionary<int, WTuple<String, Delegate>> actions8 = new Dictionary<int, WTuple<string, Delegate>>();
            i = 0;
            foreach (var entry in Maps.GetNamesAndScores().OrderBy(entry => entry.Key).Where(entry => entry.Value.Item2))
            {
                actions8[i] = new WTuple<string, Delegate>(entry.Key + entry.Value.Item1, new Func<object>(ChooseMap));
                i++;
            }
            actions8[i] = new WTuple<string, Delegate>("Back", new Func<object>(ChooseBack));
            menus_[7] = actions8;
            titles_[7] = "TUTORIAL - CHOOSE A MAP"; /* Tutorial */

            Dictionary<int, WTuple<String, Delegate>> actions9 = new Dictionary<int, WTuple<string, Delegate>>();
            i = 0;
            foreach (var entry in Maps.GetNamesAndScores().OrderBy(entry => entry.Key).Where(entry => entry.Value.Item3 && !entry.Value.Item2))
            {
                actions9[i] = new WTuple<string, Delegate>(entry.Key + entry.Value.Item1, new Func<object>(ChooseMap));
                i++;
            }
            actions9[i] = new WTuple<string, Delegate>("Back", new Func<object>(ChooseBack));
            menus_[8] = actions9;
            titles_[8] = "OFFICIAL MAPS - CHOOSE A MAP"; /* Official maps */

            Dictionary<int, WTuple<String, Delegate>> actions10 = new Dictionary<int, WTuple<string, Delegate>>();
            i = 0;
            foreach (var entry in Maps.GetNamesAndScores().OrderBy(entry => entry.Key).Where(entry => !entry.Value.Item3))
            {
                actions10[i] = new WTuple<string, Delegate>(entry.Key + entry.Value.Item1, new Func<object>(ChooseMap));
                i++;
            }
            actions10[i] = new WTuple<string, Delegate>("Back", new Func<object>(ChooseBack));
            menus_[9] = actions10;
            titles_[9] = "CUSTOM MAPS - CHOOSE A MAP"; /* Custom maps */
            
            Dictionary<int, WTuple<String, Delegate>> actions5 = new Dictionary<int, WTuple<string, Delegate>>();
            i = 0;
            foreach (var entry in Maps.Maps_.OrderBy(entry => entry.Key))
            {
                if (entry.Value.Read_only_)
                    continue;
                actions5[i] = new WTuple<string, Delegate>(entry.Key, new Func<object>(ChooseMapEdit));
                i++;
            }
            actions5[i] = new WTuple<string, Delegate>("Back", new Func<object>(ChooseBack));
            menus_[4] = actions5;
            titles_[4] = "EDIT A EXISTING MAP";
            max_height_ = screenheight - 20;
            max_width_ = screenwidth - (int)Textures.Font_.MeasureString(titles_[4]).X - 20;
            map_ = new MapEditor(actions5[0].Item1_);
            ComputeSize();

            Dictionary<int, WTuple<String, Delegate>> actions6 = new Dictionary<int, WTuple<string, Delegate>>();
            actions6[0] = new WTuple<string, Delegate>("Create a new map", new Func<object>(ChooseNewMap));
            actions6[1] = new WTuple<string, Delegate>("Edit a existing map", new Func<object>(ChooseExistingMap));
            actions6[2] = new WTuple<string, Delegate>("Back", new Func<object>(ChooseBack));
            menus_[5] = actions6;
            titles_[5] = "EDITOR";

            Dictionary<int, WTuple<String, Delegate>> actions7 = new Dictionary<int, WTuple<string, Delegate>>();
            actions7[0] = new WTuple<string, Delegate>("Yes", new Func<object>(ChooseDeleteYes));
            actions7[1] = new WTuple<string, Delegate>("No", new Func<object>(ChooseBack));
            actions7[2] = new WTuple<string, Delegate>("Back", new Func<object>(ChooseBack));
            menus_[6] = actions7;
            titles_[6] = "DELETE BEST SCORES ?";

            /*state_ = 0;
            menu_ = 0;*/
        }

        private static object DoNothing()
        {
            return null;
        }

        private static object ChooseDeleteScores()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                menu_ = 6;
                state_ = 0;
                offset_ = 0;
            }
            return null;
        }

        private static object ChooseTutorial()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                menu_ = 7;
                state_ = 0;
                offset_ = 0;
            }
            return null;
        }

        private static object ChooseOfficialMaps()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                menu_ = 8;
                state_ = 0;
                offset_ = 0;
            }
            return null;
        }

        private static object ChooseCustomMaps()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                menu_ = 9;
                state_ = 0;
                offset_ = 0;
            }
            return null;
        }

        private static object ChooseContinue()
        {
            return null;
        }

        private static object ChooseDeleteYes()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                XmlDocument xmlDocument = new XmlDocument();
                try
                {
                    xmlDocument.Load("../../../../LightContent/database/scores.xml");
                }
                catch
                {
                    Console.WriteLine("Unable to save: cannot load XML.");
                    Back();
                    return null;
                }
                XmlNode root = xmlDocument.DocumentElement;
                root.RemoveAll();
                try
                {
                    SaveState();
                    xmlDocument.Save("../../../../LightContent/database/scores.xml");
                    Console.WriteLine("Scores deleted.");
                    Initialize();
                    RestoreState();
                }
                catch
                {
                    Console.WriteLine("Unable to save: cannot write.");
                }
                Back();
            }
            return null;
        }

        private static object ChooseFullScreen()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                Drawer.Full_screen_ = !Drawer.Full_screen_;
                if (Drawer.Full_screen_)
                {
                    Drawer.EnableFullScreen();
                    menus_[menu_].Values.ElementAt(state_).Item1_ = "Full screen: ON";
                }
                else
                {
                    Drawer.DisableFullScreen();
                    menus_[menu_].Values.ElementAt(state_).Item1_ = "Full screen: OFF";
                }
            }
            return null;
        }

        private static object ChooseSounds()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                GameHandler.Sounds_ = !GameHandler.Sounds_;
                if (GameHandler.Sounds_)
                {
                    menus_[menu_].Values.ElementAt(state_).Item1_ = "Sounds: ON";
                }
                else
                {
                    menus_[menu_].Values.ElementAt(state_).Item1_ = "Sounds: OFF";
                }
            }
            return null;
        }

        private static object ChooseExistingMap()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                menu_ = 4;
                state_ = 0;
                offset_ = 0;
            }
            return null;
        }

        private static object ChooseMap()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                active_ = false;
                gameplay_ = GamePlay.Play;
                choosen_map_ = menus_[menu_].Values.ElementAt(state_).Item1_;
                if (choosen_map_.Contains(" — "))
                    choosen_map_ = choosen_map_.Substring(0, choosen_map_.IndexOf(" — "));
                GameHandler.SetCharacter(new Character());
                GameHandler.SetMap(choosen_map_);
            }
            return null;
        }

        private static object ChooseMapEdit()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                active_ = false;
                gameplay_ = GamePlay.Play;
                choosen_map_ = menus_[menu_].Values.ElementAt(state_).Item1_;
                Editor.Initialize(choosen_map_);
                gameplay_ = GamePlay.Editor;
            }
            return null;
        }

        private static object ChooseNewMap()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                menu_ = 2;
                state_ = 0;
                offset_ = 0;
            }
            return null;
        }

        private static object ChooseEditor()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                menu_ = 5;
                state_ = 0;
                offset_ = 0;
            }
            return null;
        }

        private static object ChoosePlay()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                menu_ = 3;
                state_ = 0;
                offset_ = 0;
            }
            return null;
        }

        private static object ChooseQuit()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                active_ = false;
                gameplay_ = GamePlay.Quit;
            }
            return null;
        }

        private static object ChooseOptions()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                menu_ = 1;
                state_ = 0;
                offset_ = 0;
            }
            return null;
        }

        private static object ChooseBack()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                Back();
            }
            return null;
        }

        private static void Back()
        {
            switch (menu_)
            {
                case 1:
                case 3:
                case 5:
                    menu_ = 0; /* Main screen */
                    break;
                case 2:
                case 4:
                    menu_ = 5; /* Editor */
                    break;
                case 6:
                    menu_ = 1; /* Options */
                    break;
                case 7:
                case 8:
                case 9:
                    menu_ = 3;
                    break;
                default:
                    break;
            }
            state_ = 0;
            offset_ = 0;
        }

        private static object ChooseQuality()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                Drawer.Hd_ = !Drawer.Hd_;
                if (Drawer.Hd_)
                {
                    Map.EnableHD();
                    menus_[menu_].Values.ElementAt(state_).Item1_ = "Quality: HIGH";
                }
                else
                {
                    Map.DisableHD();
                    menus_[menu_].Values.ElementAt(state_).Item1_ = "Quality: LOW";
                }
            }
            return null;
        }

        private static object ChooseWidth()
        {
            if (Input.IsPressedOnce(Keys.Right) || (Input.IsPressed(Keys.Right) && (Input.IsPressed(Keys.LeftShift) || Input.IsPressed(Keys.RightShift))))
            {
                width_++;
                menus_[menu_].Values.ElementAt(state_).Item1_ = "Width: " + width_;
            }
            else if (Input.IsPressedOnce(Keys.Left) || (Input.IsPressed(Keys.Left) && (Input.IsPressed(Keys.LeftShift) || Input.IsPressed(Keys.RightShift))))
            {
                if (width_ != 1)
                {
                    width_--;
                    menus_[menu_].Values.ElementAt(state_).Item1_ = "Width: " + width_;
                }
            }
            return null;
        }

        private static object ChooseHeight()
        {
            if (Input.IsPressedOnce(Keys.Right) || (Input.IsPressed(Keys.Right) && (Input.IsPressed(Keys.LeftShift) || Input.IsPressed(Keys.RightShift))))
            {
                height_++;
                menus_[menu_].Values.ElementAt(state_).Item1_ = "Height: " + height_;
            }
            else if (Input.IsPressedOnce(Keys.Left) || (Input.IsPressed(Keys.Left) && (Input.IsPressed(Keys.LeftShift) || Input.IsPressed(Keys.RightShift))))
            {
                if (height_ != 1)
                {
                    height_--;
                    menus_[menu_].Values.ElementAt(state_).Item1_ = "Height: " + height_;
                }
            }
            return null;
        }

        private static object ChooseDone()
        {
            if (Input.IsPressedOnce(Keys.Enter))
            {
                active_ = false;
                Editor.Initialize(width_, height_);
                gameplay_ = GamePlay.Editor;
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
            max_width_ = screenwidth - (int)Textures.Font_.MeasureString(titles_[4]).X - 20;
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
                if (menu_ == 4 && state_ != menus_[menu_].Count - 1)
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
                if (menu_ == 4 && state_ != menus_[menu_].Count - 1)
                {
                    map_ = new MapEditor(menus_[menu_].Values.ToList()[state_].Item1_);
                    ComputeSize();
                }
            }
            else if (Input.IsPressedOnce(Keys.Escape))
            {
                Back();
            }
            menus_[menu_].Values.ToList()[state_].Item2_.DynamicInvoke();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (menu_ == 4 && map_ != null && state_ != menus_[menu_].Count - 1)
            {
                spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)Textures.Font_.MeasureString(titles_[menu_]).X + (max_width_ - map_.Width_ * (int)size_) / 2 - 1, 10 + (max_height_ - map_.Height_ * (int)size_) / 2 - 1, map_.Width_ * (int)size_ + 2, map_.Height_ * (int)size_ + 2), Color.Gray);
                map_.Draw(spriteBatch, new Vector2(Textures.Font_.MeasureString(titles_[menu_]).X + (max_width_ - map_.Width_ * (int)size_) / 2, 10 + (max_height_ - map_.Height_ * (int)size_) / 2), (int)size_);
            }
            spriteBatch.DrawString(Textures.Font_, titles_[menu_], Vector2.Zero + new Vector2(0, 0), Color.Gray);
            int i = 0;
            foreach (var item in menus_[menu_].Skip(offset_).Take(rows_))
            {
                Vector2 text_size = Textures.Font_.MeasureString(item.Value.Item1_);
                spriteBatch.Draw(Textures.Pixel_, new Rectangle(state_ == item.Key ? 30 : 0, i * 30 + 30, (int)text_size.X, (int)text_size.Y), new Color(0, 0, 0, 0.75f));
                spriteBatch.DrawString(Textures.Font_, item.Value.Item1_, new Vector2(state_ == item.Key ? 30 : 0, i * 30 + 30), Color.White);
                i++;
            }
            Vector2 size = Textures.Font_.MeasureString(GameHandler.Version_);
            Vector2 size2 = Textures.Font_.MeasureString("V" + GameHandler.Version_);
            spriteBatch.DrawString(Textures.Font_, "V", new Vector2(Drawer.Graphics_.PreferredBackBufferWidth, Drawer.Graphics_.PreferredBackBufferHeight) - size2, Color.White);
            spriteBatch.DrawString(Textures.Font_, GameHandler.Version_, new Vector2(Drawer.Graphics_.PreferredBackBufferWidth, Drawer.Graphics_.PreferredBackBufferHeight) - size, Color.Gray);
        }
    }
}
