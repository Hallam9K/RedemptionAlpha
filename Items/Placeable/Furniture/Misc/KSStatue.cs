using Redemption.Globals;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class KSStatue : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Statue of the Slayer");
            Tooltip.SetDefault("[c/ffea9b:'Ashes to ashes; Dust to dust]" +
                "\n[c/ffea9b:Honor to glory; And iron to rust]" +
                "\n[c/ffea9b:Hate to bloodshed; From rise to fall]" +
                "\n[c/ffea9b:If I never have to die,]" +
                "\n[c/ffea9b:Am I alive at all?']" +
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
            Item.createTile = ModContent.TileType<KSStatueTile>();
        }
    }
}