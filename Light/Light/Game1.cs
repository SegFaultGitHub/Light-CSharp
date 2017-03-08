using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ArcadeGame
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            DateTime start = DateTime.Now;
            base.Initialize();
            Drawer.Initialize(graphics);
            Drawer.DisableFullScreen();
            Maps.Initialize();
            TitleScreen.Initialize();
            TitleScreen.Activate();
            Background.Initialize(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Particles.Initialize();
            base.Initialize();
            Console.WriteLine("Initialization took " + (DateTime.Now - start).ToString());
            Message.Compute();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Textures.LoadContent(Content, GraphicsDevice);
            Sounds.LoadContent(Content);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            if (TitleScreen.Active_)
            {
                TitleScreen.Update(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            }
            else
            {
                switch (TitleScreen.Gameplay_)
                {
                    case GamePlay.Play:
                        GameHandler.Update(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
                        break;
                    case GamePlay.Editor:
                        Editor.Update(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
                        break;
                    case GamePlay.Quit:
                        Exit();
                        break;
                    default:
                        break;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            if (TitleScreen.Active_)
            {
                GraphicsDevice.Clear(Color.Black);
                TitleScreen.Draw(spriteBatch);
            }
            else
            {
                switch (TitleScreen.Gameplay_)
                {
                    case GamePlay.Play:
                        GraphicsDevice.Clear(Color.White);
                        GameHandler.Draw(spriteBatch, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
                        break;
                    case GamePlay.Editor:
                        GraphicsDevice.Clear(Color.DarkGray);
                        Editor.Draw(spriteBatch, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
                        break;
                    default:
                        break;
                }
            }
            //spriteBatch.Draw(Textures.Spiral_, Vector2.Zero, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
