/*  Michael Ou
 *  0397225
 *  mou01@students.poly.edu
 *  Asteroids
 *  Game1.cs
 *  
 */

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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Asteroids
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // The images we will draw
        Texture2D shipTexture;
        Texture2D bulletTexture;
        Texture2D asteroidBTexture;
        Texture2D asteroidMTexture;
        Texture2D asteroidSTexture;
        // The color data for the images; used for per pixel collision
        Color[] shipTextureData;
        Color[] asteroidTextureData;
        Color[] bulletTextureData;

        // Ship 
        Vector2 shipPosition;
        float shipRotation;
        const float ShipMoveSpeed = 0.5f;
        const float ShipRotateSpeed = 0.05f;
        Vector2 shipOrigin;
        Vector2 shipDriftDirection;
        const float shootInterval = .5f;

        List<Projectile> projectiles;
        List<Asteroid> asteroids;
        Viewport viewport;
        Random rand;
        KeyboardState OldKeyState;
        
        float totalTimeElapsed = 0;
        float lastShootTime = 0;

        List<int> testInts = new List<int>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            viewport = graphics.GraphicsDevice.Viewport;
            respawn();
            projectiles = new List<Projectile>();
            asteroids = new List<Asteroid>();
            rand = new Random();
            addAsteroid();
            addAsteroid();
            addAsteroid();
            addAsteroid();
        }

        private void respawn()
        {
            shipPosition.X = (viewport.Width - shipTexture.Width) / 2;
            shipPosition.Y = (viewport.Height - shipTexture.Height) / 2;
            shipDriftDirection = new Vector2(0, 0);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load textures
            shipTexture = Content.Load<Texture2D>(@"Sprites\ship");
            bulletTexture = Content.Load<Texture2D>(@"Sprites\bullet");
            asteroidBTexture = Content.Load<Texture2D>(@"Sprites\asteroid");
            asteroidMTexture = Content.Load<Texture2D>(@"Sprites\asteroid2");
            asteroidSTexture = Content.Load<Texture2D>(@"Sprites\asteroid3");
            // Extract collision data
            shipTextureData = new Color[shipTexture.Width * shipTexture.Height];
            shipTexture.GetData(shipTextureData);

            // Calculate the ship origin
            shipOrigin =
                new Vector2(shipTexture.Width / 2, shipTexture.Height / 2);


            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        private void addAsteroid()
        {
            asteroids.Add(new Asteroid(new Vector2(rand.Next(0, viewport.Width), rand.Next(0, viewport.Height)),
                new Vector2(asteroidBTexture.Width / 2, asteroidBTexture.Height / 2), (float)(rand.NextDouble() * Math.PI * 2), asteroidBTexture));
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            totalTimeElapsed += elapsed;
            KeyboardState NewKeyState = Keyboard.GetState();
            // Get input
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

            // Allows the game to exit
            if (gamePad.Buttons.Back == ButtonState.Pressed ||
                keyboard.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // Move the player left and right with arrow keys or d-pad
            if (NewKeyState.IsKeyDown(Keys.OemTilde) && OldKeyState.IsKeyUp(Keys.OemTilde))
            {
                addAsteroid();
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                shipRotation -= ShipRotateSpeed;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                shipRotation += ShipRotateSpeed;
            }
            if (keyboard.IsKeyDown(Keys.Up))
            {
                shipPosition -= new Vector2((float)Math.Sin(shipRotation) * ShipMoveSpeed, -(float)Math.Cos(shipRotation) * ShipMoveSpeed);
                shipDriftDirection = new Vector2((float)Math.Sin(shipRotation) , -(float)Math.Cos(shipRotation) );
            }
            if (NewKeyState.IsKeyDown(Keys.D1) && OldKeyState.IsKeyUp(Keys.D1))
            {
                testInts.Add(1);
                testInts.Add(3);
                testInts.Add(2);
                Stream stream = File.Open("test.txt", FileMode.Create);
                BinaryFormatter bFormatter = new BinaryFormatter();
                bFormatter.Serialize(stream, testInts);
                stream.Close();
            }
            if (NewKeyState.IsKeyDown(Keys.D2) && OldKeyState.IsKeyUp(Keys.D2))
            {
                Stream stream = File.Open("test.txt", FileMode.Open);
                BinaryFormatter bFormatter = new BinaryFormatter();
                testInts = (List<int>)bFormatter.Deserialize(stream);
                stream.Close();
            }
            if (NewKeyState.IsKeyDown(Keys.D3) && OldKeyState.IsKeyUp(Keys.D3))
            {
                testInts.Clear();
            }
            if (NewKeyState.IsKeyDown(Keys.D4) && OldKeyState.IsKeyUp(Keys.D4))
            {
                foreach (int i in testInts)
                {
                    Console.Write(i);
                }
            }
            if (keyboard.IsKeyDown(Keys.Space) && (lastShootTime + shootInterval <= totalTimeElapsed))
            {
                lastShootTime = totalTimeElapsed;
                projectiles.Add(new Projectile(shipPosition - new Vector2(bulletTexture.Width/2, bulletTexture.Height/2),
                    new Vector2(bulletTexture.Width / 2, bulletTexture.Height / 2), shipRotation));
            }
            shipPosition -= shipDriftDirection;
            foreach (Projectile proj in projectiles)
            {
                proj.Position -= new Vector2((float)Math.Sin(proj.Rotation) * Projectile.SPEED, -(float)Math.Cos(proj.Rotation) * Projectile.SPEED);
                
            }
            // Ship stuff
            Matrix shipTransform =
                Matrix.CreateTranslation(new Vector3(-shipOrigin, 0.0f)) *
                // Matrix.CreateScale(block.Scale) *  would go here
                Matrix.CreateRotationZ(shipRotation) *
                Matrix.CreateTranslation(new Vector3(shipPosition, 0.0f));
            // Calculate the bounding rectangle of this block in world space
            Rectangle shipRectangle = CalculateBoundingRectangle(
                        new Rectangle(0, 0, shipTexture.Width, shipTexture.Height),
                        shipTransform);
            for(int i = 0; i < asteroids.Count; i++)
            {
                Asteroid a = asteroids[i];
                a.Position -= new Vector2((float)Math.Sin(a.Rotation) * Asteroid.SPEED, -(float)Math.Cos(a.Rotation) * Asteroid.SPEED);

                if (a.Position.X < 0)
                {
                    a.Position.X = viewport.Width;
                }
                if (a.Position.X > viewport.Width)
                {
                    a.Position.X = 0;
                }
                if (a.Position.Y < 0)
                {
                    a.Position.Y = viewport.Height;
                }
                if (a.Position.Y > viewport.Height)
                {
                    a.Position.Y = 0;
                }
                Matrix aTransform =
                    Matrix.CreateTranslation(new Vector3(-a.Origin, 0.0f)) *
                    // Matrix.CreateScale(block.Scale) *  would go here
                    Matrix.CreateRotationZ(a.Rotation) *
                    Matrix.CreateTranslation(new Vector3(a.Position, 0.0f));
                Rectangle aRectangle = CalculateBoundingRectangle(
                            new Rectangle(0, 0, a.Texture.Width, a.Texture.Height),
                            aTransform);

                asteroidTextureData = new Color[a.Texture.Width * a.Texture.Height];
                a.Texture.GetData(asteroidTextureData);

                if (shipRectangle.Intersects(aRectangle))
                {
                    // Check collision with bullet
                    if (IntersectPixels(shipTransform, shipTexture.Width,
                                        shipTexture.Height, shipTextureData,
                                        aTransform, a.Texture.Width,
                                        a.Texture.Height, asteroidTextureData))
                    {
                        respawn();
                    }
                }

                for (int j = 0; j < projectiles.Count; j++ )
                {
                    Projectile p = projectiles[j];
                    Matrix pTransform =
                        Matrix.CreateTranslation(new Vector3(-p.Origin, 0.0f)) *
                        // Matrix.CreateScale(block.Scale) *  would go here
                        Matrix.CreateRotationZ(p.Rotation) *
                        Matrix.CreateTranslation(new Vector3(p.Position, 0.0f));
                    Rectangle pRectangle = CalculateBoundingRectangle(
                                new Rectangle(0, 0, bulletTexture.Width, bulletTexture.Height),
                                pTransform);

                    bulletTextureData = new Color[bulletTexture.Width * bulletTexture.Height];
                    bulletTexture.GetData(bulletTextureData);
                    if (pRectangle.Intersects(aRectangle))
                    {
                        // Check collision with bullet
                        if (IntersectPixels(pTransform, bulletTexture.Width,
                                            bulletTexture.Height, bulletTextureData,
                                            aTransform, a.Texture.Width,
                                            a.Texture.Height, asteroidTextureData))
                        {
                            Console.WriteLine("hit");
                            asteroids.RemoveAt(i);
                            projectiles.RemoveAt(j);
                            Texture2D tex = a.Texture;
                            if (tex == asteroidBTexture || tex == asteroidMTexture)
                            {
                                if (tex == asteroidBTexture)
                                    tex = asteroidMTexture;
                                else
                                    tex = asteroidSTexture;

                                asteroids.Add(new Asteroid(new Vector2(a.Position.X, a.Position.Y), new Vector2(tex.Width / 2, tex.Height / 2), p.Rotation + (float)(Math.PI / 4), tex));
                                asteroids.Add(new Asteroid(new Vector2(a.Position.X, a.Position.Y), new Vector2(tex.Width / 2, tex.Height / 2), p.Rotation - (float)(Math.PI / 4), tex));
                            }
                            i++;
                            j++;
                        }
                    }
                }
            }

            if (shipPosition.X < 0)
            {
                shipPosition.X = viewport.Width;
            }
            if (shipPosition.X > viewport.Width)
            {
                shipPosition.X = 0;
            }
            if (shipPosition.Y < 0)
            {
                shipPosition.Y = viewport.Height;
            }
            if (shipPosition.Y > viewport.Height)
            {
                shipPosition.Y = 0;
            }

            OldKeyState = NewKeyState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;
            device.Clear(Color.Black);
            


            spriteBatch.Begin();

            // Draw person
            spriteBatch.Draw(shipTexture, shipPosition, null, Color.White,
                shipRotation, shipOrigin, 1.0f, SpriteEffects.None, 0.0f);
            foreach (Projectile pro in projectiles)
            {
                spriteBatch.Draw(bulletTexture, pro.Position, Color.White);
            }
            foreach (Asteroid astrd in asteroids)
            {
                //spriteBatch.Draw(asteroidTexture, astrd.Position, Color.White);
                spriteBatch.Draw(astrd.Texture, astrd.Position, null, Color.White,
                astrd.Rotation, astrd.Origin, 1.0f, SpriteEffects.None, 0.0f);
            }
            spriteBatch.End();


            base.Draw(gameTime);
        }


        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites.
        /// </summary>
        /// <param name="rectangleA">Bounding rectangle of the first sprite</param>
        /// <param name="dataA">Pixel data of the first sprite</param>
        /// <param name="rectangleB">Bouding rectangle of the second sprite</param>
        /// <param name="dataB">Pixel data of the second sprite</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                           Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }


        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(
                            Matrix transformA, int widthA, int heightA, Color[] dataA,
                            Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }


        /// <summary>
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                                           Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
    }
}
