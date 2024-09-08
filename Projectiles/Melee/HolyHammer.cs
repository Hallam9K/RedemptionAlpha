using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class HolyHammer : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ElementID.ProjHoly[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 180;
            Projectile.alpha = 100;
        }
        public override bool? CanCutTiles() => false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(CustomSounds.EarthBoom with { Pitch = -.1f }, Projectile.Center);
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 5;
            target.AddBuff(ModContent.BuffType<HolyFireDebuff>(), 120);
        }
        NPC target;
        public float opacityAlt;
        public bool moved;
        public override void AI()
        {
            Projectile.rotation.SlowRotation(Projectile.velocity.ToRotation() + MathHelper.PiOver4, MathHelper.Pi / 20);

            if (Projectile.ai[0]++ == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center + Projectile.velocity - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0;
                    Color dustColor = new(250, 245, 106) { A = 0 };
                    Main.dust[dust].color = dustColor;
                }
            }
            if (Projectile.timeLeft > 150)
                Projectile.velocity *= 0.95f;
            else
            {
                if (RedeHelper.ClosestNPC(ref target, 900, Projectile.Center, true))
                {
                    Projectile.ai[1]++;
                    Projectile.rotation += Projectile.ai[1] * Projectile.ai[1] * 0.005f * Projectile.direction;
                    if (Projectile.ai[1] > 15)
                    {
                        Projectile.Move(target.Center, 50, 5);
                        moved = true;
                    }
                }
                else
                {
                    Projectile.velocity *= 0.95f;
                    if (!moved)
                        Projectile.position += Vector2.UnitY * MathF.Sin(Projectile.ai[0] / 10);
                }
                Projectile.friendly = true;
            }
            opacityAlt = MathHelper.Lerp(0, 1, Projectile.ai[0] / 60);
            opacityAlt = MathHelper.Min(opacityAlt, 1);
        }
        public override void OnKill(int timeLeft)
        {
            RedeDraw.SpawnExplosion(Projectile.Center * 0.5f + Projectile.Center * 0.5f, Color.LightYellow, scale: 1.25f, noDust: true, rot: Projectile.rotation - MathHelper.PiOver4, shakeAmount: 0, tex: ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow3").Value);
            RedeDraw.SpawnExplosion(Projectile.Center * 0.5f + Projectile.Center * 0.5f, Color.LightYellow, scale: 1.25f, noDust: true, rot: Projectile.rotation + MathHelper.PiOver4, shakeAmount: 0, tex: ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow3").Value);

            for (int i = 0; i < 8; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            Vector2 origin = new(texture.Width() / 2f, texture.Height() / 2f);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Projectile.velocity.SafeNormalize(default) * 25f - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - (Projectile.velocity.SafeNormalize(default) * 25f) + new Vector2(Projectile.width / 2, Projectile.height / 2) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.White with { A = 0 } * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture.Value, drawPos, null, Projectile.GetAlpha(color) * opacityAlt, Projectile.rotation, origin, Projectile.scale * 1.5f, spriteEffects, 0);
            }

            return false;
        }
    }
}