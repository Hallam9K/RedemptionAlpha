using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.gtoktas
{
    public class CloudCallerPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, 6, 5)
                .WithOffset(-4, 0).WithSpriteDirection(-1);
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }
        float speed = 4;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, .5f * Projectile.Opacity, .5f * Projectile.Opacity, .5f * Projectile.Opacity);

            Player player = Main.player[Projectile.owner];
            CheckActive(player);

            Projectile.position.X += (float)Math.Sin(Projectile.localAI[0]++ / 15) / 3;
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
            Vector2 movePos = new(player.Center.X + (60 * player.direction), player.Center.Y - 30);
            if (Projectile.DistanceSQ(movePos) > 20 * 20)
            {
                if (Projectile.DistanceSQ(player.Center) > 400 * 400)
                    speed *= 1.03f;
                else if (Projectile.velocity.Length() > 4 && Projectile.DistanceSQ(player.Center) <= 400 * 400)
                    speed *= 0.96f;

                Projectile.Move(movePos, speed, 30);
            }
            else
                Projectile.velocity *= .94f;

            if (speed <= 0)
                speed = 4;
            else if (speed >= 14)
                speed = 14;

            if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
            {
                Projectile.position = player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            Texture2D face = ModContent.Request<Texture2D>(Texture + "_Face").Value;
            int height = texture.Height / 6;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White with { A = 0 }) * .3f, 0, glow.Size() / 2, Projectile.scale * .5f, effects, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(face, Projectile.Center + (Projectile.velocity) - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, face.Size() / 2, Projectile.scale, effects, 0);
            return false;
        }
        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<CloudCallerPetBuff>()))
                Projectile.timeLeft = 2;
            else
                Projectile.Kill();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate == 0)
                Projectile.Kill();
            return false;
        }
    }
}