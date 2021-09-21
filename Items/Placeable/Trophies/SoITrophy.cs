using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Trophies;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Trophies
{
    public class SoITrophy : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seed of Infection Trophy");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
			Item.createTile = ModContent.TileType<SoITrophyTile>();
		}
	}
}