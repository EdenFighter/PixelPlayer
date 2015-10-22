using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelPlayer
{
    class Level
    {
        public int mapwidth;

        public int mapheight;

        public Block[,] Blocks { get; set; }

        public Level()
        {
            mapwidth = 64;
            mapheight = 36;
            Blocks = new Block[mapwidth, mapheight];
        }
    }

    class Material
    {
        Texture2D _texture;
        float _durability;

        public Material(Texture2D texture,float durabilty)
        {
            _texture = texture;
            _durability = durabilty;
        }
        public Material(Texture2D texture)
        {
            _texture = texture;
            _durability = 1f;
        }

        public Texture2D texture 
        {
            get { return _texture; }
        }
        public float durability
        {
            get { return _durability; }
        }
    }

    class Block
    {
        Material _material;
        float _health;

        public Block(Material material)
        {
            _material = material;
            _health = material.durability;
        }

        public Material material
        {
            get { return _material; }
        }
    }
}
