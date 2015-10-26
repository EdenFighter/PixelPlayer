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
        public static int chunkSizeX = 64;
        public static int chunkSizeY = 64;
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
    }

    class Chunk
    {
        public Boolean isActive { get; set; }
        public Block[,] Blocks { get; set; }

        public Chunk()
        {
            Blocks = new Block[World.chunkSizeX, World.chunkSizeY];
        }

        public void Draw(SpriteBatch SpriteBatch, Vector2 cameraPosition)
        {
            for (int x = 0; x < World.chunkSizeX; x++)
            {
                for (int y = 0; y < World.chunkSizeY; y++)
                {
                    if (Blocks[x, y] != null)
                    {
                        SpriteBatch.Draw(Blocks[x, y].material.texture, new Rectangle((x * World.blockSize) + (int)cameraPosition.X, (y * World.blockSize) + (int)cameraPosition.Y, World.blockSize, World.blockSize), new Rectangle((World.blockSize * x) % 128, (World.blockSize * y) % 128, World.blockSize, World.blockSize), Color.White);
                    }
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
