using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Base;

namespace Redemption.Projectiles.Magic
{
    public class DragonSkull_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dragon Skull");
            ElementID.ProjFire[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 40;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
        }

        private bool faceLeft;
        private float jawRot;
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
                int mana = player.inventory[player.selectedItem].mana;
                if (Main.myPlayer == Projectile.owner)
                {
                    if (Projectile.ai[0] < 40)
                        Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation() + (Projectile.spriteDirection == -1 ? (float)Math.PI : 0);
                    else
                        Projectile.rotation.SlowRotation((Main.MouseWorld - Projectile.Center).ToRotation() + (Projectile.spriteDirection == -1 ? (float)Math.PI : 0), (float)Math.PI / (Projectile.ai[0] >= 180 ? 300 : 80));
                }
                if (Projectile.ai[0]++ == 0)
                {
                    for (int i = 0; i < 20; i++)
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 2);
                }
                if (Projectile.ai[0] >= 20 && Projectile.ai[0] < 30)
                {
                    jawRot += 0.03f;
                }
                if (Projectile.ai[0] == 40)
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, Projectile.position);
                if (Projectile.ai[0] >= 40 && Projectile.ai[0] % 3 == 0 && Projectile.ai[0] <= 180)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + RedeHelper.PolarVector(6, Projectile.rotation + MathHelper.PiOver2), RedeHelper.PolarVector(5, Projectile.rotation + (Projectile.spriteDirection == -1 ? (float)Math.PI : 0)), ModContent.ProjectileType<DragonSkullFlames_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                if (Projectile.ai[0] == 180)
                {
                    if (BasePlayer.ReduceMana(player, mana * 2))
                    {
                        player.RedemptionScreen().ScreenShakeIntensity += 6;
                        DustHelper.DrawCircle(Projectile.Center, DustID.Torch, 2, 4, 4, 1, 2, nogravity: true);
                        SoundEngine.PlaySound(SoundID.Item122, Projectile.position);
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + RedeHelper.PolarVector(6, Projectile.rotation + MathHelper.PiOver2),
                            RedeHelper.PolarVector(0, Projectile.rotation + (Projectile.spriteDirection == -1 ? (float)Math.PI : 0)), ModContent.ProjectileType<HeatRay>(),
                            Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI);
                    }
                    else
                        Projectile.ai[1] = 1;
                }
                if (Projectile.ai[0] >= 380)
                    Projectile.ai[1] = 1;
            }
            else
            {
                Projectile.ai[1] = 1;
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D jawTex = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Jaw").Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(jawTex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation + (jawRot * Projectile.spriteDirection), drawOrigin - new Vector2(Projectile.spriteDirection == -1 ? -12 : 18, 12), Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation - (jawRot * Projectile.spriteDirection), drawOrigin + new Vector2(-13 * Projectile.spriteDirection, 6), Projectile.scale, effects, 0);
            return false;
        }
    }
}