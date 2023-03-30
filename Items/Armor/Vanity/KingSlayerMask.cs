using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class KingSlayerMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("King Slayer III Mask");
            /* Tooltip.SetDefault("'Was he a slayer of kings or the king of slayers?'"
                + "\n'Or maybe he thought it was a cool name'"); */
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.vanity = true;
            Item.rare = ItemRarityID.Green;
        }
    }
}