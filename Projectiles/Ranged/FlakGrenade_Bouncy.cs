using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class FlakGrenade_Bouncy : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bouncy Flak Grenade");
            ElementID.ProjExplosive[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.ArmorPenetration = 30;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void AI()
        {
            Projectile.LookByVelocity();
            Projectile.rotation += Projectile.velocity.X / 20;
            if (Projectile.localAI[0] < 180)
                Projectile.velocity.Y += 0.2f;

            if (Projectile.localAI[0]++ == 180)
            {
                Projectile.velocity *= 0;
                Projectile.alpha = 255;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 3;

                if (Projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 8; i++)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.SpreadUp(7), ModContent.ProjectileType<FlakGrenade_Frag>(), Projectile.damage / 4, 1, Main.myPlayer, 1);
                }
                for (int i = 0; i < 15; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                    Main.dust[dust].velocity *= 4;
                    Main.dust[dust].noGravity = true;
                }
                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                    Main.dust[dust].velocity *= 8;
                    Main.dust[dust].noGravity = true;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int g = 0; g < 3; g++)
                    {
                        int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                        Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                        Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                    }
                }
                Rectangle boom = new((int)Projectile.Center.X - 80, (int)Projectile.Center.Y - 80, 160, 160);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.active || !target.CanBeChasedBy())
                        continue;

                    if (target.immune[Projectile.whoAmI] > 0 || !target.Hitbox.Intersects(boom))
                        continue;

                    target.immune[Projectile.whoAmI] = 20;
                    int hitDirection = target.RightOfDir(Projectile);
                    BaseAI.DamageNPC(target, Projectile.damage, Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                }
            }
            if (Projectile.localAI[0] == 182)
                Projectile.friendly = false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 1;
            target.immune[Projectile.owner] = 0;

            target.immune[Projectile.whoAmI] = 20;
            if (Projectile.localAI[0] < 180)
                Projectile.localAI[0] = 180;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                if (oldVelocity.X > 4 || oldVelocity.X < -4)
                    SoundEngine.PlaySound(SoundID.Tink, Projectile.position);
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                if (oldVelocity.Y > 4 || oldVelocity.Y < -4)
                    SoundEngine.PlaySound(SoundID.Tink, Projectile.position);
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity *= 0.8f;
            return false;
        }
    }
}