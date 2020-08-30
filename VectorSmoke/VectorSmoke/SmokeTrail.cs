﻿using System;
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

        VertexPositionColor[] vertices = new VertexPositionColor[6];
        
        Vector2 StartPosition = new Vector2(300, 550);
        Vector2 ControlPoint1 = new Vector2(400, 600);
        Vector2 ControlPoint2 = new Vector2(200, 200);
        Vector2 ControlPoint3 = new Vector2(600, 100);

        float Time, Time2;//, Time3;
        Random Random = new Random();
        float Width = 30;
        float t = 0;

        SpriteFont Font;

        //float tMult = 1.0f;

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
            for (int i = 0; i < 40; i++)
            {
                positions.Add(new SmokePoint() { Position = new Vector2(400, 600) });
            }
        }

        public void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("Point");
            Font = content.Load<SpriteFont>("SpriteFont");
        }

        public void Update(GameTime gameTime)
        {
            #region Positions

            StartPosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            Time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            Time2 += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //Time3 += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Time2 > 60)
            {
                positions.Insert(0, new SmokePoint() { Position = ControlPoint1 });
                Time2 = 0;
            }

            for (int i = 1; i < positions.Count; i++)
            {
                positions[i].Position += new Vector2(positions[i].HorizontalGravity, (-3 * (positions[i].Position.Y / 800)) * positions[i].VerticalFriction);

                if (positions[i].Position.Y > ControlPoint2.Y)
                {
                    positions[i].HorizontalGravity -= -Vector2.Normalize((ControlPoint2 - ControlPoint1)).X * 0.05f;
                }

                if (positions[i].Position.Y > ControlPoint3.Y && positions[i].Position.Y < ControlPoint2.Y)
                {
                    positions[i].HorizontalGravity += 0.02f;
                }

                if (positions[i].Position.Y < StartPosition.Y)
                {
                    float dist = MathHelper.Clamp(Math.Abs(positions[i].Position.X - ControlPoint1.X), 0, 120);

                    float perc = 100 - ((100 / 120) * dist);
                    positions[i].VerticalFriction = 1.0f - (perc / 500f);
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
            VFriction = MathHelper.Lerp(VFriction, NextVFriction, 0.02f);


             t += 0.1f;


             ControlPoint2 = new Vector2(StartPosition.X + Rad1 * (float)Math.Cos(t * 1), StartPosition.Y + Rad2 * (float)Math.Sin(t * 1));
            ControlPoint3 = Vector2.Lerp(ControlPoint3, ControlPoint2, (float)RandomDouble(0.0001f, 0.1f));

            if (t >= 2 * Math.PI)
            {
                t = 0;
            }
            #endregion

            #region Vertices
            ////for (int i = 0; i < positions.Count; i+=2)
            ////{
            ////    vertices[i + 1].Position = new Vector3(positions[i].Position.X - 10, positions[i].Position.Y, 0);
            ////    vertices[i].Position = new Vector3(positions[i].Position.X + 10, positions[i].Position.Y, 0);

            ////    vertices[i].Color = Color.White;
            ////}

            //for (int i = 0; i < positions.Count; i++)
            //{
            //    //vertices[i + 1].Position = new Vector3(positions[i].Position.X - 10, positions[i].Position.Y, 0);
            //    vertices[i].Position = new Vector3(positions[i].Position.X, positions[i].Position.Y, 0);
            //    vertices[i].Color = Color.White;
            //}
            #endregion

            //vertices[0].Position = new Vector3(ControlPoint1.X + 10, ControlPoint1.Y, 0);
            //vertices[1].Position = new Vector3(ControlPoint1.X - 10, ControlPoint1.Y, 0);

            //vertices[2].Position = new Vector3(positions[1].Position.X + 10, positions[1].Position.Y, 0);
            //vertices[3].Position = new Vector3(positions[1].Position.X - 10, positions[1].Position.Y, 0);

            //vertices[4].Position = new Vector3(positions[2].Position.X + 10, positions[2].Position.Y, 0);
            //vertices[5].Position = new Vector3(positions[2].Position.X - 10, positions[2].Position.Y, 0);
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

            //foreach (SmokePoint pos in positions)
            //{
            //    spriteBatch.DrawString(Font, positions.IndexOf(pos).ToString(), pos.Position, Color.Red);
            //}

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
                graphics.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, 3);
            }
        }

        public double RandomDouble(double a, double b)
        {
            return a + Random.NextDouble() * (b - a);
        }
    }
}
