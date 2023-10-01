using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Bosses.Neb.Phase2
{
    public class NebFalling : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nebuleus, Angel of the Cosmos");
        }
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 62;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            RedeSystem.Silence = true;

            Player player = Main.player[Projectile.owner];
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Enchanted_Pink, 0f, 0f, 100, default, 3f);
            Projectile.localAI[0]++;
            Projectile.velocity.Y += 0.2f;
            Projectile.rotation += 0.01f;
            player.RedemptionScreen().ScreenFocusPosition = Projectile.Center;
            player.RedemptionScreen().lockScreen = true;
            player.RedemptionScreen().cutscene = true;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<NebDefeat>(), 0, 3, Main.myPlayer);
            for (int i = 0; i < 25; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Enchanted_Pink, 0f, 0f, 100, default, 4f);
                Main.dust[dustIndex].velocity *= 1.9f;
            }
        }
    }
}
