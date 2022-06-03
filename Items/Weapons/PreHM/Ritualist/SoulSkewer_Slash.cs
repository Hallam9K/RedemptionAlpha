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
    public class SoulSkewer_Slash : WornDagger_Slash
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Skewer");
            Main.projFrames[Projectile.type] = 5;
        }
    }
}