using Redemption.Globals;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class JStatue : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Statue of ???");
            Tooltip.SetDefault("[c/ffea9b:'I've made mistakes that devastated,]" +
                "\n[c/ffea9b:Too many battles lost to tell,]" +
                "\n[c/ffea9b:If I could turn back to time to find you,]" +
                "\n[c/ffea9b:I'd find our confidence as well']" +
                "\n[c/ff0000:Unbreakable (500% Pickaxe Power)]");
        }

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 44;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ModContent.RarityType<LegendaryRarity>();
			Item.consumable = true;
			Item.value = Item.sellPrice(5, 0, 0, 0);
			Item.createTile = ModContent.TileType<JStatueTile>();
        }
    }
}