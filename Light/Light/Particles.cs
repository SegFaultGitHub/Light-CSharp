using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcadeGame
{
    class Particle
    {
        private Vector2 position_;
        private Vector2 speed_;
        private float updates_remaining_;
        private int size_;
        private int gravity_sign_;
        private Random rand_;
        Color color_;

        public Particle(Vector2 position, int gravity_sign)
        {
            gravity_sign_ = gravity_sign;
            updates_remaining_ = 64;
            position_ = position;
            rand_ = new Random();
            speed_ = new Vector2(rand_.Next(-50, 51) / 40f, gravity_sign * rand_.Next(-50, -20) / 40f);
            size_ = rand_.Next(2, 6);
            color_ = Color.Black;
        }

        public Particle(Vector2 position, int direction, int gravity_sign)
        {
            gravity_sign_ = gravity_sign;
            updates_remaining_ = 64;
            position_ = position;
            rand_ = new Random();
            if (direction == -1)
                speed_ = new Vector2(rand_.Next(-50, -10) / 40f, gravity_sign * rand_.Next(-50, -20) / 40f);
            else
                speed_ = new Vector2(rand_.Next(10, 51) / 40f, gravity_sign * rand_.Next(-50, -20) / 40f);
            size_ = rand_.Next(2, 6);
            color_ = Color.Black;
        }

        public Particle(Vector2 position, int direction, int gravity_sign, Color color)
        {
            gravity_sign_ = gravity_sign;
            updates_remaining_ = 64;
            position_ = position;
            rand_ = new Random();
            if (direction == -1)
                speed_ = new Vector2(rand_.Next(-50, -10) / 40f, gravity_sign * rand_.Next(-50, -20) / 40f);
            else
                speed_ = new Vector2(rand_.Next(10, 51) / 40f, gravity_sign * rand_.Next(-50, -20) / 40f);
            size_ = rand_.Next(2, 6);
            color_ = color;
        }

        public Particle(Vector2 position, Color color, int gravity_sign)
        {
            gravity_sign_ = gravity_sign;
            updates_remaining_ = 64;
            position_ = position;
            rand_ = new Random();
            speed_ = new Vector2(rand_.Next(-50, 51) / 40f, gravity_sign * rand_.Next(-50, -20) / 40f);
            size_ = rand_.Next(2, 6);
            color_ = color;
        }

        public bool Update()
        {
            updates_remaining_--;
            position_ += speed_;
            speed_ += /*gravity_sign_ * */new Vector2(0, 0.05f);
            return updates_remaining_ < 0;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 shift)
        {
            spriteBatch.Draw(Textures.Pixel_, new Rectangle((int)position_.X - (int)shift.X - size_ / 2, (int)position_.Y - (int)shift.Y - size_ / 2, size_, size_), new Color(color_.R, color_.G, color_.B, (4 * updates_remaining_) / 255f));
        }
    }

    class Particles
    {
        private static Random rand_;
        private static List<Particle> particles_;

        public static void Initialize()
        {
            rand_ = new Random();
            particles_ = new List<Particle>();
        }

        public static void createParticles(Character character, int n)
        {
            if (!Drawer.Hd_)
                return;
            Vector2 position = character.Position_ + new Vector2(character.Size_ / 2, Character.Gravity_sign_ == 1 ? character.Size_ : 0);
            for (int i = 0; i < n; i++)
            {
                particles_.Add(new Particle(position, Character.Gravity_sign_));
            }
        }

        public static void createParticles(Character character, int n, Color color)
        {
            if (!Drawer.Hd_)
                return;
            Vector2 position = character.Position_ + new Vector2(character.Size_ / 2, Character.Gravity_sign_ == 1 ? character.Size_ : 0);
            for (int i = 0; i < n; i++)
            {
                particles_.Add(new Particle(position, color, Character.Gravity_sign_));
            }
        }

        public static void createParticles(Character character, int n, int direction)
        {
            if (!Drawer.Hd_)
                return;
            Vector2 position = character.Position_ + new Vector2(character.Size_ / 2, Character.Gravity_sign_ == 1 ? character.Size_ : 0);
            if (n == 0)
                n = rand_.Next() % 2 == 0 ? rand_.Next(6) : 0;
            for (int i = 0; i < n; i++)
            {
                particles_.Add(new Particle(position, direction, Character.Gravity_sign_));
            }
        }

        public static void createParticles(Character character, int n, int direction, Color color)
        {
            if (!Drawer.Hd_)
                return;
            Vector2 position = character.Position_ + new Vector2(character.Size_ / 2, Character.Gravity_sign_ == 1 ? character.Size_ : 0);
            if (n == 0)
                n = rand_.Next() % 2 == 0 ? rand_.Next(6) : 0;
            for (int i = 0; i < n; i++)
            {
                particles_.Add(new Particle(position, direction, Character.Gravity_sign_, color));
            }
        }

        public static void createParticles(Vector2 position, int n, int direction)
        {
            if (!Drawer.Hd_)
                return;
            if (n == 0)
                n = rand_.Next() % 2 == 0 ? rand_.Next(6) : 0;
            for (int i = 0; i < n; i++)
            {
                particles_.Add(new Particle(position, direction, 1));
            }
        }

        public static void createParticles(Vector2 position, Color color)
        {
            if (!Drawer.Hd_)
                return;
            int n = rand_.Next() % 25 == 0 ? 1 : 0;
            for (int i = 0; i < n; i++)
            {
                particles_.Add(new Particle(position, color, 1));
            }
        }

        public static void Update()
        {
            if (!Drawer.Hd_)
                return;
            for (int i = 0; i < particles_.Count(); i++)
            {
                bool remove = particles_[i].Update();
                if (remove)
                    particles_.Remove(particles_[i]);
            }
        }

        public static void Draw(SpriteBatch spriteBatch, Map map)
        {
            if (!Drawer.Hd_)
                return;
            for (int i = 0; i < particles_.Count; i++)
                particles_[i].Draw(spriteBatch, new Vector2(map.Shift_x_ + map.F_shift_x_, map.Shift_y_ + map.F_shift_y_));
        }
    }
}
