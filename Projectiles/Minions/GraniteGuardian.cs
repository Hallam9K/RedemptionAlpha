using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class GraniteGuardian : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Granite Guardian");
            Main.projFrames[Projectile.type] = 6;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 58;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => false;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.4f, 0.5f, 0.7f);
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.frame = 0;
            }
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
                return;

            Projectile.velocity.Y += 1;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead || Projectile.DistanceSQ(player.Center) > 400 * 400)
                    continue;

                player.AddBuff(ModContent.BuffType<GraniteAuraBuff>(), 4);
            }
            if (Projectile.localAI[0]++ % 40 == 0)
                RedeDraw.SpawnCirclePulse(Projectile.Center, Color.CornflowerBlue, 1.1f, Projectile);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Y > 0)
                Projectile.velocity.Y = 0;
            return false;
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                Projectile.Kill();
                return false;
            }
            return true;
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int height = texture.Height / 6;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - Main.screenPosition, new Rectangle?(rect), lightColor * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), lightColor * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.White * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            return false;
        }
    }
}