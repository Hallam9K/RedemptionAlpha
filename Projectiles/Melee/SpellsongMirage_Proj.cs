using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using ParticleLibrary.Utilities;
using Redemption.Globals;
using Redemption.Particles;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class SpellsongMirage_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spellsong Mirage");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjCelestial[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 140;
            Projectile.alpha = 255;
            Projectile.scale = 0.1f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override bool? CanCutTiles() => false;
        public float rot;
        public override void AI()
        {
            Projectile.width = 76;
            Projectile.height = 76;
            switch (Projectile.localAI[0])
            {
                case 0:
                    Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation() + MathHelper.PiOver4;
                    rot = Projectile.rotation;
                    Projectile.scale += 0.04f;
                    Projectile.alpha -= 30;
                    Projectile.velocity *= 0.9f;
                    if (Projectile.velocity.Length() < 1 && Projectile.alpha <= 0)
                    {
                        Projectile.velocity *= 0;
                        Projectile.localAI[0] = 1;
                    }
                    break;
                case 1:
                    Projectile.rotation = rot;
                    rot.SlowRotation(Projectile.velocity.ToRotation() + MathHelper.PiOver4, (float)Math.PI / 30f);
                    if (Projectile.localAI[1] == 0)
                    {
                        Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 30;
                        Projectile.localAI[1] = 1;
                    }
                    if (Projectile.timeLeft < 40)
                    {
                        Projectile.alpha += 5;
                        if (Projectile.alpha >= 255)
                            Projectile.Kill();
                    }
                    break;
                case 2:
                    Projectile.timeLeft = 10;
                    Projectile.velocity *= 0.86f;
                    Projectile.alpha += 6;
                    if (Projectile.alpha >= 255)
                        Projectile.Kill();
                    break;
            }
            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);
            Projectile.scale = MathHelper.Clamp(Projectile.scale, 0.1f, 1);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localAI[0] = 2;
            Projectile.localNPCImmunity[target.whoAmI] = 10;
            target.immune[Projectile.owner] = 0;

            Vector2 dir = Projectile.Center.DirectionFrom(target.Center);
            Vector2 drawPos = Vector2.Lerp(Projectile.Center, target.Center, 0.9f);
            RedeParticleManager.CreateSlashParticle(drawPos, dir.RotateRandom(.5f) * 100, 1, Color.BlueViolet);
            RedeParticleManager.CreateDevilsPactParticle(drawPos, Vector2.Zero, .75f, Color.BlueViolet.WithAlpha(0), DustID.PurpleCrystalShard);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.localAI[0] == 1 && Projectile.alpha <= 50 ? null : false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(38, 38);
                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}
