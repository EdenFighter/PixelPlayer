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
            _currentChunkPosition = new Vector2((int)position.X / (World.chunkSizeX * World.blockSize), (int)position.Y / (World.chunkSizeY * World.blockSize));
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

        Vector2 _previousChunkPosition;

        public GameItem(Vector2 position, Vector2 size , Texture2D texture, World world) : base(position, size, world)
        {
            this.texture = texture;
            _boundingBox = new BoundingBox2D(this, size);
            world.allChunks[(int)_currentChunkPosition.X, (int)_currentChunkPosition.Y].items.Add(this);
        }

        public Vector2 previousChunkPosition
        {
            get { return _previousChunkPosition; }
        }

        public override void Update(GameTime gameTime)
        {
            velocity += new Vector2(0, World.gravity) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (velocity.X < 0.2 && velocity.X > -0.2) { velocity = new Vector2(0, velocity.Y); }
            else { velocity -= new Vector2(((velocity.X / 100)  * 95), 0) * (float)gameTime.ElapsedGameTime.TotalSeconds; }

            //World Collision calculation
            bool collisionFound = false;
            bool collisionInGreatFound = false;
            bool velocityEffectedByLiquid = false;
            Vector2 newVelocity = new Vector2(velocity.X, velocity.Y);
            Vector2 endPosition;
            do
            {
                endPosition = position + newVelocity;
                int greatPosX;
                int greatPosY;
                if (endPosition.X < position.X) { greatPosX = (int)endPosition.X; }
                else { greatPosX = (int)position.X; }
                if (endPosition.Y < position.Y) { greatPosY = (int)endPosition.Y; }
                else { greatPosY = (int)position.Y; }

                int greatHeight = (int)(Math.Abs(newVelocity.Y) + size.Y);
                int greatWidth = (int)(Math.Abs(newVelocity.X) + size.X);

                for (int x = (int)(endPosition.X / World.blockSize); x < (int)Math.Ceiling((endPosition.X + size.X) / World.blockSize); x++)
                {
                    for (int y = (int)(endPosition.Y / World.blockSize); y < (int)Math.Ceiling((endPosition.Y + size.Y) / World.blockSize); y++)
                    {
                        int PosX;
                        int PosY;
                        if (x < 0) { PosX = (int)world.worldsize.X * World.chunkSizeX - x; } else { PosX = x; }
                        if (y < 0) { PosY = (int)world.worldsize.Y * World.chunkSizeY - y; } else { PosY = y; }
                        int ChunkX = (x / World.chunkSizeX) % (int)world.worldsize.X;
                        int ChunkY = (y / World.chunkSizeY) % (int)world.worldsize.Y;

                        if (world.allChunks[ChunkX, ChunkY].Blocks[PosX % World.chunkSizeX, PosY % World.chunkSizeY] != null)
                        {
                            if ((byte)world.allChunks[ChunkX, ChunkY].Blocks[PosX % World.chunkSizeX, PosY % World.chunkSizeY].type == (byte)Material.Type.solid)
                            {
                                collisionFound = true;
                                if (velocity.X >= 0)
                                {
                                    if ((x * World.blockSize) < endPosition.X + size.X)
                                    {
                                        if ((velocity.Y > 0) && ((y * World.blockSize) - size.Y >= position.Y) && ((y * World.blockSize) - size.Y < endPosition.Y))
                                        {
                                            endPosition.Y = (y * World.blockSize) - size.Y;
                                            velocity = new Vector2(velocity.X, 0);
                                        }
                                        else if ((velocity.Y < 0) && ((y * World.blockSize) + World.blockSize <= position.Y) && ((y * World.blockSize) + World.blockSize > endPosition.Y))
                                        {
                                            endPosition.Y = (y * World.blockSize) + World.blockSize;
                                            velocity = new Vector2(velocity.X, 0);
                                        }
                                    }
                                }
                                else
                                {
                                    if ((x * World.blockSize) + World.blockSize > endPosition.X)
                                    {
                                        if ((velocity.Y > 0) && ((y * World.blockSize) - size.Y >= position.Y) && ((y * World.blockSize) - size.Y < endPosition.Y))
                                        {
                                            endPosition.Y = (y * World.blockSize) - size.Y;
                                            velocity = new Vector2(velocity.X, 0);
                                        }
                                        else if ((velocity.Y < 0) && ((y * World.blockSize) + World.blockSize <= position.Y) && ((y * World.blockSize) + World.blockSize > endPosition.Y))
                                        {
                                            endPosition.Y = (y * World.blockSize) + World.blockSize;
                                            velocity = new Vector2(velocity.X, 0);
                                        }
                                    }
                                }
                                if (velocity.Y >= 0)
                                {
                                    if ((y * World.blockSize) < endPosition.Y + size.Y)
                                    {
                                        if ((velocity.X > 0) && ((x * World.blockSize) - size.X >= position.X) && ((x * World.blockSize) - size.X < endPosition.X))
                                        {
                                            endPosition.X = (x * World.blockSize) - size.X;
                                            velocity = new Vector2(0, velocity.Y);
                                        }
                                        else if ((velocity.X < 0) && ((x * World.blockSize) + World.blockSize <= position.X) && ((x * World.blockSize) + World.blockSize > endPosition.X))
                                        {
                                            endPosition.X = (x * World.blockSize) + World.blockSize;
                                            velocity = new Vector2(0, velocity.Y);
                                        }
                                    }
                                }
                                else
                                {
                                    if ((y * World.blockSize) + World.blockSize > endPosition.Y)
                                    {
                                        if ((velocity.X > 0) && ((x * World.blockSize) - size.X >= position.X) && ((x * World.blockSize) - size.X < endPosition.X))
                                        {
                                            endPosition.X = (x * World.blockSize) - size.X;
                                            velocity = new Vector2(0, velocity.Y);
                                        }
                                        else if ((velocity.X < 0) && ((x * World.blockSize) + World.blockSize <= position.X) && ((x * World.blockSize) + World.blockSize > endPosition.X))
                                        {
                                            endPosition.X = (x * World.blockSize) + World.blockSize;
                                            velocity = new Vector2(0, velocity.Y);
                                        }
                                    }
                                }
                            }
                            else if(world.allChunks[ChunkX, ChunkY].Blocks[PosX % World.chunkSizeX, PosY % World.chunkSizeY].type == Material.Type.liquid)
                            {
                                if (!velocityEffectedByLiquid)
                                {
                                    if (velocity.Y > 1)
                                    {
                                        velocity = (velocity / 10) * 9;
                                    }
                                    else
                                    {
                                        velocity = new Vector2(0, 1);
                                    }
                                    velocityEffectedByLiquid = true;
                                    endPosition = position + velocity;
                                }
                            }
                        }
                    }
                }
                if (!collisionFound)
                {
                    collisionInGreatFound = false;
                    for (int x = (int)(greatPosX / World.blockSize); x < (int)Math.Ceiling((Double)(greatPosX + greatWidth) / World.blockSize); x++)
                    {
                        for (int y = (int)(greatPosY / World.blockSize); y < (int)Math.Ceiling((Double)(greatPosY + greatHeight) / World.blockSize); y++)
                        {
                            int PosX;
                            int PosY;
                            if (x < 0) { PosX = (int)world.worldsize.X * World.chunkSizeX - x; } else { PosX = x; }
                            if (y < 0) { PosY = (int)world.worldsize.Y * World.chunkSizeY - y; } else { PosY = y; }
                            int ChunkX = (PosX / World.chunkSizeX) % (int)world.worldsize.X;
                            int ChunkY = (PosY / World.chunkSizeY) % (int)world.worldsize.Y;

                            if (world.allChunks[ChunkX, ChunkY].Blocks[PosX % World.chunkSizeX, PosY % World.chunkSizeY] != null)
                            {
                                if ((byte)world.allChunks[ChunkX, ChunkY].Blocks[PosX % World.chunkSizeX, PosY % World.chunkSizeY].type == (byte)Material.Type.solid)
                                {
                                    collisionInGreatFound = true;
                                    newVelocity = newVelocity / 2;
                                }
                            }
                        }
                    }
                }
            } while (!collisionFound && collisionInGreatFound && (Math.Abs(newVelocity.X) > velocity.X / 8 || Math.Abs(newVelocity.Y) > velocity.Y / 8));

            position = endPosition;
            if (position.X < 0) position = new Vector2((world.worldsize.X * World.chunkSizeX * World.blockSize) - position.X, position.Y);
            else if (position.X > (world.worldsize.X * World.chunkSizeX * World.blockSize)) position = new Vector2(position.X - (world.worldsize.X * World.chunkSizeX * World.blockSize), position.Y);
            if (position.Y < 0) position = new Vector2(position.X, (world.worldsize.Y * World.chunkSizeY * World.blockSize) - position.Y);
            else if (position.Y > (world.worldsize.Y * World.chunkSizeY * World.blockSize)) position = new Vector2(position.X, position.Y - (world.worldsize.Y * World.chunkSizeY * World.blockSize));

            _previousChunkPosition = new Vector2(_currentChunkPosition.X, _currentChunkPosition.Y);
            _currentChunkPosition = new Vector2((int)position.X / (World.chunkSizeX * World.blockSize), (int)position.Y / (World.chunkSizeY * World.blockSize));
            if (_previousChunkPosition.X != _currentChunkPosition.X || _previousChunkPosition.Y != _currentChunkPosition.Y)
            {
                world.ChunkChangingItems.Add(this);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 CameraPosition)
        {
            spriteBatch.Draw(texture, new Rectangle((int)(CameraPosition.X + position.X), (int)(CameraPosition.Y + position.Y), (int)(size.X), (int)(size.Y)), Color.White);
        }
    }

    class GameFigure : GameObject
    {
        protected Vector2 movement;
        protected Vector2 physicVelocity;
        private bool _touchesLiquid = false;
        private bool _touchesGround = false;


        public GameFigure(Vector2 position, Vector2 size, World world) : base(position, size, world)
        {
            movement = new Vector2(0, 0);
            physicVelocity = new Vector2(0, 0);
        }
        public bool touchesGround
        {
            get { return _touchesGround; }
        }
        public bool touchesLiquid
        {
            get { return _touchesLiquid; }
        }

        public override void Update(GameTime gameTime)
        {
            physicVelocity += new Vector2(0, World.gravity) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity = physicVelocity + movement;

            //World Collision calculation
            bool collisionFound = false;
            bool collisionInGreatFound = false;
            bool velocityEffectedByLiquid = false;
            Vector2 newVelocity = new Vector2(velocity.X, velocity.Y);
            Vector2 endPosition;
            
            do
            {
                endPosition = position + newVelocity;
                int greatPosX;
                int greatPosY;
                if (endPosition.X < position.X)    { greatPosX = (int)endPosition.X; }
                else                                    { greatPosX = (int)position.X; }
                if (endPosition.Y < position.Y)    { greatPosY = (int)endPosition.Y; }
                else                                    { greatPosY = (int)position.Y; }

                int greatHeight = (int)(Math.Abs(newVelocity.Y) + size.Y);
                int greatWidth = (int)(Math.Abs(newVelocity.X) + size.X);

                for (int x = (int)(endPosition.X / World.blockSize); x < (int)Math.Ceiling((endPosition.X + size.X) / World.blockSize); x++)
                {
                    for (int y = (int)(endPosition.Y / World.blockSize); y < (int)Math.Ceiling((endPosition.Y + size.Y) / World.blockSize); y++)
                    {
                        int PosX;
                        int PosY;
                        if (x < 0) { PosX = (int)world.worldsize.X * World.chunkSizeX - x; } else { PosX = x; }
                        if (y < 0) { PosY = (int)world.worldsize.Y * World.chunkSizeY - y; } else { PosY = y; }
                        int ChunkX = (x / World.chunkSizeX) % (int)world.worldsize.X;
                        int ChunkY = (y / World.chunkSizeY) % (int)world.worldsize.Y;

                        if (world.allChunks[ChunkX, ChunkY].Blocks[PosX % World.chunkSizeX, PosY % World.chunkSizeY] != null)
                        {
                            if ((byte)world.allChunks[ChunkX, ChunkY].Blocks[PosX % World.chunkSizeX, PosY % World.chunkSizeY].type == (byte)Material.Type.solid)
                            {
                                collisionFound = true;
                                if (velocity.X >= 0)
                                {
                                    if ((x * World.blockSize) < endPosition.X + size.X)
                                    {
                                        if ((velocity.Y > 0) && ((y * World.blockSize) - size.Y >= position.Y) && ((y * World.blockSize) - size.Y < endPosition.Y))
                                        {
                                            endPosition.Y = (y * World.blockSize) - size.Y;
                                            velocity = new Vector2(velocity.X, 0);
                                            physicVelocity = new Vector2(0, 0);

                                            _touchesGround = true;
                                        }
                                        else if ((velocity.Y < 0) && ((y * World.blockSize) + World.blockSize <= position.Y) && ((y * World.blockSize) + World.blockSize > endPosition.Y))
                                        {
                                            endPosition.Y = (y * World.blockSize) + World.blockSize;
                                            velocity = new Vector2(velocity.X, 0);
                                            physicVelocity = new Vector2(0, 0);
                                        }
                                    }
                                }
                                else
                                {
                                    if ((x * World.blockSize) + World.blockSize > endPosition.X)
                                    {
                                        if ((velocity.Y > 0) && ((y * World.blockSize) - size.Y >= position.Y) && ((y * World.blockSize) - size.Y < endPosition.Y))
                                        {
                                            endPosition.Y = (y * World.blockSize) - size.Y;
                                            velocity = new Vector2(velocity.X, 0);
                                            physicVelocity = new Vector2(0, 0);

                                            _touchesGround = true;
                                        }
                                        else if ((velocity.Y < 0) && ((y * World.blockSize) + World.blockSize <= position.Y) && ((y * World.blockSize) + World.blockSize > endPosition.Y))
                                        {
                                            endPosition.Y = (y * World.blockSize) + World.blockSize;
                                            velocity = new Vector2(velocity.X, 0);
                                            physicVelocity = new Vector2(0, 0);
                                        }
                                    }
                                }
                                if (velocity.Y >= 0)
                                {
                                    if ((y * World.blockSize) < endPosition.Y + size.Y)
                                    {
                                        if ((velocity.X > 0) && ((x * World.blockSize) - size.X >= position.X) && ((x * World.blockSize) - size.X < endPosition.X))
                                        {
                                            endPosition.X = (x * World.blockSize) - size.X;
                                            velocity = new Vector2(0, velocity.Y);
                                        }
                                        else if ((velocity.X < 0) && ((x * World.blockSize) + World.blockSize <= position.X) && ((x * World.blockSize) + World.blockSize > endPosition.X))
                                        {
                                            endPosition.X = (x * World.blockSize) + World.blockSize;
                                            velocity = new Vector2(0, velocity.Y);
                                        }
                                    }
                                }
                                else
                                {
                                    if ((y * World.blockSize) + World.blockSize > endPosition.Y)
                                    {
                                        if ((velocity.X > 0) && ((x * World.blockSize) - size.X >= position.X) && ((x * World.blockSize) - size.X < endPosition.X))
                                        {
                                            endPosition.X = (x * World.blockSize) - size.X;
                                            velocity = new Vector2(0, velocity.Y);
                                        }
                                        else if ((velocity.X < 0) && ((x * World.blockSize) + World.blockSize <= position.X) && ((x * World.blockSize) + World.blockSize > endPosition.X))
                                        {
                                            endPosition.X = (x * World.blockSize) + World.blockSize;
                                            velocity = new Vector2(0, velocity.Y);
                                        }
                                    }
                                }
                            }
                            else if (world.allChunks[ChunkX, ChunkY].Blocks[PosX % World.chunkSizeX, PosY % World.chunkSizeY].type == Material.Type.liquid)
                            {
                                _touchesLiquid = true;
                                if (!velocityEffectedByLiquid)
                                {
                                    if (physicVelocity.Y > 1f || physicVelocity.Y < 0)
                                    {
                                        physicVelocity = (physicVelocity / 10) * 9;
                                        velocity = physicVelocity + movement;
                                        
                                    }
                                    else
                                    {
                                        velocity = new Vector2(0, 1f) + movement;
                                    }
                                    velocityEffectedByLiquid = true;
                                    endPosition = position + velocity;
                                }
                            }
                        }
                    }
                }
                if (!collisionFound)
                {
                    collisionInGreatFound = false;
                    for (int x = (int)(greatPosX / World.blockSize); x < (int)Math.Ceiling((Double)(greatPosX + greatWidth) / World.blockSize); x++)
                    {
                        for (int y = (int)(greatPosY / World.blockSize); y < (int)Math.Ceiling((Double)(greatPosY + greatHeight) / World.blockSize); y++)
                        {
                            int PosX;
                            int PosY;
                            if (x < 0) { PosX = (int)world.worldsize.X * World.chunkSizeX - x; } else { PosX = x; }
                            if (y < 0) { PosY = (int)world.worldsize.Y * World.chunkSizeY - y; } else { PosY = y; }
                            int ChunkX = (PosX / World.chunkSizeX) % (int)world.worldsize.X;
                            int ChunkY = (PosY / World.chunkSizeY) % (int)world.worldsize.Y;

                            if (world.allChunks[ChunkX, ChunkY].Blocks[PosX % World.chunkSizeX, PosY % World.chunkSizeY] != null)
                            {
                                if ((byte)world.allChunks[ChunkX, ChunkY].Blocks[PosX % World.chunkSizeX, PosY % World.chunkSizeY].type == (byte)Material.Type.solid)
                                {
                                    collisionInGreatFound = true;
                                    newVelocity = newVelocity / 2;
                                }
                            }
                        }
                    }
                }
            } while (!collisionFound && collisionInGreatFound && (Math.Abs(newVelocity.X) > velocity.X / 8 || Math.Abs(newVelocity.Y) > velocity.Y / 8));

            if (velocity.Y != 0) _touchesGround = false;

            position = endPosition;
            if (position.X < 0) position = new Vector2((world.worldsize.X * World.chunkSizeX * World.blockSize) - position.X, position.Y);
            else if(position.X > (world.worldsize.X * World.chunkSizeX * World.blockSize)) position = new Vector2(position.X - (world.worldsize.X * World.chunkSizeX * World.blockSize), position.Y);
            if (position.Y < 0) position = new Vector2(position.X, (world.worldsize.Y * World.chunkSizeY * World.blockSize) - position.Y);
            else if (position.Y > (world.worldsize.Y * World.chunkSizeY * World.blockSize)) position = new Vector2(position.X, position.Y - (world.worldsize.Y * World.chunkSizeY * World.blockSize));
            _currentChunkPosition = new Vector2((int)position.X / (World.chunkSizeX * World.blockSize), (int)position.Y / (World.chunkSizeX * World.blockSize));
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
            speed = 7 * 20;
            jumpEnergy = 150 * 20;
            _currentChunkPosition = new Vector2((int)position.X / (World.chunkSizeX * World.blockSize),(int)position.Y / (World.chunkSizeX * World.blockSize));

            _boundingBox = new BoundingBox2D(this, size);
        }

        public override void Update(GameTime gameTime)
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            if (state.IsConnected)
            {
                movement = new Vector2(state.ThumbSticks.Left.X * speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                if (touchesGround && state.ThumbSticks.Left.Y < -0.1f) { physicVelocity += new Vector2(0, -1.0f * jumpEnergy * (float)gameTime.ElapsedGameTime.TotalSeconds); }
            }
            else
            {
                movement = new Vector2(0, 0);
                if (Keyboard.GetState().IsKeyDown(Keys.A)) { movement += new Vector2(-1.0f * speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0); }
                if (Keyboard.GetState().IsKeyDown(Keys.D)) { movement += new Vector2(1.0f * speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0); }
                if (touchesGround && Keyboard.GetState().IsKeyDown(Keys.W)) { physicVelocity += new Vector2(0, -1.0f * jumpEnergy * (float)gameTime.ElapsedGameTime.TotalSeconds); }
            }

            base.Update(gameTime);

            //Calculate current Chunk
            _currentChunkPosition = new Vector2((int)position.X / (World.chunkSizeX * World.blockSize), (int)position.Y / (World.chunkSizeX * World.blockSize));
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
    }
}
