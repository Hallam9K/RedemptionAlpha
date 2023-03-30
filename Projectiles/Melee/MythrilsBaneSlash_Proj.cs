using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class MythrilsBaneSlash_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wind Slice");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjWind[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.scale = 1.5f;
        }
        private float squish;
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
            Projectile.velocity *= 0.9f;
            Projectile.alpha += 8;
            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }

        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.alpha <= 200 ? null : false;
        public override bool? CanCutTiles() => false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 30;
            target.immune[Projectile.owner] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 scale = new(Projectile.scale + squish, Projectile.scale - squish);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(12, 12);
                Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, Projectile.GetAlpha(color) * 0.2f, Projectile.rotation, drawOrigin, scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * 0.5f, Projectile.rotation, drawOrigin, new Vector2(scale.X + 0.2f, scale.Y + 0.2f), effects, 0);
            return false;
        }
    }
}
