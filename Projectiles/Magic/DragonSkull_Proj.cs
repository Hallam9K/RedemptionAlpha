using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class DragonSkull_Proj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.GetGlobalProjectile<RedeProjectile>().Unparryable = true;
        }

        private bool faceLeft;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (player.channel && Projectile.ai[1] == 0 && Main.myPlayer == Projectile.owner)
            {
                if (Main.MouseWorld.X > Projectile.Center.X)
                {
                    if (faceLeft)
                    {
                        Projectile.rotation -= MathHelper.Pi;
                        faceLeft = false;
                    }
                    Projectile.spriteDirection = 1;
                }
                else
                {
                    if (!faceLeft)
                    {
                        Projectile.rotation += MathHelper.Pi;
                        faceLeft = true;
                    }
                    Projectile.spriteDirection = -1;
                }

                if (Main.myPlayer == Projectile.owner)
                    Projectile.rotation.SlowRotation((Main.MouseWorld - Projectile.Center).ToRotation() + (Projectile.spriteDirection == -1 ? (float)Math.PI : 0), (float)Math.PI / (Projectile.ai[0] >= 180 ? 300 : 80));
                if (Projectile.ai[0]++ == 0)
                {
                    for (int i = 0; i < 20; i++)
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 2);
                }
                if (Projectile.ai[0] == 40)
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, Projectile.position);
                if (Projectile.ai[0] >= 40 && Projectile.ai[0] % 3 == 0 && Projectile.ai[0] <= 180)
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center,
                        RedeHelper.PolarVector(8, Projectile.rotation + (Projectile.spriteDirection == -1 ? (float)Math.PI : 0)), ProjectileID.Flames,
                        Projectile.damage, Projectile.knockBack, Projectile.owner);

                if (Projectile.ai[0] == 180)
                {
                    player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 6;
                    DustHelper.DrawCircle(Projectile.Center, DustID.Torch, 2, 4, 4, 1, 2, nogravity: true);
                    SoundEngine.PlaySound(SoundID.Item122, Projectile.position);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center,
                        RedeHelper.PolarVector(0, Projectile.rotation + (Projectile.spriteDirection == -1 ? (float)Math.PI : 0)), ModContent.ProjectileType<HeatRay>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI);
                }
                if (Projectile.ai[0] >= 380)
                    Projectile.ai[1] = 1;
            }
            else
            {
                Projectile.ai[1] = 1;
                Projectile.alpha += 5;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}