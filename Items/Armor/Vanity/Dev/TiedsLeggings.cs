using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity.Dev
{
    [AutoloadEquip(EquipType.Legs)]
    public class TiedsLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tied's Marvelous Leggings");
            // Tooltip.SetDefault("'Great for impersonating devs!'");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 22;
            Item.rare = ItemRarityID.Cyan;
            Item.vanity = true;
        }
    }
}