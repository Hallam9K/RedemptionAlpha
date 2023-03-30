using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class ThornMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("'Looks painful...'");
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.vanity = true;
            Item.rare = ItemRarityID.Green;
        }
    }
}