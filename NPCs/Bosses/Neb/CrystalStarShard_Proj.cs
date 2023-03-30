using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.Neb
{
    public class CrystalStarShard_Proj : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Star Shard");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjCelestial[Type] = true;
        }

        public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
		public override void AI()
		{
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
		}
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Main.DiscoColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}