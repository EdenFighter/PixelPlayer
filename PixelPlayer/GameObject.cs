using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PixelPlayer
{
    abstract class GameObject
    {
        public Vector2 position { get; set; }
        public Vector2 velocity { get; set; }
        public Vector2 size { get; }
        protected World world;
        protected Vector2 _currentChunkPosition;
        protected BoundingBox2D _boundingBox;

        public GameObject(Vector2 position, Vector2 size, World world)
        {
            this.position = position;
            this.size = size;
            this.world = world;
        }

        public BoundingBox2D boundingBox
        {
            get { return _boundingBox; }
        }
        public Vector2 currentChunkPosition
        {
            get { return _currentChunkPosition; }
        }
        
        abstract public void Update(GameTime gameTime);
        abstract public void Draw(SpriteBatch spriteBatch, Vector2 CameraPosition);
    }

    class GameItem : GameObject
    {
        public Texture2D texture { get; }

        public GameItem(Vector2 position, Vector2 size , Texture2D texture, World world) : base(position, size, world)
        {
            this.texture = texture;
            _boundingBox = new BoundingBox2D(this, size);
        }

        public override void Update(GameTime gameTime)
        {
            velocity += new Vector2(0, World.gravity) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (velocity.X < 0.1 && velocity.X > -0.1) { velocity = new Vector2(0, velocity.Y); }
            else { velocity -= new Vector2(((velocity.X / 4)  * 3), 0) * (float)gameTime.ElapsedGameTime.TotalSeconds; }

            this.boundingBox.position = position + new Vector2(velocity.X, 0);
            //For each Block in the Chunk
            for (int x = 0; x < World.chunkSizeX; x++)
            {
                for (int y = 0; y < World.chunkSizeY; y++)
                {
                    //For all Chunks around the current one
                    for (int chunkX = -1; chunkX <= 1; chunkX++)
                    {
                        for (int chunkY = -1; chunkY <= 1; chunkY++)
                        {
                            //If Chunk is at the other side of the playfield
                            int chunkposX = (int)(chunkX + _currentChunkPosition.X);
                            int chunkposY = (int)(chunkY + _currentChunkPosition.Y);
                            if (chunkposX < 0) chunkposX = (int)world.worldsize.X - 1;
                            if (chunkposY < 0) chunkposY = (int)world.worldsize.Y - 1;
                            if (chunkposX > (int)world.worldsize.X - 1) chunkposX = 0;
                            if (chunkposY > (int)world.worldsize.Y - 1) chunkposY = 0;

                            Chunk chunkToTest = world.allChunks[chunkposX, chunkposY];

                            //Check collison for each Block in the chunkToTest
                            if (chunkToTest.Blocks[x, y] != null)
                            {
                                if (boundingBox.collides(chunkToTest.Blocks[x, y]))
                                {
                                    if (velocity.X / 4 < 1 && velocity.X / 4 > -1)
                                    {
                                        velocity = new Vector2(0, velocity.Y);
                                        this.boundingBox.position = position;
                                    }
                                    else
                                    {
                                        velocity = new Vector2((velocity.X / 2), velocity.Y);
                                        this.boundingBox.position = position + new Vector2(velocity.X, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            this.boundingBox.position = position + new Vector2(0, velocity.Y);
            //For each Block in the Chunk
            for (int x = 0; x < World.chunkSizeX; x++)
            {
                for (int y = 0; y < World.chunkSizeY; y++)
                {
                    //For all Chunks around the current one
                    for (int chunkX = -1; chunkX <= 1; chunkX++)
                    {
                        for (int chunkY = -1; chunkY <= 1; chunkY++)
                        {
                            //If Chunk is at the other side of the playfield
                            int chunkposX = (int)(chunkX + _currentChunkPosition.X);
                            int chunkposY = (int)(chunkY + _currentChunkPosition.Y);
                            if (chunkposX < 0) chunkposX = (int)world.worldsize.X - 1;
                            if (chunkposY < 0) chunkposY = (int)world.worldsize.Y - 1;
                            if (chunkposX > (int)world.worldsize.X - 1) chunkposX = 0;
                            if (chunkposY > (int)world.worldsize.Y - 1) chunkposY = 0;

                            Chunk chunkToTest = world.allChunks[chunkposX, chunkposY];

                            //Check collison for each Block in the chunkToTest
                            if (chunkToTest.Blocks[x, y] != null)
                            {
                                if (boundingBox.collides(chunkToTest.Blocks[x, y]))
                                {
                                    if (velocity.Y / 4 < 1 && velocity.Y / 4 > -1)
                                    {
                                        velocity = new Vector2(velocity.X, 0);
                                        this.boundingBox.position = position;
                                    }
                                    else
                                    {
                                        velocity = new Vector2(velocity.X, (velocity.Y / 2));
                                        this.boundingBox.position = position + new Vector2(0, velocity.Y);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Setting new Position for the Object
            position = position + velocity;
            position = new Vector2(position.X % (world.worldsize.X * World.chunkSizeX * World.blockSize), position.Y % (world.worldsize.Y * World.chunkSizeY * World.blockSize));
            if (position.X < 0) position = new Vector2((world.worldsize.X * World.chunkSizeX * World.blockSize) - position.X, position.Y);
            if (position.X < 0) position = new Vector2(position.X, (world.worldsize.Y * World.chunkSizeY * World.blockSize) - position.Y);
            //Calculate current Chunk
            _currentChunkPosition = new Vector2((int)position.X / (World.chunkSizeX * World.blockSize), (int)position.Y / (World.chunkSizeX * World.blockSize));
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 CameraPosition)
        {
            spriteBatch.Draw(texture, new Rectangle((int)(CameraPosition.X + position.X), (int)(CameraPosition.Y + position.Y), (int)(size.X), (int)(size.Y)), Color.White);
        }
    }

    class GameFigure : GameObject
    {
        public GameFigure(Vector2 position, Vector2 size, World world) : base(position, size, world) { }

        public override void Update(GameTime gameTime)
        {
            
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 CameraPosition)
        {

        }
    }

    class GamePlayer : GameFigure
    {
        float speed;
        float jumpEnergy;
        static Texture2D[] playerTextures = new Texture2D[4];

        public GamePlayer(Vector2 position, Vector2 size, World world) : base(position, size, world)
        {
            speed = 10;
            jumpEnergy = 20;
            _currentChunkPosition = new Vector2((int)position.X / (World.chunkSizeX * World.blockSize),(int)position.Y / (World.chunkSizeX * World.blockSize));

            _boundingBox = new BoundingBox2D(this, size);
        }

        public override void Update(GameTime gameTime)
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);

            Vector2 movement = new Vector2(state.ThumbSticks.Left.X * speed * (float)gameTime.ElapsedGameTime.TotalSeconds, -(state.ThumbSticks.Left.Y * jumpEnergy * (float)gameTime.ElapsedGameTime.TotalSeconds)) * 20;

            velocity += (new Vector2(0, World.gravity)) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            this.boundingBox.position = position + new Vector2(velocity.X, 0) + new Vector2(movement.X, 0);
            //For each Block in the Chunk
            for (int x = 0; x < World.chunkSizeX; x++)
            {
                for (int y = 0; y < World.chunkSizeY; y++)
                {
                    //For all Chunks around the current one
                    for (int chunkX = -1; chunkX <= 1; chunkX++)
                    {
                        for (int chunkY = -1; chunkY <= 1; chunkY++)
                        {
                            //If Chunk is at the other side of the playfield
                            int chunkposX = (int)(chunkX + _currentChunkPosition.X);
                            int chunkposY = (int)(chunkY + _currentChunkPosition.Y);
                            if (chunkposX < 0) chunkposX = (int)world.worldsize.X - 1;
                            if (chunkposY < 0) chunkposY = (int)world.worldsize.Y - 1;
                            if (chunkposX > (int)world.worldsize.X - 1) chunkposX = 0;
                            if (chunkposY > (int)world.worldsize.Y - 1) chunkposY = 0;

                            Chunk chunkToTest = world.allChunks[chunkposX, chunkposY];

                            //Check collison for each Block in the chunkToTest
                            if (chunkToTest.Blocks[x, y] != null)
                            {
                                if (boundingBox.collides(chunkToTest.Blocks[x, y]))
                                {
                                    if (velocity.X / 4 < 1 && velocity.X / 4 > -1)
                                    {
                                        velocity = new Vector2(0, velocity.Y);
                                        movement = new Vector2(0, movement.Y);
                                        this.boundingBox.position = position;
                                    }
                                    else
                                    {
                                        velocity = new Vector2((velocity.X / 2), velocity.Y);
                                        movement = new Vector2((movement.X / 2), movement.Y);
                                        this.boundingBox.position = position + new Vector2(velocity.X, 0) + new Vector2(movement.X, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            this.boundingBox.position = position + new Vector2(0, velocity.Y) + new Vector2(0, movement.Y);
            //For each Block in the Chunk
            for (int x = 0; x < World.chunkSizeX; x++)
            {
                for (int y = 0; y < World.chunkSizeY; y++)
                {
                    //For all Chunks around the current one
                    for (int chunkX = -1; chunkX <= 1; chunkX++)
                    {
                        for (int chunkY = -1; chunkY <= 1; chunkY++)
                        {
                            //If Chunk is at the other side of the playfield
                            int chunkposX = (int)(chunkX + _currentChunkPosition.X);
                            int chunkposY = (int)(chunkY + _currentChunkPosition.Y);
                            if (chunkposX < 0) chunkposX = (int)world.worldsize.X - 1;
                            if (chunkposY < 0) chunkposY = (int)world.worldsize.Y - 1;
                            if (chunkposX > (int)world.worldsize.X - 1) chunkposX = 0;
                            if (chunkposY > (int)world.worldsize.Y - 1) chunkposY = 0;

                            Chunk chunkToTest = world.allChunks[chunkposX, chunkposY];

                            //Check collison for each Block in the chunkToTest
                            if (chunkToTest.Blocks[x, y] != null)
                            {
                                if (boundingBox.collides(chunkToTest.Blocks[x, y]))
                                {
                                    if (velocity.Y / 4 < 1 && velocity.Y / 4 > -1)
                                    {
                                        velocity = new Vector2(velocity.X, 0);
                                        movement = new Vector2(movement.X, 0);
                                        this.boundingBox.position = position;
                                    }
                                    else
                                    {
                                        velocity = new Vector2(velocity.X, (velocity.Y / 2));
                                        movement = new Vector2(movement.X, (movement.Y / 2));
                                        this.boundingBox.position = position + new Vector2(0, velocity.Y) + new Vector2(0, movement.Y);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Setting new Position for the Object
            position = position + velocity + movement;
            position = new Vector2(position.X % (world.worldsize.X * World.chunkSizeX * World.blockSize), position.Y % (world.worldsize.Y * World.chunkSizeY * World.blockSize));
            if (position.X < 0) position = new Vector2((world.worldsize.X * World.chunkSizeX * World.blockSize) - position.X, position.Y);
            if (position.X < 0) position = new Vector2(position.X, (world.worldsize.Y * World.chunkSizeY * World.blockSize) - position.Y);

            //Calculate current Chunk
            _currentChunkPosition = new Vector2((int)position.X / (World.chunkSizeX * World.blockSize), (int)position.Y / (World.chunkSizeX * World.blockSize));

            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 CameraPosition)
        {
            spriteBatch.Draw(playerTextures[0], new Rectangle((int)(CameraPosition.X + position.X), (int)(CameraPosition.Y + position.Y), (int)(size.X), (int)(size.Y / 4)), Color.White);
            spriteBatch.Draw(playerTextures[1], new Rectangle((int)(CameraPosition.X + position.X), (int)(CameraPosition.Y + position.Y + (size.Y / 4)), (int)(size.X), (int)(size.Y / 2)), Color.White);
            spriteBatch.Draw(playerTextures[3], new Rectangle((int)(CameraPosition.X + position.X + (size.X / 4)), (int)(CameraPosition.Y + position.Y + ((size.Y / 4) * 3)), (int)(size.X / 2), (int)(size.Y / 4)), Color.White);
       }
        public static void LoadContent(ContentManager Content)
        {
            playerTextures[0] = Content.Load<Texture2D>("Textures/male_head");
            playerTextures[1] = Content.Load<Texture2D>("Textures/male_body");
            playerTextures[2] = Content.Load<Texture2D>("Textures/male_arm");
            playerTextures[3] = Content.Load<Texture2D>("Textures/male_leg");
        }
    }

    class BoundingBox2D
    {
        public Vector2 position { get; set; }
        public Vector2 size { get; }

        public BoundingBox2D(GameObject gameObject, Vector2 size)
        {
            this.position = gameObject.position;
            this.size = size;
        }
        public BoundingBox2D(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
        }

        public Boolean collides(BoundingBox2D boundingBox)
        {
            if (new Rectangle((int)(position.X), (int)(position.Y), (int)size.X, (int)size.Y).Intersects(new Rectangle((int)boundingBox.position.X, (int)boundingBox.position.Y, (int)boundingBox.size.X, (int)boundingBox.size.Y)))
                return true;
            return false;
        }

        public Boolean collides(GameObject gameObject)
        {
            return collides(gameObject.boundingBox);
        }

        public Boolean collides(Block block)
        {
            return collides(block.boundingBox);
        }
    }
}
