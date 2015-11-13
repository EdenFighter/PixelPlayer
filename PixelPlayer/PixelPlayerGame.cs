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
        Texture2D[] playerTextures = new Texture2D[4];
        Texture2D bombTexture;
        public static Texture2D testTexture;
        public static Texture2D testTexture2;

        GamePlayer player;

        public PixelPlayerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            windowResolution = new Vector2(1280,720);
            graphics.PreferredBackBufferWidth = (int)windowResolution.X;
            graphics.PreferredBackBufferHeight = (int)windowResolution.Y;
        }

        protected override void Initialize()
        {
            base.Initialize();

            world = new World(new Vector2(5, 3));
            player = new GamePlayer(new Vector2(80, 80), new Vector2(36, 96), world);

            cameraPosition = new Vector2(0, 0);

            for (int x = 0; x < World.chunkSizeX; x++)
            {
                for (int y = 0; y < World.chunkSizeY; y++)
                {
                    world.allChunks[0, 1].Blocks[x, y] = new Block(materials[2]);
                    //world.allChunks[0, 1].Blocks[x, y].fulllevel = World.blockSize * ((World.blockSize / 4) * 3);
                    world.allChunks[1, 1].Blocks[x, y] = new Block(materials[1]);
                    //world.allChunks[1, 1].Blocks[x, y].fulllevel = World.blockSize * ((World.blockSize / 4) * 3);
                }
            }

            for (int x = 0; x < (World.chunkSizeX / 2); x++)
            {
                for (int y = 0; y < World.chunkSizeY; y++)
                {
                    if (y > ((World.chunkSizeY / 2) - (x / 4)))
                    {
                        if (y < (World.chunkSizeY / 4) * 3)
                        {
                            world.allChunks[0, 1].Blocks[x, y] = new Block(materials[0]);
                            world.allChunks[1, 1].Blocks[x, y] = new Block(materials[0]);
                            world.allChunks[2, 1].Blocks[x, y] = new Block(materials[0]);
                            world.allChunks[3, 1].Blocks[x, y] = new Block(materials[0]);
                            world.allChunks[4, 1].Blocks[x, y] = new Block(materials[0]);
                        }
                        else
                        {
                            world.allChunks[0, 1].Blocks[x, y] = new Block(materials[1]);
                            world.allChunks[1, 1].Blocks[x, y] = new Block(materials[1]);
                            world.allChunks[2, 1].Blocks[x, y] = new Block(materials[1]);
                            world.allChunks[3, 1].Blocks[x, y] = new Block(materials[1]);
                            world.allChunks[4, 1].Blocks[x, y] = new Block(materials[1]);
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
                            world.allChunks[0, 1].Blocks[x, y] = new Block(materials[0]);
                            world.allChunks[1, 1].Blocks[x, y] = new Block(materials[0]);
                            world.allChunks[2, 1].Blocks[x, y] = new Block(materials[0]);
                            world.allChunks[3, 1].Blocks[x, y] = new Block(materials[0]);
                            world.allChunks[4, 1].Blocks[x, y] = new Block(materials[0]);
                        }
                        else
                        {
                            world.allChunks[0, 1].Blocks[x, y] = new Block(materials[1]);
                            world.allChunks[1, 1].Blocks[x, y] = new Block(materials[1]);
                            world.allChunks[2, 1].Blocks[x, y] = new Block(materials[1]);
                            world.allChunks[3, 1].Blocks[x, y] = new Block(materials[1]);
                            world.allChunks[4, 1].Blocks[x, y] = new Block(materials[1]);
                        }
                    }
                }
            }
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            materials[0] = new Material(Content.Load<Texture2D>("Textures/dirt"), Material.Type.solid);
            materials[1] = new Material(Content.Load<Texture2D>("Textures/stone"), Material.Type.solid);
            materials[2] = new Material(Content.Load<Texture2D>("Textures/water"), Material.Type.liquid);
            materials[3] = new Material(Content.Load<Texture2D>("Textures/lava"), Material.Type.liquid);

            bombTexture = Content.Load<Texture2D>("Textures/bomb");

            GamePlayer.LoadContent(Content);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        bool APressed = false;
        bool BPressed = false;

        protected override void Update(GameTime gameTime)
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if ((GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.B)) && !BPressed)
            {
                for (int x = 0; x < 64; x++)
                {
                    for (int y = 0; y < (World.chunkSizeY); y++)
                    {
                        if (world.allChunks[1, 1].Blocks[x, y] != null)
                        {
                            world.allChunks[1, 1].Blocks[x, y] = null;
                        }
                        else
                        {
                            world.allChunks[1, 1].Blocks[x, y] = new Block(materials[1]);
                        }
                    }
                }
                world.allChunks[0, 1].isActive = !world.allChunks[0, 1].isActive;
                BPressed = true;
            }
            else if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Released && Keyboard.GetState().IsKeyUp(Keys.B))
            {
                BPressed = false;
            }
            if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F)) && !APressed)
            {
                GameItem bomb = new GameItem(new Vector2(player.position.X + (player.size.X / 2) - 20, player.position.Y + (player.size.Y / 2) - 20),new Vector2(40,40), bombTexture, world);
                bomb.velocity = new Vector2(state.ThumbSticks.Right.X * 20 * (float)gameTime.ElapsedGameTime.TotalSeconds, -(state.ThumbSticks.Right.Y * 20 * (float)gameTime.ElapsedGameTime.TotalSeconds)) * 20;
                APressed = true;
            }
            else if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Released && Keyboard.GetState().IsKeyUp(Keys.F))
            {
                APressed = false;
            }

            //Enviroment
            world.Update(gameTime);

            //Camera Movement
            cameraPosition = -(new Vector2(player.position.X - (graphics.PreferredBackBufferWidth / 2) + (player.size.X / 2), player.position.Y - (graphics.PreferredBackBufferHeight / 2) + (player.size.Y / 2)));

            //Movement
            player.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            world.Draw(spriteBatch, cameraPosition, windowResolution);

            for (int y = 0; y < world.worldsize.Y; y++)
            {
                world.allChunks[(int)world.worldsize.X - 1, y].DrawBlocks(spriteBatch, cameraPosition + (new Vector2(World.chunkSizeX * World.blockSize * -1, World.chunkSizeY * World.blockSize * y)), windowResolution);
                world.allChunks[0, y].DrawBlocks(spriteBatch, cameraPosition + (new Vector2(World.chunkSizeX * World.blockSize * (int)world.worldsize.X, World.chunkSizeY * World.blockSize * y)), windowResolution);
            }

            player.Draw(spriteBatch, cameraPosition);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
