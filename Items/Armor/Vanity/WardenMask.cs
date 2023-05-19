using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class WardenMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Warden's Mask");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 22;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }
    }
}