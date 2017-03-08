using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcadeGame
{
    class Message
    {
        private static bool display_;
        public static bool Display_
        {
            get { return Message.display_; }
        }
        private static string text_;
        private static Vector2 max_size_, position_, size_;
        

        public static void Compute()
        {
            int width = Drawer.Graphics_.PreferredBackBufferWidth;
            int height = Drawer.Graphics_.PreferredBackBufferHeight;
            Vector2 character = Textures.Font_.MeasureString(" ") - new Vector2(0, 1);
            max_size_ = new Vector2((int)Math.Floor(width / character.X), (int)Math.Floor(height / character.Y));
        }

        public static void Display(string text, bool freeze = true)
        {
            if (freeze)
                GameHandler.FreezeCharacter();
            text += " ";
            display_ = true;
            int start = 0;
            List<string> result = new List<string>();
            int max_width = (int)max_size_.X;
            text.Replace("\t", "    ");
            while (true)
            {
                max_width = Math.Min(text.Length - start, max_width);
                if (max_width <= 0)
                    break;
                string current_text = text.Substring(start, max_width);
                int first_return = current_text.IndexOf('\n');
                int last_space;
                if (first_return != -1)
                    last_space = Math.Min(current_text.LastIndexOf(' '), first_return);
                else
                    last_space = current_text.LastIndexOf(' ');
                if (last_space == -1)
                    result.Add(current_text);
                else
                    result.Add(current_text.Substring(0, last_space));
                if (last_space == -1)
                    start += max_width;
                else
                    start += last_space + 1;
            }
            text_ = "";
            for (int i = 0; i < result.Count; i++)//string str in result)
                text_ += result[i] + (i == result.Count - 1 ? "" : "\n");
            size_ = Textures.Font_.MeasureString(text_);
            position_ = new Vector2(Drawer.Graphics_.PreferredBackBufferWidth, Drawer.Graphics_.PreferredBackBufferHeight) / 2 - size_ / 2;
        }

        public static void Close()
        {
            display_ = false;
            GameHandler.UnfreezeCharacter();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Pixel_, new Rectangle(0, (int)position_.Y, Drawer.Graphics_.PreferredBackBufferWidth, (int)size_.Y), new Color(0, 0, 0, 128));
            spriteBatch.DrawString(Textures.Font_, text_, position_, Color.White);
        }
    }
}
