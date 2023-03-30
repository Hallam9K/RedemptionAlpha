using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Armor.Vanity.Dev
{
    [AutoloadEquip(EquipType.Legs)]
    public class HallamLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Halm's Casual Jeans");
            // Tooltip.SetDefault("'Great for impersonating devs!'");
            ArmorIDs.Legs.Sets.HidesBottomSkin[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs)] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 14;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(1, 0, 0, 0);
            Item.expert = true;
            Item.vanity = true;
        }
        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            if (male) equipSlot = Redemption.halmMaleLegID;
            if (!male) equipSlot = Redemption.halmFemLegID;
        }
    }
}