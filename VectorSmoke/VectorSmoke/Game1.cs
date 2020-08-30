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

namespace VectorSmoke
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D Point;
        List<SmokeTrail> TrailList = new List<SmokeTrail>();
        BasicEffect BasicEffect;
        Matrix Projection;
        static Random Random = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            BasicEffect = new BasicEffect(GraphicsDevice);
            Projection = Matrix.CreateOrthographicOffCenter(0, 1920, 1080, 0, 0, 1);

            BasicEffect.Projection = Projection;
            BasicEffect.VertexColorEnabled = true;

            //SmokeTrail2.StartPosition = new Vector2(1920 / 2, 1080 / 2) + new Vector2(-35, 0);

            for (int i = 0; i < 5; i++)
            {
                SmokeTrail trail = new SmokeTrail(Random.Next(40, 70));
                trail.StartPosition = new Vector2(1920 / 2, 1080 / 2) + new Vector2(Random.Next(-50, 50), 0);
                TrailList.Add(trail);
            }
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Point = Content.Load<Texture2D>("Point");
            //SmokeTrail.LoadContent(Content);
            //SmokeTrail2.LoadContent(Content);

            foreach (SmokeTrail trail in TrailList)
            {
                trail.LoadContent(Content);
            }
        }
        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        protected override void Update(GameTime gameTime)
        {
            //SmokeTrail.Update(gameTime);
            //SmokeTrail2.Update(gameTime);

            foreach (SmokeTrail trail in TrailList)
            {
                trail.Update(gameTime);
            }
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            foreach (SmokeTrail trail in TrailList)
            {
                trail.DrawVector(GraphicsDevice, BasicEffect);
            }
            //SmokeTrail.DrawVector(GraphicsDevice, BasicEffect);
            //SmokeTrail2.DrawVector(GraphicsDevice, BasicEffect);            

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, RasterizerState.CullNone);
            foreach (SmokeTrail trail in TrailList)
            {
                trail.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
