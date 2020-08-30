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

        VertexPositionColor[] vertices = new VertexPositionColor[81];
        
        Vector2 StartPosition = new Vector2(300, 550);
        Vector2 ControlPoint1 = new Vector2(400, 600);
        Vector2 ControlPoint2 = new Vector2(200, 200);
        Vector2 ControlPoint3 = new Vector2(600, 100);

        float CurrentTime, Time2, MaxTime;
        Random Random = new Random();
        float MaxWidth = 10;
        float t = 0;

        SpriteFont Font;

        class SmokePoint
        {
            public Vector2 Position, Direction, Tangent;
            public float VerticalFriction = 1.0f;
            public float HorizontalGravity;
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

        public SmokeTrail()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Positions.Add(new SmokePoint() { Position = new Vector2(400, 600) });
            }

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

            //if (CurrentKeyboardState != PreviousKeyboardState)
            {
                #region Positions
                CurrentTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                Time2 += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                #region Add a new point
                if (Time2 > 60)
                {
                    if (Positions.Count >= 40)
                    {
                        Positions.RemoveAt(40);
                    }

                    Positions.Insert(0, new SmokePoint() { Position = ControlPoint1 });

                    Time2 = 0;
                }
                #endregion


                for (int i = 80; i >= 1; i -= 2)
                {
                    float newWidth = MathHelper.Lerp(MaxWidth, 0, (1.0f / (vertices.Length/2)) * (i/2));

                    Vector2 Dir;
                    double angle;
                    Vector2 newDir;


                    Dir = Vector2.Normalize(Positions[i/2].Position - Positions[(i/2) - 1].Position);
                    angle = Math.Atan2(Dir.Y, Dir.X) - MathHelper.ToRadians(90);
                    newDir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                    if (i == 2)
                    {
                        newDir = new Vector2(-1, 0);
                    }

                    vertices[i].Position = new Vector3(Positions[i / 2].Position.X - (newDir.X * newWidth), Positions[i / 2].Position.Y - (newDir.Y * newWidth), 0);
                    vertices[i].Color = Color.White;
                    if (float.IsNaN(vertices[i].Position.X) == true)
                    {
                        int p = 0;
                    }

                    vertices[i - 1].Position = new Vector3(Positions[i / 2].Position.X + (newDir.X * newWidth), Positions[i / 2].Position.Y + (newDir.Y * newWidth), 0);
                    vertices[i - 1].Color = Color.White;
                    if (float.IsNaN(vertices[i - 1].Position.X) == true)
                    {
                        int p = 0;
                    }
                }

                vertices[1].Position = new Vector3(Positions[0].Position.X - 10, Positions[0].Position.Y, 0);
                vertices[1].Color = Color.White;

                vertices[0].Position = new Vector3(Positions[0].Position.X + 10, Positions[0].Position.Y, 0);
                vertices[0].Color = Color.White;

                #region Apply forces to the points
                for (int i = 1; i < Positions.Count; i++)
                {
                    //Move the SmokePoint position
                    Positions[i].Position += new Vector2(Positions[i].HorizontalGravity, (-10 * (Positions[i].Position.Y / 800)) * Positions[i].VerticalFriction);

                    if (Positions[i].Position.Y > ControlPoint2.Y)
                    {
                        Positions[i].HorizontalGravity -= -Vector2.Normalize((ControlPoint2 - ControlPoint1)).X * 0.08f;
                    }

                    if (Positions[i].Position.Y > ControlPoint3.Y && 
                        Positions[i].Position.Y < ControlPoint2.Y)
                    {
                        Positions[i].HorizontalGravity += 0.02f;
                    }

                    if (Positions[i].Position.Y < ControlPoint3.Y)
                    {
                        float dist = MathHelper.Clamp(Math.Abs(Positions[i].Position.X - ControlPoint1.X), 0, 120);

                        float perc = 100 - ((100 / 120) * dist);
                        Positions[i].VerticalFriction = 1.0f - (perc / 200f);
                    }
                }
                #endregion

                #region Move the control points
                StartPosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

                if (CurrentTime > MaxTime)
                {
                    NextRad1 = Random.Next(50, 500);
                    NextRad2 = Random.Next(50, 250);

                    NextVFriction = MathHelper.Lerp(VFriction, (float)RandomDouble(0.7f, 0.999f), 0.1f);

                    CurrentTime = 0;

                    MaxTime = Random.Next(150, 800);
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
                #endregion
            }

            PreviousKeyboardState = CurrentKeyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, StartPosition, Color.Red);
            spriteBatch.Draw(Texture, ControlPoint1, Color.Yellow);
            spriteBatch.Draw(Texture, ControlPoint2, Color.Turquoise);
            spriteBatch.Draw(Texture, ControlPoint3, Color.Coral);

            //foreach (SmokePoint pos in Positions)
            //{
            //    spriteBatch.Draw(Texture, pos.Position, Color);
            //}

            //for (int i = 0; i < vertices.Length; i++)
            //{
            //    spriteBatch.DrawString(Font, i.ToString(), new Vector2(vertices[i].Position.X, vertices[i].Position.Y), Color.Red);
            //}
        }

        public void DrawVector(GraphicsDevice graphics, BasicEffect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 79);
            }
        }

        public double RandomDouble(double a, double b)
        {
            return a + Random.NextDouble() * (b - a);
        }
    }
}


