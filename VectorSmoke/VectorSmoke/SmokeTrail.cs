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

        VertexPositionColor[] vertices = new VertexPositionColor[41];
        VertexPositionColor[] vertices2 = new VertexPositionColor[81];

        Vector2 StartPosition = new Vector2(1920/2, 1080/2);

        Vector2 ControlPoint1 = new Vector2(400, 600);
        Vector2 ControlPoint2 = new Vector2(200, 200);
        Vector2 ControlPoint3 = new Vector2(600, 100);

        float CurrentTime, Time2, MaxTime, Time3, MaxTime3;
        static Random Random = new Random();
        float MaxWidth = 10;
        float t = 0;
        float tMult = 1.0f;

        SpriteFont Font;

        class SmokePoint
        {
            public Vector2 Position, Direction, Tangent;
            public float VerticalFriction = 1.0f;
            public float HorizontalGravity;
            public float Width = 10;
        }

        float Rad1 = 100;
        float NextRad1;

        float Rad2 = 100;
        float NextRad2;

        float VFriction = 1.0f;
        float NextVFriction = 1.0f;
        List<SmokePoint> Positions = new List<SmokePoint>();

        KeyboardState CurrentKeyboardState, PreviousKeyboardState;

        Color Color = Color.White * 0.5f;

        float eightSize = 0.25f;

        public SmokeTrail()
        {
            for (int i = 0; i < vertices.Length/2 + 1; i++)
            {
                SmokePoint smokePoint = new SmokePoint() 
                { 
                    Position = new Vector2(400, 600)
                };
                smokePoint.Width = Random.Next(10, 20);

                Positions.Add(smokePoint);
            }

            MaxTime3 = 1000f;

            MaxTime = 250;
        }

        public void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("Point");
            Font = content.Load<SpriteFont>("SpriteFont");
        }

        public void Update(GameTime gameTime)
        {
            CurrentKeyboardState = Keyboard.GetState();

            CurrentTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            Time2 += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            Time3 += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (CurrentKeyboardState != PreviousKeyboardState)
            {
                int p = 0;
            }

            #region Add a new point
            if (Time2 > 60)
            {
                if (Positions.Count > vertices.Length/2)
                {
                    Positions.RemoveAt(vertices.Length/2);
                }

                Positions.Insert(0, new SmokePoint() { Position = StartPosition, Width = Random.Next(10, 20) });

                Time2 = 0;
            }
            #endregion

            #region Vertices
            for (int i = 1; i < Positions.Count - 1; i++)
            {
                Vector2 Dir;
                double angle;
                Vector2 newDir;

                Dir = Positions[i-1].Position - Positions[i + 1].Position;
                Dir.Normalize();

                Positions[i].Tangent = Dir;

                angle = Math.Atan2(Dir.Y, Dir.X) - MathHelper.ToRadians(90);
                newDir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                Positions[i].Direction = newDir;
            }

            Positions[0].Direction = new Vector2(1, 0);
            //Positions[0].Width = 20;

            for (int i = 0; i < vertices.Length - 1; i += 2)
            {
                MaxWidth = Positions[i / 2].Width;
                float newWidth = MathHelper.Lerp(MaxWidth, 0, (1.0f / (vertices.Length / 2)) * (i / 2));

                Vector2 newDir = Positions[i / 2].Direction;
                Vector2 dir = Positions[i / 2].Tangent;

                vertices[i].Position = new Vector3(Positions[i / 2].Position.X - (newDir.X * newWidth),
                                                   Positions[i / 2].Position.Y - (newDir.Y * newWidth), 0);

                vertices[i + 1].Position = new Vector3(Positions[i / 2].Position.X + (newDir.X * newWidth),
                                                       Positions[i / 2].Position.Y + (newDir.Y * newWidth), 0);


                vertices2[i].Position = new Vector3(Positions[i / 2].Position.X - (dir.X * newWidth),
                                                    Positions[i / 2].Position.Y - (dir.Y * newWidth), 0);

                vertices2[i + 1].Position = new Vector3(Positions[i / 2].Position.X + (dir.X * newWidth),
                                                       Positions[i / 2].Position.Y + (dir.Y * newWidth), 0);

                vertices[i].Color = Color.White;
                vertices[i + 1].Color = Color.White;
            }

            vertices[vertices.Length - 1].Position = new Vector3(Positions[Positions.Count-1].Position.X, 
                                                                 Positions[Positions.Count-1].Position.Y, 0);
            #endregion

            #region Apply forces to the points
            for (int i = 1; i < Positions.Count; i++)
            {
                Positions[i].Position += new Vector2(
                    Positions[i].HorizontalGravity, 
                    (-20 * (Positions[i].Position.Y / 800)) * Positions[i].VerticalFriction);

                if (Positions[i].Position.Y > ControlPoint2.Y)
                {
                    Positions[i].HorizontalGravity -= -Vector2.Normalize((ControlPoint2 - StartPosition)).X * 0.08f;
                }

                if (Positions[i].Position.Y > ControlPoint3.Y &&
                    Positions[i].Position.Y < ControlPoint2.Y)
                {
                    Positions[i].HorizontalGravity -= -Vector2.Normalize((ControlPoint2 - StartPosition)).X * 0.2f;
                }

                if (Positions[i].Position.Y < ControlPoint3.Y)
                {
                    float dist = MathHelper.Clamp(Math.Abs(Positions[i].Position.X - StartPosition.X), 0, 120);

                    float perc = 100 - ((100 / 120) * dist);
                    Positions[i].VerticalFriction = 1.0f - (perc / 200f);
                }
            }
            #endregion

            if (Time3 > MaxTime3)
            {
                if (tMult == -1.0f)
                {
                    tMult = 1.0f;
                }
                else
                {
                    tMult = -1.0f;
                }

                Time3 = 0;
                MaxTime3 = Random.Next(1500, 8000);
            }

            #region Move the control points
            if (CurrentTime > MaxTime)
            {
                NextRad1 = Random.Next(100, 500);
                NextRad2 = Random.Next(100, 250);
                NextVFriction = MathHelper.Lerp(VFriction, (float)RandomDouble(0.7f, 0.999f), 0.1f);

                CurrentTime = 0;

                MaxTime = Random.Next(150, 800);
                eightSize = (float)RandomDouble(0.1f, 0.8f);
            }

            Rad1 = MathHelper.Lerp(Rad1, NextRad1, 0.2f);
            Rad2 = MathHelper.Lerp(Rad2, NextRad2, 0.2f);
            VFriction = MathHelper.Lerp(VFriction, NextVFriction, 0.02f);
            
            t += 0.1f;

            ControlPoint1 = new Vector2(
                2 * (float)Math.Cos(eightSize * t),
                    (float)Math.Sin((eightSize * 2f) * t)) * 200f + new Vector2(1920/2, 350);

            //ControlPoint1 = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            ControlPoint2 = new Vector2(
                Rad1 * (float)Math.Cos(t * tMult),
                Rad2 * (float)Math.Sin(t * tMult)) + ControlPoint1;

            ControlPoint3 = Vector2.Lerp(ControlPoint3, ControlPoint2, (float)RandomDouble(0.0001f, 0.1f));

            if (t >= 8 * Math.PI)
            {
                t = 0;
            }
            #endregion

            PreviousKeyboardState = CurrentKeyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(Texture, StartPosition, Color.Red);

            //spriteBatch.Draw(Texture, ControlPoint1, Color.Orange);
            //spriteBatch.Draw(Texture, ControlPoint2, Color.Gold);
            //spriteBatch.Draw(Texture, ControlPoint3, Color.Lime);

            //foreach (SmokePoint point in Positions)
            //{
            //    spriteBatch.Draw(Texture, point.Position, Color.Red);
            //}

            //foreach (VertexPositionColor vert in vertices)
            //{
            //    spriteBatch.Draw(Texture, new Rectangle((int)vert.Position.X, (int)vert.Position.Y, 2, 2), null, Color.White, 0, new Vector2(1, 1), SpriteEffects.None, 0);
            //}

            //for (int i = 0; i < vertices.Count(); i++)
            //{
            //    spriteBatch.DrawString(Font, i.ToString(), new Vector2(vertices[i].Position.X, vertices[i].Position.Y), Color.Yellow, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            //}
        }

        public void DrawVector(GraphicsDevice graphics, BasicEffect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, vertices.Length - 2);
            }

            //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    graphics.DrawUserPrimitives(PrimitiveType.LineList, vertices2, 0, vertices2.Length / 2);
            //}
        }

        public double RandomDouble(double a, double b)
        {
            return a + Random.NextDouble() * (b - a);
        }
    }
}


