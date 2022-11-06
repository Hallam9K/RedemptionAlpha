using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Effects.PrimitiveTrails;

namespace Redemption.Projectiles.Magic
{
    public class WaterOrb : ModProjectile, ITrailProjectile
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
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(95, 220, 214), new Color(34, 78, 146)), new RoundCap(), new DefaultTrailPosition(), 100f, 260f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_1", AssetRequestMode.ImmediateLoad).Value, 0.03f, 1f, 1f));
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
            if (tile2 is { HasUnactuatedTile: true } && Main.tileSolid[tile2.TileType])
                Projectile.timeLeft -= 4;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 0);

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item21 with { Volume = 0.5f }, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GemSapphire, 0, 0, Scale: 3);
                Main.dust[dust].velocity *= 2f;
                Main.dust[dust].noGravity = true;
            }
        }           
    }
}