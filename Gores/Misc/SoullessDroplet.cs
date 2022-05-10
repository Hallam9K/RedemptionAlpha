using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Gores.Misc
{
    public class SoullessDroplet : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.Frame = new SpriteFrame(1, 15, 0, 0);
            gore.behindTiles = true;
            gore.timeLeft = Gore.goreTime * 3;
        }

        public override bool Update(Gore gore)
        {
            if (gore.position.Y < Main.worldSurface * 16.0 + 8.0)
                gore.alpha = 0;
            else
                gore.alpha = 100;

            int frameDuration = 4;
            gore.frameCounter++;
            if (gore.Frame.CurrentRow <= 4)
            {
                int tileX = (int)(gore.position.X / 16f);
                int tileY = (int)(gore.position.Y / 16f) - 1;
                if (WorldGen.InWorld(tileX, tileY, 0) && !Main.tile[tileX, tileY].HasTile)
                    gore.active = false;

                if (gore.Frame.CurrentRow == 0 || gore.Frame.CurrentRow == 1 || gore.Frame.CurrentRow == 2)
                    frameDuration = 24 + Main.rand.Next(256);

                if (gore.Frame.CurrentRow == 3)
                    frameDuration = 24 + Main.rand.Next(96);

                if (gore.frameCounter >= frameDuration)
                {
                    gore.frameCounter = 0;
                    gore.Frame.CurrentRow++;
                    if (gore.Frame.CurrentRow == 5)
                    {
                        int droplet = Gore.NewGore(new EntitySource_Misc("Gore"), gore.position, gore.velocity, gore.type, 1f);
                        Main.gore[droplet].Frame.CurrentRow = 9;
                        Main.gore[droplet].velocity *= 0f;
                    }
                }
            }
            else if (gore.Frame.CurrentRow <= 6)
            {
                frameDuration = 8;
                if (gore.frameCounter >= frameDuration)
                {
                    gore.frameCounter = 0;
                    gore.Frame.CurrentRow++;
                    if (gore.Frame.CurrentRow == 7)
                        gore.active = false;
                }
            }
            else if (gore.Frame.CurrentRow <= 9)
            {
                frameDuration = 6;
                gore.velocity.Y += 0.2f;
                if (gore.velocity.Y < 0.5f)
                    gore.velocity.Y = 0.5f;

                if (gore.velocity.Y > 12f)
                    gore.velocity.Y = 12f;

                if (gore.frameCounter >= frameDuration)
                {
                    gore.frameCounter = 0;
                    gore.Frame.CurrentRow++;
                }
                if (gore.Frame.CurrentRow > 9)
                    gore.Frame.CurrentRow = 7;
            }
            else
            {
                gore.velocity.Y += 0.1f;
                if (gore.frameCounter >= frameDuration)
                {
                    gore.frameCounter = 0;
                    gore.Frame.CurrentRow++;
                }
                gore.velocity *= 0f;
                if (gore.Frame.CurrentRow > 14)
                    gore.active = false;
            }

            Vector2 oldVelocity = gore.velocity;
            gore.velocity = Collision.TileCollision(gore.position, gore.velocity, 16, 14, false, false, 1);
            if (gore.velocity != oldVelocity)
            {
                if (gore.Frame.CurrentRow < 10)
                {
                    gore.Frame.CurrentRow = 10;
                    gore.frameCounter = 0;
                    SoundEngine.PlaySound(SoundID.Drip, (int)gore.position.X + 8, (int)gore.position.Y + 8, Main.rand.Next(2));
                }
            }
            else if (Collision.WetCollision(gore.position + gore.velocity, 16, 14))
            {
                if (gore.Frame.CurrentRow < 10)
                {
                    gore.Frame.CurrentRow = 10;
                    gore.frameCounter = 0;
                    SoundEngine.PlaySound(SoundID.Drip, (int)gore.position.X + 8, (int)gore.position.Y + 8, 2);
                }
                int tileX = (int)(gore.position.X + 8f) / 16;
                int tileY = (int)(gore.position.Y + 14f) / 16;
                if (Main.tile[tileX, tileY] != null && Main.tile[tileX, tileY].LiquidAmount > 0)
                {
                    gore.velocity *= 0f;
                    gore.position.Y = tileY * 16 - Main.tile[tileX, tileY].LiquidAmount / 16;
                }
            }

            gore.position += gore.velocity;
            return false;
        }
    }
}