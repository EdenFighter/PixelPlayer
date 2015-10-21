using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelPlayer
{
    class Player
    {
        public Vector2 position { get; set; }
        public Vector2 size { get; }
        public Vector2 velocity { get; set; }
        public Vector2 movement { get; set; }
        public SimpleBoundingBox2D boundingBox { get; }
        public float speed { get; }
        public float jumpEnergy { get; }

        BodyPart Head;
        BodyPart LegLeft;
        BodyPart LegRight;
        BodyPart Body;
        BodyPart ArmLeft;
        BodyPart ArmRight;

        public Player(Vector2 position, Vector2 size, Texture2D Head, Texture2D Body, Texture2D Arm, Texture2D Leg)
        {
            this.position = position;
            this.size = size;

            this.Head = new BodyPart(Head,new Vector2(0, 0));
            this.LegLeft = new BodyPart(Leg, new Vector2(0, 0));
            this.LegRight = new BodyPart(Leg, new Vector2(0, 0));
            this.Body = new BodyPart(Body, new Vector2(0, 0));
            this.ArmLeft = new BodyPart(Arm, new Vector2(0, 0));
            this.ArmRight = new BodyPart(Arm, new Vector2(0, 0));

            velocity = new Vector2(0, 0);
            speed = 40f;
            jumpEnergy = 100f;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(Head.texture,new Rectangle((int)(Head.position.X + position.X + this.position.X), (int)(Head.position.Y + position.Y + this.position.Y),(int)(size.X),(int)(size.Y / 4)),Color.White);
            spriteBatch.Draw(Body.texture, new Rectangle((int)(Body.position.X + position.X + this.position.X), (int)(Body.position.Y + position.Y + this.position.Y + (size.Y / 4)), (int)(size.X), (int)(size.Y / 2)), Color.White);
            spriteBatch.Draw(Body.texture, new Rectangle((int)(Body.position.X + position.X + this.position.X + (size.X / 4)), (int)(Body.position.Y + position.Y + this.position.Y + ((size.Y / 4) * 3)), (int)(size.X / 2), (int)(size.Y / 4)), Color.White);
        }
    }

    class BodyPart
    {
        public Vector2 position { get; set; }
        public Texture2D texture { get; }

        public BodyPart(Texture2D texture , Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }
    }

    class SimpleBoundingBox2D
    {
        Rectangle leftBoundingbox;
        Rectangle rightBoundingbox;
        Rectangle topBoundingbox;
        Rectangle bottomBoundingbox;
        Vector2 size;

        public SimpleBoundingBox2D(Vector2 size)
        {
            this.size = size;
        }
    }
}
