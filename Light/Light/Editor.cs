using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace ArcadeGame
{
    class Editor
    {
        private static bool tutorial_ = false;
        public static bool Tutorial_
        {
            get { return Editor.tutorial_; }
            set { Editor.tutorial_ = value; }
        }
        private static bool custom_ = true;
        public static bool Custom_
        {
            get { return Editor.custom_; }
            set { Editor.custom_ = value; }
        }
        private static MapEditor map_;
        private static MouseState mouseState_, oldMouseState_;
        private static int width_, height_;
        private static int content_ = 0;
        private static int layer_ = 0;
        private static bool side_ = true;
        private static List<WTuple<int, string>> contents_;
        private static string name_;
        private static bool thread_part1_, thread_part2_;
        private static string thread_part1_str_, thread_part2_str_;
        private static float shade_;
        private static string next_map_;
        public static string Next_map_
        {
            get { return Editor.next_map_; }
            set { Editor.next_map_ = value; }
        }
        private static int max_width_, x_;
        private static Thread move_panel_;

        public static void Initialize(int width, int height)
        {
            InitContents();
            map_ = new MapEditor(width, height);
            width_ = width;
            height_ = height;
            name_ = "";
            next_map_ = "";
            shade_ = 0;
            thread_part1_ = false;
            thread_part2_ = false;
            thread_part1_str_ = "";
            thread_part2_str_ = "";
        }

        public static void Initialize(string choosen_map)
        {
            InitContents();
            map_ = new MapEditor(choosen_map);
            next_map_ = Maps.Maps_[choosen_map].Next_map_;
            tutorial_ = Maps.Maps_[choosen_map].Tutorial_;
            custom_ = !Maps.Maps_[choosen_map].Official_;
            width_ = map_.Width_;
            height_ = map_.Height_;
            name_ = choosen_map;
            shade_ = 0;
            thread_part1_ = false;
            thread_part2_ = false;
            thread_part1_str_ = "";
            thread_part2_str_ = "";
        }

        private static void InitContents()
        {
            contents_ = new List<WTuple<int, string>>();
            contents_.Add(new WTuple<int, string>(-1, "Void"));
            contents_.Add(new WTuple<int, string>(1, "Wall"));
            contents_.Add(new WTuple<int, string>(2, "Torch"));
            contents_.Add(new WTuple<int, string>(3, "Water"));
            contents_.Add(new WTuple<int, string>(7, "Acid"));
            contents_.Add(new WTuple<int, string>(4, "Jump pad"));
            contents_.Add(new WTuple<int, string>(5, "Gravity pad"));
            contents_.Add(new WTuple<int, string>(8, "Checkpoint"));
            contents_.Add(new WTuple<int, string>(6, "Next level"));
            contents_.Add(new WTuple<int, string>(9, "Coin"));
            contents_.Add(new WTuple<int, string>(10, "Cell changer"));
            contents_.Add(new WTuple<int, string>(11, "Cell type 1"));
            contents_.Add(new WTuple<int, string>(12, "Cell type 2"));
            contents_.Add(new WTuple<int, string>(13, "Portal 1"));
            contents_.Add(new WTuple<int, string>(14, "Portal 2"));
            //contents_.Add(new WTuple<int, string>(15, "???"));
            contents_.Add(new WTuple<int, string>(16, "Laser"));
            contents_.Add(new WTuple<int, string>(-2, "Message"));
            max_width_ = -1;
            x_ = -max_width_;
            foreach (var i in contents_)
            {
                if (Textures.Font_.MeasureString(i.Item2_).X > max_width_)
                    max_width_ = (int)Textures.Font_.MeasureString(i.Item2_).X;
            }
            InitThread();
            x_ = -max_width_ - 35;
        }

        private static void InitThread()
        {
            move_panel_ = new Thread(() =>
            {
                while (x_ < 0)
                {
                    x_+= 4;
                    x_ = Math.Min(x_, 0);
                    Thread.Sleep(1);
                }
                Thread.Sleep(1500);
                while (x_ > -max_width_ - 35)
                {
                    x_-= 4;
                    Thread.Sleep(1);
                }
            });
        }

        public static void Update(int screenwidth, int screenheight)
        {
            mouseState_ = Mouse.GetState();
            if (!thread_part1_ && !thread_part2_)
            {
                if (MapChooser.Active_)
                {
                    MapChooser.Update(screenwidth, screenheight);
                }
                else if (EditorOptions.Active_)
                {
                    EditorOptions.Update(screenwidth, screenheight);
                }
                else
                {
                    #region Zoom
                    if (mouseState_.ScrollWheelValue - oldMouseState_.ScrollWheelValue < 0)
                    {
                        if (Map.Size_ > 2)
                            Map.Size_--;
                        int max_shift_x = Math.Max(0, width_ * Map.Size_ - screenwidth);
                        int max_shift_y = Math.Max(0, height_ * Map.Size_ - screenheight);
                        map_.Shift_x_ = Math.Min(max_shift_x, map_.Shift_x_);
                        map_.Shift_y_ = Math.Min(max_shift_y, map_.Shift_y_);
                    }
                    else if (mouseState_.ScrollWheelValue - oldMouseState_.ScrollWheelValue > 0)
                    {
                        if (Map.Size_ < 200)
                            Map.Size_++;
                        int max_shift_x = Math.Max(0, width_ * Map.Size_ - screenwidth);
                        int max_shift_y = Math.Max(0, height_ * Map.Size_ - screenheight);
                        map_.Shift_x_ = Math.Min(max_shift_x, map_.Shift_x_);
                        map_.Shift_y_ = Math.Min(max_shift_y, map_.Shift_y_);
                    }
                    #endregion
                    if (mouseState_.X <= 105 && mouseState_.Y <= 55)
                    {
                        side_ = true;
                    }
                    else
                    {
                        side_ = false;
                    }
                    #region Shift
                    if (width_ * Map.Size_ < screenwidth)
                    {
                        map_.F_shift_x_ = -(screenwidth - width_ * Map.Size_) / 2;
                        map_.Shift_x_ = 0;
                    }
                    else
                    {
                        map_.F_shift_x_ = 0;
                        if (mouseState_.X < 100 && map_.Shift_x_ - 4 >= 0)
                            map_.Shift_x_ -= 4;
                        if (mouseState_.X >= screenwidth - 100 && map_.Shift_x_ + 4 <= width_ * Map.Size_ - screenwidth)
                            map_.Shift_x_ += 4;
                    }
                    if (height_ * Map.Size_ < screenheight)
                    {
                        map_.F_shift_y_ = -(screenheight - height_ * Map.Size_) / 2;
                        map_.Shift_y_ = 0;
                    }
                    else
                    {
                        map_.F_shift_y_ = 0;
                        if (mouseState_.Y < 100 && map_.Shift_y_ - 4 >= 0)
                            map_.Shift_y_ -= 4;
                        if (mouseState_.Y >= screenheight - 100 && map_.Shift_y_ + 4 <= height_ * Map.Size_ - screenheight)
                            map_.Shift_y_ += 4;
                    }
                    #endregion
                    #region Inputs
                    if (mouseState_.LeftButton == ButtonState.Pressed)
                    {
                        if (Input.IsPressed(Keys.LeftShift) || Input.IsPressed(Keys.RightShift))
                            map_.SetPosition((mouseState_.X + map_.Shift_x_ + map_.F_shift_x_) / Map.Size_, (mouseState_.Y + map_.Shift_y_ + map_.F_shift_y_) / Map.Size_);
                        else
                            map_.ChangeCell((mouseState_.X + map_.Shift_x_ + map_.F_shift_x_) / Map.Size_, (mouseState_.Y + map_.Shift_y_ + map_.F_shift_y_) / Map.Size_, layer_, contents_[content_].Item1_);
                    }
                    else if (mouseState_.RightButton == ButtonState.Pressed)
                        try
                        {
                            int n = 0;
                            foreach (var w in contents_)
                            {
                                if (w.Item1_ == map_.Cells_[(mouseState_.X + map_.Shift_x_ + map_.F_shift_x_) / Map.Size_, (mouseState_.Y + map_.Shift_y_ + map_.F_shift_y_) / Map.Size_].Content_[layer_])
                                    break;
                                n++;
                            }
                            content_ = n;
                            if (move_panel_.IsAlive)
                                move_panel_.Abort();
                            InitThread();
                            move_panel_.Start();
                        }
                        catch { }
                    if (oldMouseState_.LeftButton == ButtonState.Pressed && mouseState_.LeftButton == ButtonState.Released)
                    {
                        if (contents_[content_].Item1_ == 6)
                        {
                            MapChooser.Initialize(screenwidth, screenheight);
                            MapChooser.Activate();
                        }
                    }
                    if (Input.IsPressedOnce(Keys.Up))
                    {
                        content_--;
                        if (content_ < 0)
                            content_ = contents_.Count() - 1;
                        if (move_panel_.IsAlive)
                            move_panel_.Abort();
                        InitThread();
                        move_panel_.Start();
                        
                    }
                    else if (Input.IsPressedOnce(Keys.Down))
                    {
                        content_++;
                        content_ %= contents_.Count();
                        if (move_panel_.IsAlive)
                            move_panel_.Abort();
                        InitThread();
                        move_panel_.Start();
                    }
                    if (Input.IsPressedOnce(Keys.Right))
                    {
                        layer_ = layer_ == 2 ? layer_ : layer_ + 1; /* FIXME: 3 layers => layer_ == 2 */
                    }
                    else if (Input.IsPressedOnce(Keys.Left))
                    {
                        layer_ = layer_ == 0 ? layer_ : layer_ - 1;
                    }
                    if (Input.IsPressedOnce(Keys.S))
                    {
                        Save();
                    }
                    if (Input.IsPressedOnce(Keys.O))
                    {
                        EditorOptions.Initialize(screenwidth, screenheight);
                        EditorOptions.Activate();
                    }
                    if (Input.IsPressedOnce(Keys.Escape))
                    {
                        Thread thread2 = new Thread(() =>
                            {
                                thread_part1_str_ = "Updating map...";
                                Thread thread1 = new Thread(() =>
                                {
                                    thread_part1_ = true;
                                    Maps.Initialize(name_);
                                    thread_part1_ = false;
                                    thread_part2_str_ = "Map updated.";
                                    thread_part2_ = true;
                                });
                                thread1.Start();
                                DateTime start = DateTime.Now;
                                while (DateTime.Now - start < new TimeSpan(0, 0, 0, 0, 250))
                                {
                                    shade_ = 0.75f * (float)(DateTime.Now - start).TotalMilliseconds / 250f;
                                }
                                while (thread_part1_) ;
                                Thread.Sleep(500);
                                start = DateTime.Now;
                                while (DateTime.Now - start < new TimeSpan(0, 0, 0, 0, 250))
                                {
                                    shade_ = 0.75f + 0.25f * (float)(DateTime.Now - start).TotalMilliseconds / 250f;
                                }
                                TitleScreen.Initialize();
                                TitleScreen.Activate();
                                Map.Size_ = 20;
                                Thread.Sleep(50);
                                thread_part2_ = false;
                            });
                        thread2.Start();
                    }
                    #endregion
                    #region Auto update layer
                    switch (contents_[content_].Item1_)
                    {
                        case 3:
                        case 7:
                            layer_ = 1;
                            break;
                        case -2:
                            layer_ = 2;
                            break;
                        case 2:
                        case 4:
                        case 5:
                        case 6:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                        case 16:
                            layer_ = 0;
                            break;
                        default:
                            break;
                    }
                    #endregion
                }
            }
            oldMouseState_ = mouseState_;
        }

        private static void Save()
        {
            Thread thread2 = new Thread(() =>
                {
                    thread_part1_str_ = "Saving...";
                    Thread thread1 = new Thread(() =>
                    {
                        #region Thread
                        thread_part1_ = true;
                        XmlDocument xmlDocument = new XmlDocument();
                        try
                        {
                            xmlDocument.Load("../../../../LightContent/database/maps.xml");
                        }
                        catch
                        {
                            thread_part1_ = false;
                            thread_part2_str_ = "Unable to save: cannot load XML.";
                            thread_part2_ = true;
                            Console.WriteLine("Unable to save: cannot load XML.");
                        }
                        XmlNode root = xmlDocument.DocumentElement;
                        int index = 0;
                        XmlNode node = xmlDocument.SelectSingleNode("//maps");
                        if (node != null)
                        {
                            if (name_ == "")
                            {
                                index = 1 + Convert.ToInt16(node.Attributes["index"].Value);
                                node.Attributes["index"].Value = (index).ToString();
                                XmlElement elem = xmlDocument.CreateElement("map");
                                XmlElement name = xmlDocument.CreateElement("name");
                                name.InnerText = "map" + index;
                                name_ = "map" + index;
                                XmlElement width = xmlDocument.CreateElement("width");
                                width.InnerText = width_.ToString();
                                XmlElement height = xmlDocument.CreateElement("height");
                                height.InnerText = height_.ToString();
                                XmlElement content = xmlDocument.CreateElement("content");
                                content.InnerText = map_.GetContent();
                                XmlElement position = xmlDocument.CreateElement("position");
                                position.InnerText = map_.Position_.X + " " + map_.Position_.Y;
                                XmlElement gravity = xmlDocument.CreateElement("gravity");
                                gravity.InnerText = "1";
                                XmlElement tutorial = xmlDocument.CreateElement("tutorial");
                                XmlElement official = xmlDocument.CreateElement("official");
                                XmlElement next_map = xmlDocument.CreateElement("next-map");
                                next_map.InnerText = next_map_ == "" ? name_ : next_map_;
                                elem.AppendChild(name);
                                elem.AppendChild(width);
                                elem.AppendChild(height);
                                elem.AppendChild(content);
                                elem.AppendChild(position);
                                elem.AppendChild(gravity);
                                if (tutorial_)
                                    elem.AppendChild(tutorial);
                                if (!custom_)
                                    elem.AppendChild(official);
                                if (map_.Messages_.Count > 0)
                                {
                                    XmlElement messages = xmlDocument.CreateElement("messages");
                                    foreach (var entry in map_.Messages_)
                                    {
                                        XmlElement message = xmlDocument.CreateElement("message");
                                        message.SetAttribute("value", entry.Key.ToString());
                                        message.InnerText = entry.Value;
                                        messages.AppendChild(message);
                                    }
                                    elem.AppendChild(messages);
                                }
                                elem.AppendChild(next_map);
                                root.AppendChild(elem);
                                Console.WriteLine("Saving new map: " + name_ + "...");
                            }
                            else
                            {
                                node = root.SelectSingleNode("descendant::map[name='" + name_ + "']");
                                root.RemoveChild(node);
                                XmlElement elem = xmlDocument.CreateElement("map");
                                XmlElement name = xmlDocument.CreateElement("name");
                                name.InnerText = name_;
                                XmlElement width = xmlDocument.CreateElement("width");
                                width.InnerText = width_.ToString();
                                XmlElement height = xmlDocument.CreateElement("height");
                                height.InnerText = height_.ToString();
                                XmlElement content = xmlDocument.CreateElement("content");
                                content.InnerText = map_.GetContent();
                                XmlElement position = xmlDocument.CreateElement("position");
                                position.InnerText = map_.Position_.X + " " + map_.Position_.Y;
                                XmlElement gravity = xmlDocument.CreateElement("gravity");
                                gravity.InnerText = "1";
                                XmlElement tutorial = xmlDocument.CreateElement("tutorial");
                                XmlElement official = xmlDocument.CreateElement("official");
                                XmlElement next_map = xmlDocument.CreateElement("next-map");
                                next_map.InnerText = next_map_ == "" ? name_ : next_map_;
                                elem.AppendChild(name);
                                elem.AppendChild(width);
                                elem.AppendChild(height);
                                elem.AppendChild(content);
                                elem.AppendChild(position);
                                elem.AppendChild(gravity);
                                if (tutorial_)
                                    elem.AppendChild(tutorial);
                                if (!custom_)
                                    elem.AppendChild(official);
                                if (map_.Messages_.Count > 0)
                                {
                                    XmlElement messages = xmlDocument.CreateElement("messages");
                                    foreach (var entry in map_.Messages_)
                                    {
                                        XmlElement message = xmlDocument.CreateElement("message");
                                        message.SetAttribute("value", entry.Key.ToString());
                                        message.InnerText = entry.Value;
                                        messages.AppendChild(message);
                                    }
                                    elem.AppendChild(messages);
                                }
                                elem.AppendChild(next_map);
                                root.AppendChild(elem);
                                Console.WriteLine("Saving map: " + name_ + "...");
                            }
                            try
                            {
                                xmlDocument.Save("../../../../LightContent/database/maps.xml");
                                thread_part1_ = false;
                                thread_part2_str_ = name_ + " saved.";
                                thread_part2_ = true;
                                Console.WriteLine("Save complete.");
                            }
                            catch
                            {
                                thread_part1_ = false;
                                thread_part2_str_ = "Unable to save: cannot write.";
                                thread_part2_ = true;
                                Console.WriteLine("Unable to save: cannot write.");
                            }
                        }
                        else
                        {
                            thread_part1_ = false;
                            thread_part2_str_ = "Unable to save: wrong XML format.";
                            thread_part2_ = true;
                            Console.WriteLine("Unable to save: wrong XML format.");
                        }
                        #endregion
                    });
                    thread1.Start();
                    DateTime start = DateTime.Now;
                    while (DateTime.Now - start < new TimeSpan(0, 0, 0, 0, 250))
                    {
                        shade_ = 0.75f * (float)(DateTime.Now - start).TotalMilliseconds / 250f;
                    }
                    while (thread_part1_) ;
                    Thread.Sleep(500);
                    start = DateTime.Now;
                    while (DateTime.Now - start < new TimeSpan(0, 0, 0, 0, 250))
                    {
                        shade_ = 0.75f * (1 - (float)(DateTime.Now - start).TotalMilliseconds / 250f);
                    }
                    thread_part2_ = false;
                });
            thread2.Start();
        }

        private void RunThreads(int count, bool[] states, string[] messages, Delegate[] functions, Delegate[] before, Delegate[] after)
        {
            Thread main_thread = new Thread(() =>
               {
                   for (int i = 0; i < count; i++)
                   {
                   Thread thread = new Thread(() =>
                   {
                       states[i] = true;
                       functions[i].DynamicInvoke();
                       states[i] = false;
                   });
                   thread.Start();
                   before[i].DynamicInvoke();
                   while (thread_part1_) ;
                   after[i].DynamicInvoke();
                   }
               });
            main_thread.Start();
        }

        public static void Draw(SpriteBatch spriteBatch, int screenwidth, int screenheight)
        {
            map_.Draw(spriteBatch, screenwidth, screenheight);
            //mouseState_.X <= 100 && mouseState_.Y <= 60
            spriteBatch.Draw(Textures.Pixel_, new Rectangle(0 + (!side_ ? 0 : screenwidth - 105), 0, 105, 55), new Color(0, 0, 0, 128));
            spriteBatch.DrawString(Textures.Font_, "Layer: " + layer_, Vector2.Zero + (!side_ ? Vector2.Zero : new Vector2(screenwidth - 105, 0)), Color.White);
            spriteBatch.DrawString(Textures.Font_, "Content:", new Vector2(0, 30) + (!side_ ? Vector2.Zero : new Vector2(screenwidth - 105, 0)), Color.White);
            int[] content = { contents_[content_].Item1_, -1, -1 };
            new CellEditor(content).Draw(spriteBatch, (int)Textures.Font_.MeasureString("Content:").X + (!side_ ? 0 : screenwidth - 105), 30, 20);
            int n = 0;
            spriteBatch.Draw(Textures.Pixel_, new Rectangle(x_, 55, max_width_ + 30 + 5, 25 * contents_.Count() + 10), new Color(0, 0, 0, 128));
            foreach (var i in contents_)
            {
                if (content_ == n)
                {
                    spriteBatch.Draw(Textures.Pixel_, new Rectangle(x_ + 2, 65 + n * 25 - 3, max_width_ + 30 + 1, 26), new Color(0, 0, 0, 128));
                }
                int[] temp = { i.Item1_, -1, -1 };
                new CellEditor(temp).Draw(spriteBatch, 5 + x_, 65 + n * 25, 20);
                spriteBatch.DrawString(Textures.Font_, i.Item2_, new Vector2(30 + x_, 65 + n * 25), Color.White);
                n++;
            }
            if (MapChooser.Active_)
            {
                MapChooser.Draw(spriteBatch, screenwidth, screenheight);
            }
            else if (EditorOptions.Active_)
            {
                EditorOptions.Draw(spriteBatch, screenwidth, screenheight);
            }
            if (thread_part1_)
            {
                spriteBatch.Draw(Textures.Pixel_, new Rectangle(0, 0, screenwidth, screenheight), new Color(0f, 0f, 0f, shade_));
                Vector2 v = Textures.Font_.MeasureString(thread_part1_str_);
                spriteBatch.DrawString(Textures.Font_, thread_part1_str_, new Vector2(screenwidth, screenheight) / 2 - v / 2, new Color(1f, 1f, 1f, shade_));
            }
            else if (thread_part2_)
            {
                spriteBatch.Draw(Textures.Pixel_, new Rectangle(0, 0, screenwidth, screenheight), new Color(0f, 0f, 0f, shade_));
                Vector2 v = Textures.Font_.MeasureString(thread_part2_str_);
                spriteBatch.DrawString(Textures.Font_, thread_part2_str_, new Vector2(screenwidth, screenheight) / 2 - v / 2, new Color(1f, 1f, 1f, shade_));
            }
        }
    }
}
