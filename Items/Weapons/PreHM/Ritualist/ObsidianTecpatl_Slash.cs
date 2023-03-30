using Terraria;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class ObsidianTecpatl_Slash : WornDagger_Slash
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Ritualist/SoulSkewer_Slash";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Obsidian Tecpatl");
            Main.projFrames[Projectile.type] = 5;
        }
    }
}