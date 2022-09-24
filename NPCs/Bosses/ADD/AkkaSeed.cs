using Microsoft.Xna.Framework;
using Redemption.Base;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class AkkaSeed : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Thorn/ThornSeed";
        private bool spawnDust;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn Seed");
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
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.45f;
            Projectile.velocity.X = 0;
            if (!spawnDust)
            {
                int dustType = DustID.Grass;
                int pieCut = 8;
                for (int m = 0; m < pieCut; m++)
                {
                    int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, dustType, 0f, 0f, 100, Color.White, 1.6f);
                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(6f, 0f), m / (float)pieCut * 6.28f);
                    Main.dust[dustID].noLight = false;
                    Main.dust[dustID].noGravity = true;
                }
                spawnDust = true;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        { 
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-16f, -8f), ModContent.ProjectileType<Akka_CursedThorn>(), Projectile.damage, 1, Main.myPlayer);
            return true;
        }
    }
}