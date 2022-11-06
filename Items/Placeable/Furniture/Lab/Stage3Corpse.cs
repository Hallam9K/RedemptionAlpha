using Redemption.Tiles.Furniture.Lab;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class Stage3Corpse : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystallized Corpse");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Stage3CorpseTile>(), 0);
            Item.width = 32;
            Item.height = 34;
            Item.maxStack = 9999;
            Item.value = 500;
            Item.rare = ItemRarityID.Green;
        }
    }
}