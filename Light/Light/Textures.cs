using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcadeGame
{
    static class Textures
    {
        private static SpriteFont font_;
        public static SpriteFont Font_
        {
            get { return Textures.font_; }
        }
        private static Texture2D pixel_;
        public static Texture2D Pixel_
        {
            get { return Textures.pixel_; }
        }
        private static Texture2D portal_;
        public static Texture2D Portal_
        {
            get { return Textures.portal_; }
        }
        private static Texture2D coin_;
        public static Texture2D Coin_
        {
            get { return Textures.coin_; }
        }
        private static Texture2D circle_;
        public static Texture2D Circle_
        {
            get { return Textures.circle_; }
        }
        private static Texture2D torch_;
        public static Texture2D Torch_
        {
            get { return Textures.torch_; }
        }
        private static Texture2D laser_;
        public static Texture2D Laser_
        {
            get { return Textures.laser_; }
        }
        private static Texture2D spiral_;
        public static Texture2D Spiral_
        {
            get { return Textures.spiral_; }
        }

        public static void LoadContent(ContentManager Content, GraphicsDevice graphicsDevice)
        {
            #region Pixel
            pixel_ = new Texture2D(graphicsDevice, 1, 1);
            Color[] white = new Color[1];
            white[0] = Color.White;
            pixel_.SetData(white, 0, 1);
            #endregion
            #region Portal
            portal_ = new Texture2D(graphicsDevice, 200, 400);
            Color[] pixels_portal = new Color[200 * 400];
            Vector2 center = new Vector2(200, 400) / 2;
            int n = 0;
            for (int j = 0; j < 400; j++)
                for (int i = 0; i < 200; i++)
                {
                    float distance = Vector2.Distance(new Vector2(i, j), center) * 2;
                    if (distance > 120 && distance <= 190)
                        pixels_portal[n] = Color.White;
                    else
                        pixels_portal[n] = new Color(0, 0, 0, 0);
                    n++;
                }
            portal_.SetData(pixels_portal, 0, 200 * 400);
            #endregion
            #region Coin
            coin_ = new Texture2D(graphicsDevice, 200, 200);
            Color[] pixels_coin = new Color[200 * 200];
            center = new Vector2(200, 200) / 2;
            n = 0;
            for (int i = 0; i < 200; i++)
                for (int j = 0; j < 200; j++)
                {
                    float distance = Vector2.Distance(new Vector2(i, j), center) * 2;
                    if (distance > 95 && distance <= 135)
                        pixels_coin[n] = Color.Goldenrod;
                    else if (distance <= 95)
                        pixels_coin[n] = Color.Gold;
                    n++;
                }
            coin_.SetData(pixels_coin, 0, 200 * 200);
            #endregion
            #region Circle
            circle_ = new Texture2D(graphicsDevice, 200, 200);
            Color[] pixels_circle = new Color[200 * 200];
            center = new Vector2(200, 200) / 2;
            n = 0;
            for (int i = 0; i < 200; i++)
                for (int j = 0; j < 200; j++)
                {
                    float distance = Vector2.Distance(new Vector2(i, j), center) * 2;
                    if (distance > 75 && distance <= 135)
                        pixels_circle[n] = Color.White;
                    n++;
                }
            circle_.SetData(pixels_circle, 0, 200 * 200);
            #endregion
            #region Torch
            torch_ = new Texture2D(graphicsDevice, 200, 200);
            Color[] pixels_torch = new Color[200 * 200];
            center = new Vector2(200, 200) / 2;
            n = 0;
            for (int i = 0; i < 200; i++)
                for (int j = 0; j < 200; j++)
                {
                    if (j >= 80 && j < 80 + 40 && i >= 90 && i < 90 + 80)
                        pixels_torch[n] = Color.Brown;
                    if (j >= 70 && j < 70 + 60 && i >= 30 && i < 30 + 60)
                        pixels_torch[n] = Color.Orange;
                    n++;
                }
            torch_.SetData(pixels_torch, 0, 200 * 200);
            #endregion
            #region Laser
            laser_ = new Texture2D(graphicsDevice, 200, 200);
            Color[] pixels_laser = new Color[200 * 200];
            center = new Vector2(200, 200) / 2;
            n = 0;
            for (int i = 0; i < 200; i++)
                for (int j = 0; j < 200; j++)
                {
                    float distance = Vector2.Distance(new Vector2(i, j), center) * 2;
                    if (j >= 100 && j < 190 && i >= 80 && i <= 120)
                        pixels_laser[n] = new Color(0.3f, 0.3f, 0.3f);
                    if (distance <= 130)
                        pixels_laser[n] = Color.Black;
                    n++;
                }
            laser_.SetData(pixels_laser, 0, 200 * 200);
            #endregion
            #region Spiral
            spiral_ = new Texture2D(graphicsDevice, 200, 200);
            Color[] pixels_spiral = new Color[200 * 200];
            center = new Vector2(200, 200) / 2;
            n = 0;
            for (int i = 0; i < 200; i++)
                for (int j = 0; j < 200; j++)
                {
                    Vector2 diff = new Vector2(i, j) - center;
                    float temp = diff.X;
                    diff.X = diff.Y;
                    diff.Y = temp;
                    diff.Normalize();
                    float angle = 0f;
                    if (diff.X >= 0 && diff.Y < 0)
                        angle = (float)Math.Acos(Vector2.Dot(diff, new Vector2(1, 0)));
                    else if (diff.X >= 0 && diff.Y > 0)
                        angle = (float)Math.PI + (float)Math.Acos(Vector2.Dot(diff, new Vector2(-1, 0)));
                    else if (diff.X < 0 && diff.Y >= 0)
                        angle = (float)Math.PI + (float)Math.Acos(Vector2.Dot(diff, new Vector2(-1, 0)));
                    else
                        angle = (float)Math.Acos(Vector2.Dot(diff, new Vector2(1, 0)));
                    float distance = Vector2.Distance(new Vector2(i, j), center);
                    if (distance > (angle / Math.PI * 180f) * 1f - 35 && distance < (angle / Math.PI * 180f) * 0.55f + 35)
                        pixels_spiral[n] = Color.White;
                    n++;
                }
            spiral_.SetData(pixels_spiral, 0, 200 * 200);
            #endregion
            font_ = Content.Load<SpriteFont>("Audimat Mono");
        }
    }
}
