using Redemption.Projectiles.Ranged;
using Terraria;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class CrossBomb_Beam : CorruptedDoubleRifle_Beam
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Beam");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 50;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 500;
        }
        public override void OnKill(int timeLeft)
        {
        }
    }
}
