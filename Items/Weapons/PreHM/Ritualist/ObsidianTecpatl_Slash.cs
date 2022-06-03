using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;
using Terraria.DataStructures;
using Redemption.DamageClasses;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class ObsidianTecpatl_Slash : WornDagger_Slash
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Ritualist/SoulSkewer_Slash";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Obsidian Tecpatl");
            Main.projFrames[Projectile.type] = 5;
        }
    }
}