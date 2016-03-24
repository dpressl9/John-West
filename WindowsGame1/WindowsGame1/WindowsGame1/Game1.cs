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

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Establish GDM and SpriteBatches.
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch, Background, Character;

        //Establish "past" and "current" KeyboardStates.
        KeyboardState oldState, newState;

        //Establish motionState to hold 0-15, each value corresponding to one of the possible combinations from UDLR Key-Downs.
        byte motionState = 0;
        int counter = 0;
        bool toggle = false;
        const float walkingSpeed = 100.0f; //Arbitrary default movement speed for player.
        const float runningMultiplier = 1.25f; //Arbitrary multiplier in case of Shift or some defined RunKey being defined. being pressed.
        float angularSpeed = (float)Math.Sqrt(Convert.ToDouble(2 * (walkingSpeed * walkingSpeed))); //Magic. MAGIC DOESN'T WORK. DO NOT TOUCH FOR NOW.
        float angleSpeed = (float)((1+ (walkingSpeed - (Math.Sqrt(Convert.ToDouble(2 * walkingSpeed * walkingSpeed)))) / walkingSpeed) * walkingSpeed); //MAGIC. This one "works."

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;

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
            oldState = Keyboard.GetState();
            
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 

        Texture2D myTile, ROOM1_FLOOR, JohnWest, ROOM1_WALLS, ROOM1_SWITCH, ROOM1_BACKGROUND, ROOM_DOOR_LEFT, ROOM_DOOR_RIGHT, ROOM_DOOR_TOP, ROOM_DOOR_BOTTOM;
        SoundEffect walking;
        SoundEffectInstance walkingInstance;

        //Vector2 spritePosition = new Vector2(50.0f, 50.0f);

        Vector2 spritePosition;
        Vector2 doorPosition;
        Vector2 spriteSpeed = new Vector2(50.0f, 50.0f);

        Vector2 velocity;

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Background = new SpriteBatch(GraphicsDevice);
            Character = new SpriteBatch(GraphicsDevice);
            myTile = Content.Load<Texture2D>("Room_Assets\\Room_1\\GroundTile1");
            ROOM1_FLOOR = Content.Load<Texture2D>("Room_Assets\\Room_1\\ROOM1_FLOOR");
            JohnWest = Content.Load<Texture2D>("John_West\\JohnWest_RIGHT_DOWNSCALE");
            ROOM1_WALLS = Content.Load<Texture2D>("Room_Assets\\Room_1\\ROOM1_WALLS");
            ROOM1_SWITCH = Content.Load<Texture2D>("Room_Assets\\Room_1\\ROOM1_SWITCH");
            ROOM1_BACKGROUND = Content.Load<Texture2D>("Room_Assets\\Room_1\\ROOM1_BACKGROUND_TOP_DOOR");
            ROOM_DOOR_TOP = Content.Load<Texture2D>("Room_Assets\\Room_1\\DOOR_TOP_DEFAULT");
            ROOM_DOOR_BOTTOM = Content.Load<Texture2D>("Room_Assets\\Room_1\\ROOM_DOOR_OPEN_RIGHT");
            ROOM_DOOR_LEFT = Content.Load<Texture2D>("Room_Assets\\Room_1\\ROOM_DOOR_LEFT");
            ROOM_DOOR_RIGHT = Content.Load<Texture2D>("Room_Assets\\Room_1\\ROOM_DOOR_RIGHT");
            walking = Content.Load<SoundEffect>("ws1");

            doorPosition = new Vector2(((graphics.GraphicsDevice.Viewport.Width - ROOM_DOOR_TOP.Width) / 2), (graphics.GraphicsDevice.Viewport.Height - ROOM1_WALLS.Height) / 2 - ROOM_DOOR_TOP.Height);
            //walkingInstance = walking.CreateInstance();
            //walkingInstance.IsLooped = false;
            spritePosition = new Vector2(((graphics.GraphicsDevice.Viewport.Width - JohnWest.Width) / 2.0f), ((graphics.GraphicsDevice.Viewport.Height - JohnWest.Height) / 2.0f));

            // EXAMPLE: myCharacter = Content.Load<Texture2D>("Character");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            // TODO: Add your update logic here

            UpdateInput();

            if (newState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (newState.IsKeyDown(Keys.E))
            {

                if (Vector2.Distance(new Vector2(((graphics.GraphicsDevice.Viewport.Width / 2) - 99), (((graphics.GraphicsDevice.Viewport.Height - ROOM1_WALLS.Height) / 2) - 59)), spritePosition) < 100.0f)
                {
                    toggle = true;
                }
            }

            if (toggle)
            {

                doorPosition += new Vector2(0.0f, 50.0f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(doorPosition.Y > 91 + ROOM_DOOR_TOP.Height)
                {
                    ROOM_DOOR_TOP.Dispose();
                    toggle = false;
                }
            }


            //The following if-statement assigns values to motionState for later velocity alteration.
            if(!newState.Equals(oldState))
            {
                if (newState.IsKeyDown(Keys.Up))
                {
                    if (newState.IsKeyDown(Keys.Down))
                    {
                        if (newState.IsKeyDown(Keys.Left))
                        {
                            if (newState.IsKeyDown(Keys.Right))
                            {
                                motionState = 0;
                            }
                            else
                            {
                                motionState = 1;
                            }
                        }
                        else
                        {
                            if (newState.IsKeyDown(Keys.Right))
                            {
                                motionState = 2;
                            }
                            else
                            {
                                motionState = 3;
                            }
                        }
                    }
                    else
                    {
                        if (newState.IsKeyDown(Keys.Left))
                        {
                            if (newState.IsKeyDown(Keys.Right))
                            {
                                motionState = 4;
                            }
                            else
                            {
                                motionState = 5;
                            }
                        }
                        else
                        {
                            if (newState.IsKeyDown(Keys.Right))
                            {
                                motionState = 6;
                            }
                            else
                            {
                                motionState = 7;
                            }
                        }
                    }
                }
                else
                {
                    if (newState.IsKeyDown(Keys.Down))
                    {
                        if (newState.IsKeyDown(Keys.Left))
                        {
                            if (newState.IsKeyDown(Keys.Right))
                            {
                                motionState = 8;
                            }
                            else
                            {
                                motionState = 9;
                            }
                        }
                        else
                        {
                            if (newState.IsKeyDown(Keys.Right))
                            {
                                motionState = 10;
                            }
                            else
                            {
                                motionState = 11;
                            }
                        }
                    }
                    else
                    {
                        if (newState.IsKeyDown(Keys.Left))
                        {
                            if (newState.IsKeyDown(Keys.Right))
                            {
                                motionState = 12;
                            }
                            else
                            {
                                motionState = 13;
                            }
                        }
                        else
                        {
                            if (newState.IsKeyDown(Keys.Right))
                            {
                                motionState = 14;
                            }
                            else
                            {
                                motionState = 15;
                            }
                        }
                    }
                }
            }
            //Switch Case for motionState to assign velocity values.
            switch (motionState)
            {
                case 0:
                    velocity.X = 0;
                    velocity.Y = 0;
                    break;
                case 1:
                    velocity.X = -walkingSpeed;
                    velocity.Y = 0;
                    Face(false);
                    break;
                case 2:
                    velocity.X = walkingSpeed;
                    velocity.Y = 0;
                    Face(true);
                    break;
                case 3:
                    goto case 0;
                case 4:
                    velocity.X = 0;
                    velocity.Y = -walkingSpeed;
                    break;
                case 5:
                    velocity.X = -angleSpeed;
                    velocity.Y = -angleSpeed;
                    Face(false);
                    break;
                case 6:
                    velocity.X = angleSpeed;
                    velocity.Y = -angleSpeed;
                    Face(true);
                    break;
                case 7:
                    goto case 4;
                case 8:
                    velocity.X = 0;
                    velocity.Y = walkingSpeed;
                    break;
                case 9:
                    velocity.X = -angleSpeed;
                    velocity.Y = angleSpeed;
                    Face(false);
                    break;
                case 10:
                    velocity.X = angleSpeed;
                    velocity.Y = angleSpeed;
                    Face(true);
                    break;
                case 11:
                    goto case 8;
                case 12:
                    goto case 0;
                case 13:
                    goto case 1;
                case 14:
                    goto case 2;
                case 15:
                    goto case 0;
            }
            //post-motionState Check to stop when motionKeys are unpressed.
            if (newState.IsKeyUp(Keys.Up) && newState.IsKeyUp(Keys.Down) && newState.IsKeyUp(Keys.Left) && newState.IsKeyUp(Keys.Right))
            {
                velocity = new Vector2(0.0f, 0.0f);
            }
            //post-motionState Check to run.
            if (newState.IsKeyDown(Keys.LeftShift) || newState.IsKeyDown(Keys.RightShift))
            {
                velocity.X *= runningMultiplier;
                velocity.Y *= runningMultiplier;
            }

            spritePosition += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            int MaxX = graphics.GraphicsDevice.Viewport.Width - ((graphics.GraphicsDevice.Viewport.Width - 1280) / 2) - JohnWest.Width;
            int MinX = (graphics.GraphicsDevice.Viewport.Width - 1280) / 2;
            int MaxY = graphics.GraphicsDevice.Viewport.Height - ((graphics.GraphicsDevice.Viewport.Height - 720) / 2) - JohnWest.Height;
            int MinY = ((graphics.GraphicsDevice.Viewport.Height - 720) / 2) - (JohnWest.Height * 3 / 4);

            if (spritePosition.X > MaxX)
            {
                velocity.X *= 0;
                spritePosition.X = MaxX;
            }
            else if (spritePosition.X < MinX)
            {
                velocity.X *= 0;
                spritePosition.X = MinX;
            }

            if (spritePosition.Y > MaxY)
            {
                velocity.Y *= 0;
                spritePosition.Y = MaxY;
            }
            else if (spritePosition.Y < MinY)
            {
                velocity.Y *= 0;
                spritePosition.Y = MinY;
            }

            base.Update(gameTime);
        }

        private void UpdateInput()
        {
            newState = Keyboard.GetState();

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            // Draw the sprite.
            

            

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (!ROOM_DOOR_TOP.IsDisposed)
            {
                spriteBatch.Draw(ROOM_DOOR_TOP, new Rectangle((int)doorPosition.X, (int)doorPosition.Y, ROOM_DOOR_TOP.Width, ROOM_DOOR_TOP.Height), null, Color.DarkGray, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, 1.0f);
            }

            spriteBatch.End();





            for (int i = (graphics.GraphicsDevice.Viewport.Width - ROOM1_WALLS.Width) / 2; i < (graphics.GraphicsDevice.Viewport.Width) - ((graphics.GraphicsDevice.Viewport.Width - ROOM1_WALLS.Width) / 2); i += ROOM1_FLOOR.Width)
            {
                for (int j = (graphics.GraphicsDevice.Viewport.Height - ROOM1_WALLS.Height) / 2; j < (graphics.GraphicsDevice.Viewport.Height) - ((graphics.GraphicsDevice.Viewport.Height - ROOM1_WALLS.Height) / 2); j += ROOM1_FLOOR.Height)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    spriteBatch.Draw(ROOM1_FLOOR, new Vector2(i, j), Color.SandyBrown);
                    spriteBatch.End();
                }
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            
            spriteBatch.Draw(ROOM1_BACKGROUND, new Vector2(0.0f, 0.0f), Color.White);
            
            spriteBatch.Draw(ROOM1_SWITCH, new Vector2(((graphics.GraphicsDevice.Viewport.Width / 2) - 99), (((graphics.GraphicsDevice.Viewport.Height - ROOM1_WALLS.Height) / 2) - 59)), Color.White);



            spriteBatch.End();


            


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            spriteBatch.Draw(JohnWest, spritePosition, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void Face(bool isRight)
        {
            if (isRight)
            {
                JohnWest = Content.Load<Texture2D>("John_West\\JohnWest_RIGHT_DOWNSCALE");
            }
            else
            {
                JohnWest = Content.Load<Texture2D>("John_West\\JohnWest_LEFT_DOWNSCALE");
            }
        }
        protected void SinkDoor()
        {

        }
    }
}
