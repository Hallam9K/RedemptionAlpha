using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class NuclearWarhead : ModItem
	{
		public override void SetStaticDefaults()
		{
            /* Tooltip.SetDefault("Right-click the placed warhead to view the side panel" +
                "\nDetonation will create a wasteland\n" +
                "Can only detonate within the outer thirds of the world on the surface, and while no unexplodable tiles are nearby"); */
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<NuclearWarheadTile>(), 0);
			Item.width = 26;
			Item.height = 34;
			Item.maxStack = 1;
			Item.rare = ItemRarityID.Lime;
			Item.value = Item.sellPrice(0, 15, 0, 0);
        }
    }
}