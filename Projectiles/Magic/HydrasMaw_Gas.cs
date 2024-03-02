using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class HydrasMaw_Gas : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjWind[Type] = true;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = 3;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 200;
            Projectile.timeLeft = 120;
            Projectile.rotation = RedeHelper.RandomRotation();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HydraAcidDebuff>(), 600);
        public override bool? CanHitNPC(NPC target)
        {
            return !target.friendly && Projectile.penetrate > 1 ? null : false;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, .3f, .6f, .6f);
            Projectile.rotation += Projectile.velocity.Length() / 40 * Projectile.spriteDirection;
            Projectile.velocity *= .92f;

            if (Collision.SolidCollision(Projectile.Center - Vector2.One, 1, 1) || Projectile.penetrate <= 1)
            {
                Projectile.velocity *= .94f;
                Projectile.alpha += 2;
            }

            if (Projectile.timeLeft <= 60)
                Projectile.alpha = (int)MathHelper.Lerp(255f, 200f, Projectile.timeLeft / 60f);
            else if (Projectile.alpha >= 200)
                Projectile.timeLeft = 60;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(new Color(lightColor.R, lightColor.G, lightColor.B, 50)) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(new Color(lightColor.R, lightColor.G, lightColor.B, 50)), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}