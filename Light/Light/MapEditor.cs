using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcadeGame
{
    class MapEditor
    {
        private Vector2 position_;
        public Vector2 Position_
        {
            get { return position_; }
        }
        private int f_shift_x_ = 0, f_shift_y_ = 0;
        public int F_shift_x_
        {
            get { return f_shift_x_; }
            set { f_shift_x_ = value; }
        }
        public int F_shift_y_
        {
            get { return f_shift_y_; }
            set { f_shift_y_ = value; }
        }
        private int shift_x_, shift_y_;
        public int Shift_x_
        {
            get { return shift_x_; }
            set { shift_x_ = value; }
        }
        public int Shift_y_
        {
            get { return shift_y_; }
            set { shift_y_ = value; }
        }
        private CellEditor[,] cells_;
        internal CellEditor[,] Cells_
        {
            get { return cells_; }
            set { cells_ = value; }
        }
        private int width_, height_;
        public int Width_
        {
            get { return width_; }
        }
        public int Height_
        {
            get { return height_; }
        }
        private Dictionary<int, string> messages_;
        public Dictionary<int, string> Messages_
        {
            get { return messages_; }
            set { messages_ = value; }
        }

        public MapEditor(string name)
        {
            Map map = Maps.Maps_[name];
            width_ = map.Width_;
            height_ = map.Height_;
            position_ = map.Initial_position_;
            cells_ = new CellEditor[width_, height_];
            for (int i = 0; i < width_; i++)
            {
                for (int j = 0; j < height_; j++)
                {
                    cells_[i, j] = new CellEditor(map.Map_[i, j].Content_);
                    cells_[i, j].Message_index_ = map.Map_[i, j].Message_index_;
                }
            }
            messages_ = new Dictionary<int, string>();
            foreach (var entry in map.Messages_)
            {
                messages_[entry.Key] = entry.Value;
            }
        }

        public MapEditor(int width, int height)
        {
            messages_ = new Dictionary<int, string>();
            position_ = new Vector2(1);
            width_ = width;
            height_ = height;
            cells_ = new CellEditor[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i == 0 || j == 0 || i == width - 1 || j == height - 1)
                    {
                        int[] content = { 1, -1, -1 };
                        cells_[i, j] = new CellEditor(content);
                    }
                    else
                    {
                        int[] content = { -1, -1, -1 };
                        cells_[i, j] = new CellEditor(content);
                    }
                }
            }
        }

        public void SetPosition(int i, int j)
        {
            position_ = new Vector2(i, j);
        }

        public string GetContent()
        {
            string result = "\n";
            for (int j = 0; j < height_; j++)
            {
                for (int i = 0; i < width_; i++)
                {
                    result += cells_[i, j].GetContent() + ' ';
                }
                result += '\n';
            }
            return result;
        }

        public void ChangeCell(int i, int j, int layer, int content)
        {
            try
            {
                if (layer == 0 && cells_[i, j].Content_[0] == -2 && content != -2)
                {
                    cells_[i, j].Message_index_ = -1;
                }
                else if (layer == 0 && cells_[i, j].Content_[0] == -2 && content == -2)
                    ;
                cells_[i, j].Content_[layer] = content;
            }
            catch (Exception) { }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 origin, int size)
        {
            for (int j = 0; j < height_; j++)
            {
                for (int i = 0; i < width_; i++)
                {
                    if (i % 2 == 0)
                        if (j % 2 == 0)
                            spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)origin.X + size * i, (int)origin.Y + size * j, size, size), new Color(192, 192, 192));
                        else
                            spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)origin.X + size * i, (int)origin.Y + size * j, size, size), new Color(224, 224, 224));
                    else
                        if (j % 2 == 1)
                            spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)origin.X + size * i, (int)origin.Y + size * j, size, size), new Color(192, 192, 192));
                        else
                            spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)origin.X + size * i, (int)origin.Y + size * j, size, size), new Color(224, 224, 224));
                    cells_[i, j].Draw(spriteBatch, (int)origin.X + size * i, (int)origin.Y + size * j, size);
                }
            }
            spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)origin.X + size * (int)position_.X, (int)origin.Y + size * (int)position_.Y, size, size), new Color(255, 0, 0, 128));
        }

        public void Draw(SpriteBatch spriteBatch, int screenwidth, int screenheight)
        {
            for (int j = shift_y_ / Map.Size_; j <= shift_y_ / Map.Size_ + screenheight / Map.Size_ + 1 && j < height_; j++)
            {
                for (int i = shift_x_ / Map.Size_; i <= shift_x_ / Map.Size_ + screenwidth / Map.Size_ + 1 && i < width_; i++)
                {
                    if (i % 2 == 0)
                        if (j % 2 == 0)
                            spriteBatch.Draw(Textures.Pixel_, new Rectangle(i * Map.Size_ - (shift_x_ + f_shift_x_), j * Map.Size_ - (shift_y_ + f_shift_y_), Map.Size_, Map.Size_), new Color(192, 192, 192));
                        else
                            spriteBatch.Draw(Textures.Pixel_, new Rectangle(i * Map.Size_ - (shift_x_ + f_shift_x_), j * Map.Size_ - (shift_y_ + f_shift_y_), Map.Size_, Map.Size_), new Color(224, 224, 224));
                    else
                        if (j % 2 == 1)
                            spriteBatch.Draw(Textures.Pixel_, new Rectangle(i * Map.Size_ - (shift_x_ + f_shift_x_), j * Map.Size_ - (shift_y_ + f_shift_y_), Map.Size_, Map.Size_), new Color(192, 192, 192));
                        else
                            spriteBatch.Draw(Textures.Pixel_, new Rectangle(i * Map.Size_ - (shift_x_ + f_shift_x_), j * Map.Size_ - (shift_y_ + f_shift_y_), Map.Size_, Map.Size_), new Color(224, 224, 224));
                    cells_[i, j].Draw(spriteBatch, i * Map.Size_ - (shift_x_ + f_shift_x_), j * Map.Size_ - (shift_y_ + f_shift_y_));
                }
            }
            spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)position_.X * Map.Size_ - (shift_x_ + f_shift_x_), (int)position_.Y * Map.Size_ - (shift_y_ + f_shift_y_), Map.Size_, Map.Size_), new Color(255, 0, 0, 128));
        }
    }
}
