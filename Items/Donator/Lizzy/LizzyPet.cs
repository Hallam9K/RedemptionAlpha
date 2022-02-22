using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Biomes;
using Redemption.Globals;
using Redemption.NPCs.Friendly;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Lizzy
{
    public class LizzyPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lizzy");
            Main.projFrames[Projectile.type] = 11;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
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

            int KS3ID = NPC.FindFirstNPC(ModContent.NPCType<KS3Sitting>());
            if (KS3ID >= 0 && player.DistanceSQ(Main.npc[KS3ID].Center) < 300 * 300)
            {
                Projectile.tileCollide = false;
                NPC npc = Main.npc[KS3ID];
                Vector2 sleepPos = new(npc.Center.X, npc.position.Y - 14);
                Projectile.Move(sleepPos, 8, 1);
                if (Projectile.Center == sleepPos)
                {
                    if (Projectile.localAI[0]++ % 600 == 0)
                        EmoteBubble.NewBubble(89, new WorldUIAnchor(Projectile), 180);
                    Projectile.spriteDirection = -1;
                    Projectile.rotation = 0;
                    Projectile.velocity *= 0;
                    Projectile.frame = 8;
                }
                else
                {
                    Projectile.localAI[0] = 0;
                    Projectile.rotation = Projectile.velocity.X * 0.05f;
                    if (Projectile.frame < 9)
                        Projectile.frame = 9;
                    if (Projectile.frameCounter++ >= 5)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame > 10)
                            Projectile.frame = 9;
                    }
                }
                return;
            }
            if (Projectile.ai[0] != 0 && Projectile.ai[0] == 1)
            {
                if (Projectile.ai[0] == 1)
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                else
                    Projectile.rotation = Projectile.velocity.X * 0.05f;

                if (Projectile.frame < 9)
                    Projectile.frame = 9;
                if (Projectile.frameCounter++ >= 5)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame > 10)
                        Projectile.frame = 9;
                }
            }
            else
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
                        if (++Projectile.frame >= 7)
                            Projectile.frame = 1;
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
            Texture2D maskOverlay = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_MaskOverlay").Value;
            Texture2D swordOverlay = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_SwordOverlay").Value;
            Texture2D xmasOverlay = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_XmasOverlay").Value;
            int height = texture.Height / 11;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Rectangle rectSword = new(0, y, swordOverlay.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 drawOriginSword = new(swordOverlay.Width / 2, Projectile.height / 2);
            Vector2 center = new(Projectile.Center.X, Projectile.Center.Y - 18);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (RedeHelper.BossActive())
                Main.EntitySpriteDraw(swordOverlay, center - Main.screenPosition, new Rectangle?(rectSword), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOriginSword, Projectile.scale, effects, 0);

            Main.EntitySpriteDraw(texture, center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            if (Main.xMas)
                Main.EntitySpriteDraw(xmasOverlay, center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            if (Main.player[Projectile.owner].InModBiome(ModContent.GetInstance<WastelandPurityBiome>()))
                Main.EntitySpriteDraw(maskOverlay, center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<LizzyPetBuff>()))
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