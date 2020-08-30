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
        Vector2 ControlPoint1 = new Vector2(400, 300);
        Vector2 ControlPoint2 = new Vector2(200, 200);
        Vector2 ControlPoint3 = new Vector2(600, 100);

        float Time;
        Random Random = new Random();
        float Width = 30;

        class SmokePoint
        {
            public Vector2 Position, Velocity;
            public float VerticalFriction = 1.0f;
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
            //ControlPoint2 = Vector2.Lerp(ControlPoint2, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), 0.2f);
            //ControlPoint3 = Vector2.Lerp(ControlPoint3, ControlPoint2, 0.01f);

            Time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Time > 60)
            {
                positions.Insert(0, new SmokePoint() { Position = ControlPoint1 });
                Time = 0;

                ControlPoint2 = Vector2.Lerp(ControlPoint2, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), 0.2f);
                ControlPoint3 = Vector2.Lerp(ControlPoint3, ControlPoint2, 0.01f);
            }

            for (int i = 1; i < positions.Count; i++)
            {
                positions[i].Position += new Vector2(positions[i].HorizontalGravity, (-3 * (positions[i].Position.Y/400)) * positions[i].VerticalFriction);

                if (positions[i].Position.Y > ControlPoint2.Y)
                {
                    //positions[i].Position = Vector2.Lerp(positions[i].Position, ControlPoint2, 0.02f);
                    positions[i].HorizontalGravity -= -Vector2.Normalize((ControlPoint2 - ControlPoint1)).X * 0.02f;
                }

                if (positions[i].Position.Y > ControlPoint3.Y && positions[i].Position.Y < ControlPoint2.Y)
                {
                    positions[i].HorizontalGravity += 0.02f;
                    
                    //positions[i].Position = Vector2.Lerp(positions[i].Position, ControlPoint3, 0.02f);
                }

                if (positions[i].Position.Y < ControlPoint3.Y)
                {
                    positions[i].VerticalFriction = 0.9999f;
                }
            }

            

            if (positions.Count >= 80)
            {                
                positions.RemoveAt(79);
            }            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, StartPosition, Color.Red);

            spriteBatch.Draw(Texture, ControlPoint1, Color.Yellow);
            spriteBatch.Draw(Texture, ControlPoint2, Color.Yellow);
            spriteBatch.Draw(Texture, ControlPoint3, Color.Yellow);


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
