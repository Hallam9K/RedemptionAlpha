using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.ADD
{
    public class AkkaIsland : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Floating Island");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 776;
            Projectile.height = 335;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.scale = 2;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.GetModPlayer<ScreenPlayer>().Rumble(2, 3);

            if (Projectile.alpha > 0 && Projectile.timeLeft >= 60)
                Projectile.alpha -= 10;
            if (Projectile.timeLeft < 60)
                Projectile.alpha += 10;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(Projectile.width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos + drawOrigin + new Vector2(0, 120), null, color, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center + drawOrigin + new Vector2(0, 120) - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            return false;
        }
    }
    public class AkkaIslandSummoner : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Island Summoner");
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 190;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] == 5)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, player.Center.Y - 1500), new Vector2(0f, 15f), ModContent.ProjectileType<AkkaIslandWarning>(), 0, 0, Projectile.owner, 0, 0);
            }
            if (Projectile.localAI[0] == 120)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, player.Center.Y - 1500), new Vector2(0f, 5f), ModContent.ProjectileType<AkkaIsland>(), 0, 0, Projectile.owner, 0, 0);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, player.Center.Y - 1244), new Vector2(0f, 5f), ModContent.ProjectileType<AkkaIslandHitbox>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 0);
                Projectile.Kill();
            }
        }
    }
    public class AkkaIslandHitbox : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Floating Island");
            ElementID.ProjEarth[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 1448;
            Projectile.height = 312;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player player = Main.player[p];
                if (player.active && !player.dead && Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    player.velocity.Y = Projectile.velocity.Y * 2;
                }
            }
        }
    }
}