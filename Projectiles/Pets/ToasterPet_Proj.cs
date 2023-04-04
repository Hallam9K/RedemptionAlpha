using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Pets;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class ToasterPet_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Household Heatray");
            Main.projFrames[Projectile.type] = 9;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, 5, 5)
                .WithOffset(2, 0).WithSpriteDirection(-1);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyDino);
            Projectile.width = 20;
            Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            AIType = ProjectileID.BabyDino;
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.dino = false;

            CheckActive(player);

            if (Projectile.ai[0] == 1)
            {
                if (frameY < 6)
                    frameY = 6;
                if (++frameCounter >= 5)
                {
                    frameCounter = 0;
                    if (++frameY >= 8)
                        frameY = 6;
                }
            }
            else
            {
                if (Projectile.velocity.X > -1 && Projectile.velocity.X < 1)
                {
                    frameY = 0;
                    if (Main.rand.NextBool(1000) && Main.netMode != NetmodeID.Server)
                    {
                        SoundEngine.PlaySound(SoundID.Item16, Projectile.position);
                        Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(2, -12), new Vector2(0, -2),
                            ModContent.Find<ModGore>("Redemption/ToasterPet_Toast").Type);
                    }
                }
                else
                {
                    if (frameY < 1)
                        frameY = 1;

                    if (frameCounter++ >= 4)
                    {
                        frameCounter = 0;
                        if (++frameY >= 5)
                            frameY = 1;
                    }
                }
            }
            if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
            {
                Projectile.position = player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            return true;
        }
        private int frameY;
        private int frameCounter;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 9;
            int y = height * frameY;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - new Vector2(0, 1) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<ToasterPetBuff>()))
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