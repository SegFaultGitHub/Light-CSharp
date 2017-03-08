using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ArcadeGame
{
    class Map
    {
        private bool tutorial_;
        public bool Tutorial_
        {
            get { return tutorial_; }
            set { tutorial_ = value; }
        }
        private bool official_;
        public bool Official_
        {
            get { return official_; }
            set { official_ = value; }
        }
        private bool read_only_;
        public bool Read_only_
        {
            get { return read_only_; }
            set { read_only_ = value; }
        }
        private string name_;
        public string Name_
        {
            get { return name_; }
            set { name_ = value; }
        }
        private string next_map_;
        public string Next_map_
        {
            get { return next_map_; }
        }
        private Vector2 initial_position_;
        public Vector2 Initial_position_
        {
            get { return initial_position_; }
        }
        private int checkpoint_gravity_;
        public int Checkpoint_gravity_
        {
            get { return checkpoint_gravity_; }
            set { checkpoint_gravity_ = value; }
        }
        private int checkpoint_block_;
        public int Checkpoint_block_
        {
            get { return checkpoint_block_; }
            set { checkpoint_block_ = value; }
        }
        private Vector2 checkpoint_;
        public Vector2 Checkpoint_
        {
            get { return checkpoint_; }
            set { checkpoint_ = value; }
        }
        private int initial_gravity_;
        public int Initial_gravity_
        {
            get { return initial_gravity_; }
        }
        private static int block_type_enabled_ = 1; // 1 || 0
        public static int Block_type_enabled_
        {
            get { return block_type_enabled_; }
            set { block_type_enabled_ = value; }
        }
        private static int size_ = 20;
        public static int Size_
        {
            get { return Map.size_; }
            set { Map.size_ = value; }
        }

        private int light_power_ = 4;
        /* Normal distance: 6 */
        private int view_distance_ = 6;
        private int width_;
        private int height_;
        public int Width_
        {
            get { return width_; }
        }
        public int Height_
        {
            get { return height_; }
        }

        private Cell[,] map_;
        public Cell[,] Map_
        {
            get { return map_; }
        }
        private double[,] shadows_; /* 1: full light, 0: full shadow */
        private double[,] shadows_smooth_; /* 1: full light, 0: full shadow */
        public double[,] Shadows_smooth_
        {
            get { return shadows_smooth_; }
        }
        private double[,] shadows_smooth_goal_;
        private List<Vector2> lights_;
        /*private Dictionary<Vector2, double[,]>[] shadows_lights_;
        private Dictionary<Vector2, double[,]>[] Shadows_lights_
        {
            set { shadows_lights_ = value; }
        }*/
        private List<Vector2> portals1_, portals2_;
        private List<Vector2> moving_cells_;
        private Random rand_;
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
        private int goal_shift_x_, goal_shift_y_;
        public int Goal_shift_x_
        {
            get { return goal_shift_x_; }
            set { goal_shift_x_ = value; }
        }
        public int Goal_shift_y_
        {
            get { return goal_shift_y_; }
            set { goal_shift_y_ = value; }
        }
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
        private static int reduce_speed_ = 10;
        private int test = 0;

        private Dictionary<int, string> messages_;
        public Dictionary<int, string> Messages_
        {
            get { return messages_; }
            set { messages_ = value; }
        }

        public Map(string name, int width, int height, Cell[,] map, Vector2 position, int gravity, string next_map, bool read_only, bool tutorial, bool official, Dictionary<int, string> messages)
        {
            messages_ = messages;
            tutorial_ = tutorial;
            official_ = official;
            read_only_ = read_only;
            name_ = name;
            moving_cells_ = new List<Vector2>();
            next_map_ = next_map;
            initial_gravity_ = gravity;
            initial_position_ = position;
            checkpoint_ = initial_position_;
            map_ = map;
            width_ = width;
            height_ = height;
            rand_ = new Random();
            shadows_ = new double[width_, height_];
            shadows_smooth_ = new double[width_, height_];
            shadows_smooth_goal_ = new double[width_, height_];
            lights_ = new List<Vector2>();
            //shadows_lights_ = new Dictionary<Vector2, double[,]>[2];
            portals1_ = new List<Vector2>();
            portals2_ = new List<Vector2>();
            //for (int b = 0; b <= 1; b++)
            {
                //shadows_lights_[b] = new Dictionary<Vector2, double[,]>();
                //Map.Block_type_enabled_ = b;
                for (int i = 0; i < width_; i++)
                {
                    for (int j = 0; j < height_; j++)
                    {
                        /*if (map[i, j].Light_source_)
                        {
                            lights_.Add(new Vector2(i, j));
                            //shadows_lights_[b][new Vector2(i, j)] = null;
                        }*/
                        if (map[i, j].Moving_)
                        {
                            moving_cells_.Add(new Vector2(i, j));
                        }
                        if (map[i, j].Content_[0] == 13)
                        {
                            portals1_.Add(new Vector2(i, j));
                        }
                        if (map[i, j].Content_[0] == 14)
                        {
                            portals2_.Add(new Vector2(i, j));
                        }
                        if (map[i, j].Content_[0] == 16)
                        {
                            bool left = !(i - 1 >= 0 && !map[i - 1, j].Walkable_);
                            bool right = !(i + 1 < width && !map[i + 1, j].Walkable_);
                            bool up = !(j - 1 >= 0 && !map[i, j - 1].Walkable_);
                            bool down = !(j + 1 < height && !map[i, j + 1].Walkable_);
                            bool[] bools = { right, up, left, down, right, up, left, down };
                            List<List<int>> sequences = new List<List<int>>();
                            List<int> current = new List<int>();
                            for (int ii = 0; ii < bools.Length; ii++)
                            {
                                if (!bools[ii])
                                {
                                    sequences.Add(new List<int>(current));
                                    current.Clear();
                                }
                                else
                                {
                                    current.Add(ii);
                                }
                            }
                            sequences.Add(current);
                            List<int> longuest = new List<int>();;
                            foreach (List<int> list in sequences)
                            {
                                if (list.Count > longuest.Count)
                                    longuest = list;
                            }
                            float start = 0f;
                            float angle = 0f;
                            if (longuest.Count != 0)
                            {
                                start = longuest[0] * 90f;
                                angle = Math.Min(360f, longuest.Count * 90f - 90f);
                            }
                            map[i, j].Var_[0, 1] = start;
                            map[i, j].Var_[0, 3] = angle;
                        }
                    }
                }
                /*for (int k = 0; k < shadows_lights_[b].Count(); k++)
                {
                    Vector2 source = shadows_lights_[b].ElementAt(k).Key;
                    for (int i = Math.Max(0, (int)source.X - light_power_); i < (int)source.X + light_power_ && i < width; i++)
                    {
                        for (int j = Math.Max(0, (int)source.Y - light_power_); j < (int)source.Y + light_power_ && j < height; j++)
                        {
                            if (!map_[i, j].Opaque())
                                checkLuminosity(i, j, shadows_lights_[b].ElementAt(k).Key.X * size_ + size_ / 2, shadows_lights_[b].ElementAt(k).Key.Y * size_ + size_ / 2, light_power_);
                        }
                    }
                    shadows_lights_[b][shadows_lights_[b].ElementAt(k).Key] = (double[,])shadows_smooth_goal_.Clone();
                    shadows_ = new double[width_, height_];
                    shadows_smooth_goal_ = new double[width_, height_];
                }*/
            }
            checkpoint_block_ = Map.Block_type_enabled_;
            block_type_enabled_ = 1;
        }

        public void SetCheckpoint(Vector2 position, int gravity)
        {
            checkpoint_ = position;
            checkpoint_gravity_ = gravity;
            checkpoint_block_ = Map.Block_type_enabled_;
        }

        public static void EnableHD()
        {
            reduce_speed_ = 10;
        }

        public static void DisableHD()
        {
            reduce_speed_ = 1;
        }

        public Vector2 GetNearestPortal1(Vector2 vector)
        {
            Vector2 result = Vector2.Zero;
            int min_distance = Int32.MaxValue;
            foreach (Vector2 v in portals1_)
            {
                int distance = (int)(Math.Max(v.X, vector.X) - Math.Min(v.X, vector.X)) + (int)(Math.Max(v.Y, vector.Y) - Math.Min(v.Y, vector.Y));
                if (distance != 0 && distance <= min_distance)
                {
                    min_distance = distance;
                    result = v;
                }
            }
            if (result == Vector2.Zero)
                return vector;
            return result;
        }

        public Vector2 GetNearestPortal2(Vector2 vector)
        {
            Vector2 result = Vector2.Zero;
            float min_distance = Int32.MaxValue;
            foreach (Vector2 v in portals2_)
            {
                float distance = Vector2.Distance(v, vector);
                if (distance != 0 && distance <= min_distance)
                {
                    min_distance = distance;
                    result = v;
                }
            }
            if (result == Vector2.Zero)
                return vector;
            return result;
        }

        public void checkLuminosity(float x_cell, float y_cell, float x_source, float y_source, float view_distance)
        {
            if (x_cell < 0 || x_cell >= width_ || y_cell < 0 || y_cell >= height_)
                return;
            int x = (int)x_cell;
            int y = (int)y_cell;
            double distance = Math.Sqrt(Math.Pow(x_cell - x_source / size_, 2) + Math.Pow(y_cell - y_source / size_, 2));
            double power = Math.Max(0, (view_distance - distance) / view_distance);
            if (shadows_smooth_goal_[x, y] >= 1f)
                return;
            //bool opaque = map_[x, y].Opaque();
            //map_[x, y].Opaque_ = false;
            x_cell *= size_;
            y_cell *= size_;
            float init_x_cell = x_cell;
            float init_y_cell = y_cell;
            Vector2 vector;
            int max_collisions = 0;
            #region Test direct lines
            if ((int)x_cell / size_ == (int)x_source / size_ && (int)y_cell / size_ == (int)y_source / size_)
            {
                shadows_[x, y] = 0;
            }
            else if ((int)x_cell / size_ == (int)x_source / size_)
            {
                if ((int)y_cell / size_ > (int)y_source / size_)
                    for (int j = (int)y_cell / size_ - 1; j != (int)y_source / size_; j--)
                    {
                        if (map_[(int)x_cell / size_, j].Opaque())
                        {
                            max_collisions = 3;
                            break;
                        }
                    }
                else
                    for (int j = (int)y_cell / size_ + 1; j != (int)y_source / size_; j++)
                    {
                        if (map_[(int)x_cell / size_, j].Opaque())
                        {
                            max_collisions = 3;
                            break;
                        }
                    }
            }
            else if ((int)y_cell / size_ == (int)y_source / size_)
            {
                if ((int)x_cell / size_ > (int)x_source / size_)
                    for (int i = (int)x_cell / size_ - 1; i != (int)x_source / size_; i--)
                    {
                        if (map_[i, (int)y_cell / size_].Opaque())
                        {
                            max_collisions = 3;
                            break;
                        }
                    }
                else
                    for (int i = (int)x_cell / size_ + 1; i != (int)x_source / size_; i++)
                    {
                        if (map_[i, (int)y_cell / size_].Opaque())
                        {
                            max_collisions = 3;
                            break;
                        }
                    }
            }
            #endregion
            #region Test diagonals
            else if (x_cell / size_ > x_source / size_) /* Test left */
            {
                if (y_cell / size_ > y_source / size_) /* Test up */
                {
                    x_cell = init_x_cell;
                    y_cell = init_y_cell;
                    vector = new Vector2(x_source - x_cell, y_source - y_cell);
                    vector.Normalize();
                    while (x_cell > x_source || y_cell > y_source)
                    {
                        if (map_[x, y].Opaque())
                        {
                            if (((int)x_cell / size_ != x || (int)y_cell / size_ != y) && map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                            {
                                max_collisions++;
                                break;
                            }
                        }
                        else if (map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                        {
                            max_collisions++;
                            break;
                        }
                        x_cell += vector.X;
                        y_cell += vector.Y;
                    }
                    x_cell = init_x_cell + (size_ - 1);
                    y_cell = init_y_cell;
                    vector = new Vector2(x_source - x_cell, y_source - y_cell);
                    vector.Normalize();
                    while (x_cell > x_source || y_cell > y_source)
                    {
                        if (map_[x, y].Opaque())
                        {
                            if (((int)x_cell / size_ != x || (int)y_cell / size_ != y) && map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                            {
                                max_collisions++;
                                break;
                            }
                        }
                        else if (map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                        {
                            max_collisions++;
                            break;
                        }
                        x_cell += vector.X;
                        y_cell += vector.Y;
                    }
                    x_cell = init_x_cell;
                    y_cell = init_y_cell + (size_ - 1);
                    vector = new Vector2(x_source - x_cell, y_source - y_cell);
                    vector.Normalize();
                    while (x_cell > x_source || y_cell > y_source)
                    {
                        if (map_[x, y].Opaque())
                        {
                            if (((int)x_cell / size_ != x || (int)y_cell / size_ != y) && map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                            {
                                max_collisions++;
                                break;
                            }
                        }
                        else if (map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                        {
                            max_collisions++;
                            break;
                        }
                        x_cell += vector.X;
                        y_cell += vector.Y;
                    }
                }
                else if (y_cell / size_ < y_source / size_) /* Test down */
                {
                    vector = new Vector2(x_source - x_cell, y_source - y_cell);
                    vector.Normalize();
                    while (x_cell >= x_source || y_cell <= y_source)
                    {
                        if (map_[x, y].Opaque())
                        {
                            if (((int)x_cell / size_ != x || (int)y_cell / size_ != y) && map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                            {
                                max_collisions++;
                                break;
                            }
                        }
                        else if (map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                        {
                            max_collisions++;
                            break;
                        }
                        x_cell += vector.X;
                        y_cell += vector.Y;
                    }
                    x_cell = init_x_cell;
                    y_cell = init_y_cell + (size_ - 1);
                    vector = new Vector2(x_source - x_cell, y_source - y_cell);
                    vector.Normalize();
                    while (x_cell >= x_source || y_cell <= y_source)
                    {
                        if (map_[x, y].Opaque())
                        {
                            if (((int)x_cell / size_ != x || (int)y_cell / size_ != y) && map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                            {
                                max_collisions++;
                                break;
                            }
                        }
                        else if (map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                        {
                            max_collisions++;
                            break;
                        }
                        x_cell += vector.X;
                        y_cell += vector.Y;
                    }
                    x_cell = init_x_cell + (size_ - 1);
                    y_cell = init_y_cell + (size_ - 1);
                    vector = new Vector2(x_source - x_cell, y_source - y_cell);
                    vector.Normalize();
                    while (x_cell >= x_source || y_cell <= y_source)
                    {
                        if (map_[x, y].Opaque())
                        {
                            if (((int)x_cell / size_ != x || (int)y_cell / size_ != y) && map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                            {
                                max_collisions++;
                                break;
                            }
                        }
                        else if (map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                        {
                            max_collisions++;
                            break;
                        }
                        x_cell += vector.X;
                        y_cell += vector.Y;
                    }
                }
            }
            else if (x_cell / size_ < x_source / size_) /* Test right */
            {
                if (y_cell / size_ > y_source / size_) /* Test up */
                {
                    x_cell = init_x_cell + (size_ - 1);
                    y_cell = init_y_cell;
                    vector = new Vector2(x_source - x_cell, y_source - y_cell);
                    vector.Normalize();
                    while (x_cell <= x_source || y_cell >= y_source)
                    {
                        if (map_[x, y].Opaque())
                        {
                            if (((int)x_cell / size_ != x || (int)y_cell / size_ != y) && map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                            {
                                max_collisions++;
                                break;
                            }
                        }
                        else if (map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                        {
                            max_collisions++;
                            break;
                        }
                        x_cell += vector.X;
                        y_cell += vector.Y;
                    }
                    x_cell = init_x_cell + (size_ - 1);
                    y_cell = init_y_cell + (size_ - 1);
                    vector = new Vector2(x_source - x_cell, y_source - y_cell);
                    vector.Normalize();
                    while (x_cell <= x_source || y_cell >= y_source)
                    {
                        if (map_[x, y].Opaque())
                        {
                            if (((int)x_cell / size_ != x || (int)y_cell / size_ != y) && map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                            {
                                max_collisions++;
                                break;
                            }
                        }
                        else if (map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                        {
                            max_collisions++;
                            break;
                        }
                        x_cell += vector.X;
                        y_cell += vector.Y;
                    }
                    x_cell = init_x_cell;
                    y_cell = init_y_cell + (size_ - 1);
                    vector = new Vector2(x_source - x_cell, y_source - y_cell);
                    vector.Normalize();
                    while (x_cell <= x_source || y_cell >= y_source)
                    {
                        if (map_[x, y].Opaque())
                        {
                            if (((int)x_cell / size_ != x || (int)y_cell / size_ != y) && map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                            {
                                max_collisions++;
                                break;
                            }
                        }
                        else if (map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                        {
                            max_collisions++;
                            break;
                        }
                        x_cell += vector.X;
                        y_cell += vector.Y;
                    }
                }
                else if (y_cell / size_ < y_source / size_) /* Test down */
                {
                    x_cell = init_x_cell + (size_ - 1);
                    y_cell = init_y_cell;
                    vector = new Vector2(x_source - x_cell, y_source - y_cell);
                    vector.Normalize();
                    while (x_cell <= x_source || y_cell <= y_source)
                    {
                        if (map_[x, y].Opaque())
                        {
                            if (((int)x_cell / size_ != x || (int)y_cell / size_ != y) && map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                            {
                                max_collisions++;
                                break;
                            }
                        }
                        else if (map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                        {
                            max_collisions++;
                            break;
                        }
                        x_cell += vector.X;
                        y_cell += vector.Y;
                    }
                    x_cell = init_x_cell + (size_ - 1);
                    y_cell = init_y_cell + (size_ - 1);
                    vector = new Vector2(x_source - x_cell, y_source - y_cell);
                    vector.Normalize();
                    while (x_cell <= x_source || y_cell <= y_source)
                    {
                        if (map_[x, y].Opaque())
                        {
                            if (((int)x_cell / size_ != x || (int)y_cell / size_ != y) && map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                            {
                                max_collisions++;
                                break;
                            }
                        }
                        else if (map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                        {
                            max_collisions++;
                            break;
                        }
                        x_cell += vector.X;
                        y_cell += vector.Y;
                    }
                    x_cell = init_x_cell;
                    y_cell = init_y_cell;
                    vector = new Vector2(x_source - x_cell, y_source - y_cell);
                    vector.Normalize();
                    while (x_cell <= x_source || y_cell <= y_source)
                    {
                        if (map_[x, y].Opaque())
                        {
                            if (((int)x_cell / size_ != x || (int)y_cell / size_ != y) && map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                            {
                                max_collisions++;
                                break;
                            }
                        }
                        else if (map_[(int)x_cell / size_, (int)y_cell / size_].Opaque())
                        {
                            max_collisions++;
                            break;
                        }
                        x_cell += vector.X;
                        y_cell += vector.Y;
                    }
                }
            }
            #endregion
            shadows_[x, y] += (3 - Math.Min(3, max_collisions)) / 3f;
            x_cell = x + 0.5f;
            y_cell = y + 0.5f;
            shadows_smooth_goal_[(int)(x_cell - 0.5f), (int)(y_cell - 0.5f)] += power * shadows_[(int)(x_cell - 0.5f), (int)(y_cell - 0.5f)];
            //map_[x, y].Opaque_ = opaque;
        }

        public void changeViewDistance(int view_distance, Character character, int screenwidth, int screenheight)
        {
            view_distance_ = view_distance;
            UpdateLuminosity(character, screenwidth, screenheight);
        }

        public void ResetLight()
        {
            shadows_smooth_goal_ = new double[width_, height_];
            shadows_ = new double[width_, height_];
            shadows_smooth_ = new double[width_, height_];

        }

        public void UpdateLuminosity(Character character, int screenwidth, int screenheight)
        {
            shadows_smooth_goal_ = new double[width_, height_];
            shadows_ = new double[width_, height_];
            lights_.Clear();
            for (int j = Math.Max(0, shift_y_ / size_ - light_power_); j <= shift_y_ / size_ + screenheight / size_ + 1 + light_power_ && j < height_; j++)
            {
                for (int i = Math.Max(0, shift_x_ / size_ - light_power_); i <= shift_x_ / size_ + screenwidth / size_ + 1 + light_power_ && i < width_; i++)
                {
                    float distance = Vector2.Distance(new Vector2(character.Position_.X + character.Size_ / 2, character.Position_.Y + character.Size_ / 2) / size_, new Vector2(i, j));
                    //if (distance <= view_distance_ || shadows_lights_[Map.Block_type_enabled_].Keys.Contains(new Vector2(i, j)))
                    if (distance <= view_distance_ || map_[i, j].Light_source_)//lights_.Contains(new Vector2(i, j)))
                    {
                        checkLuminosity(i, j, character.Position_.X + character.Size_ / 2, character.Position_.Y + character.Size_ / 2, view_distance_);
                        if (map_[i, j].Light_source_ && shadows_[i, j] >= 1f)
                            lights_.Add(new Vector2(i, j));
                    }
                }
            }
            if (character.Content_[1] != 3)
            {
                //foreach (Vector2 vector in shadows_lights_[Map.Block_type_enabled_].Keys)
                for (int k = 0; k < lights_.Count; k++)
                {
                    Vector2 vector = lights_[k];
                    shadows_smooth_goal_[(int)vector.X, (int)vector.Y] = 1f;
                    for (int j = Math.Max(0, (int)vector.Y - light_power_ + 1); j <= (int)vector.Y + light_power_ - 1 && j < height_; j++)
                    {
                        for (int i = Math.Max(0, (int)vector.X - light_power_ + 1); i <= (int)vector.X + light_power_ - 1 && i < width_; i++)
                        {
                            checkLuminosity(i, j, size_ * vector.X, size_ * vector.Y, light_power_);
                        }
                    }
                    /*for (int j = shift_y_ / size_; j <= shift_y_ / size_ + screenheight / size_ + 1 && j < height_; j++)
                    {
                        for (int i = shift_x_ / size_; i <= shift_x_ / size_ + screenwidth / size_ + 1 && i < width_; i++)
                        {
                            shadows_smooth_goal_[i, j] += shadows_lights_[Map.Block_type_enabled_][vector][i, j];
                        }
                    }*/
                }
            }
        }

        public void Update(Character character, int screenwidth, int screenheight)
        {
            #region Shift
            int char_x, char_y;
            char_x = (int)character.Position_.X + (int)(character.Size_ / 2);
            char_y = (int)character.Position_.Y + (int)(character.Size_ / 2);
            if (width_ * Map.Size_ < screenwidth)
            {
                f_shift_x_ = -(screenwidth - width_ * Map.Size_) / 2;
            }
            else
            {
                if (char_x > screenwidth / 2 && char_x < width_ * size_ - screenwidth / 2)
                    goal_shift_x_ = char_x - screenwidth / 2;
                else if (char_x <= screenwidth / 2)
                    goal_shift_x_ = 0;
                else
                    goal_shift_x_ = width_ * size_ - screenwidth;
            }
            if (height_ * Map.Size_ < screenheight)
            {
                f_shift_y_ = -(screenheight - height_ * Map.Size_) / 2;
            }
            else
            {
                if (char_y > screenheight / 2 && char_y < height_ * size_ - screenheight / 2)
                    goal_shift_y_ = char_y - screenheight / 2;
                else if (char_y <= screenheight / 2)
                    goal_shift_y_ = 0;
                else
                    goal_shift_y_ = height_ * size_ - screenheight;
            }
            int coeff = (int)(Math.Sqrt(new Vector2(goal_shift_x_ - shift_x_, goal_shift_y_ - shift_y_).Length()));
            if (shift_x_ != goal_shift_x_)
                shift_x_ += coeff * Math.Sign(goal_shift_x_ - shift_x_);
            if (shift_y_ != goal_shift_y_)
                shift_y_ += coeff * Math.Sign(goal_shift_y_ - shift_y_);
            if (Math.Abs(goal_shift_x_ - shift_x_) < coeff)
                shift_x_ = goal_shift_x_;
            if (Math.Abs(goal_shift_y_ - shift_y_) < coeff)
                shift_y_ = goal_shift_y_;
            #endregion
            //if (test == 1)
            //{
            //    for (int i = 0; i < moving_cells_.Count; i++)
            //    {
            //        Vector2 vect = moving_cells_[i];
            //        if (map_[(int)vect.X, (int)vect.Y].Content_[0] == 10)
            //        {
            //            if (vect.X == 18)
            //            { 
            //                ;
            //            }
            //            if (map_[(int)vect.X + map_[(int)vect.X, (int)vect.Y].Direction_, (int)vect.Y].Content_[0] != 0)
            //            {
            //                Cell cell = map_[(int)vect.X, (int)vect.Y];
            //                map_[(int)vect.X, (int)vect.Y] = map_[(int)vect.X + map_[(int)vect.X, (int)vect.Y].Direction_, (int)vect.Y];
            //                map_[(int)vect.X + map_[(int)vect.X, (int)vect.Y].Direction_, (int)vect.Y] = cell;
            //                moving_cells_[i] = new Vector2((int)vect.X + map_[(int)vect.X, (int)vect.Y].Direction_, (int)vect.Y);
            //            }
            //            else
            //            {
            //                map_[(int)vect.X, (int)vect.Y].Direction_ *= -1;
            //            }
            //        }
            //    }
            //}
            //test++;
            //test %= 20;

            UpdateLuminosity(character, screenwidth, screenheight);

            for (int i = 0; i < width_; i++)
                for (int j = 0; j < height_; j++)
                {
                    map_[i, j].Update(i, j);
                    if (map_[i, j].Content_[0] == 16)
                    {
                        Vector2 direction = new Vector2((float)Math.Cos(map_[i, j].Var_[0, 1] / 180f * Math.PI), (float)Math.Sin(-map_[i, j].Var_[0, 1] / 180f * Math.PI));
                        direction.Normalize();
                        Vector2 current_pos = new Vector2(i, j) * size_ + new Vector2(size_) / 2;
                        while (current_pos.X >= 0 && current_pos.X < width_ * size_ && current_pos.Y >= 0 && current_pos.Y < height_ * size_)
                        {
                            if (current_pos.X >= character.Position_.X && current_pos.X < character.Position_.X + character.Size_
                                && current_pos.Y >= character.Position_.Y && current_pos.Y < character.Position_.Y + character.Size_)
                                character.Die(500, this);
                            if (!map_[(int)(current_pos.X / size_), (int)(current_pos.Y / size_)].Walkable())
                                break;
                            current_pos += direction;
                        }
                        map_[i, j].Var_[0, 5] = (current_pos - new Vector2(i, j) * size_ - new Vector2(size_) / 2).Length();
                    }
                }

            for (int j = Math.Max(0, shift_y_ / size_ - (screenheight / 2) / size_); j <= shift_y_ / size_ + screenheight / size_ + 1 + (screenheight / 2) / size_ && j < height_; j++)
            {
                for (int i = Math.Max(0, shift_x_ / size_ - (screenwidth / 2) / size_); i <= shift_x_ / size_ + screenwidth / size_ + 1 + (screenwidth / 2) / size_ && i < width_; i++)
                {
                    float rate = (float)reduce_speed_;
                    if ((double)Math.Abs(shadows_smooth_goal_[i, j] - shadows_smooth_[i, j]) < 1f / rate)
                        shadows_smooth_[i, j] = shadows_smooth_goal_[i, j];
                    shadows_smooth_[i, j] += (double)(shadows_smooth_goal_[i, j] - shadows_smooth_[i, j]) / rate;
                }
            }
        }
    }
}