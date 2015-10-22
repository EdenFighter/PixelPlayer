using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelPlayer
{
    class Item
    {
        public Texture2D texture { get; }
        public Vector2 position { get; set; }
        public Vector2 size { get; }
        public Vector2 velocity { get; set; }
        float _angle;

        public Item(Vector2 position, Vector2 size, Texture2D texture)
        {
            this.position = position;
            this.texture = texture;
            this.size = size;
            velocity = new Vector2(0, 0);
        }

        public float angle
        {
            get
            {
                return _angle;
            }
            set
            {
                if (value <= 0)
                {
                    _angle = 0;
                }
                else
                {
                    _angle = value % 360;
                }
            }
        }
    }
}
