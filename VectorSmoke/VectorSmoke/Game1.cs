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
        SmokeTrail SmokeTrail = new SmokeTrail();
        SmokeTrail SmokeTrail2 = new SmokeTrail();
        BasicEffect BasicEffect;
        Matrix Projection;

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
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Point = Content.Load<Texture2D>("Point");
            SmokeTrail.LoadContent(Content);
            SmokeTrail2.LoadContent(Content);

            SmokeTrail2.StartPosition = new Vector2(1920 / 2 + 80, 1080 / 2);
        }
        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        protected override void Update(GameTime gameTime)
        {
            SmokeTrail.Update(gameTime);
            SmokeTrail2.Update(gameTime);
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            //SmokeTrail.DrawVector(GraphicsDevice, BasicEffect);            

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, RasterizerState.CullNone);
            SmokeTrail.Draw(spriteBatch);
            SmokeTrail2.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
