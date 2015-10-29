using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelPlayer
{
    class World
    {
        public static int blockSize = 8;
        public static float gravity = 10f;
        public static int chunkSize = 64;
        public static int chunkSizeX = chunkSize;
        public static int chunkSizeY = chunkSize;
        public Chunk[,] allChunks;
        public Vector2 worldsize { get; }

        public World(Vector2 worldsize)
        {
            this.worldsize = worldsize;
            allChunks = new Chunk[(int)worldsize.X, (int)worldsize.Y];

            for (int x = 0; x < worldsize.X; x++)
            {
                for (int y = 0; y < worldsize.Y; y++)
                {
                    allChunks[x, y] = new Chunk();
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int x = 0; x < worldsize.X; x++)
            {
                for (int y = 0; y < worldsize.Y; y++)
                {
                    if (allChunks[x, y].isActive) allChunks[x, y].Update(gameTime);
                }
            }
        }

        public void Draw (SpriteBatch spriteBatch, Vector2 cameraPosition, Vector2 resolution)
        {
            //For all Chunks
            for (int x = 0; x < worldsize.X; x++)
            {
                for (int y = 0; y < worldsize.Y; y++)
                {
                    allChunks[x, y].Draw(spriteBatch, cameraPosition + (new Vector2(World.chunkSizeX * World.blockSize * x, World.chunkSizeY * World.blockSize * y)), resolution);
                }
            }
        }
    }

    class Chunk
    {
        public Boolean isActive { get; set; }
        public Block[,] Blocks { get; set; }
        public List<GameItem> items;

        public Chunk()
        {
            Blocks = new Block[World.chunkSizeX, World.chunkSizeY];
            items = new List<GameItem>();
            isActive = true;
        }

        public void Update(GameTime gameTime)
        {
            foreach (GameItem item in items)
            {
                item.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch SpriteBatch, Vector2 cameraPosition, Vector2 resolution)
        {
            for (int x = 0; x < World.chunkSizeX; x++)
            {
                for (int y = 0; y < World.chunkSizeY; y++)
                {
                    if (Blocks[x, y] != null)
                    {
                        int posX = (x * World.blockSize) + (int)cameraPosition.X;
                        int posY = (y * World.blockSize) + (int)cameraPosition.Y;
                        //If Block is insight of the screen
                        if ((posX + World.blockSize > 0) && posX < (int)resolution.X && (posY + World.blockSize) > 0 && posY < (int)resolution.Y)
                        {
                            SpriteBatch.Draw(Blocks[x, y].material.texture, new Rectangle(posX, posY, World.blockSize, World.blockSize), new Rectangle((World.blockSize * x) % 128, (World.blockSize * y) % 128, World.blockSize, World.blockSize), Color.White);
                        }
                    }
                }
            }
            foreach (GameItem item in items)
            {
                //If Item is insight of the screen
                int posX = (int)(item.position.X + cameraPosition.X);
                int posY = (int)(item.position.Y + cameraPosition.Y);
                if ((posX + item.size.X) > 0 && posX < (int)resolution.X && (posY + item.size.Y) > 0 && posY < (int)resolution.Y)
                {
                    item.Draw(SpriteBatch, cameraPosition);
                }
            }
        }
    }

    class Block
    {
        public Material material { get; }
        public float health { get; set; }
        public Vector2 chunkGridPosition { get; }
        public BoundingBox2D boundingBox { get; }

        public Block(Material material,Vector2 chunkGridPosition, Vector2 chunkPosition)
        {
            this.material = material;
            this.chunkGridPosition = chunkGridPosition;
            health = material.durability;

            boundingBox = new BoundingBox2D((chunkGridPosition * World.blockSize) + new Vector2(chunkPosition.X * World.chunkSizeX * World.blockSize, chunkPosition.Y * World.chunkSizeY * World.blockSize), new Vector2(World.blockSize,World.blockSize));
        }
    }

    class Material
    {
        public Texture2D texture { get; }
        public float durability { get; }

        public Material(Texture2D texture, float durabilty)
        {
            this.texture = texture;
            this.durability = durabilty;
        }
        public Material(Texture2D texture)
        {
            this.texture = texture;
            this.durability = 1f;
        }
    }
}
