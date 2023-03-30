using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.BaseExtension;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class ThornSeed : ModProjectile
    {
        private bool spawnDust;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Thorn Seed");
        }
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.alpha = 0;
            Projectile.Redemption().ParryBlacklist = true;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.Center.Y < player.Center.Y)
                fallThrough = true;
            else
                fallThrough = false;
            return true;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.45f;
            Projectile.velocity.X = 0;
            if (!spawnDust)
            {
                for (int m = 0; m < 8; m++)
                {
                    int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, DustID.Grass, 0f, 0f, 100, Color.White, 1.6f);
                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(6f, 0f), m / (float)8 * 6.28f);
                    Main.dust[dustID].noLight = false;
                    Main.dust[dustID].noGravity = true;
                }
                spawnDust = true;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player player = Main.player[Projectile.owner];
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, Projectile.Center.Y - 36), Vector2.Zero, ModContent.ProjectileType<ThornTrap>(), Projectile.damage, 3, Projectile.owner);
            }
            return true;
        }
    }
}