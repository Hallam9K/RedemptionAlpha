using Microsoft.Xna.Framework;
using Redemption.Items.Usable;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeItem : GlobalItem
    {
        public override void ModifyHitNPC(Item item, Terraria.Player player, Terraria.NPC target, ref int damage,
            ref float knockBack, ref bool crit)
        {
            if (item.axe > 0 && crit)
                damage += damage / 2;
        }
        public override void OnCreate(Item item, ItemCreationContext context)
        {
            if (item.type == ModContent.ItemType<AlignmentTeller>())
                RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Greetings, I am the Chalice of Alignment, and I believe any action can be redeemed.", 180, 30, 0, Color.DarkGoldenrod);
        }
    }
}