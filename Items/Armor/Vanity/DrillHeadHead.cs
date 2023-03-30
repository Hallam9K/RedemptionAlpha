using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class DrillHeadHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Drill Head Head");
            // Tooltip.SetDefault("'How much drill would a drill head head if a drill head could head drill?'");
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 18;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }
    }
}