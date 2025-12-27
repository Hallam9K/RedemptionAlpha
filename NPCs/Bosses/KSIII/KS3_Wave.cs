using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Utilities;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.NPCs.Bosses.KSIII.Friendly;
using Redemption.Projectiles;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_Wave : ModRedeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("King Slayer III");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjWind[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 192;
            Projectile.height = 88;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 100;
            Projectile.timeLeft = 30;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.White.WithAlpha(0)) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White.WithAlpha(0)), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[2]];
            if (!npc.active || npc.ModNPC is not KS3)
                Projectile.Kill();

            Vector2 HitPos = new(npc.Center.X, npc.Center.Y + 40);
            Projectile.Center = HitPos;
            Projectile.velocity = npc.velocity;
        }
    }
}