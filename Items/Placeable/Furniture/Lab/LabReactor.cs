using Redemption.Tiles.Furniture.Lab;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabReactor : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Reactor");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LabReactorTile>(), 0);
			Item.width = 48;
			Item.height = 26;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 10000;
			Item.rare = ItemRarityID.LightPurple;
		}
    }
}