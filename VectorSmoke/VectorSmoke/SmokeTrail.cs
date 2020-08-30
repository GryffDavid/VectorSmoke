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

        VertexPositionColor[] vertices = new VertexPositionColor[39*2];
        
        Vector2 StartPosition = new Vector2(300, 550);
        Vector2 ControlPoint1 = new Vector2(400, 600);
        Vector2 ControlPoint2 = new Vector2(200, 200);
        Vector2 ControlPoint3 = new Vector2(600, 100);

        float Time, Time2;
        Random Random = new Random();
        float Width = 30;
        float t = 0;

        class SmokePoint
        {
            public Vector2 Position, Velocity;
            public float VerticalFriction = 1.0f;
            public float HorizontalGravity;
        }

        float Rad1 = 100;
        float NextRad1;

        float Rad2 = 100;
        float NextRad2;

        float VFriction = 1.0f;
        float NextVFriction = 1.0f;
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
            #region Positions

            StartPosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            Time2 += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Time2 > 60)
            {
                positions.Insert(0, new SmokePoint() { Position = ControlPoint1 });
                Time2 = 0;
            }

            for (int i = 1; i < positions.Count; i++)
            {
                positions[i].Position += new Vector2(positions[i].HorizontalGravity, (-3 * (positions[i].Position.Y / 1000)) * positions[i].VerticalFriction);

                if (positions[i].Position.Y > ControlPoint2.Y)
                {
                    positions[i].HorizontalGravity -= -Vector2.Normalize((ControlPoint2 - ControlPoint1)).X * 0.05f;
                }

                if (positions[i].Position.Y > ControlPoint3.Y && positions[i].Position.Y < ControlPoint2.Y)
                {
                    positions[i].HorizontalGravity += 0.02f;
                }

                if (positions[i].Position.Y < StartPosition.Y && positions[i].VerticalFriction == 1.0f)
                {
                    float dist = Math.Abs(positions[i].Position.X - ControlPoint1.X);

                    //if (dist >= 20)
                    //{
                        float fric = 1.0f - ((100f / 120f) * dist) / 100f;
                        positions[i].VerticalFriction *= MathHelper.Clamp(fric, 0.8f, 1f);
                    //}
                    //else
                    //{
                    //    positions[i].VerticalFriction = VFriction;
                    //}
                }
            }

            if (positions.Count >= 40)
            {
                positions.RemoveAt(39);
            }

            if (Time > 250)
            {
                NextRad1 = Random.Next(50, 500);
                NextRad2 = Random.Next(50, 250);

                NextVFriction = MathHelper.Lerp(VFriction, (float)RandomDouble(0.7f, 0.999f), 0.1f);

                Time = 0;
            }

            Rad1 = MathHelper.Lerp(Rad1, NextRad1, 0.2f);
            Rad2 = MathHelper.Lerp(Rad2, NextRad2, 0.2f);
            VFriction = MathHelper.Lerp(VFriction, NextVFriction, 0.3f);


             t += 0.1f;


            ControlPoint2 = new Vector2(StartPosition.X + Rad1 * (float)Math.Cos(t), StartPosition.Y + Rad2 * (float)Math.Sin(t));
            ControlPoint3 = Vector2.Lerp(ControlPoint3, ControlPoint2, (float)RandomDouble(0.0001f, 0.1f));

            if (t >= 2 * Math.PI)
            {
                t = 0;
            }
            #endregion

            #region Vertices
            //for (int i = 0; i < positions.Count; i+=2)
            //{
            //    vertices[i + 1].Position = new Vector3(positions[i].Position.X - 10, positions[i].Position.Y, 0);
            //    vertices[i].Position = new Vector3(positions[i].Position.X + 10, positions[i].Position.Y, 0);

            //    vertices[i].Color = Color.White;
            //}

            for (int i = 0; i < positions.Count; i++)
            {
                //vertices[i + 1].Position = new Vector3(positions[i].Position.X - 10, positions[i].Position.Y, 0);
                vertices[i].Position = new Vector3(positions[i].Position.X, positions[i].Position.Y, 0);

                vertices[i].Color = Color.White;
            }
            #endregion
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, StartPosition, Color.Red);
            spriteBatch.Draw(Texture, ControlPoint1, Color.Yellow);
            spriteBatch.Draw(Texture, ControlPoint2, Color.Turquoise);
            spriteBatch.Draw(Texture, ControlPoint3, Color.Yellow);

            foreach (SmokePoint pos in positions)
            {
                spriteBatch.Draw(Texture, pos.Position, Color.White);
            }

            //foreach (VertexPositionColor vert in vertices)
            //{
            //    spriteBatch.Draw(Texture, new Vector2(vert.Position.X, vert.Position.Y), Color.White);
            //}
        }

        public void DrawVector(GraphicsDevice graphics, BasicEffect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();                
                //graphics.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 78);
                //graphics.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 38);
                graphics.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, 38);
            }
        }

        public double RandomDouble(double a, double b)
        {
            return a + Random.NextDouble() * (b - a);
        }

        //public void Draw(GraphicsDevice graphics, BasicEffect effect)
        //{

        //}
    }
}
