using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Keeper
{
    public class KeeperBloodWave : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood Wave");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjBlood[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.5f, Projectile.Opacity * 0.2f, Projectile.Opacity * 0.2f);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.alpha += 5;
            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }
        public override bool CanHitPlayer(Player target) => Projectile.alpha < 200;
        public override bool? CanHitNPC(NPC target) => target.friendly && Projectile.alpha < 200 ? null : false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Width);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Bleeding, 260);

            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (host.life < host.lifeMax - 60)
            {
                int steps = (int)host.Distance(target.Center) / 8;
                for (int i = 0; i < steps; i++)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Dust dust = Dust.NewDustDirect(Vector2.Lerp(host.Center, target.Center, (float)i / steps), 2, 2, DustID.LifeDrain, Scale: 2);
                        dust.velocity = target.DirectionTo(dust.position) * 2;
                        dust.noGravity = true;
                    }
                }
                host.life += 60;
                host.HealEffect(60);
            }
        }
    }
}