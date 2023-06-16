using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Buffs.Pets;
using Redemption.Globals;
using Redemption.Particles;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class BasanPet_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Baby Basan");
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
            AIType = ProjectileID.BabyDino;
        }
        private bool pecking;
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.dino = false;
            Lighting.AddLight(Projectile.Center, .5f * Projectile.Opacity, .3f * Projectile.Opacity, .1f * Projectile.Opacity);
            if (Main.rand.NextBool(8))
            {
                ParticleManager.NewParticle(Projectile.Center - new Vector2(5 + Main.rand.Next(12) * Projectile.spriteDirection, -14 + Main.rand.Next(6)), new Vector2(Main.rand.Next(-2, 3), -Main.rand.Next(0, 3)), new EmberParticle(), Color.White, .6f, 0, 2, Layer: Particle.Layer.BeforeNPCs);
            }
            if (Main.rand.NextBool(100))
            {
                ParticleManager.NewParticle(RedeHelper.RandAreaInEntity(Projectile), new Vector2(Main.rand.Next(-1, 2), -Main.rand.Next(1, 3)), new EmberParticle(), Color.White, Main.rand.NextFloat(.4f, 1f), Layer: Particle.Layer.BeforeNPCs);
                ParticleManager.NewParticle(RedeHelper.RandAreaInEntity(Projectile), new Vector2(Main.rand.Next(-1, 2), -Main.rand.Next(1, 3)), new EmberParticle(), Color.White, Main.rand.NextFloat(.4f, 1f));
            }
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

                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && other.type == ModContent.ProjectileType<CockatricePet_Proj>() && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
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
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            int height = texture.Height / 16;
            int y = height * frameY;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 2) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center + new Vector2(0, 2) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<BasanPetBuff>()))
                Projectile.timeLeft = 2;
            else
                Projectile.Kill();
        }
    }
}