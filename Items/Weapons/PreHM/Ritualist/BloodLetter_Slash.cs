using Redemption.Globals;
using Terraria;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class BloodLetter_Slash : WornDagger_Slash
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood Letter");
            Main.projFrames[Projectile.type] = 5;
            ElementID.ProjBlood[Type] = true;
        }
    }
}