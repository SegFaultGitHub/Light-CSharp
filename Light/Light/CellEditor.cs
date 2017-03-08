using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcadeGame
{
    class CellEditor
    {
        private int[] content_;
        public int[] Content_
        {
            get { return content_; }
            set { content_ = value; }
        }
        private int message_index_;
        public int Message_index_
        {
            get { return message_index_; }
            set { message_index_ = value; }
        }

        public CellEditor(int[] content)
        {
            content_ = content;
        }

        public string GetContent()
        {
            return content_[0] + "/" + content_[1] + "/" + (content_[2] == -2 ? "m" + message_index_ : content_[2].ToString());
        }

        public void Draw(SpriteBatch spriteBatch, int x, int y, int size = 0)
        {
            if (size == 0)
                size = Map.Size_;
            int layer = 1;
            foreach (int n in content_)
            {
                switch (n)
                {
                    case -2: // Message
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x, y, size, size), new Color(0, 150, 150, 255 / layer));
                        break;
                    case -1: // Default
                    case 0:
                        break;
                    case 1:
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x, y, size, size), new Color(0, 0, 0, 255 / layer));
                        break;
                    case 2:
                        spriteBatch.Draw(Textures.Torch_, new Rectangle(x, y, size, size), new Color(255, 255, 255, 255 / layer));
                        break;
                    case 3:
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x, y, size, size), new Color(0, 0, 255, 255 / layer));
                        break;
                    case 4: // Jump pad
                        spriteBatch.Draw(Textures.Circle_, new Rectangle(x, y, size, size), new Color(191, 63, 63, 255 / layer));
                        break;
                    case 5: // Gravity pad
                        spriteBatch.Draw(Textures.Circle_, new Rectangle((int)(x - 0 / 2), (int)(y - 0 / 2), (int)(size + 0), (int)(size + 0)), new Color(63, 63, 255));
                        spriteBatch.Draw(Textures.Circle_, new Rectangle((int)(x - (-0 - 3f * size / 4f) / 2), (int)(y - (-0 - 3f * size / 4f) / 2), (int)(size + (-0 - 3f * size / 4f)), (int)(size + (-0 - 3f * size / 4f))), new Color(31, 31, 127));
                        break;
                    case 6:
                        for (int i = 0; i < 4f; i++)
                            spriteBatch.Draw(Textures.Spiral_, new Rectangle(x + size / 2, y + size / 2, size, size), null, new Color(255, (255 - 192) / 4, (255 - 192) / 4, 255 / layer), (float)i * (float)Math.PI / 2, new Vector2(Textures.Spiral_.Width / 2, Textures.Spiral_.Height / 2), SpriteEffects.None, 0f);
                        break;
                    case 7:
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x, y, size, size), new Color(0, 255, 0, 255 / layer));
                        break;
                    case 8:
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x + size / 4, y + size / 4, size / 2, size / 2), new Color(255, 128, 64, 255 / layer));
                        break;
                    case 9:
                        spriteBatch.Draw(Textures.Coin_, new Rectangle(x, y, size, size), new Color(255, 255, 255, 255 / layer));
                        break;
                    case 10: // Block changer
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x + size / 4, y + size / 4, size / 2, size / 2), new Color(64, 128, 64, 255 / layer));
                        break;
                    case 11: // Block type 1
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x, y, size, size), new Color(255, 0, 0, 255 / layer));
                        break;
                    case 12: // Block type 2
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x, y, size, size), new Color(128, 0, 0, 255 / layer));
                        break;
                    case 13: // Portal 1
                        spriteBatch.Draw(Textures.Portal_, new Rectangle(x, y, size, size), new Color(0, 0, 255, 255 / layer));
                        break;
                    case 14: // Portal 2
                        spriteBatch.Draw(Textures.Portal_, new Rectangle(x, y, size, size), new Color(255, 0, 0, 255 / layer));
                        break;
                    case 16: // Laser
                        spriteBatch.Draw(Textures.Laser_, new Rectangle(x, y, size, size), new Color(255, 255, 255, 255 / layer));
                        break;
                    default: // Unknown
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x, y, size, size), new Color(255, 0, 255, 255 / layer));
                        break;
                }
                layer++;
            }
        }
    }
}
