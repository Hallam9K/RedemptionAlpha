using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Hostile
{
    public class BloatedClinger_Gas : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Lab/Behemoth/GreenGas_Proj";
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
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            scale = .5f;
            Projectile.alpha = 100;
            Projectile.timeLeft = 120;
            Projectile.rotation = RedeHelper.RandomRotation();
        }
        private float scale;
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<BileDebuff>(), 420);
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, .3f, .6f, .3f);
            Projectile.rotation += Projectile.velocity.Length() / 40 * Projectile.spriteDirection;
            Projectile.velocity *= .92f;

            if (scale < 1)
                scale += .01f;
            if (Collision.SolidCollision(Projectile.Center - Vector2.One, 1, 1) || Projectile.penetrate <= 1)
            {
                Projectile.velocity *= .94f;
                Projectile.alpha += 2;
            }

            if (Projectile.timeLeft <= 60)
                Projectile.alpha = (int)MathHelper.Lerp(255f, 100f, Projectile.timeLeft / 60f);
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
                Color color = new Color(lightColor.R, lightColor.G, lightColor.B, 50) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale * scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(lightColor.R, lightColor.G, lightColor.B, 50) * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale * scale, SpriteEffects.None, 0);
            return false;
        }
    }
}