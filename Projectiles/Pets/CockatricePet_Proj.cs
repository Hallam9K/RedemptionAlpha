using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Pets;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class CockatricePet_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Baby Cockatrice");
            Main.projFrames[Projectile.type] = 16;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, 9, 5).WithOffset(2, 0).WithSpriteDirection(-1);
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyDino);
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            tailChain = new CockatricePetPhys();
            AIType = ProjectileID.BabyDino;
        }
        private static IPhysChain tailChain;
        private bool pecking;
        public override bool PreAI()
        {
            Projectile.GetGlobalProjectile<ProjPhysChain>().projPhysChain[0] = tailChain;
            Projectile.GetGlobalProjectile<ProjPhysChain>().projPhysChainOffset[0] = new Vector2(20f * Projectile.spriteDirection, 13f);
            Projectile.GetGlobalProjectile<ProjPhysChain>().projPhysChainDir[0] = -Projectile.spriteDirection;

            Player player = Main.player[Projectile.owner];
            player.dino = false;
            if (pecking)
            {
                Projectile.velocity *= 0;
                Projectile.rotation = 0;

                if (frameY < 9)
                    frameY = 9;

                frameCounter++;
                if (frameCounter >= 5)
                {
                    frameCounter = 0;
                    frameY++;
                    if (frameY > 15)
                    {
                        frameY = 0;
                        pecking = false;
                    }
                }
                return true;
            }
            if (Projectile.velocity.Y == 0)
            {
                Projectile.rotation = 0;
                if (Projectile.velocity.X > -1 && Projectile.velocity.X < 1)
                {
                    frameY = 0;
                    if (Main.rand.NextBool(200) && !pecking)
                        pecking = true;
                }
                else
                {
                    if (frameY < 1)
                        frameY = 1;

                    frameCounter += (int)(Projectile.velocity.X * 0.75f);
                    if (frameCounter is >= 5 or <= -5)
                    {
                        frameCounter = 0;
                        frameY++;
                        if (frameY > 8)
                            frameY = 1;
                    }
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                frameY = 2;
            }
            return true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);
            float overlapVelocity = 0.2f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && other.type == ModContent.ProjectileType<BasanPet_Proj>() && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                        Projectile.velocity.X -= overlapVelocity;
                    else
                        Projectile.velocity.X += overlapVelocity;

                    if (Projectile.position.Y < other.position.Y)
                        Projectile.velocity.Y -= overlapVelocity;
                    else
                        Projectile.velocity.Y += overlapVelocity;
                }
            }
        }
        private int frameY;
        private int frameCounter;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 16;
            int y = height * frameY;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 2) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<CockatricePetBuff>()))
                Projectile.timeLeft = 2;
            else
                Projectile.Kill();
        }
    }
    internal class CockatricePetPhys : IPhysChain
    {
        public Texture2D GetTexture(Mod mod)
        {
            return ModContent.Request<Texture2D>("Redemption/Projectiles/Pets/CockatricePet_Tail").Value;
        }
        public Texture2D GetGlowmaskTexture(Mod mod) => null;

        public int NumberOfSegments => 3;
        public int MaxFrames => 1;
        public int FrameCounterMax => 0;
        public bool Glow => false;
        public bool HasGlowmask => false;
        public int Shader => 0;
        public int GlowmaskShader => 0;

        public Color GetColor(PlayerDrawSet drawInfo, Color baseColour)
        {
            return baseColour;
        }

        public Vector2 AnchorOffset => new(10, 0);

        public Vector2 OriginOffset(int index) //padding
        {
            return index switch
            {
                0 => new Vector2(0, 0),
                1 => new Vector2(-2, 0),
                _ => new Vector2(-8, 0),
            };
        }

        public int Length(int index)
        {
            return index switch
            {
                0 => 10,
                1 => 12,
                _ => 16,
            };
        }

        public Rectangle GetSourceRect(Texture2D texture, int index)
        {
            return texture.Frame(NumberOfSegments, 1, NumberOfSegments - 1 - index, 0);
        }
        public Vector2 Force(Player player, int index, int dir, float gravDir, float time, NPC npc = null, Projectile proj = null)
        {
            Vector2 force = new(
                -dir * 0.5f,
                Player.defaultGravity * (0.5f + NumberOfSegments * NumberOfSegments * 0.5f / (1 + index))
                );

            if (!Main.gameMenu)
            {
                float windPower = 0.6f * dir * -10;

                // Wave in the wind
                force.X += 16f * proj.spriteDirection;
                force.Y -= 8;
                force -= proj.velocity * 2;
                force.Y += (float)(Math.Sin(time * 1f * windPower - index * Math.Sign(force.X)) * 0.25f * windPower) * 6f * dir;
            }
            return force;
        }
    }
}