using Terraria.ModLoader;
using Terraria;
using Redemption.Globals;
using Redemption.Items.Usable;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class SoulSkewer_Slash : WornDagger_Slash
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Soul Skewer");
            Main.projFrames[Projectile.type] = 5;
            ElementID.ProjShadow[Type] = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
            {
                for (int i = 0; i < Main.rand.Next(1, 3); i++)
                {
                    int s = Item.NewItem(Projectile.GetSource_FromAI(), target.getRect(), ModContent.ItemType<RitSpirit>(), 1, false, 0, true);
                    Main.item[s].scale = 0.4f;
                }
            }
        }
    }
}