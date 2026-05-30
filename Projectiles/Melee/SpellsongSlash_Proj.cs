using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Particles;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class SpellsongSlash_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Arcane Wave");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjArcane[Type] = true;
            ElementID.ProjCelestial[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.scale = 1.5f;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = 6;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        private float squish;
        public float[] oldrot = new float[5];
        public Vector2[] oldPos = new Vector2[5];
        public override void AI()
        {
            Projectile.LookByVelocity();
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            if (Projectile.ai[0] == 1)
                squish -= 0.02f;
            else
                squish += 0.02f;

            Projectile.velocity *= 0.91f;
            Projectile.alpha += 8;
            if (Projectile.alpha >= 255)
                Projectile.Kill();

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                oldPos[k] = oldPos[k - 1];
                oldrot[k] = oldrot[k - 1];
            }
            oldPos[0] = Projectile.Center;
            oldrot[0] = Projectile.rotation;
        }
        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.alpha <= 200 ? null : false;
        public override bool? CanCutTiles() => false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 drawPos = Main.rand.NextVector2FromRectangle(target.Hitbox);
            Vector2 randVel = Projectile.velocity.SafeNormalize(default).RotateRandom(0.5f);
            RedeParticleManager.CreateSlashParticle(drawPos, randVel.RotatedBy(MathF.PI * 0 / 3) * 20, 1.5f, Color.BlueViolet, 12);
            RedeParticleManager.CreateSlashParticle(drawPos, randVel.RotatedBy(MathF.PI * 1 / 4) * 10, 1f, Color.BlueViolet, 12);
            RedeParticleManager.CreateSlashParticle(drawPos, randVel.RotatedBy(MathF.PI * 3 / 4) * 10, 1f, Color.BlueViolet, 12);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 scale = new Vector2(1 + squish, 1 - squish) * Projectile.scale;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = oldPos[k] - Main.screenPosition;
                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, Projectile.GetAlpha(color), oldrot[k], drawOrigin, scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White) * 0.5f, Projectile.rotation, drawOrigin, new Vector2(scale.X + 0.2f, scale.Y + 0.2f), effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}
