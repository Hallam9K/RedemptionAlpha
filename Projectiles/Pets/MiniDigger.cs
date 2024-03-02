using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Pets;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class MiniDigger : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Projectile.type], 5)
                .WithOffset(2, -20f).WithSpriteDirection(-1)
                .WithCode(DelegateMethods.CharacterPreview.Float);
        }
        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 76;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);

            Projectile.position.Y += (float)Math.Sin(Projectile.localAI[0]++ / 15) / 3;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            if (Projectile.velocity.X < -1 || Projectile.velocity.X > 1)
                Projectile.LookByVelocity();

            Vector2 movePos = new(player.Center.X + (60 * -player.direction), player.Center.Y - 50);
            if (Projectile.DistanceSQ(movePos) > 90 * 90)
                Projectile.Move(movePos, 3, 10);
            else
                Projectile.velocity *= .94f;

            Projectile.ai[0]++;
            switch (Projectile.ai[1])
            {
                case 0:
                    if (Projectile.ai[0] >= Projectile.localAI[1])
                        Projectile.alpha += 5;
                    if (Projectile.alpha >= 255)
                    {
                        Projectile.velocity *= 0f;
                        Projectile.position = new Vector2(Main.rand.NextBool(2) ? player.Center.X - 180 : player.Center.X + 180, player.Center.Y - 80);
                        Projectile.ai[1] = 1;
                    }
                    break;
                case 1:
                    Projectile.alpha -= 5;
                    if (Projectile.alpha <= 30)
                    {
                        Projectile.localAI[1] = Main.rand.Next(120, 180);
                        Projectile.ai[0] = 0;
                        Projectile.ai[1] = 0;
                    }
                    break;
            }
            weaponRot = (float)Math.Sin(Projectile.localAI[0] / 15) / 3;
            if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
            {
                Projectile.position = player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }
        private float weaponRot;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D weapon = ModContent.Request<Texture2D>(Texture + "_Weapon").Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 drawOrigin2 = new(weapon.Width / 2 + (4 * Projectile.spriteDirection), weapon.Height / 2 - 28);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(weapon, Projectile.Center + new Vector2(22 * Projectile.spriteDirection, 0) - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + weaponRot, drawOrigin2, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<MiniDiggerBuff>()))
                Projectile.timeLeft = 2;
        }
    }
}