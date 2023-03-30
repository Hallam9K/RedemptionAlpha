using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class OOMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Obliterator Mask");
            // Tooltip.SetDefault("'Yo.'");
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 18;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }
    }
}