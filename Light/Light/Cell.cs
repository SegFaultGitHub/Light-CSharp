using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ArcadeGame
{
    class Cell
    {
        private int[] content_;
        public int[] Content_
        {
            get { return content_; }
            set { content_ = value; }
        }
        private bool walkable_;
        public bool Walkable_
        {
            get { return walkable_; }
        }
        private bool opaque_;
        public bool Opaque_
        {
            get { return opaque_; }
            set { opaque_ = value; }
        }
        private bool light_source_;
        public bool Light_source_
        {
            get { return light_source_; }
            set { light_source_ = value; }
        }
        private float sign_;
        private float[,] var_;
        public float[,] Var_
        {
            get { return var_; }
            set { var_ = value; }
        }
        private static Random rand_ = new Random();
        private bool freeze_;
        public bool Freeze_
        {
            get { return freeze_; }
            set { freeze_ = value; }
        }
        private bool moving_;
        public bool Moving_
        {
            get { return moving_; }
            set { moving_ = value; }
        }
        private int direction_;
        public int Direction_
        {
            get { return direction_; }
            set { direction_ = value; }
        }
        private static float rotate_speed_ = 0.25f;
        private int message_index_ = -1;
        public int Message_index_
        {
            get { return message_index_; }
            set { message_index_ = value; }
        }


        public Cell(int[] content, int i = 0, int j = 0, int message_index = -1)
        {
            message_index_ = message_index;
            direction_ = 1;
            var_ = new float[3, 9];
            freeze_ = false;
            content_ = content;
            //moving_ = content[0] == 10;
            walkable_ = content[0] != 1/* && content[0] != 10*/;
            opaque_ = content[0] == 1/* || content[0] == 10*/;
            light_source_ = content.Contains(2);
            if (content_[1] == 3)
            {
                sign_ = 1;
                var_[1, 1] = rand_.Next(128, 255);
            }
            else if (content[0] == 5)
            {
                sign_ = 1f;
                var_[0, 0] = -Map.Size_;
            }
            else if (content[0] == 4)
            {
                sign_ = 1f;
                var_[0, 0] = -7f;
            }
            else if (content_[0] == 6)
            {
                var_[0, 1] = rand_.Next(128, 255);
                var_[0, 2] = rand_.Next(128, 255);
                var_[0, 3] = rand_.Next(128, 255);
                var_[0, 4] = rand_.Next(128, 255);
                var_[0, 5] = rand_.Next(1) % 2 == 0 ? 1 : -1;
                var_[0, 6] = rand_.Next(1) % 2 == 0 ? 1 : -1;
                var_[0, 7] = rand_.Next(1) % 2 == 0 ? 1 : -1;
                var_[0, 8] = rand_.Next(1) % 2 == 0 ? 1 : -1;
                var_[0, 0] = rand_.Next(128, 255);
            }
            else if (content_[0] == 16) /* laser */
            {
                var_[0, 1] = 0; // current angle
                var_[0, 2] = 0; // total modification
                var_[0, 3] = 0; // max angle
                var_[0, 4] = rotate_speed_; // sign
                var_[0, 5] = 0; // Length
            }
        }

        public bool Walkable()
        {
            if (content_[0] == 11 || content_[0] == 12) // Changing cells
            {
                if (content_[0] == 11)
                    return Map.Block_type_enabled_ == 0;
                else
                    return Map.Block_type_enabled_ == 1;
            }
            return walkable_;
        }

        public bool Opaque()
        {
            if (content_[0] == 11 || content_[0] == 12) // Changing cells
            {
                if (content_[0] == 12)
                    return Map.Block_type_enabled_ == 0;
                else
                    return Map.Block_type_enabled_ == 1;
            }
            return opaque_;
        }

        public void Update(int i, int j)
        {
            if (content_[1] == 3 || content_[1] == 7)
            {
                if (var_[1, 1] < 128)
                    sign_ = rand_.Next(1, 5);
                else if (var_[1, 1] > 255)
                    sign_ = -rand_.Next(1, 5);
                var_[1, 1] += sign_;
            }
            if (content_[0] == 2)
            {
                Particles.createParticles(new Vector2(i * Map.Size_ + Map.Size_ / 2, j * Map.Size_ + Map.Size_ / 2 - 3), rand_.Next() % 2 == 0 ? Color.Orange : Color.Red);
            }
            if (content_[0] == 13 || content_[0] == 14)
            {
                Vector2 position = GameHandler.Character_.Position_ + new Vector2(GameHandler.Character_.Size_);
                Vector2 diff = new Vector2((i + 0.5f) * Map.Size_, (j + 0.5f) * Map.Size_) - position;
                float temp = diff.X;
                diff.X = diff.Y;
                diff.Y = temp;
                diff.Normalize();
                float angle = 0f;
                if (diff.X > 0 && diff.Y < 0)
                    angle = (float)Math.Acos(Vector2.Dot(diff, new Vector2(1, 0)));
                else if (diff.X > 0 && diff.Y > 0)
                    angle = (float)Math.PI + (float)Math.Acos(Vector2.Dot(diff, new Vector2(-1, 0)));
                else if (diff.X < 0 && diff.Y > 0)
                    angle = (float)Math.PI + (float)Math.Acos(Vector2.Dot(diff, new Vector2(-1, 0)));
                else
                    angle = (float)Math.Acos(Vector2.Dot(diff, new Vector2(1, 0)));
                var_[0, 0] = angle;
                if (Math.Abs(var_[0, 1] - var_[0, 0]) < 0.2f)
                    var_[0, 1] = var_[0, 0];
                else
                    var_[0, 1] += Math.Sign(var_[0, 0] - var_[0, 1]) * 0.1f;
            }
            if (content_[0] == 6)
            {
                for (int k = 1; k <= 4; k++)
                {
                    if (var_[0, k] < 128)
                        var_[0, k + 4] = rand_.Next(1, 5);
                    else if (var_[0, k] > 255)
                        var_[0, k + 4] = -rand_.Next(1, 5);
                    var_[0, k] += var_[0, k + 4];
                }
                var_[0, 0] += 0.05f;
            }
            if (content_[0] == 16) // Laser
            {
                if (var_[0, 3] != 0f)
                {
                    if (var_[0, 3] == 360f)
                    {
                        var_[0, 1] += rotate_speed_;
                        var_[0, 1] += 360f;
                        var_[0, 1] %= 360f;
                    }
                    else
                    {
                        if (var_[0, 2] >= var_[0, 3])
                            var_[0, 4] = -rotate_speed_;
                        else if (var_[0, 2] <= 0)
                            var_[0, 4] = rotate_speed_;
                        var_[0, 1] += var_[0, 4];
                        var_[0, 2] += var_[0, 4];
                    }
                }
                Vector2 direction = new Vector2((float)Math.Cos(-var_[0, 1] / 180f * Math.PI), (float)Math.Sin(-var_[0, 1] / 180f * Math.PI));
                direction.Normalize();
                Particles.createParticles(new Vector2(i + 0.5f, j + 0.5f) * Map.Size_ + direction * var_[0, 5], Color.Red);
            }
        }

        public void Activate()
        {
            if (freeze_)
                return;
            freeze_ = true;
            Thread thread = new Thread(() =>
            {
                if (content_[0] == 5)
                {
                    sign_ = 1f;
                    do
                    {
                        var_[0, 0] += sign_;
                        Thread.Sleep(12);
                        if (var_[0, 0] >= Map.Size_ / 4)
                            sign_ = -1f;
                    } while (var_[0, 0] >= -Map.Size_);
                    freeze_ = false;
                }
                else if (content_[0] == 4)
                {
                    sign_ = 1f;
                    do
                    {
                        var_[0, 0] += sign_;
                        Thread.Sleep(10);
                        if (var_[0, 0] >= 5f)
                            sign_ = -1f;
                    } while (var_[0, 0] >= -7f);
                    freeze_ = false;
                }
            });
            thread.Start();
        }

        public void Draw(SpriteBatch spriteBatch, int x, int y, int min, int? max = null)
        {
            foreach (int content in content_.Skip(min))
            {
                if (max != null && min > max)
                {
                    return;
                }
                min++;
                switch (content)
                {
                    case -2:
                    case -1: // Default
                    case 0: // Nothing
                        break;
                    case 1: // Wall
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x, y, Map.Size_, Map.Size_), Color.Black);
                        break;
                    case 2: // Light
                        spriteBatch.Draw(Textures.Torch_, new Rectangle(x, y, Map.Size_, Map.Size_), Color.White);
                        break;
                    case 3: // Water
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x, y, Map.Size_, Map.Size_), new Color((int)(var_[1, min - 1] - 128) / 4, (int)(var_[1, min - 1] - 128) / 4, (int)var_[1, min - 1], 128));
                        break;
                    case 4: // Jump pad
                        spriteBatch.Draw(Textures.Circle_, new Rectangle((int)(x - var_[0, 0] / 2), (int)(y - var_[0, 0] / 2), (int)(Map.Size_ + var_[0, 0]), (int)(Map.Size_ + var_[0, 0])), new Color(191, 63, 63));
                        break;
                    case 5: // Gravity pad
                        spriteBatch.Draw(Textures.Circle_, new Rectangle((int)(x - var_[0, 0] / 2), (int)(y - var_[0, 0] / 2), (int)(Map.Size_ + var_[0, 0]), (int)(Map.Size_ + var_[0, 0])), new Color(63, 63, 255));
                        spriteBatch.Draw(Textures.Circle_, new Rectangle((int)(x - (-var_[0, 0] - 3f * Map.Size_ / 4f) / 2), (int)(y - (-var_[0, 0] - 3f * Map.Size_ / 4f) / 2), (int)(Map.Size_ + (-var_[0, 0] - 3f * Map.Size_ / 4f)), (int)(Map.Size_ + (-var_[0, 0] - 3f * Map.Size_ / 4f))), new Color(31, 31, 127));
                        break;
                    case 6: // Next level
                        for (int i = 0; i < 4f; i++)
                            spriteBatch.Draw(Textures.Spiral_, new Rectangle(x + Map.Size_ / 2, y + Map.Size_ / 2, Map.Size_, Map.Size_), null, new Color((int)var_[0, i + 1], (int)(var_[0, i + 1] - 192) / 4, (int)(var_[0, i + 1] - 192) / 4), var_[0, 0] + ((float)i * (float)Math.PI / 2), new Vector2(Textures.Spiral_.Width / 2, Textures.Spiral_.Height / 2), SpriteEffects.None, 0f);
                        break;
                    case 7: // Acid
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x, y, Map.Size_, Map.Size_), new Color((int)(var_[1, min - 1] - 128) / 4, (int)var_[1, min - 1] / 2 + 64, (int)(var_[1, min - 1] - 128) / 4, 128));
                        break;
                    case 8: // Checkpoint
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x + Map.Size_ / 4, y + Map.Size_ / 4, Map.Size_ / 2, Map.Size_ / 2), new Color(255, 128, 64));
                        break;
                    case 9: // Coin
                        spriteBatch.Draw(Textures.Coin_, new Rectangle(x, y, Map.Size_, Map.Size_), Color.White);
                        break;
                    case 10: // Block changer
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x + Map.Size_ / 4, y + Map.Size_ / 4, Map.Size_ / 2, Map.Size_ / 2), new Color(64, 128, 64));
                        break;
                    case 11: // Block type 1
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x, y, Map.Size_, Map.Size_), new Color(255, 0, 0, Map.Block_type_enabled_ == 0 ? 128 : 255));
                        break;
                    case 12: // Block type 2
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x, y, Map.Size_, Map.Size_), new Color(255, 0, 0, Map.Block_type_enabled_ == 1 ? 128 : 255));
                        break;
                    case 13: // Portal 1
                        spriteBatch.Draw(Textures.Portal_, new Rectangle(x + Map.Size_ / 2, y + Map.Size_ / 2, Map.Size_, Map.Size_), null, new Color(0, 0, 255), var_[0, 0], new Vector2(Textures.Portal_.Width / 2, Textures.Portal_.Height / 2), SpriteEffects.None, 0f);
                        break;
                    case 14: // Portal 2
                        spriteBatch.Draw(Textures.Portal_, new Rectangle(x + Map.Size_ / 2, y + Map.Size_ / 2, Map.Size_, Map.Size_), null, new Color(255, 0, 0), var_[0, 0], new Vector2(Textures.Portal_.Width / 2, Textures.Portal_.Height / 2), SpriteEffects.None, 0f);
                        break;
                    case 16: // Laser
                        /* Draw in drawer */
                        break;
                    default: // Unknown
                        spriteBatch.Draw(Textures.Pixel_, new Rectangle(x, y, Map.Size_, Map.Size_), new Color(255, 0, 255));
                        break;
                }
            }
        }
    }
}
