using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Trophies;

namespace Redemption.Items.Placeable.Trophies
{
    public class KeeperTrophy : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Keeper Trophy");
        }
        public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = 100;
			Item.rare = ItemRarityID.Blue;
			Item.createTile = ModContent.TileType<KeeperTrophyTile>();
		}
	}
}