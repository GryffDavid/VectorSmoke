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

        //SmokeTrail SmokeTrail = new SmokeTrail(new Vector2(400, 800));
        List<SmokeTrail> trailList = new List<SmokeTrail>();
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
            BasicEffect = new BasicEffect(GraphicsDevice);
            Projection = Matrix.CreateOrthographicOffCenter(0, 1920, 1080, 0, 0, 1);

            BasicEffect.Projection = Projection;
            BasicEffect.VertexColorEnabled = true;

            for (int i = 0; i < 10; i++)
            {
                SmokeTrail SmokeTrail = new SmokeTrail(new Vector2(Random.Next(200, 1920-200), 800));
                trailList.Add(SmokeTrail);
            }
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        
        protected override void UnloadContent()
        {

        }
        
        protected override void Update(GameTime gameTime)
        {
            foreach (SmokeTrail trail in trailList)
            {
                trail.Update(gameTime);
            }
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            foreach (SmokeTrail trail in trailList)
            {
                trail.DrawVector(GraphicsDevice, BasicEffect);
            }

            //SmokeTrail.DrawVector(GraphicsDevice, BasicEffect);            

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, RasterizerState.CullNone);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
