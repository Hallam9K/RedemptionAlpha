using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity.Dev
{
    [AutoloadEquip(EquipType.Head)]
    public class TiedsMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tied's Mask");
            // Tooltip.SetDefault("'Great for impersonating devs!'");
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.rare = ItemRarityID.Cyan;
            Item.vanity = true;
        }
    }
}