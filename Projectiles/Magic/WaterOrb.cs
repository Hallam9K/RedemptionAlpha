using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class WaterOrb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.GetGlobalProjectile<RedeGlobalProjectile>().Unparryable = true;
        }

        float rot = 0.02f;
        public override void AI()
        {
            Projectile.ai[1] += rot;
            if (Projectile.ai[1] > (Projectile.localAI[0] == 0 ? 0.10666f : 0.16f))
            {
                Projectile.localAI[0] = 1;
                rot = -0.02f;
            }
            else if (Projectile.ai[1] < -0.16f)
            {
                rot = 0.02f;
            }
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0] == 0 ? Projectile.ai[1] : -Projectile.ai[1]);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

            Point tile = new Vector2(Projectile.Center.X, Projectile.Center.Y).ToTileCoordinates();
            Tile tile2 = Main.tile[tile.X, tile.Y];
            if (tile2 is { IsActiveUnactuated: true } && Main.tileSolid[tile2.type])
                Projectile.timeLeft -= 4;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            lightColor.A = 0;
            return Color.White;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item21.WithVolume(0.5f), Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GemSapphire, 0, 0, Scale: 3);
                Main.dust[dust].velocity *= 2f;
                Main.dust[dust].noGravity = true;
            }
        }           
    }
}