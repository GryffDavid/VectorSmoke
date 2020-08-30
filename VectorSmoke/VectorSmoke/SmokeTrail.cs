using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VectorSmoke
{
    public class SmokeTrail
    {
        Texture2D Texture;

        VertexPositionColor[] vertices = new VertexPositionColor[5];
        int[] indices = new int[6];

        //public List<Vector2> positions = new List<Vector2>();
        
        Vector2 StartPosition = new Vector2(400, 400);
        Vector2 ControlPoint = new Vector2(400, 360);

        Vector2 ControlPoint2 = new Vector2(400, 360);

        float Time;
        Random Random = new Random();
        float Width = 30;

        class SmokePoint
        {
            public Vector2 Position, Velocity;
            public float VerticalFriction;
            public float HorizontalGravity;
        }

        List<SmokePoint> positions = new List<SmokePoint>();

        public SmokeTrail()
        {

        }

        public void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("Point");
        }

        public void Update(GameTime gameTime)
        {
            //ControlPoint2 = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            ControlPoint2 = Vector2.Lerp(ControlPoint, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), 0.01f);

            Time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Time > 60)
            {
                Vector2 dir = ControlPoint - StartPosition;
                dir.Normalize();
                positions.Add(new SmokePoint() { Position = ControlPoint });
                Time = 0;
            }

            for (int i = 1; i < positions.Count; i++)
            {
                positions[i].Position += new Vector2(positions[i].HorizontalGravity, -3 * (positions[i].Position.Y/400));

                if (i == 50)
                    positions[i].Position = Vector2.Lerp(positions[i].Position, ControlPoint2, 0.2f);
            }

            if (positions.Count >= 80)
            {                
                positions.RemoveAt(0);
            }        
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, StartPosition, Color.Red);
            spriteBatch.Draw(Texture, ControlPoint, Color.Yellow);

            foreach (SmokePoint pos in positions)
            {
                spriteBatch.Draw(Texture, pos.Position, Color.White);
            }
        }

        //public void Draw(GraphicsDevice graphics, BasicEffect effect)
        //{

        //}
    }
}
