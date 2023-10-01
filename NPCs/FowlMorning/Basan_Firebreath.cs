using Redemption.Globals;
using Redemption.Projectiles.Magic;
using Terraria;

namespace Redemption.NPCs.FowlMorning
{
    public class Basan_Firebreath : DragonSkullFlames_Proj
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Firebreath");
            ElementID.ProjFire[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = false;
            Projectile.hostile = true;
        }
    }
}
