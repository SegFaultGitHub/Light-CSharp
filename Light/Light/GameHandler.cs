using Microsoft.Xna.Framework;
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
    class GameHandler
    {
        private static string version_ = "0.7";
        public static string Version_
        {
            get { return GameHandler.version_; }
        }
        private static Map map_;
        private static Character dash_character_ = null;
        internal static Character Dash_character_
        {
            get { return GameHandler.dash_character_; }
            set { GameHandler.dash_character_ = value; }
        }
        private static Character character_;
        public static Character Character_
        {
            get { return GameHandler.character_; }
        }
        private static bool sounds_ = true;
        public static bool Sounds_
        {
            get { return GameHandler.sounds_; }
            set { GameHandler.sounds_ = value; }
        }
        private static DateTime timer_start_, timer_end_;
        private static bool timer_on_;
        private static Thread dash_character_fade_;

        public static void SetCharacter(Character character)
        {
            character_ = character;
        }

        public static void CreateDashCharacter()
        {
            dash_character_ = new Character();
            dash_character_.Position_ = character_.Position_;
            dash_character_.Color_ = character_.Color_;
            if (dash_character_fade_ != null && dash_character_fade_.IsAlive)
                dash_character_fade_.Abort();
            dash_character_fade_ = new Thread(() =>
            {
                while (dash_character_.Shade_ > 0)
                {
                    dash_character_.Shade_ -= 0.05f;
                    dash_character_.Color_ = new Color(dash_character_.Color_.R, dash_character_.Color_.G, dash_character_.Color_.B, dash_character_.Shade_);
                    Thread.Sleep(5);
                }
                dash_character_ = null;
            });
            dash_character_fade_.Start();
        }

        public static void FreezeCharacter()
        {
            character_.Freeze();
        }

        public static void UnfreezeCharacter()
        {
            character_.Unfreeze();
        }

        public static void SetMap(string str, bool map_finished = false)
        {
            Message.Close();
            timer_on_ = false;
            Drawer.Fade(500);
            character_.Freeze(500);
            Thread thread = new Thread(() =>
            {
                #region New best score
                do
                {
                    if (map_finished)
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        try
                        {
                            xmlDocument.Load("../../../../LightContent/database/scores.xml");
                        }
                        catch
                        {
                            Console.WriteLine("Unable to save: cannot load XML.");
                            break;
                        }
                        XmlNode root = xmlDocument.DocumentElement;
                        XmlNode node = xmlDocument.SelectSingleNode("//scores");
                        node = root.SelectSingleNode("descendant::score[name='" + map_.Name_ + "']");
                        if (node != null)
                        {
                            XmlNode score = root.SelectSingleNode("//best-score");
                            if (score != null)
                            {
                                string score_str = score.InnerText;
                                int hours = Convert.ToInt32(score_str.Split(':')[0]);
                                int minutes = Convert.ToInt32(score_str.Split(':')[1]);
                                int seconds = Convert.ToInt32(score_str.Split(':')[2].Split('.')[0]);
                                int milliseconds = Convert.ToInt32(score_str.Split(':')[2].Split('.')[1].Substring(0, 3));
                                TimeSpan prev = new TimeSpan(0, hours, minutes, seconds, milliseconds);
                                if (timer_end_ - timer_start_ >= prev)
                                    break;
                            }
                            root.RemoveChild(node);
                        }
                        XmlElement elem = xmlDocument.CreateElement("score");
                        XmlElement name = xmlDocument.CreateElement("name");
                        name.InnerText = map_.Name_;
                        XmlElement best_score = xmlDocument.CreateElement("best-score");
                        best_score.InnerText = (timer_end_ - timer_start_).ToString();
                        elem.AppendChild(name);
                        elem.AppendChild(best_score);
                        root.AppendChild(elem);
                        Console.WriteLine("Saving new score...");
                        try
                        {
                            xmlDocument.Save("../../../../LightContent/database/scores.xml");
                            Console.WriteLine("Score saved.");
                            TitleScreen.Initialize();
                        }
                        catch
                        {
                            Console.WriteLine("Unable to save: cannot write.");
                        }
                    }
                } while (false);
                #endregion
                Thread.Sleep(500);
                map_ = Maps.Maps_[str];
                map_.ResetLight();
                map_.SetCheckpoint(map_.Initial_position_, map_.Initial_gravity_);
                character_.SetPosition(map_, map_.Initial_position_);
                Character.Gravity_sign_ = map_.Initial_gravity_;
                Map.Block_type_enabled_ = 1;
                timer_start_ = DateTime.Now;
                timer_end_ = DateTime.Now;
                timer_on_ = true;
            });
            thread.Start();
        }

        public static void Update(int screenwidth, int screenheight)
        {
            if (map_ != null)
            {
                Background.Update(screenwidth, screenheight);
                Particles.Update();
                map_.Update(character_, screenwidth, screenheight);
                character_.Update(map_, screenwidth, screenheight);
                if (Input.IsPressedOnce(Keys.Escape))
                {
                    map_ = null;
                    //Maps.Initialize();
                    //TitleScreen.Initialize(screenwidth, screenheight);
                    TitleScreen.Activate();
                }
                else if (Input.IsPressedOnce(Keys.R))
                {
                    SetMap(map_.Name_);
                }
                if (Input.IsPressedOnce(Keys.Enter))
                {
                    Message.Close();
                }
                if (timer_on_)
                    timer_end_ = DateTime.Now;
            }
        }

        public static void Draw(SpriteBatch spriteBatch, int screenwidth, int screenheight)
        {
            if (map_ != null)
                Drawer.Draw(spriteBatch, map_, character_, dash_character_, timer_end_ - timer_start_, screenwidth, screenheight);
            else
                spriteBatch.Draw(Textures.Pixel_, new Rectangle(0, 0, screenwidth, screenheight), Color.Black);
        }
    }
}
