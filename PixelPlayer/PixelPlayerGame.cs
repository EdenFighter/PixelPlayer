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

        Material[] materials = new Material[5];
        Level level;
        static int blocksize = 20;
        Vector2 gamefielPosition;
        Texture2D[] playerTextures = new Texture2D[4];
        Texture2D bombTexture;

        Boolean buttonRightBackPressed = false;
        Item[] allItems = new Item[0];

        Player player;

        float gameSpeed = 200f;

        public PixelPlayerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
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

            gamefielPosition = new Vector2(0, 0);

            level = new Level();
            for (int x = 0; x < level.mapwidth / 2; x++)
            {
                level.Blocks[x, 5] = new Block(materials[1]);

                for (int y = 6; y < 8; y++)
                {
                    level.Blocks[x, y] = new Block(materials[0]);
                }
            }

            for (int x = 5; x < level.mapwidth - 6; x++)
            {
                level.Blocks[x, 13] = new Block(materials[1]);
            }
            level.Blocks[(level.mapwidth - 6), 13] = new Block(materials[0]);
            level.Blocks[(level.mapwidth - 6), 12] = new Block(materials[1]);
            for (int x = (level.mapwidth - 5); x < level.mapwidth; x++)
            {
                level.Blocks[x, 11] = new Block(materials[1]);

                for (int y = 12; y < 14; y++)
                {
                    level.Blocks[x, y] = new Block(materials[0]);
                }
            }
            for (int x = 0; x < level.mapwidth; x++)
            {
                for (int y = 14; y < level.mapheight; y++)
                {
                    level.Blocks[x, y] = new Block(materials[0]);
                }
            }
            for (int x = 0; x < 5; x++)
            {
                for (int y = 8; y < 14; y++)
                {
                    level.Blocks[x, y] = new Block(materials[0]);
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
            materials[1] = new Material(Content.Load<Texture2D>("Textures/dirt_grass"));
            materials[2] = new Material(Content.Load<Texture2D>("Textures/grass"));

            bombTexture = Content.Load<Texture2D>("Textures/ore_ruby");

            playerTextures[0] = Content.Load<Texture2D>("Textures/male_head");
            playerTextures[1] = Content.Load<Texture2D>("Textures/male_body");
            playerTextures[2] = Content.Load<Texture2D>("Textures/male_arm");
            playerTextures[3] = Content.Load<Texture2D>("Textures/male_leg");

            player = new Player(new Vector2(0, 0), new Vector2(36, 96), playerTextures[0], playerTextures[1], playerTextures[2], playerTextures[3]);

            // TODO: use this.Content to load your game content here
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

            if(state.Buttons.A == ButtonState.Pressed && !buttonRightBackPressed)
            {
                Item[] temp = new Item[allItems.Length + 1];
                for(int i = 0; i < allItems.Length; i++)
                {
                    temp[i] = allItems[i];
                }
                temp[allItems.Length] = new Item(new Vector2(player.position.X + (player.size.X/2), player.position.Y + (player.size.Y / 3)), new Vector2(20,20), bombTexture);
                allItems = temp;

                buttonRightBackPressed = true;
            }
            if(state.Buttons.A == ButtonState.Released) buttonRightBackPressed = false;



            //Enviroment
            player.velocity += new Vector2(0, 20f);
            for (int i = 0; i < allItems.Length; i++)
            {
                allItems[i].velocity += new Vector2(0, 20f);
                if (allItems[i].velocity.X > 1) allItems[i].velocity += new Vector2(-1, 0);
                else if (allItems[i].velocity.X < -1) allItems[i].velocity += new Vector2(1, 0);
                else allItems[i].velocity = new Vector2(0, allItems[i].velocity.Y);
            }

            //Player Input
            player.movement = new Vector2(state.ThumbSticks.Left.X * player.speed * (float)gameTime.ElapsedGameTime.TotalSeconds, -(state.ThumbSticks.Left.Y * player.jumpEnergy * (float)gameTime.ElapsedGameTime.TotalSeconds));

            gamefielPosition -= new Vector2(state.ThumbSticks.Right.X * gameSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, -(state.ThumbSticks.Right.Y * gameSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds));

            //gamefielPosition += (10 * playerVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);

            //Collision
            Vector2 velocity = ((player.velocity + (gameSpeed * player.movement)) * (float)gameTime.ElapsedGameTime.TotalSeconds);

            Rectangle playerBoxLeft = new Rectangle((int)(player.position.X - (velocity.X - 1f)), (int)player.position.Y + 2, 1, (int)player.size.Y - 4);
            Rectangle playerBoxRight = new Rectangle((int)(player.position.X + (velocity.X - 1f) + player.size.X), (int)player.position.Y + 2, 1, (int)player.size.Y - 4);
            Rectangle playerBoxTop = new Rectangle((int)player.position.X + 2, (int)(player.position.Y - (velocity.Y - 1f)), (int)player.size.X - 4, 1);
            Rectangle playerBoxBottom = new Rectangle((int)player.position.X + 2, (int)(player.position.Y + (velocity.Y - 1f) + player.size.Y), (int)player.size.X - 4, 1);

            for (int x = 0; x < level.mapwidth; x++)
            {
                for (int y = 0; y < level.mapheight; y++)
                {
                    Block block = level.Blocks[x, y];
                    if (!(block == null))
                    {
                        Rectangle box = new Rectangle(x * blocksize, y * blocksize, blocksize, blocksize);
                        if(box.Intersects(playerBoxBottom) && velocity.Y > 0)
                        {
                            velocity = new Vector2 (velocity.X, 0);
                            player.velocity = new Vector2(0, 0);
                        }
                        if (box.Intersects(playerBoxTop) && velocity.Y < 0)
                        {
                            velocity = new Vector2(velocity.X, 0);
                        }
                        if (box.Intersects(playerBoxLeft) && velocity.X < 0)
                        {
                            velocity = new Vector2(0, velocity.Y);
                        }
                        if (box.Intersects(playerBoxRight) && velocity.X > 0)
                        {
                            velocity = new Vector2(0, velocity.Y);
                        }
                    }
                }
            }

            //Movement
            player.position += velocity;

            for (int i = 0; i < allItems.Length; i++)
            {
                allItems[i].position += allItems[i].velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

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

            for (int x = 0; x < level.mapwidth; x++)
            {
                for (int y = 0; y < level.mapheight; y++)
                {
                    Block block = level.Blocks[x, y];
                    if (!(block == null))
                    {
                        spriteBatch.Draw(block.material.texture, new Rectangle((int)(gamefielPosition.X + (x * 20)), (int)(gamefielPosition.Y + (y * 20)), blocksize, blocksize), Color.White);
                    }
                }
            }

            player.Draw(spriteBatch, gamefielPosition);

            for (int i = 0; i < allItems.Length; i++)
            {
                spriteBatch.Draw(allItems[i].texture, new Rectangle((int)(allItems[i].position.X + gamefielPosition.X), (int)(allItems[i].position.Y + gamefielPosition.Y), (int)allItems[i].size.X, (int)allItems[i].size.Y), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
