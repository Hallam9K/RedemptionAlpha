using Redemption.Rarities;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Plants
{
    public class ToxicAngel2 : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purified Toxic Angel");
            Tooltip.SetDefault("'Tastes like jelly'");
            SacrificeTotal = 5;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
    }
}
