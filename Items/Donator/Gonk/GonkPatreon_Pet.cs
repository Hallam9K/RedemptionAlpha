using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Biomes;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Gonk
{
    public class GonkPatreon_Pet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Samus");
            Main.projFrames[Projectile.type] = 12;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 32;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);
            Projectile.LookByVelocity();

            if (Projectile.ai[0] != 0 && Projectile.ai[0] == 1)
            {
                if (Projectile.ai[0] == 1)
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                else
                    Projectile.rotation = Projectile.velocity.X * 0.05f;

                if (Projectile.frame < 8)
                    Projectile.frame = 8;
                if (Projectile.frameCounter++ >= 3)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame > 11)
                        Projectile.frame = 8;
                }
            }
            else
            {
                if (Projectile.velocity.Y >= -0.1f && Projectile.velocity.Y <= 0.1f)
                {
                    Projectile.rotation = 0;
                    if (Projectile.velocity.X == 0)
                        Projectile.frame = 0;
                    else
                    {
                        Projectile.frameCounter += (int)Math.Abs(Projectile.velocity.X * 0.5f) + 1;
                        if (Projectile.frameCounter >= 6)
                        {
                            Projectile.frameCounter = 0;
                            if (++Projectile.frame > 7)
                                Projectile.frame = 1;
                        }
                    }
                }
                else
                {
                    Projectile.rotation = Projectile.velocity.X * 0.05f;
                    if (Projectile.frame < 8)
                        Projectile.frame = 8;
                    if (Projectile.frameCounter++ >= 3)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame > 11)
                            Projectile.frame = 8;
                    }
                }
            }

            if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
            {
                Projectile.position = player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            BaseAI.AIMinionFighter(Projectile, ref Projectile.ai, player, true, 6, 8, 60, 1000, 2000, 0.1f, 6, 10);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D gravityOverlay = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_GravityOverlay").Value;
            Texture2D lightOverlay = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_LightOverlay").Value;
            Texture2D phazonOverlay = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_PhazonOverlay").Value;
            int height = texture.Height / 12;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 center = new(Projectile.Center.X, Projectile.Center.Y);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Point water = Projectile.Center.ToTileCoordinates();
            if (Main.tile[water.X, water.Y].LiquidType == LiquidID.Water && Main.tile[water.X, water.Y].LiquidAmount > 0)
                Main.EntitySpriteDraw(gravityOverlay, center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            //if (Main.player[Projectile.owner].InModBiome(ModContent.GetInstance<WastelandPurityBiome>())) // TODO: Samus overlay for Soulless Dimension
            //    Main.EntitySpriteDraw(lightOverlay, center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            if (Main.player[Projectile.owner].InModBiome(ModContent.GetInstance<WastelandPurityBiome>()))
                Main.EntitySpriteDraw(phazonOverlay, center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<GonkPetBuff>()))
                Projectile.timeLeft = 2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate == 0)
                Projectile.Kill();
            return false;
        }
    }
}