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
        public List<GameItem> ChunkChangingItems;

        public World(Vector2 worldsize)
        {
            this.worldsize = worldsize;
            allChunks = new Chunk[(int)worldsize.X, (int)worldsize.Y];
            ChunkChangingItems = new List<GameItem>();

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

            foreach (GameItem item in ChunkChangingItems)
            {
                allChunks[(int)item.currentChunkPosition.X, (int)item.currentChunkPosition.Y].items.Add(item);
                allChunks[(int)item.previousChunkPosition.X, (int)item.previousChunkPosition.Y].items.Remove(item);
            }
            ChunkChangingItems.Clear();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition, Vector2 resolution)
        {
            //For all Chunks
            for (int x = 0; x < worldsize.X; x++)
            {
                for (int y = 0; y < worldsize.Y; y++)
                {
                    if (allChunks[x, y].isActive)
                    {
                        allChunks[x, y].DrawBlocks(spriteBatch, cameraPosition + (new Vector2(World.chunkSizeX * World.blockSize * x, World.chunkSizeY * World.blockSize * y)), resolution);
                        allChunks[x, y].DrawItems(spriteBatch, cameraPosition, resolution);
                    }
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
            if (isActive)
            {
                //Update every Item in the Chunk
                foreach (GameItem item in items)
                {
                    item.Update(gameTime);
                }
                /*for (int x = 0; x < World.chunkSizeX; x++)
                {
                    for (int y = 0; y < World.chunkSizeY; y++)
                    {
                        if (Blocks[x, y] != null)
                        {
                            if (Blocks[x, y].type.Equals(Material.Type.liquid))
                            {
                                if(Blocks[x, y + 1] == null)
                                {

                                }
                            }
                        }
                    }
                }*/
            }
        }

        public void DrawBlocks(SpriteBatch SpriteBatch, Vector2 cameraPosition, Vector2 resolution)
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
                            if ((byte)Blocks[x, y].type == (byte)Material.Type.solid)
                            {
                                SpriteBatch.Draw(Blocks[x, y].material.texture, new Rectangle(posX, posY, World.blockSize, World.blockSize), new Rectangle((World.blockSize * x) % 128, (World.blockSize * y) % 128, World.blockSize, World.blockSize), Color.White);
                            }
                            else if (Blocks[x,y].type == Material.Type.liquid)
                            {
                                SpriteBatch.Draw(Blocks[x, y].material.texture, new Rectangle(posX, posY + (World.blockSize - (int)(Blocks[x, y].fulllevel / World.blockSize)), World.blockSize, (int)(Blocks[x, y].fulllevel / World.blockSize)), new Rectangle((World.blockSize * x) % 128, (World.blockSize * y) % 128, World.blockSize, World.blockSize), Color.White);
                            }
                        }
                    }
                }
            }
        }
        public void DrawItems(SpriteBatch SpriteBatch, Vector2 cameraPosition, Vector2 resolution)
        {
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
        public Material.Type type { get; }
        public int fulllevel { get; set; }

        public Block(Material material)
        {
            this.material = material;
            health = material.durability;
            type = material.type;
            fulllevel = World.blockSize * World.blockSize;
        }

        public void Update(GameTime gameTime) { }
    }

    class Material
    {
        public Texture2D texture { get; }
        public float durability { get; }
        public Type type;

        public Material(Texture2D texture, Type type, float durabilty)
        {
            this.texture = texture;
            this.durability = durabilty;
            this.type = type;
        }
        public Material(Texture2D texture, Type type)
        {
            this.texture = texture;
            this.durability = 1f;
            this.type = type;
        }

        public enum Type : byte
        {
            liquid,
            solid,
            passable
        }
    }
}
