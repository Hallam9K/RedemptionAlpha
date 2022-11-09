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
            DisplayName.SetDefault("Halm's Casual Jeans");
            Tooltip.SetDefault("'Great for impersonating devs!'");
            SacrificeTotal = 1;
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
    }
}