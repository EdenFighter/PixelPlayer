using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PixelPlayer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class PixelPlayerGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Vector2 windowResolution;

        Material[] materials = new Material[5];
        World world;
        Vector2 cameraPosition;
        GameItem bomb;
        Texture2D[] playerTextures = new Texture2D[4];
        Texture2D bombTexture;
        public static Texture2D testTexture;

        GamePlayer player;

        public PixelPlayerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            windowResolution = new Vector2(1280,720);
            graphics.PreferredBackBufferWidth = (int)windowResolution.X;
            graphics.PreferredBackBufferHeight = (int)windowResolution.Y;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            world = new World(new Vector2(5, 3));
            player = new GamePlayer(new Vector2(80, 80), new Vector2(36, 96), world);

            cameraPosition = new Vector2(0, 0);

            for (int x = 0; x < (World.chunkSizeX / 2); x++)
            {
                for (int y = 0; y < World.chunkSizeY; y++)
                {
                    if (y > ((World.chunkSizeY / 2) - (x / 4)))
                    {
                        if (y < (World.chunkSizeY / 4) * 3)
                        {
                            world.allChunks[0, 0].Blocks[x, y] = new Block(materials[0], new Vector2(x, y), new Vector2(0, 0));
                            world.allChunks[1, 0].Blocks[x, y] = new Block(materials[0], new Vector2(x, y), new Vector2(1, 0));
                            world.allChunks[2, 0].Blocks[x, y] = new Block(materials[0], new Vector2(x, y), new Vector2(2, 0));
                            world.allChunks[3, 0].Blocks[x, y] = new Block(materials[0], new Vector2(x, y), new Vector2(3, 0));
                            world.allChunks[4, 0].Blocks[x, y] = new Block(materials[0], new Vector2(x, y), new Vector2(4, 0));
                        }
                        else
                        {
                            world.allChunks[0, 0].Blocks[x, y] = new Block(materials[1], new Vector2(x, y), new Vector2(0, 0));
                            world.allChunks[1, 0].Blocks[x, y] = new Block(materials[1], new Vector2(x, y), new Vector2(1, 0));
                            world.allChunks[2, 0].Blocks[x, y] = new Block(materials[1], new Vector2(x, y), new Vector2(2, 0));
                            world.allChunks[3, 0].Blocks[x, y] = new Block(materials[1], new Vector2(x, y), new Vector2(3, 0));
                            world.allChunks[4, 0].Blocks[x, y] = new Block(materials[1], new Vector2(x, y), new Vector2(4, 0));
                        }
                    }
                }
            }
            for (int x = (World.chunkSizeX / 2); x < World.chunkSizeX; x++)
            {
                for (int y = 0; y < World.chunkSizeY; y++)
                {
                    if (y > ((World.chunkSizeY / 2) - ((World.chunkSizeX - x) / 4)))
                    {
                        if (y < (World.chunkSizeY / 4) * 3)
                        {
                            world.allChunks[0, 0].Blocks[x, y] = new Block(materials[0], new Vector2(x, y), new Vector2(0, 0));
                            world.allChunks[1, 0].Blocks[x, y] = new Block(materials[0], new Vector2(x, y), new Vector2(1, 0));
                            world.allChunks[2, 0].Blocks[x, y] = new Block(materials[0], new Vector2(x, y), new Vector2(2, 0));
                            world.allChunks[3, 0].Blocks[x, y] = new Block(materials[0], new Vector2(x, y), new Vector2(3, 0));
                            world.allChunks[4, 0].Blocks[x, y] = new Block(materials[0], new Vector2(x, y), new Vector2(4, 0));
                        }
                        else
                        {
                            world.allChunks[0, 0].Blocks[x, y] = new Block(materials[1], new Vector2(x, y), new Vector2(0, 0));
                            world.allChunks[1, 0].Blocks[x, y] = new Block(materials[1], new Vector2(x, y), new Vector2(1, 0));
                            world.allChunks[2, 0].Blocks[x, y] = new Block(materials[1], new Vector2(x, y), new Vector2(2, 0));
                            world.allChunks[3, 0].Blocks[x, y] = new Block(materials[1], new Vector2(x, y), new Vector2(3, 0));
                            world.allChunks[4, 0].Blocks[x, y] = new Block(materials[1], new Vector2(x, y), new Vector2(4, 0));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            materials[0] = new Material(Content.Load<Texture2D>("Textures/dirt"));
            materials[1] = new Material(Content.Load<Texture2D>("Textures/stone"));
            testTexture = Content.Load<Texture2D>("Textures/lava");

            bombTexture = Content.Load<Texture2D>("Textures/bomb");

            GamePlayer.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            GamePadState state = GamePad.GetState(PlayerIndex.One);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed)
            {
                for (int x = 0; x < 64; x++)
                {
                    for (int y = 0; y < (World.chunkSizeY / 2); y++)
                    {
                        world.allChunks[0, 0].Blocks[x, y] = null;
                    }
                }
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
            {
                bomb = new GameItem(new Vector2(player.position.X + (player.size.X / 2) - 20, player.position.Y + (player.size.Y / 2) - 20),new Vector2(40,40), bombTexture, world);
                bomb.velocity = new Vector2(state.ThumbSticks.Right.X * 20 * (float)gameTime.ElapsedGameTime.TotalSeconds, -(state.ThumbSticks.Right.Y * 20 * (float)gameTime.ElapsedGameTime.TotalSeconds)) * 20;
            }

            //Enviroment
            world.Update(gameTime);

            //Camera Movement
            cameraPosition = -(new Vector2(player.position.X - (graphics.PreferredBackBufferWidth / 2) + (player.size.X / 2), player.position.Y - (graphics.PreferredBackBufferHeight / 2) + (player.size.Y / 2)));

            //Movement
            player.Update(gameTime);

            if (bomb != null) bomb.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            world.Draw(spriteBatch, cameraPosition, windowResolution);

            for (int y = 0; y < world.worldsize.Y; y++)
            {
                world.allChunks[(int)world.worldsize.X - 1, y].Draw(spriteBatch, cameraPosition + (new Vector2(World.chunkSizeX * World.blockSize * -1, World.chunkSizeY * World.blockSize * y)), windowResolution);
                world.allChunks[0, y].Draw(spriteBatch, cameraPosition + (new Vector2(World.chunkSizeX * World.blockSize * (int)world.worldsize.X, World.chunkSizeY * World.blockSize * y)), windowResolution);
            }

            player.Draw(spriteBatch, cameraPosition);
            if(bomb != null) bomb.Draw(spriteBatch, cameraPosition);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
