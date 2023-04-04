using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Pets;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class TiedPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tied");
            Main.projFrames[Projectile.type] = 16;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, 8, 5)
                .WithOffset(2, 0).WithSpriteDirection(-1);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyDino);
            Projectile.width = 20;
            Projectile.height = 34;
            AIType = ProjectileID.BabyDino;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.dino = false;

            if (Projectile.ai[0] == 1)
            {
                if (frameY < 8)
                    frameY = 8;
                if (++frameCounter >= 5)
                {
                    frameCounter = 0;
                    if (++frameY >= 15)
                        frameY = 8;
                }
            }
            else
            {
                if (Projectile.velocity.X > -1 && Projectile.velocity.X < 1)
                    frameY = 0;
                else
                {
                    if (frameY < 1)
                        frameY = 1;

                    frameCounter += (int)(Projectile.velocity.X * 0.5f);
                    if (frameCounter >= 2 || frameCounter <= -2)
                    {
                        frameCounter = 0;
                        if (++frameY >= 7)
                            frameY = 1;
                    }
                }
            }
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);
            if (player.ZoneDesert || player.ZoneUndergroundDesert || player.ZoneUnderworldHeight)
            {
                if (Main.rand.NextBool(4))
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water);
                    Main.dust[dustIndex].noGravity = false;
                    Main.dust[dustIndex].velocity *= 0f;
                }
                if (Main.rand.NextBool(900))
                {
                    EmoteBubble.NewBubble(2, new WorldUIAnchor(Projectile), 120);
                }
            }
            if (Main.rand.NextBool(10000))
            {
                switch (Main.rand.Next(7))
                {
                    case 0:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "Boo!", false, false);
                        break;
                    case 1:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "Got a sauna?", false, false);
                        break;
                    case 2:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "spooky scary skeletons~", false, false);
                        break;
                    case 3:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "noniin...", false, false);
                        break;
                    case 4:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "My ladle knows no mercy.", false, false);
                        break;
                    case 5:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "Too dapper for the dagger", false, false);
                        break;
                    case 6:
                        CombatText.NewText(Projectile.getRect(), Color.Green, "ded", false, false);
                        break;
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
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 2) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<TiedPetBuff>()))
                Projectile.timeLeft = 2;
        }
    }
}
