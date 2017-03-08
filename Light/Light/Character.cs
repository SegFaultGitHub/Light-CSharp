using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ArcadeGame
{
    class Character
    {
        private int score_;
        private Vector2 position_;
        public Vector2 Position_
        {
            get { return position_; }
            set { position_ = value; }
        }
        private static int shift_x_, shift_y_;
        public static int Shift_x_
        {
            get { return shift_x_; }
            set { shift_x_ = value; }
        }
        public static int Shift_y_
        {
            get { return shift_y_; }
            set { shift_y_ = value; }
        }
        private Vector2 speed_;
        public Vector2 Speed_
        {
            get { return speed_; }
            set { speed_ = value; }
        }
        private Vector2 speedbis_;
        private float speed_length_;
        public float Speed_length_
        {
            get { return speed_length_; }
        }
        private Vector2 gravity_;
        private Vector2 jump_;
        private float inertia_ = 0.5f;
        private bool grounded_;
        private int max_speed_ = Map.Size_ - 1;
        private int size_ = 15;
        public int Size_
        {
            get { return size_; }
        }
        private int[] content_;
        public int[] Content_
        {
            get { return content_; }
            set { content_ = value; }
        }
        private Color color_, next_color_;
        public Color Color_
        {
            get { return color_; }
            set { color_ = value; }
        }
        private bool force_jump_ = false;
        private static int gravity_sign_ = 1;
        public static int Gravity_sign_
        {
            get { return gravity_sign_; }
            set { gravity_sign_ = value; }
        }
        private Vector2 gravity_air_ = new Vector2(0, 0.25f);
        private Vector2 gravity_water_ = new Vector2(0, 0.05f);
        private Vector2 jump_air_ = new Vector2(0, -5.5f);
        private Vector2 jump_water_ = new Vector2(0, -0.825f);
        private float speed_air_ = 2.5f;
        private float speed_water_ = 1.5f;
        private bool previous_move_;
        private bool freeze_;
        private bool freeze_move_;
        private float shade_;
        private bool death_in_progress_;

        public float Shade_
        {
            get { return shade_; }
            set { shade_ = value; }
        }
        private bool draw_text_;
        private string text_;
        private bool is_focus_;
        public bool Is_focus_
        {
            get { return is_focus_; }
            set { is_focus_ = value; }
        }
        public DateTime last_dash_;
        public TimeSpan dash_delay_ = new TimeSpan(0, 0, 0, 0, 500);
        private List<int> ignored_messages_;
        private int current_message_;

        public Character()
        {
            ignored_messages_ = new List<int>();
            current_message_ = -1;
            int[] content = { 1, 1, 1 };
            content_ = content;
            color_ = Color.Red;
            freeze_ = false;
            draw_text_ = false;
            shade_ = 1f;
            last_dash_ = DateTime.Now;
        }

        public void Freeze(int ms)
        {
            Thread thread = new Thread(() =>
            {
                freeze_ = true;
                Thread.Sleep(ms);
                freeze_ = false;
            });
            thread.Start();
        }

        public void Freeze()
        {
            freeze_move_ = true;
        }

        public void Unfreeze()
        {
            freeze_move_ = false;
        }

        public void Die(int ms, Map map)
        {
            if (death_in_progress_)
                return;
            Thread thread = new Thread(() =>
            {
                death_in_progress_ = true;
                freeze_ = true;
                shade_ = 1f;
                DateTime start = DateTime.Now;
                while (DateTime.Now - start < new TimeSpan(0, 0, 0, 0, ms))
                {
                    shade_ = Math.Max(0, 1 - 2f * (float)(DateTime.Now - start).TotalMilliseconds / (float)ms);
                }
                shade_ = 1f;
                map.ResetLight();
                SetPosition(map, map.Checkpoint_);
                gravity_sign_ = map.Checkpoint_gravity_;
                Map.Block_type_enabled_ = map.Checkpoint_block_;
                Thread.Sleep(ms);
                freeze_ = false;
                death_in_progress_ = false;
            });
            thread.Start();
        }

        public void SetPosition(Map map, Vector2 position, bool keep_inertia = false)
        {
            ignored_messages_ = new List<int>();
            current_message_ = -1;
            position *= Map.Size_;
            position_ = position + new Vector2((Map.Size_ - size_) / 2, Map.Size_ - size_);
            if (!keep_inertia)
            {
                switch (content_[1])
                {
                    case 3:
                        gravity_ = gravity_sign_ * gravity_water_;
                        break;
                    default:
                        gravity_ = gravity_sign_ * gravity_air_;
                        break;
                }
                jump_ = Vector2.Zero;
                speed_ = Vector2.Zero;
                map.Update(this, Drawer.Graphics_.PreferredBackBufferWidth, Drawer.Graphics_.PreferredBackBufferHeight);
                map.Shift_x_ = map.Goal_shift_x_;
                map.Shift_y_ = map.Goal_shift_y_;
            }
        }

        public void Update(Map map, int screenwidth, int screenheight)
        {
            #region Character color
            if (color_ == new Color(255, 0, 0))
            {
                next_color_ = new Color(255, 255, 0);
            }
            else if (color_ == new Color(255, 255, 0))
            {
                next_color_ = new Color(0, 255, 0);
            }
            else if (color_ == new Color(0, 255, 0))
            {
                next_color_ = new Color(0, 255, 255);
            }
            else if (color_ == new Color(0, 255, 255))
            {
                next_color_ = new Color(0, 0, 255);
            }
            else if (color_ == new Color(0, 0, 255))
            {
                next_color_ = new Color(255, 0, 255);
            }
            else if (color_ == new Color(255, 0, 255))
            {
                next_color_ = new Color(255, 0, 0);
            }
            color_.R += (byte)Math.Sign(next_color_.R - color_.R);
            color_.G += (byte)Math.Sign(next_color_.G - color_.G);
            color_.B += (byte)Math.Sign(next_color_.B - color_.B);
            color_.A = (byte)(shade_ * 255f);
            #endregion
            if (!freeze_)
            {
                #region Displacement
                if ((!force_jump_ && (Input.IsPressed(Keys.Up) || Input.IsPressed(Keys.Space))) && !freeze_move_)
                {
                    switch (content_[1])
                    {
                        case 3:
                            gravity_ = Vector2.Zero;
                            Jump(gravity_sign_ * jump_water_);
                            break;
                        default:
                            /*if (grounded_ && Math.Abs(speed_.Y) + Math.Abs(jump_.Y) >= Math.Abs(gravity_.Y))
                                jump_.Y = -5.5f - gravity_.Y / 2;*/
                            if (Jump(gravity_sign_ * jump_air_))
                                Particles.createParticles(this, 25);
                            break;
                    }
                }
                else
                {
                    force_jump_ = false;
                }
                if ((Input.IsPressed(Keys.Right) && !Input.IsPressed(Keys.Left)) && !freeze_move_)
                {
                    if ((Input.IsPressedOnce(Keys.LeftShift) || Input.IsPressedOnce(Keys.RightShift)) && DateTime.Now - last_dash_ >= dash_delay_)
                    {
                        last_dash_ = DateTime.Now;
                        GameHandler.CreateDashCharacter();
                        for (int i = 0; i < 100 && StepMove(map, new Vector2(1, 0)); i++);
                    }
                    else
                    {
                        switch (content_[1])
                        {
                            case 3:
                                if (speed_.X < speed_water_)
                                    speed_.X += inertia_;
                                else if (speed_.X > speed_water_)
                                    speed_.X -= inertia_;
                                break;
                            default:
                                if (speed_.X < speed_air_ / (Input.IsPressed(Keys.RightControl) || Input.IsPressed(Keys.LeftControl) ? 1.5f : 1))
                                    speed_.X += inertia_;
                                else if (speed_.X > speed_air_ / (Input.IsPressed(Keys.RightControl) || Input.IsPressed(Keys.LeftControl) ? 1.5f : 1))
                                    speed_.X -= inertia_;
                                break;
                        }
                    }
                }
                else if ((Input.IsPressed(Keys.Left) && !Input.IsPressed(Keys.Right)) && !freeze_move_)
                {
                    if ((Input.IsPressedOnce(Keys.LeftShift) || Input.IsPressedOnce(Keys.RightShift)) && DateTime.Now - last_dash_ >= dash_delay_)
                    {
                        last_dash_ = DateTime.Now;
                        GameHandler.CreateDashCharacter();
                        for (int i = 0; i < 100 && StepMove(map, new Vector2(-1, 0)); i++) ;
                    }
                    else
                    {
                        switch (content_[1])
                        {
                            case 3:
                                if (speed_.X > -speed_water_)
                                    speed_.X -= inertia_;
                                else if (speed_.X < -speed_water_)
                                    speed_.X += inertia_;
                                break;
                            default:
                                if (speed_.X > -speed_air_ / (Input.IsPressed(Keys.RightControl) || Input.IsPressed(Keys.LeftControl) ? 1.5f : 1))
                                    speed_.X -= inertia_;
                                else if (speed_.X < -speed_air_ / (Input.IsPressed(Keys.RightControl) || Input.IsPressed(Keys.LeftControl) ? 1.5f : 1))
                                    speed_.X += inertia_;
                                break;
                        }
                    }
                }
                else
                {
                    if (speed_.X < 0)
                    {
                        if (speed_.X > -inertia_)
                            speed_.X = 0;
                        else
                            speed_.X += inertia_;
                    }
                    else if (speed_.X > 0)
                    {
                        if (speed_.X < inertia_)
                            speed_.X = 0;
                        else
                            speed_.X -= inertia_;
                    }
                }
                #endregion
                speedbis_ = speed_ + gravity_ + jump_;
                if (speedbis_.Length() > max_speed_)
                {
                    speedbis_.Normalize();
                    speedbis_ *= max_speed_;
                }
                if (content_[1] == 3)
                {
                    gravity_ += gravity_sign_ * gravity_water_;
                }
                else
                {
                    gravity_ += gravity_sign_ * gravity_air_;
                }
                bool move1 = false;
                bool move2 = false;
                try
                {
                    move1 = Move(map, new Vector2(speedbis_.X, 0));
                    move2 = Move(map, new Vector2(0, speedbis_.Y));
                }
                catch
                {
                    Die(500, map);
                }
                speed_length_ = (speed_ + gravity_ + jump_).Length();
                grounded_ = !move2;
                if (previous_move_ && gravity_ == Vector2.Zero) // Fall
                {
                    Particles.createParticles(this, 25);
                }
                previous_move_ = move2;
                if (move1 && !move2 && gravity_ == Vector2.Zero)
                {
                    if (speed_.X > 0)
                    {
                        Particles.createParticles(this, 0, 1);
                    }
                    else if (speed_.X < 0)
                    {
                        Particles.createParticles(this, 0, -1);
                    }
                }
                int x = ((int)position_.X + size_ / 2) / Map.Size_;
                int y = ((int)position_.Y + size_ / 2) / Map.Size_;
                int[] previous_content = content_;
                if (content_ != map.Map_[x, y].Content_)
                {
                    SpecialCells(map, screenwidth, screenheight, x, y, previous_content);
                }
                if (move1 || move2)
                {
                    content_ = map.Map_[(int)(position_.X + size_ / 2) / Map.Size_, (int)(position_.Y + size_ / 2) / Map.Size_].Content_;
                }
                else
                    content_ = map.Map_[(int)(position_.X + size_ / 2) / Map.Size_, (int)(position_.Y + size_ / 2) / Map.Size_].Content_;
            }
            shift_x_ = map.Shift_x_ + map.F_shift_x_;
            shift_y_ = map.Shift_y_ + map.F_shift_y_;
        }

        private void SpecialCells(Map map, int screenwidth, int screenheight, int x, int y, int[] previous_content)
        {
            content_ = map.Map_[x, y].Content_;
            if (previous_content[1] != 3 && content_[1] == 3) // Water
            {
                gravity_ = Vector2.Zero;
                jump_ = Vector2.Zero;
                map.changeViewDistance(4, this, screenwidth, screenheight);
                if (Input.IsPressed(Keys.Up))
                {
                    Jump(gravity_sign_ * new Vector2(0, -3.5f), true);
                    Particles.createParticles(this, 25, new Color(32, 32, 255));
                }
                //else
                //{
                    Sounds.PlaySplash();
                //}
            }
            if (content_[0] == 4) // Jump pad
            {
                Sounds.PlayJumpPad();
                map.Map_[x, y].Activate();
                gravity_ = Vector2.Zero;
                jump_ = Vector2.Zero;
                Jump(gravity_sign_ * new Vector2(0, -7.2f), true);
            }
            if (content_[0] == 5) // Gravity pad
            {
                Sounds.PlayGravityPad();
                map.Map_[x, y].Activate();
                gravity_sign_ *= -1;
                jump_ = Vector2.Zero;
                switch (content_[1])
                {
                    case 3:
                        gravity_ = gravity_sign_ * gravity_water_;
                        break;
                    default:
                        gravity_ = gravity_sign_ * gravity_air_;
                        break;
                }
            }
            if (content_[0] == 6)
            {
                GameHandler.SetMap(map.Next_map_, true);
            }
            if (content_[0] == 10)
            {
                Thread thread = new Thread(() =>
                {
                    text_ = " Blocks\n" + 
                            "switched";
                    draw_text_ = true;
                    Thread.Sleep(1500);
                    draw_text_ = false;
                });
                thread.Start();
                Map.Block_type_enabled_++;
                Map.Block_type_enabled_ %= 2;
            }
            if (previous_content[1] != 7 && content_[1] == 7)
            {
                gravity_sign_ = map.Initial_gravity_;
                Die(500, map);
                Drawer.Fade(500);
            }
            if (content_[0] == 8 && (map.Checkpoint_ != new Vector2(x, y) || map.Checkpoint_gravity_ != gravity_sign_))
            {
                Thread thread = new Thread(() =>
                {
                    Console.WriteLine("Checkpoint set");
                    text_ = "Checkpoint set";
                    draw_text_ = true;
                    Thread.Sleep(1500);
                    draw_text_ = false;
                });
                thread.Start();
                map.SetCheckpoint(new Vector2(x, y), gravity_sign_);
            }
            if (content_[0] == 9)
            {
                score_++;
                int[] new_content = { -1, map.Map_[x, y].Content_[1], map.Map_[x, y].Content_[2] };
                map.Map_[x, y] = new Cell(new_content);
            }
            if (content_[0] == 13)
            {
                SetPosition(map, map.GetNearestPortal1(new Vector2(x, y)), true);
                gravity_ = Vector2.Zero;
                jump_ = Vector2.Zero;
                Jump(new Vector2(0, -speedbis_.Y), true);
            }
            if (content_[0] == 14)
            {
                SetPosition(map, map.GetNearestPortal2(new Vector2(x, y)), true);
                gravity_ = Vector2.Zero;
                jump_ = Vector2.Zero;
                Jump(new Vector2(0, speedbis_.Y), true);
            }
            if (content_[0] == 15)
            {
                map.Map_[x, y].Activate();
                gravity_sign_ *= -1;
                jump_ = Vector2.Zero;
                switch (content_[1])
                {
                    case 3:
                        gravity_ = gravity_sign_ * gravity_water_;
                        break;
                    default:
                        gravity_ = gravity_sign_ * gravity_air_;
                        break;
                }
                gravity_ = Vector2.Zero;
                jump_ = Vector2.Zero;
                Jump(gravity_sign_ * new Vector2(0, -7f), true);
            }
            if (previous_content[1] != content_[1] && content_[1] != 3)
            {
                /* Normal distance : 6 */
                map.changeViewDistance(6, this, screenwidth, screenheight);
            }
            if (content_[2] == -2)
            {
                int index = map.Map_[x, y].Message_index_;
                if (!ignored_messages_.Contains(index))
                {
                    if (current_message_ != index)
                    {
                        current_message_ = index;
                        if (index != -1)
                            Message.Display(map.Messages_[index], false);
                    }
                }
            }
            else if (previous_content[2] == -2)
            {
                ignored_messages_.Add(current_message_);
                Message.Close();
            }
        }

        public bool Jump(Vector2 vector, bool force_jump = false)
        {
            force_jump_ = force_jump;
            if (force_jump)
                gravity_ = Vector2.Zero;
            if (gravity_ == Vector2.Zero)
            {
                jump_ = vector;
                return true;
            }
            return false;
            /*else if (jumps_ < max_jumps_)
            {
                jump_ = vector;
                gravity_ = Vector2.Zero;
                jumps_++;
            }*/
        }

        public bool Move(Map map, Vector2 speed)
        {
            if (speed == Vector2.Zero)
                return false;
            Vector2 postMovePosition = position_ + speed;
            Vector2 cornerUL = postMovePosition;
            Vector2 cornerUR = postMovePosition + new Vector2(size_ - 1, 0);
            Vector2 cornerDL = postMovePosition + new Vector2(0, size_ - 1);
            Vector2 cornerDR = postMovePosition + new Vector2(size_ - 1, size_ - 1);
            bool collision = false;
            int i, j;
            #region Move left
            i = (int)cornerUL.X / Map.Size_;
            for (j = (int)cornerUL.Y / Map.Size_; j <= (int)cornerDL.Y / Map.Size_; j++)
            {
                if (!map.Map_[i, j].Walkable())
                {
                    collision = true;
                }
            }
            #endregion
            #region Move right
            i = (int)cornerUR.X / Map.Size_;
            for (j = (int)cornerUR.Y / Map.Size_; j <= (int)cornerDR.Y / Map.Size_; j++)
            {
                if (!map.Map_[i, j].Walkable())
                {
                    collision = true;
                }
            }
            #endregion
            #region Move up
            j = (int)cornerUL.Y / Map.Size_;
            for (i = (int)cornerUL.X / Map.Size_; i <= (int)cornerUR.X / Map.Size_; i++)
            {
                if (!map.Map_[i, j].Walkable())
                {
                    collision = true;
                }
            }
            #endregion
            #region Move down
            j = (int)cornerDL.Y / Map.Size_;
            for (i = (int)cornerDL.X / Map.Size_; i <= (int)cornerDR.X / Map.Size_; i++)
            {
                if (!map.Map_[i, j].Walkable())
                {
                    collision = true;
                }
            }
            #endregion
            if (collision)
            {
                while (StepMove(map, new Vector2(Math.Sign(speed.X), 0))) ;
                while (StepMove(map, new Vector2(0, Math.Sign(speed.Y)))) ;
                return false;
            }
            position_ = postMovePosition;
            return true;
        }

        public bool StepMove(Map map, Vector2 speed)
        {
            if (speed == Vector2.Zero)
                return false;
            Vector2 postMovePosition = position_ + speed;
            Vector2 cornerUL = postMovePosition;
            Vector2 cornerUR = postMovePosition + new Vector2(size_ - 1, 0);
            Vector2 cornerDL = postMovePosition + new Vector2(0, size_ - 1);
            Vector2 cornerDR = postMovePosition + new Vector2(size_ - 1, size_ - 1);
            int i, j;
            bool result = true;
            #region Move left
            i = (int)cornerUL.X / Map.Size_;
            for (j = (int)cornerUL.Y / Map.Size_; j <= (int)cornerDL.Y / Map.Size_; j++)
            {
                if (!map.Map_[i, j].Walkable())
                {
                    if (speed.Y != 0)
                    {
                        if (gravity_sign_ * speed.Y > 0)
                        {
                            jump_ = Vector2.Zero;
                            jump_ = -gravity_ / 10;
                            gravity_ = Vector2.Zero;
                        }
                        else
                        {
                            jump_ = -gravity_;
                        }
                    }
                    else if (speed_.X != 0)
                        speed_ = Vector2.Zero;
                    result = false;
                }
            }
            #endregion
            #region Move right
            i = (int)cornerUR.X / Map.Size_;
            for (j = (int)cornerUR.Y / Map.Size_; j <= (int)cornerDR.Y / Map.Size_; j++)
            {
                if (!map.Map_[i, j].Walkable())
                {
                    if (speed.Y != 0)
                    {
                        if (gravity_sign_ * speed.Y > 0)
                        {
                            jump_ = Vector2.Zero;
                            jump_ = -gravity_ / 10;
                            gravity_ = Vector2.Zero;
                        }
                        else
                        {
                            jump_ = -gravity_;
                        }
                    }
                    else if (speed_.X != 0)
                        speed_ = Vector2.Zero;
                    result = false;
                }
            }
            #endregion
            #region Move up
            j = (int)cornerUL.Y / Map.Size_;
            for (i = (int)cornerUL.X / Map.Size_; i <= (int)cornerUR.X / Map.Size_; i++)
            {
                if (!map.Map_[i, j].Walkable())
                {
                    if (speed.Y != 0)
                    {
                        if (gravity_sign_ * speed.Y > 0)
                        {
                            jump_ = Vector2.Zero;
                            jump_ = -gravity_ / 10;
                            gravity_ = Vector2.Zero;
                        }
                        else
                        {
                            jump_ = -gravity_;
                        }
                    }
                    else if (speed_.X != 0)
                        speed_ = Vector2.Zero;
                    result = false;
                }
            }
            #endregion
            #region Move down
            j = (int)cornerDL.Y / Map.Size_;
            for (i = (int)cornerDL.X / Map.Size_; i <= (int)cornerDR.X / Map.Size_; i++)
            {
                if (!map.Map_[i, j].Walkable())
                {
                    if (speed.Y != 0)
                    {
                        if (gravity_sign_ * speed.Y > 0)
                        {
                            jump_ = Vector2.Zero;
                            jump_ = -gravity_ / 10;
                            gravity_ = Vector2.Zero;
                        }
                        else
                        {
                            jump_ = -gravity_;
                        }
                    }
                    else if (speed_.X != 0)
                        speed_ = Vector2.Zero;
                    result = false;
                }
            }
            #endregion
            if (!result)
                return false;
            position_ = postMovePosition;
            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)position_.X - shift_x_, (int)position_.Y - shift_y_, size_, size_), color_);
            spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)position_.X - shift_x_, (int)position_.Y - shift_y_ + 3, 3, size_ - 3), new Color(0f, 0f, 0f, shade_));
            spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)position_.X - shift_x_, (int)position_.Y - shift_y_, size_, 3), new Color(0f, 0f, 0f, shade_));
            spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)position_.X - shift_x_ + size_ - 3, (int)position_.Y - shift_y_ + 3, 3, size_ - 6), new Color(0f, 0f, 0f, shade_));
            spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)position_.X - shift_x_ + 3, (int)position_.Y - shift_y_ + size_ - 3, size_ - 3, 3), new Color(0f, 0f, 0f, shade_));
            if (draw_text_)
            {
                Vector2 size = Textures.Font_.MeasureString(text_);
                spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)position_.X - (int)(shift_x_ - size_ / 2 + size.X / 2) - 2, (int)position_.Y - (int)(shift_y_ - size_ / 2 + size.Y / 2) - 2, (int)size.X + 4, (int)size.Y + 4), new Color(0, 0, 0, 0.5f));
                spriteBatch.DrawString(Textures.Font_, text_, position_ - new Vector2(shift_x_ - size_ / 2 + size.X / 2, shift_y_ - size_ / 2 + size.Y / 2), Color.White);
            }
        }
    }
}
