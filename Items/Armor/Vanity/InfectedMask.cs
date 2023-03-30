using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class InfectedMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("'Become infected... Cosmetically'");
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 12;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }
    }
}